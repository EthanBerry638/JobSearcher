using Helpers;
using JobFetcherManager;
using JobListingManager;

namespace Menus
{
    public class MenuManager
    {
        private readonly IJobProvider _provider;

        public MenuManager(IJobProvider provider)
        {
            _provider = provider;
        }

        public async Task LoopAsync()
        {
            Console.Clear();
            Console.WriteLine("Fetching jobs...\n");
            Helper.Pause(1000);

            for (int page = 1; page < 2; page++)
            {
                var jobs = await _provider.GetJobsAsync(page);
                var filteredJobs = Helper.ApplyFilter(jobs);

                DisplayJobs(filteredJobs);

                await Task.Delay(2000);
            }
        }

        private void DisplayJobs(List<JobListing> jobs)
        {
            if (jobs == null || jobs.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️ No jobs found. Try adjusting your search criteria.\n");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✅ Found {jobs.Count} jobs:\n");
            Helper.Pause(1000);
            Console.ResetColor();

            foreach (var job in jobs)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"🔹 {job.Title} at {job.Company} ({job.Location})");
                Console.WriteLine($"📅 Posted: {job.PostedDate.ToShortDateString()}");
                Console.WriteLine($"🔗 Apply: {job.Url}");
                Console.WriteLine(new string('-', 40));
                Console.ResetColor();
            }
        }
    }
}