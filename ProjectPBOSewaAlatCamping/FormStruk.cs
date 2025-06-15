using ProjectPBOSewaAlatCamping.dataAccess;
using ProjectPBOSewaAlatCamping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectPBOSewaAlatCamping
{
    public partial class FormStruk : Form
    {
        public FormStruk(int idTransaksi)
        {
            InitializeComponent();
            this.Text = "Struk Pembayaran";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            RichTextBox strukBox = new RichTextBox
            {
                Location = new Point(10, 10),
                Size = new Size(360, 430),
                ReadOnly = true,
                Font = new Font("Consolas", 10)
            };

            Button btnClose = new Button
            {
                Text = "Tutup",
                Location = new Point(150, 440),
                Size = new Size(100, 30)
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(strukBox);
            this.Controls.Add(btnClose);

            // Ambil data dari DAO
            TransaksiDAO dao = new TransaksiDAO();
            DataTable header = dao.AmbilHeaderTransaksi(idTransaksi);
            DataTable detailTransaksi = dao.AmbilDetailTransaksi(idTransaksi);

            if (header.Rows.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("========== STRUK PEMBAYARAN SEWA ALAT CAMPING ==========");
                sb.AppendLine($"ID Transaksi : {header.Rows[0]["ID Transaksi"]}");
                sb.AppendLine($"Nama         : {header.Rows[0]["NamaPelanggan"]}");
                sb.AppendLine($"Tanggal      : {Convert.ToDateTime(header.Rows[0]["Tanggal"]).ToString("dd/MM/yyyy")}");
                sb.AppendLine("--------------------------------------------------------");
                sb.AppendLine("Barang           Qty  Lama  Subtotal");

                decimal total = 0;
                foreach (DataRow row in detailTransaksi.Rows)
                {
                    string nama = row["Nama_Alat"].ToString();
                    int qty = Convert.ToInt32(row["Jumlah"]);
                    int hari = Convert.ToInt32(row["Durasi (Hari)"]);
                    decimal harga = Convert.ToDecimal(row["Total_Harga"]);
                    decimal subtotal = qty * hari * harga;

                    sb.AppendLine($"{nama,-15} {qty}x  {hari}h  Rp{subtotal:N0}");
                    total += subtotal;
                }

                sb.AppendLine("--------------------------------------------------------");
                sb.AppendLine($"TOTAL BAYAR: Rp {total:N0}");
                sb.AppendLine("========================================================");

                strukBox.Text = sb.ToString();
            }
            else
            {
                strukBox.Text = "Data transaksi tidak ditemukan.";
            }
        }

        private void FormStruk_Load(object sender, EventArgs e)
        {

        }
    }
}
