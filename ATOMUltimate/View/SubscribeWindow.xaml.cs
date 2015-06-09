using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ATOMUltimate.View
{
    /// <summary>
    /// Interaction logic for SubscribeWindow.xaml
    /// </summary>
    public partial class SubscribeWindow : Window
    {
        public SubscribeWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            
            List<string> errorList = new List<string>();
            foreach (string url2 in UrlTextBox.Text.Split('\n', '\r'))
            {
                
                if (string.IsNullOrEmpty(url2))
                {
                    continue;
                }
                string url = url2;
                //if (!url.StartsWith("http://"))
                //    url = "http://" + url;
                try
                {
                    SubscriptionManager.Subscribe(url);
                }
                catch (Exception)
                {
                    errorList.Add(url2);
                }
            }
            this.DialogResult = true;
            this.Hide();
            if (errorList.Count !=0)
            {
                MessageBox.Show("Następujące Linki nie mogły być przetworone \n" + errorList.Aggregate((s, s1) => s+"\n"+s1),"Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
            }

            
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
            
        }

    }
}
