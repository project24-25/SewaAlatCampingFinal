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
    public partial class FormUpdate : Form
    {

        private DatabaseAlat dbAlat = new DatabaseAlat();
        private int _currentId;
        private string imagePath = "";
        private byte[]? fotobyteLama = Array.Empty<byte>();


        public FormUpdate(int id)
        {
           InitializeComponent();
        _currentId = id;
        fotobyteLama = Array.Empty<byte>();

   
        comboBoxAlat.DropDownStyle = ComboBoxStyle.DropDownList;
        textBoxnamaup.KeyPress += textBoxnamaup_KeyPress;
        textBoxhargaUp.KeyPress += textBoxhargaUp_KeyPress;
        textBoxStock.KeyPress += textBoxStock_KeyPress;

            LoadComboBoxAlat();
            LoadDataAlat(id);
        }


        private void textBoxnamaup_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetterOrDigit(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Nama hanya boleh berisi huruf, angka, dan spasi!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBoxhargaUp_KeyPress(object sender, KeyPressEventArgs e)
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
                    MessageBox.Show("Titik hanya boleh satu dan tidak boleh di awal!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    e.Handled = false;
                }
            }
            else if (e.KeyChar == '-')
            {
                
                if (txt.SelectionStart != 0 || txt.Text.Contains("-"))
                {
                    e.Handled = true;
                    MessageBox.Show("Tanda minus hanya boleh di awal dan hanya satu!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("Harga hanya boleh angka, titik, atau tanda minus!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void textBoxStock_KeyPress(object sender, KeyPressEventArgs e)
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
            else if (e.KeyChar == '-')
            {
                if (txt.SelectionStart != 0 || txt.Text.Contains("-"))
                {
                    e.Handled = true;
                    MessageBox.Show("Tanda minus hanya boleh di awal dan hanya satu!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("Stok hanya boleh angka dan tanda minus!", "Input Tidak Valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadComboBoxAlat()
        {
            try
            {
                DataTable dataAlat = dbAlat.AmbilSemuaAlat();
                comboBoxAlat.DisplayMember = "nama";
                comboBoxAlat.ValueMember = "id";
                comboBoxAlat.DataSource = dataAlat;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat daftar alat: {ex.Message}", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void comboBoxAlat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAlat.SelectedValue != null && int.TryParse(comboBoxAlat.SelectedValue.ToString(), out int selectedId))
            {
                _currentId = selectedId;
                LoadDataAlat(_currentId);
            }
        }

        private void LoadDataAlat(int id)
        {
            try
            {
                Console.WriteLine($"DEBUG: Memeriksa alat dengan ID = {id}");

                if (id <= 0)
                {
                    MessageBox.Show("ID alat valid.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                AlatCamping? alat = dbAlat.AmbilAlatById(id); 

                if (alat == null)
                {
                    MessageBox.Show($"Data alat dengan ID {id} tidak ditemukan.", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Console.WriteLine($"ERROR: Data alat dengan ID {id} tidak ditemukan.");
                    return;
                }

                textBoxnamaup.Text = alat.Nama ?? "";
                textBoxhargaUp.Text = alat.Harga.ToString();
                textBoxStock.Text = alat.stock > 0 ? alat.stock.ToString() : "0";

                fotobyteLama = alat.Foto ?? Array.Empty<byte>();

                pictureBoxupgambar.Image = ConvertBytesToImage(fotobyteLama);
                imagePath = "";

                Console.WriteLine($"DEBUG: Data alat ditemukan! ID = {id}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data alat: {ex.Message}", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        private Image? ConvertBytesToImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0) return null;
            try
            {
                using MemoryStream ms = new MemoryStream(imageBytes);
                return Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kesalahan saat mengonversi gambar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private void buttoninsert_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Pilih Gambar"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imagePath = openFileDialog.FileName;
                pictureBoxupgambar.Image = Image.FromFile(imagePath);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBoxnamaup.Text) || string.IsNullOrEmpty(textBoxhargaUp.Text) || string.IsNullOrEmpty(textBoxStock.Text))
                {
                    MessageBox.Show("Harap isi nama, harga, dan stok baru!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                if (!decimal.TryParse(textBoxhargaUp.Text, out decimal perubahanHarga))
                {
                    MessageBox.Show("Harga harus berupa angka!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                
                AlatCamping? alatLama = dbAlat.AmbilAlatById(_currentId);
                if (alatLama == null)
                {
                    MessageBox.Show("Data alat tidak ditemukan!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal hargaFinal = alatLama.Harga + perubahanHarga;

                if (hargaFinal <= 0)
                {
                    MessageBox.Show("Harga akhir tidak boleh kurang dari atau sama dengan 0!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(textBoxStock.Text, out int stock))
        {
            MessageBox.Show("Stok harus berupa bilangan bulat!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

                string namaBaru = textBoxnamaup.Text;
                byte[]? fotoBytes = File.Exists(imagePath) ? File.ReadAllBytes(imagePath) : fotobyteLama ?? Array.Empty<byte>();

                Console.WriteLine($"Mengirim data update: ID={_currentId}, Nama={namaBaru}, Harga={hargaFinal}, Stok={stock}, Foto={(fotoBytes.Length > 0 ? "Ada Foto" : "Foto Kosong")}");

                bool berhasil = dbAlat.PerbaruiAlat(_currentId, namaBaru, hargaFinal, stock, fotoBytes);

                if (berhasil)
                {
                    MessageBox.Show("Data alat berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Gagal memperbarui data alat!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi error saat memperbarui data: {ex.Message}", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonclose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
