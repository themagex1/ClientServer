using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{
    public class Wpis //klasa sluzaca do uporzadkowania wpisow do historii
    {
        public int ID { get; set; }
        public int IDO { get; set; }
        public string liczba1 { get; set; }
        public string liczba2 { get; set; }
        public string wynik { get; set; }
        public string operacja { get; set; }
        public Wpis(string liczba1, string liczba2, string operacja, int ID, int IDO, string wynik)
        {
            this.ID = ID;
            this.IDO = IDO;
            this.liczba1 = liczba1;
            this.liczba2 = liczba2;
            this.operacja = operacja;
            this.wynik = wynik;
        }

        public Wpis()
        {
            this.ID = 0;
            this.IDO = 0;
            this.liczba1 = "0";
            this.liczba2 = "0";
            this.operacja = "";
            this.wynik = "0";
        }
    }
    
}
