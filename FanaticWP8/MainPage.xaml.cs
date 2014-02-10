using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FanaticWP8.Resources;
using Fanatic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FanaticWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        double[] counter = new double[] { 0, 0, 0 };
        double team1Score = 90;
        double team2Score = 57;
        double team3Score = 1042;


        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            TwentiethOfASecond.Completed += TwentiethOfASecond_Completed;
            DisplayTotals();
            PopulateTeamPanorama();
            PopulateTicketPanorama();
        }

        private void PopulateTicketPanorama()
        {
            ResetTicketPanorama();

            if (App.Fan.Tickets.Count() > 0)
            {

                List<Ticket> TicketList = (from t in App.Fan.Tickets
                                           orderby t.StartTime
                                           select t).ToList();

                for (int i = 0; i < TicketList.Count(); i++)
                {
                    Grid g = new Grid();
                    g.Height = 120;
                    g.Width = 120;
                    g.Margin = new Thickness(0, 0, 10, 10);
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(7) });
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(6) });
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(7) });
                    g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(7) });
                    g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });
                    g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });
                    g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(6) });
                    g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                    g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(7) });
                    
                    //Date text
                    TextBlock date = new TextBlock();
                    date.Text = TicketList[i].StartTime.ToShortDateString();
                    date.TextAlignment = TextAlignment.Center;
                    date.FontSize = 12;
                    Grid.SetColumn(date, 1);
                    Grid.SetRow(date, 1);
                    Grid.SetColumnSpan(date, 3);
                    g.Children.Add(date);

                    //Venue text
                    TextBlock venue = new TextBlock();
                    venue.Text = TicketList[i].Location.Name;
                    venue.TextAlignment = TextAlignment.Center;
                    venue.FontSize = 12;
                    Grid.SetColumn(venue, 1);
                    Grid.SetRow(venue, 2);
                    Grid.SetColumnSpan(venue, 3);
                    g.Children.Add(venue);

                    //Away team
                    Image image = new Image();
                    Uri uri = new Uri("Assets/Logos/" + TicketList[i].AwayTeam.League + "/" + TicketList[i].AwayTeam.Abbreviation + ".png", UriKind.Relative);
                    ImageSource imageSource = new BitmapImage(uri);
                    image.Source = imageSource;
                    Grid.SetColumn(image, 1);
                    Grid.SetRow(image, 4);
                    g.Children.Add(image);

                    //Home team
                    image = new Image();
                    uri = new Uri("Assets/Logos/" + TicketList[i].HomeTeam.League + "/" + TicketList[i].HomeTeam.Abbreviation + ".png", UriKind.Relative);
                    imageSource = new BitmapImage(uri);
                    image.Source = imageSource;
                    Grid.SetColumn(image, 3);
                    Grid.SetRow(image, 4);
                    g.Children.Add(image);

                    TicketPanel.Children.Add(g);

                    //Grid g = new Grid();
                    //g.Width = 120;
                    //g.Height = 120;
                    //g.Margin = new Thickness(0, 0, 10, 10);
                    //Rectangle r = new Rectangle();
                    //r.Fill = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF));
                    //g.Children.Add(r);
                    //TextBlock t = new TextBlock();
                    //t.Text = "+";
                    //t.VerticalAlignment = VerticalAlignment.Center;
                    //t.HorizontalAlignment = HorizontalAlignment.Center;
                    //t.TextAlignment = TextAlignment.Center;
                    //t.FontSize = 60;
                    //t.Margin = new Thickness(0, -15, 0, 0);
                    //g.Children.Add(t);
                    //Ellipse e = new Ellipse();
                    //e.Stroke = new SolidColorBrush(Colors.White);
                    //e.Fill = new SolidColorBrush(Colors.Transparent);
                    //e.StrokeThickness = 3;
                    //e.Margin = new Thickness(30);
                    //g.Children.Add(e);
                }
            }
        }

        void TwentiethOfASecond_Completed(object sender, EventArgs e)
        {
            DisplayTotals();
        }

        private void PopulateTeamPanorama()
        {
            ResetTeamPanorama();
            
            if (App.Fan.MLB.Abbreviation != "NONE")
            {
                Team t = App.Fan.MLB;
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Fan.NFL.Abbreviation != "NONE")
            {
                Team t = App.Fan.NFL;
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Fan.NBA.Abbreviation != "NONE")
            {
                Team t = App.Fan.NBA;
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Fan.NHL.Abbreviation != "NONE")
            {
                Team t = App.Fan.NHL;
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Fan.MLS.Abbreviation != "NONE")
            {
                Team t = App.Fan.MLS;
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Fan.MiLB.Abbreviation != "NONE")
            {
                Team t = App.Fan.MiLB;
                TeamPanel.Children.Add(BuildImage(t));
            }
        }

        private Image BuildImage(Team t)
        {
            Image image = new Image();
            image.Width = 120;
            image.Height = 120;
            image.Margin = new Thickness(0, 0, 10, 10);
            image.Tap += Team_Tap;
            image.DataContext = t;
            Uri uri = new Uri("Assets/Logos/" + t.League + "/" + t.Abbreviation + ".png", UriKind.Relative);
            ImageSource imageSource = new BitmapImage(uri);
            image.Source = imageSource;
            return image;
        }

        private void ResetTeamPanorama()
        {
            TeamPanel.Children.Clear();
            Grid g = BuildAddButton();
            Ellipse e = g.Children[2] as Ellipse;
            e.Tap += AddNewTeam_Tap;
            TeamPanel.Children.Add(g);
        }

        private void ResetTicketPanorama()
        {
            TicketPanel.Children.Clear();
            Grid g = BuildAddButton();
            Ellipse e = g.Children[2] as Ellipse;
            e.Tap += AddNewTicket_Tap;
            TicketPanel.Children.Add(g);
        }

        private Grid BuildAddButton()
        {
            Grid g = new Grid();
            g.Width = 120;
            g.Height = 120;
            g.Margin = new Thickness(0, 0, 10, 10);
            Rectangle r = new Rectangle();
            r.Fill = new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF));
            g.Children.Add(r);
            TextBlock t = new TextBlock();
            t.Text = "+";
            t.VerticalAlignment = VerticalAlignment.Center;
            t.HorizontalAlignment = HorizontalAlignment.Center;
            t.TextAlignment = TextAlignment.Center;
            t.FontSize = 60;
            t.Margin = new Thickness(0, -15, 0, 0);
            g.Children.Add(t);
            Ellipse e = new Ellipse();
            e.Stroke = new SolidColorBrush(Colors.White);
            e.Fill = new SolidColorBrush(Colors.Transparent);
            e.StrokeThickness = 3;
            e.Margin = new Thickness(30);
            g.Children.Add(e);
            return g;
        }

        private void Team_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void AddNewTeam_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectLeague.xaml?actionpage=SelectTeam.xaml", UriKind.Relative));
        }

        private void AddNewTicket_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectLeague.xaml?actionpage=AddNewTicket.xaml", UriKind.Relative));
        }

        private void DisplayTotals()
        {
            counter[0] += team1Score * .04;
            counter[1] += team2Score * .04;
            counter[2] += team3Score * .04;
            Team1Score.Text = String.Format("{0:n0}", counter[0]);
            Team2Score.Text = String.Format("{0:n0}", counter[1]);
            Team3Score.Text = String.Format("{0:n0}", counter[2]);
            TotalScore.Text = String.Format("{0:n0}", (counter[0] + counter[1] + counter[2]));

            if ((team1Score > counter[0]) || (team2Score > counter[1]) || (team3Score > counter[2]))
            {
                TwentiethOfASecond.Begin();
            }
            else
            {
                Team1Score.Text = String.Format("{0:n0}", team1Score);
                Team2Score.Text = String.Format("{0:n0}", team2Score);
                Team3Score.Text = String.Format("{0:n0}", team3Score);
                TotalScore.Text = String.Format("{0:n0}", (team1Score + team2Score + team3Score));
            }


        }
    }
}