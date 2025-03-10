using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Map
{
    public class Camp
    {
        public static Dictionary<int, Color> Colors = new Dictionary<int, Color>
        {
            // -1 meas grey
            { -1, HexToColor("606060") },
            { 0, HexToColor("ff0000") },
            { 1, HexToColor("00ff00") },
            { 2, HexToColor("0000ff") },
            { 3, HexToColor("99ffff") },
            { 4, HexToColor("a09077") },
            { 5, HexToColor("ffffff") },
            { 6, HexToColor("bbbbaa") },
            { 7, HexToColor("888888") },
            { 8, HexToColor("555555") },
            { 9, HexToColor("99aa77") },
            { 10, HexToColor("889977") },
            { 11, HexToColor("c9d29b") },
            { 12, HexToColor("708755") },
            { 13, HexToColor("8f965a") },
            { 14, HexToColor("88aa55") },
            { 15, HexToColor("d2b98b") },
            { 16, HexToColor("5f8128") },
            { 17, HexToColor("879944") }
        };
        static Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }
    }
}
