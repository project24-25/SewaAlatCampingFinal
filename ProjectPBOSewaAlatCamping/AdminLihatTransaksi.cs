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
    public partial class AdminLihatTransaksi : Form
    {
        private DataGridView dgvTransaksi;
        private TransaksiDAO transaksiDAO = new TransaksiDAO();
        public AdminLihatTransaksi()
        {
            InitializeComponent();
            InitializeUI();
            LoadTransaksi();
        }


        private void InitializeUI()
        {
            this.Text = "Data Transaksi Pelanggan";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            dgvTransaksi = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            this.Controls.Add(dgvTransaksi);
        }

        private void LoadTransaksi()
        {
            DataTable transaksiGabung = transaksiDAO.AmbilSemuaTransaksiGabungAlat();
            dgvTransaksi.DataSource = transaksiGabung;

            dgvTransaksi.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvTransaksi.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            if (dgvTransaksi.Columns.Contains("Total Harga"))
            {
                dgvTransaksi.Columns["Total Harga"].DefaultCellStyle.Format = "C0";
            }

            if (dgvTransaksi.Columns.Contains("Alat Disewa"))
            {
                dgvTransaksi.Columns["Alat Disewa"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvTransaksi.Columns["Alat Disewa"].ToolTipText = "Daftar alat yang disewa dalam transaksi ini.";
            }
        }
    }
}
