// NTP packet structure implementation
// Made by Hackcat specially for https://xakep.ru

using System;
using System.IO;
using System.Collections.Generic;

namespace NtpTun_SERVER
{
    public struct NtpPacket
    {
        public byte First8bits;         //First 2 - const
        public const byte Stratum = 0x3;
        public byte Poll;               //8 bit request/response data
        public byte Precision;          //8 bit response data
        public uint RootDelay;          //32 bit response data
        public uint RootDisp;           //32 bit response data
        public uint RefID;              //32 bit response data
        public ulong Reference;         //64 bit response data
        public ulong Originate;         //64 bit request data
        public ulong Receive;           //64 bit response data
        public ulong Transmit;          //64 bit request data
        public NtpPacket Parse(byte[] data)
        {
            var r = new NtpPacket();
            //NTP packet is 48 bytes long
            r.First8bits = data[0];
            r.Poll = data[2];
            r.Precision = data[3];
            r.RootDelay = BitConverter.ToUInt32(data, 4);
            r.RootDisp = BitConverter.ToUInt32(data, 8);
            r.RefID = BitConverter.ToUInt32(data, 12);
            r.Reference = BitConverter.ToUInt64(data, 16);
            r.Originate = BitConverter.ToUInt64(data, 24);
            r.Receive = BitConverter.ToUInt64(data, 32);
            r.Transmit = BitConverter.ToUInt64(data, 40);
            return r;
        }
        public InPacket GetBuiltInPacketC(byte[] packet)
        {
            InPacket _packet = new InPacket();
            NtpPacket ntpPacket = this.Parse(packet);
            _packet.PacketID = 0;
            _packet.Data = new byte[17];
            _packet.Data[0] = ntpPacket.Poll;
            BitConverter.GetBytes(ntpPacket.Originate).CopyTo(_packet.Data, 1);
            BitConverter.GetBytes(ntpPacket.Transmit).CopyTo(_packet.Data, 9);
            return _packet;
        }
        public NtpPacket EmbedDataToPacketS(byte[] data)
        {
            var nd = new byte[30]; //Max 30 bytes of data in response
            data.CopyTo(nd, 0);
            data = nd;
            NtpPacket result = new NtpPacket();
            result.First8bits = 0x1B;
            result.Poll = data[0];
            result.Precision = data[1];
            result.RootDelay = BitConverter.ToUInt32(data, 2);
            result.RootDisp = BitConverter.ToUInt32(data, 6);
            result.RefID = BitConverter.ToUInt32(data, 10);
            result.Reference = BitConverter.ToUInt64(data, 14);
            result.Receive = BitConverter.ToUInt64(data, 22);
            return result;
        }
        public byte[] BuildPacket()
        {
            byte[] arr = new byte[48];
            arr[0] = First8bits;
            arr[1] = Stratum;
            arr[2] = Poll;//Convert.ToByte(new Random().Next(4,15)); //Fill Poll field using RFC
            arr[3] = Precision;
            BitConverter.GetBytes(RootDelay).CopyTo(arr, 4);
            //First 8 bytes filled
            BitConverter.GetBytes(RefID).CopyTo(arr, 8);
            //12 bytes
            BitConverter.GetBytes(Reference).CopyTo(arr, 12);
            BitConverter.GetBytes(Originate).CopyTo(arr, 20);
            BitConverter.GetBytes(Receive).CopyTo(arr, 28);
            BitConverter.GetBytes(Transmit).CopyTo(arr, 36);
            return arr;
        }
    }
    public struct InPacket
    {
        public byte PacketID;
        public byte[] Data;
    }
}
