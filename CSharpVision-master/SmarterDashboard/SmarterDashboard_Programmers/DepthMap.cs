using System;
using System.Collections.Generic;
using System.Drawing;

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SmarterDashboard_Programmers
{

    
    /// <summary>
    /// Used to save depth image from a depth sensor
    /// </summary>
    class DepthMap
    {
        private Bitmap depthMap;

        private int height;
        private int width;

        
        public int GetDepth(int x, int y)
        {
            if (depthMap == null)
            {
                return 0;
            }
            System.Drawing.Color c = depthMap.GetPixel(x, y);
            return c.R + (4080 - c.G * 16);           
        }

        public DepthMap(Bitmap bmp)
        {
            depthMap = bmp;
            if (bmp == null)
            {
                depthMap = new Bitmap(640,480);
            }
            this.height = bmp.Height;
            this.width = bmp.Width;
        }

        public DepthMap Copy()
        {
            DepthMap copy = new DepthMap(depthMap);
            return copy;
        }

        private Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public Vector3D GetPixelInSpace(int x, int y, double horizontalAngle = 0, double verticalAngle = 0) {
            double vecZ = GetDepth(x, y);
            double vecY = (y * vecZ) / height - (height / 2d);
            double vecX = (x * vecZ) / width - (width / 2d);
            
            return new Vector3D(vecZ, vecX, vecY);
        }
       

        public Image GetImage()
        {
            return depthMap;
        }
    }
}
