using System;
using Emgu.CV.Structure;
using Emgu.CV;

namespace SmarterDashboard_Programmers
{
    class HsvThreshData : ThreshData
    {
        public Image<Gray, byte> InRange(Image<Hsv, byte> image)
        {
            return image.InRange((Hsv)Min, (Hsv)Max);
        }
    }

    class BgrThreshData : ThreshData
    {
        public Image<Gray, byte> InRange(Image<Bgr, byte> image)
        {
            return image.InRange((Bgr)Min, (Bgr)Max);
        }
    }

    abstract class ThreshData
    {
        public IColor Min, Max;
        public int Erode, Dilate;

        public void Blur(ref Image<Gray, byte> img)
        {
            img = img.Erode(Erode).Dilate(Dilate); // Remove noise of small blobs that come from objects that aren't vision targets (white noise)
            img = img.Dilate(Dilate).Erode(Erode); // Remove noise that's inside the vision targets' blobs (black noise)
            // TODO: probably no need for the second line when detecting vision targets, only when detecting complex things (such as things that reflect light, like the bins from recycle rush)
        }
    }
}
