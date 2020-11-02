using System;

namespace WebServer.Application.Abstractions.Domain
{
    public interface IEventEntity
    {
        Guid Id { get; set; }
    }
}
