using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnk.Log.Monitor.Areas.HeartBeat
{
    /// <summary>
    /// HeartBeat Receiver which receives the message from the Payment Reader
    /// </summary>
    class HeartBeatReceiver
    {
        #region Constant
        public const int CRITICAL_TIMEOUT = 60;
        public const int MAJOR_TIMEOUT = 40;
        public const int WARNING_TIMEOUT = 20;
        #endregion

        #region Private Global Variables
        
        public bool _hbStop { get; set; }
        
        TcpListener _hbreceive;
        ArrayList m_socketListenersList;
        int _port = 31001;
        string address = "127.0.0.1";
        #endregion

        #region Constructor
        public HeartBeatReceiver()
        {
            try
            {
                int intAddress = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes(), 0);
                IPAddress myip = Dns.GetHostEntry("localhost").AddressList[0];
                System.Diagnostics.EventLog.WriteEntry("REACH TILL CONSTRUCTOR", "YE");
                _hbreceive = new TcpListener(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 31001));
                System.Diagnostics.EventLog.WriteEntry("REACH TILL CONSTRUCTOR 2", "YE");

            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("Please", ex.InnerException.ToString());
                _hbreceive = null;
                
            }
        }
        #endregion

        #region Start the Heart Beat Service on separate thread
        public void StartServer()
        {
            System.Diagnostics.EventLog.WriteEntry("Start server", "YE");

            if (_hbreceive != null)
            {
                // Create a ArrayList for storing SocketListeners before
                // starting the server.
                m_socketListenersList = new ArrayList();

                // Start the Server and start the thread to listen client 
                // requests.
                System.Diagnostics.EventLog.WriteEntry("START", "1");

                _hbreceive.Start();
                Thread _receiveHbThread = new Thread(new ThreadStart(ServerThreadStart));
                System.Diagnostics.EventLog.WriteEntry("START THREAD", "2");

                _receiveHbThread.Start();
            }
        }
        #endregion

        #region Server Thread Start
        private void ServerThreadStart()
        {
            System.Diagnostics.EventLog.WriteEntry("IN THREAD", "1");

            Socket ServerSocket = null;
            HeartBeatSocketListener socketListener = null;
            try
            {
                while (!_hbStop)
                {
                    System.Diagnostics.EventLog.WriteEntry("IN THREAD", "2");

                    ServerSocket = _hbreceive.AcceptSocket();

                    socketListener = new HeartBeatSocketListener(ServerSocket);
                    System.Diagnostics.EventLog.WriteEntry("IN THREAD", "3");

                    lock (m_socketListenersList)
                    {
                        m_socketListenersList.Add(socketListener);
                    }

                    socketListener.StartSocketListener();
                }
            }
            catch (Exception ex)
            {
                _hbStop = true;
            }
        }
        #endregion
    }
}
