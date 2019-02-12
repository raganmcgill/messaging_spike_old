using System;
using System.IO;
using System.Threading.Tasks;
using message_types.events;
using MassTransit;
using Notifications.Helpers;
using Notifications.Models;

namespace Notifications.consumers
{
    public class DatabaseUpdatedConsumer : IConsumer<SchemaChanged>
    {
        private const string AppId = "RedgateNotifications.App";

        public DatabaseUpdatedConsumer()
        {
            Models.DatabaseRegisteredConsumer.ShortCutCreator.TryCreateShortcut("RedgateNotifications.App", "RedgateNotifications");
        }

        [MTAThread]
        public Task Consume(ConsumeContext<SchemaChanged> context)
        {
            var dbDetails = context.Message;
            var connectionDetails = dbDetails.Database.ConnectionDetails;
            var notification = new RedgateNotifiation
            {
                Title = "Schema change",
                Message = $"{connectionDetails.Database} now has {dbDetails.Database.Tables.Count} tables",
                ImagePath = Path.GetFullPath("img/search.png")
            };

            CommonHelper.SaveNotification(notification);

            CommonHelper.ShowToast(AppId, notification);

            return Task.CompletedTask;
        }
    }
}