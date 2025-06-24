using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using ProjectPBOSewaAlatCamping.dataAccess; 

namespace ProjectPBOSewaAlatCamping
{
    public partial class Berandaadmin : Form
    {

        private DatabaseAlat dbAlat = new DatabaseAlat();
        private int stokSaatIni;
        private int idAlat;
        private FlowLayoutPanel flowLayoutPanelAlat;


        public Berandaadmin()
        {
            InitializeComponent();
            InitializeFlowLayoutPanel();
            LoadAlatDariDatabase();
        }



        private void InitializeFlowLayoutPanel()
        {
            flowLayoutPanelAlat = new FlowLayoutPanel
            {
                Location = new Point(50, 100),
                Size = new Size(980, 400),
                AutoScroll = true
            };

            flowLayoutPanel2.Controls.Add(flowLayoutPanelAlat);
        }


        private void RefreshStok()
        {
            labelStok.Text = stokSaatIni.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }



        private void buttonTambahStok_Click(object sender, EventArgs e)
        {
            if (idAlat > 0 && dbAlat.PerbaruiStokAlat(idAlat, 1))
                if (idAlat > 0 && dbAlat.PerbaruiStokAlat(idAlat, 1))
                {
                    stokSaatIni++;
                    RefreshStok();
                    LoadAlatDariDatabase();
                }


        }

        private void buttonKurangiStok_Click(object sender, EventArgs e)
        {
            if (stokSaatIni > 0 && idAlat > 0 && dbAlat.PerbaruiStokAlat(idAlat, -1))
            {
                stokSaatIni--;
                RefreshStok();
                LoadAlatDariDatabase();
            }
            else
            {
                MessageBox.Show("Stok tidak bisa negatif!", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void labelStok_Click(object sender, EventArgs e)
        {

        }




        private void buttonTambahAlat_Click(object sender, EventArgs e)
        {
            FormTambahAlat formTambahAlat = new FormTambahAlat();
            formTambahAlat.FormClosed += (s, args) => LoadAlatDariDatabase();
            formTambahAlat.Show();
        }





        private void LoadAlatDariDatabase()
        {
            try
            {
                DataTable dataAlat = dbAlat.AmbilSemuaAlat();

                if (dataAlat.Rows.Count == 0)
                {
                    MessageBox.Show("Tidak ada data alat!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                flowLayoutPanelAlat.Controls.Clear();

                foreach (DataRow row in dataAlat.Rows)
                {
                    Panel panelAlat = new Panel
                    {
                        Width = 180,
                        Height = 250,
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(15)
                    };

                    Label labelNama = new Label
                    {
                        Text = row["nama"].ToString(),
                        AutoSize = true,
                        Location = new Point(10, 10),
                        Font = new Font("Arial", 12, FontStyle.Bold)
                    };

                    Label labelHarga = new Label
                    {
                        Text = $"Harga: {Convert.ToInt32(row["harga"]).ToString("N0")}",
                        AutoSize = true,
                        Location = new Point(10, 40),
                        Font = new Font("Arial", 10, FontStyle.Regular)
                    };

                    Label labelStok = new Label
                    {
                        Text = $"Stock: {row["stock"].ToString()}",
                        AutoSize = true,
                        Location = new Point(10, 70),
                        ForeColor = Convert.ToInt32(row["stock"]) > 0 ? Color.Black : Color.Red
                    };

                    PictureBox pictureBoxAlat = new PictureBox
                    {
                        Size = new Size(160, 160),
                        Location = new Point(10, 100),
                        Image = ConvertBytesToImage(row["foto"] as byte[]),
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };

                    panelAlat.Controls.Add(labelNama);
                    panelAlat.Controls.Add(labelHarga);
                    panelAlat.Controls.Add(labelStok);
                    panelAlat.Controls.Add(pictureBoxAlat);


                    flowLayoutPanelAlat.Controls.Add(panelAlat);
                }


                flowLayoutPanelAlat.Visible = true;

                MessageBox.Show("Jumlah alat yang berhasil dimuat: " + flowLayoutPanelAlat.Controls.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error memuat data alat: {ex.Message}", "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private Image ConvertBytesToImage(byte[] imageBytes)
        {
            try
            {

                if (imageBytes == null || imageBytes.Length == 0)
                {
                    return GetDefaultImage();
                }

                using MemoryStream ms = new MemoryStream(imageBytes);
                return Image.FromStream(ms);
            }
            catch
            {
                return GetDefaultImage();
            }
        }

        private Image GetDefaultImage()
        {
            try
            {
                return Properties.Resources.default_image;
            }
            catch
            {
                return CreatePlaceholderImage();
            }
        }

        private Image CreatePlaceholderImage()
        {
            Bitmap bmp = new Bitmap(160, 160);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.DrawRectangle(Pens.DarkGray, 0, 0, bmp.Width - 1, bmp.Height - 1);
                using Font font = new Font("Arial", 12, FontStyle.Bold);
                SizeF textSize = g.MeasureString("No Image", font);
                g.DrawString("No Image", font, Brushes.Black,
                    (bmp.Width - textSize.Width) / 2, (bmp.Height - textSize.Height) / 2);
            }
            return bmp;
        }

        private void buttonUpdateProduk_Click(object sender, EventArgs e)
        {
            FormUpdate formUpdate = new FormUpdate(idAlat);
            formUpdate.FormClosed += (s, args) => LoadAlatDariDatabase();
            formUpdate.ShowDialog();

        }

        private void buttonHapusProduk_Click(object sender, EventArgs e)
        {
            FormHapusAlat formHapusAlat = new FormHapusAlat();
            formHapusAlat.FormClosed += (s, args) => LoadAlatDariDatabase();
            formHapusAlat.ShowDialog();

        }


        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (var lihatDaftarSewa = new LihatDaftarSewa())
            {
                lihatDaftarSewa.ShowDialog();
            }
            this.Show();


        }



        private void buttonVerifikasiPembayaran_Click_1(object sender, EventArgs e)
        {
            VerivfikasiPembayaran formVerifikasi = new VerivfikasiPembayaran();
            formVerifikasi.ShowDialog();
        }

        private void buttonLihattransaksi_Click(object sender, EventArgs e)
        {
            AdminLihatTransaksi adminLihatTransaksi = new AdminLihatTransaksi();
            adminLihatTransaksi.ShowDialog();
        }
    }
}


