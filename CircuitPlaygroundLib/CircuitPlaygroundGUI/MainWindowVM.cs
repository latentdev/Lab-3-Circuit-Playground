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
            model.DataReceived += Model_DataReceived;
        }

        private void Model_DataReceived(object sender, EventArgs e)
        {
            AccelData accel = model.ReadAccel;
            AccelX = accel.X;
            AccelY = accel.Y;
            AccelZ = accel.Z;
            TempC = model.ReadTempC;
            TempF = model.ReadTempF;
            Light = model.ReadLight;
            Sound = model.ReadSound;
            LeftButton = model.ReadLeftButton;
            RightButton = model.ReadRightButton;
            SlideSwitch = model.ReadSlideSwitch;
            IsHeld = model.IsBeingHeld();
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

        private bool leftButton;
        public bool LeftButton
        {
            get { return leftButton; }
            set { leftButton = value; NotifyPropertyChanged(); }
        }
        private bool rightButton;

        public bool RightButton
        {
            get { return rightButton; }
            set { rightButton = value; NotifyPropertyChanged(); }
        }
        private bool slideSwitch;

        public bool SlideSwitch
        {
            get { return slideSwitch; }
            set { slideSwitch = value; NotifyPropertyChanged(); }
        }

        public List<string> Devices { get { return model.GetDevices().ToList(); } }

        private string selectedPort;
        public string SelectedPort
        {
            get { return selectedPort; }
            set { selectedPort = value; NotifyPropertyChanged(); }
        }

        private bool isHeld;

        public bool IsHeld
        {
            get { return isHeld; }
            set { isHeld = value; NotifyPropertyChanged(); }
        }

        public byte PinSlider { get; set; }
        public byte RedSlider { get; set; }
        public byte GreenSlider { get; set; }
        public byte BlueSlider { get; set; }

        public byte PinSlider2 { get; set; }
        public byte RedSlider2 { get; set; }
        public byte GreenSlider2 { get; set; }
        public byte BlueSlider2 { get; set; }

        public byte FadeSlider { get; set; }
        public byte RedSlider3 { get; set; }
        public byte GreenSlider3 { get; set; }
        public byte BlueSlider3 { get; set; }

        public bool RepeatCheckBox { get; set; }

        public string Filename { get; set; }

        public RelayCommand OpenCommand { get { return new RelayCommand((x) => Open(x)); } }
        private void Open(object x)
        {

            model.Open(SelectedPort);
        }
        public RelayCommand SetPixelsCommand { get { return new RelayCommand((x) => SetPixels(x)); } }
        private void SetPixels(object x)
        {
            LedColor led;
            led.R = RedSlider;
            led.G = GreenSlider;
            led.B = BlueSlider;
            model.SetPixelColor(PinSlider,led);
        }

        public RelayCommand ClearPixelsCommand { get { return new RelayCommand((x) => ClearPixels(x)); } }
        private void ClearPixels(object x)
        {
            model.ClearAllPixels();
        }

        public RelayCommand SetPixelCrossfadeCommand { get { return new RelayCommand((x) => SetPixelCrossfade(x)); } }
        private void SetPixelCrossfade(object x)
        {
            LedColor start;
            start.R = RedSlider2;
            start.G = GreenSlider2;
            start.B = BlueSlider2;
            LedColor finish;
            finish.R = RedSlider3;
            finish.G = GreenSlider3;
            finish.B = BlueSlider3;
            model.SetPixelCrossFade(PinSlider2,start,finish,FadeSlider,RepeatCheckBox);
        }

        public RelayCommand ExportSensorDataCommand { get { return new RelayCommand((x) => ExportSensorData(x)); } }
        private void ExportSensorData(object x)
        {
            model.ExportSensorData(Filename);
        }
    }
}

