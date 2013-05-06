namespace Boco.OnMyWay.App
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Windows;

    using Boco.OnMyWay.App.Helpers;

    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Maps.Services;
    using Microsoft.Phone.Maps.Toolkit;
    using Microsoft.Phone.Tasks;

    using Windows.Devices.Geolocation;

    public partial class MainPage : PhoneApplicationPage
    {
        private GeoCoordinateWatcher _watcher;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            InitializeMap();
            Loaded += OnMainPageLoad;
            DataContext = App.ViewModel;
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void OnMainPageLoad(object sender, RoutedEventArgs e)
        {
            _watcher.Start();
        }

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
            App.ViewModel.DurationToDestination = result.EstimatedDuration;

            this.DoContactSelection();
        }


        private void DoContactSelection()
        {
            PhoneNumberChooserTask phoneNumberChooser = new PhoneNumberChooserTask();
            phoneNumberChooser.Completed += OnPhoneNumberChooserCompleted;
            phoneNumberChooser.Show();
        }

        private void OnPhoneNumberChooserCompleted(object sender, PhoneNumberResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                this.DoSms(e.PhoneNumber);
            }
        }

        private void DoSms(string theNumber)
        {
            SmsComposeTask smsTask = new SmsComposeTask();
            smsTask.To = theNumber;
            smsTask.Body = string.Format("Hey, I'm on my way and will be there in {0}", FriendlyTimeHelper.TimeSpanToFriendlyTime(App.ViewModel.DurationToDestination));
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