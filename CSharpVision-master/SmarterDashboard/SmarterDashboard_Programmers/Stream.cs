using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
namespace SmarterDashboard_Programmers
{
    enum CameraTypes { IPCamera, USBCamera, Webcam, Kinect, Jetson }

    #region JetsonStream
    class JetsonStream : Stream
    {
        private const bool ROBORIO_BRIDGE = false;
        private const String MANUAL_JETSON_IP = "192.168.1.215";
        private const int PORT = 14590;
        private const String ROBORIO_IP = "roboRIO-4590.local";
        private Thread netThread;

        private bool working;

        public JetsonStream(bool isBgr, UpdateGuiOnNewFrameFunction UpdateGuiOnNewFrameFunction = null)
            : base(isBgr, UpdateGuiOnNewFrameFunction)
        { }

        public override void Init()
        {
            working = true;

            netThread = new Thread(netThreadFunc);
            netThread.Start();

        }
        public override void StopStream()
        {
            working = false;
            netThread.Abort();
        }
        public override void RestartStream()
        {
            StopStream();
            Init();
        }

        public override void ChangeResolution(int resolutionWidth)
        {
            MessageBox.Show("Can't change JetsonStream resolution");
        }

        public void netThreadFunc()
        {
            if (ROBORIO_BRIDGE)
            {

            } else {
                BitmapPacketReceiver rec = new BitmapPacketReceiver(PORT, MANUAL_JETSON_IP);
                rec.startClient();
                while (working)
                {
                    stream_NewFrame(null, new NewFrameEventArgs(rec.receiveImage()));
                }
                rec.closeClient();
            }
        }
    }
    #endregion

    //Stream By The Kinect sensor
    #region KinectStream
    class KinectStream : Stream
    {
        private KinectSensor kinectSensor = null;

        private string connectedStatus;

        private const int MaxDepthDistance = 4000;
        private const int MinDepthDistance = 800;
        private const int MaxDepthDistanceOffset = 3200;

        


        public KinectStream(bool isBgr, UpdateGuiOnNewFrameFunction UpdateGuiOnNewFrameFunction = null)
            : base(isBgr, UpdateGuiOnNewFrameFunction)
        {}

        public override void Init()
        {
            if (kinectSensor != null)
            {
                throw new Exception("Stream Allready Initialized. Use RestartStream To Create A New One");
            }
            DiscoverKinectSensor();
            
        }
        public override void StopStream()
        {
            if (kinectSensor != null)
            {
                kinectSensor.Stop();
                kinectSensor.Dispose();
            }
        }
        public override void RestartStream()
        {
            StopStream();
            Init();
        }

        public override void ChangeResolution(int resolutionWidth)
        {
            MessageBox.Show("Can't change KinectStream resolution");
        }

        private bool InitializeKinect()
        {
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            kinectSensor.ColorStream.Enable(ColorImageFormat.YuvResolution640x480Fps15);
            kinectSensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinectSensor_AllFramesReady);


            try
            {
                kinectSensor.Start();
            }
            catch
            {
                this.connectedStatus = "Unable to start the Kinect Sensor";
                return false;
            }
            return true;
        }
        public void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (this.kinectSensor == e.Sensor)
            {
                if (e.Status == KinectStatus.Disconnected ||
                    e.Status == KinectStatus.NotPowered)
                {
                    this.kinectSensor = null;
                    this.DiscoverKinectSensor();
                }
            }
        }

        private void DiscoverKinectSensor()
        {
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    // Found one, set our sensor to this
                    kinectSensor = sensor;
                    break;
                }
            }

            if (kinectSensor == null)
            {
                this.connectedStatus = "Found none Kinect Sensors connected to USB";
                return;
            }

            // You can use the kinectSensor.Status to check for status
            // and give the user some kind of feedback
            switch (kinectSensor.Status)
            {
                case KinectStatus.Connected:
                    {
                        this.connectedStatus = "Status: Connected";
                        break;
                    }
                case KinectStatus.Disconnected:
                    {
                        this.connectedStatus = "Status: Disconnected";
                        break;
                    }
                case KinectStatus.NotPowered:
                    {
                        this.connectedStatus = "Status: Connect the power";
                        break;
                    }
                default:
                    {
                        this.connectedStatus = "Status: Error";
                        break;
                    }
            }
            Console.WriteLine("Kinect " + this.connectedStatus);

            // Init the found and connected device
            if (kinectSensor.Status == KinectStatus.Connected)
            {
                this.InitializeKinect();
            }
        }

        private Bitmap ImageToBitmap(ColorImageFrame Image)
        {
            byte[] pixeldata = new byte[Image.PixelDataLength];
            Image.CopyPixelDataTo(pixeldata);
            Bitmap bmap = new Bitmap(Image.Width, Image.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Drawing.Imaging.BitmapData bmapdata = bmap.LockBits(
                new Rectangle(0, 0, Image.Width, Image.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(pixeldata, 0, ptr, Image.PixelDataLength);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }
     

        public void kinectSensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null)
                {
                    using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
                    {
                        if (colorImageFrame != null)
                        {


                            byte[] pixelsFromFrame = new byte[colorImageFrame.PixelDataLength];

                            colorImageFrame.CopyPixelDataTo(pixelsFromFrame);

                            stream_NewDepthFrame(this, ImageToBitmap(colorImageFrame), SliceVisionImage(depthImageFrame));
                        }
                    }

                }
            }
        }


        private DepthMap SliceVisionImage(DepthImageFrame image, int min = MinDepthDistance, int max = MaxDepthDistance)
        {
            int width = image.Width;
            int height = image.Height;

            //var depthFrame = image.Image.Bits;
            short[] rawDepthData = new short[image.PixelDataLength];

            image.CopyPixelDataTo(rawDepthData);

            var pixels = new byte[height * width * 4];
            int usage = 0;
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;
            for (int depthIndex = 0, colorIndex = 0;
                depthIndex < rawDepthData.Length && colorIndex < pixels.Length;
                depthIndex++, colorIndex += 4)
            {

                // Calculate the distance represented by the two depth bytes
                int depth = rawDepthData[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                if (depthIndex == rawDepthData.Length - 1)
                {
                    usage = depth;
                }
                // Map the distance to an intesity that can be represented in RGB

                if (depth > min && depth < max)
                {
                    // Apply the intensity to the color channels
                    pixels[colorIndex + BlueIndex] =  this.CalculateIntensityFromDistance(depth); //blue - depth precentage out of 255. 255 - 800mm 0 - 4000mm
                    pixels[colorIndex + GreenIndex] = (byte)(255 - ((depth & 0x000FF0) / 0x000010)); //green - depth divided by 16, used to save the depth
                    pixels[colorIndex + RedIndex] = (byte)((depth & 0x00000F) / 0x000001); //red                    
                }
            }

            BitmapSource src = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, pixels, width * 4);
            DepthMap map = new DepthMap(BitmapFromSource(src));
            return map;

        }

        private Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        private byte CalculateIntensityFromDistance(int distance)
        {
            // This will map a distance value to a 0 - 255 range
            // for the purposes of applying the resulting value
            // to RGB pixels.
            int newMax = distance - MinDepthDistance;
            if (newMax > 0)
                return (byte)(255 - (255 * newMax
                / (MaxDepthDistanceOffset)));
            else
                return (byte)255;
        }

    }
    #endregion

    //Stream By The Robot'S Camera
    #region IPCameraStream
    class IPCameraStream : Stream
    {
        private MJPEGStream stream;
        private const string CameraURL = "http://10.45.90.11/axis-cgi/mjpg/video.cgi?";
        private const float ResolutionRatio = 0.75f;
        public const int CameraFieldOfViewX = 47;
        public const int CameraFieldOfViewY = (int)(47.0f * ResolutionRatio);

        // Ctor
        public IPCameraStream(bool isBgr, UpdateGuiOnNewFrameFunction UpdateGuiOnNewFrameFunction = null)
            : base(isBgr, UpdateGuiOnNewFrameFunction)
        {
            ChangeFps(DefaultFPS);
            ChangeCompression(DefaultCompression);
        }

        public override void Init()
        {
            if (stream != null)
            {
                throw new Exception("Stream Allready Initialized. Use RestartStream To Create A New One");
            }
            stream = new MJPEGStream(CameraURL);
            stream.NewFrame += stream_NewFrame;
            stream.Proxy = null;
            setSource();
            stream.Start();
            isRunning = true;
        }

        public override void StopStream()
        {
            if (stream != null)
            {
                stream.SignalToStop();
                stream.WaitForStop();
                stream = null;
                isRunning = false;
            }
        }

        public override void RestartStream()
        {
            StopStream();
            Init();
        }

        private void setSource()
        {
            if (stream != null)
                stream.Source = CameraURL + "fps=" + wantedFps + "&resolution=" + Resolution.Width + "x" + Resolution.Height + "&compression=" + Compression;
        }

        public override void UpdateStreamMonitoringVariables()
        {
            base.UpdateStreamMonitoringVariables();
            if (stream != null) Bits = (int)((stream.BytesReceived * 8) / 1000000.0);
        }

        public override void ChangeResolution(Size resolution)
        {
            base.ChangeResolution(resolution);
            setSource();
        }

        public override void ChangeResolution(int resolutionWidth)
        {
            ChangeResolution(new Size(resolutionWidth, resolutionWidth * (int)ResolutionRatio));
        }

        public override void ChangeFps(int fps)
        {
            base.ChangeFps(fps);
            setSource();
        }

        public override void ChangeCompression(int cmp)
        {
            base.ChangeCompression(cmp);
            setSource();
        }
    }
    #endregion


    //Stream By Webcamera (Camera That's Connected To The Computer)
    #region WebCameraStream
    class WebCameraStream : Stream
    {
        VideoCaptureDevice stream;
        VideoCapabilities[] vc;

        public WebCameraStream(bool isBgr, UpdateGuiOnNewFrameFunction UpdateGuiOnNewFrameFunction = null)
            : base(isBgr, UpdateGuiOnNewFrameFunction)
        {
        }

        public override void Init()
        {
            if (stream != null)
            {
                throw new Exception("Stream Allready Initialized. Use RestartStream To Create A New One");
            }

            stream = new VideoCaptureDevice(new FilterInfoCollection(FilterCategory.VideoInputDevice)[0].MonikerString);

            vc = stream.VideoCapabilities;
            ChangeResolution(Resolution.Width);
            stream.NewFrame += new NewFrameEventHandler(stream_NewFrame);
            stream.Start();
            isRunning = true;
        }

        public override void StopStream()
        {
            if (stream != null)
            {
                if (stream.IsRunning)
                {
                    stream.NewFrame -= stream_NewFrame;
                    stream.SignalToStop();
                    stream = null;
                    isRunning = false;
                }
            }
        }

        public override void RestartStream()
        {
            StopStream();
            Init();
        }

        public override void UpdateStreamMonitoringVariables()
        {
            base.UpdateStreamMonitoringVariables();
            if (stream != null) Bits = (int)((stream.BytesReceived * 8) / 1000000.0);
        }

        // Use y<0 to only care for resolution.width
        public override void ChangeResolution(Size resolution)
        {
            base.ChangeResolution(resolution);
            if (stream != null)
            {
                foreach (VideoCapabilities v in vc)
                {
                    if (v.FrameSize.Width == resolution.Width && (resolution.Height < 0 || resolution.Height == v.FrameSize.Height))
                    {
                        stream.VideoResolution = v;

                        base.ChangeResolution(new Size(v.FrameSize.Width, v.FrameSize.Height));
                        return;
                    }
                }
                string res = "";
                foreach (VideoCapabilities v in vc)
                {
                    res += v.FrameSize.Width + ":" + v.FrameSize.Height + ", ";
                }
                MessageBox.Show(string.Format("Resolution not supported by Webcam- Default Resolution Is Being Used. Supported resolutions are:\r\n {0}", res), "", MessageBoxButtons.OK);
            }
        }

        public override void ChangeResolution(int resolutionWidth)
        {
            ChangeResolution(new Size(resolutionWidth, -1));
        }

        public override void ChangeFps(int fps)
        {
            MessageBox.Show("Can't change webcam FPS");
        }

        public override void ChangeCompression(int cmp)
        {
            MessageBox.Show("Can't change webcam Compression");
        }
    }
    #endregion


    //Stream By The New Camera Connected To The Roborio Via USB
    #region USBCamera
    class USBCamera : Stream
    {
        public static readonly byte[] MAGIC_NUMBERS = { 1, 0, 0, 0 };
        public static readonly Size[] SupportedResolutions = { new Size(640, 480), new Size(320, 240), new Size(160, 120) };
        int current_resolution_index; //Resolution == SupportedResolution[current_resolution_index]
        Thread cameraStreamThread;
        Socket cameraSocket;
        bool stopStream = false;
        long _bits;


        public USBCamera(bool isBgr, UpdateGuiOnNewFrameFunction UpdateGuiOnNewFrameFunction = null)
            : base(isBgr, UpdateGuiOnNewFrameFunction)
        {
            ChangeFps(DefaultFPS);
            ChangeCompression(-1);
        }

        public override void Init()
        {
            if (cameraSocket != null)
            {
                throw new Exception("Stream Already Initialized. Use RestartStream To Create A New One");
            }
            if (cameraStreamThread == null)
                cameraStreamThread = new Thread(WorkerThread);
            if (!cameraStreamThread.IsAlive)
                cameraStreamThread.Start();
        }

        public override void StopStream()
        {
            if (cameraStreamThread != null && cameraStreamThread.IsAlive)
            {
                stopStream = true;
                cameraStreamThread = null;
            }
            if (cameraSocket != null)
            {
                cameraSocket.Close();
                cameraSocket.Dispose();
            }
            cameraSocket = null;
        }

        public override void RestartStream()
        {
            StopStream();
            Init();
        }

        public void WorkerThread()
        {
            isRunning = false;
            while (!stopStream)
            {
                try
                {
                    cameraSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //init connection
                    cameraSocket.Connect("roboRIO-4590.local", 1180);
                    cameraSocket.Send(IntToByteArray(new int[] { 30, -1, current_resolution_index }));
                }
                catch (SocketException)
                {
                    if (cameraSocket != null)
                    {
                        cameraSocket.Close();
                        cameraSocket = null;
                    }

                    Console.WriteLine("Connection Failed");//e.ToString());
                    //Thread.Sleep(250);

                    continue;
                }
                // no errors
                isRunning = true;
                break;
            }
            byte[] data = null;
            //connection made, start receiving packets
            while (!stopStream)
            {
                try
                {
                    //check magic
                    byte[] magic = new byte[MAGIC_NUMBERS.Length];
                    cameraSocket.Receive(magic);
                    if (!Enumerable.SequenceEqual(magic, MAGIC_NUMBERS))
                        continue;

                    //size
                    byte[] _size = new byte[sizeof(int)];
                    cameraSocket.Receive(_size);
                    int size = BytesToInt(_size);

                    //data
                    data = new byte[size];
                    int len = 0, actualSize = 0;
                    do
                    {
                        len = cameraSocket.Receive(data, actualSize, size - actualSize, 0);
                        actualSize += len;
                    }
                    while (actualSize < size);
                }
                catch (Exception)
                {
                    continue;
                }
                this._bits += data.Length;
                if (data.Length >= 4 && data[0] == 255 && data[1] == 216 && data[(data.Length - 2)] == 255 && data[(data.Length - 1)] == 217)
                {
                    //got new valid frame
                    //Console.WriteLine("Emperor BOBO");
                    //Bitmap img = null;
                    //using (MemoryStream ms = new MemoryStream(data))
                    //{
                    //    img = (Bitmap)Image.FromStream(ms).Clone(); //¯\_(ツ)_/¯
                    //}



                    //ImageConverter imgCon = new ImageConverter();
                    //Bitmap img = (Bitmap)imgCon.ConvertFrom(data);


                    Bitmap img;
                    using (MemoryStream memoryStream = new MemoryStream(data))
                    using (Image newImage = Image.FromStream(memoryStream))
                        img = new Bitmap(newImage);


                    stream_NewFrame(null, new NewFrameEventArgs(img));
                }
            }
            stopStream = false;
            isRunning = false;
        }



        public override void ChangeResolution(Size resolution)
        {
            if (isRunning)
            {
                for (int i = 0; i < SupportedResolutions.Length; i++)
                    if (SupportedResolutions[i].Width == resolution.Width &&
                        (resolution.Height == -1 || SupportedResolutions[i].Height == resolution.Height))
                    {
                        base.ChangeResolution(resolution);
                        current_resolution_index = i;
                        RestartStream();
                        return;
                    }
                MessageBox.Show("Resolution Not Supported. Used Default. Supported Resoultions Are: " + SupportedResolutions.ToString());
            }
            else
                base.ChangeResolution(resolution);
        }

        public override void ChangeResolution(int resolutionWidth)
        {
            ChangeResolution(new Size(resolutionWidth, -1));
        }

        public override void ChangeFps(int fps)
        {
            base.ChangeFps(fps);
        }

        public override void ChangeCompression(int cmp)
        {
            base.ChangeCompression(cmp);
        }



        public override void UpdateStreamMonitoringVariables()
        {
            base.UpdateStreamMonitoringVariables();
            base.Bits = _bits;
            _bits = 0;
        }


        public static int BytesToInt(byte[] bytes)
        {
            return bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3];
        }

        public static byte[] IntToByteArray(int[] nums)
        {
            byte[] ret = new byte[nums.Length * 4];
            int counter = 0;

            for (int i = 0; i < ret.Length; i += 4)
            {
                ret[i] = (byte)(nums[counter] >> 24);
                ret[i + 1] = (byte)((nums[counter] & (0xff << 16)) >> 16);
                ret[i + 2] = (byte)((nums[counter] & (0xff << 8)) >> 8);
                ret[i + 3] = (byte)(nums[counter] & 0xff);

                counter++;
            }
            return ret;
        }
    }
    #endregion




    //Abstract Class For Stream Type Classes
    #region abstract Stream
    abstract class Stream
    {
        protected readonly Size DefaultResolution = new Size(640, 480);
        protected readonly int DefaultFPS = 30;
        protected readonly int DefaultCompression = 30;

        public delegate void UpdateGuiOnNewFrameFunction(Image originalFrame, Image proccessedFrame, Image depthImage);

        UpdateGuiOnNewFrameFunction updateGuiFunction;
        protected ImageProcessor imgProcessor;
        protected int wantedFps; //wanted fps, not actual.

        public VisionImage OriginalCurrentFrame { get; private set; }
        public VisionImage ProccessedCurrentFrame { get; private set; }

        public VisionImage OriginalCurrentDepthFrame { get; private set; }
        public VisionImage ProccessedCurrentDepthFrame { get; private set; }

        public bool isRunning = false;
        public bool newData = false;
        public ImageProcessor.ToteDetectionType ToteDetectionMethod;

        //counters
        private System.Diagnostics.Stopwatch delayStopWatch;
        protected int fpsCounter = 0;
        protected int delayCounter = 0;
        protected double bitsReceivedCounter = 0;
        public bool detectBins;

        //current
        public Size Resolution { get; private set; }
        public int Compression { get; protected set; }
        public int Fps { get; private set; } // actual current Fps
        public int Delay { get; private set; }
        public long Bits { get; protected set; }


        public Stream(bool isBgr, UpdateGuiOnNewFrameFunction UpdateGuiOnNewFrameFunction = null)
        {
            this.updateGuiFunction = UpdateGuiOnNewFrameFunction;

            ChangeResolution(DefaultResolution);

            delayStopWatch = new System.Diagnostics.Stopwatch();

            if (isBgr) imgProcessor = new BgrImageProcessor();
            else imgProcessor = new HsvImageProcessor();
        }

        public abstract void Init();
        public abstract void StopStream();
        public abstract void RestartStream();


        // Called For Each Frame Received
        protected void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //Restart The Stopwatch To Calculate How Much Time Does It Take To Process The Image
            delayStopWatch.Restart();



            OriginalCurrentFrame = new VisionImage((Image)eventArgs.Frame.Clone(), null);
            try {
                ProccessedCurrentFrame = imgProcessor.ProcessImage((Bitmap)OriginalCurrentFrame.colorImage.Clone(), ToteDetectionMethod, detectBins, OriginalCurrentFrame.depthMap.Copy());
            }
            catch
            {
                ProccessedCurrentFrame = imgProcessor.ProcessImage((Bitmap)OriginalCurrentFrame.colorImage.Clone(), ToteDetectionMethod, detectBins);
            }
            updateGuiFunction(OriginalCurrentFrame.colorImage, ProccessedCurrentFrame.colorImage, null);

            newData = true;
            fpsCounter++;
            delayStopWatch.Stop();
            delayCounter += (int)delayStopWatch.ElapsedMilliseconds;
        }

        protected void stream_NewDepthFrame(object sender, Bitmap colorImage, DepthMap depthMap)
        {
            //Restart The Stopwatch To Calculate How Much Time Does It Take To Process The Image
            delayStopWatch.Restart();

            Image depthImage = depthMap.GetImage();
            Thread.Sleep(20);
            OriginalCurrentFrame = new VisionImage(colorImage, depthMap);
            ProccessedCurrentFrame = imgProcessor.ProcessImage((Bitmap)OriginalCurrentFrame.colorImage.Clone(), ToteDetectionMethod, detectBins, OriginalCurrentFrame.depthMap.Copy());

            updateGuiFunction(OriginalCurrentFrame.colorImage, ProccessedCurrentFrame.colorImage,
                depthImage);

            newData = true;
            fpsCounter++;
            delayStopWatch.Stop();
            delayCounter += (int)delayStopWatch.ElapsedMilliseconds;
        }

        // Called Each Second By Timer And Updates Fps Label
        public virtual void UpdateStreamMonitoringVariables()
        {
            Fps = fpsCounter;
            if (fpsCounter != 0) Delay = (delayCounter / fpsCounter ) ;
            Bits = (int)(bitsReceivedCounter / 1000000);

            bitsReceivedCounter = 0;
            fpsCounter = 0;
            delayCounter = 0;
        }



        // Changes The Resolution Variable And The Label
        public virtual void ChangeResolution(Size newResolution)
        {
            Resolution = newResolution;
        }

        public abstract void ChangeResolution(int resolutionWidth);

        // Change The Wanted Fps Of The Stream
        public virtual void ChangeFps(int fps)
        {
            this.wantedFps = fps;
        }

        // Change Compression
        public virtual void ChangeCompression(int cmp)
        {
            this.Compression = cmp;
        }



        // Toggles Between Bgr And Hsv Color Formats
        public void ChangeColorFormat(bool isBgr)
        {
            if (isBgr && !(imgProcessor is BgrImageProcessor))
                imgProcessor = new BgrImageProcessor();
            if (!isBgr && !(imgProcessor is HsvImageProcessor))
                imgProcessor = new HsvImageProcessor();
        }

        public void SetThreshMin(int a, int b, int c)
        {
            imgProcessor.SetThreshMin(a, b, c);
        }
        public void SetThreshMax(int a, int b, int c)
        {
            imgProcessor.SetThreshMax(a, b, c);
        }
        public IColor GetThreshMin()
        {
            return imgProcessor.GetThreshMin();
        }
        public IColor GetThreshMax()
        {
            return imgProcessor.GetThreshMax();
        }
        public void SetErode(int erode)
        {
            imgProcessor.SetErode(erode);
        }
        public void SetDilate(int dilate)
        {
            imgProcessor.SetDilate(dilate);
        }
        public int GetErode()
        {
            return imgProcessor.GetErode();
        }
        public int GetDilate()
        {
            return imgProcessor.GetDilate();
        }


        // Returns An Image After Being Processed
        public VisionImage ProcessCustomImage(Image img)
        {
            return imgProcessor.ProcessImage((Bitmap)img, ToteDetectionMethod, detectBins,null);
        }

        public double GetAngleToBin()
        {
            return imgProcessor.angleToBin;
        }
    }
    #endregion

}