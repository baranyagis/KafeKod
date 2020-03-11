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
        BindingList<Urun> blUrunler;

        public UrunlerForm(KafeContext kafeVeri)
        {
            db = kafeVeri;
            InitializeComponent();
            dgvUrunler.AutoGenerateColumns = false;
            blUrunler = new BindingList<Urun>(db.Urunler);
            dgvUrunler.DataSource = blUrunler;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAd.Text.Trim();
            if (urunAd=="")
            {
                MessageBox.Show("Ürün adını girmediniz!!");
            }
            blUrunler.Add(new Urun
            {
                UrunAd = urunAd,
                BirimFiyat = nudBirimFiyat.Value

            });
            db.Urunler.Sort();

        }

        private void dgvUrunler_DataError(object sender, DataGridViewDataErrorEventArgs e) //FİYAT KISMINA HARF GİRMEMESİ İÇİN
        {
            MessageBox.Show("Geçersiz bir değer girdiniz");
        }

        private void dgvUrunler_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex==0) //UrunAd'ı düzenliyorsa
            {
                if (((string)e.FormattedValue).Trim() == "")
                {
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "Ürünün adı boş olamaz";
                    e.Cancel = true;
                }
                else
                    dgvUrunler.Rows[e.RowIndex].ErrorText = "";
            }
        }
    }
}
