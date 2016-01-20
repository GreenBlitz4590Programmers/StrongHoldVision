using System;
using Emgu.CV.Structure;
using Emgu.CV;

namespace SmarterDashboard_Drivers
{
    //{Format:HSV,MinHue:130,MinSaturation:148,MinValue:92,MaxHue:180,MaxSaturation:255,MaxValue:255,Blur:0}

    class ThreshData
    {
        // min and max colors for threshold
        public Hsv Min, Max;
        public int Erode, Dilate;



        /// <summary>
        /// Returns a Gray image after thresh by current thresh values
        /// </summary>
        /// <param name="img">Image to thresh</param>
        /// <returns>The image after threshold</returns>
        public Image<Gray, byte> InRange(Image<Hsv, byte> image)
        {
            return image.InRange((Hsv)Min, (Hsv)Max);
        }

        /// <summary>
        /// Pixalite the given image
        /// </summary>
        public void Blur(ref Image<Gray, byte> img)
        {
            img = img.Erode(Erode).Dilate(Dilate); // Remove noise of small blobs that come from objects that aren't vision targets (white noise)
            img = img.Dilate(Dilate).Erode(Erode); // Remove noise that's inside the vision targets' blobs (black noise)
            // TODO: probably no need for the second line when detecting vision targets, only when detecting complex things (such as things that reflect light, like the bins from recycle rush)
        }

        public void LoadFromFile(string path)
        {
            string data = System.IO.File.ReadAllText(path);

            data = data.Substring(1, data.Length - 2);
            string[] args = data.Split(',');

            foreach (string s in args)
            {
                string[] pair = s.Split(':'); // ((Key, Value))

                switch (pair[0])
                {
                    case "Min1": Min.Hue = int.Parse(pair[1]);
                        break;
                    case "Min2": Min.Satuation = int.Parse(pair[1]);
                        break;
                    case "Min3": Min.Value = int.Parse(pair[1]);
                        break;
                    case "Max1": Max.Hue = int.Parse(pair[1]);
                        break;
                    case "Max2": Max.Satuation = int.Parse(pair[1]);
                        break;
                    case "Max3": Max.Value = int.Parse(pair[1]);
                        break;
                    case "Erode": Erode = int.Parse(pair[1]);
                        break;
                    case "Dilate": Dilate = int.Parse(pair[1]);
                        break;
                }
            }
            //{Format:Hsv,Min1:130,Min2:148,Min3:92,Max1:180,Max2:255,Max3:255,Erode:0,Dilate:0}

        }
    }
}
