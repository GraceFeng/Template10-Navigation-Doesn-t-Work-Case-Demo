using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemNullWhenUsingNavifationService.Models;
using SystemNullWhenUsingNavifationService.Views;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace SystemNullWhenUsingNavifationService.ViewModels
{
    public class NewTymlyViewModel : ViewModelBase
    {
        private Tymly newTymly;
        public Tymly New_Tymly { get { return newTymly; } set { Set(ref newTymly, value); } }

        private bool trafficFlow;

        public NewTymlyViewModel()
        {
            newTymly = new Tymly();
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            New_Tymly = (suspensionState.ContainsKey(nameof(New_Tymly))) ? suspensionState[nameof(New_Tymly)] as Tymly : parameter as Tymly;
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(New_Tymly)] = New_Tymly;
            }
            await Task.CompletedTask;
        }

        public bool TrafficFlow
        {
            get { return trafficFlow; Debug.WriteLine(trafficFlow); }
            set { trafficFlow = !trafficFlow; }
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void SaveNewTymly() =>
            NavigationService.Navigate(typeof(Views.NewTymly), New_Tymly);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);
    }
}