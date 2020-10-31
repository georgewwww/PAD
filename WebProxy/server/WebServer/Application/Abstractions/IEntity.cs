using System;

namespace WebServer.Application.Abstractions
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
