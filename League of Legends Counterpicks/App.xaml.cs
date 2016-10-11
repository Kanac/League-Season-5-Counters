using League_of_Legends_Counterpicks.Common;
using League_of_Legends_Counterpicks.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using Windows.ApplicationModel.Background;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.ApplicationInsights;
using Windows.ApplicationModel.Store;
using System.Diagnostics;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;
using League_of_Legends_Counterpicks.DataModel;


// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace League_of_Legends_Counterpicks
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        public static Microsoft.WindowsAzure.MobileServices.MobileServiceClient MobileService = new Microsoft.WindowsAzure.MobileServices.MobileServiceClient(
        "https://leagueseason5counters.azure-mobile.net/",
        "iovrWRisXiqXDzBpSreDZoSWrCPskN14")
        {
            SerializerSettings = new MobileServiceJsonSerializerSettings()
              {
                  CamelCasePropertyNames = true
              }
        };

        // For in app purchases
        public static LicenseInformation licenseInformation;

        public static bool firstLoad = true;

        private TransitionCollection transitions;

        public static string AppId = "bf747944-c75c-4f2a-a027-7c159b32261d";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Enable Azure Application Insights
            WindowsAppInitializer.InitializeAsync();



            this.InitializeComponent();
            this.Suspending += this.OnSuspending;

            //Only for IAP simulation purposes
#if DEBUG
            licenseInformation = CurrentAppSimulator.LicenseInformation;
#else
            licenseInformation = CurrentApp.LicenseInformation;
#endif
            this.UnhandledException += this.Application_UnhandledException;
        }

        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine(e.Message);
            e.Handled = true;

        }
        
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active.
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page.
                rootFrame = new Frame();

                // Associate the frame with a SuspensionManager key.
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // TODO: Change this value to a cache size that is appropriate for your application.
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate.
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue.
                    }
                }

                // Place the frame in the current Window.
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter.
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active.
            Window.Current.Activate();

            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        public static async void IsInternetAvailable()
        {
            var profiles = NetworkInformation.GetConnectionProfiles();
            var internetProfile = NetworkInformation.GetInternetConnectionProfile();
            var isInternetEnabled = profiles.Any(s => s.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess)
                || (internetProfile != null
                        && internetProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
            if (!isInternetEnabled)
            {
                MessageDialog messageBox = new MessageDialog("Make sure your internet connection is working and try again!");
                await messageBox.ShowAsync();
                Application.Current.Exit();
            }

        }
    }
}
