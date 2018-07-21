using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
 
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;

namespace ImCup2018
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MeasurementPage : ContentPage
    {
        private WineMeasurement measurement;
        MediaFile file;

        public MeasurementPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            measurement = new WineMeasurement();
            file = null;
        }

        private async void FromCameraButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                if (CrossMedia.Current.IsTakePhotoSupported && CrossMedia.Current.IsCameraAvailable)
                {
                    try
                    {
                        file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                        {
                            SaveToAlbum = true,
                            Directory = "ImCup2018",
                            Name = measurement.Time.ToString("yyyy.MM.dd_hh.mm.ss.jpg"),
                        });
                        if (file != null)
                        {
                            CameraInput.Source = ImageSource.FromStream(() =>
                            {
                                return file.GetStream();
                            });
                            FromCameraButton.BackgroundColor = Color.Green;
                            FromCameraButton.Text = "From camera";

                        }
                    }
                    catch (Exception ex)
                    { }
                }
                if (file == null && CrossMedia.Current.IsPickPhotoSupported)
                {
                    Console.WriteLine("FROM FILE");
                    try
                    {
                        file = await CrossMedia.Current.PickPhotoAsync();
                        CameraInput.Source = ImageSource.FromStream(()=> { return file.GetStream(); });
                        FromCameraButton.BackgroundColor = Color.Green;
                        FromCameraButton.Text = "From file";
                    }
                    catch (Exception ex)
                    { }
                }
                if (file == null)
                {
                    FromCameraButton.BackgroundColor = Color.Red;
                    FromCameraButton.Text = "Error";
                }

                if (FromCameraButton.BackgroundColor == Color.Green && FromCupButton.BackgroundColor == Color.Green)
                {
                    ReadyButton.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void FromCupButton_Clicked(object sender, EventArgs e)
        {
            IBluetoothLE bluetoothBLE = CrossBluetoothLE.Current;
            IAdapter adapter = CrossBluetoothLE.Current.Adapter;
            List<IDevice> deviceList = new List<IDevice>();
            IDevice device = null;

            var state = bluetoothBLE.State;
            CupInput.Text += $"BLE State: {state}\n";

            /*if (bluetoothBLE.State == BluetoothState.Off)
            {
                await DisplayAlert("Error", "Bluetooth disabled.", "OK");
            }
            else
            {
                deviceList.Clear();

                adapter.ScanTimeout = 3000;
                adapter.ScanMode = ScanMode.Balanced;

                adapter.DeviceDiscovered += (obj, a) =>
                {
                    if (!deviceList.Contains(a.Device))
                        deviceList.Add(a.Device);
                };


                CupInput.Text += $"Start scaning\n";
                await adapter.StartScanningForDevicesAsync();

                FromCupButton.Text = "Scaning\n";
            }
            CupInput.Text += $"Device count: {deviceList.Count}";

            foreach(var dev in deviceList)
            {
                CupInput.Text += $"{dev.Name} {dev.Id} {dev.State}\n";
                if (dev.Name == "CC41-A")
                {
                    device = dev;
                }
            }*/
                

            CupInput.Text += "Connecting...";
            try
            {
                device = await adapter.ConnectToKnownDeviceAsync(new Guid("00000000-0000-0000-0000-00158310d640"));
                CupInput.Text += $"{device.Name}: {device.State}\n";
            }
            catch(Exception ex)
            {
                CupInput.Text += $"Error: {ex.Message}\n";
                FromCupButton.Text = "Error";
                return;
            }
            CupInput.Text += "OK\n";



            // start
            var srv = await device.GetServiceAsync(new Guid("0000ffe0-0000-1000-8000-00805f9b34fb"));
            var ch = await srv.GetCharacteristicAsync(new Guid("0000ffe1-0000-1000-8000-00805f9b34fb"));
            await ch.WriteAsync(Encoding.UTF8.GetBytes("1"));
            string hex = BitConverter.ToString(Encoding.UTF8.GetBytes("1"));
            CupInput.Text += $"Hex Repr: {hex}\n";

            var services = await device.GetServicesAsync();
            foreach(var service in services)
            {
                //CupInput.Text += $"Service: {service.Name} {service.Id}\n";
                var chars = await service.GetCharacteristicsAsync();
                foreach (var chr in chars)
                {
                    CupInput.Text += $"--Char: {chr.Name} [{chr.Id},{chr.Uuid}[{chr.StringValue},{chr.Value}] {chr.CanRead} {chr.CanWrite} {chr.CanUpdate}]\n";
                    var desriptors = await chr.GetDescriptorsAsync();
                    foreach (var desc in desriptors)
                    {
                        //CupInput.Text += $"----Desc: {desc.Name} [{desc.Id},{desc.Value}]\n";
                        var res = await desc.ReadAsync();
                        var str = Encoding.UTF8.GetString(res);
                        //CupInput.Text += ($"Readed: /{str}/\n");
                    }
                    if (chr.CanRead)
                    {
                        byte[] bytes;
                        bytes = await chr.ReadAsync();
                        var str = Encoding.UTF8.GetString(bytes);
                        CupInput.Text += ($"Readed: /{str}/\n");
                    }
                    if (chr.CanUpdate)
                    {
                        CupInput.Text += ($"Updatable: /{chr.Name}/\n");
                        /*chr.ValueUpdated += async (obj, a) =>
                        {
                            try
                            {
                                var res = await a.Characteristic.ReadAsync();
                                var str = Encoding.UTF8.GetString(res);
                                Console.WriteLine($"[BLE {chr.Name}: '{str}']");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[BLE ERROR {chr.Name}: '{ex.Message}']");
                            }
                        };
                        await chr.StartUpdatesAsync();*/
                    }
                }
            }
            
            Console.WriteLine(CupInput.Text);
            
            FromCupButton.Text = "Finished";

            /*FromCupButton.BackgroundColor = Color.Green;
            if (FromCameraButton.BackgroundColor == Color.Green && FromCupButton.BackgroundColor == Color.Green)
            {
                ReadyButton.IsVisible = true;
                FromCameraButton.IsVisible = false;
                FromCupButton.IsVisible = false;
            }*/
        }

        private async void ReadyButton_Clicked(object sender, EventArgs e)
        {
            ReadyButton.BackgroundColor = Color.Green;
            try
            {
                Console.WriteLine("Uploading mediafile");
                string file_name = $"{measurement.Time.ToString("yyyy.MM.dd_hh.mm.ss")}";
                await CloudStorage.UploadBlobStream("pictures",
                    file_name+".jpg",
                    file.GetStream());

                measurement.Photo = CloudStorage.GetBlobUri("pictures", file_name + ".jpg");
                measurement.Text = "Processing...";
                //Set spectrum
                //??

                Console.WriteLine("Uploading measurement");
                await CloudStorage.UploadBlobText("measurements", file_name + ".txt",
                    measurement.ToString());
                Console.WriteLine("OK!");
            }
            catch (Exception ex)
            {
            }
            await Navigation.PopAsync();
        }
    }
}