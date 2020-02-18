using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafeKod.Data
{
    public enum SiparisDurum {Aktif,Odendi,Iptal }
    public class Siparis
    {
        public int MasaNo { get; set; }
        public DateTime? AcilisZamani { get; set; }
        public DateTime? KapanisZamani { get; set; }
        public SiparisDurum Durum { get; set; }

        public List<SiparisDetay> siparisdetaylar { get; set; }

        public string ToplamTutarTL => string.Format("{0:0.00}₺", ToplamTutar());

        public decimal ToplamTutar() => siparisdetaylar.Sum(Fiyat);


        private decimal Fiyat(SiparisDetay x)
        {
            return x.Tutar();
        }
    }
}
