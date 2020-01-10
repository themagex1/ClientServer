using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Client
{
    public class Op
    {
        public static void operacje(UdpClient client, IPEndPoint ip, byte[] data, int ID)
        {

            string OP = string.Empty, ST = string.Empty, ER = string.Empty, OH = string.Empty;
            int NS = 0, count = 0, n = 0, bigbrain = 0;
            double liczba1 = 0;
            double wynik = 0;
            double liczba2 = 0;
            string operacja = string.Empty, wiadomosc = string.Empty;
            string[][] AllSplits = new string[100][];
            Console.WriteLine("Podaj operację: "); //zapytanie o operację
            operacja = Console.ReadLine();
            DateTime ZC;
            List<Wpis> Historia = new List<Wpis>();
            Wpis noname = new Wpis();
            try //zapytanie o liczby w zależności od wybranej operacji
            {
                //w przypadku silni i hisid potrzebujemy tylko 1 liczby
                if (operacja.ToLower() == "silnia" || operacja.ToLower() == "hisid")
                {
                    Console.WriteLine("Podaj liczbę 1: ");
                    liczba1 = Convert.ToDouble(Console.ReadLine());
                    NS = 2;
                }
                //podawanie liczb
                else if (operacja.ToLower() == "mnozenie" || operacja.ToLower() == "dzielenie" || operacja.ToLower() == "odejmowanie" || operacja.ToLower() == "dodawanie" || operacja.ToLower() == "hisido")
                {
                    Console.WriteLine("Podaj liczbę 1: ");
                    liczba1 = Convert.ToDouble(Console.ReadLine());

                    Console.WriteLine("Podaj liczbę 2: ");
                    liczba2 = Convert.ToDouble(Console.ReadLine());

                    NS = 3;
                }

                else throw new Exception("Nieznana operacja!!!");
            }
            //wysyłanie komunikatów
            catch (Exception ex) { Console.WriteLine($"Wystąpił błąd: {ex.Message}, typ: {ex.GetType()}"); operacje(client, ip, data, ID); return; }
            ZC = DateTime.Now;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%OP: " + operacja.ToLower() + "%"; //operacja
            data = Encoding.ASCII.GetBytes(wiadomosc);
            client.Send(data, data.Length);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%ST: brak%"; //status
            data = Encoding.ASCII.GetBytes(wiadomosc);
            client.Send(data, data.Length);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%L1: " + liczba1 + "%"; //liczba 1
            data = Encoding.ASCII.GetBytes(wiadomosc);
            client.Send(data, data.Length);
            wiadomosc = string.Empty;

            NS--;

            if (NS == 0)
            {
                ZC = DateTime.Now;
                wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%L2: " + liczba2 + "%"; //liczba 2
                data = Encoding.ASCII.GetBytes(wiadomosc);
                client.Send(data, data.Length);
                wiadomosc = string.Empty;
            }




            while (true) //oczekiwanie na zwrotne informacje
            {
                data = client.Receive(ref ip);

                string dane = Encoding.ASCII.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' });
                AllSplits[count] = split;
                count++;
                if (Convert.ToInt32(split[5]) == 0) { break; }
            }
            foreach (string[] i in AllSplits) //segregacja informacji z komunikatów
            {

                if (OH.ToLower() == " hisid" || OH.ToLower() == " hisido")
                {

                    if (i[6] == "OP") noname.operacja = i[7];
                    else if (i[6] == "ST") ST = i[7];
                    else if (i[6] == "L1") { noname.liczba1 = i[7]; n++; }
                    else if (i[6] == "L2") { noname.liczba2 = i[7]; n++; }
                    else if (i[6] == "LW") { noname.wynik = i[7]; n++; }
                    else if (i[6] == "IDO") { noname.IDO = Convert.ToInt32(i[7]); n++; }
                    else if (i[6] == "ER") ER = i[7];
                    if ((noname.operacja == " silnia" && n == 3) || n == 4) { n = 0; Historia.Add(noname); noname = new Wpis(); }
                }
                else
                {
                    if (i == null) break;
                    if (i[6] == "OP") { OP = i[7]; if (OP.ToLower() == " hisid" || OP.ToLower() == " hisido") OH = OP; }
                    else if (i[6] == "ST") ST = i[7];
                    else if (i[6] == "L1") liczba1 = Convert.ToInt32(i[7]);
                    else if (i[6] == "L2") liczba2 = Convert.ToInt32(i[7]);
                    else if (i[6] == "LW") wynik = Convert.ToDouble(i[7]);
                }
                bigbrain++;
                if (bigbrain == count) { bigbrain = 0; break; }

            }
            if (ST == " ERROR_0")
                Console.WriteLine("Wystapil blad : [Niepoprawny format]");
            else if (ST == " ERROR_1")
                Console.WriteLine("Wystapil blad : [Dzielenie przez zero]");
            else if (ST == " ERROR_2")
                Console.WriteLine("Wystapil blad : [Brak dostępu]");
            else if (ST == " ERROR_3")
                Console.WriteLine("Wystapil blad : [Brak operacji]");
            else if (ST == " ERROR_4")
                Console.WriteLine("Wystapil blad : [Przekroczono rozmiar stosu]");
            else if (OH.ToLower() == " hisid" || OH.ToLower() == " hisido")
            {
                Console.WriteLine("");
                Console.WriteLine("Historia obliczen dla sesji {0}.:", ID); //Wyświetlanie Historii obliczeń
                foreach (var i in Historia)
                {
                    Console.WriteLine("----------------Obliczenie {0}.------------------", i.IDO);
                    Console.WriteLine("Operacja :{0}", i.operacja);
                    Console.WriteLine("Liczba 1: {0}", i.liczba1);
                    if (i.operacja.ToLower() != "  silnia") { Console.WriteLine("Liczba 2: {0}", i.liczba2); }
                    Console.WriteLine("Wynik: {0}", i.wynik);
                }
                Console.WriteLine("---------------------------------------------");
                OH = String.Empty;
            }
            else
            {
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Operacja: {0}",OP);
                Console.WriteLine("Wynik: {0} ", wynik); //Wyświetlenie wyniku obliczeń
                Console.WriteLine("---------------------------------------------");
            }
        }

    }
}
