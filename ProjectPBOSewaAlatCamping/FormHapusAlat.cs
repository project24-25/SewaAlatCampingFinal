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

namespace ProjectPBOSewaAlatCamping
{
    public partial class FormHapusAlat : Form
    {
        private DatabaseAlat dbAlat = new DatabaseAlat();
        private int id;
        public FormHapusAlat()
        {
            InitializeComponent();
            LoadComboBoxAlat();

            
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadComboBoxAlat()
        {
            try
            {
                DataTable dataAlat = dbAlat.AmbilSemuaAlat();

                comboBox1.DisplayMember = "nama";
                comboBox1.ValueMember = "nama";
                comboBox1.DataSource = dataAlat;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat daftar alat: {ex.Message}", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null && int.TryParse(comboBox1.SelectedValue.ToString(), out int selectedId))
            {
                id = selectedId;
            }
        }

        private void buttonSelesai_Click(object sender, EventArgs e)
        {
            string namaInput = textBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(namaInput))
            {
                MessageBox.Show("Silakan ketik nama alat yang ingin dihapus terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (string.IsNullOrWhiteSpace(namaInput))
            {
                MessageBox.Show("Nama alat tidak boleh kosong atau hanya spasi!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (namaInput.Contains("."))
            {
                MessageBox.Show("Nama alat tidak boleh mengandung titik (.)!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           
            if (!namaInput.Any(char.IsDigit))
            {
                MessageBox.Show("Nama alat harus mengandung minimal satu angka!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var semuaAlat = dbAlat.AmbilSemuaAlat();

                var barisAlat = semuaAlat.AsEnumerable().FirstOrDefault(row =>
                    row["nama"].ToString() == namaInput);

                if (barisAlat == null)
                {
                    MessageBox.Show("Nama alat tidak ditemukan. Pastikan penulisan nama alat sesuai, termasuk huruf besar/kecil!",
                        "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool berhasil = dbAlat.HapusAlatByNama(namaInput);

                if (berhasil)
                {
                    MessageBox.Show("Alat berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadComboBoxAlat(); 
                    textBox1.Clear();
                }
                else
                {
                    MessageBox.Show("Gagal menghapus alat.", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan saat menghapus: {ex.Message}", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            var berandaadmin = new Berandaadmin();
            berandaadmin.Show();
        }
    }
}
