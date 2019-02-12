using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using message_types.commands;
using message_types.events;
using MassTransit;
using Notifications.Helpers;
using Notifications.Models;

// https://docs.microsoft.com/en-us/uwp/api/windows.ui.notifications.toastnotificationmanager.createtoastnotifier

namespace Notifications.consumers
{
    public class DatabaseRegisteredConsumer : IConsumer<DatabaseRegistered>
    {
        private const string AppId = "RedgateNotifications.App";

        public DatabaseRegisteredConsumer()
        {
            Models.DatabaseRegisteredConsumer.ShortCutCreator.TryCreateShortcut("RedgateNotifications.App", "RedgateNotifications");       
        }

        [MTAThread]
        public Task Consume(ConsumeContext<DatabaseRegistered> context)
        {
            var dbDetails = context.Message;

            var notification = new RedgateNotifiation
            {
                Title = "Database Registry",
                Message = $"{dbDetails.Database.Server}-{dbDetails.Database.Database} has been registered",
                ImagePath = Path.GetFullPath("img/search.png")
            };

            CommonHelper.SaveNotification(notification);

            CommonHelper.ShowToast(AppId, notification);

            return Task.CompletedTask;
        }
       
    }
}