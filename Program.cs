using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{    
    
    public class Program
    {
      
        static void Main(string[] args)  //TU JEST MAIN 
        {
            int ID = 0; //poczatkowe ID
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 11000);

            UdpClient server = new UdpClient(11000);

           
            Polaczenie.polaczenie(server, ip, ID); 

            server.Close(); //zamkniecie serwera

           
        }
    }
}


