using System;
using System.Net;
using System.Net.Sockets;

namespace hiro
{
    public class HiroSocket
    {
        public Socket ClientSocket { get; set; }
        public string IP;
        public HiroSocket(Socket clientSocket)
        {
            this.ClientSocket = clientSocket;
            this.IP = GetIPStr();
        }
        public string GetIPStr()
        {
            EndPoint? ep = ClientSocket.RemoteEndPoint;
            string resStr = ep != null ? ((IPEndPoint)ep).Address.ToString() : "";
            return resStr;
        }
    }
    public class SocketConnection : IDisposable
    {
        public Byte[] msgBuffer = new byte[1024];
        private readonly Socket? _clientSocket = null;
        public Socket? ClientSocket
        {
            get { return this._clientSocket; }
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
                ClientSocket.BeginConnect(ip, port, ConnectCallback, this.ClientSocket);
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                if (ar.AsyncState != null)
                {
                    Socket handler = (Socket)ar.AsyncState;
                    handler.EndConnect(ar);
                }
            }
            catch
            {

            }
        }
        #endregion
        #region 发送数据
        public void Send(string data)
        {
            Send(System.Text.Encoding.UTF8.GetBytes(data));
        }
        private void Send(byte[] byteData)
        {
            try
            {
                if (ClientSocket != null)
                    ClientSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), ClientSocket);
            }
            catch
            {

            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                if (ar.AsyncState != null)
                {
                    Socket handler = (Socket)ar.AsyncState;
                    handler.EndSend(ar);
                    OnDataSendCompleted(new());
                }
            }
            catch
            {

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
                    int REnd = ClientSocket.EndReceive(ar);
                    if (REnd > 0)
                    {
                        byte[] data = new byte[REnd];
                        Array.Copy(msgBuffer, 0, data, 0, REnd);
                        OnDataRecevieCompleted(new());
                        //ClientSocket.BeginReceive(msgBuffer, 0, msgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
                    }
                    else
                        Dispose();
                }
                else
                    Dispose();
            }
            catch
            {

            }
        }
        protected virtual void OnDataRecevieCompleted(EventArgs e)
        {
            DataRecevieCompleted?.Invoke(this, e);
        }

        public event EventHandler<EventArgs>? DataRecevieCompleted;
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

            }
        }
        #endregion
    }
}