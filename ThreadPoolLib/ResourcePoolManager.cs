using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ThreadPoolLib
{
    public class ResourcePoolManager
    {
        private readonly IDictionary<string, Semaphore> resourcePools = new Dictionary<string, Semaphore>();

        public void AddResourcePool(string resourceName, int maxConcurrentConsumers)
        {
            this.resourcePools.Add(resourceName, new Semaphore(maxConcurrentConsumers, maxConcurrentConsumers));
        }

        public void ReleaseResource(string resourceName)
        {
            this.resourcePools[resourceName].Release(1);
        }

        public void RequestMultipleResources(string[] resourceNames)
        {
            Semaphore[] resources = resourceNames.Select(s => this.resourcePools[s]).ToArray();

            WaitHandle.WaitAll(resources);
        }

        public void RequestResource(string resourceName)
        {
            this.resourcePools[resourceName].WaitOne();
        }
    }
}