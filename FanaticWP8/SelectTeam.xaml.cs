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
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FanaticWP8
{
    public partial class SelectTeam : PhoneApplicationPage
    {
        string CurrentLeague;
        string Action;

        List<Team> Teams = new List<Team>();
        
        public SelectTeam()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("league"))
            {
                CurrentLeague = NavigationContext.QueryString["league"];
            }

            if (NavigationContext.QueryString.ContainsKey("action"))
            {
                Action = NavigationContext.QueryString["action"];
            }

            if (CurrentLeague != "" || Action != "")
                LoadTeams(CurrentLeague);
            else
                NavigationService.GoBack();
        }

        private void LoadTeams(string league)
        {
            List<Team> CurrentTeams = new List<Team>();
            switch (league)
            {
                case "MLB":
                    CurrentTeams = App.MLB;
                    break;
                case "NFL":
                    CurrentTeams = App.NFL;
                    break;
                case "NBA":
                    CurrentTeams = App.NBA;
                    break;
                case "NHL":
                    CurrentTeams = App.NHL;
                    break;
                case "MLS":
                    CurrentTeams = App.MLS;
                    break;
                case "MiLB":
                    CurrentTeams = App.MiLB;
                    break;
            }

            foreach (Team t in CurrentTeams)
            {
                TeamPanel.Children.Add(BuildImage(t));
            }
            
        }

        private void SaveTeam(string league, Team t)
        {
            switch (league)
            {
                case "MLB":
                    App.Fan.MLB = t;
                    break;
                case "NFL":
                    App.Fan.NFL = t;
                    break;
                case "NBA":
                    App.Fan.NBA = t;
                    break;
                case "NHL":
                    App.Fan.NHL = t;
                    break;
                case "MLS":
                    App.Fan.MLS = t;
                    break;
                case "MiLB":
                    App.Fan.MiLB = t;
                    break;
            }
            App.Settings["Fan"] = App.Fan;
            App.Settings.Save();
        }

        private Image BuildImage(Team team)
        {
            Image image = new Image();
            image.Width = 100;
            image.Height = 100;
            image.Margin = new Thickness(0, 0, 10, 10);
            image.Tap += Team_Tap;
            image.DataContext = team;
            Uri uri = new Uri("Assets/Logos/" + team.League + "/" + team.Abbreviation + ".png", UriKind.Relative);
            ImageSource imageSource = new BitmapImage(uri);
            image.Source = imageSource;
            return image;
        }

        void Team_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            
            if (Action == "Away")
            {
                App.TicketBuilder.AwayTeam = i.DataContext as Team;
                NavigationService.GoBack();
            }
            else if (Action == "Home")
            {
                App.TicketBuilder.HomeTeam = i.DataContext as Team;
                NavigationService.GoBack();
            }
            else
            {
                SaveTeam(CurrentLeague, i.DataContext as Team);
                NavigationService.RemoveBackEntry();
                NavigationService.GoBack();
            }
        }
    }
}