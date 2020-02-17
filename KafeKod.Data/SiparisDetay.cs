using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafeKod.Data
{
   public  class SiparisDetay
    {
        public int Adet { get; set; }

        public string UrunAd { get; set; }

        public decimal BirimFiyat { get; set; }

        public decimal Tutar() => Adet * BirimFiyat;

        public void SarkiSoyle() => Console.WriteLine("Havada ay ışığı");

    }
}
