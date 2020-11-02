using WebServer.Application.Abstractions.Domain;
using WebServer.Domain.Events;

namespace WebServer.Application.Abstractions
{
    public interface IEventSynchronizer<T, TEvent> where T : IEntity where TEvent : IEventEntity
    {
        string InsertQueue { get; }
        string UpdateQueue { get; }
        string DeleteQueue { get; }

        void OnInsertEvent(EntityInsertEvent<TEvent> @event);
        void OnUpdateEvent(EntityUpdateEvent<TEvent> @event);
        void OnDeleteEvent(EntityDeleteEvent @event);
    }
}
