using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafeKod.Data
{
    public class KafeContext:DbContext
    {
        public KafeContext():base("name=KafeContext")
        {     
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //MODEL OLUŞTURULURKEN İSTENİLENLERİ YAZIYORUZ BURADAKİ AMAÇ:CASCADE DELETİNG KAPATMA
        // ((1)) CASCADE DELETE:Art arda silme, bir satırı silmenin ilgili satırların silinmesini otomatik 
        // ((2)) olarak tetiklemesine izin veren bir özelliği anlatmak için Veritabanı terminolojisinde yaygın olarak kullanılır. 
        {
           // modelBuilder.Entity<Urun>().ToTable("Urunler") TABLO ADINI VERME

            modelBuilder.Entity<Urun>()
                .HasMany(x => x.SiparisDetaylar)
                .WithRequired(x => x.Urun)
                .HasForeignKey(x => x.UrunId)
                .WillCascadeOnDelete(false);
        }

        public DbSet<Urun> Urunler { get; set; }

        public DbSet<Siparis> Siparisler { get; set; }

        public DbSet<SiparisDetay> SiparisDetaylar { get; set; }
    }
}
