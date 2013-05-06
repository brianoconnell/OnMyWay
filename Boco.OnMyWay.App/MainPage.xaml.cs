namespace Boco.OnMyWay.App
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Diagnostics;
    using System.Windows;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Maps.Services;
    using Microsoft.Phone.Maps.Toolkit;

    using Windows.Devices.Geolocation;

    public partial class MainPage : PhoneApplicationPage
    {
        private GeoCoordinateWatcher _watcher;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            InitializeMap();

            Loaded += OnMainPageLoaded;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private async void InitializeMap()
        {
            _watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default) { MovementThreshold = 10 };
            _watcher.PositionChanged += this.OnPositionChanged;

            var geoLocator = new Geolocator { DesiredAccuracyInMeters = 10 };

            try
            {
                Geoposition geoposition = await geoLocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10));
                var currentLocation = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
                UserLocationMarker = new UserLocationMarker();
                UserLocationMarker.GeoCoordinate = currentLocation;
                UserLocationMarker.Visibility = Visibility.Visible;
                App.ViewModel.CurrentLocation = currentLocation;
                theMap.SetView(currentLocation, 13);

            }
            catch (Exception)
            {
                // Inform the user that the location cannot be determined.

                App.ViewModel.CurrentLocation = null;
            }
        }

        private void OnMainPageLoaded(object sender, RoutedEventArgs e)
        {
            _watcher.Start();
        }

        void OnPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            theMap.SetView(e.Position.Location, 13);
            App.ViewModel.CurrentLocation = e.Position.Location;
        }

        private async void OnMapHold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var selectedDestination = theMap.ConvertViewportPointToGeoCoordinate(e.GetPosition(theMap));

            var query = new RouteQuery();
            query.InitialHeadingInDegrees = App.ViewModel.CurrentLocation.Course;
            query.TravelMode = TravelMode.Driving;
            query.Waypoints = new List<GeoCoordinate> { App.ViewModel.CurrentLocation, selectedDestination };
            query.RouteOptimization = RouteOptimization.MinimizeTime;
            var result = await query.GetRouteAsync();
            Debug.WriteLine("Duration to destination: {0}", result.EstimatedDuration);
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