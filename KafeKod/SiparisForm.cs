using KafeKod.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeKod
{
    public partial class SiparisForm : Form
    {
        KafeVeri db;
        Siparis siparis;
        BindingList<SiparisDetay> blSiparisDetaylar; //DataGridView ı tektikler
        public SiparisForm(KafeVeri kafeVeri,Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            blSiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            InitializeComponent();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler;
           // cboUrun.SelectedItem = null;
            dgvSiparisDetaylari.DataSource = blSiparisDetaylar;

        }

        private void TutarGuncelle()
        {
            lblTutar.Text = siparis.ToplamTutarTL;
        }

        private void MasaNoGuncelle()
        {
            Text = "Masa " + siparis.MasaNo;
            lblMasaNo.Text = siparis.MasaNo.ToString("00");
        }

        private void btnAnaSayfa_Click(object sender, EventArgs e) => Close();
        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cboUrun.SelectedItem==null)
            {
                MessageBox.Show("Ürün seçiniz!!");
                return;
            }
            Urun seciliUrun = (Urun)cboUrun.SelectedItem;
            var sd = new SiparisDetay
            {
                UrunAd = seciliUrun.UrunAd,
                BirimFiyat = seciliUrun.BirimFiyat,
                Adet = (int)nudAdet.Value
            };
            blSiparisDetaylar.Add(sd);
            cboUrun.SelectedItem = null;
            nudAdet.Value = 1;
            TutarGuncelle();
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Sipariş iptal edilecektir",
                "Sipariş İptal onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Iptal;
                siparis.KapanisZamani = DateTime.Now;
                Close();
            }

        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("Hesap kapanacaktır onaylıyor musunuz ?",
                "Hesap Kapatma",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Odendi;
                siparis.KapanisZamani = DateTime.Now;
                siparis.OdenenTutar = siparis.ToplamTutar();
                this.Close();
            }

        }
    }
}
