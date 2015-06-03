using System;
using System.Collections.Generic;
using System.Linq;
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
            var url = UrlTextBox.Text;
            SubscriptionManager.Subscribe(url);
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
            
        }

    }
}
