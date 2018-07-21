using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using System.IO;
using System.ComponentModel;
using Xamarin.Forms;

namespace ImCup2018
{
    class WineMeasurement : INotifyPropertyChanged
    {
        private DateTime time;
        private string text;
        private double[] spectrum;
        private string wineid;
        private Uri photo;

        public WineMeasurement()
        {
            time = DateTime.Now;
            text = "";
            spectrum = new double[24];
            for (int i = 0; i < 24; i++)
                spectrum[i] = 0.0;
            wineid = "";
            photo = null; 
        }
        public override string ToString()
        {
            string res = "";
            res += $"{time.ToString("yyyy.MM.dd_hh.mm.ss")}|";
            res += $"{wineid}|";
            res += $"{text}|";
            res += $"{photo}|";

            for (int i = 0; i < spectrum.Length; i++)
            {
                res += $"{spectrum[i]}/";
            }
            return res;
        }
        public static WineMeasurement Parse(string st)
        {
            var args = st.Split('|');
            var spec_str = args[4].Split('/');
            double[] spec = new double[spec_str.Length - 1];
            DateTime time = DateTime.ParseExact(args[0], "yyyy.MM.dd_hh.mm.ss", null);
            for (int i = 0; i < spec.Length - 1; i++)
            {
                spec[i] = float.Parse(spec_str[i], CultureInfo.InvariantCulture.NumberFormat);
            }

            return new WineMeasurement()
            {
                Time = time,
                WineID = args[1],
                Text = args[2],
                Spectrum = spec,
                Photo = new Uri(args[3]),
            };
        }


        public Uri Photo
        {
            get { return photo; }
            set
            {
                if(value != photo)
                {
                    photo = value;
                    OnPropertyChanged("Photo");
                }
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    text = value;
                    OnPropertyChanged("Text");
                }
            }
        }
        public DateTime Time
        {
            get { return time; }
            set
            {
                if (value != time)
                {
                    time = value;
                    OnPropertyChanged("Time");
                }
            }
        }

        public string WineID
        {
            get { return wineid; }
            set
            {
                if (value != wineid)
                {
                    wineid = value;
                    OnPropertyChanged("WineID");
                }
            }
        }

        public double[] Spectrum
        {
            get { return spectrum; }
            set
            {
                if (value != spectrum)
                {
                    spectrum = value;
                    OnPropertyChanged("Spectrum");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
