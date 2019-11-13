//UDP socket implementation
//Used DarkGuy's implementation from https://gist.github.com/darkguy2008/413a6fea3a5b4e67e5e0d96f750088a9
//For https://xakep.ru

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NtpTun_SERVER;

namespace NtpTun_SERVER
{
    public class UDPSocket
    {
        public Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int buffsize = 48;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;
        public delegate void r_callback(byte[] ntp);
        private r_callback _Callback;
        public delegate void s_reply(byte packet_id);
        private s_reply s_Reply;
        public class State
        {
            public byte[] buffer = new byte[buffsize];
        }

        public void Server(string address, int port, r_callback callback, s_reply rep_cb)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _Callback = callback;
            s_Reply = rep_cb;
            Receive();
        }

        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
            }, state);
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, buffsize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                _socket.BeginReceiveFrom(so.buffer, 0, buffsize, SocketFlags.None, ref epFrom, recv, so);
                Thread.Sleep(200);
                _Callback(so.buffer);
                //s_Reply(so.buffer[5]);
                //Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
            }, state);
        }
    }

    public class BidirectionalUDPSocket
    {
        private UDPSocket udpsocket = new UDPSocket();
        public BidirectionalUDPSocket(IPAddress address, int port)
        {
            udpsocket.Server(address.ToString(), port, r_c, rep_cp);
        }

        private void rep_cp(byte packet_id)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] data)
        {

        }
        public byte[] Read()
        {
            return null;
        }
        private void rep_cp(byte packet_id, UDPSocket ep)
        {
            throw new NotImplementedException();
        }

        private void r_c(byte[] ntp)
        {
            throw new NotImplementedException();
        }
    }
}
