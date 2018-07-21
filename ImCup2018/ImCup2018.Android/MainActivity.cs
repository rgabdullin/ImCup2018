using System;
using System.Linq;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Bluetooth;
using Android.OS;
using Plugin.CurrentActivity;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android;
using Java.Util;
using System.Threading.Tasks;
using System.Text;

namespace ImCup2018.Droid
{
    [Activity(Label = "ImCup2018", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            CrossCurrentActivity.Current.Init(this, bundle);

            ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation }, 0);

            LoadApplication(new App());
            //Test();
        }
        private async void Test()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
                throw new Exception("No Bluetooth adapter found.");

            if (!adapter.IsEnabled)
                throw new Exception("Bluetooth adapter is not enabled.");

            var device = adapter.GetRemoteDevice("00:15:83:10:D6:40");
            if (device == null)
                throw new Exception("Named device not found.");
            
            var _socket = device.CreateInsecureRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
            await _socket.ConnectAsync();

            byte[] buffer = Encoding.UTF8.GetBytes("1");
            byte[] input = new byte[1000];
            await _socket.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            await _socket.InputStream.ReadAsync(input, 0, 1000);

            string st = Encoding.UTF8.GetString(input);
            Console.WriteLine($"FROMCUP: {st}");
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

