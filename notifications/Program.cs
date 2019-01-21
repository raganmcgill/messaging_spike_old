using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MassTransit;
using MassTransit.RabbitMqTransport;
using notifications.Properties;
using Tulpep.NotificationWindow;
using System;
using System.IO;
using System.Diagnostics;
using System.Xml;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
using XmlNodeList = Windows.Data.Xml.Dom.XmlNodeList;

namespace notifications
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            





            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                IRabbitMqHost rabbitMqHost = rabbit.Host(new Uri("rabbitmq://localhost"), settings =>
                {
                    settings.Password("guest");
                    settings.Username("guest");
                });
               // PopupNotifier popup = new PopupNotifier { TitleText = "Alert", ContentText = "sdsd" };

               // popup.Popup();
                rabbit.ReceiveEndpoint(rabbitMqHost, "redgate.notifications", conf =>
                {
                    PopupNotifier popup = new PopupNotifier { TitleText = "Alert", ContentText = "xxxx" };

                    popup.Popup();
                    conf.Consumer<NotificationConsumer>();
                });
            });

            rabbitBusControl.Start();
            //Console.ReadLine();
            //Application.Run(new Form1());

            Application.Run(new MyCustomApplicationContext());

            rabbitBusControl.Stop();

        }

        public class MyCustomApplicationContext : ApplicationContext
        {
            private NotifyIcon trayIcon;

            public MyCustomApplicationContext()
            {
                // Initialize Tray Icon
                trayIcon = new NotifyIcon()
                {
                    ContextMenu = new ContextMenu(new MenuItem[] {
                        new MenuItem("Exit", Exit)
                    }),
                    Visible = true
                };

//                PopupNotifier popup = new PopupNotifier { TitleText = "Alert", ContentText = "Starting" };
//
//                popup.Popup();
            }

            void Exit(object sender, EventArgs e)
            {
                // Hide tray icon, otherwise it will remain shown until user mouses over it
                trayIcon.Visible = false;

                Application.Exit();
            }
        }
    }
}
