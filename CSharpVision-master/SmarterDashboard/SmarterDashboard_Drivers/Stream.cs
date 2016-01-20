using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Linq;


namespace SmarterDashboard_Drivers
{
    class Stream
    {
        public readonly Size DefaultResolution = new Size(640, 480);
        protected readonly int DefaultFPS = 30;
        public readonly int DefaultCompression = -1;

        public delegate void UpdateGuiOnNewFrameFunction(Image proccessedFrame);

        UpdateGuiOnNewFrameFunction updateGuiFunction;
        protected ImageProcessor imgProcessor;

        public Image OriginalCurrentFrame { get; private set; }
        public Image ProccessedCurrentFrame { get; private set; }
        public bool isRunning = false;
        public bool newData = false;
        public ImageProcessor.ToteDetectionType ToteDetectionMethod = ImageProcessor.ToteDetectionType.Edges;
        public bool detectBins = true;

        //counters
        private System.Diagnostics.Stopwatch delayStopWatch;
        protected int fpsCounter = 0;
        protected int delayCounter = 0;
        protected double bitsReceivedCounter = 0;

        //current
        public int Fps { get; private set; } // actual current Fps
        public int Delay { get; private set; }
        public long Bits { get; protected set; }



        public static readonly byte[] MAGIC_NUMBERS = { 1, 0, 0, 0 };
        public static readonly Size[] SupportedResolutions = { new Size(640, 480), new Size(320, 240), new Size(160, 120) };
        int current_resolution_index; //Resolution == SupportedResolution[current_resolution_index]
        Thread cameraStreamThread;
        Socket cameraSocket;
        bool stopStream = false;
        long _bits;



        public Stream(UpdateGuiOnNewFrameFunction UpdateGuiOnNewFrameFunction = null)
        {
            this.updateGuiFunction = UpdateGuiOnNewFrameFunction;
            delayStopWatch = new System.Diagnostics.Stopwatch();
            current_resolution_index = 0;
            imgProcessor = new ImageProcessor();
        }

        public void Init()
        {
            if (cameraSocket != null)
            {
                throw new Exception("Stream Allready Initialized. Use RestartStream To Create A New One");
            }
            if (cameraStreamThread == null)
                cameraStreamThread = new Thread(WorkerThread);
            if (!cameraStreamThread.IsAlive)
                cameraStreamThread.Start();
        }

        public void StopStream()
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

        public void RestartStream()
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
                    Thread.Sleep(250);

                    continue;
                }
                // no errors
                isRunning = true;
                break;
            }
            byte[] data = null;
            Queue<byte> magic = new Queue<byte>();
            int badMagicCount = 0;

            //connection made, start receiving packets
            while (!stopStream)
            {
                try
                {
                    //check magic
                    byte[] b = new byte[1];
                    cameraSocket.Receive(b);
                    
                    if (b[0] == 0)
                        badMagicCount++;
                    if (badMagicCount > 100)
                        RestartStream();

                    magic.Enqueue(b[0]);
                    if (magic.Count > MAGIC_NUMBERS.Length)
                        magic.Dequeue();

                    if (!Enumerable.SequenceEqual(magic, MAGIC_NUMBERS))
                        continue;

                    badMagicCount = 0;

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
                    if (stopStream) break;
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



        // Called For Each Frame Received
        protected void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //Restart The Stopwatch To Calculate How Much Time Does It Take To Process The Image
            delayStopWatch.Restart();


            ProccessedCurrentFrame = imgProcessor.ProcessImage((Bitmap)eventArgs.Frame.Clone(), ToteDetectionMethod, detectBins);
            updateGuiFunction(ProccessedCurrentFrame);


            newData = true;
            fpsCounter++;
            delayStopWatch.Stop();
            delayCounter += (int)delayStopWatch.ElapsedMilliseconds;
        }

        // Called Each Second By Timer And Updates Fps Label
        public virtual void UpdateStreamMonitoringVariables()
        {
            Fps = fpsCounter;
            if (fpsCounter != 0) Delay = delayCounter / fpsCounter;
            Bits = (int)(bitsReceivedCounter / 1000000);

            bitsReceivedCounter = 0;
            fpsCounter = 0;
            delayCounter = 0;
            Bits = _bits;
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

        public double GetAngleToBin()
        {
            return imgProcessor.angleToBin;
        }

        public void LoadLastThreshData(string path)
        {
            imgProcessor.LoadLastThreshData(path);
        }
    }
}