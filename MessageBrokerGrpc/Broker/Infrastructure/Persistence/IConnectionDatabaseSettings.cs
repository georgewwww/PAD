namespace Broker.Infrastructure.Persistence
{
    public interface IConnectionDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string ConnectionsCollectionName { get; set; }
        string MessagesCollectionName { get; set; }
    }
}
