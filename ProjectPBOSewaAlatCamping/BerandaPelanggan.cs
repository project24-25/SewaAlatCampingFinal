using Npgsql;
using ProjectPBOSewaAlatCamping.dataAccess;
using ProjectPBOSewaAlatCamping.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;


namespace ProjectPBOSewaAlatCamping
{
    public partial class BerandaPelanggan : Form
    {
        private Label labelTotal;
        private decimal totalHarga = 0;
        private ListBox listBoxTransaksi;
        private FlowLayoutPanel flowLayoutPanelUtama;
        private TextBox textBoxNama;
        private TextBox textBoxEmail;
        private readonly string namaLogin;
        private readonly string emailLogin;

        private List<DetailTransaksi> transaksiList = new List<DetailTransaksi>();
        private TransaksiDAO transaksiDAO = new TransaksiDAO();
        private DatabaseAlat dbAlat = new DatabaseAlat();



        public BerandaPelanggan(string nama, string email)
        {
            InitializeComponent();
            namaLogin = nama;
            emailLogin = email;
            InitializeUIComponents();
            LoadEquipment();
            this.Resize += (s, e) => 
            ResizeLayout();
            CekTransaksiBerakhirHariIni();

        }
        private void InitializeUIComponents()
        {
            dbAlat = new DatabaseAlat();

            this.Text = "Beranda Pelanggan - SEWA ALAT CAMPING";
            this.Size = new Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;


            Label labelJudul = new Label
            {
                Text = "Daftar Alat Camping",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            Label labelNama = new Label
            {
                Text = "Nama Pelanggan:",
                Location = new Point(30, 70),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };

            textBoxNama = new TextBox
            {
                Name = "textBoxNama",
                Location = new Point(210, 68),
                Width = 300,
                Text = namaLogin,
                ReadOnly = true,
                Enabled = false
            };

            Label labelEmail = new Label
            {
                Text = "Email Pelanggan:",
                Location = new Point(30, 100),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };

            textBoxEmail = new TextBox
            {
                Name = "textBoxEmail",
                Location = new Point(210, 98),
                Width = 300,
                Text = emailLogin,
                ReadOnly = true,
                Enabled = false

            };


            flowLayoutPanelUtama = new FlowLayoutPanel
            {
                AutoScroll = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };



            listBoxTransaksi = new ListBox
            {
                Font = new Font("Arial", 10),
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = SelectionMode.One
            };
            listBoxTransaksi.DoubleClick += (s, e) => HapusItemTransaksi();



            labelTotal = new Label
            {
                Text = "Total: Rp 0",
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };



            Button buttonSelesai = new Button
            {
                Name = "buttonSelesai",
                Text = "Selesai Transaksi",
                Font = new Font("Arial", 10),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            buttonSelesai.Click += buttonSelesai_Click;

            Button buttonRefresh = new Button
            {
                Name = "buttonRefresh",
                Text = "Refresh",
                Font = new Font("Arial", 10),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };
            buttonRefresh.Click += (s, e) => LoadEquipment();

            Button buttonHapus = new Button
            {
                Name = "buttonHapus",
                Text = "Hapus Item",
                Font = new Font("Arial", 10),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat
            };
            buttonHapus.Click += (s, e) => HapusItemTransaksi();

            Button buttonLogout = new Button
            {
                Name = "buttonLogout",
                Text = "Logout",
                Font = new Font("Arial", 9),
                Location = new Point(10, 625),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat
            };
            buttonLogout.Click += (s, e) => Application.Exit();

            Button btnRiwayat = new()
            {
                Name = "buttonRiwayat",
                Text = "Lihat Riwayat",
                Location = new Point(20, 625),
                BackColor = Color.LightSteelBlue,
                Width = 200,
                Visible = false
            };
            btnRiwayat.Click += (s, e) =>
            {
                var riwayatForm = new RiwayatTransaksiPelanggan(textBoxNama.Text.Trim(), textBoxEmail.Text.Trim());
                riwayatForm.ShowDialog();
            };

            this.Controls.Add(labelJudul);
            this.Controls.Add(labelNama);
            this.Controls.Add(textBoxNama);
            this.Controls.Add(labelEmail);
            this.Controls.Add(textBoxEmail);
            this.Controls.Add(flowLayoutPanelUtama);
            this.Controls.Add(listBoxTransaksi);
            this.Controls.Add(labelTotal);
            this.Controls.Add(buttonSelesai);
            this.Controls.Add(buttonRefresh);
            this.Controls.Add(buttonHapus);
            this.Controls.Add(buttonLogout);
            this.Controls.Add(btnRiwayat);





            ResizeLayout();
            CekDanTampilkanRiwayat(null, null);
        }

        private void CekDanTampilkanRiwayat(object sender, EventArgs e)
        {
            Button btnRiwayat = this.Controls["buttonRiwayat"] as Button;
            string nama = textBoxNama.Text.Trim();
            string email = textBoxEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(email))
            {
                btnRiwayat.Visible = false;
                return;
            }

            bool adaRiwayat = CekAdaRiwayatTransaksi(nama, email);

            btnRiwayat.Visible = true;
            btnRiwayat.Enabled = adaRiwayat;
            btnRiwayat.Text = adaRiwayat ? "Lihat Riwayat Transaksi" : "Belum Ada Riwayat";
            btnRiwayat.BackColor = adaRiwayat ? Color.LightSteelBlue : Color.LightGray;
        }

        private bool CekAdaRiwayatTransaksi(string nama, string email)
        {
            var dt = transaksiDAO.AmbilTransaksiByNamaEmail(nama, email);
            return dt != null && dt.Rows.Count > 0;
        }



        private void ResizeLayout()
        {
            int margin = 30;
            int sidePanelWidth = 360;
            int height = this.ClientSize.Height;
            int width = this.ClientSize.Width;

            flowLayoutPanelUtama.Location = new Point(margin, 120);
            flowLayoutPanelUtama.Size = new Size(width - sidePanelWidth - 3 * margin, height - 150);

            listBoxTransaksi.Location = new Point(width - sidePanelWidth, 120);
            listBoxTransaksi.Size = new Size(sidePanelWidth - margin, height - 250);

            labelTotal.Location = new Point(width - sidePanelWidth, height - 120);

            Control buttonSelesai = this.Controls["buttonSelesai"];
            Control buttonRefresh = this.Controls["buttonRefresh"];
            Control buttonHapus = this.Controls["buttonHapus"];
            Control btnLogout = this.Controls["buttonLogout"];
            Control btnRiwayat = this.Controls["buttonRiwayat"];

            buttonSelesai.Location = new Point(width - sidePanelWidth, height - 90);
            buttonRefresh.Location = new Point(width - sidePanelWidth + 110, height - 90);
            buttonHapus.Location = new Point(width - sidePanelWidth + 220, height - 90);
            btnLogout.Location = new Point(width - sidePanelWidth, height - 50);
            btnRiwayat.Location = new Point(width - sidePanelWidth + 110, height - 50);
    

        }

        private void LoadEquipment()
        {
            flowLayoutPanelUtama.Controls.Clear();
            var listAlat = dbAlat.AmbilSemuaAlatCamping();

            foreach (var alat in listAlat)
            {
                var panel = new Panel
                {
                    Size = new Size(200, 300), 
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.WhiteSmoke,
                    Margin = new Padding(10)
                };

                var pictureBox = new PictureBox
                {
                    Size = new Size(180, 100),
                    Location = new Point(10, 10),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = ConvertBytesToImage(alat.Foto)
                };

                var labelNama = new Label
                {
                    Text = alat.Nama,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(10, 115),
                    Size = new Size(180, 20)
                };

                var labelHarga = new Label
                {
                    Text = $"Harga: Rp {alat.Harga:N0}",
                    Location = new Point(10, 140),
                    Size = new Size(180, 20),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Arial", 9)
                };

                var labelStok = new Label
                {
                    Text = $"Stock: {alat.stock}",
                    Location = new Point(10, 160),
                    Size = new Size(180, 20),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Arial", 9),
                    ForeColor = alat.stock == 0 ? Color.Red : Color.Black
                };

                var numericJumlah = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = Math.Max(1, alat.stock),
                    Value = 1,
                    Location = new Point(10, 185),
                    Width = 60,
                    Enabled = alat.stock > 0,
                    ReadOnly = true,
                    Increment = 1,
                    TextAlign = HorizontalAlignment.Center
                };

                var numericDurasi = new NumericUpDown
                {
                    Minimum = 1,
                    Maximum = 30,
                    Value = 1,
                    Location = new Point(10, 225),
                    Width = 60,
                    ReadOnly = true, 
                    Increment = 1,
                    TextAlign = HorizontalAlignment.Center
                };

                var labelHari = new Label
                {
                    Text = "/hari",
                    Location = new Point(numericDurasi.Right + 5, numericDurasi.Top + 3),
                    AutoSize = true
                };

                var buttonSewa = new Button
                {
                    Text = "Sewa",
                    Location = new Point(90, 185),
                    Size = new Size(90, 30),
                    BackColor = alat.stock > 0 ? Color.Orange : Color.LightGray,
                    Enabled = alat.stock > 0,
                    Font = new Font("Arial", 9)
                };

                int id = alat.Id;
                string nama = alat.Nama;
                decimal harga = alat.Harga;

                buttonSewa.Click += (s, e) =>
                {
                    int durasi = (int)numericDurasi.Value;
                    int jumlah = (int)numericJumlah.Value;
                    if (jumlah > 0 && jumlah <= alat.stock)
                    {
                        AddToTransaction(id, nama, harga, jumlah, durasi);
                    }
                    else
                    {
                        MessageBox.Show("Jumlah tidak valid atau melebihi stok tersedia.", "Peringatan",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };

               
                panel.Controls.Add(pictureBox);
                panel.Controls.Add(labelNama);
                panel.Controls.Add(labelHarga);
                panel.Controls.Add(labelStok);
                panel.Controls.Add(numericJumlah);
                panel.Controls.Add(numericDurasi);
                panel.Controls.Add(labelHari);
                panel.Controls.Add(buttonSewa);

               
                flowLayoutPanelUtama.Controls.Add(panel);
            }
        }



        private Image ConvertBytesToImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return CreatePlaceholderImage(); 

            using (var ms = new System.IO.MemoryStream(imageBytes))
            {
                return Image.FromStream(ms);
            }
        }

        private Image CreatePlaceholderImage()
        {
            Bitmap bmp = new Bitmap(80, 80);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                g.DrawRectangle(Pens.DarkGray, 0, 0, bmp.Width - 1, bmp.Height - 1);
                using Font font = new Font("Arial", 8);
                SizeF textSize = g.MeasureString("No Image", font);
                g.DrawString("No Image", font, Brushes.Black,
                    (bmp.Width - textSize.Width) / 2, (bmp.Height - textSize.Height) / 2);
            }
            return bmp;
        }


        private void AddToTransaction(int id, string name, decimal price, int jumlah,int durasi)
        {
            var alat = dbAlat.AmbilAlatById(id);
            if (alat == null || alat.stock < jumlah)
            {
                MessageBox.Show("Stok alat tidak mencukupi!", "Peringatan",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existing = transaksiList.FirstOrDefault(t => t.AlatId == id);
            if (existing != null)
            {
                transaksiList.Remove(existing);
                existing.Jumlah += jumlah;
                existing.DurasiSewa = durasi;
                transaksiList.Add(existing);
            }
            else
            {
                transaksiList.Add(new DetailTransaksi
                {
                    AlatId = id,
                    NamaAlat = name,
                    HargaSatuan = price,
                    Jumlah = jumlah,
                    DurasiSewa = durasi 
                });
            }
            if (durasi > 7)
            {
                MessageBox.Show("Penyewaan lebih dari 7 hari akan dikenai tambahan deposit.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            RefreshListBox();
            UpdateTotalLabel();

            dbAlat.PerbaruiStokAlat(id, -jumlah);
            LoadEquipment();
        }






        private void HapusItemTransaksi()
        {
            if (listBoxTransaksi.SelectedItem == null)
            {
                MessageBox.Show("Pilih item yang ingin dihapus dari keranjang.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedStr = listBoxTransaksi.SelectedItem.ToString();
            var match = Regex.Match(selectedStr, @"ID:(\d+)");
            if (match.Success)
            {
                int id = int.Parse(match.Groups[1].Value);
                var item = transaksiList.FirstOrDefault(t => t.AlatId == id);
                if (item != null)
                {
                    dbAlat.PerbaruiStokAlat(id, item.Jumlah);
                    transaksiList.Remove(item);
                    RefreshListBox();
                    UpdateTotalLabel();
                    LoadEquipment();
                }
                else
                {
                    MessageBox.Show("Item tidak ditemukan dalam transaksi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Gagal menemukan ID item yang dipilih.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotalLabel()
        {
            totalHarga = transaksiList.Sum(t => t.HargaSatuan * t.Jumlah * t.DurasiSewa);
            labelTotal.Text = $"Total: Rp {totalHarga:N0}";
        }

        private void RefreshListBox()
        {
            listBoxTransaksi.Items.Clear();
            foreach (var t in transaksiList)
            {
                decimal subTotal = t.HargaSatuan * t.Jumlah * t.DurasiSewa;
                string display = $"ID:{t.AlatId} | {t.NamaAlat} (x{t.Jumlah} - {t.DurasiSewa} hari)\nSubtotal: Rp {subTotal:N0}";
                listBoxTransaksi.Items.Add(display);
            }
        }
        private void buttonSelesai_Click(object sender, EventArgs e)
        {
            if (transaksiList.Count == 0)
            {
                MessageBox.Show("Tidak ada alat yang dipilih untuk disewa!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string namaPelanggan = textBoxNama.Text.Trim();
            string emailPelanggan = textBoxEmail.Text.Trim();

            if (string.IsNullOrEmpty(namaPelanggan) || namaPelanggan.Length < 3)
            {
                MessageBox.Show("Silakan isi nama pelanggan minimal 3 karakter!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(emailPelanggan) || !emailPelanggan.Contains("@"))
            {
                MessageBox.Show("Silakan isi email pelanggan yang valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var userDAO = new DataAccess(); 
            bool userValid = userDAO.CekUserTerdaftar(namaPelanggan, emailPelanggan);

            if (!userValid)
            {
                MessageBox.Show("Nama dan email tidak ditemukan dalam data pengguna yang terdaftar!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            var formPembayaran = new FormMetodePembayaran(totalHarga, namaPelanggan, transaksiList);
            var result = formPembayaran.ShowDialog();

            if (result == DialogResult.OK && formPembayaran.InfoPembayaran.Berhasil)
            {
                var transaksi = new Transaksi
                {
                    Tanggal = DateTime.Now,
                    TotalHarga = totalHarga,
                    NamaPelanggan = namaPelanggan,
                    MetodePembayaran = formPembayaran.InfoPembayaran.Metode,
                    BuktiPembayaran = formPembayaran.InfoPembayaran.BuktiTransfer,
                    Status = "Menunggu Konfirmasi",
                    DetailItems = transaksiList,
                    TanggalAkhirSewa = DateTime.Now.AddDays(transaksiList.Max(t => t.DurasiSewa)) 
                };

                bool simpan = transaksiDAO.SimpanTransaksi(transaksi);
                if (simpan)
                {
                    DateTime tanggalTerakhirSewa = DateTime.Now.AddDays(transaksiList.Max(t => t.DurasiSewa));

                   
                    MessageBox.Show(
                        $"Halo {namaPelanggan},\nMasa sewa Anda akan berakhir pada:\n{tanggalTerakhirSewa:dddd, dd MMMM yyyy}.",
                        "Notifikasi Masa Sewa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    MessageBox.Show("Transaksi berhasil disimpan. Menunggu verifikasi admin.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    transaksiList.Clear();
                    RefreshListBox();
                    UpdateTotalLabel();
                    totalHarga = 0;
                    labelTotal.Text = "Total: Rp 0";

                    

                    LoadEquipment(); 
                }
                else
                {
                    MessageBox.Show("Gagal menyimpan transaksi!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (CekAdaRiwayatTransaksi(namaPelanggan, emailPelanggan))
                {
                    Control btnRiwayat = this.Controls["buttonRiwayat"];
                    btnRiwayat.Visible = true;
                }

                
            }


        }
        private void CekTransaksiBerakhirHariIni()
        {
            var dt = transaksiDAO.AmbilTransaksiByNamaEmail(namaLogin, emailLogin);
            if (dt == null || dt.Rows.Count == 0) return;

            foreach (DataRow row in dt.Rows)
            {
                if (dt.Columns.Contains("Tanggal Akhir Sewa") &&
                    DateTime.TryParse(row["Tanggal Akhir Sewa"]?.ToString(), out DateTime tanggalAkhir))
                {
                    if (tanggalAkhir.Date == DateTime.Today)
                    {
                        MessageBox.Show(
                            $"Hai {namaLogin},\nSalah satu sewa kamu akan *berakhir hari ini* ({tanggalAkhir:dd MMMM yyyy}).\nSegera kembalikan alat !!!.",
                            "⚠️ Pengembalian Hari Ini",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        break; 
                    }
                }
            }
        }

    }
}
