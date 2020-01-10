using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Client
{
    
    //komentarze

    class Program
    {

       

       
        static void Main(string[] args)                                                                         //TU JEST MAIN TU JEST MAIN TU JEST MAIN TU JEST MAIN
        {
            UdpClient client = new UdpClient();
            if (Pol.polaczenieClient(ref client)== false ) return;// użytkownik podaje IP oraz port   
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 11000);

            int n = 0;
            int ID = 0;
            string odp;
            byte[] data;
            string[][] AllSplits = new string[4][];
            DateTime ZC;
            while (true) //oczekiwanie na nadanie ID
            {
                data = client.Receive(ref ip);

                string dane = Encoding.ASCII.GetString(data, 0, data.Length);
                string[] split = dane.Split(new char[] { ':', '%' });
                AllSplits[n] = split;
                n++;
                if (Convert.ToInt32(split[5]) <= 0)
                {
                 
                    n = 0;

                    try //Potwierdzenie nadania ID
                    {
                        ID = Convert.ToInt32(split[3]);
                        ZC = DateTime.Now;
                        Console.WriteLine("ID Twojej sesji to: {0}",ID);
                        Console.WriteLine("");
                        string temp = "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 1%OP: NadanieID%";
                        data = Encoding.ASCII.GetBytes(temp);
                        client.Send(data, data.Length);
                        temp = string.Empty;
                        ZC = DateTime.Now;
                        temp = "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 0%ST: OK%";
                        data = Encoding.ASCII.GetBytes(temp);
                        client.Send(data, data.Length);
                    }
                    catch (Exception) { }
                    break;
                }
            }
            Op.operacje(client, ip, data, ID); //wybranie operacji i wysłanie odpowiednich komunikatów do serwera

            do //zapytanie o kontynuacje pracy
            {
                Console.WriteLine("Czy chcesz wykonać ponowne obliczenia ? (0/1)");
                odp = Console.ReadLine();
                if (odp == "1") {
                    ZC = DateTime.Now;
                    string wiadomosc = string.Empty; 
                    wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 1%OP: PONOW% ";
                    data = Encoding.ASCII.GetBytes(wiadomosc);
                    client.Send(data, data.Length);

                    wiadomosc = string.Empty;
                    ZC = DateTime.Now;
                    wiadomosc = "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 0%ST: BRAK%";
                    data = Encoding.ASCII.GetBytes(wiadomosc);
                    client.Send(data, data.Length);
                    wiadomosc = string.Empty;

                    Op.operacje(client, ip, data, ID);
                }
                else if (odp == "0") {

                    string wiadomosc = string.Empty;
                    ZC = DateTime.Now;
                    wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 1%OP: KONIEC%";
                    data = Encoding.ASCII.GetBytes(wiadomosc);
                    client.Send(data, data.Length);

                    wiadomosc = string.Empty;
                    ZC = DateTime.Now;
                    wiadomosc = "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: 0%ST: BRAK%";
                    data = Encoding.ASCII.GetBytes(wiadomosc);
                    client.Send(data, data.Length);
                    wiadomosc = string.Empty;
                    client.Close();
                }
                else Console.WriteLine("Niepoprawna odpowiedź");
            } while (odp != "0"); 
              
         }

          
        }
    }
