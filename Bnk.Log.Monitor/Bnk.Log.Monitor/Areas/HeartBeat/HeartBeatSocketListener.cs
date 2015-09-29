using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnk.Log.Monitor.Areas.HeartBeat
{
    class HeartBeatSocketListener
    {
        Socket requestSocket;
        Thread clientListenThread;
        

        public HeartBeatSocketListener(Socket clientSocket)
        {
            requestSocket = clientSocket;
        }

        public void StartSocketListener()
        {
            if (requestSocket != null)
            {
                clientListenThread =
                  new Thread(new ThreadStart(SocketListenerThreadStart));

                clientListenThread.Start();
            }
        }

        private void SocketListenerThreadStart()
        {
            int size = 0;
            Byte[] byteBuffer = new Byte[1024];

            m_lastReceiveDateTime = DateTime.Now;
            m_currentReceiveDateTime = DateTime.Now;

            Timer t1 = new Timer(new TimerCallback(CheckClientCommInterval),
              null, 20000, 20000);

            Timer t2 = new Timer(new TimerCallback(CheckClientCommInterval),
              null, 40000, 40000);

            Timer t3 = new Timer(new TimerCallback(CheckClientCommInterval),
              null, 60000, 60000);

            while (!m_stopClient)
            {
                try
                {
                    size = requestSocket.Receive(byteBuffer);
                    m_currentReceiveDateTime = DateTime.Now;
                    ParseReceiveBuffer(byteBuffer, size);
                }
                catch (SocketException se)
                {
                    m_stopClient = true;
                    m_markedForDeletion = true;
                }
            }
            //t.Change(Timeout.Infinite, Timeout.Infinite);
            //t = null;
        }

            private void ParseReceiveBuffer(byte[] byteBuffer,int size)
            {
                string alive = System.Text.Encoding.ASCII.GetString(byteBuffer);
                System.Diagnostics.Debug.WriteLine(alive);
            }

        
        private void CheckClientCommInterval(object state)
        {
            System.Diagnostics.Debug.WriteLine("Client Communication Interval exceeded");
        }



        public bool m_markedForDeletion { get; set; }

        public bool m_stopClient { get; set; }

        public DateTime m_lastReceiveDateTime { get; set; }

        public DateTime m_currentReceiveDateTime { get; set; }
    }
}
