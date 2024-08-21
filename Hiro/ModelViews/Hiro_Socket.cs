using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Hiro.ModelViews
{
    public class Hiro_Socket
    {
        public Socket ClientSocket { get; set; }
        public string IP;
        public Hiro_Socket(Socket clientSocket)
        {
            ClientSocket = clientSocket;
            IP = GetIPStr();
        }
        public string GetIPStr()
        {
            var ep = ClientSocket.RemoteEndPoint;
            var resStr = ep != null ? ((IPEndPoint)ep).Address.ToString() : "";
            return resStr;
        }
    }
    public class SocketConnection : IDisposable
    {
        public byte[] msgBuffer = new byte[1024];
        public string receivedMsg = string.Empty;
        private readonly Socket? _clientSocket = null;
        public Socket? ClientSocket
        {
            get { return _clientSocket; }
        }
        #region 构造
        public SocketConnection(Socket? sock)
        {
            _clientSocket = sock;
        }
        #endregion
        #region 连接
        public void Connect(IPAddress ip, int port)
        {
            if (ClientSocket != null)
                ClientSocket.BeginConnect(ip, port, ConnectCallback, ClientSocket);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                if (ar.AsyncState == null)
                    return;
                Socket handler = (Socket)ar.AsyncState;
                handler.EndConnect(ar);
            }
            catch
            {
                // ignored
            }
        }
        #endregion
        #region 发送数据
        public void Send(string data)
        {
            Send(Encoding.UTF8.GetBytes(data));
        }
        private void Send(byte[] byteData)
        {
            try
            {
                ClientSocket?.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), ClientSocket);
            }
            catch
            {
                // ignored
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                if (ar.AsyncState == null)
                    return;
                var handler = (Socket)ar.AsyncState;
                handler.EndSend(ar);
                OnDataSendCompleted(EventArgs.Empty);
            }
            catch
            {
                // ignored
            }
        }

        protected virtual void OnDataSendCompleted(EventArgs e)
        {
            DataSendCompleted?.Invoke(this, e);
        }
        #endregion
        #region 接收数据
        public void ReceiveData()
        {
            if (ClientSocket != null)
                ClientSocket.BeginReceive(msgBuffer, 0, msgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (ClientSocket != null)
                {
                    var REnd = ClientSocket.EndReceive(ar);
                    if (REnd > 0)
                    {
                        receivedMsg = Encoding.ASCII.GetString(msgBuffer);
                        OnDataReceiveCompleted(new());
                    }
                    else
                    {
                        Dispose();
                    }

                }
                else
                    Dispose();
            }
            catch
            {
                // ignored
            }
        }
        protected virtual void OnDataReceiveCompleted(EventArgs e)
        {
            DataReceiveCompleted?.Invoke(this, e);
            receivedMsg = string.Empty;
        }

        public event EventHandler<EventArgs>? DataReceiveCompleted;
        public event EventHandler<EventArgs>? DataSendCompleted;
        public void Dispose()
        {
            try
            {
                if (ClientSocket != null)
                {
                    ClientSocket.Shutdown(SocketShutdown.Both);
                    ClientSocket.Close();
                }
            }
            catch
            {
                // ignored
            }
        }
        #endregion
    }
}