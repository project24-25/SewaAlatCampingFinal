using ProjectPBOSewaAlatCamping.dataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ProjectPBOSewaAlatCamping
{

    public partial class RiwayatTransaksiPelanggan : Form
    {

        private TransaksiDAO transaksiDAO = new TransaksiDAO();
        private string namaPelanggan;
        private string emailPelanggan;
        private DataGridView dgvRiwayat;
        public RiwayatTransaksiPelanggan(string nama, string email)
        {
            InitializeComponent();
            namaPelanggan = nama;
            emailPelanggan = email;
            InitializeUI();
            LoadRiwayat();
        }

        private void InitializeUI()
        {
            this.Text = "Riwayat Transaksi Saya";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvRiwayat = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            this.Controls.Add(dgvRiwayat);
        }

        private void LoadRiwayat()
        {
            if (string.IsNullOrWhiteSpace(namaPelanggan) || string.IsNullOrWhiteSpace(emailPelanggan))
            {
                MessageBox.Show("Nama dan email harus diisi untuk melihat riwayat transaksi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            if (!transaksiDAO.CekUserTerdaftar(namaPelanggan, emailPelanggan))
            {
                MessageBox.Show("Nama dan email tidak terdaftar sebagai pelanggan.", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            DataTable dt = transaksiDAO.AmbilTransaksiByNamaEmail(namaPelanggan, emailPelanggan);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Anda belum pernah melakukan transaksi penyewaan.\nRiwayat tidak tersedia.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            if (!dt.Columns.Contains("Status Sewa"))
                dt.Columns.Add("Status Sewa", typeof(string));

            DateTime hariIni = DateTime.Today;

            foreach (DataRow row in dt.Rows)
            {
                string statusSewa = "🔵 Tidak diketahui";

                if (dt.Columns.Contains("Tanggal Akhir Sewa") &&
                    DateTime.TryParse(row["Tanggal Akhir Sewa"]?.ToString(), out DateTime tanggalAkhir))
                {
                    if (tanggalAkhir.Date > DateTime.Today)
                    {
                        statusSewa = "🟢 Masih aktif";
                    }
                    else if (tanggalAkhir.Date == DateTime.Today)
                    {
                        statusSewa = "🟡 Berakhir hari ini";
                    }
                    else
                    {
                        statusSewa = "🔴 Sudah berakhir";
                    }
                }

                row["Status Sewa"] = statusSewa;
            }


            dgvRiwayat.DataSource = dt;

            foreach (DataGridViewRow row in dgvRiwayat.Rows)
            {
                string status = row.Cells["Status Sewa"]?.Value?.ToString()?.ToLower() ?? "";

                if (status.Contains("sudah berakhir"))
                {
                    row.DefaultCellStyle.BackColor = Color.LightSalmon;
                }
                else if (status.Contains("berakhir hari ini"))
                {
                    row.DefaultCellStyle.BackColor = Color.Khaki;
                }
                else if (status.Contains("aktif"))
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }
    }
}
