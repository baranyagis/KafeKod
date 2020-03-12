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
        public event EventHandler<MasaTasimaEventArgs> MasaTasima; 

        KafeContext db;
        Siparis siparis;
        public SiparisForm(KafeContext kafeVeri,Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            InitializeComponent();
            dgvSiparisDetaylari.AutoGenerateColumns = false;
            MasaNolariYukle();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler.Where(x=>!x.StoktaYok).ToList();
            // cboUrun.SelectedItem = null;
            dgvSiparisDetaylari.DataSource = siparis.SiparisDetaylar;

        }

        private void MasaNolariYukle()
        {
            cboMasaNo.Items.Clear();
            for (int i = 1; i <= Properties.Settings.Default.MasaAdet; i++)
            {
                if (!db.Siparisler.Any(x=> x.MasaNo==i && x.Durum==SiparisDurum.Aktif)) 
                {
                    cboMasaNo.Items.Add(i);
                }

            }
        }

        private void TutarGuncelle()
        {
            lblTutar.Text = siparis.SiparisDetaylar
                            .Sum(x => x.Adet * x.BirimFiyat).ToString("0.00") + " TL";
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
                UrunId=seciliUrun.Id,
                UrunAd = seciliUrun.UrunAd,
                BirimFiyat = seciliUrun.BirimFiyat,
                Adet = (int)nudAdet.Value,
            };
            siparis.SiparisDetaylar.Add(sd);
            db.SaveChanges();
            dgvSiparisDetaylari.DataSource = new BindingSource(siparis.SiparisDetaylar,null); //ÇİZDİRME GARANTİLİDİR
            cboUrun.SelectedIndex = 0;
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
                db.SaveChanges();
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
                siparis.OdenenTutar = siparis.SiparisDetaylar.Sum(x => x.Adet * x.BirimFiyat);
                db.SaveChanges();
                this.Close();
            }

        }

        private void dgvSiparisDetaylari_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Right)
            {
                int rowIndex = dgvSiparisDetaylari.HitTest(e.X, e.Y).RowIndex;
                if (rowIndex>-1)
                {
                    dgvSiparisDetaylari.ClearSelection();
                    dgvSiparisDetaylari.Rows[rowIndex].Selected = true;
                    cmsSiparisDetay.Show(MousePosition);
                }
                
            }
        }

        private void tsmSiparisDetaySil_Click(object sender, EventArgs e)
        {
            if (dgvSiparisDetaylari.SelectedRows.Count>0)
            {
                var seciliSatir = dgvSiparisDetaylari.SelectedRows[0];
                var sipDetay = (SiparisDetay)seciliSatir.DataBoundItem;
                db.SiparisDetaylar.Remove(sipDetay);
                db.SaveChanges();
            } 

            TutarGuncelle();
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedItem==null)
            {
                MessageBox.Show("Lütfen masa numarası seçiniz");
                return;
            }

            int eskiMasaNo = siparis.MasaNo;
            int hedefMasaNo = (int)cboMasaNo.SelectedItem;

            if (MasaTasima != null)
            {
                var args = new MasaTasimaEventArgs
                {
                    EskiMasaNo = eskiMasaNo,
                    YeniMasaNo = hedefMasaNo,
                    TasinanSiparis = siparis

                };
                MasaTasima(this, args); //EVENT ÇAĞIRMA
            }

            siparis.MasaNo = hedefMasaNo;
            db.SaveChanges();
            MasaNoGuncelle();
            MasaNolariYukle();


        }
    }

    public class MasaTasimaEventArgs:EventArgs
    {
        public Siparis TasinanSiparis { get; set; }
        public int EskiMasaNo { get; set; }
        public int YeniMasaNo { get; set; }

    }
}
