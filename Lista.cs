using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{
    public class Lista
    {
        public int ID { get; set; }
        public int IDO { get; set; }
        public int liczba1 { get; set; }
        public int liczba2 { get; set; }
        public int wynik { get; set; }
        public string operacja { get; set; }
        public Lista(int liczba1, string operacja, int ID, int IDO, int wynik)
        {
            this.ID = ID;
            this.IDO = IDO;
            this.liczba1 = liczba1;
            this.operacja = operacja;
            this.wynik = wynik;
        }
        
    }
    
}
