using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{
    public class Obliczenia
    {
        static int Dodawanie(int a, int b) => a + b; //dodawanie dwoch liczb
        static int Odejmowanie(int a, int b) => a - b; //odejmowanie dwoch liczb
        static int Mnozenie(int a, int b) => a * b; //mnozenie dwoch liczb
        static int Silnia(int a) //silnia 
        {
            if (a < 2) return 1;
            return a * Silnia(a - 1);
        }
        public static void obliczenia(UdpClient server, IPEndPoint ip, byte[] data, int ID, ref int IDO, List<Wpis> Historia) //Funkcja odbierająca komunikaty dotyczące obliczeń i przeprowadzająca je
        {
            //zmiennie niezbedne do wykonania
            DateTime ZC;
            string wiadomosc = string.Empty, OP = string.Empty, dane, ST = "OK";
            int l1 = 0, l2 = 0;
            double wynik = 0;
            string[][] AllSplits = new string[4][];
            int n = 0, NS;
            Wpis temp = new Wpis();
            temp.ID = ID;
            while (true)  //oczekiwanie na komunikaty okreslajace operacje oraz liczby na których ma ona być przeprowadzona
            {
                data = server.Receive(ref ip);
                dane = Encoding.ASCII.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' });
                AllSplits[n] = split;
                Console.WriteLine(dane);
                n++;
                if (Convert.ToInt32(split[5]) <= 0) break;
            }
            foreach (string[] i in AllSplits) //interpretacja komunikatow
            {
                if (i == null) break;
                if (i[6] == "OP")
                {
                    temp.operacja = OP = i[7];
                }
                else if (i[6] == "L1") 
                    try 
                    {
                        temp.liczba1 = i[7];
                        l1 = Convert.ToInt32(i[7]); 
                    }
                    catch (Exception e) 
                    {
                        if (OP.ToLower() != " hisid" && OP.ToLower() != " hisido")
                        {
                            temp.IDO = IDO++;
                            obslugaBledow.ObslugaBledow(server, e, ref ID, OP, ip, temp, ref Historia, 0);
                        }
                        else
                            obslugaBledow.ObslugaBledow(server, e, ref ID, OP, ip, 0);
                        return; 
                    }
                else if (i[6] == "L2") 
                    try 
                    {
                        temp.liczba2 = i[7];
                        l2 = Convert.ToInt32(i[7]); 
                    }
                    catch (Exception e) 
                    {
                        if (OP.ToLower() != " hisid" && OP.ToLower() != " hisido")
                        {
                            temp.IDO = IDO++;
                            obslugaBledow.ObslugaBledow(server, e, ref ID, OP, ip, temp, ref Historia, 0);
                        }
                        else
                         obslugaBledow.ObslugaBledow(server, e, ref ID, OP, ip, 0);
                        return; 
                    }
            }

            // obliczanie wynikow
            if (OP.ToLower() == " mnozenie") 
            { 
                wynik = Mnozenie(l1, l2); 
                temp.IDO = IDO++; 
            }
            else if (OP.ToLower() == " dzielenie") 
                try 
                { 
                    wynik = l1 / l2; 
                    temp.IDO = IDO++; 
                }
                catch (DivideByZeroException e) 
                {
                    temp.IDO = IDO++;
                    obslugaBledow.ObslugaBledow(server, e, ref ID, OP, ip, temp,ref Historia,1); 
                    return; 
                }
            else if (OP.ToLower() == " dodawanie") 
            { 
                wynik = Dodawanie(l1, l2); 
                temp.IDO = IDO++; 
            }
            else if (OP.ToLower() == " odejmowanie") 
            { 
                wynik = Odejmowanie(l1, l2); 
                temp.IDO = IDO++; 
            }
            // wywolanie przez uzytkownika podania calej historii
            else if (OP.ToLower() == " hisid") 
                try
                {
                    if (l1 != ID) throw new Exception("brakDostepu!");
                    else 
                    { 
                        His.HisID(Historia, l1, server, ip); 
                        return; 
                    }
                }
                catch (Exception e)
                {
                    
                    obslugaBledow.ObslugaBledow(server, e, ref ID, OP, ip, 2); 
                    return;
                }
            //wywolanie przez uzytkownika podania historii obliczenia
            else if (OP.ToLower() == " hisido") 
                try
                {
                    if (l1 != ID) throw new Exception("2");
                    else if (l2 > IDO - 1) throw new Exception("3");
                    else 
                    { 
                        His.HisIDO(Historia, l1, l2, server, ip); 
                        return; 
                    }
                }
                catch (Exception e)
                {
                    obslugaBledow.ObslugaBledow(server, e, ref ID, OP, ip,Convert.ToInt32(e.Message));
                    return;
                }
            else if (OP.ToLower() == " silnia") 
                try 
                { 
                    temp.IDO = IDO++; 
                    if (l1 >= 12 || l1 < 0 ) throw new Exception("4");
                    wynik = Silnia(l1); 
                   
                }
                catch (Exception e) 
                { 
                    obslugaBledow.ObslugaBledow(server, e, ref ID, OP, ip, temp, ref Historia,4); 
                    return; 
                }
            temp.wynik = Convert.ToString(wynik);
            if (OP.ToLower() != " hisido" || OP.ToLower() != " hisid")
                Historia.Add(temp);
            NS = 2;
            //przygotowanie komunikatow
            Console.WriteLine(wynik);
            ZC = DateTime.Now;
            wiadomosc = string.Empty;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%OP: " + OP.ToLower() + "%"; //odsyłanie komunikatu z rodzajem operacji , jej statusem i wynikiem
            data = Encoding.ASCII.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%ST: " + ST + "%";
            data = Encoding.ASCII.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%LW: " + wynik + "%";
            data = Encoding.ASCII.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;
        }
    }
}
