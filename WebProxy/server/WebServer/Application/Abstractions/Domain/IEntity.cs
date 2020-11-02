using System;

namespace WebServer.Application.Abstractions.Domain
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
