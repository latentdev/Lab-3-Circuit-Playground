using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitPlaygroundLib
{
    struct stats
    {
        public AccelData accelData;
        public float tempC;
        public float tempF;
        public ushort light;
        public ushort sound;
        public bool leftButton;
        public bool rightButton;
        public bool slideSwitch;
    }

    public class CircuitPlayground : ICircuitPlayground, ILab3
    {
        private List<stats> Stats = new List<stats>();
        private List<AccelData> past = new List<AccelData>();
        private SerialPort port;
        private AccelData accelData;
        private AccelData oldAccelData;
        private float tempC;
        private float tempF;
        private ushort light;
        private ushort sound;
        private bool leftButton;
        private bool rightButton;
        private bool slideSwitch;
        private bool save = true;
        public bool IsOpen
        {
            get
            {
                return port.IsOpen;
            }
        }

        public AccelData ReadAccel
        {
            get
            {
                return accelData;
            }
        }

        public float ReadTempC
        {
            get
            {
                return tempC;
            }
        }

        public float ReadTempF
        {
            get
            {
                return tempF;
            }
        }

        public ushort ReadLight
        {
            get
            {
                return light;
            }
        }

        public ushort ReadSound
        {
            get
            {
                return sound;
            }
        }

        public bool ReadLeftButton
        {
            get
            {
                return leftButton;
            }
        }

        public bool ReadRightButton
        {
            get
            {
                return rightButton;
            }
        }

        public bool ReadSlideSwitch
        {
            get
            {
                return slideSwitch;
            }
        }

        public bool ReadHeld
        {
            get
            {
                return IsBeingHeld();
            }
        }

        public event EventHandler<EventArgs> DataReceived;

        public void SetPort(string portname)
        {
            port.PortName = portname;
        }

        public void ClearAllPixels()
        {
            if (port.IsOpen)
            {
                byte[] buffer = new byte[13];
                buffer[0] = 0x02;
                port.Write(buffer, 0, buffer.Length);
                /*for (int i = 0; i < 10; i++)
                {
                    buffer[1] = (byte)i;

                    port.Write(buffer, 0, buffer.Length);
                }*/
            }
        }

        public void Close()
        {
            if (port.IsOpen)
            {
                port.Close();
            }
        }

        public string[] GetDevices()
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length==1)
            { 
                Open(ports[0]);
            }
            return ports;
        }

        public bool Open(string device)
        {
            try
            {
                port = new SerialPort();

                port.BaudRate = 9600;
                port.DtrEnable = true;
                port.PortName = device;
                port.Open();
                port.DataReceived += Port_DataReceived;

                return true;
            }
            catch
            {
                return false;
            }

        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = port.ReadLine();

            try
            {

                float[] parts = data.Trim().Split(',').Select(float.Parse).ToArray();

                if (parts.Length == 10)
                {
                    oldAccelData = accelData;
                    accelData.X = parts[0];
                    accelData.Y = parts[1];
                    accelData.Z = parts[2];
                    tempC = parts[3];
                    tempF = parts[4];
                    light = (ushort)parts[5];
                    sound = (ushort)parts[6];

                    if (parts[7] > 0)
                        leftButton = true;
                    else
                        leftButton = false;

                    if (parts[8] > 0)
                        rightButton = true;
                    else
                        rightButton = false;

                    if (parts[9] > 0)
                        slideSwitch = true;
                    else
                        slideSwitch = false;
                    if (save == true)
                    {
                        stats Stat;
                        Stat.accelData = accelData;
                        Stat.tempC = tempC;
                        Stat.tempF = tempF;
                        Stat.light = light;
                        Stat.sound = sound;
                        Stat.leftButton = leftButton;
                        Stat.rightButton = rightButton;
                        Stat.slideSwitch = slideSwitch;

                        Stats.Add(Stat);
                    }
                    DataReceived?.Invoke(this,new EventArgs());

                }
            }
            catch { }
        }

        public void SetPixelColor(int pixel, LedColor color)
        {
            if (port.IsOpen)
            {
                byte[] buffer = new byte[13];
                buffer[0] = 0xAA;

                buffer[1] = Convert.ToByte(pixel);
                buffer[2] = color.R;
                buffer[3] = color.G;
                buffer[4] = color.B;

                port.Write(buffer, 0, buffer.Length);
            }
        }

        public void SetPixelCrossFade(int pixel, LedColor start, LedColor end, int fadeTime, bool repeat = false)
        {
            if (port.IsOpen)
            {
                byte[] buffer = new byte[13];
                buffer[0] = 0x01;

                buffer[1] = Convert.ToByte(pixel);
                buffer[2] = start.R;
                buffer[3] = start.G;
                buffer[4] = start.B;
                buffer[5] = end.R;
                buffer[6] = end.G;
                buffer[7] = end.B;
                var time = System.BitConverter.GetBytes(fadeTime);
                buffer[8] = time[0];
                buffer[9] = time[1];
                buffer[10] = time[2];
                buffer[11] = time[3];
                buffer[12] = Convert.ToByte(repeat);


                port.Write(buffer, 0, buffer.Length);
            }
        }

        public bool IsBeingHeld()
        {
            AccelData container;
            container.X = Math.Abs(accelData.X-oldAccelData.X);
            container.Y = Math.Abs(accelData.Y-oldAccelData.Y);
            container.Z = Math.Abs(accelData.Z-oldAccelData.Z);
            past.Add(container);
            if(past.Count>20)
            {
                container.X = 0;
                container.Y = 0;
                container.Z = 0;
                for(int i=0;i<past.Count;i++)
                {
                    container.X += past[i].X;
                    container.Y += past[i].Y;
                    container.Z += past[i].Z;
                }
                container.X = container.X / past.Count;
                container.Y = container.Y / past.Count;
                container.Z = container.Z / past.Count;
                past.RemoveAt(0);
            }
            if (container.X > .11 || container.Y > .11 || container.Z > .11)
                return true;
            else
                return false;

        }

        public void SetPattern(int id)
        {
            throw new NotImplementedException();
        }

        public void ExportSensorData(string filename)
        {
            save = false;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename+".csv"))
            {
                file.WriteLine("X, Y, Z, Temp C, Temp F, Light, Sound, Left Button, Right Button, Slide Switch");
                foreach (var Stat in Stats)
                {
                        file.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",Stat.accelData.X,Stat.accelData.Y,Stat.accelData.Z,Stat.tempC,Stat.tempF,Stat.light,Stat.sound,Stat.leftButton,Stat.rightButton,Stat.slideSwitch);
                }
            }
            save = true;
        }
    }
}
