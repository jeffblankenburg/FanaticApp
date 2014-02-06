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
        }

        void TwentiethOfASecond_Completed(object sender, EventArgs e)
        {
            DisplayTotals();
        }

        private void PopulateTeamPanorama()
        {
            ResetTeamPanorama();
            
            if (App.Settings["MLB"] != null)
            {
                Team t = (Team)App.Settings["MLB"];
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Settings["NFL"] != null)
            {
                Team t = (Team)App.Settings["NFL"];
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Settings["NBA"] != null)
            {
                Team t = (Team)App.Settings["NBA"];
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Settings["NHL"] != null)
            {
                Team t = (Team)App.Settings["NHL"];
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Settings["MLS"] != null)
            {
                Team t = (Team)App.Settings["MLS"];
                TeamPanel.Children.Add(BuildImage(t));
            }
            if (App.Settings["MiLB"] != null)
            {
                Team t = (Team)App.Settings["MiLB"];
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
            e.Tap += AddNewTeam_Tap;
            g.Children.Add(e);
            TeamPanel.Children.Add(g);
        }

        private void Team_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void AddNewTeam_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddNewTeam.xaml", UriKind.Relative));
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