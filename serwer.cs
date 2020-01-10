using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{
   

    

    class Program
    {
        static int Dodawanie(int a, int b) => a + b;

        static int Odejmowanie(int a, int b) => a - b;

        static int Mnozenie(int a, int b) => a * b;
        
        
        static void ObslugaBledow(UdpClient server, Exception e,int ID,string OP,IPEndPoint ip)
        {
            string temp = string.Empty;
            byte[] data;
            DateTime ZC=DateTime.Now;
            temp = "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:2%OP:" + OP;
            data = Encoding.Unicode.GetBytes(temp);
            server.Send(data, data.Length, ip);
            temp = string.Empty;
            temp = "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:1%ST:ERROR";
            data = Encoding.Unicode.GetBytes(temp);
            server.Send(data, data.Length, ip);
            temp = string.Empty;
            temp = "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:0%ER:"+e.Message;
            data = Encoding.Unicode.GetBytes(temp);
            server.Send(data, data.Length, ip);
            temp = string.Empty;
        }

        static int Silnia(int a)
        {
            if (a < 2) return 1;
            return a * Silnia(a - 1);
        }

     

        static void obliczenia(UdpClient server, IPEndPoint ip, byte[] data, int ID) //Funkcja odbierająca komunikaty dotyczące obliczeń i przeprowadzająca je
        {
            
            DateTime ZC; 
            string wiadomosc = string.Empty, OP = string.Empty, dane, ST = "OK";
            int l1 = 0, l2 = 0;
            double wynik = 0;
            string[][] AllSplits = new string[4][];
            int n = 0, NS;

            while (true)  //oczekiwanie na komunikaty okreslajace operacje oraz liczby na których ma ona być przeprowadzona
            {
                data = server.Receive(ref ip);
                dane = Encoding.Unicode.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' });
                AllSplits[n] = split;
                Console.WriteLine(dane);
                n++;
                if (Convert.ToInt32(split[5]) <= 0) break;
            }
            foreach (string[] i in AllSplits) //interpretacja komunikatow
            {
                if (i == null) break;
                if (i[6] == "OP") OP = i[7];
                else if (i[6] == "L1") try { l1 = Convert.ToInt32(i[7]); }
                    catch (FormatException e) { ObslugaBledow(server, e, ID, OP, ip); return; }
                else if (i[6] == "L2") try { l2 = Convert.ToInt32(i[7]); }
                    catch (FormatException e) { ObslugaBledow(server, e, ID, OP, ip); return; }
            }
            if (OP.ToLower() == "mnozenie") wynik = Mnozenie(l1, l2);
            else if (OP.ToLower() == "dzielenie") try { wynik = l1 / l2; }
                catch (DivideByZeroException e) { ObslugaBledow(server, e, ID, OP, ip);return; }
            else if (OP.ToLower() == "dodawanie") wynik = Dodawanie(l1, l2);
            else if (OP.ToLower() == "odejmowanie") wynik = Odejmowanie(l1, l2);
            else if (OP.ToLower() == "silnia") try { wynik = Silnia(l1); }
                catch(StackOverflowException e) { ObslugaBledow(server, e, ID, OP, ip);return; }
            NS = 2;

            Console.WriteLine(wynik);
            ZC = DateTime.Now;
            wiadomosc = string.Empty; 
            wiadomosc += "ZC:"+ ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:" + NS + "%OP:" + OP.ToLower() + "%"; //odsyłanie komunikatu z rodzajem operacji , jej statusem i wynikiem
            data = Encoding.Unicode.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:" + NS + "%ST:" + ST + "%";
            data = Encoding.Unicode.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:" + NS + "%LW:" + wynik + "%";
            data = Encoding.Unicode.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;
        }

        static void polaczenie(UdpClient server,IPEndPoint ip, int ID) 
        {

            int n = 0;
            string dane;
            string[][] AllSplits = new string[4][];
            byte[] data;
            string wiadomosc = string.Empty;
        
            while (true)                                        //oczekiwanie na połączenie z klientem
            {
                data = server.Receive(ref ip);
                if (data != null)
                    break;
            }
            DateTime ZC = DateTime.Now;
            wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:1%OP:NadanieID%";
            data = Encoding.Unicode.GetBytes(wiadomosc); //Nadanie ID
            server.Send(data, data.Length, ip);
            ZC = DateTime.Now;
            wiadomosc = string.Empty;
            wiadomosc += "ZC:" + ZC.ToString("HH-mm-sstt") + "%ID:" + ID + "%NS:0%ST:BRAK%";
            data = Encoding.Unicode.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;
            while (true)         //Oczekiwanie na potwierdzenie odbioru ID
            {


                data = server.Receive(ref ip);

                dane = Encoding.Unicode.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' });
                AllSplits[n] = split;
                Console.WriteLine(dane);
                n++;
                if (Convert.ToInt32(split[5]) <= 0)
                {
                    n = 0;
                    if (split != null)
                    {
                        if (AllSplits[1][7] == "OK")
                            break;
                    }
                }

            }
            obliczenia(server, ip, data, ID);

            do 
            {
                while (true) //oczekiwanie na informacje o dalszych obliczeniach lub zakończeniu pracy
                {
                    data = server.Receive(ref ip);
                    dane = Encoding.Unicode.GetString(data, 0, data.Length);
                    string[] split = dane.Split(new char[] { ':', '%' });
                    AllSplits[n] = split;
                    Console.WriteLine(dane);
                    n++;
                    if (Convert.ToInt32(split[5]) <= 0) { n = 0; break; }
                }

                if (AllSplits[0][6] == "OP")
                {
                    if (AllSplits[0][7].ToLower() == "koniec") { ID++; polaczenie(server, ip, ID); }
                    else if (AllSplits[0][7].ToLower() == "ponow") { obliczenia(server, ip, data, ID); }
                }

            } while (true);
        }



        static void Main(string[] args)                                          //TU JEST MAIN TU JEST MAIN TU JEST MAIN TU JEST MAIN TU JEST MAIN
        {
            int ID = 0;

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 11000);

            UdpClient server = new UdpClient(/*"127.0.0.1" ,*/11000);

            // ZC:czas%ID:0%NS:0%OP:mnozenie%
            polaczenie(server, ip, ID);

           
            
            
            server.Close();

            /*
            while (true)
            {
                byte[] data = server.Receive(ref ip);

                string dane = Encoding.Unicode.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' });
                Console.WriteLine(dane);
                if (split != null)
                {
                    foreach (string n in split)
                        Console.WriteLine(n);
                    break;
                }
                
            }
            */

            //server.Close();
            /* 
            // This constructor arbitrarily assigns the local port number.
            UdpClient udpClient = new UdpClient(11000);
            try
            {
                udpClient.Connect("127.0.0.1", 11000);

                // Sends a message to the host to which you have connected.
                Byte[] sendBytes = Encoding.ASCII.GetBytes("1");

                udpClient.Send(sendBytes, sendBytes.Length);
                
                // Sends a message to a different host using optional hostname and port parameters.
                UdpClient udpClientB = new UdpClient();
                udpClientB.Send(sendBytes, sendBytes.Length, "127.0.0.1", 11000);

                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                Console.WriteLine("This is the message you received " +
                                             returnData.ToString());
                Console.WriteLine("This message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());
               
                udpClient.Close();
                udpClientB.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            */
            //Console.Read();
        }
    }
}


