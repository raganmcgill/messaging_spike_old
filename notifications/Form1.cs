using System;
using System.Windows.Forms;
using Tulpep.NotificationWindow;

namespace notifications
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PopupNotifier popup = new PopupNotifier {TitleText = "Alert", ContentText = "Test Content"};
            popup.Popup();
        }

        [MTAThread]
        public static void pop()
        {
            PopupNotifier popup = new PopupNotifier { TitleText = "Alert", ContentText = "Test Content" };

            popup.Popup();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pop();
        }
    }
}
