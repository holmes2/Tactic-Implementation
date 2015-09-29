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

            Timer t = new Timer(new TimerCallback(CheckClientCommInterval),
              null, 15000, 15000);

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
            t.Change(Timeout.Infinite, Timeout.Infinite);
            t = null;
        }

            private void ParseReceiveBuffer(byte[] byteBuffer,int size)
            {
 	                System.Diagnostics.Debug.WriteLine("Parse the Buffer");
            }

        
        private void CheckClientCommInterval(object state)
        {
            Console.WriteLine("Take care Client die");
        }



        public bool m_markedForDeletion { get; set; }

        public bool m_stopClient { get; set; }

        public DateTime m_lastReceiveDateTime { get; set; }

        public DateTime m_currentReceiveDateTime { get; set; }
    }
}
