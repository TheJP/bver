using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uebung01
{
    public partial class Form : System.Windows.Forms.Form
    {

        private Bitmap opencvImage;
        private Bitmap myImage;

        public Form()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Bayer();

            Timer t = new Timer();
            t.Interval = 500;
            t.Tick += (timer, arg) => {
                if (pictureBox.Image == opencvImage) { pictureBox.Image = myImage; Text = nameof(myImage); }
                else { pictureBox.Image = opencvImage; Text = nameof(opencvImage); }
            };
            t.Start();
        }

        private void Bayer()
        {
            var width = 800;
            var height = 600;
            var header = 1024;
            Mat img = new Mat(height, width, Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            byte[] data = new byte[height * width];
            using (Stream input = new FileStream("Schlittentest.raw", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                input.Read(data, 0, header); //Skip header
                input.Read(data, 0, height * width);
            }
            img.SetTo(data);
            Mat rgb = new Mat();
            CvInvoke.CvtColor(img, rgb, Emgu.CV.CvEnum.ColorConversion.BayerBg2Rgb);

            //Setup an own rgb image
            Mat mine = new Mat(height, width, Emgu.CV.CvEnum.DepthType.Cv8U, 3);
            byte[] myDat = new byte[3 * height * width];
            Func<int, byte> get = (loc) => data[Math.Max(0, Math.Min(data.Length - 1, loc))];
            Func<int, byte> myget = (loc) => myDat[Math.Max(0, Math.Min(myDat.Length - 1, loc))];
            Func<int, int[], byte> all = (loc, permutations) => (byte) permutations.Average(p => get(loc + p));
            Func<int, byte> green = (loc) => all(loc, new int[] { -width, -1, +1, width });
            Func<int, byte> redBlue = (loc) => all(loc, new int[] { -width - 1, width + 1, -width + 1, width - 1 });
            for (int v = 0; v < height; ++v)
            {
                var row = v * width;
                var rowEven = v % 2 == 0;
                for (int u = 0; u < width; ++u)
                {
                    var loc = row + u;
                    var colEven = u % 2 == 0;
                    myDat[3 * loc]     = colEven && rowEven ? data[loc] :
                        (!colEven && !rowEven ? redBlue(loc) : (byte)0);
                    myDat[3 * loc + 1] = colEven == rowEven ? green(loc) : data[loc];
                    myDat[3 * loc + 2] = !colEven && !rowEven ? data[loc] :
                        (colEven && rowEven ? redBlue(loc) : (byte)0);


                    //if (colEven && rowEven)
                    //{
                    //    myDat[3 * loc] = 0;
                    //    myDat[3 * loc + 1] = 0;
                    //    myDat[3 * loc + 2] = get(loc);
                    //}
                    //else if (!colEven && !rowEven)
                    //{
                    //    myDat[3 * loc] = get(loc);
                    //    myDat[3 * loc + 1] = 0;
                    //    myDat[3 * loc + 2] = 0;
                    //}
                    //else if (colEven && !rowEven)
                    //{
                    //    myDat[3 * loc] = 0;
                    //    myDat[3 * loc + 1] = get(loc);
                    //    myDat[3 * loc + 2] = 0;
                    //}
                    //else
                    //{
                    //    myDat[3 * loc] = 0;
                    //    myDat[3 * loc + 1] = get(loc);
                    //    myDat[3 * loc + 2] = 0;
                    //}

                    //"Perfect" Bayer
                    //if (colEven && rowEven)
                    //{
                    //    myDat[3 * loc] = get(loc);
                    //    myDat[3 * loc + 1] = (byte)((get(loc + 1) + get(loc + width)) / 2);
                    //    myDat[3 * loc + 2] = get(loc + width + 1);
                    //}
                    //else if (!colEven && !rowEven)
                    //{
                    //    myDat[3 * loc] = get(loc - width - 1);
                    //    myDat[3 * loc + 1] = (byte)((get(loc - 1) + get(loc - width)) / 2);
                    //    myDat[3 * loc + 2] = get(loc);
                    //}
                    //else if (colEven && !rowEven)
                    //{
                    //    myDat[3 * loc] = get(loc - width);
                    //    myDat[3 * loc + 1] = (byte)((get(loc) + get(loc - width + 1)) / 2);
                    //    myDat[3 * loc + 2] = get(loc + 1);
                    //}
                    //else
                    //{
                    //    myDat[3 * loc] = get(loc - 1);
                    //    myDat[3 * loc + 1] = (byte)((get(loc) + get(loc + width - 1)) / 2);
                    //    myDat[3 * loc + 2] = get(loc + width);
                    //}
                }
            }
            for (int v = 0; v < height; ++v)
            {
                var row = v * width;
                var rowEven = v % 2 == 0;
                for (int u = 0; u < width; ++u)
                {
                    var loc = row + u;
                    var colEven = u % 2 == 0;
                    if (colEven != rowEven)
                    {
                        myDat[3 * loc] = (byte)new int[]{ -width, -1, +1, width }.Average(p => myget(3*(loc + p)));
                        myDat[3 * loc + 2] = (byte)new int[] { -width, -1, +1, width }.Average(p => myget(3 * (loc + p) + 2));
                    }
                }
            }
            mine.SetTo(myDat);

            myImage = mine.Bitmap;
            opencvImage = rgb.Bitmap;
        }
    }
}
