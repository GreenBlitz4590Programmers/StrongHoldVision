using System;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;
using Renci.SshNet;
using System.Threading;
using System.Net.Sockets;
using System.Net;

/** Omer, I've changed some stuff in the code, and I am leaving a few notes:
       - The DepthMap will never return null for the bitmap, it will initialize an empty bitmap, which will result black frame
       - The calling of the frame update will never use blank image, it will initialize an empty bitmap
       - The DepthMap initialized the empty bitmap on creation if the given bitmap is equal to null
       - Message on whatsup for any questions, because Oz is not going to let me go
       - Delete this comment
**/
namespace SmarterDashboard_Programmers
{
    public partial class ProgrammersWindow : MetroFramework.Forms.MetroForm
    {
        Stream stream;
        NetworkTable robotNetworkTable;

        public static string LastThreshDataFilePath = @"..\..\..\LastThreshData.txt";
        public const string CapturedImagesFolderPath = @"..\..\..\CapturedImages";
        PictureBox wooshPicture;
        System.Diagnostics.Stopwatch wooshCounter = new System.Diagnostics.Stopwatch();
        int wooshAnimationTime = 400;


        //Const Booleans Determining The Camera Type And The Color Format Being Used
        const bool isRoboRio = true;
        const CameraTypes cameraType = CameraTypes.IPCamera;
        public const bool dontStream = false;
        bool isBgr = false;



        //Ctor And Initializations
        public ProgrammersWindow()
        {
            InitializeComponent();
            //Initialize Timers
            tmrFps.Start();
            tmrNetworkTables.Start();


            openFileDialog1.InitialDirectory = CapturedImagesFolderPath;

            tmrWoosh.Tick += tmrWoosh_Tick;



            InitStream();
            InitNetworkTable();
            LoadLastThreshData();




            //Sets The Checkboxes By Desired Defaults
            chboxLockBars.Checked = false;
            chboxTotes.CheckState = CheckState.Checked;
            chboxBins.CheckState = CheckState.Unchecked;
            chboxUseNT.CheckState = CheckState.Checked;


            

            // Initialize Stream
            if (!dontStream)
                stream.Init();
        }

        void InitStream()
        {
            //Initialize The Stream Manager Object, Managing The Camera Stream And The Image Processing
            switch (cameraType)
            {
                case CameraTypes.Jetson:
                    stream = new JetsonStream(isBgr, UpdateGuiOnNewFrame);
                    break;
                case CameraTypes.Kinect:
                    stream = new KinectStream(isBgr, UpdateGuiOnNewFrame);
                    break;
                case CameraTypes.IPCamera:
                    stream = new IPCameraStream(isBgr, UpdateGuiOnNewFrame);
                    break;
                case CameraTypes.Webcam:
                    stream = new WebCameraStream(isBgr, UpdateGuiOnNewFrame);
                    break;
                case CameraTypes.USBCamera:
                default:
                    stream = new USBCamera(isBgr, UpdateGuiOnNewFrame);
                    break;
            }
        }

        void InitNetworkTable()
        {
            if (cameraType != CameraTypes.Webcam)
            {
                robotNetworkTable = new NetworkTable(NetworkTableNewData, isRoboRio);
                new Thread(() =>
                {
                    while (!stream.isRunning) Thread.Sleep(100);
                    robotNetworkTable.Start();
                });
            }
        }

        //Load The Thresh Data From The Last Time It Was Changed. <-- Lastthreshdata.Txt
        private void LoadLastThreshData()
        {
            string data = System.IO.File.ReadAllText(LastThreshDataFilePath);

            data = data.Substring(1, data.Length - 2);
            string[] args = data.Split(',');

            foreach (string s in args)
            {
                string[] pair = s.Split(':'); // ((Key, Value))

                switch (pair[0])
                {
                    case "Format": isBgr = pair[1] == "BGR";
                        rdioIsBgr.Checked = isBgr;
                        rdioHsv.Checked = !isBgr;
                        break;
                    case "Min1": barMin1.Value = int.Parse(pair[1]);
                        break;
                    case "Min2": barMin2.Value = int.Parse(pair[1]);
                        break;
                    case "Min3": barMin3.Value = int.Parse(pair[1]);
                        break;
                    case "Max1": barMax1.Value = int.Parse(pair[1]);
                        break;
                    case "Max2": barMax2.Value = int.Parse(pair[1]);
                        break;
                    case "Max3": barMax3.Value = int.Parse(pair[1]);
                        break;
                    case "Erode": barErode.Value = int.Parse(pair[1]);
                        break;
                    case "Dilate": barDilate.Value = int.Parse(pair[1]);
                        break;
                }
            }
            //{Format:Hsv,Min1:130,Min2:148,Min3:92,Max1:180,Max2:255,Max3:255,Erode:0,Dilate:0}

            //If we read all zero Bar.value won't change and min/max colors will point to null
            if (stream.GetThreshMin() == null)
                stream.SetThreshMin(0, 0, 0);
            if (stream.GetThreshMin() == null)
                stream.SetThreshMax(0, 0, 0);
        }

        //Stop The Stream When Form'S Closing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (robotNetworkTable != null) robotNetworkTable.Close();
            stream.StopStream();
            Environment.Exit(0);
        }


        //Timer Event That Updates All The Values That Should Be Sent To The Network Table
        private void tmrNetworkTables_Tick(object sender, EventArgs e)
        {
            if (robotNetworkTable != null && chboxUseNT.Checked && stream.OriginalCurrentFrame != null)
            {
                //Size res = stream.OriginalCurrentFrame.colorImage.Size;

                //TODO: calculate stuff

                robotNetworkTable.SetVariable("AngleToBin", stream.GetAngleToBin());
                //TODO: add more
            }
        }


        //Called Each Time The NetworkTable Receives Data
        private void NetworkTableNewData(string source, string key, object value, bool isNew)
        {
            string _value = value.ToString();
            if (!isNew)
            {
                MetroFramework.Controls.MetroLabel lbl = (MetroFramework.Controls.MetroLabel)pnlDashboard.Controls.Find(key, false)[0];
                lbl.Text = key + ": " + _value.Substring(0, Math.Min(8, _value.Length));
            }
            else
            {
                MetroFramework.Controls.MetroLabel newLabel = new MetroFramework.Controls.MetroLabel();
                newLabel.AutoSize = true;
                newLabel.Name = key;
                newLabel.Text = key + ": " + _value.Substring(0, Math.Min(8, _value.Length));
                newLabel.Location = new System.Drawing.Point(1, (pnlDashboard.Controls.Count - 2) * 20);

                pnlDashboard.Controls.Add(newLabel);
            }
        }


        //Each Second Update Frame Rate
        private void timerFps_Tick(object sender, EventArgs e)
        {
            if (stream != null)
            {
               
                stream.UpdateStreamMonitoringVariables();
                lblFps.Text = "FPS: " + stream.Fps;
                lblBits.Text = "MBi Received: " + stream.Bits / 1000000.0;
                lblImgPrcsDelay.Text = "Image Proccesing Delay: " + stream.Delay;

                lblRes.Text = string.Format("Resolution: {0}x{1}", stream.Resolution.Width, stream.Resolution.Height);
                lblCompression.Text = "Compression: " + stream.Compression;
                
            }
        }

        //Woosh Update
        void tmrWoosh_Tick(object sender, EventArgs e)
        {
            if (wooshCounter.ElapsedMilliseconds > wooshAnimationTime)
            {
                tmrWoosh.Stop();
                wooshCounter.Stop();
            }

            //----// #Lerping
            double amount = (double)wooshCounter.ElapsedMilliseconds / (double)wooshAnimationTime;
            wooshPicture.Size = new Size((int)(picNormal.Size.Width + (-picNormal.Size.Width) * amount), (int)(picNormal.Size.Height + (0 - picNormal.Size.Height) * amount));
            wooshPicture.Location = new Point((int)(picNormal.Location.X + ((chboxCapture.Location.X + ((double)chboxCapture.Size.Width / 1.7)) - picNormal.Location.X) * amount),
                                              (int)(picNormal.Location.Y + ((chboxCapture.Location.Y + ((double)chboxCapture.Size.Height / 1.7)) - picNormal.Location.Y) * amount));
        }

        //Start And Stop The Video Stream. Not Working On Webcams
        private void chboxStopStart_CheckedChanged(object sender, EventArgs e)
        {
            if (chboxStopStart.Checked)
            {
                stream.StopStream();
                chboxStopStart.Text = "Resume Streaming";
            }
            else
            {
                stream.RestartStream();
                chboxStopStart.Text = "Stop Streaming";
            }
        }

        //Stops The Stream And Loads An Image From A File To Be Processed
        private void chboxLoadImage_Click(object sender, EventArgs e)
        {
            if (chboxLoadImage.Checked)
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        picNormal.Image = Image.FromFile(openFileDialog1.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("Unable To Load Image At {0}", openFileDialog1.FileName);
                        return;
                    }

                    chboxStopStart.Checked = true;
                    chboxLoadImage.Text = "[ Unload Image ]";
                    ProcessCustomImage();
                }
                else
                {
                    chboxLoadImage.Checked = false;
                }
            }
            else
            {
                chboxStopStart.Checked = false;
                picNormal.Image.Dispose();
                picThresh.Image.Dispose();
                picDepth.Image.Dispose();
                picNormal.Image = null;
                picThresh.Image = null;
                picDepth.Image = null;
                chboxLoadImage.Text = "[ Load Image ]";
            }
        }

        //Toggle Between Totes Finding Methods
        private void chboxTotesVisionType_CheckedChanged(object sender, EventArgs e)
        {
            switch (chboxTotes.CheckState)
            {
                case CheckState.Checked:
                    chboxTotes.Text = "Find Totes [ Math ]";
                    stream.ToteDetectionMethod = ImageProcessor.ToteDetectionType.Math;
                    break;
                case CheckState.Indeterminate:
                    chboxTotes.Text = "Find Totes [ Edges ]";
                    stream.ToteDetectionMethod = ImageProcessor.ToteDetectionType.Edges;
                    break;
                case CheckState.Unchecked:
                    chboxTotes.Text = "Find Totes [ None ]";
                    stream.ToteDetectionMethod = ImageProcessor.ToteDetectionType.None;
                    break;
            }


            ProcessCustomImage();
        }

        //Toggle Between Bumpers Finding Methods
        private void chboxBins_CheckedChanged(object sender, EventArgs e)
        {
            if (stream != null)
                stream.detectBins = chboxBins.Checked;
            ProcessCustomImage();
        }

        //Lock Or Unlock The Scrollbars
        private void chboxLockBars_CheckedChanged(object sender, EventArgs e)
        {
            if (chboxLockBars.Checked)
            {
                chboxLockBars.Text = "[ Unlock Panel ]";
                chboxLockBars.ForeColor = Color.FromArgb(15, 153, 232);
            }
            else
            {
                chboxLockBars.Text = "[ Lock Panel ]";
                chboxLockBars.ForeColor = Color.Black;
            }

            barMin1.Enabled = !chboxLockBars.Checked;
            barMin2.Enabled = !chboxLockBars.Checked;
            barMin3.Enabled = !chboxLockBars.Checked;
            barMax1.Enabled = !chboxLockBars.Checked;
            barMax2.Enabled = !chboxLockBars.Checked;
            barMax3.Enabled = !chboxLockBars.Checked;
            barErode.Enabled = !chboxLockBars.Checked;
            txtFps.Enabled = !chboxLockBars.Checked;
            txtRes.Enabled = !chboxLockBars.Checked;
            txtCompression.Enabled = !chboxLockBars.Checked;
            rdioIsBgr.Enabled = !chboxLockBars.Checked;
            rdioHsv.Enabled = !chboxLockBars.Checked;
            chboxTotes.Enabled = !chboxLockBars.Checked;
            chboxBins.Enabled = !chboxLockBars.Checked;
        }

        //Capture The Current Frame And Saves It To A File
        private void chboxCapture_Click(object sender, EventArgs e)
        {
            chboxCapture.Checked = false;

            if (stream != null && picNormal.Image != null)
            {
                string name = ((DateTime.Now).ToString() + DateTime.Now.Millisecond).Replace("/", "").Replace(":", "");
                try
                {
                    picNormal.Image.Save(CapturedImagesFolderPath + "/" + name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch
                {
                    MessageBox.Show("Error saving captured image");
                }

                wooshPicture = new PictureBox();
                wooshPicture.Size = picNormal.Size;
                wooshPicture.Location = picNormal.Location;
                wooshPicture.SizeMode = picNormal.SizeMode;
                wooshPicture.Image = (Image)picNormal.Image.Clone();

                this.Controls.Add(wooshPicture);
                wooshPicture.BringToFront();

                wooshCounter.Restart();
                tmrWoosh.Start();
            }
        }

        void UpdateGuiOnNewFrame(Image original, Image proccessed, Image depth)
        {
            //Thread Safety
            if (this.picNormal.InvokeRequired)
            {
                Stream.UpdateGuiOnNewFrameFunction d = new Stream.UpdateGuiOnNewFrameFunction(UpdateGuiOnNewFrame);
                //fix the depth into black image, AS YOU WANTED TO.
                //Here are the changes I've made (some of them)
                if (depth == null)
                    depth = new Bitmap(640, 480);
                if (original == null)
                    original = new Bitmap(640, 480);
                if (proccessed == null)
                    proccessed = new Bitmap(640, 480);

                this.Invoke(d, new object[] { original, depth, proccessed });
             
            }
            else
            {
                this.picNormal.Image = original;
                    this.picThresh.Image = proccessed;
                    this.picDepth.Image = depth;
            }
        }

        //Toggle Between Bgr And Hsv
        private void rdioIsBgr_CheckedChanged(object sender, EventArgs e)
        {
            this.isBgr = rdioIsBgr.Checked;

            if (isBgr)
            {
                lblMin1.Text = "Blue";
                lblMax1.Text = "Blue";
                lblMin2.Text = "Red";
                lblMax2.Text = "Red";
                lblMin3.Text = "Green";
                lblMax3.Text = "Green";

                barMin1.Maximum = 255;
                barMax1.Maximum = 255;
            }
            else
            {
                lblMin1.Text = "Hue";
                lblMax1.Text = "Hue";
                lblMin2.Text = "Saturation";
                lblMax2.Text = "Saturation";
                lblMin3.Text = "Value";
                lblMax3.Text = "Value";

                barMin1.Maximum = 180;
                barMax1.Maximum = 180;
            }

            if (stream != null)
            {
                stream.ChangeColorFormat(isBgr);

                stream.SetThreshMin(barMin1.Value, barMin2.Value, barMin3.Value);
                stream.SetThreshMax(barMax1.Value, barMax2.Value, barMax3.Value);
                stream.SetDilate(barDilate.Value);
                stream.SetErode(barErode.Value);
            }

            ProcessCustomImage();
        }

        //Handles The Enter Press (To Change Fps And Resolution - Only When Using The Robot'S Camera)
        private void _KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((char)13)) // Enter Pressed
            {
                TextBox temp = (TextBox)sender; // In Which Textbox The Enter Was Pressed
                if (!string.IsNullOrWhiteSpace(temp.Text))
                {
                    try
                    {
                        if (temp.Name == txtFps.Name) stream.ChangeFps(Convert.ToInt32(temp.Text));
                        if (temp.Name == txtRes.Name) stream.ChangeResolution(Convert.ToInt32(temp.Text));
                        if (temp.Name == txtCompression.Name) stream.ChangeCompression(Convert.ToInt32(temp.Text));
                    }
                    catch
                    {
                        MessageBox.Show("Fps and Resolution-Width can only be numbers", "", MessageBoxButtons.OK);
                    }
                }
                temp.Text = "";
            }
        }

        //Occures When A Scrollbar Is Being Scrolled
        private void Bar_Scroll(object sender, ScrollEventArgs e)
        {
            if (sender.Equals(barMin1) || sender.Equals(barMin2) || sender.Equals(barMin3))
            {
                stream.SetThreshMin(barMin1.Value, barMin2.Value, barMin3.Value);
                min1Label.Text = barMin1.Value + "";
                min2Label.Text = barMin2.Value + "";
                min3Label.Text = barMin3.Value + "";
            }

            if (sender.Equals(barMax1) || sender.Equals(barMax2) || sender.Equals(barMax3))
            {
                stream.SetThreshMax(barMax1.Value, barMax2.Value, barMax3.Value);
                max1Label.Text = barMax1.Value + "";
                max2Label.Text = barMax2.Value + "";
                max3Label.Text = barMax3.Value + "";
            }


            if (sender.Equals(barErode))
            {
                stream.SetErode(barErode.Value);
                lblErode.Text = barErode.Value + "";
            }
            if (sender.Equals(barDilate))
            {
                stream.SetDilate(barDilate.Value);
                lblDilate.Text = barDilate.Value + "";
            }



            string data = string.Format("Format:" + (isBgr ? "BGR" : "HSV") + "," +
                    "Min1:{0},Min2:{1},Min3:{2}," +
                    "Max1:{3},Max2:{4},Max3:{5}," +
                    "Erode:{6},Dilate:{7}",
                    new object[]{
                                                barMin1.Value, barMin2.Value, barMin3.Value,
                                                barMax1.Value, barMax2.Value, barMax3.Value,
                                                barErode.Value, barDilate.Value
                                            }
                );

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(LastThreshDataFilePath))
            {
                writer.Write("{" + data + "}");
            }





            ProcessCustomImage();
        }

        //Process The Image Loaded Into Picnormal And Apply It To Picthresh
        private void ProcessCustomImage()
        {
            if (chboxLoadImage.Checked)
            {
                picThresh.Image = stream.ProcessCustomImage(picNormal.Image).colorImage;
            }
        }

        //temp function
        public void updateBinDepth(int depth)
        {
            BinDepthLabel.Text = "Bin Depth: " + depth;
        }


        private void ProgrammersWindow_Load(object sender, EventArgs e)
        {

        }

        private void picDepth_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
