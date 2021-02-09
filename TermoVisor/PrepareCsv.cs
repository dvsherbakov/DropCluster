using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TermoVisor
{
    public class PrepareCsv
    {
        private string f_FileName;
        private int Width { get; set; }
        private int Height { get; set; }
        private float f_Min;
        private float f_Max;
        private float f_K;
        private BitmapImage f_Img;
        public BitmapImage Img => f_Img;
        private float[][] f_RawData;
        public PrepareCsv()
        {
            Width = 0;
            Height = 0;
        }
        public void OpenFile(string fName)
        {
            f_FileName = fName;
            var result = new List<float[]>();

            using (var reader = new StreamReader(fName))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line == null) continue;
                    var values = line.Split(',').Select(a =>
                    {
                        if (float.TryParse(a, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
                            return number;
                        return 0;
                    }).ToArray();
                    if (values.Length > Width) Width = values.Length;
                    result.Add(values);
                }
            }
            Height = result.Count;

            GenerateBitmap(result.ToArray());
        }

        private void GenerateBitmap(float[][] data)
        {
            TwoDimMinMax(data);
            var newBitmap = new Bitmap(Width, Height);


            for (var j = 0; j < Height; j++)
            {
                for (var i = 0; i < Width; i++)
                {
                    var newColor = GetColorFromValue(data[j][i]);

                    newBitmap.SetPixel(i, j, newColor);
                }
            }

            f_RawData = data;
            f_Img = BitmapToImageSource(newBitmap);
        }

        public BitmapImage GetImage => f_Img;

        private System.Drawing.Color GetColorFromValue(float value)
        {
            var r = (int)((255 * (value - f_Min) * f_K) / 100);
            var b = (int)((255 * (100 - (value - f_Min) * f_K)) / 100);

            return System.Drawing.Color.FromArgb(r, 0, b);
        }

        private void TwoDimMinMax(float[][] data)
        {
            f_Max = data[0][0];
            f_Min = data[0][0];

            for (var j = 0; j < Width; j++)
            {
                for (var i = 0; i < Height; i++)
                {
                    if (data[i][j] > f_Max) f_Max = data[i][j];
                    if (data[i][j] < f_Min) f_Min = data[i][j];
                }
            }

            var zLvl = f_Max - f_Min;
            f_K = 100 / zLvl;
        }

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = memory;
                imageSource.CacheOption = BitmapCacheOption.OnLoad;
                imageSource.EndInit();

                return imageSource;
            }
        }

        public float GetTemp(int x, int y)
        {
            if (y > 0 && y < Height && x>0 && x<Width)
                return f_RawData[y][x];
            return 0f;
        }
    }
}
