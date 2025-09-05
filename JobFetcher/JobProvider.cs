using System.Text;
using System.Text.Json;
using JobListingManager;
using ApiUsage;
using Helpers;

namespace JobFetcherManager
{
    public interface IJobProvider
    {
        Task<List<JobListing>> GetJobsAsync(int pageNumber);
    }

    public class AdzunaJobProvider : IJobProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string AppId = "8e952900";
        private const string AppKey = "a8531e4972606137d931f6222e1a4392";
        private const int ResultsPerPage = 50;

        public async Task<List<JobListing>> GetJobsAsync(int pageNumber = 1)
        {
            string url = $"https://api.adzuna.com/v1/api/jobs/gb/search/{pageNumber}?app_id={AppId}&app_key={AppKey}&results_per_page=5&what=apprentice&where=Leeds";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to fetch adzuna jobs, error: {response.StatusCode}");
                return new List<JobListing>();
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();

            var charset = response.Content.Headers.ContentType?.CharSet?.ToLowerInvariant();
            charset = charset switch
            {
                "utf8" => "utf-8",
                null => "utf-8",
                _ => charset
            };


            Encoding encoding = Encoding.GetEncoding(charset);

            string decoded = string.Empty;

            try
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                decoded = encoding.GetString(content);

                var parsed = JsonSerializer.Deserialize<AdzunaResponse>(decoded);

                var jobs = parsed?.Results?.Select(job => new JobListing
                {
                    Title = job.Title,
                    Company = job.Company?.DisplayName ?? "Unknown",
                    Location = job.Location?.DisplayName ?? "Unknown",
                    Description = job.Description,
                    Url = job.RedirectUrl,
                    PostedDate = DateTime.TryParse(job.Created, out var date) ? date : DateTime.MinValue,
                    Source = "Adzuna"
                }).ToList() ?? new List<JobListing>();

                if (jobs.Count > 0)
                {
                    ApiUsageTracker.Increment("adzuna", "searchjobs");
                }

                return jobs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                Console.WriteLine(decoded.Length > 500 ? decoded.Substring(0, 500) + "..." : decoded);
                return new List<JobListing>();
            }
        }
    }

    public class RemotiveJobProvider : IJobProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<JobListing>> GetJobsAsync(int pageNumber = 1)
        {
            string url = "https://remotive.io/api/remote-jobs?search=apprentice";

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("JobFetcherManager/1.0");
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"\nTried to fetch remotive jobs, error: {response.StatusCode}");
                return new List<JobListing>();
            }

            try
            {
                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonSerializer.Deserialize<RemotiveResponse>(json);

                var jobs = parsed?.Jobs?.Select(job => new JobListing
                {
                    Title = job.Title,
                    Company = job.CompanyName ?? "Unknown",
                    Location = job.CandidateRequiredLocation ?? "Unknown",
                    Description = job.Description,
                    Url = job.Url,
                    PostedDate = DateTime.TryParse(job.PublicationDate, out var date) ? date : DateTime.MinValue,
                    Source = "Remotive"
                }).ToList() ?? new List<JobListing>();

                if (jobs.Count > 0)
                {
                    ApiUsageTracker.Increment("remotive", "searchjobs");
                }

                return jobs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return new List<JobListing>();
            }
        }
    }

    public class JoobleProvider : IJobProvider
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string ApiKey = "05a24876-e42b-4c9f-9119-5873321333e7";
        private const string Endpoint = $"https://jooble.org/api/{ApiKey}";

        public async Task<List<JobListing>> GetJobsAsync(int pageNumber = 1)
        {
            var requestBody = new
            {
                keywords = "c# .net apprenticeship leeds",
                location = "Leeds, England",
                radius = "20", //km 
                page = pageNumber.ToString(),
                companysearch = "false"
            };

            var response = await _httpClient.PostAsJsonAsync(Endpoint, requestBody);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"\nTried to fetch Jooble jobs, error: {response.StatusCode}");
                return new List<JobListing>();
            }

            try
            {
                var json = await response.Content.ReadAsStringAsync();
                var parsed = JsonSerializer.Deserialize<JoobleResponse>(json);

                var jobs = parsed?.Jobs?.Select(job => new JobListing
                {
                    Title = job.Title,
                    Company = job.Company ?? "Unknown",
                    Location = job.Location ?? "Unknown",
                    Description = job.Snippet,
                    Url = job.Link,
                    PostedDate = job.Updated,
                    Source = "Jooble"
                }).ToList() ?? new List<JobListing>();

                if (jobs.Count > 0)
                {
                    ApiUsageTracker.Increment("jooble", "searchjobs");
                }

                var filteredJobs = jobs.Where(job => Helper.IsRelevant(job.Title!, job.Description!)).ToList();

                if (filteredJobs.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n⚠️ No relevant jobs found from Jooble.");
                    Console.ResetColor();
                    return new List<JobListing>();
                }
 
                Console.WriteLine($"✅ Filtered {jobs.Count} → {filteredJobs.Count} relevant jobs from Jooble");

                return filteredJobs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return new List<JobListing>();
            }
        }
    }
}