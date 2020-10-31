using WebServer.Application.Abstractions;

namespace WebServer.Infrastructure.Persistence
{
    public class ConnectionDatabaseSettings : IConnectionDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ActorsCollectionName { get; set; }
        public string MoviesCollectionName { get; set; }
    }
}
