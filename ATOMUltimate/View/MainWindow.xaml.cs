using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using ATOMUltimate.Model;
using RazorEngine;

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

            //na początku programu żaden feed nie jest zaznaczony, więc button jest wygaszony
            UnsubscribeButton.IsEnabled = false;
            ShowDefaultView();
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

            if(atom != null)
                AtomBrowser.NavigateToString(atom.ToHtlm());
            if (SubscriptionsTreeView.SelectedItem == null)
            {
                UnsubscribeButton.IsEnabled = false;
                ShowDefaultView();
            }
            else
                UnsubscribeButton.IsEnabled = true;
                
            SubscriptionManager.Sync(atom);
            AtomBrowser.NavigateToString(atom.ToHtlm());
        }
        private void ShowDefaultView()
        {
            string defaultContent = new StreamReader(@"..\..\View\Default.cshtml").ReadToEnd();
            AtomBrowser.NavigateToString(Razor.Parse(defaultContent, this));
        }
        private void UnsubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            var item = SubscriptionsTreeView.SelectedItem as Atom;
            _model.Feeds.Remove(item);
            SubscriptionManager.Unsubscribe(item);

            
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