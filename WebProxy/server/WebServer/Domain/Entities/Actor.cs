using System;
using WebServer.Application.Abstractions;

namespace WebServer.Domain.Entities
{
    public class Actor : IEntity
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ImageLink { get; set; }
        public int BirthYear { get; set; }
        public string BirthDate { get; set; }
        public string Description { get; set; }
    }
}
