using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KafeKod.Data;
using KafeKod.Properties;
using Newtonsoft.Json;

namespace KafeKod
{
    public partial class Form1 : Form
    {
        KafeVeri db;

        public Form1()
        {

            VerileriOku();
            InitializeComponent();
            MasalariOlustur();
        }

        private void VerileriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json);
            }
            catch (Exception)
            {
                db = new KafeVeri();
            }
        }

        #region 3 Urunler Oluştur
        private void OrnekleriYukle()
        {
            db.Urunler = new List<Urun>
            {
                new Urun { UrunAd="Kola",BirimFiyat=4.00m},
                new Urun { UrunAd="Çay",BirimFiyat=2.5m}
            };
            db.Urunler.Sort();
        }
        #endregion

        private void MasalariOlustur()
        {
            #region 2 MasaİçinResimler
            ImageList il = new ImageList(); // 1 İmageList oluşturulur
            il.Images.Add("bos", Resources.masabos); // 2 Resim listeye eklenir
            il.Images.Add("dolu", Resources.masadolu);
            il.ImageSize = new Size(60, 60); // 3 Boyut hazırlanır
            lvwMasalar.LargeImageList = il; // 4 ListView a ekledik
            #endregion


            #region 1 MASALARI EKLE
            ListViewItem lvi;
            for (int masaNo = 1; masaNo <= db.MasaAdet; masaNo++)
            {
                lvi = new ListViewItem("Masa " + masaNo);
                //Masa boş mu dolu mu?
                Siparis sip = db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);

                if (sip==null)
                {
                    lvi.Tag = masaNo;
                    lvi.ImageKey = "bos";
                }
                else
                {
                    lvi.Tag = sip;
                    lvi.ImageKey = "dolu";
                }
                lvwMasalar.Items.Add(lvi);
            }
            #endregion
        }

        private void lvwMasalar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var lvi = lvwMasalar.SelectedItems[0];
                lvi.ImageKey = "dolu";

                #region 4 Masada biri oturuyor mu yoksa boş mu
                Siparis sip;


                if (lvi.Tag is Siparis)
                {
                    sip = (Siparis)lvi.Tag;
                }
                else
                {
                    sip = new Siparis();
                    sip.MasaNo = (int)lvi.Tag;
                    sip.AcilisZamani = DateTime.Now;
                    lvi.Tag = sip;
                    db.AktifSiparisler.Add(sip);
                }

                SiparisForm frmSiparis1 = new SiparisForm(db, sip);
                frmSiparis1.MasaTasima += FrmSiparis1_MasaTasima;
                frmSiparis1.ShowDialog();



                if (sip.Durum != SiparisDurum.Aktif)
                {
                    lvi.Tag = sip.MasaNo;
                    lvi.ImageKey = "bos";
                    db.AktifSiparisler.Remove(sip);
                    db.GecmisSiparis.Add(sip);
                }
                #endregion
            }


        }

        private void FrmSiparis1_MasaTasima(object sender, MasaTasimaEventArgs e)
        {
            //1 Eski masa boşalt 2 yeni masaya sipariş koy

            ListViewItem lviEskiMasa = MasaBul(e.EskiMasaNo);
            lviEskiMasa.Tag = e.EskiMasaNo;
            lviEskiMasa.ImageKey = "bos";

            ListViewItem lviYeniMasa = MasaBul(e.YeniMasaNo);
            lviYeniMasa.Tag = e.TasinanSiparis;
            lviYeniMasa.ImageKey = "dolu";
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            var frm = new GecmisSiparislerForm(db);
            frm.ShowDialog();
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            var frm = new UrunlerForm(db);
            frm.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           string json = JsonConvert.SerializeObject(db);
            File.WriteAllText("veri.json", json);
        }

        public ListViewItem MasaBul(int masaNo)
        {
            foreach (ListViewItem item in lvwMasalar.Items)
            {
                if (item.Tag is int && (int)item.Tag == masaNo)
                {
                    return item;
                }
                else if(item.Tag is Siparis && ((Siparis)item.Tag).MasaNo==masaNo)
                    return item;
            }
            return null;
        }

    }

    
}
