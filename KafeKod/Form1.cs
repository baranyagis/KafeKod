using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KafeKod.Data;
using KafeKod.Properties;

namespace KafeKod
{
    public partial class Form1 : Form
    {
        KafeVeri db;
        int masaAdet = 20;

        public Form1()
        {
            db = new KafeVeri(); // 3 Kafeveriyi açtık
            OrnekleriYukle();
            InitializeComponent();
            MasalariOlustur();
        }

        #region 3 Urunler Oluştur
        private void OrnekleriYukle()
        {
            db.Urunler = new List<Urun>
            {
                new Urun { UrunAd="Kola",BirimFiyat=3.99m},
                new Urun { UrunAd="Çay",BirimFiyat=2.5m}
            };
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
            for (int masaNo = 1; masaNo <= masaAdet; masaNo++)
            {
                lvi = new ListViewItem("Masa " + masaNo);
                lvi.Tag = masaNo;
                lvi.ImageKey = "bos";
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

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            var frm = new GecmisSiparislerForm(db);
            frm.ShowDialog();
        }
    }
}
