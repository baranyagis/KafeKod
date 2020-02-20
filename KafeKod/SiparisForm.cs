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

        KafeVeri db;
        Siparis siparis;
        BindingList<SiparisDetay> blSiparisDetaylar; //DataGridView ı tektikler
        public SiparisForm(KafeVeri kafeVeri,Siparis siparis)
        {
            db = kafeVeri;
            this.siparis = siparis;
            blSiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            InitializeComponent();
            MasaNolariYukle();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler;
           // cboUrun.SelectedItem = null;
            dgvSiparisDetaylari.DataSource = blSiparisDetaylar;

        }

        private void MasaNolariYukle()
        {
            cboMasaNo.Items.Clear();
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                if (!db.AktifSiparisler.Any(x=> x.MasaNo==i)) 
                {
                    cboMasaNo.Items.Add(i);
                }

            }
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
                blSiparisDetaylar.Remove(sipDetay);
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
