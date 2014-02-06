﻿using System;
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

        List<Team> Teams = new List<Team>();
        List<Team> MLB = new List<Team> { new Team { League = "MLB", Abbreviation = "ARI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Arizona", Mascot = "Diamondbacks" },
                                          new Team { League = "MLB", Abbreviation = "ATL", PrimaryColor = "01487E", SecondaryColor = "D60D39", TertiaryColor = "FFFFFF", City = "Atlanta", Mascot = "Braves" },
                                          new Team { League = "MLB", Abbreviation = "BAL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Baltimore", Mascot = "Orioles" },
                                          new Team { League = "MLB", Abbreviation = "BOS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Boston", Mascot = "Red Sox" },
                                          new Team { League = "MLB", Abbreviation = "CHC", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "Cubs" },
                                          new Team { League = "MLB", Abbreviation = "CHW", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "White Sox" },
                                          new Team { League = "MLB", Abbreviation = "CIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cincinnati", Mascot = "Reds" },
                                          new Team { League = "MLB", Abbreviation = "CLE", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cleveland", Mascot = "Indians" },
                                          new Team { League = "MLB", Abbreviation = "COL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Colorado", Mascot = "Rockies" },
                                          new Team { League = "MLB", Abbreviation = "DET", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Detroit", Mascot = "Tigers" },
                                          new Team { League = "MLB", Abbreviation = "HOU", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Houston", Mascot = "Astros" },
                                          new Team { League = "MLB", Abbreviation = "KC",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Kansas City", Mascot = "Royals" },
                                          new Team { League = "MLB", Abbreviation = "LAA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Los Angeles", Mascot = "Angels" },
                                          new Team { League = "MLB", Abbreviation = "LAD", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Los Angeles", Mascot = "Dodgers" },
                                          new Team { League = "MLB", Abbreviation = "MIA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Miami", Mascot = "Marlins" },
                                          new Team { League = "MLB", Abbreviation = "MIL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Milwaukee", Mascot = "Brewers" },
                                          new Team { League = "MLB", Abbreviation = "MIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Minnesota", Mascot = "Twins" },
                                          new Team { League = "MLB", Abbreviation = "NYM", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Mets" },
                                          new Team { League = "MLB", Abbreviation = "NYY", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Yankees" },
                                          new Team { League = "MLB", Abbreviation = "OAK", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Oakland", Mascot = "Athletics" },
                                          new Team { League = "MLB", Abbreviation = "PHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Philadelphia", Mascot = "Phillies" },
                                          new Team { League = "MLB", Abbreviation = "PIT", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Pittsburgh", Mascot = "Pirates" },
                                          new Team { League = "MLB", Abbreviation = "SD",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Diego", Mascot = "Padres" },
                                          new Team { League = "MLB", Abbreviation = "SF",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Francisco", Mascot = "Giants" },
                                          new Team { League = "MLB", Abbreviation = "SEA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Seattle", Mascot = "Mariners" },
                                          new Team { League = "MLB", Abbreviation = "STL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "St. Louis", Mascot = "Cardinals" },
                                          new Team { League = "MLB", Abbreviation = "TB",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Tampa Bay", Mascot = "Rays" },
                                          new Team { League = "MLB", Abbreviation = "TEX", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Texas", Mascot = "Rangers" },
                                          new Team { League = "MLB", Abbreviation = "TOR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Toronto", Mascot = "Blue Jays" },
                                          new Team { League = "MLB", Abbreviation = "WAS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Washington", Mascot = "Nationals" }};

        List<Team> NFL = new List<Team> { new Team { League = "NFL", Abbreviation = "ARI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Arizona", Mascot = "Cardinals" },
                                          new Team { League = "NFL", Abbreviation = "ATL", PrimaryColor = "01487E", SecondaryColor = "D60D39", TertiaryColor = "FFFFFF", City = "Atlanta", Mascot = "Falcons" },
                                          new Team { League = "NFL", Abbreviation = "BAL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Baltimore", Mascot = "Ravens" },
                                          new Team { League = "NFL", Abbreviation = "BUF", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Buffalo", Mascot = "Bills" },
                                          new Team { League = "NFL", Abbreviation = "CAR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Carolina", Mascot = "Panthers" },
                                          new Team { League = "NFL", Abbreviation = "CHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "Bears" },
                                          new Team { League = "NFL", Abbreviation = "CIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cincinnati", Mascot = "Bengals" },
                                          new Team { League = "NFL", Abbreviation = "CLE", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cleveland", Mascot = "Browns" },
                                          new Team { League = "NFL", Abbreviation = "DAL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Dallas", Mascot = "Cowboys" },
                                          new Team { League = "NFL", Abbreviation = "DEN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Denver", Mascot = "Broncos" },
                                          new Team { League = "NFL", Abbreviation = "DET", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Detroit", Mascot = "Lions" },
                                          new Team { League = "NFL", Abbreviation = "GB",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Green Bay", Mascot = "Packers" },
                                          new Team { League = "NFL", Abbreviation = "HOU", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Houston", Mascot = "Texans" },
                                          new Team { League = "NFL", Abbreviation = "IND", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Indianapolis", Mascot = "Colts" },
                                          new Team { League = "NFL", Abbreviation = "JAC", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Jacksonville", Mascot = "Jaguars" },
                                          new Team { League = "NFL", Abbreviation = "KC",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Kansas City", Mascot = "Chiefs" },
                                          new Team { League = "NFL", Abbreviation = "MIA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Miami", Mascot = "Dolphins" },
                                          new Team { League = "NFL", Abbreviation = "MIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Minnesota", Mascot = "Vikings" },
                                          new Team { League = "NFL", Abbreviation = "NE",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New England", Mascot = "Patriots" },
                                          new Team { League = "NFL", Abbreviation = "NO",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New Orleans", Mascot = "Saints" },
                                          new Team { League = "NFL", Abbreviation = "NYG", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Giants" },
                                          new Team { League = "NFL", Abbreviation = "NYJ", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Jets" },
                                          new Team { League = "NFL", Abbreviation = "OAK", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Oakland", Mascot = "Raiders" },
                                          new Team { League = "NFL", Abbreviation = "PHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Philadelphia", Mascot = "Eagles" },
                                          new Team { League = "NFL", Abbreviation = "PIT", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Pittsburgh", Mascot = "Steelers" },
                                          new Team { League = "NFL", Abbreviation = "SD",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Diego", Mascot = "Chargers" },
                                          new Team { League = "NFL", Abbreviation = "SEA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Seattle", Mascot = "Seahawks" },
                                          new Team { League = "NFL", Abbreviation = "SF",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Francisco", Mascot = "49ers" },
                                          new Team { League = "NFL", Abbreviation = "STL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "St. Louis", Mascot = "Rams" },
                                          new Team { League = "NFL", Abbreviation = "TB",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Tampa Bay", Mascot = "Buccaneers" },
                                          new Team { League = "NFL", Abbreviation = "TEN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Tennessee", Mascot = "Titans" },
                                          new Team { League = "NFL", Abbreviation = "WAS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Washington", Mascot = "Redskins" },};

        List<Team> NHL = new List<Team> { new Team { League = "NHL", Abbreviation = "ANA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Arizona", Mascot = "Diamondbacks" },
                                          new Team { League = "NHL", Abbreviation = "BOS", PrimaryColor = "01487E", SecondaryColor = "D60D39", TertiaryColor = "FFFFFF", City = "Atlanta", Mascot = "Braves" },
                                          new Team { League = "NHL", Abbreviation = "BUF", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Baltimore", Mascot = "Orioles" },
                                          new Team { League = "NHL", Abbreviation = "CAL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Boston", Mascot = "Red Sox" },
                                          new Team { League = "NHL", Abbreviation = "CAR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "Cubs" },
                                          new Team { League = "NHL", Abbreviation = "CBJ", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "White Sox" },
                                          new Team { League = "NHL", Abbreviation = "CHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cincinnati", Mascot = "Reds" },
                                          new Team { League = "NHL", Abbreviation = "COL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cleveland", Mascot = "Indians" },
                                          new Team { League = "NHL", Abbreviation = "DAL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Colorado", Mascot = "Rockies" },
                                          new Team { League = "NHL", Abbreviation = "DET", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Detroit", Mascot = "Tigers" },
                                          new Team { League = "NHL", Abbreviation = "EDM", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Houston", Mascot = "Astros" },
                                          new Team { League = "NHL", Abbreviation = "FLA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Kansas City", Mascot = "Royals" },
                                          new Team { League = "NHL", Abbreviation = "LA",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Los Angeles", Mascot = "Angels" },
                                          new Team { League = "NHL", Abbreviation = "MIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Los Angeles", Mascot = "Dodgers" },
                                          new Team { League = "NHL", Abbreviation = "MON", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Miami", Mascot = "Marlins" },
                                          new Team { League = "NHL", Abbreviation = "NJ",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Milwaukee", Mascot = "Brewers" },
                                          new Team { League = "NHL", Abbreviation = "NSH", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Minnesota", Mascot = "Twins" },
                                          new Team { League = "NHL", Abbreviation = "NYI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Mets" },
                                          new Team { League = "NHL", Abbreviation = "NYR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Yankees" },
                                          new Team { League = "NHL", Abbreviation = "OTT", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Oakland", Mascot = "Athletics" },
                                          new Team { League = "NHL", Abbreviation = "PHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Philadelphia", Mascot = "Phillies" },
                                          new Team { League = "NHL", Abbreviation = "PHX", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Pittsburgh", Mascot = "Pirates" },
                                          new Team { League = "NHL", Abbreviation = "PIT", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Diego", Mascot = "Padres" },
                                          new Team { League = "NHL", Abbreviation = "SJ",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Francisco", Mascot = "Giants" },
                                          new Team { League = "NHL", Abbreviation = "STL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Seattle", Mascot = "Mariners" },
                                          new Team { League = "NHL", Abbreviation = "TB", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "St. Louis", Mascot = "Cardinals" },
                                          new Team { League = "NHL", Abbreviation = "TOR",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Tampa Bay", Mascot = "Rays" },
                                          new Team { League = "NHL", Abbreviation = "VAN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Texas", Mascot = "Rangers" },
                                          new Team { League = "NHL", Abbreviation = "WAS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Toronto", Mascot = "Blue Jays" },
                                          new Team { League = "NHL", Abbreviation = "WIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Washington", Mascot = "Nationals" }};

        List<Team> NBA = new List<Team>();
        List<Team> MLS = new List<Team>();
        List<Team> MiLB = new List<Team>();
        
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

            if (CurrentLeague != "")
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
                    CurrentTeams = MLB;
                    break;
                case "NFL":
                    CurrentTeams = NFL;
                    break;
                case "NBA":
                    CurrentTeams = NBA;
                    break;
                case "NHL":
                    CurrentTeams = NHL;
                    break;
                case "MLS":
                    CurrentTeams = MLS;
                    break;
                case "MiLB":
                    CurrentTeams = MiLB;
                    break;
            }

            foreach (Team t in CurrentTeams)
            {
                TeamPanel.Children.Add(BuildImage(t));
            }
            
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
            
            //<Image Source="Assets/Logos/MLB/MLB.png" Width="100" Margin="0,0,10,10" />
        }

        void Team_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image i = sender as Image;
            App.Settings[CurrentLeague] = i.DataContext as Team;
            NavigationService.RemoveBackEntry();
            NavigationService.GoBack();
        }
    }
}