using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using ATOMUltimate.Model;
using Microsoft.Win32;
using RazorEngine;

namespace ATOMUltimate.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _model;
        private bool willNavigate;

        public MainWindow()
        {
            SetBrowserFeatureControl();
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

        private static void SetBrowserFeatureControl()
        {
            // http://msdn.microsoft.com/en-us/library/ee330720(v=vs.85).aspx

            // WebBrowser Feature Control settings are per-process
            var fileName = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

            // make the control is not running inside Visual Studio Designer
            if (String.Compare(fileName, "devenv.exe", true) == 0 || String.Compare(fileName, "XDesProc.exe", true) == 0)
                return;

            SetBrowserFeatureControlKey("FEATURE_BROWSER_EMULATION", fileName, GetBrowserEmulationMode());
        }

        private static void SetBrowserFeatureControlKey(string feature, string appName, uint value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(
                String.Concat(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\", feature),
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                key.SetValue(appName, (UInt32)value, RegistryValueKind.DWord);
            }
        }

        private static UInt32 GetBrowserEmulationMode()
        {
            int browserVersion = 7;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
                RegistryKeyPermissionCheck.ReadSubTree,
                System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = ieKey.GetValue("svcVersion");
                if (null == version)
                {
                    version = ieKey.GetValue("Version");
                    if (null == version)
                        throw new ApplicationException("Microsoft Internet Explorer is required!");
                }
                int.TryParse(version.ToString().Split('.')[0], out browserVersion);
            }

            // Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode. Default value for Internet Explorer 10.
            UInt32 mode = 10000;

            switch (browserVersion)
            {
                case 7:
                    // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. Default value for applications hosting the WebBrowser Control.
                    mode = 7000;
                    break;
                case 8:
                    // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. Default value for Internet Explorer 8
                    mode = 8000;
                    break;
                case 9:
                    // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
                    mode = 9000;
                    break;
                default:
                    // use IE10 mode by default
                    break;
            }

            return mode;
        }

        private void SubscriptionsTreeView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Atom atom = SubscriptionsTreeView.SelectedItem as Atom;
            if (atom==null)
            {
                return;
            }
            
            SubscriptionManager.Sync(atom);
            
            willNavigate = false;
            AtomBrowser.NavigateToString(atom.ToHtlm());
            
            if (SubscriptionsTreeView.SelectedItem == null)
            {
                UnsubscribeButton.IsEnabled = false;
                ShowDefaultView();
            }
            else
                UnsubscribeButton.IsEnabled = true;
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

            
        private void webBrowser1_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            // first page needs to be loaded in webBrowser control
            if (!willNavigate)
            {
                willNavigate = true;
                return;
            }

            string linkUrl = e.Uri.ToString();

            //oznacz entry z tym linkiem jako przeczytany
            //todo: odhaczyć bez przeładowania
            var atom = SubscriptionsTreeView.SelectedItem as Atom;
            var entries = atom.Entries.Where(x => x.Link.Any(l => l.Href == linkUrl));
            foreach (var entry in entries)
            {
                entry.Przeczytany = true;
            }

            // cancel navigation to the clicked link in the webBrowser control
            e.Cancel = true;
            var startInfo = new ProcessStartInfo
            {
                FileName = linkUrl
            };

            Process.Start(startInfo);
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