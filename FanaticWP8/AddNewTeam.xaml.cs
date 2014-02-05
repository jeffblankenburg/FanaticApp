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
    public partial class AddNewTeam : PhoneApplicationPage
    {
        List<League> Leagues = new List<League>();
        
        public AddNewTeam()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void LoadData(int league)
        {

        }

        private void League_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            
            BitmapImage bi = i.Source as BitmapImage;
            
            if (bi.UriSource.ToString().Contains("MLB"))
            {
                LeagueSlider();
            }
        }

        private void LeagueSlider()
        {
            SlideLeagues.Begin();
        }
    }
}