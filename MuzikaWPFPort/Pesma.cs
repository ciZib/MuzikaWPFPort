using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuzikaWPFPort
{
    class Pesma
    {
        public string ImePesme { get; set; }
        public string Path { get; set; }
        public string DuzinaTrajanja { get; set; }
        public Pesma(string ime, string path, string duzina)
        {
            ImePesme = ime;
            Path = path;
            DuzinaTrajanja = duzina;
        }
        public override string ToString() => $"{ImePesme}:\t{DuzinaTrajanja}";
    }
}