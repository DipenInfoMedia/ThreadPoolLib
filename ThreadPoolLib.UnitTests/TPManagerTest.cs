using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ThreadPoolLib.UnitTests
{
    [TestClass]
    public class TPManagerTest
    {
        [TestMethod]
        public void Test3Tasks()
        {
            // Arrange
            TPManager tpManager = new TPManager();
            tpManager.AddMultipleResourcea(new string[] { "A", "B", "C", "D" });

            // Task 1
            tpManager.QueueTask(1, "A");
            // Task 2
            tpManager.QueueTask(2, "B");
            // Task 3
            tpManager.QueueTask(3, "A", "B");

            // Act
            tpManager.RunAllTasks();

            // Just see the order
            foreach (var taskId in tpManager.GetTasksExecutionOrder())
            {
                Debug.WriteLine(taskId);
            }

            // Assert
            // Can only check if the 3rd task was "3"
            Assert.IsTrue(tpManager.GetTasksExecutionOrder()[2] == 3);
        }

        [TestMethod]
        public void Test5Tasks()
        {
            // Arrange
            TPManager tpManager = new TPManager();
            tpManager.AddMultipleResourcea(new string[] { "A", "B", "C", "D" });

            // Task 1
            tpManager.QueueTask(1, "A");
            // Task 2
            tpManager.QueueTask(2, "B");
            // Task 3
            tpManager.QueueTask(3, "A", "B");
            // Task 4
            tpManager.QueueTask(4, "C");
            // Task 5
            tpManager.QueueTask(5, "B");

            // Act
            tpManager.RunAllTasks();

            // Just see the order
            foreach (var taskId in tpManager.GetTasksExecutionOrder())
            {
                Debug.WriteLine(taskId);
            }

            // Assert
            // Can only check if the 4th task was "3" and 5th task executed was "5"
            Assert.IsTrue(tpManager.GetTasksExecutionOrder()[3] == 3);
            Assert.IsTrue(tpManager.GetTasksExecutionOrder()[4] == 5);
        }
    }
}