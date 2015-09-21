﻿using Emgu.CV;
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
        public Form()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Mat img = new Mat(600, 800, Emgu.CV.CvEnum.DepthType.Cv8U, 1);
            byte[] data = new byte[600 * 800];
            using(Stream input = new FileStream("Schlittentest.raw", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                input.Read(data, 0, 1024); //Skip header
                input.Read(data, 0, 600 * 800);
            }
            img.SetTo(data);
            Mat rgb = new Mat();
            CvInvoke.CvtColor(img, rgb, Emgu.CV.CvEnum.ColorConversion.BayerBg2Bgr);

            //panAndZoomPictureBox1.Image = img.Bitmap;
            panAndZoomPictureBox1.Image = rgb.Bitmap;
        }
    }
}