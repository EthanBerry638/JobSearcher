using JobListingManager;

namespace Helpers
{
    public static class Helper
    {
        public static void Pause(int ms) => Thread.Sleep(ms);
        public static List<JobListing> ApplyFilter(List<JobListing> jobs)
        {
            return jobs
                .Where(job =>
                    !string.IsNullOrWhiteSpace(job.Title) &&
                    !job.Title.ToLowerInvariant().Contains("senior") &&
                    !job.Description!.ToLowerInvariant().Contains("remote") &&
                    !job.Location!.ToLowerInvariant().Contains("remote"))
                .ToList();
        }
        public static bool IsRelevant(string title, string description)
        {
            var keywords = new[] { "apprentice", "junior", "trainee", "graduate", "entry level" };
            var techTerms = new[] { "software", "developer", "engineer", "c#", ".net"};

            string content = (title + " " + description).ToLowerInvariant();
            return keywords.Any(k => content.Contains(k)) && techTerms.Any(t => content.Contains(t));
        }
    }
}