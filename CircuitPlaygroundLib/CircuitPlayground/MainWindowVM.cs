using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CircuitPlaygroundLib;

namespace CircuitPlaygroundGUI
{
    class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private CircuitPlayground model = new CircuitPlayground();
        public MainWindowVM()
        {
            model.SensorDataReceived += Model_SensorDataReceived;
        }

        private void Model_SensorDataReceived(object sender, SensorEventArgs e)
        {
            AccelX = e.X;
            AccelY = e.Y;
            AccelZ = e.Z;
            TempC = e.tempC;
            TempF = e.tempF;
            Light = e.light;
            Sound = e.sound;
        }
        private float accelX;

        public float AccelX
        {
            get { return accelX; }
            set { accelX = value; NotifyPropertyChanged(); }
        }
        private float accelY;

        public float AccelY
        {
            get { return accelY; }
            set { accelY = value; NotifyPropertyChanged(); }
        }
        private float accelZ;

        public float AccelZ
        {
            get { return accelZ; }
            set { accelZ = value; NotifyPropertyChanged(); }
        }

        private float tempC;

        public float TempC
        {
            get { return tempC; }
            set { tempC = value; NotifyPropertyChanged(); }
        }
        private float tempF;

        public float TempF
        {
            get { return tempF; }
            set { tempF = value; NotifyPropertyChanged(); }
        }
        private float light;

        public float Light
        {
            get { return light; }
            set { light = value; NotifyPropertyChanged(); }
        }
        private float sound;

        public float Sound
        {
            get { return sound; }
            set { sound = value; NotifyPropertyChanged(); }
        }

        public List<string> Devices { get { return model.GetDevices().ToList(); } }
        public string SelectedPort { get; set; }

        public byte PinSlider { get; set; }
        public byte RedSlider { get; set; }
        public byte GreenSlider { get; set; }
        public byte BlueSlider { get; set; }

        public RelayCommand OpenCommand { get { return new RelayCommand((x) => Open(x)); } }
        private void Open(object x)
        {
            model.SetPort(SelectedPort);
            model.Open();
        }
        public RelayCommand SetPixelsCommand { get { return new RelayCommand((x) => SetPixels(x)); } }
        private void SetPixels(object x)
        {
            model.SetPixelColor(PinSlider, RedSlider, GreenSlider, BlueSlider);
        }

        public RelayCommand ClearPixelsCommand { get { return new RelayCommand((x) => ClearPixels(x)); } }
        private void ClearPixels(object x)
        {
            model.ClearPixels();
        }
    }
}

