using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Client
{
    class Program
    {

        static void operacje(UdpClient client,IPEndPoint ip, byte[] data, int ID)
        {
            string OP, ST=string.Empty,ER=string.Empty;
            int NS = 0, count = 0;
            double liczba1 = 0;
            double wynik = 0;
            double liczba2 = 0;
            string operacja=string.Empty, wiadomosc = string.Empty;
            string[][] AllSplits = new string[4][];
            Console.WriteLine("Podaj operację: ");
            operacja = Console.ReadLine();
            DateTime ZC;
            try
            {
                if (operacja.ToLower() == "silnia")
                {
                    Console.WriteLine("Podaj liczbę 1: ");
                    liczba1 = Convert.ToDouble(Console.ReadLine());
                    NS = 2;
                }

                else if (operacja.ToLower() == "mnozenie" || operacja.ToLower() == "dzielenie" || operacja.ToLower() == "odejmowanie" || operacja.ToLower() == "dodawanie")
                {
                    Console.WriteLine("Podaj liczbę 1: ");
                    liczba1 = Convert.ToDouble(Console.ReadLine());

                    Console.WriteLine("Podaj liczbę 2: ");
                    liczba2 = Convert.ToDouble(Console.ReadLine());

                    NS = 3;
                }

                else throw new Exception("Nieznana operacja!!!");
            }

            catch (Exception ex) { Console.WriteLine($"Wystąpił błąd: {ex.Message}, typ: {ex.GetType()}"); operacje(client, ip, data, ID);return; }
            ZC = DateTime.Now;
            wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:" + NS + "%OP:" + operacja.ToLower() + "%";
            data = Encoding.Unicode.GetBytes(wiadomosc);
            client.Send(data, data.Length);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:" + NS + "%ST:brak%";
            data = Encoding.Unicode.GetBytes(wiadomosc);
            client.Send(data, data.Length);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:" + NS + "%L1:" + liczba1 + "%";
            data = Encoding.Unicode.GetBytes(wiadomosc);
            client.Send(data, data.Length);
            wiadomosc = string.Empty;

            NS--;

            if (NS == 0)
            {
                ZC = DateTime.Now;
                wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:" + NS + "%L2:" + liczba2 + "%";
                data = Encoding.Unicode.GetBytes(wiadomosc);
                client.Send(data, data.Length);
                wiadomosc = string.Empty;
            }




            while (true)
            {
                data = client.Receive(ref ip);

                string dane = Encoding.Unicode.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' });
                AllSplits[count] = split;
                count++;
                Console.WriteLine(dane);
                if (Convert.ToInt32(split[5]) == 0) { count = 0; break; }
            }
            foreach (string[] i in AllSplits)
            {
                if (i == null) break;
                if (i[6] == "OP") OP = i[7];
                else if (i[6] == "ST") ST = i[7];
                else if (i[6] == "LW") wynik = Convert.ToDouble(i[7]);
                else if (i[6] == "ER") ER = i[7];

            }
            if (ST.ToLower() == "error")
                Console.WriteLine("Wystapil blad {0}", ER);
            else
            Console.WriteLine("Wynik to: {0} ", wynik);
             
        }

        static UdpClient polaczenieClient()
        {
            
            int port;
            string dec;
            UdpClient klient = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 11000);
            Console.WriteLine("Podaj adres IP odbiorcy: ");
            string IP = Console.ReadLine();
            Console.WriteLine("Podaj port odbiorcy: ");
            try {  port = Convert.ToInt32(Console.ReadLine()); }
            catch (FormatException e) { Console.WriteLine(e.Message); port=0; }
            
            while (true)
            {
                if (port<65535 && port>1023 && IP=="127.0.0.1") //zmiana ip
                {
                    klient = new UdpClient(IP, port);
                    klient.Send(Encoding.Unicode.GetBytes("1"), 1);
                    return klient;
                }
                else
                {
                    Console.WriteLine("Nie udalo sie nawiazac polaczenia. Czy chcesz spróbować ponownie?(0/1)");
                    dec = Console.ReadLine();
                    if (dec == "1") return polaczenieClient();
                    else if (dec == "0") return klient = new UdpClient("110.0.0.0", 2137);
                    else Console.WriteLine("Niepoprawna odpowiedz");
                }
            }

        }
        static void Main(string[] args)                                                                         //TU JEST MAIN TU JEST MAIN TU JEST MAIN TU JEST MAIN
        {
            
            UdpClient client = polaczenieClient();
            if (client == new UdpClient("110.0.0.0", 2137)) return;
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 11000);

            //client.Connect("127.0.0.1", 11000);
            int n = 0;
            int ID = 0;
            string odp;
            byte[] data;
            string[][] AllSplits = new string[4][];
            DateTime ZC;
            while (true)
            {
                data = client.Receive(ref ip);

                string dane = Encoding.Unicode.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' });
                AllSplits[n] = split;
                Console.WriteLine(dane);
                n++;
                if (Convert.ToInt32(split[5]) <= 0)
                {
                    n = 0;

                    try
                    {
                        ID = Convert.ToInt32(split[3]);
                        ZC = DateTime.Now;
                        string temp = "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:1%OP:NadanieID%";
                        data = Encoding.Unicode.GetBytes(temp);
                        client.Send(data, data.Length);
                        temp = string.Empty;
                        ZC = DateTime.Now;
                        temp = "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:0%ST:OK%";
                        data = Encoding.Unicode.GetBytes(temp);
                        client.Send(data, data.Length);
                    }
                    catch (Exception) { }
                    break;
                }
            }
            operacje(client, ip, data, ID);

            do
            {
                Console.WriteLine("Czy chcesz wykonać ponowne obliczenia ? (0/1)");
                odp = Console.ReadLine();
                if (odp == "1") {
                    ZC = DateTime.Now;
                    string wiadomosc = string.Empty; 
                    wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:1%OP:PONOW%";
                    data = Encoding.Unicode.GetBytes(wiadomosc);
                    client.Send(data, data.Length);

                    wiadomosc = string.Empty;
                    ZC = DateTime.Now;
                    wiadomosc = "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:0%ST:BRAK%";
                    data = Encoding.Unicode.GetBytes(wiadomosc);
                    client.Send(data, data.Length);
                    wiadomosc = string.Empty;

                    operacje(client, ip, data, ID);
                }
                else if (odp == "0") {

                    string wiadomosc = string.Empty;
                    ZC = DateTime.Now;
                    wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:1%OP:KONIEC%";
                    data = Encoding.Unicode.GetBytes(wiadomosc);
                    client.Send(data, data.Length);

                    wiadomosc = string.Empty;
                    ZC = DateTime.Now;
                    wiadomosc = "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:0%OST:BRAK%";
                    data = Encoding.Unicode.GetBytes(wiadomosc);
                    client.Send(data, data.Length);
                    wiadomosc = string.Empty;
                    client.Close();
                }
                else Console.WriteLine("Niepoprawna odpowiedź");
            } while (odp != "0"); 
              
         }

            /*

            UdpClient udpClient = new UdpClient();

            //udpClient.Connect("127.0.0.1", 11000);

            byte[] temp = new byte[256];

            udpClient.Send(temp, temp.Length, "127.0.0.1", 11000);

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 11000);

            Byte[] receiveBytes = udpClient.Receive(ref iPEndPoint);

            string data = Encoding.ASCII.GetString(receiveBytes);

            foreach(var s in receiveBytes)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine(data);

            udpClient.Close();*/
        }
    }
