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
        public static User Fan;
        public static Ticket TicketBuilder = new Ticket();

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
        public static List<Venue> Venues = new List<Venue> { 
                                                            new Venue{ Id = 0,  Capacity = 45050, OpeningYear = 1966, SurfaceType = Surface.Grass, Name = "Angel Stadium of Anaheim"}, 
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
                                                            new Venue{ Id = 24, Capacity = 39504, OpeningYear = 2010, SurfaceType = Surface.Grass, Name = "Target Field"},
                                                            new Venue{ Id = 25, Capacity = 43772, OpeningYear = 1990, SurfaceType = Surface.Artificial, Name = "Tropicana Field"},
                                                            new Venue{ Id = 26, Capacity = 50096, OpeningYear = 1996, SurfaceType = Surface.Grass, Name = "Turner Field"},
                                                            new Venue{ Id = 27, Capacity = 40615, OpeningYear = 1991, SurfaceType = Surface.Grass, Name = "U.S. Cellular Field"},
                                                            new Venue{ Id = 28, Capacity = 41160, OpeningYear = 1914, SurfaceType = Surface.Grass, Name = "Wrigley Field"},
                                                            new Venue{ Id = 29, Capacity = 52325, OpeningYear = 2009, SurfaceType = Surface.Grass, Name = "Yankee Stadium"}};

        public static List<Team> MLB = new List<Team> { 
                                          new Team { HomeVenue = Venues[03], League = "MLB", Abbreviation = "ARI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Arizona", Mascot = "Diamondbacks" },
                                          new Team { HomeVenue = Venues[26], League = "MLB", Abbreviation = "ATL", PrimaryColor = "01487E", SecondaryColor = "D60D39", TertiaryColor = "FFFFFF", City = "Atlanta", Mascot = "Braves" },
                                          new Team { HomeVenue = Venues[18], League = "MLB", Abbreviation = "BAL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Baltimore", Mascot = "Orioles" },
                                          new Team { HomeVenue = Venues[09], League = "MLB", Abbreviation = "BOS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Boston", Mascot = "Red Sox" },
                                          new Team { HomeVenue = Venues[28], League = "MLB", Abbreviation = "CHC", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "Cubs" },
                                          new Team { HomeVenue = Venues[27], League = "MLB", Abbreviation = "CHW", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "White Sox" },
                                          new Team { HomeVenue = Venues[11], League = "MLB", Abbreviation = "CIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cincinnati", Mascot = "Reds" },
                                          new Team { HomeVenue = Venues[21], League = "MLB", Abbreviation = "CLE", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cleveland", Mascot = "Indians" },
                                          new Team { HomeVenue = Venues[07], League = "MLB", Abbreviation = "COL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Colorado", Mascot = "Rockies" },
                                          new Team { HomeVenue = Venues[06], League = "MLB", Abbreviation = "DET", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Detroit", Mascot = "Tigers" },
                                          new Team { HomeVenue = Venues[15], League = "MLB", Abbreviation = "HOU", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Houston", Mascot = "Astros" },
                                          new Team { HomeVenue = Venues[12], League = "MLB", Abbreviation = "KC",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Kansas City", Mascot = "Royals" },
                                          new Team { HomeVenue = Venues[00], League = "MLB", Abbreviation = "LAA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Los Angeles", Mascot = "Angels" },
                                          new Team { HomeVenue = Venues[08], League = "MLB", Abbreviation = "LAD", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Los Angeles", Mascot = "Dodgers" },
                                          new Team { HomeVenue = Venues[13], League = "MLB", Abbreviation = "MIA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Miami", Mascot = "Marlins" },
                                          new Team { HomeVenue = Venues[02], League = "MLB", Abbreviation = "MIL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Milwaukee", Mascot = "Brewers" },
                                          new Team { HomeVenue = Venues[24], League = "MLB", Abbreviation = "MIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Minnesota", Mascot = "Twins" },
                                          new Team { HomeVenue = Venues[04], League = "MLB", Abbreviation = "NYM", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Mets" },
                                          new Team { HomeVenue = Venues[29], League = "MLB", Abbreviation = "NYY", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Yankees" },
                                          new Team { HomeVenue = Venues[17], League = "MLB", Abbreviation = "OAK", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Oakland", Mascot = "Athletics" },
                                          new Team { HomeVenue = Venues[05], League = "MLB", Abbreviation = "PHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Philadelphia", Mascot = "Phillies" },
                                          new Team { HomeVenue = Venues[20], League = "MLB", Abbreviation = "PIT", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Pittsburgh", Mascot = "Pirates" },
                                          new Team { HomeVenue = Venues[19], League = "MLB", Abbreviation = "SD",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Diego", Mascot = "Padres" },
                                          new Team { HomeVenue = Venues[01], League = "MLB", Abbreviation = "SF",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Francisco", Mascot = "Giants" },
                                          new Team { HomeVenue = Venues[23], League = "MLB", Abbreviation = "SEA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Seattle", Mascot = "Mariners" },
                                          new Team { HomeVenue = Venues[02], League = "MLB", Abbreviation = "STL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "St. Louis", Mascot = "Cardinals" },
                                          new Team { HomeVenue = Venues[25], League = "MLB", Abbreviation = "TB",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Tampa Bay", Mascot = "Rays" },
                                          new Team { HomeVenue = Venues[10], League = "MLB", Abbreviation = "TEX", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Texas", Mascot = "Rangers" },
                                          new Team { HomeVenue = Venues[22], League = "MLB", Abbreviation = "TOR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Toronto", Mascot = "Blue Jays" },
                                          new Team { HomeVenue = Venues[16], League = "MLB", Abbreviation = "WAS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Washington", Mascot = "Nationals" }};

        public static List<Team> NFL = new List<Team> { new Team { League = "NFL", Abbreviation = "ARI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Arizona", Mascot = "Cardinals" },
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
                                          new Team { League = "NFL", Abbreviation = "WAS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Washington", Mascot = "Redskins" }};

        public static List<Team> NHL = new List<Team> { new Team { League = "NHL", Abbreviation = "ANA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Arizona", Mascot = "Diamondbacks" },
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
                                          new Team { League = "NHL", Abbreviation = "TB",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "St. Louis", Mascot = "Cardinals" },
                                          new Team { League = "NHL", Abbreviation = "TOR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Tampa Bay", Mascot = "Rays" },
                                          new Team { League = "NHL", Abbreviation = "VAN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Texas", Mascot = "Rangers" },
                                          new Team { League = "NHL", Abbreviation = "WAS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Toronto", Mascot = "Blue Jays" },
                                          new Team { League = "NHL", Abbreviation = "WIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Washington", Mascot = "Nationals" }};

        public static List<Team> MLS = new List<Team> { new Team { League = "MLS", Abbreviation = "CHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Arizona", Mascot = "Diamondbacks" },
                                          new Team { League = "MLS", Abbreviation = "CHV", PrimaryColor = "01487E", SecondaryColor = "D60D39", TertiaryColor = "FFFFFF", City = "Atlanta", Mascot = "Braves" },
                                          new Team { League = "MLS", Abbreviation = "CLB", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Baltimore", Mascot = "Orioles" },
                                          new Team { League = "MLS", Abbreviation = "COL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Boston", Mascot = "Red Sox" },
                                          new Team { League = "MLS", Abbreviation = "DAL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "Cubs" },
                                          new Team { League = "MLS", Abbreviation = "DC",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "White Sox" },
                                          new Team { League = "MLS", Abbreviation = "HOU", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cincinnati", Mascot = "Reds" },
                                          new Team { League = "MLS", Abbreviation = "KC",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cleveland", Mascot = "Indians" },
                                          new Team { League = "MLS", Abbreviation = "LA",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Colorado", Mascot = "Rockies" },
                                          new Team { League = "MLS", Abbreviation = "MON", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Detroit", Mascot = "Tigers" },
                                          new Team { League = "MLS", Abbreviation = "NE",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Houston", Mascot = "Astros" },
                                          new Team { League = "MLS", Abbreviation = "NY",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Kansas City", Mascot = "Royals" },
                                          new Team { League = "MLS", Abbreviation = "PHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Los Angeles", Mascot = "Angels" },
                                          new Team { League = "MLS", Abbreviation = "POR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Los Angeles", Mascot = "Dodgers" },
                                          new Team { League = "MLS", Abbreviation = "RSL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Miami", Mascot = "Marlins" },
                                          new Team { League = "MLS", Abbreviation = "SEA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Milwaukee", Mascot = "Brewers" },
                                          new Team { League = "MLS", Abbreviation = "SJ",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Minnesota", Mascot = "Twins" },
                                          new Team { League = "MLS", Abbreviation = "TOR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Mets" },
                                          new Team { League = "MLS", Abbreviation = "VAN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Yankees" }};

        public static List<Team> NBA = new List<Team> { new Team { League = "NBA", Abbreviation = "ATL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Arizona", Mascot = "Cardinals" },
                                          new Team { League = "NBA", Abbreviation = "BKN", PrimaryColor = "01487E", SecondaryColor = "D60D39", TertiaryColor = "FFFFFF", City = "Atlanta", Mascot = "Falcons" },
                                          new Team { League = "NBA", Abbreviation = "BOS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Baltimore", Mascot = "Ravens" },
                                          new Team { League = "NBA", Abbreviation = "CHA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Buffalo", Mascot = "Bills" },
                                          new Team { League = "NBA", Abbreviation = "CHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Carolina", Mascot = "Panthers" },
                                          new Team { League = "NBA", Abbreviation = "CLE", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Chicago", Mascot = "Bears" },
                                          new Team { League = "NBA", Abbreviation = "DAL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cincinnati", Mascot = "Bengals" },
                                          new Team { League = "NBA", Abbreviation = "DEN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Cleveland", Mascot = "Browns" },
                                          new Team { League = "NBA", Abbreviation = "DET", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Dallas", Mascot = "Cowboys" },
                                          new Team { League = "NBA", Abbreviation = "GS",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Denver", Mascot = "Broncos" },
                                          new Team { League = "NBA", Abbreviation = "HOU", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Detroit", Mascot = "Lions" },
                                          new Team { League = "NBA", Abbreviation = "IND", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Green Bay", Mascot = "Packers" },
                                          new Team { League = "NBA", Abbreviation = "LAC", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Houston", Mascot = "Texans" },
                                          new Team { League = "NBA", Abbreviation = "LAL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Indianapolis", Mascot = "Colts" },
                                          new Team { League = "NBA", Abbreviation = "MEM", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Jacksonville", Mascot = "Jaguars" },
                                          new Team { League = "NBA", Abbreviation = "MIA", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Kansas City", Mascot = "Chiefs" },
                                          new Team { League = "NBA", Abbreviation = "MIL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Miami", Mascot = "Dolphins" },
                                          new Team { League = "NBA", Abbreviation = "MIN", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Minnesota", Mascot = "Vikings" },
                                          new Team { League = "NBA", Abbreviation = "NO",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New England", Mascot = "Patriots" },
                                          new Team { League = "NBA", Abbreviation = "NYK", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New Orleans", Mascot = "Saints" },
                                          new Team { League = "NBA", Abbreviation = "OKC", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Giants" },
                                          new Team { League = "NBA", Abbreviation = "ORL", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "New York", Mascot = "Jets" },
                                          new Team { League = "NBA", Abbreviation = "PHI", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Oakland", Mascot = "Raiders" },
                                          new Team { League = "NBA", Abbreviation = "PHX", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Philadelphia", Mascot = "Eagles" },
                                          new Team { League = "NBA", Abbreviation = "POR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Pittsburgh", Mascot = "Steelers" },
                                          new Team { League = "NBA", Abbreviation = "SA",  PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Diego", Mascot = "Chargers" },
                                          new Team { League = "NBA", Abbreviation = "SAC", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Seattle", Mascot = "Seahawks" },
                                          new Team { League = "NBA", Abbreviation = "TOR", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "San Francisco", Mascot = "49ers" },
                                          new Team { League = "NBA", Abbreviation = "UTAH",PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "St. Louis", Mascot = "Rams" },
                                          new Team { League = "NBA", Abbreviation = "WAS", PrimaryColor = "A71930", SecondaryColor = "DBCEAC", TertiaryColor = "000000", City = "Tampa Bay", Mascot = "Buccaneers" }};

        public static List<Team> MiLB = new List<Team>();
    }
}