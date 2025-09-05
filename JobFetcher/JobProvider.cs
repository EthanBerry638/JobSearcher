using System.Text;
using System.Text.Json;
using JobListingManager;
using ApiUsage;

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
        private const int ResultsPerPage = 20;

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

            if (charset == "utf8") charset = "utf-8";

            Encoding encoding;
            try
            {
                encoding = Encoding.GetEncoding(charset ?? "utf-8");
            }
            catch
            {
                Console.WriteLine($"Unsupported charset '{charset}', falling back to UTF-8.");
                encoding = Encoding.UTF8;
            }

            var content = encoding.GetString(bytes);

            try
            {
                var parsed = JsonSerializer.Deserialize<AdzunaResponse>(content);

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
                Console.WriteLine(content);
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
                Console.WriteLine($"Tried to fetch remotive jobs, error: {response.StatusCode}");
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

                return jobs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return new List<JobListing>();
            }
        }
    }
}