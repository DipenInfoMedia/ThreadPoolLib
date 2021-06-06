using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPoolLib
{
    public class TPManager
    {
        private readonly List<Task> AllTasks = new List<Task>();
        private readonly ConcurrentBag<Tuple<int, DateTime>> OrderExecuted = new ConcurrentBag<Tuple<int, DateTime>>();

        // Resource with the tasks that will call it
        // <resourceName, associatedTasks>
        private readonly Dictionary<string, List<Task>> ResourcePool = new Dictionary<string, List<Task>>();

        public void AddMultipleResourcea(string[] resourceNamea)
        {
            foreach (string resourceName in resourceNamea)
            {
                AddResource(resourceName);
            }
        }

        public void AddResource(string resourceName)
        {
            ResourcePool.Add(resourceName, new List<Task>());
        }

        public List<int> GetTasksExecutionOrder()
        {
            SortedList<DateTime, int> executionOrder = new SortedList<DateTime, int>();

            foreach (var taskDetails in OrderExecuted)
            {
                executionOrder.Add(taskDetails.Item2, taskDetails.Item1);
            }
            return executionOrder.Values.ToList();
        }

        /// <summary>
        /// The task will have to be executed on FIFO basis when the same resource is required. As per the requirement in the document.
        /// Random order is possible with simple use of semaphores for blocking on non shared resources
        /// If Task 3 and Task 5 are waiting for Task 2, then Task 3 gets to start first once Task 2 completes (Task 5 has to wait for Task 2)
        /// </summary>
        /// <param name="resourcesRequired"></param>
        public void QueueTask(int taskid, params string[] resourcesRequired)
        {
            // Get all the tasks that are already queued for these resources
            List<Task> tasksToWaitFor = new List<Task>();
            resourcesRequired.ToList().ForEach(r => tasksToWaitFor.AddRange(ResourcePool[r]));

            // create a new task that will wait for other tasks
            Task t = CreateTask(taskid, tasksToWaitFor);

            AllTasks.Add(t);

            // Add this task as an associated task for all the resources
            resourcesRequired.ToList().ForEach(r => ResourcePool[r].Add(t));
        }

        public void RunAllTasks()
        {
            AllTasks.ForEach(t => t.Start());
            Task.WaitAll(AllTasks.ToArray());
        }

        private Task CreateTask(int taskId, List<Task> tasksToWaitFor)
        {
            return new Task(() =>
            {
                Debug.WriteLine($"Start Task : {taskId} at {DateTime.Now}");

                // Wait for other tasks to complete
                if (tasksToWaitFor.Count > 0)
                {
                    Task.WaitAll(tasksToWaitFor.ToArray());
                }

                // The task just sleeps for random time
                // get random wait time upto 1.5 secs
                Random random = new Random();
                int sleepTime = random.Next(1, 3) * 500;

                //Task.Delay(sleepTime).Wait();
                Thread.Sleep(sleepTime);

                // update the order executed
                OrderExecuted.Add(new Tuple<int, DateTime>(taskId, DateTime.Now));

                Debug.WriteLine($"End Task : {taskId} at {DateTime.Now}");
            });
        }
    }
}