namespace Boco.OnMyWay.App.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Device.Location;
    using System.Runtime.CompilerServices;

    using Boco.OnMyWay.App.Annotations;

    public class AppViewModel : INotifyPropertyChanged
    {
        public GeoCoordinate CurrentLocation { get; set; }

        private TimeSpan _durationToDestination;
        public TimeSpan DurationToDestination
        {
            get
            {
                return _durationToDestination;
            }
            set
            {
                _durationToDestination = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}