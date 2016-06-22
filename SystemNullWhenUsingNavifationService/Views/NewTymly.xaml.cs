using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using SystemNullWhenUsingNavifationService.Models;
using SystemNullWhenUsingNavifationService.ViewModels;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SystemNullWhenUsingNavifationService.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewTymly : Page
    {
        private uint desiredAccuracy_meters = 0;
        private CancellationTokenSource cancelTokenSrc = null;

        private IEnumerable<string> zonalIds;

        private IReadOnlyList<GetContactDetails> condetails;
        public IReadOnlyList<GetContactDetails> ConDetails { get { return condetails; } set { condetails = value; } }
        private ObservableCollection<GetContactDetails> contacts;
        public ObservableCollection<GetContactDetails> Contacts { get { return contacts; } set { contacts = value; } }

        public NewTymly()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            DataContext = new NewTymlyViewModel();
            searchLocation.Loaded += Map_Loaded;
            searchLocation.MapTapped += Map_Tapped;
            searchLocation.MapDoubleTapped += Map_DoubledTapped;
            searchLocation.Style = MapStyle.Road;
        }

        private void Map_DoubledTapped(MapControl sender, MapInputEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void Map_Tapped(MapControl sender, MapInputEventArgs args)
        {
            var tappedPositon = args.Location.Position;
            if (sender.ZoomLevel < 20)
            {
                sender.ZoomLevel += 2;
            }
            else if (sender.ZoomLevel >= 20)
            {
                sender.ZoomLevel = 12;
            }
        }

        private async void Map_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var requestLocationAccess = await Geolocator.RequestAccessAsync();
                switch (requestLocationAccess)
                {
                    case GeolocationAccessStatus.Allowed:
                        cancelTokenSrc = new CancellationTokenSource();
                        CancellationToken token = cancelTokenSrc.Token;

                        Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = desiredAccuracy_meters };
                        Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);

                        searchLocation.Center =
                            new Geopoint(new BasicGeoposition()
                            {
                                Latitude = pos.Coordinate.Point.Position.Latitude,
                                Longitude = pos.Coordinate.Point.Position.Longitude
                            });
                        searchLocation.ZoomLevel = 1;
                        break;
                }
            }
            catch (TaskCanceledException)
            {
                throw new TaskCanceledException();
            }
            finally
            {
                cancelTokenSrc = null;
            }
        }

        private async void ShowOnMap(double lati, double longi)
        {
            try
            {
                var accessLocation = await Geolocator.RequestAccessAsync();

                switch (accessLocation)
                {
                    case GeolocationAccessStatus.Allowed:
                        cancelTokenSrc = new CancellationTokenSource();
                        CancellationToken token = cancelTokenSrc.Token;

                        Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = desiredAccuracy_meters };
                        Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);

                        searchLocation.Center =
                            new Geopoint(new BasicGeoposition()
                            {
                                Latitude = lati,
                                Longitude = longi,
                            });
                        searchLocation.ZoomLevel = 5;
                        break;
                }
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                cancelTokenSrc = null;
            }
        }

        private void styleCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (mapStyle.SelectedIndex)
            {
                case 0:
                    searchLocation.Style = MapStyle.None;
                    break;

                case 1:
                    searchLocation.Style = MapStyle.Road;
                    break;

                case 2:
                    searchLocation.Style = MapStyle.Aerial;
                    break;

                case 3:
                    searchLocation.Style = MapStyle.AerialWithRoads;
                    break;

                case 4:
                    searchLocation.Style = MapStyle.Terrain;
                    break;
            }
        }

        public void InputLocation_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            //{
            //    zonalIds = TzdbDateTimeZoneSource.Default.ZoneLocations.
            //        Where(loc => loc.CountryName.IndexOf(sender.Text, StringComparison.CurrentCultureIgnoreCase) > -1).
            //        Select(loc => loc.ZoneId);
            //    sender.ItemsSource = zonalIds.ToList();
            //}
        }

        public void InputLocation_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            //MapGlyphs mapGlyph = new MapGlyphs();
            //if (args.ChosenSuggestion != null)
            //{
            //    var selectedZonalId = args.ChosenSuggestion as string;

            //    string state = selectedZonalId.Remove(0, selectedZonalId.IndexOf("/") + 1).Replace("_", " ");

            //    var country = TzdbDateTimeZoneSource.Default.ZoneLocations.
            //    Where(zoneloc => zoneloc.ZoneId == selectedZonalId).Select(zoneloc => zoneloc.CountryName).
            //    ElementAtOrDefault(0);

            //    var timeZone = TzdbDateTimeZoneSource.Default.ZoneLocations.
            //        Where(tym => tym.ZoneId == selectedZonalId).Select(tymZone => TzdbDateTimeZoneSource.Default.WindowsMapping.
            //        MapZones.FirstOrDefault(tym => tym.TzdbIds.Contains(selectedZonalId))).
            //        Where(tym => tym != null).Select(tym => tym.WindowsId).Distinct();

            //    var lattitude = TzdbDateTimeZoneSource.Default.ZoneLocations.
            //        Where(lat => lat.ZoneId == selectedZonalId).Select(tym => tym.Latitude);

            //    var longitude = TzdbDateTimeZoneSource.Default.ZoneLocations.
            //        Where(lon => lon.ZoneId == selectedZonalId).Select(tym => tym.Longitude);

            //    if (timeZone.Count() != 0)
            //    {
            //        var dateTime = TimeZoneInfo.ConvertTime(
            //            DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(timeZone.ElementAt(0)));
            //        if (mapGlyph.PlaceIcon.ContainsKey(state))
            //        {
            //            locationText.Text = state;
            //            ViewModel.New_Tymly.Location = state;
            //            ViewModel.New_Tymly.MapIcon = mapGlyph.MapLocationIcon(state);
            //        }
            //        else
            //        {
            //            locationText.Text = country;
            //            ViewModel.New_Tymly.Location = country;
            //            ViewModel.New_Tymly.MapIcon = mapGlyph.MapLocationIcon(country);
            //        }

            //        dateText.Text = dateTime.ToString("D");
            //        timeText.Text = dateTime.ToString("T");
            //        ViewModel.New_Tymly.Datetime = dateTime;
            //        IconPath.Data = PathMarkupToGeometry(ViewModel.New_Tymly.MapIcon);
            //    }
            //    else
            //    {
            //    }
            //    ShowOnMap(lattitude.ElementAt(0), longitude.ElementAt(0));
            //}
        }

        public void InputLocation_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            //MapGlyphs mapGlyph = new MapGlyphs();
            //var place = args.SelectedItem as string;
            //string state = place.Remove(0, place.IndexOf("/") + 1).Replace("_", " ");

            //var country = TzdbDateTimeZoneSource.Default.ZoneLocations.
            //Where(zoneloc => zoneloc.ZoneId == place).Select(zoneloc => zoneloc.CountryName).
            //ElementAtOrDefault(0);
            //var timeZone = TzdbDateTimeZoneSource.Default.ZoneLocations.
            //        Where(x => x.ZoneId == place).Select(tz => TzdbDateTimeZoneSource.Default.WindowsMapping.
            //        MapZones.FirstOrDefault(x => x.TzdbIds.Contains(place))).
            //        Where(x => x != null).Select(x => x.WindowsId).Distinct();
            //var latitude = TzdbDateTimeZoneSource.Default.ZoneLocations.
            //    Where(x => x.ZoneId == place).Select(tz => tz.Latitude);
            //var longitude = TzdbDateTimeZoneSource.Default.ZoneLocations.
            //    Where(x => x.ZoneId == place).Select(tz => tz.Longitude);

            //if (timeZone.Count() != 0)
            //{
            //    var dateTime = TimeZoneInfo.ConvertTime(
            //        DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById(timeZone.ElementAt(0)));
            //    if (mapGlyph.PlaceIcon.ContainsKey(state))
            //    {
            //        locationText.Text = state;
            //        ViewModel.New_Tymly.Location = state;
            //        ViewModel.New_Tymly.MapIcon = mapGlyph.MapLocationIcon(state);
            //    }
            //    else
            //    {
            //        locationText.Text = country;
            //        ViewModel.New_Tymly.Location = country;
            //        ViewModel.New_Tymly.MapIcon = mapGlyph.MapLocationIcon(country);
            //    }

            //    dateText.Text = dateTime.ToString("D");
            //    timeText.Text = dateTime.ToString("T");
            //    ViewModel.New_Tymly.Datetime = dateTime;
            //    IconPath.Data = PathMarkupToGeometry(ViewModel.New_Tymly.MapIcon);
            //}
            //else
            //{
            //}

            //ShowOnMap(latitude.ElementAt(0), longitude.ElementAt(0));
        }

        private Geometry PathMarkupToGeometry(string pathMarkup)
        {
            string xaml =
            "<Path " +
            "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
            "<Path.Data>" + pathMarkup + "</Path.Data></Path>";
            var path = XamlReader.Load(xaml) as Windows.UI.Xaml.Shapes.Path;
            // Detach the PathGeometry from the Path
            Geometry geometry = path.Data;
            path.Data = null;
            return geometry;
        }

        private async void AddContacts(object sender, RoutedEventArgs e)
        {
            //var contactPicker = new ContactPicker();
            //contactPicker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.Email);
            //contactPicker.DesiredFieldsWithContactFieldType.Add(ContactFieldType.PhoneNumber);

            //var selectedContacts = await contactPicker.PickContactsAsync();
            //ContactStore contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);

            //if (selectedContacts != null && selectedContacts.Count > 0)
            //{
            //    foreach (var item in selectedContacts)
            //    {
            //        Contact realContact = await contactStore.GetContactAsync(item.Id);

            //        ViewModel.New_Tymly.ContactList.Add(new ContactItemAdapter(realContact));
            //        var t = new ContactItemAdapter(realContact).Thumbnail;
            //        Debug.WriteLine(t);
            //    }
            //}
        }

        private void DeleteContact(object sender, RoutedEventArgs e)
        {
            //ContactItemAdapter item = (ContactItemAdapter)(sender as Button).DataContext;
            //Debug.WriteLine(item.Name);
            //if (addContacts.Items.Contains(item))
            //{
            //    ViewModel.New_Tymly.ContactList.Remove(item);
            //}
        }
    }
}