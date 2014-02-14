using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FanaticWP8.Resources;
using System.IO.IsolatedStorage;
using Fanatic;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAzure.MobileServices;
using Facebook.Client;
using System.Threading.Tasks;
using TweetSharp;

namespace FanaticWP8
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        public static IsolatedStorageSettings Settings = IsolatedStorageSettings.ApplicationSettings;
        public static MobileServiceClient MobileService = new MobileServiceClient("https://fanaticapp.azure-mobile.net/", "sCUZwWODAHEHZSpHUgaJhmWkAXLByB75");
        public static User Fan;
        public static Ticket TicketBuilder = new Ticket();
        public static readonly string FacebookAppId = "137042686448025";
        internal static string FacebookAccessToken = String.Empty;
        internal static string FacebookId = String.Empty;
        public static bool IsFacebookAuthenticated = false;
        public static FacebookSessionClient FacebookSessionClient = new FacebookSessionClient(FacebookAppId);



        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            if (!Settings.Contains("User"))
            {
                Fan = new User();
                Settings["User"] = Fan;
            }
            else
            {
                Fan = (User)Settings["User"];
            }



        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }

        //http://en.wikipedia.org/wiki/List_of_Major_League_Baseball_stadiums#Map_of_current_stadiums
        //http://en.wikipedia.org/wiki/List_of_current_National_Football_League_stadiums
        //http://en.wikipedia.org/wiki/List_of_National_Basketball_Association_arenas
        //http://en.wikipedia.org/wiki/NHL_arenas
        //http://en.wikipedia.org/wiki/Soccer-specific_stadium#Current_MLS_soccer-specific_stadiums
        //http://en.wikipedia.org/wiki/List_of_Triple-A_baseball_stadiums
        //http://en.wikipedia.org/wiki/List_of_Premier_League_stadiums


        public static List<Venue> Venues = new List<Venue> 
        { 
            //MLB
            new Venue{ Id = 0,  Capacity = 45050, OpeningYear = 1966, SurfaceType = Surface.Grass, Name = "Angel Stadium of Anaheim", Address="2000 Gene Autry Way", City="Anaheim", State="CA", ZipCode="92806"}, 
            new Venue{ Id = 1,  Capacity = 41503, OpeningYear = 2000, SurfaceType = Surface.Grass, Name = "AT&T Park"},
            new Venue{ Id = 2,  Capacity = 43975, OpeningYear = 2006, SurfaceType = Surface.Grass, Name = "Busch Stadium"},
            new Venue{ Id = 3,  Capacity = 48633, OpeningYear = 1998, SurfaceType = Surface.Grass, Name = "Chase Field"},
            new Venue{ Id = 4,  Capacity = 41800, OpeningYear = 2009, SurfaceType = Surface.Grass, Name = "Citi Field"},
            new Venue{ Id = 5,  Capacity = 43651, OpeningYear = 2004, SurfaceType = Surface.Grass, Name = "Citizens Bank Park"},
            new Venue{ Id = 6,  Capacity = 41782, OpeningYear = 2000, SurfaceType = Surface.Grass, Name = "Comerica Park"},
            new Venue{ Id = 7,  Capacity = 50455, OpeningYear = 1995, SurfaceType = Surface.Grass, Name = "Coors Field"},
            new Venue{ Id = 8,  Capacity = 56000, OpeningYear = 1962, SurfaceType = Surface.Grass, Name = "Dodger Stadium"},
            new Venue{ Id = 9,  Capacity = 37493, OpeningYear = 1912, SurfaceType = Surface.Grass, Name = "Fenway Park"},
            new Venue{ Id = 10, Capacity = 48114, OpeningYear = 1994, SurfaceType = Surface.Grass, Name = "Globe Life Park in Arlington"},
            new Venue{ Id = 11, Capacity = 42271, OpeningYear = 2003, SurfaceType = Surface.Grass, Name = "Great American Ball Park"},
            new Venue{ Id = 12, Capacity = 37903, OpeningYear = 1973, SurfaceType = Surface.Grass, Name = "Kauffman Stadium"},
            new Venue{ Id = 13, Capacity = 37442, OpeningYear = 2012, SurfaceType = Surface.Grass, Name = "Marlins Park"},
            new Venue{ Id = 14, Capacity = 41900, OpeningYear = 2001, SurfaceType = Surface.Grass, Name = "Miller Park"},
            new Venue{ Id = 15, Capacity = 40963, OpeningYear = 2000, SurfaceType = Surface.Grass, Name = "Minute Maid Park"},
            new Venue{ Id = 16, Capacity = 41546, OpeningYear = 2008, SurfaceType = Surface.Grass, Name = "Nationals Park"},
            new Venue{ Id = 17, Capacity = 34077, OpeningYear = 1966, SurfaceType = Surface.Grass, Name = "O.co Coliseum"},
            new Venue{ Id = 18, Capacity = 48876, OpeningYear = 1992, SurfaceType = Surface.Grass, Name = "Oriole Park at Camden Yards"},
            new Venue{ Id = 19, Capacity = 46000, OpeningYear = 2004, SurfaceType = Surface.Grass, Name = "Petco Park"},
            new Venue{ Id = 20, Capacity = 38362, OpeningYear = 2001, SurfaceType = Surface.Grass, Name = "PNC Park"},
            new Venue{ Id = 21, Capacity = 43545, OpeningYear = 1994, SurfaceType = Surface.Grass, Name = "Progressive Field"},
            new Venue{ Id = 22, Capacity = 49539, OpeningYear = 1989, SurfaceType = Surface.Artificial, Name = "Rogers Centre"},
            new Venue{ Id = 23, Capacity = 47116, OpeningYear = 1999, SurfaceType = Surface.Grass, Name = "Safeco Field"},
            new Venue{ Id = 24, Capacity = 39504, OpeningYear = 2010, SurfaceType = Surface.Grass, Name = "Target Field", Address="1 Twins Way", City="Minneapolis", State="MN", ZipCode="55403"},
            new Venue{ Id = 25, Capacity = 43772, OpeningYear = 1990, SurfaceType = Surface.Artificial, Name = "Tropicana Field"},
            new Venue{ Id = 26, Capacity = 50096, OpeningYear = 1996, SurfaceType = Surface.Grass, Name = "Turner Field"},
            new Venue{ Id = 27, Capacity = 40615, OpeningYear = 1991, SurfaceType = Surface.Grass, Name = "U.S. Cellular Field"},
            new Venue{ Id = 28, Capacity = 41160, OpeningYear = 1914, SurfaceType = Surface.Grass, Name = "Wrigley Field"},
            new Venue{ Id = 29, Capacity = 52325, OpeningYear = 2009, SurfaceType = Surface.Grass, Name = "Yankee Stadium"},
            //NFL
            new Venue{ Id = 30, Capacity = 85000, OpeningYear = 1997, SurfaceType = Surface.Grass, Name = "FedExField", RoofType = RoofType.Open},
            new Venue{ Id = 31, Capacity = 82566, OpeningYear = 2010, SurfaceType = Surface.Artificial, Name = "MetLife Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 32, Capacity = 80750, OpeningYear = 1957, SurfaceType = Surface.Grass, Name = "Lambeau Field", RoofType = RoofType.Open},
            new Venue{ Id = 33, Capacity = 80000, OpeningYear = 2009, SurfaceType = Surface.Artificial, Name = "AT&T Stadium", RoofType = RoofType.Retractable},
            new Venue{ Id = 34, Capacity = 76416, OpeningYear = 1972, SurfaceType = Surface.Grass, Name = "Arrowhead Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 35, Capacity = 76125, OpeningYear = 2001, SurfaceType = Surface.Grass, Name = "Sports Authority Field at Mile High", RoofType = RoofType.Open},
            new Venue{ Id = 36, Capacity = 75540, OpeningYear = 1987, SurfaceType = Surface.Grass, Name = "Sun Life Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 37, Capacity = 73778, OpeningYear = 1996, SurfaceType = Surface.Grass, Name = "Bank of America Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 38, Capacity = 73208, OpeningYear = 1975, SurfaceType = Surface.Artificial, Name = "Mercedes-Benz Superdome", RoofType = RoofType.Dome},
            new Venue{ Id = 39, Capacity = 73200, OpeningYear = 1999, SurfaceType = Surface.Grass, Name = "First Energy Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 40, Capacity = 73079, OpeningYear = 1973, SurfaceType = Surface.Grass, Name = "Ralph Wilson Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 41, Capacity = 73079, OpeningYear = 1992, SurfaceType = Surface.Artificial, Name = "Georgia Dome", RoofType = RoofType.Dome},
            new Venue{ Id = 42, Capacity = 73079, OpeningYear = 2002, SurfaceType = Surface.Grass, Name = "Reliant Stadium", RoofType = RoofType.Retractable},
            new Venue{ Id = 43, Capacity = 73079, OpeningYear = 1998, SurfaceType = Surface.Artificial, Name = "M&T Bank Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 44, Capacity = 73079, OpeningYear = 1967, SurfaceType = Surface.Grass, Name = "Qualcomm Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 45, Capacity = 73079, OpeningYear = 1999, SurfaceType = Surface.Grass, Name = "LP Field", RoofType = RoofType.Open},
            new Venue{ Id = 46, Capacity = 73079, OpeningYear = 2002, SurfaceType = Surface.Artificial, Name = "Gillette Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 47, Capacity = 73079, OpeningYear = 2003, SurfaceType = Surface.Grass, Name = "Lincoln Financial Field", RoofType = RoofType.Open},
            new Venue{ Id = 48, Capacity = 73079, OpeningYear = 2014, SurfaceType = Surface.Grass, Name = "Levi's Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 49, Capacity = 73079, OpeningYear = 1995, SurfaceType = Surface.Grass, Name = "EverBank Field", RoofType = RoofType.Open},
            new Venue{ Id = 50, Capacity = 73079, OpeningYear = 2002, SurfaceType = Surface.Grass, Name = "CenturyLink Field", RoofType = RoofType.Open},
            new Venue{ Id = 51, Capacity = 73079, OpeningYear = 1995, SurfaceType = Surface.Artificial, Name = "Edward Jones Dome", RoofType = RoofType.Dome},
            new Venue{ Id = 52, Capacity = 73079, OpeningYear = 1998, SurfaceType = Surface.Grass, Name = "Raymond James Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 53, Capacity = 73079, OpeningYear = 2000, SurfaceType = Surface.Artificial, Name = "Paul Brown Stadium", RoofType = RoofType.Open},
            new Venue{ Id = 54, Capacity = 73079, OpeningYear = 2001, SurfaceType = Surface.Grass, Name = "Heinz Field", RoofType = RoofType.Open},
            new Venue{ Id = 55, Capacity = 73079, OpeningYear = 2002, SurfaceType = Surface.Artificial, Name = "Ford Field", RoofType = RoofType.Dome},
            new Venue{ Id = 56, Capacity = 73079, OpeningYear = 2006, SurfaceType = Surface.Grass, Name = "University of Phoenix Stadium", RoofType = RoofType.Retractable},
            new Venue{ Id = 57, Capacity = 73079, OpeningYear = 2008, SurfaceType = Surface.Artificial, Name = "Lucas Oil Stadium", RoofType = RoofType.Retractable},
            new Venue{ Id = 58, Capacity = 73079, OpeningYear = 1924, SurfaceType = Surface.Grass, Name = "Soldier Field", RoofType = RoofType.Open},
            new Venue{ Id = 59, Capacity = 73079, OpeningYear = 2009, SurfaceType = Surface.Artificial, Name = "TCF Bank Stadium", RoofType = RoofType.Open},
            //NHL
            new Venue{ Id = 60, Capacity = 18819, OpeningYear = 1999, Name = "Air Canada Centre"},
            new Venue{ Id = 61, Capacity = 18532, OpeningYear = 2001, Name = "American Airlines Center"},
            new Venue{ Id = 62, Capacity = 19250, OpeningYear = 1998, Name = "BB&T Center"},
            new Venue{ Id = 63, Capacity = 21273, OpeningYear = 1996, Name = "Bell Center"},
            new Venue{ Id = 64, Capacity = 17113, OpeningYear = 1996, Name = "Bridgestone Arena"},
            new Venue{ Id = 65, Capacity = 19153, OpeningYear = 1996, Name = "Canadian Tire Centre"},
            new Venue{ Id = 66, Capacity = 18387, OpeningYear = 2010, Name = "Consol Energy Center"},
            new Venue{ Id = 67, Capacity = 19070, OpeningYear = 1996, Name = "First Niagara Center"},
            new Venue{ Id = 68, Capacity = 17174, OpeningYear = 1993, Name = "Honda Center"},
            new Venue{ Id = 69, Capacity = 17125, OpeningYear = 2003, Name = "Jobing.com Center"},
            new Venue{ Id = 70, Capacity = 20066, OpeningYear = 1979, Name = "Joe Louis Arena"},
            new Venue{ Id = 71, Capacity = 18006, OpeningYear = 1968, Name = "Madison Square Garden"},
            new Venue{ Id = 72, Capacity = 15004, OpeningYear = 2004, Name = "MTS Centre"},
            new Venue{ Id = 73, Capacity = 16170, OpeningYear = 1972, Name = "Nassau Veterans Memorial Coliseum"},
            new Venue{ Id = 74, Capacity = 18144, OpeningYear = 2000, Name = "Nationwide Arena"},
            new Venue{ Id = 75, Capacity = 18007, OpeningYear = 1999, Name = "Pepsi Center"},
            new Venue{ Id = 76, Capacity = 18680, OpeningYear = 1999, Name = "PNC Arena"},
            new Venue{ Id = 77, Capacity = 17625, OpeningYear = 2007, Name = "Prudential Center"},
            new Venue{ Id = 78, Capacity = 16839, OpeningYear = 1974, Name = "Rexall Place"},
            new Venue{ Id = 79, Capacity = 18910, OpeningYear = 1995, Name = "Rogers Arena"},
            new Venue{ Id = 80, Capacity = 19289, OpeningYear = 1983, Name = "Scotiabank Saddledome"},
            new Venue{ Id = 81, Capacity = 17562, OpeningYear = 1993, Name = "SAP Center at San Jose"},
            new Venue{ Id = 82, Capacity = 20082, OpeningYear = 1994, Name = "Scottrade Center"},
            new Venue{ Id = 83, Capacity = 18867, OpeningYear = 1999, Name = "Staples Center"},
            new Venue{ Id = 84, Capacity = 19204, OpeningYear = 1996, Name = "Tampa Bay Times Forum"},
            new Venue{ Id = 85, Capacity = 17565, OpeningYear = 1995, Name = "TD Garden"},
            new Venue{ Id = 86, Capacity = 19717, OpeningYear = 1994, Name = "United Center"},
            new Venue{ Id = 87, Capacity = 18506, OpeningYear = 1997, Name = "Verizon Center"},
            new Venue{ Id = 88, Capacity = 19537, OpeningYear = 1996, Name = "Wells Fargo Center"},
            new Venue{ Id = 89, Capacity = 17954, OpeningYear = 2000, Name = "Xcel Energy Center"},
            //NBA
            new Venue{ Id = 90, Capacity = 19600, OpeningYear = 2000, Name = "American Airlines Arena"},
            new Venue{ Id = 91, Capacity = 18846, OpeningYear = 2010, Name = "Amway Center"},
            new Venue{ Id = 92, Capacity = 18581, OpeningYear = 2002, Name = "AT&T Center"},
            new Venue{ Id = 93, Capacity = 18165, OpeningYear = 1999, Name = "Bankers Life Fieldhouse"},
            new Venue{ Id = 94, Capacity = 17732, OpeningYear = 2012, Name = "Barclays Center"},
            new Venue{ Id = 95, Capacity = 18717, OpeningYear = 1988, Name = "BMO Harris Bradley Center"},
            new Venue{ Id = 96, Capacity = 18203, OpeningYear = 2002, Name = "Chesapeake Energy Arena"},
            new Venue{ Id = 97, Capacity = 19911, OpeningYear = 1991, Name = "EnergySolutions Arena"},
            new Venue{ Id = 98, Capacity = 18119, OpeningYear = 2004, Name = "FedExForum"},
            new Venue{ Id = 99, Capacity = 19980, OpeningYear = 1995, Name = "Moda Center"},
            new Venue{ Id = 100,Capacity = 19596, OpeningYear = 1966, Name = "Oracle Arena"},
            new Venue{ Id = 101,Capacity = 18118, OpeningYear = 1999, Name = "Philips Arena"},
            new Venue{ Id = 102,Capacity = 20562, OpeningYear = 1994, Name = "Quicken Loans Arena"},
            new Venue{ Id = 103,Capacity = 17317, OpeningYear = 1988, Name = "Sleep Train Arena"},
            new Venue{ Id = 104,Capacity = 17003, OpeningYear = 1999, Name = "Smoothie King Center"},
            new Venue{ Id = 105,Capacity = 19356, OpeningYear = 1990, Name = "Target Center"},
            new Venue{ Id = 106,Capacity = 22076, OpeningYear = 1988, Name = "The Palace of Auburn Hills"},
            new Venue{ Id = 107,Capacity = 19077, OpeningYear = 2005, Name = "Time Warner Cable Arena"},
            new Venue{ Id = 108,Capacity = 18023, OpeningYear = 2003, Name = "Toyota Center"},
            new Venue{ Id = 109,Capacity = 18422, OpeningYear = 1992, Name = "US Airways Center"},
            //MLS
            new Venue{ Id = 110,Capacity = 22000, OpeningYear = 2012, Name = "BBVA Compass Stadium"},
            new Venue{ Id = 111,Capacity = 21000, OpeningYear = 1983, Name = "BC Place"},
            new Venue{ Id = 112,Capacity = 21859, OpeningYear = 2007, Name = "BMO Field"},
            new Venue{ Id = 113,Capacity = 10525, OpeningYear = 1962, Name = "Buck Shaw Stadium"},
            new Venue{ Id = 114,Capacity = 20145, OpeningYear = 1999, Name = "Columbus Crew Stadium"},
            new Venue{ Id = 115,Capacity = 17424, OpeningYear = 2007, Name = "Dick's Sporting Goods Park"},
            new Venue{ Id = 116,Capacity = 18500, OpeningYear = 2010, Name = "PPL Park"},
            new Venue{ Id = 117,Capacity = 20674, OpeningYear = 1926, Name = "Providence Park"},
            new Venue{ Id = 118,Capacity = 19647, OpeningYear = 1961, Name = "RFK Stadium"},
            new Venue{ Id = 119,Capacity = 25000, OpeningYear = 2010, Name = "Red Bull Arena"},
            new Venue{ Id = 120,Capacity = 20213, OpeningYear = 2008, Name = "Rio Tinto Stadium"},
            new Venue{ Id = 121,Capacity = 20521, OpeningYear = 2008, Name = "Saputo Stadium"},
            new Venue{ Id = 122,Capacity = 18467, OpeningYear = 2011, Name = "Sporting Park"},
            new Venue{ Id = 123,Capacity = 27000, OpeningYear = 2003, Name = "StubHub Center"},
            new Venue{ Id = 124,Capacity = 20000, OpeningYear = 2006, Name = "Toyota Park"},
            new Venue{ Id = 125,Capacity = 20500, OpeningYear = 2005, Name = "Toyota Stadium"},
            //FAPL
            new Venue{ Id = 126,Capacity = 45362, OpeningYear = 1884, Name = "Anfield", Latitude=53.430833, Longitude=-2.960833, City="Liverpool"},
            new Venue{ Id = 127,Capacity = 35303, OpeningYear = 1904, Name = "Boleyn Ground", Latitude=51.531944, Longitude=0.039444, City="London"},
            new Venue{ Id = 128,Capacity = 28383, OpeningYear = 1997, Name = "Brittania Stadium", Latitude=52.988333, Longitude=-2.175556, City="Stoke-on-Trent"},
            new Venue{ Id = 129,Capacity = 26828, OpeningYear = 2009, Name = "Cardiff City Stadium", Latitude=0, Longitude=0, City="Cardiff"},
            new Venue{ Id = 130,Capacity = 27033, OpeningYear = 1935, Name = "Carrow Road", Latitude=0, Longitude=0, City="Norwich"},
            new Venue{ Id = 131,Capacity = 47726, OpeningYear = 2003, Name = "City of Manchester Stadium", Latitude=0, Longitude=0, City="Manchester"},
            new Venue{ Id = 132,Capacity = 25700, OpeningYear = 1896, Name = "Craven Cottage", Latitude=0, Longitude=0, City="London"},
            new Venue{ Id = 133,Capacity = 60355, OpeningYear = 2006, Name = "Emirates Stadium", Latitude=0, Longitude=0, City="London"},
            new Venue{ Id = 134,Capacity = 40157, OpeningYear = 1892, Name = "Goodison Park", Latitude=0, Longitude=0, City="Liverpool"},
            new Venue{ Id = 135,Capacity = 26500, OpeningYear = 1900, Name = "The Hawthorns", Latitude=0, Longitude=0, City="West Bromwich"},
            new Venue{ Id = 136,Capacity = 25404, OpeningYear = 2002, Name = "KC Stadium", Latitude=0, Longitude=0, City="Kingston upon Hull"},
            new Venue{ Id = 137,Capacity = 20532, OpeningYear = 2005, Name = "Liberty Stadium", Latitude=0, Longitude=0, City="Swansea"},
            new Venue{ Id = 138,Capacity = 76212, OpeningYear = 1910, Name = "Old Trafford", Latitude=0, Longitude=0, City="Manchester"},
            new Venue{ Id = 139,Capacity = 52387, OpeningYear = 1880, Name = "St. James' Park", Latitude=0, Longitude=0, City="Newcastle"},
            new Venue{ Id = 140,Capacity = 32689, OpeningYear = 2001, Name = "St. Mary's Stadium", Latitude=0, Longitude=0, City="Southampton"},
            new Venue{ Id = 141,Capacity = 26309, OpeningYear = 1924, Name = "Selhurst Park", Latitude=0, Longitude=0, City="London"},
            new Venue{ Id = 142,Capacity = 49000, OpeningYear = 1997, Name = "Stadium of Light", Latitude=0, Longitude=0, City="Sunderland"},
            new Venue{ Id = 143,Capacity = 42055, OpeningYear = 1877, Name = "Stamford Bridge", Latitude=0, Longitude=0, City="London"},
            new Venue{ Id = 144,Capacity = 42788, OpeningYear = 1897, Name = "Villa Park", Latitude=0, Longitude=0, City="Birmingham"},
            new Venue{ Id = 145,Capacity = 36310, OpeningYear = 1899, Name = "White Hart Lane", Latitude=0, Longitude=0, City="London"},
        };

        public static List<Team> MLB = new List<Team>
        { 
            new Team { HomeVenue = Venues[03], League = "MLB", Abbreviation = "ARI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Arizona", Mascot = "Diamondbacks" },
            new Team { HomeVenue = Venues[26], League = "MLB", Abbreviation = "ATL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Atlanta", Mascot = "Braves" },
            new Team { HomeVenue = Venues[18], League = "MLB", Abbreviation = "BAL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Baltimore", Mascot = "Orioles" },
            new Team { HomeVenue = Venues[09], League = "MLB", Abbreviation = "BOS", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Boston", Mascot = "Red Sox" },
            new Team { HomeVenue = Venues[28], League = "MLB", Abbreviation = "CHC", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Chicago", Mascot = "Cubs" },
            new Team { HomeVenue = Venues[27], League = "MLB", Abbreviation = "CHW", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Chicago", Mascot = "White Sox" },
            new Team { HomeVenue = Venues[11], League = "MLB", Abbreviation = "CIN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Cincinnati", Mascot = "Reds" },
            new Team { HomeVenue = Venues[21], League = "MLB", Abbreviation = "CLE", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Cleveland", Mascot = "Indians" },
            new Team { HomeVenue = Venues[07], League = "MLB", Abbreviation = "COL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Colorado", Mascot = "Rockies" },
            new Team { HomeVenue = Venues[06], League = "MLB", Abbreviation = "DET", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Detroit", Mascot = "Tigers" },
            new Team { HomeVenue = Venues[15], League = "MLB", Abbreviation = "HOU", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Houston", Mascot = "Astros" },
            new Team { HomeVenue = Venues[12], League = "MLB", Abbreviation = "KC",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Kansas City", Mascot = "Royals" },
            new Team { HomeVenue = Venues[00], League = "MLB", Abbreviation = "LAA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Los Angeles", Mascot = "Angels" },
            new Team { HomeVenue = Venues[08], League = "MLB", Abbreviation = "LAD", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Los Angeles", Mascot = "Dodgers" },
            new Team { HomeVenue = Venues[13], League = "MLB", Abbreviation = "MIA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Miami", Mascot = "Marlins" },
            new Team { HomeVenue = Venues[02], League = "MLB", Abbreviation = "MIL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Milwaukee", Mascot = "Brewers" },
            new Team { HomeVenue = Venues[24], League = "MLB", Abbreviation = "MIN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Minnesota", Mascot = "Twins" },
            new Team { HomeVenue = Venues[04], League = "MLB", Abbreviation = "NYM", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New York", Mascot = "Mets" },
            new Team { HomeVenue = Venues[29], League = "MLB", Abbreviation = "NYY", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New York", Mascot = "Yankees" },
            new Team { HomeVenue = Venues[17], League = "MLB", Abbreviation = "OAK", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Oakland", Mascot = "Athletics" },
            new Team { HomeVenue = Venues[05], League = "MLB", Abbreviation = "PHI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Philadelphia", Mascot = "Phillies" },
            new Team { HomeVenue = Venues[20], League = "MLB", Abbreviation = "PIT", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Pittsburgh", Mascot = "Pirates" },
            new Team { HomeVenue = Venues[19], League = "MLB", Abbreviation = "SD",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "San Diego", Mascot = "Padres" },
            new Team { HomeVenue = Venues[01], League = "MLB", Abbreviation = "SF",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "San Francisco", Mascot = "Giants" },
            new Team { HomeVenue = Venues[23], League = "MLB", Abbreviation = "SEA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Seattle", Mascot = "Mariners" },
            new Team { HomeVenue = Venues[02], League = "MLB", Abbreviation = "STL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "St. Louis", Mascot = "Cardinals" },
            new Team { HomeVenue = Venues[25], League = "MLB", Abbreviation = "TB",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Tampa Bay", Mascot = "Rays" },
            new Team { HomeVenue = Venues[10], League = "MLB", Abbreviation = "TEX", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Texas", Mascot = "Rangers" },
            new Team { HomeVenue = Venues[22], League = "MLB", Abbreviation = "TOR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Toronto", Mascot = "Blue Jays" },
            new Team { HomeVenue = Venues[16], League = "MLB", Abbreviation = "WAS", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Washington", Mascot = "Nationals" }
        };

        public static List<Team> NFL = new List<Team>
        { 
            new Team { HomeVenue = Venues[56], League = "NFL", Abbreviation = "ARI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Arizona", Mascot = "Cardinals" },
            new Team { HomeVenue = Venues[41], League = "NFL", Abbreviation = "ATL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Atlanta", Mascot = "Falcons" },
            new Team { HomeVenue = Venues[43], League = "NFL", Abbreviation = "BAL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Baltimore", Mascot = "Ravens" },
            new Team { HomeVenue = Venues[40], League = "NFL", Abbreviation = "BUF", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Buffalo", Mascot = "Bills" },
            new Team { HomeVenue = Venues[37], League = "NFL", Abbreviation = "CAR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Carolina", Mascot = "Panthers" },
            new Team { HomeVenue = Venues[58], League = "NFL", Abbreviation = "CHI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Chicago", Mascot = "Bears" },
            new Team { HomeVenue = Venues[53], League = "NFL", Abbreviation = "CIN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Cincinnati", Mascot = "Bengals" },
            new Team { HomeVenue = Venues[39], League = "NFL", Abbreviation = "CLE", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Cleveland", Mascot = "Browns" },
            new Team { HomeVenue = Venues[33], League = "NFL", Abbreviation = "DAL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Dallas", Mascot = "Cowboys" },
            new Team { HomeVenue = Venues[35], League = "NFL", Abbreviation = "DEN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Denver", Mascot = "Broncos" },
            new Team { HomeVenue = Venues[55], League = "NFL", Abbreviation = "DET", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Detroit", Mascot = "Lions" },
            new Team { HomeVenue = Venues[32], League = "NFL", Abbreviation = "GB",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Green Bay", Mascot = "Packers" },
            new Team { HomeVenue = Venues[42], League = "NFL", Abbreviation = "HOU", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Houston", Mascot = "Texans" },
            new Team { HomeVenue = Venues[57], League = "NFL", Abbreviation = "IND", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Indianapolis", Mascot = "Colts" },
            new Team { HomeVenue = Venues[49], League = "NFL", Abbreviation = "JAC", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Jacksonville", Mascot = "Jaguars" },
            new Team { HomeVenue = Venues[34], League = "NFL", Abbreviation = "KC",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Kansas City", Mascot = "Chiefs" },
            new Team { HomeVenue = Venues[36], League = "NFL", Abbreviation = "MIA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Miami", Mascot = "Dolphins" },
            new Team { HomeVenue = Venues[59], League = "NFL", Abbreviation = "MIN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Minnesota", Mascot = "Vikings" },
            new Team { HomeVenue = Venues[46], League = "NFL", Abbreviation = "NE",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New England", Mascot = "Patriots" },
            new Team { HomeVenue = Venues[38], League = "NFL", Abbreviation = "NO",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New Orleans", Mascot = "Saints" },
            new Team { HomeVenue = Venues[31], League = "NFL", Abbreviation = "NYG", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New York", Mascot = "Giants" },
            new Team { HomeVenue = Venues[31], League = "NFL", Abbreviation = "NYJ", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New York", Mascot = "Jets" },
            new Team { HomeVenue = Venues[17], League = "NFL", Abbreviation = "OAK", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Oakland", Mascot = "Raiders" },
            new Team { HomeVenue = Venues[47], League = "NFL", Abbreviation = "PHI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Philadelphia", Mascot = "Eagles" },
            new Team { HomeVenue = Venues[54], League = "NFL", Abbreviation = "PIT", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Pittsburgh", Mascot = "Steelers" },
            new Team { HomeVenue = Venues[44], League = "NFL", Abbreviation = "SD",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "San Diego", Mascot = "Chargers" },
            new Team { HomeVenue = Venues[50], League = "NFL", Abbreviation = "SEA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Seattle", Mascot = "Seahawks" },
            new Team { HomeVenue = Venues[48], League = "NFL", Abbreviation = "SF",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "San Francisco", Mascot = "49ers" },
            new Team { HomeVenue = Venues[51], League = "NFL", Abbreviation = "STL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "St. Louis", Mascot = "Rams" },
            new Team { HomeVenue = Venues[52], League = "NFL", Abbreviation = "TB",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Tampa Bay", Mascot = "Buccaneers" },
            new Team { HomeVenue = Venues[45], League = "NFL", Abbreviation = "TEN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Tennessee", Mascot = "Titans" },
            new Team { HomeVenue = Venues[30], League = "NFL", Abbreviation = "WAS", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Washington", Mascot = "Redskins" }
        };

        public static List<Team> NHL = new List<Team>
        { 
            new Team { HomeVenue = Venues[68], League = "NHL", Abbreviation = "ANA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Anaheim", Mascot = "Ducks" },
            new Team { HomeVenue = Venues[85], League = "NHL", Abbreviation = "BOS", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Boston", Mascot = "Bruins" },
            new Team { HomeVenue = Venues[67], League = "NHL", Abbreviation = "BUF", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Buffalo", Mascot = "Sabres" },
            new Team { HomeVenue = Venues[80], League = "NHL", Abbreviation = "CAL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Calgary", Mascot = "Flames" },
            new Team { HomeVenue = Venues[76], League = "NHL", Abbreviation = "CAR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Carolina", Mascot = "Hurricanes" },
            new Team { HomeVenue = Venues[74], League = "NHL", Abbreviation = "CBJ", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Columbus", Mascot = "Blue Jackets" },
            new Team { HomeVenue = Venues[86], League = "NHL", Abbreviation = "CHI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Chicago", Mascot = "Blackhawks" },
            new Team { HomeVenue = Venues[75], League = "NHL", Abbreviation = "COL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Colorado", Mascot = "Avalanche" },
            new Team { HomeVenue = Venues[61], League = "NHL", Abbreviation = "DAL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Dallas", Mascot = "Stars" },
            new Team { HomeVenue = Venues[70], League = "NHL", Abbreviation = "DET", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Detroit", Mascot = "Red Wings" },
            new Team { HomeVenue = Venues[78], League = "NHL", Abbreviation = "EDM", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Edmonton", Mascot = "Oilers" },
            new Team { HomeVenue = Venues[62], League = "NHL", Abbreviation = "FLA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Florida", Mascot = "Panthers" },
            new Team { HomeVenue = Venues[83], League = "NHL", Abbreviation = "LA",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Los Angeles", Mascot = "Kings" },
            new Team { HomeVenue = Venues[89], League = "NHL", Abbreviation = "MIN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Minnesota", Mascot = "Wild" },
            new Team { HomeVenue = Venues[63], League = "NHL", Abbreviation = "MON", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Montral", Mascot = "Canadiens" },
            new Team { HomeVenue = Venues[77], League = "NHL", Abbreviation = "NJ",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New Jersey", Mascot = "Devils" },
            new Team { HomeVenue = Venues[64], League = "NHL", Abbreviation = "NSH", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Nashville", Mascot = "Predators" },
            new Team { HomeVenue = Venues[73], League = "NHL", Abbreviation = "NYI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New York", Mascot = "Islanders" },
            new Team { HomeVenue = Venues[71], League = "NHL", Abbreviation = "NYR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New York", Mascot = "Rangers" },
            new Team { HomeVenue = Venues[65], League = "NHL", Abbreviation = "OTT", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Ottawa", Mascot = "Senators" },
            new Team { HomeVenue = Venues[88], League = "NHL", Abbreviation = "PHI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Philadelphia", Mascot = "Flyers" },
            new Team { HomeVenue = Venues[69], League = "NHL", Abbreviation = "PHX", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Phoenix", Mascot = "Coyotes" },
            new Team { HomeVenue = Venues[66], League = "NHL", Abbreviation = "PIT", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Pittsburgh", Mascot = "Penguins" },
            new Team { HomeVenue = Venues[81], League = "NHL", Abbreviation = "SJ",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "San Jose", Mascot = "Sharks" },
            new Team { HomeVenue = Venues[82], League = "NHL", Abbreviation = "STL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "St. Louis", Mascot = "Blues" },
            new Team { HomeVenue = Venues[84], League = "NHL", Abbreviation = "TB",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Tampa Bay", Mascot = "Lightning" },
            new Team { HomeVenue = Venues[60], League = "NHL", Abbreviation = "TOR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Toronto", Mascot = "Maple Leafs" },
            new Team { HomeVenue = Venues[79], League = "NHL", Abbreviation = "VAN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Vancouver", Mascot = "Canucks" },
            new Team { HomeVenue = Venues[87], League = "NHL", Abbreviation = "WAS", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Washington", Mascot = "Capitals" },
            new Team { HomeVenue = Venues[72], League = "NHL", Abbreviation = "WIN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Winnipeg", Mascot = "Jets" }
        };

        public static List<Team> MLS = new List<Team>
        { 
            new Team { HomeVenue = Venues[124],League = "MLS", Abbreviation = "CHI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Chicago", Mascot = "Fire" },
            new Team { HomeVenue = Venues[123],League = "MLS", Abbreviation = "CHV", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Chivas", Mascot = "USA" },
            new Team { HomeVenue = Venues[114],League = "MLS", Abbreviation = "CLB", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Columbus", Mascot = "Crew" },
            new Team { HomeVenue = Venues[115],League = "MLS", Abbreviation = "COL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Colorado", Mascot = "Rapids" },
            new Team { HomeVenue = Venues[125],League = "MLS", Abbreviation = "DAL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "FC", Mascot = "Dallas" },
            new Team { HomeVenue = Venues[118],League = "MLS", Abbreviation = "DC",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "D.C.", Mascot = "United" },
            new Team { HomeVenue = Venues[110],League = "MLS", Abbreviation = "HOU", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Houston", Mascot = "Dynamo" },
            new Team { HomeVenue = Venues[122],League = "MLS", Abbreviation = "KC",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Sporting", Mascot = "Kansas City" },
            new Team { HomeVenue = Venues[123],League = "MLS", Abbreviation = "LA",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Los Angeles", Mascot = "Galaxy" },
            new Team { HomeVenue = Venues[121],League = "MLS", Abbreviation = "MON", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Montreal", Mascot = "Impact" },
            new Team { HomeVenue = Venues[46], League = "MLS", Abbreviation = "NE",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New England", Mascot = "Revolution" },
            new Team { HomeVenue = Venues[119],League = "MLS", Abbreviation = "NY",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New York", Mascot = "Red Bulls" },
            new Team { HomeVenue = Venues[116],League = "MLS", Abbreviation = "PHI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Philadelphia Union", Mascot = "Angels" },
            new Team { HomeVenue = Venues[117],League = "MLS", Abbreviation = "POR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Portland", Mascot = "Timbers" },
            new Team { HomeVenue = Venues[120],League = "MLS", Abbreviation = "RSL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Real", Mascot = "Salt Lake" },
            new Team { HomeVenue = Venues[50], League = "MLS", Abbreviation = "SEA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Seattle", Mascot = "Sounders" },
            new Team { HomeVenue = Venues[113],League = "MLS", Abbreviation = "SJ",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "San Jose", Mascot = "Earthquakes" },
            new Team { HomeVenue = Venues[112],League = "MLS", Abbreviation = "TOR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Toronto", Mascot = "FC" },
            new Team { HomeVenue = Venues[111],League = "MLS", Abbreviation = "VAN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Vancouver", Mascot = "Whitecaps FC" }
        };

        public static List<Team> NBA = new List<Team> 
        { 
            new Team { HomeVenue = Venues[101],League = "NBA", Abbreviation = "ATL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Atlanta", Mascot = "Hawks" },
            new Team { HomeVenue = Venues[94], League = "NBA", Abbreviation = "BKN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Brooklyn", Mascot = "Nets" },
            new Team { HomeVenue = Venues[85], League = "NBA", Abbreviation = "BOS", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Boston", Mascot = "Celtics" },
            new Team { HomeVenue = Venues[107],League = "NBA", Abbreviation = "CHA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Charlotte", Mascot = "Bobcats" },
            new Team { HomeVenue = Venues[86], League = "NBA", Abbreviation = "CHI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Chicago", Mascot = "Bulls" },
            new Team { HomeVenue = Venues[102],League = "NBA", Abbreviation = "CLE", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Cleveland", Mascot = "Cavaliers" },
            new Team { HomeVenue = Venues[61], League = "NBA", Abbreviation = "DAL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Dallas", Mascot = "Mavericks" },
            new Team { HomeVenue = Venues[75], League = "NBA", Abbreviation = "DEN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Denver", Mascot = "Nuggets" },
            new Team { HomeVenue = Venues[106],League = "NBA", Abbreviation = "DET", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Detroit", Mascot = "Pistons" },
            new Team { HomeVenue = Venues[100],League = "NBA", Abbreviation = "GS",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Golden State", Mascot = "Warriors" },
            new Team { HomeVenue = Venues[108],League = "NBA", Abbreviation = "HOU", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Houston", Mascot = "Rockets" },
            new Team { HomeVenue = Venues[93], League = "NBA", Abbreviation = "IND", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Indiana", Mascot = "Pacers" },
            new Team { HomeVenue = Venues[83], League = "NBA", Abbreviation = "LAC", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Los Angeles", Mascot = "Clippers" },
            new Team { HomeVenue = Venues[83], League = "NBA", Abbreviation = "LAL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Los Angeles", Mascot = "Lakers" },
            new Team { HomeVenue = Venues[98], League = "NBA", Abbreviation = "MEM", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Memphis", Mascot = "Grizzlies" },
            new Team { HomeVenue = Venues[90], League = "NBA", Abbreviation = "MIA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Miami", Mascot = "Heat" },
            new Team { HomeVenue = Venues[95], League = "NBA", Abbreviation = "MIL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Milwaukee", Mascot = "Bucks" },
            new Team { HomeVenue = Venues[105],League = "NBA", Abbreviation = "MIN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Minnesota", Mascot = "Timberwolves" },
            new Team { HomeVenue = Venues[104],League = "NBA", Abbreviation = "NO",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New Orleans", Mascot = "Pelicans" },
            new Team { HomeVenue = Venues[71], League = "NBA", Abbreviation = "NYK", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "New York", Mascot = "Knicks" },
            new Team { HomeVenue = Venues[96], League = "NBA", Abbreviation = "OKC", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Oklahoma City", Mascot = "Thunder" },
            new Team { HomeVenue = Venues[91], League = "NBA", Abbreviation = "ORL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Orlando", Mascot = "Magic" },
            new Team { HomeVenue = Venues[88], League = "NBA", Abbreviation = "PHI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Philadelphia", Mascot = "76ers" },
            new Team { HomeVenue = Venues[109],League = "NBA", Abbreviation = "PHX", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Phoenix", Mascot = "Suns" },
            new Team { HomeVenue = Venues[99], League = "NBA", Abbreviation = "POR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Portland", Mascot = "Trailblazers" },
            new Team { HomeVenue = Venues[92], League = "NBA", Abbreviation = "SA",  PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "San Antonio", Mascot = "Spurs" },
            new Team { HomeVenue = Venues[103],League = "NBA", Abbreviation = "SAC", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Sacramento", Mascot = "Kings" },
            new Team { HomeVenue = Venues[60], League = "NBA", Abbreviation = "TOR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Toronto", Mascot = "Raptors" },
            new Team { HomeVenue = Venues[97], League = "NBA", Abbreviation = "UTAH",PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Utah", Mascot = "Jazz" },
            new Team { HomeVenue = Venues[87], League = "NBA", Abbreviation = "WAS", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Washington", Mascot = "Wizards" }
        };

        public static List<Team> MiLB = new List<Team>
        {
            //new Team { HomeVenue = Venues[00], League = "MiLB", Abbreviation = "ATL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Atlanta", Mascot = "Hawks" },
        };

        //http://en.wikipedia.org/wiki/List_of_Premier_League_clubs
        public static List<Team> FAPL = new List<Team>
        {
            new Team { HomeVenue = Venues[133],League = "FAPL", Abbreviation = "ARS", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Arsenal", Mascot = "" },
            new Team { HomeVenue = Venues[143],League = "FAPL", Abbreviation = "CHE", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Chelsea", Mascot = "" },
            new Team { HomeVenue = Venues[131],League = "FAPL", Abbreviation = "MCI", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Manchester City", Mascot = "" },
            new Team { HomeVenue = Venues[126],League = "FAPL", Abbreviation = "LIV", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Liverpool", Mascot = "" },
            new Team { HomeVenue = Venues[145],League = "FAPL", Abbreviation = "TOT", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Tottenham Hotspur", Mascot = "" },
            new Team { HomeVenue = Venues[134],League = "FAPL", Abbreviation = "EVE", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Everton", Mascot = "" },
            new Team { HomeVenue = Venues[138],League = "FAPL", Abbreviation = "MUN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Manchester United", Mascot = "" },
            new Team { HomeVenue = Venues[140],League = "FAPL", Abbreviation = "SOU", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Southampton", Mascot = "" },
            new Team { HomeVenue = Venues[139],League = "FAPL", Abbreviation = "NEW", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Newcastle United", Mascot = "" },
            new Team { HomeVenue = Venues[137],League = "FAPL", Abbreviation = "SWA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Swansea City", Mascot = "" },
            new Team { HomeVenue = Venues[127],League = "FAPL", Abbreviation = "WHU", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "West Ham United", Mascot = "" },
            new Team { HomeVenue = Venues[144],League = "FAPL", Abbreviation = "AVL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Aston Villa", Mascot = "" },
            new Team { HomeVenue = Venues[136],League = "FAPL", Abbreviation = "HUL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Hull City", Mascot = "" },
            new Team { HomeVenue = Venues[128],League = "FAPL", Abbreviation = "STK", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Stoke City", Mascot = "" },
            new Team { HomeVenue = Venues[141],League = "FAPL", Abbreviation = "CRY", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Crystal Palace", Mascot = "" },
            new Team { HomeVenue = Venues[130],League = "FAPL", Abbreviation = "NOR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Norwich City", Mascot = "" },
            new Team { HomeVenue = Venues[135],League = "FAPL", Abbreviation = "WBA", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "West Bromwich Albion", Mascot = "" },
            new Team { HomeVenue = Venues[142],League = "FAPL", Abbreviation = "SUN", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Sunderland", Mascot = "" },
            new Team { HomeVenue = Venues[129],League = "FAPL", Abbreviation = "CAR", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Cardiff City", Mascot = "" },
            new Team { HomeVenue = Venues[132],League = "FAPL", Abbreviation = "FUL", PrimaryColor = "", SecondaryColor = "", TertiaryColor = "", City = "Fulham", Mascot = "" }
        };

        internal static System.Windows.Media.Brush GetBackground(string league)
        {
            ImageBrush ib = new ImageBrush();
            Uri uri = new Uri("Assets/Backgrounds/" + league + ".png", UriKind.Relative);
            ImageSource imageSource = new BitmapImage(uri);
            ib.ImageSource = imageSource;
            ib.Opacity = .2;
            return ib;
        }
    }
}