using System;
using System.IO;
using System.Linq;
using System.Windows;
using ATOMUltimate.Model;

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
                    //TODO: nie dodawać duplikatów
                    /*
                    SubscriptionsTreeView.Items.MoveCurrentToLast();
                    //SubscriptionsTreeView.Items.R
                    if (SubscriptionManager.Feeds.Last() == (Atom) SubscriptionsTreeView.SelectedItem)
                        return;
                     * */
                    SubscriptionsTreeView.Items.Add(SubscriptionManager.Feeds.Last());
                }
            }
        }
    }
}
