using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BookManagement
{
    public class HSLColor
    {
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Lightness { get; set; }

        public HSLColor(double hue, double saturation, double lightness)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
        }

        public static HSLColor FromRGB(Color rgbColor)
        {
            double r = (double)rgbColor.R / 255;
            double g = (double)rgbColor.G / 255;
            double b = (double)rgbColor.B / 255;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            double hue = 0;
            double saturation = 0;
            double lightness = (max + min) / 2;

            if (delta != 0)
            {
                saturation = delta / (lightness < 0.5 ? (2 * lightness) : (2 - 2 * lightness));

                if (max == r)
                {
                    hue = ((g - b) / delta) % 6;
                }
                else if (max == g)
                {
                    hue = ((b - r) / delta) + 2;
                }
                else
                {
                    hue = ((r - g) / delta) + 4;
                }
            }

            hue = hue * 60;
            if (hue < 0)
            {
                hue += 360;
            }

            return new HSLColor(hue, saturation, lightness);
        }

        public Color ToRGB()
        {
            double c = (1 - Math.Abs(2 * Lightness - 1)) * Saturation;
            double x = c * (1 - Math.Abs((Hue / 60) % 2 - 1));
            double m = Lightness - c / 2;

            double r, g, b;

            if (Hue < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (Hue < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (Hue < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (Hue < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (Hue < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }

            r += m;
            g += m;
            b += m;

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
    }
}
