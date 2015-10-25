using Emgu.CV;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Emgu.CV.CvEnum;

namespace PGM
{
    class Program
    {
        static void Main(string[] args)
        {   
            if (args.Length != 2 || !File.Exists(args[0]))
            {
                Console.WriteLine("Usage: {0} <file from> <file to>", Path.GetFileName(Assembly.GetEntryAssembly().Location));
                return;
            }
            Mat image = CvInvoke.Imread(args[0], LoadImageType.Grayscale);
            if(image.Cols * image.Rows == 0)
            {
                Console.Error.WriteLine("Input file format not supported");
                return;
            }
            using (TextWriter writer = new StreamWriter(args[1], false, Encoding.ASCII))
            {
                //Saves a lot of space to use unix line endings instead of windows
                //(in the bridge example it's a compresion rate of about 1.3)
                writer.NewLine = "\n";
                writer.WriteLine("P2"); //Type: P2 = Graymap / ASCII
                writer.WriteLine("#Converted by JP"); //Comment
                writer.WriteLine($"{image.Cols} {image.Rows}"); //Width and Height
                writer.WriteLine("255"); //Depths 0-255 allowed
                var size = image.Cols * image.Rows;
                byte[] data = image.GetData();
                for (int i = 0; i < size; ++i)
                {
                    writer.WriteLine(data[i]);
                }
            }
        }
    }
}
