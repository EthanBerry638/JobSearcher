using Helpers;
using JobFetcherManager;
using JobListingManager;

namespace Menus
{
    public class MenuManager
    {
        private readonly List<IJobProvider> _providers;

        public MenuManager(List<IJobProvider> providers)
        {
            _providers = providers;
        }

        public async Task LoopAsync()
        {
            Console.Clear();
            Console.WriteLine("Fetching jobs...");
            Helper.Pause(1000);

            int maxPages = 5;

            foreach (var provider in _providers)
            {
                int pageNumber = 1;

                while (pageNumber <= maxPages)
                {
                    var jobs = await provider.GetJobsAsync(pageNumber);
                    if (jobs == null || jobs.Count == 0)
                        break;

                    var filteredJobs = Helper.ApplyFilter(jobs);
                    DisplayJobs(filteredJobs);

                    pageNumber++;
                    await Task.Delay(2000);
                }
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
                Console.WriteLine($"🔹 {job.Title} at {job.Company} ({job.Location}) [{job.Source}]");
                Console.WriteLine($"📅 Posted: {job.PostedDate.ToShortDateString()}");
                Console.WriteLine($"🔗 Apply: {job.Url}");
                Console.WriteLine(new string('-', 40));
                Console.ResetColor();
            }
        }
    }
}