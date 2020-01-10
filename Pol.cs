using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Client
{
    public class Pol
    {
        public static bool polaczenieClient(ref UdpClient klient)
        {
            byte[] data;
            int port;
            DateTime ZC;
            string dec;
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 11000);
            Console.WriteLine("Podaj adres IP odbiorcy: ");
            string IP = Console.ReadLine();
            Console.WriteLine("Podaj port odbiorcy: ");
            try { port = Convert.ToInt32(Console.ReadLine()); }
            catch (FormatException e) { Console.WriteLine(e.Message); port = 0; }
            string[] spr = IP.Split('.');
            while (true)
            {
                if (port < 65535 && port > 1023 && spr.Length == 4 && IP!="0.0.0.0") //Sprawdzenie poprawności formy podanych parametrów
                {
                    string wiadomosc = string.Empty;
                    ZC = DateTime.Now;
                    wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: BRAK%NS: 0%OP: POLACZ%";  // komunikat poczatkowy przed nadaniem ID sesji 
                    klient = new UdpClient(IP, port);
                    data = Encoding.ASCII.GetBytes(wiadomosc);
                    klient.Send(data,data.Length);
                    return true;
                }
                else
                {
                    Console.WriteLine("Nie udalo sie nawiazac polaczenia. Czy chcesz spróbować ponownie?(0/1)");
                    dec = Console.ReadLine();
                    if (dec == "1")  return polaczenieClient(ref klient); 
                    else if (dec == "0") return false;
                    else Console.WriteLine("Niepoprawna odpowiedz");
                }
            }

        }
    }
}
