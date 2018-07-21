using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ImCup2018
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            DebugWriteLine($"Test\n{Application.Current}\n");
        }

        public void DebugWriteLine(string st)
        {
            DebugLabel.Text += st + "\n";
        }

        private async void WinesButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WinesPage());
            DebugWriteLine("WinesPage added");
        }

        private async void HistoryButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistoryPage());
            DebugWriteLine("HistoryPage added");
        }

        private async void NewButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MeasurementPage());
            DebugWriteLine("MeasurementPage added");
        }

        private async void AboutButton_Clicked(object sender, EventArgs e)
        {
            DebugWriteLine("Test WineMeasurement");
            var meas = new WineMeasurement()
            {
                Time = DateTime.Now,
                WineID = "WineID",
                Text = "Text1\nText2\nText3",
                Photo = CloudStorage.GetBlobUri("container1","test1.txt"),
                Spectrum = new double[24],
            };

            var meas_str = meas.ToString();
            DebugWriteLine(meas_str);

            DebugWriteLine("Uploading");
            await CloudStorage.UploadBlobText("container1", "test1.txt", meas_str.ToString());
            DebugWriteLine("Downloading");
            meas_str = await CloudStorage.DownloadBlobText("container1", "test1.txt");

            DebugWriteLine("Parsed WineMeasurement");
            var meas2 = WineMeasurement.Parse(meas_str);
            DebugWriteLine(meas2.ToString());
        }
    }
}
