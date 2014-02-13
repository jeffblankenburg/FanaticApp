using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Fanatic;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace FanaticWP8
{
    public partial class SelectLeague : PhoneApplicationPage
    {
        string ActionPage = "";
        
        public SelectLeague()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.ContainsKey("actionpage"))
            {
                ActionPage = NavigationContext.QueryString["actionpage"];
            }

            if (ActionPage == "")
                NavigationService.GoBack();

            App.TicketBuilder = new Ticket();
        }

        private void League_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            BitmapImage bi = i.Source as BitmapImage;
            
            if (bi.UriSource.ToString().Contains("MLB"))
            {
                NavigationService.Navigate(new Uri("/" + ActionPage + "?league=MLB", UriKind.Relative));
            }
            else if (bi.UriSource.ToString().Contains("NFL"))
            {
                NavigationService.Navigate(new Uri("/" + ActionPage + "?league=NFL", UriKind.Relative));
            }
            else if (bi.UriSource.ToString().Contains("NBA"))
            {
                NavigationService.Navigate(new Uri("/" + ActionPage + "?league=NBA", UriKind.Relative));
            }
            else if (bi.UriSource.ToString().Contains("NHL"))
            {
                NavigationService.Navigate(new Uri("/" + ActionPage + "?league=NHL", UriKind.Relative));
            }
            else if (bi.UriSource.ToString().Contains("MLS"))
            {
                NavigationService.Navigate(new Uri("/" + ActionPage + "?league=MLS", UriKind.Relative));
            }
            else if (bi.UriSource.ToString().Contains("MiLB"))
            {
                NavigationService.Navigate(new Uri("/" + ActionPage + "?league=MiLB", UriKind.Relative));
            }
            else if (bi.UriSource.ToString().Contains("FAPL"))
            {
                NavigationService.Navigate(new Uri("/" + ActionPage + "?league=FAPL", UriKind.Relative));
            }
        }
    }
}