namespace Broker.Infrastructure.Persistence
{
    public class ConnectionDatabaseSettings : IConnectionDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionsCollectionName { get; set; }
        public string MessagesCollectionName { get; set; }
    }
}
