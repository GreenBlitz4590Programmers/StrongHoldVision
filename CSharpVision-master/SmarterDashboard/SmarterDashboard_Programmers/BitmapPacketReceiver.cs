using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Drawing.Imaging;

namespace SmarterDashboard_Programmers
{
    class BitmapPacketReceiver
    {
        private int port;
        private String address;
        private UdpClient outClient;
        private UdpClient inClient;
        private bool firstFrame = true;

        public BitmapPacketReceiver(int port, String address)
        {
            this.address = address;
            this.port = port;
        }

        public void startClient()
        {
            inClient = new UdpClient(port);
            outClient = new UdpClient();
            firstFrame = true;
        }

        public Bitmap receiveImage()
        {
            Console.WriteLine("New Frame!");
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
            if (!firstFrame)
            {
                outClient.Send(new byte[] { 0 }, 1, address, port);
            }
            Byte[] bytes = inClient.Receive(ref ip);
            int height = BitConverter.ToInt32(bytes, 0);
            int width = BitConverter.ToInt32(bytes, 4);
            Bitmap bmp = new Bitmap(width, height);
            byte[][] bytesRec = new byte[height / 16][];
            Console.WriteLine("Got 1st packet");
            for (int i = 0; i < height / 16; i++)
            {
                IPEndPoint ip1 = new IPEndPoint(IPAddress.Any, 0);
                bytesRec[i] = inClient.Receive(ref ip1);
            }
            Console.WriteLine("Received Image");
            for (int y1 = 0; y1 < height / 16; y1++) {
                for (int y2 = 0; y2 < 16; y2++) {
                    int y = 16 * y1 + y2;
                    for (int x = 0; x < width; x++) {
                        int pos = y2 * width + x;
                        bmp.SetPixel(x, y, Color.FromArgb(bytesRec[y1][3 * pos + 2], bytesRec[y1][3 * pos + 1], bytesRec[y1][3 * pos]));
                    }
                }
            }
            firstFrame = false;
            return bmp;

        }

        public void closeClient()
        {
            inClient.Close();
            outClient.Close();
        }
    }
}
