using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace ATOMUltimate.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SubscriptionManager.Initialize();

            foreach (var item in SubscriptionManager.Feeds)
            {
                SubscriptionsTreeView.Items.Add(item);
            }
        }

        private void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            SubscribeWindow subscribeWindow = new SubscribeWindow();
            bool? result = subscribeWindow.ShowDialog();
            if (result.HasValue)
            {
                if (result.Value == true)
                {
                    SubscriptionsTreeView.Items.Add(SubscriptionManager.Feeds.Last());
                }
            }
        }
    }
}
