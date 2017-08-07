using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace PortScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            string _adress = args[0], _log = " ";
            //int _progress = 0;
            int _startPort = Convert.ToInt32(args[1]), _endPort = Convert.ToInt32(args[2]);

            Console.Clear();
            Console.WriteLine("Scanning...");

            for (int _currentPort = _startPort; _currentPort < _endPort; _currentPort++)
            {
                //Console.WriteLine("Progress {0}%", ((_endPort - _startPort)/100) * _currentPort);

                TcpClient _tcpScan = new TcpClient();
                //_tcpScan.SendTimeout = 5;

                try
                {
                    _tcpScan.Connect(_adress, _currentPort);
                    _log += String.Concat(_adress, ":", _currentPort, "\t open\r\n");
                    Console.WriteLine(String.Concat(_adress, ":", _currentPort, "\t open\r"));
                    //Console.WriteLine(_tcpScan.ToString);
                    _tcpScan.Close();
                }
                catch
                {
                    _log += String.Concat(_adress, ":", _currentPort, "\t closed\r\n");
                    Console.WriteLine(String.Concat(_adress, ":", _currentPort, "\t closed\r"));
                }
            }
            //Console.WriteLine(_log);
        }
    }
}
