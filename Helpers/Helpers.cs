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

    }
}