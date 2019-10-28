using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyLogic.ImageProcessing
{
    public class Corrector
    {

        private double[,] valueImage;
        private double[,] H;
        private double[,] S;
        private double[,] V;


        public int Alpha { get; set; } = 6;
        public Corrector(Bitmap bitmap)
        {
            valueImage = new double[bitmap.Width, bitmap.Height];
            H = new double[bitmap.Width, bitmap.Height];
            S = new double[bitmap.Width, bitmap.Height];
            V = new double[bitmap.Width, bitmap.Height];

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    valueImage[i, j] = bitmap.GetPixel(i, j).GetBrightness();
                    ColorToHSV(bitmap.GetPixel(i, j), out double h, out double s, out double v);
                    H[i, j] = h;
                    S[i, j] = s;
                    V[i, j] = v;
                    valueImage[i, j] = V[i, j] * 255;

                }
            }
            Console.WriteLine();
        }

        private double GetDiference(int x1, int y1, int x2, int y2)
        {
            return valueImage[x1, y1] - valueImage[x2, y2];
        }


        public Bitmap Process()
        {
            double sdmin = double.MaxValue;

            for (int i = 0; i < valueImage.GetLength(0) - 8; i++)
            {
                for (int j = 0; j < valueImage.GetLength(1) - 8; j++)
                {
                    double std = valueImage
                        .SubMatrix(i, j, i + 7, j + 7)
                        .AsIEnumerable()
                        .StdDev();
                    sdmin = Math.Min(sdmin, std);
                }
            }
            double p = sdmin * Alpha;
            double[,] Vn = valueImage.SubMatrix(0, 0, valueImage.GetLength(0) - 1, valueImage.GetLength(1) - 1);
            double[,] Vm = new double[valueImage.GetLength(0), valueImage.GetLength(1)];

            int nei = 4;
            for (int i = 0; i < Vm.GetLength(0); i++)
            {
                for (int j = 0; j < Vm.GetLength(1); j++)
                {
                    int fi = Math.Max(i - nei, 0);
                    int fj = Math.Max(j - nei, 0);
                    int ti = Math.Min(i + nei, Vm.GetLength(0) - 1);
                    int tj = Math.Min(j + nei, Vm.GetLength(1) - 1);

                    double avg = Vn.SubMatrix(fi, fj, ti, tj).AsIEnumerable().Average();
                    Vm[i, j] = avg;
                }
            }



            for (int i = 2; i < valueImage.GetLength(0) - 2; i++)
            {
                for (int j = 2; j < valueImage.GetLength(1) - 2; j++)
                {

                    var B = valueImage
                        .SubMatrix(i - 1, j - 1, i + 1, j + 1);
                    var uu = valueImage[i - 2, j];
                    var dd = valueImage[i + 2, j];
                    var ll = valueImage[i, j - 2];
                    var rr = valueImage[i, j + 2];

                    double corr = 0;
                    var BB = valueImage
                        .SubMatrix(i - 1, j - 1, i + 1, j + 1);
                    BB[1, 1] = BB[0, 0];

                    double sdd = BB.AsIEnumerable().StdDev();

                    double mm = (B[0, 0] + B[0, 1] + B[0, 2] + B[1, 0]) / 4;
                    if (sdd < p / 4)
                    {
                        if (Math.Abs(mm - Vn[i, j]) > p / 4)
                        {
                            Vn[i, j] = Vm[i, j];
                        }
                    }

                    // |
                    // |->
                    // |
                    double a = B[0, 1];
                    double b = B[1, 1];
                    double c = B[2, 1];
                    double da = B[0, 1] - B[0, 2];
                    double db = B[1, 1] - B[1, 2];
                    double dc = B[2, 1] - B[2, 2];


                    // d = (abs(da) + abs(db) + abs(dc)) / 3;
                    double d = Math.Min(Math.Abs(da), Math.Min(Math.Abs(db), Math.Abs(dc)));
                    d /= p;
                    if (d > 1)
                    {
                        d = 1;
                    }
                    d = 1 - d;
                    double diff = B[1, 2] - B[1, 1];
                    corr = corr + d * diff;


                    //    |
                    // < -|
                    //    |
                    a = B[0, 1];
                    b = B[1, 1];
                    c = B[2, 1];
                    da = a - B[0, 0];
                    db = b - B[1, 0];
                    dc = c - B[2, 0];


                    // % d = (abs(da) + abs(db) + abs(dc)) / 3;
                    d = Math.Min(Math.Abs(da), Math.Min(Math.Abs(db), Math.Abs(dc)));
                    d = d / p;
                    if (d > 1)
                        d = 1;
                    d = 1 - d;
                    diff = B[1, 0] - B[1, 1];

                    corr = corr + d * diff;


                    //%   ||
                    //% _ _ _
                    //%
                    a = B[1, 0];
                    b = B[1, 1];
                    c = B[1, 2];
                    da = a - B[0, 0];
                    db = b - B[0, 1];
                    dc = c - B[0, 2];

                    // % d = (abs(da) + abs(db) + abs(dc)) / 3;
                    d = Math.Min(Math.Abs(da), Math.Min(Math.Abs(db), Math.Abs(dc)));
                    d = d / p;
                    if (d > 1)
                        d = 1;
                    d = 1 - d;
                    diff = B[0, 1] - B[1, 1];
                    corr = corr + d * diff;

                    // _ _ _
                    //   ||
                    a = B[1, 0];
                    b = B[1, 1];
                    c = B[1, 2];
                    da = a - B[2, 1];
                    db = b - B[2, 1];
                    dc = c - B[2, 2];


                    // d = [abs[da] + abs[db] + abs[dc]] / 3;
                    d = Math.Min(Math.Abs(da), Math.Min(Math.Abs(db), Math.Abs(dc)));

                    d = d / p;

                    if (d > 1)
                        d = 1;

                    d = 1 - d;
                    diff = B[2, 1] - B[1, 1];

                    corr = corr + d * diff;


                    // \ 
                    // < -\
                    //    \
                    a = B[0, 0]; b = B[1, 1]; c = B[2, 2];
                    da = a - ll; db = b - B[2, 0]; dc = c - dd;

                    // d = [Math.Abs(da] + Math.Abs(db] + Math.Abs(dc]] / 3;
                    d = Math.Min(Math.Abs(da), Math.Min(Math.Abs(db), Math.Abs(dc)));

                    d = d / p;

                    if (d > 1)
                        d = 1;

                    d = 1 - d;
                    diff = B[2, 0] - B[1, 1];

                    corr = corr + d * diff;


                    // \ 
                    //   \->
                    //    \
                    a = B[0, 0]; b = B[1, 1]; c = B[2, 2];
                    da = a - uu; db = b - B[0, 2]; dc = c - rr;


                    // d = [Math.Abs(da] + Math.Abs(db] + Math.Abs(dc]] / 3;
                    d = Math.Min(Math.Abs(da), Math.Min(Math.Abs(db), Math.Abs(dc)));

                    d = d / p;

                    if (d > 1)
                        d = 1;


                    d = 1 - d;
                    diff = B[0, 2] - B[1, 1];

                    corr = corr + d * diff;



                    //     /
                    // < -/
                    // /
                    a = B[0, 2]; b = B[1, 1]; c = B[2, 0];
                    da = a - uu; db = b - B[0, 0]; dc = c - ll;


                    // d = [Math.Abs(da] + Math.Abs(db] + Math.Abs(dc]] / 3;
                    d = Math.Min(Math.Abs(da), Math.Min(Math.Abs(db), Math.Abs(dc)));

                    d = d / p;

                    if (d > 1)
                        d = 1;


                    d = 1 - d;
                    diff = B[0, 0] - B[1, 1];

                    corr = corr + d * diff;



                    //     /
                    //   / ->
                    // /
                    a = B[0, 2]; b = B[1, 1]; c = B[2, 0];
                    da = a - rr; db = b - B[2, 2]; dc = c - dd;


                    // d = [Math.Abs(da] + Math.Abs(db] + Math.Abs(dc]] / 3;
                    d = Math.Min(Math.Abs(da), Math.Min(Math.Abs(db), Math.Abs(dc)));
                    d = d / p;

                    if (d > 1)
                        d = 1;


                    d = 1 - d;
                    diff = B[2, 2] - B[1, 1];

                    corr = corr + d * diff;

                    Vn[i, j] = Vn[i, j] + corr / 8;
                }
            }


            Bitmap nBitmap = new Bitmap(H.GetLength(0), H.GetLength(1));
            for (int i = 0; i < nBitmap.Width; i++)
            {
                for (int j = 0; j < nBitmap.Height; j++)
                {
                    nBitmap.SetPixel(i, j, HsvToColor(H[i, j], S[i, j], Vn[i, j] / 255));
                }
            }
            return nBitmap;
        }


        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color HsvToColor(double h, double S, double V)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }

            int r = Clamp((int)(R * 255.0));
            int g = Clamp((int)(G * 255.0));
            int b = Clamp((int)(B * 255.0));
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }



    }
}
