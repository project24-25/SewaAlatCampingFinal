using Npgsql;
using ProjectPBOSewaAlatCamping.dataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectPBOSewaAlatCamping
{
    public partial class FormTambahAlat : Form
    {
        private ProjectPBOSewaAlatCamping.dataAccess.DatabaseAlat DatabaseAlat = new ProjectPBOSewaAlatCamping.dataAccess.DatabaseAlat();
        private string imagePath = "";
        
        public FormTambahAlat()
        {
            InitializeComponent();
            
            pictureBoxAlat.SizeMode = PictureBoxSizeMode.StretchImage;
            textBoxNamaAlat.KeyPress += textBoxNamaAlat_KeyPress;
            textBoxHargaAlat.KeyPress += textBoxHargaAlat_KeyPress;

        }

        private void buttonPilihGambar_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG Files|*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = openFileDialog.FileName;
                pictureBoxAlat.Image = Image.FromFile(imagePath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); 

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string nama = textBoxNamaAlat.Text.Trim();
                string hargaText = textBoxHargaAlat.Text.Trim();

                if (string.IsNullOrWhiteSpace(nama))
                {
                    MessageBox.Show("Nama alat tidak boleh kosong.");
                    return;
                }

                if (!Regex.IsMatch(nama, @"^[a-zA-Z0-9 ]+$"))
                {
                    MessageBox.Show("Nama alat hanya boleh huruf, angka, dan spasi. Tidak boleh menggunakan simbol.");
                    return;
                }

                if (DatabaseAlat.CekNamaAlatSudahAda(nama))
                {
                    MessageBox.Show("Nama alat sudah ada dalam database. Gunakan nama lain.");
                    return;
                }

                if (!decimal.TryParse(hargaText, out decimal harga) || harga <= 0)
                {
                    MessageBox.Show("Harga harus berupa angka positif.");
                    return;
                }

                DatabaseAlat.TambahAlat(nama, harga, imagePath);
                MessageBox.Show("Data berhasil disimpan!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan: {ex.Message}");
            }
        }

        private void textBoxNamaAlat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Hanya huruf, angka, dan spasi yang diperbolehkan untuk nama alat.", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

      
        private void textBoxHargaAlat_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox txt = sender as TextBox;

           
            if (char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (e.KeyChar == '.')
            {
                if (txt.Text.Contains(".") || txt.SelectionStart == 0)
                {
                    e.Handled = true;
                    MessageBox.Show("Hanya satu titik diperbolehkan, dan tidak boleh di awal!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("Hanya angka dan titik desimal yang diperbolehkan!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private void FormTambahAlat_Load(object sender, EventArgs e)
        {

        }
    }
}


