using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ImCup2018
{
    public class DateTimeToLocalDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString("yyyy-MM-dd hh:mm:ss");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {
        private ObservableCollection<WineMeasurement> Items;

        private async void LoadFromCloud()
        {
            var blobList = await CloudStorage.GetBlobList("measurements");
            Console.WriteLine($"Measurements count: {blobList.Count}");
            foreach (var blob in blobList)
            {
                var meas = WineMeasurement.Parse(await blob.DownloadTextAsync());
                Console.WriteLine($"Measurement: {meas.Time}");
                Items.Add(meas);
            }
        }
        public HistoryPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            Items = new ObservableCollection<WineMeasurement>();
			
			MyListView.ItemsSource = Items;

            Items.Add(new WineMeasurement()
            {
                Text="Lol\nKek\nCheburek",
                Time = DateTime.Now,
                WineID = "wineid",
                Photo = new Uri("https://avatars.mds.yandex.net/get-pdb/225396/65ae80dd-d073-4616-a3d9-8ebfcbb07edd/s1200"),
            });

            LoadFromCloud();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            ((ListView)sender).SelectedItem = null;
        }
    }
}
