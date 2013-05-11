// ---------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// ---------------------------------------------------------------------
namespace Boco.OnMyWay.App
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Windows;

    using Boco.OnMyWay.App.Helpers;
    using Boco.OnMyWay.App.Resources;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Maps.Services;
    using Microsoft.Phone.Maps.Toolkit;
    using Microsoft.Phone.Tasks;

    using Windows.Devices.Geolocation;

    /// <summary>
    /// The main page.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// The GeoCoordinateWatcher to monitor changes in position.
        /// </summary>
        private GeoCoordinateWatcher _watcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            InitializeMap();
            Loaded += OnMainPageLoad;
            DataContext = App.ViewModel;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Called when the page has loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMainPageLoad(object sender, RoutedEventArgs e)
        {
            _watcher.Start();
        }

        /// <summary>
        /// Initializes the map.
        /// </summary>
        private async void InitializeMap()
        {
            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High) { MovementThreshold = 10 };
            _watcher.PositionChanged += this.OnPositionChanged;
            _watcher.Start();

            var geoLocator = new Geolocator { DesiredAccuracyInMeters = 10 };

            try
            {
                Geoposition geoposition = await geoLocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10));
                var currentLocation = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
                UserLocationMarker = new UserLocationMarker();
                UserLocationMarker.GeoCoordinate = currentLocation;
                UserLocationMarker.Visibility = Visibility.Visible;

            }
            catch (Exception)
            {
                // Inform the user that the location cannot be determined.

                App.ViewModel.CurrentLocation = null;
            }
        }

        /// <summary>
        /// Called when the position of the device changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void OnPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            theMap.SetView(e.Position.Location, 13);
            App.ViewModel.CurrentLocation = e.Position.Location;
        }

        /// <summary>
        /// Handles the hold gensture on the map.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.GestureEventArgs"/> instance containing the event data.</param>
        private async void OnMapHold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedDestination = theMap.ConvertViewportPointToGeoCoordinate(e.GetPosition(theMap));

            var query = new RouteQuery();
            query.InitialHeadingInDegrees = App.ViewModel.CurrentLocation.Course;
            query.TravelMode = TravelMode.Driving;
            query.Waypoints = new List<GeoCoordinate> { App.ViewModel.CurrentLocation, selectedDestination };
            query.RouteOptimization = RouteOptimization.MinimizeTime;
            var result = await query.GetRouteAsync();
            App.ViewModel.DurationToDestination = result.EstimatedDuration;

            this.CreateSms();
        }

        /// <summary>
        /// Creates the SMS message.
        /// </summary>
        private void CreateSms()
        {
            SmsComposeTask smsTask = new SmsComposeTask();
            smsTask.Body = string.Format(AppResources.MessageFormat, FriendlyTimeHelper.TimeSpanToFriendlyTime(App.ViewModel.DurationToDestination));
            smsTask.Show();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}