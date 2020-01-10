using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{
    public class Polaczenie
    {
        public static void polaczenie(UdpClient server, IPEndPoint ip, int ID)
        {

            int n = 0, IDO = 0;
            string dane;
            string[][] AllSplits = new string[4][];
            byte[] data;
            string wiadomosc = string.Empty;
            List<Wpis> Historia = new List<Wpis>();

            while (true) //oczekiwanie na nawiazanie polaczenia
            {
                data = server.Receive(ref ip);
                dane = Encoding.ASCII.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' });
                AllSplits[n] = split;
                Console.WriteLine(dane);
                n++;
                if (Convert.ToInt32(split[5]) <= 0) { n = 0; break; }
            }
            DateTime ZC = DateTime.Now;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 1%OP: NadanieID%";
            data = Encoding.ASCII.GetBytes(wiadomosc); //Nadanie ID
            server.Send(data, data.Length, ip);
            ZC = DateTime.Now;
            wiadomosc = string.Empty;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 0%ST: BRAK%";
            data = Encoding.ASCII.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;
            while (true)         //Oczekiwanie na potwierdzenie odbioru ID
            {


                data = server.Receive(ref ip);

                dane = Encoding.ASCII.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' }); //podzielenie komunikatu na czesci i wrzucenie do tablicy
                AllSplits[n] = split;
                Console.WriteLine(dane);
                n++;
                if (Convert.ToInt32(split[5]) <= 0)
                {
                    n = 0;
                    if (split != null) 
                    {
                        if (AllSplits[1][7] == " OK")
                            break;
                    }
                }

            }
            Obliczenia.obliczenia(server, ip, data, ID, ref IDO, Historia);

            do
            {
                while (true) //oczekiwanie na informacje o dalszych obliczeniach lub zakończeniu pracy
                {
                    data = server.Receive(ref ip);
                    dane = Encoding.ASCII.GetString(data, 0, data.Length);
                    string[] split = dane.Split(new char[] { ':', '%' });
                    AllSplits[n] = split;
                    Console.WriteLine(dane);
                    n++;
                    if (Convert.ToInt32(split[5]) <= 0) { n = 0; break; }
                }

                if (AllSplits[0][6] == "OP") 
                {
                     if (AllSplits[0][7].ToLower() == " koniec") //wywolanie oczekiwania na kolejne polaczenie
                    {
                        ID++;
                        polaczenie(server, ip, ID);
                    }
                    else if (AllSplits[0][7].ToLower() == " ponow") //wywolanie oczekiwania na kolejne operacje
                    {
                        Obliczenia.obliczenia(server, ip, data, ID, ref IDO, Historia);
                    }
                }

            } while (true);
        }
    }
}
