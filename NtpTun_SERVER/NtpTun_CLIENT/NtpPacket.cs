// NTP packet structure implementation
// Made by Hackcat specially for https://xakep.ru

using System.IO;
using System.Collections.Generic;
using System;

namespace NtpTun_PoC
{
    public struct NtpPacket
    {
        public byte First8bits; //First 2 - const, 6 last - PKT_ID
        public const byte Stratum = 0x3;
        public byte Poll; //8 bit request data
        public byte Precision; //8 bit response data
        public uint RootDelay; //32 bit response data
        public uint RefID; //32 bit response data
        public ulong Reference; //64 bit response data
        public ulong Originate; //64 bit request data
        public ulong Receive; //64 bit response data
        public ulong Transmit; //64 bit request data
        public NtpPacket Parse(byte[] data)
        {
            return new NtpPacket();
        }
        public InPacket GetBuiltInPacket(byte[] packet)
        {
            InPacket _packet = new InPacket();
            NtpPacket ntpPacket = this.Parse(packet);
            _packet.PacketID = (byte)((ntpPacket.First8bits << 2) >> 2);
            _packet.Data = new byte[17];
            _packet.Data[0] = ntpPacket.Poll;
            BitConverter.GetBytes(ntpPacket.Originate).CopyTo(_packet.Data, 1);
            BitConverter.GetBytes(ntpPacket.Transmit).CopyTo(_packet.Data, 9);
            return _packet;
        }
        public NtpPacket EmbedDataToPacketC(byte[] data)
        {
            NtpPacket result = new NtpPacket();
            result.First8bits = 0x1B;
            result.Poll = data[0];
            result.Originate = BitConverter.ToUInt64(data, 1);
            result.Transmit = BitConverter.ToUInt64(data, 9);
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