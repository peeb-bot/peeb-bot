using System;
using Peeb.Bot.Models;

namespace Peeb.Bot.UnitTests
{
    public class StubEntity : Entity
    {
        public Guid Id { get; private set; }
        public string State { get; private set; }

        public StubEntity()
        {
            Id = Guid.NewGuid();
            State = "Added";

            Publish(new StubEntityAddedNotification());
        }

        public void Modify()
        {
            State = "Modified";

            Publish(new StubEntityModifiedNotification());
        }

        public void Delete()
        {
            State = "Deleted";

            Publish(new StubEntityDeletedNotification());
        }
    }
}
