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
        int _port = 3499;
        string address = "192.168.0.23";
        #endregion

        #region Constructor
        public HeartBeatReceiver()
        {
            try
            {
                int intAddress = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes(), 0);
                IPAddress myip = new IPAddress(intAddress);
                _hbreceive = new TcpListener(myip, _port);
            }
            catch (Exception ex)
            {
                _hbreceive = null;
                
            }
        }
        #endregion

        #region Start the Heart Beat Service on separate thread
        public void StartServer()
        {
            if (_hbreceive != null)
            {
                // Create a ArrayList for storing SocketListeners before
                // starting the server.
                m_socketListenersList = new ArrayList();

                // Start the Server and start the thread to listen client 
                // requests.
                _hbreceive.Start();
                Thread _receiveHbThread = new Thread(new ThreadStart(ServerThreadStart));
                _receiveHbThread.Start();
            }
        }
        #endregion

        #region Server Thread Start
        private void ServerThreadStart()
        {
            Socket ServerSocket = null;
            HeartBeatSocketListener socketListener = null;
            try
            {
                while (!_hbStop)
                {
                    ServerSocket = _hbreceive.AcceptSocket();

                    socketListener = new HeartBeatSocketListener(ServerSocket);

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
