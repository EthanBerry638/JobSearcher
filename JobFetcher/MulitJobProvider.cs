/* 

************* NOT USING FOR NOW **************

using JobFetcherManager;
using JobListingManager;

namespace Providers
{
    public class MultiJobProvider : IJobProvider
    {
        private readonly List<IJobProvider> _providers;

        public MultiJobProvider(params IJobProvider[] providers)
        {
            _providers = providers.ToList();
        }

        public async Task<List<JobListing>> GetJobsAsync(int pageNumber)
        {
            var allJobs = new List<JobListing>();

            foreach (var provider in _providers)
            {
                var jobs = await provider.GetJobsAsync(pageNumber);
                allJobs.AddRange(jobs);
            }

            return allJobs;
        }
    }
} */ 