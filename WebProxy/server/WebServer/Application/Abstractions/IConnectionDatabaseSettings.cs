namespace WebServer.Application.Abstractions
{
    public interface IConnectionDatabaseSettings
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string ActorsCollectionName { get; set; }
        string MoviesCollectionName { get; set; }
    }
}
