using Moq;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using TaskService.Repository;
using TaskService.Model;
using TaskService.MongoDB;
using MongoDB.Driver.Linq;

namespace TaskserviceTests
{
    public class TaskRepositoryTests
    {
        [Fact]
        public async Task GetAllTasks_ReturnsTasks()
        {
            // Arrange
            var mockMongoDBService = new Mock<MongoDBService>();
            var mockCollection = new Mock<IMongoCollection<TaskModel>>();

            // Simuleren van een MongoDB-respons
            var tasks = new List<TaskModel>
            {
                new TaskModel { Id = Guid.NewGuid(), Title = "Task 1", Description = "First task" },
                new TaskModel { Id = Guid.NewGuid(), Title = "Task 2", Description = "Second task" }
            };

            mockCollection.Setup(x => x.Find(It.IsAny<FilterDefinition<TaskModel>>(), It.IsAny<FindOptions<TaskModel>>(), CancellationToken.None))
                          .ReturnsAsync(tasks);

            mockMongoDBService.Setup(x => x.GetTasksCollection()).Returns(mockCollection.Object);

            var taskRepository = new TaskRepository(mockMongoDBService.Object);

            // Act
            var result = await taskRepository.GetAllTasks();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Task 1", result.First().Title);
        }
    }
}
