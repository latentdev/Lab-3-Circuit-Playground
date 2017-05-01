using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitPlaygroundLib
{
    public struct LedColor
    {
        public byte R;
        public byte G;
        public byte B;
    }
    public struct AccelData
    {
        public float X;
        public float Y;
        public float Z;
    }

    public interface ICircuitPlayground
    {
        /// <summary>
        /// Returns an array of device identifiers which should be passed into Open()
        /// to identify which device to open. 
        /// </summary>
        /// <returns>A string array of device identifiers.</returns>
        string[] GetDevices();

        /// <summary>
        /// Returns true if the device is open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Opens the device and allows for reading and writing.
        /// </summary>
        /// <param name="device">The device identifier.</param>
        /// <returns>Returns true if successfully opened. False if failed to open.</returns>
        bool Open(string device);

        /// <summary>
        /// Closes the device and cleans up any open device handles.
        /// </summary>
        void Close();

        /// <summary>
        /// This event should fire whenever any data is received from the device. 
        /// </summary>
        event EventHandler<EventArgs> DataReceived;

        /// <summary>
        /// Gets the current accel sensor reading.
        /// Returns a struct containing the X, Y, and Z values.
        /// </summary>
        AccelData ReadAccel { get; }

        /// <summary>
        /// Gets the current temperature in C.
        /// </summary>
        float ReadTempC { get; }
        /// <summary>
        /// Gets the current temperature in F.
        /// </summary>
        float ReadTempF { get; }

        /// <summary>
        /// Gets the current light sensor value.
        /// 10-bit value. From 0 to 1023.
        /// </summary>
        UInt16 ReadLight { get; }
        /// <summary>
        /// Gets the current microphone sensor value. 
        /// 10-bit value. From 0 to 1023.
        /// </summary>
        UInt16 ReadSound { get; }

        /// <summary>
        /// Reads the left button state.
        /// True if pressed, false if not.
        /// </summary>
        bool ReadLeftButton { get; }
        /// <summary>
        /// Reads the right button state.
        /// True if pressed, false if not.
        /// </summary>
        bool ReadRightButton { get; }
        /// <summary>
        /// Reads the slide swtich state.
        /// True if in the left or (+) position, false if in the right (-) position.
        /// </summary>
        bool ReadSlideSwitch { get; }

        /// <summary>
        /// Sets the LED to a specified color. 
        /// </summary>
        /// <param name="pixel">The pixel to set. 0-9</param>
        /// <param name="color">The color to set the pixel.</param>
        void SetPixelColor(int pixel, LedColor color);

        /// <summary>
        /// Turns off all the pixels.
        /// </summary>
        void ClearAllPixels();

    }
}
