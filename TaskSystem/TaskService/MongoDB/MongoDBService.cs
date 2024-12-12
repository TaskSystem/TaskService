using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using TaskService.Model;

namespace TaskService.MongoDB
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;

        public MongoDBService()
        {
            try
            {
                // Stel de GuidRepresentatie in voor de gehele applicatie
                BsonSerializer.RegisterSerializer(
                    typeof(Guid),
                    new GuidSerializer(GuidRepresentation.Standard) // Standaard Guid serialisatie
                );

                // Verbind met de MongoDB-database
                var client = new MongoClient("mongodb+srv://zaka013:mjYz6GKYJpnDgeFv@taskservicecluster.eqrzp.mongodb.net/?retryWrites=true&w=majority&appName=TaskServiceCluster");
                _database = client.GetDatabase("TaskServiceCluster");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fout bij het verbinden met MongoDB: " + ex.Message);
            }
        }

        // Verkrijg de Task-collectie
        public IMongoCollection<TaskModel> GetTasksCollection()
        {
            return _database.GetCollection<TaskModel>("tasks");
        }

        // Verkrijg de Comment-collectie
        public IMongoCollection<Comment> GetCommentsCollection()
        {
            return _database.GetCollection<Comment>("comments");
        }
    }
}
