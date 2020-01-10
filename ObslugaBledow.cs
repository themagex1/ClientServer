using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{
    public class obslugaBledow
    {
        public static void ObslugaBledow(UdpClient server, Exception e, ref int ID, string OP, IPEndPoint ip, Wpis Mes,ref List<Wpis> Historia,int n )
        {
            Mes.wynik="ERROR";
            Historia.Add(Mes); //dodawanie bledow do historii
            string temp = string.Empty;
            byte[] data;
            DateTime ZC = DateTime.Now;
            temp = "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 1%OP: " + OP+"%";
            data = Encoding.ASCII.GetBytes(temp);
            server.Send(data, data.Length, ip); //wyslanie operacji w ktorej wystapil blad
            temp = string.Empty;
            temp = "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 0%ST: ERROR_"+ n + "%";
            data = Encoding.ASCII.GetBytes(temp);
            server.Send(data, data.Length, ip); //wyslanie kodu bledu 
            temp = string.Empty;
        }
         public static void ObslugaBledow(UdpClient server, Exception e, ref int ID, string OP, IPEndPoint ip,int n )
        {
          // oddzielna wariacja dla bledow przy wywolaniu bledow HISID i HISIDO
            string temp = string.Empty;
            byte[] data;
            DateTime ZC = DateTime.Now;
            temp = "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 1%OP: " + OP+"%";
            data = Encoding.ASCII.GetBytes(temp);
            server.Send(data, data.Length, ip);
            temp = string.Empty;
            temp = "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 0%ST: ERROR_"+ n + "%";
            data = Encoding.ASCII.GetBytes(temp);
            server.Send(data, data.Length, ip);
            temp = string.Empty;
        }
    }
}
