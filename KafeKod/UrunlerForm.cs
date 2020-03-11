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
    public partial class UrunlerForm : Form
    {
        KafeContext db;

        public UrunlerForm(KafeContext kafeVeri)
        {
            db = kafeVeri;
            InitializeComponent();
            dgvUrunler.AutoGenerateColumns = false;
            dgvUrunler.DataSource = db.Urunler.OrderBy(x => x.UrunAd).ToList();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAd.Text.Trim();
            if (urunAd=="")
            {
                MessageBox.Show("Ürün adını girmediniz!!");
            }
            db.Urunler.Add(new Urun
            {
                UrunAd = urunAd,
                BirimFiyat = nudBirimFiyat.Value

            });
            db.SaveChanges();
            dgvUrunler.DataSource = db.Urunler.OrderBy(x => x.UrunAd).ToList(); //ÜRÜN ADINA GÖRE SIRALAMA

        }

        private void dgvUrunler_DataError(object sender, DataGridViewDataErrorEventArgs e) //FİYAT KISMINA HARF GİRMEMESİ İÇİN
        {
            MessageBox.Show("Geçersiz bir değer girdiniz");
        }

        private void dgvUrunler_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex==0) //UrunAd'ı düzenliyorsa
            {
                if (e.FormattedValue.ToString().Trim() == "")
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "Ürünün adı boş olamaz";
                    e.Cancel = true;
                }
                else
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "";
                    db.SaveChanges();
                }
            }
        }
    }
}
