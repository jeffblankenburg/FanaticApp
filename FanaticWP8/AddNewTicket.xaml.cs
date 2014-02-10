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
using System.Windows.Shapes;

namespace FanaticWP8
{
    public partial class AddNewTicket : PhoneApplicationPage
    {
        public string CurrentLeague = "";
        
        public AddNewTicket()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("league"))
            {
                CurrentLeague = NavigationContext.QueryString["league"];
                App.TicketBuilder.League = CurrentLeague;
            }

            if (CurrentLeague == "")
                NavigationService.GoBack();
            else
            {
                LayoutRoot.Background = App.GetBackground(CurrentLeague);
                LoadData();
            }
        }

        private void LoadData()
        {
            if (App.TicketBuilder.AwayTeam != null)
            {
                AwayTeamBox.Children.Clear();
                AwayTeamBox.Children.Add(BuildImage(App.TicketBuilder.AwayTeam, "Away"));
            }
            if (App.TicketBuilder.HomeTeam != null)
            {
                HomeTeamBox.Children.Clear();
                HomeTeamBox.Children.Add(BuildImage(App.TicketBuilder.HomeTeam, "Home"));
                VenueText.Text = App.TicketBuilder.HomeTeam.HomeVenue.Name;
            }
        }

        private Image BuildImage(Team t, string position)
        {
            Image image = new Image();
            image.Name = position;
            image.Width = 120;
            image.Height = 120;
            image.Tap += Team_Tap;
            image.DataContext = t;
            Uri uri = new Uri("Assets/Logos/" + t.League + "/" + t.Abbreviation + ".png", UriKind.Relative);
            ImageSource imageSource = new BitmapImage(uri);
            image.Source = imageSource;
            return image;
        }

        private void Team_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Ellipse el = new Ellipse();
            string teamtype = "";
            Image i = new Image();

            if (sender.GetType() == el.GetType())
            {
                el = sender as Ellipse;
                teamtype = el.Name;
            }
            else if (sender.GetType() == i.GetType())
            {
                i = sender as Image;
                teamtype = i.Name;
            }
          
            NavigationService.Navigate(new Uri("/SelectTeam.xaml?action=" + teamtype + "&league=" + CurrentLeague, UriKind.Relative));
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.RemoveBackEntry();
            NavigationService.GoBack();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            DateTime date = (DateTime)TicketDate.Value;
            DateTime time = (DateTime)TicketTime.Value;
            App.TicketBuilder.Section = SectionBox.Text;
            App.TicketBuilder.Row = RowBox.Text;
            App.TicketBuilder.Seat = SeatBox.Text;
            App.TicketBuilder.Price = PriceBox.Text.ToString();
            App.TicketBuilder.Notes = NotesBox.Text;
            App.TicketBuilder.StartTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            App.TicketBuilder.Location = App.TicketBuilder.HomeTeam.HomeVenue;

            App.Fan.Tickets.Add(App.TicketBuilder);
            App.TicketBuilder = new Ticket();

            NavigationService.RemoveBackEntry();
            NavigationService.GoBack();
        }


    }
}