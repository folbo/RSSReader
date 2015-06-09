using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly MainViewModel _model;

        public MainWindow()
        {
            InitializeComponent();
            SubscriptionManager.Initialize();
            _model = new MainViewModel();
            DataContext = _model;
            foreach (Atom feed in SubscriptionManager.Feeds)
            {
                _model.Feeds.Add(feed);
            }


        }

        private void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            SubscribeWindow subscribeWindow = new SubscribeWindow();
            bool? result = subscribeWindow.ShowDialog();
            _model.Feeds.Clear();
            foreach (Atom feed in SubscriptionManager.Feeds)
            {
                _model.Feeds.Add(feed);
            }
            
        }

        private void SubscriptionsTreeView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Atom atom = SubscriptionsTreeView.SelectedItem as Atom;

            AtomBrowser.NavigateToString(atom.ToHtlm());
        }





    }


    public class MainViewModel
    {
        public ObservableCollection<Atom> Feeds
        {
            get { return _feeds; }
            set
            {
                _feeds = value;
            }
        }

        private ObservableCollection<Atom> _feeds = new ObservableCollection<Atom>();

    }
}