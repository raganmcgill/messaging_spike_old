using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using message_types.commands;
using message_types.events;
using MassTransit;
using Newtonsoft.Json;
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

            SaveNotification(notification);

            ShowToast(AppId, notification);

            return Task.CompletedTask;
        }

        private void SaveNotification(RedgateNotifiation notification)
        {
            {
                var subPath = $@"C:\dev\Stores\notifications";

                if (!Directory.Exists(subPath))
                {
                    Directory.CreateDirectory(subPath);
                }

                var filename = $"{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.txt";

                var serilisedNotification = JsonConvert.SerializeObject(notification);

                var path = Path.Combine(subPath, filename);

                File.WriteAllText(path, serilisedNotification);
            }
        }

        static void ShowToast(string appId,RedgateNotifiation notification)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            if (!string.IsNullOrWhiteSpace(notification.ImagePath))
            {
                toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

                String imagePath = "file:///" + notification.ImagePath;
                XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;
            }

            var stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(notification.Title));
            stringElements[1].AppendChild(toastXml.CreateTextNode(notification.Message));
            
            var toast = new ToastNotification(toastXml);

            var events = new ToastEvents();

            toast.Activated += events.ToastActivated;
            toast.Dismissed += events.ToastDismissed;
            toast.Failed += events.ToastFailed;

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
        }
    }
}