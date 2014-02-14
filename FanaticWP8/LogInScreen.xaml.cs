using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;
using Facebook.Client;
using System.Threading.Tasks;

namespace FanaticWP8
{
    public partial class LogInScreen : PhoneApplicationPage
    {
        public LogInScreen()
        {
            InitializeComponent();
        }

        private void Authenticate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            Image i = sender as Image;
            CheckAuthentication(i);
        }

        private async void CheckAuthentication(Image i)
        {
            if (i.Name.Contains("Microsoft")) await Authenticate(MobileServiceAuthenticationProvider.MicrosoftAccount);
            else if (i.Name.Contains("Facebook")) await Authenticate(MobileServiceAuthenticationProvider.Facebook);
            //{
            //    await AuthenticateFacebook();
            //    //string path = "fbconnect://authorize?client_id={137042686448025}&redirect_uri=msft-{f1f9b070-f351-499a-8a12-2ccee6095f76}:";
            //    //Uri uri = new Uri(path);
            //    //var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            //}//await Authenticate(MobileServiceAuthenticationProvider.Facebook);
            else if (i.Name.Contains("Twitter")) await Authenticate(MobileServiceAuthenticationProvider.Twitter);
            else if (i.Name.Contains("Google")) await Authenticate(MobileServiceAuthenticationProvider.Google);
        }

        private MobileServiceUser user;
        private async System.Threading.Tasks.Task Authenticate(MobileServiceAuthenticationProvider msap)
        {
            try
            {
                user = await App.MobileService.LoginAsync(msap);
            }
            catch
            {

            }
            
            //THIS NEEDS TO BE FIXED TO STORE THE USER'S CREDENTIALs.
            //while (user == null)
            //{
            //    string message;
            //    try
            //    {
            //        user = await App.MobileService.LoginAsync(msap);
            //        message =
            //            string.Format("You are now logged in - {0}", user.UserId);
            //    }
            //    catch (InvalidOperationException)
            //    {
            //        message = "You must log in. Login Required";
            //    }

            //    MessageBox.Show(message);
            //}
        }

        private FacebookSession FacebookSession;
        private async Task AuthenticateFacebook()
        {
            try
            {
                FacebookSession = await App.FacebookSessionClient.LoginAsync("user_about_me");
                App.FacebookAccessToken = FacebookSession.AccessToken;
                App.FacebookId = FacebookSession.FacebookId;
                App.IsFacebookAuthenticated = true;

                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Register.xaml", UriKind.Relative)));

            }
            catch (InvalidOperationException e)
            {

            }
        }
    }
}