using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SmarterDashboard_Programmers
{
    class VisionImage
    {

        public VisionImage(Image colorImage, DepthMap depthMap)
        {
            this.colorImage = colorImage;
            this.depthMap = depthMap;
        }

        public DepthMap depthMap { get; private set; }

        public Image colorImage { get; private set; }
    }
}
