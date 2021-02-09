using System.Collections.Generic;
using Drawing = System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace TermoClient
{
    class PrepareCsv
    {
        private string f_FileName;
        public int Width { get; set; }
        public int Height { get; set; }
        private float f_Min;
        private float f_Max;
        private float f_K;
        private BitmapImage f_Img;
        public BitmapImage Img
        {
            get => f_Img;
        }

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

                    var values = line.Split(',').Select(a =>
                    {
                        if (float.TryParse(a, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out float number))
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
            Bitmap newBitmap = new Bitmap(Width, Height);


            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    Color newColor = GetColorFromValue(data[j][i]);

                    newBitmap.SetPixel(i, j, newColor);
                }
            }

            f_Img = BitmapToImageSource(newBitmap);
        }
               
        public BitmapImage GetImage => f_Img;

        private Color GetColorFromValue(float value)
        {
            int R = (int)((255 * (value - f_Min) * f_K) / 100);
            int B = (int)((255 * (100 - (value - f_Min) * f_K)) / 100);

            return Color.FromArgb(R, 0, B);
        }

        private void TwoDimMinMax(float[][] data)
        {
            f_Max = data[0][0];
            f_Min = data[0][0];

            for (int j = 0; j < Width; j++)
            {
                for (int i = 0; i < Height; i++)
                {
                    if (data[i][j] > f_Max) f_Max = data[i][j];
                    if (data[i][j] < f_Min) f_Min = data[i][j];
                }
            }

            var zLvl = f_Max - f_Min;
            f_K = 100 / zLvl;
        }

        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
