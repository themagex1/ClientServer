using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{
    public class His
    {
        //KLIENT
        // ZC:czas%ID:0%NS:2%OP:hisid%
        // ZC:czas%ID:0%NS:1%ST:BRAK%
        // ZC:czas%ID:0%NS:0%L1:1%

        //SERWER
        // ZC:czas%ID:0%NS:0%OP:hisid%
        // ZC:czas%ID:0%NS:0%ST:OK% /  // ZC:czas%ID:0%NS:0%ST:bdost%
        // ZC:czas%ID:0%NS:0%OP:mnozenie%
        // ZC:czas%ID:0%NS:0%IDO:0%
        // ZC:czas%ID:0%NS:0%L1:2%
        // ZC:czas%ID:0%NS:0%L2:3%
        // ZC:czas%ID:0%NS:0%LW:6%
        public static void HisID(List<Wpis> Historia,int ID, UdpClient server, IPEndPoint ip)
        {
            int NS;
            DateTime ZC;
            string wiadomosc;
            byte[] data;
            int silnia = 0;
            List<Wpis> temp = new List<Wpis>();
            foreach(var i in Historia) //odczyt historii danej sesji
            {
                if (i.ID == ID)
                {
                    temp.Add(i);
                    if (i.operacja == "silnia") silnia++;
                }
            }
            NS = (temp.Count()*5)-silnia+1; //obliczenie ilosci komunikatow do wyslania
            ZC = DateTime.Now;
            wiadomosc = string.Empty;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%OP: HISID%"; //odsyłanie komunikatu sygnalizujacego przeslanie historii po id sesji
            data = Encoding.ASCII.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc = string.Empty;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%ST: OK%"; //odsyłanie komunikatu ze statusem 
            data = Encoding.ASCII.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;

            NS--;
            foreach (var i in temp)
            {
                ZC = DateTime.Now;
                wiadomosc = string.Empty;
                wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%OP: " + i.operacja + "%"; //odsyłanie komunikatu z rodzajem operacji 
                data = Encoding.ASCII.GetBytes(wiadomosc);
                server.Send(data, data.Length, ip);
                wiadomosc = string.Empty;

                NS--;

                ZC = DateTime.Now;
                wiadomosc = string.Empty;
                wiadomosc += "ZC:" + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%IDO: " + i.IDO + "%"; //odsyłanie komunikatu z ID operacji
                data = Encoding.ASCII.GetBytes(wiadomosc);
                server.Send(data, data.Length, ip);
                wiadomosc = string.Empty;

                NS--;

                ZC = DateTime.Now;
                wiadomosc = string.Empty;
                wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%L1: " + i.liczba1 + "%"; //odsyłanie komunikatu z 1. liczba
                data = Encoding.ASCII.GetBytes(wiadomosc);
                server.Send(data, data.Length, ip);
                wiadomosc = string.Empty;

                NS--;
                if (i.operacja != "silnia")
                {
                    ZC = DateTime.Now;
                    wiadomosc = string.Empty;
                    wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%L2: " + i.liczba2 + "%"; //odsyłanie komunikatu z 2. liczba
                    server.Send(data, data.Length, ip);
                    wiadomosc = string.Empty;

                    NS--;
                }
                ZC = DateTime.Now;
                wiadomosc = string.Empty;
                wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%LW: " + i.wynik + "%"; //odsyłanie komunikatu z wynikiem
                data = Encoding.ASCII.GetBytes(wiadomosc);
                server.Send(data, data.Length, ip);
                wiadomosc = string.Empty;

                NS--;
            }

        }
        public static void HisIDO(List<Wpis> Historia, int ID, int IDO, UdpClient server, IPEndPoint ip)
        {
            int NS;
            DateTime ZC;
            string wiadomosc;
            byte[] data;
            int silnia = 0;
            List<Wpis> temp = new List<Wpis>();
            foreach (var i in Historia)
            {
                if (i.ID == ID)
                {
                    if (i.IDO == IDO)
                    {
                        temp.Add(i);
                        if (i.operacja == "silnia") silnia++;
                    }
                }
            }
            NS = (temp.Count() * 5) - silnia + 1;
            ZC = DateTime.Now;
            wiadomosc = string.Empty;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%OP: HISIDO%"; //odsyłanie komunikatu sygnalizujacego przeslanie historii po id obliczen
            data = Encoding.ASCII.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;

            NS--;
            ZC = DateTime.Now;
            wiadomosc = string.Empty;
            wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%ST: OK%";  //odsyłanie komunikatu ze statusem 
            data = Encoding.ASCII.GetBytes(wiadomosc);
            server.Send(data, data.Length, ip);
            wiadomosc = string.Empty;

            NS--;
            foreach (var i in temp)
            {
                ZC = DateTime.Now;
                wiadomosc = string.Empty;
                wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%OP: " + i.operacja + "%"; //odsyłanie komunikatu z rodzajem operacji
                data = Encoding.ASCII.GetBytes(wiadomosc);
                server.Send(data, data.Length, ip);
                wiadomosc = string.Empty;

                NS--;

                ZC = DateTime.Now;
                wiadomosc = string.Empty;
                wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%IDO: " + i.IDO + "%"; //odsyłanie komunikatu z ID obliczen
                data = Encoding.ASCII.GetBytes(wiadomosc);
                server.Send(data, data.Length, ip);
                wiadomosc = string.Empty;

                NS--;

                ZC = DateTime.Now;
                wiadomosc = string.Empty;
                wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%L1: " + i.liczba1 + "%"; //odsyłanie komunikatu z liczba 1.
                data = Encoding.ASCII.GetBytes(wiadomosc);
                server.Send(data, data.Length, ip);
                wiadomosc = string.Empty;

                NS--;
                if (i.operacja != "silnia")
                {
                    ZC = DateTime.Now;
                    wiadomosc = string.Empty;
                    wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%L2: " + i.liczba2 + "%"; //odsyłanie komunikatu z liczba 2.
                    data = Encoding.ASCII.GetBytes(wiadomosc);
                    server.Send(data, data.Length, ip);
                    wiadomosc = string.Empty;

                    NS--;
                }
                ZC = DateTime.Now;
                wiadomosc = string.Empty;
                wiadomosc += "ZC: " + ZC.ToString("dd/MM/yyyy HH-mm-sstt") + "%ID: " + ID + "%NS: " + NS + "%LW: " + i.wynik + "%"; //odsyłanie komunikatu z wynikiem
                data = Encoding.ASCII.GetBytes(wiadomosc);
                server.Send(data, data.Length, ip);
                wiadomosc = string.Empty;

                NS--;
            }
        }
    }
}
