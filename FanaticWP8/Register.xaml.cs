using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Facebook;
using System.Windows.Media.Imaging;

namespace FanaticWP8
{
    public partial class Register : PhoneApplicationPage
    {
        public Register()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LoadFacebookData();
        }

        private void LoadFacebookData()
        {
            var fb = new FacebookClient(App.FacebookAccessToken);
            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {

                }

                var result = (IDictionary<string, object>)e.GetResultData();

                Dispatcher.BeginInvoke(() =>
                    {
                        var profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", App.FacebookId, "square", App.FacebookAccessToken);

                        ProfilePicture.Source = new BitmapImage(new Uri(profilePictureUrl));
                        FirstNameBox.Text = result["first_name"].ToString();
                        LastNameBox.Text = result["last_name"].ToString();
                        if (result["gender"].ToString().ToLower() == "male")
                            GenderPicker.SelectedIndex = 1;

                        var location = (IDictionary<string, object>)result["location"];
                        LocationBox.Text = location["name"].ToString();
                        //var favoriteteams = (List<IDictionary<string, object>>)result["favorite_teams"];
                    }
                    
                    );
            };

            fb.GetTaskAsync("me");
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            //WHERE DO THEY GO ONCE THEY ARE REGISTERED?
        }
    }
}