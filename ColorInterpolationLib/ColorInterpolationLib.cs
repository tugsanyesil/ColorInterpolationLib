using System.Drawing;
using System;
using System.Linq;
using System.IO;

namespace ColorInterpolationLib
{
    public class ColorInterpolation
    {
        public static ColorInterpolation Sample => new ColorInterpolation(new Color[] { Color.Black, Color.White }, new double[] { 0, 1 });

        public Color[] Colors;
        public readonly double[] Doubles;

        public ColorInterpolation(Color[] Colors, double[] Doubles)
        {
            var sorted = Colors.Zip(Doubles, (c, d) => new Tuple<Color, double>(c, d)).ToList();
            sorted.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));

            for (int i = 0; i < sorted.Count; i++)
            {
                Colors[i] = sorted[i].Item1;
                Doubles[i] = sorted[i].Item2;
            }

            this.Colors = Colors;
            this.Doubles = Doubles;
        }

        public double ColorToInt(Color Color)
        {
            return 0;
        }

        public Color? DoubleToColor(double Double)
        {
            int min = 0;
            int max = 0;
            for (int i = 0; i < Doubles.Length; i++)
            {
                if (Doubles[i] == Double) { return Colors[i]; }
                else if (Doubles[i] > Double)
                {
                    max = i;
                    min = i - 1;
                    if (min < 0 && min >= Doubles.Length || max < 0 && max >= Doubles.Length) { return null; }
                    break;
                }
            }

            var cmin = Colors[min];
            var cmax = Colors[max];
            var dmin = Doubles[min];
            var dmax = Doubles[max];

            return
                Color.FromArgb
                (
                    (int)Interpolation(Double, cmin.R, cmax.R, dmin, dmax),
                    (int)Interpolation(Double, cmin.G, cmax.G, dmin, dmax),
                    (int)Interpolation(Double, cmin.B, cmax.B, dmin, dmax)
                );
        }

        public double Interpolation(double M, double Min, double Max, double oldMin, double oldMax) => (Max - Min) * (M - oldMin) / (oldMax - oldMin) + Min;

        public void Save(string FileName)
        {
            if (!File.Exists(FileName)) { File.Create(FileName).Close(); }

            string[] Lines = new string[Doubles.Length];
            for (int i = 0; i < Doubles.Length; i++)
            {
                Lines[i] += Colors[i].A.ToString() + " " +
                            Colors[i].R.ToString() + " " +
                            Colors[i].G.ToString() + " " +
                            Colors[i].B.ToString() + " " +
                            Doubles[i].ToString();
            }

            File.WriteAllLines(FileName, Lines);
        }
        public static ColorInterpolation Load(string FileName)
        {
            if (File.Exists(FileName))
            {
                string[] Lines = File.ReadAllLines(FileName);

                if (Lines.Length > 0)
                {
                    Color[] Colors = new Color[Lines.Length];
                    double[] Doubles = new double[Lines.Length];

                    for (int i = 0; i < Lines.Length; i++)
                    {
                        string[] Line = Lines[i].Split(' ');
                        if (Line.Length == 5)
                        {
                            Colors[i] = Color.FromArgb(int.Parse(Line[0]), int.Parse(Line[1]), int.Parse(Line[2]), int.Parse(Line[3]));
                            Doubles[i] = double.Parse(Line[4]);
                        }
                        else
                        {
                            return Sample;
                        }
                    }

                    return new ColorInterpolation(Colors, Doubles);
                }
                else
                {
                    return Sample;
                }
            }
            else
            {
                return Sample;
            }
        }
    }
}