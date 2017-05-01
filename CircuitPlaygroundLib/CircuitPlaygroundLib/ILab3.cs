using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitPlaygroundLib
{
    public interface ILab3
    {
        /// <summary>
        /// Immediately sets the RGB LED to the starting color. Then fades the 
        /// LED to the ending color over the specified time.
        /// </summary>
        /// <param name="pixel">The LED to set. 0-9</param>
        /// <param name="start">The starting color.</param>
        /// <param name="end">The ending color.</param>
        /// <param name="fadeTime">The time in milliseconds to apply the fade over.</param>
        /// <param name="repeat">Repeats the fade if true. Defaults to false.</param>
        void SetPixelCrossFade(int pixel, LedColor start, LedColor end,
        int fadeTime, bool repeat = false);

        /// <summary>
        /// Determines if the Circuit Playground is being held up intead of flat on a surface. 
        /// </summary>
        /// <returns>Returns true if being held.</returns>
        bool IsBeingHeld();

        /// <summary>
        /// Sets the LEDs to the specified pattern. The pattern comes from Lab 2 Part 4.
        /// ID of 0 will be the full 3 part pattern.
        /// ID of 1 will be the first pattern.
        /// ID of 2 will be the second pattern.
        /// ID of 3 will be the third pattern. 
        /// </summary>
        /// <param name="id">The ID of the pattern to change to.</param>
        void SetPattern(int id);

        /// <summary>
        /// Exports a log of the light sensor data to a csv file.
        /// </summary>
        /// <param name="filename">The filename to export as.</param>
        void ExportSensorData(string filename);
    }
}
