// NTP tunneling technique PoC implementation
// Made by Hackcat specially for https://xakep.ru

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NtpTun_SERVER
{
    class Program
    {

        static string ReadLine(String def, String prompt = "")
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            s = String.IsNullOrWhiteSpace(s) ? def : s;
            return s;
        }
        static void Main(string[] args)
        {
            Console.WriteLine($"         NtpTun_PoC  (Specially for https://xakep.ru)\r\n");
            var ip = ReadLine("0.0.0.0", $"Enter interface IP [0.0.0.0]: ");
            int port = int.Parse(ReadLine("123", "Enter port to listen on [123]: "));
            Console.WriteLine($"[>] Starting UDP listener on udp://{ip}:{port}");
            Console.WriteLine($"Please press ALLOW on firewall prompt");
            UDPSocket usock = new UDPSocket();
            try
            {
                usock.Server(ip, port, t_func_c0, t_func_c1);
            }
            catch { Console.WriteLine($"[!] Cannot start an UDP server at {ip}:{port}. Press any key to exit..."); Console.ReadKey(true); return; }
            Console.WriteLine($"[+] Successfully placed NTP server on udp://{ip}:{port}");
            Console.WriteLine($"[>] Waiting for incoming connections...");
            Console.CancelKeyPress += OnCancelPressed;
            while (!is_cancel)
            {
                //Main loop
            }
            Console.WriteLine($"[>] Exit signal caught");
            Console.WriteLine($"[>] Stopping NTP server...");
            try
            {
                usock._socket.Close();
                Console.WriteLine($"[+] Success");
            }
            catch
            {
                Console.WriteLine($"[!] Failed.");
            }
            Console.WriteLine($"[>] NtpTun PoC has beed stopped. Press any key to close...");
            Console.ReadKey(true);
        }

        private static void t_func_c1(byte packet_id)
        {
            throw new NotImplementedException();
        }

        static bool is_cancel = false;
        private static void OnCancelPressed(object sender, ConsoleCancelEventArgs e)
        {
            is_cancel = true;
            e.Cancel = true;
        }

        static void t_func_c1(byte packet_id, UDPSocket ep)
        {
            //ep.Send(null);
            Console.WriteLine(">  Packet caught. ID: " + packet_id);
        }

        static void t_func_c0(byte[] data_ntp)
        {
            String _data = "";
            for (int i = 0; i < data_ntp.Length; i++)
            {
                if (i == 5 || i == 0 || i == 1) continue;
                if (data_ntp[i] == 0) continue;
                _data += Encoding.ASCII.GetString(new byte[]{ data_ntp[i] });
            }
            Console.WriteLine(">  Packet caught. ID: " + data_ntp[5] + $"\r\nData: {_data}");
        }

    }
}
