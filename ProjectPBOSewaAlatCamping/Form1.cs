using Microsoft.VisualBasic.ApplicationServices;
using Npgsql;
using ProjectPBOSewaAlatCamping.dataAccess;
using ProjectPBOSewaAlatCamping.Models;
using System.Text.RegularExpressions;


namespace ProjectPBOSewaAlatCamping
{
    public partial class Form1 : Form
    {

        private readonly DataAccess dataAccess;


        public Form1()
        {
            InitializeComponent();
            dataAccess = new DataAccess();

            textBox1Emailreg.KeyPress += EmailTextBox_KeyPress;

            

            textBox1Loguser.KeyPress += AlphaNumericOnly_KeyPress;
            textBox2Passuser.UseSystemPasswordChar = true;
            textBox4Pass.UseSystemPasswordChar = true;
            textBox2Passuser.KeyPress += PasswordTextBox_KeyPress;
            textBox3Reg.KeyPress += AlphaNumericOnly_KeyPress;     
            textBox4Pass.KeyPress += AlphaNumericOnly_KeyPress;    
            textBox1NoTelp.KeyPress += NomorSaja_KeyPress;

            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            checkBox2Show.CheckedChanged += checkBox2Show_CheckedChanged;
        }

        private void AlphaNumericOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            
            if (!char.IsControl(ch) && !char.IsLetterOrDigit(ch))
            {
                e.Handled = true;
            }
        }

        private void NomorSaja_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        private void PasswordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            
            if (!char.IsControl(ch) && !char.IsLetterOrDigit(ch))
            {
                e.Handled = true; 
            }
        }

        private void EmailTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            
            if (!char.IsControl(ch) &&
                !char.IsLetterOrDigit(ch) &&
                ch != '@' && ch != '.' && ch != '_' && ch != '-')
            {
                e.Handled = true; 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string usernameOrEmail = textBox1Loguser.Text.Trim();
            string password = textBox2Passuser.Text;


            if (!Regex.IsMatch(usernameOrEmail, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Username/Email hanya boleh huruf dan angka tanpa spasi atau simbol.");
                return;
            }

            user user = dataAccess.LoginUserReturnUser(usernameOrEmail, password);

            if (user != null)
            {
                MessageBox.Show("Login berhasil!");

                if (user.Role == "admin")
                {
                    var berandaadmin = new Berandaadmin();
                    berandaadmin.Show();
                }
                else if (user.Role == "pelanggan")
                {
                    var beranda = new BerandaPelanggan(user.Nama, user.Email);
                    beranda.Show();
                }

                this.Hide();
            }
            else
            {
                MessageBox.Show("Username/Email atau Password salah.");
            }


        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            string username = textBox3Reg.Text.Trim();
            string email = textBox1Emailreg.Text.Trim();
            string password = textBox4Pass.Text;
            string Notelp = textBox1NoTelp.Text.Trim();

            
            if (username.Length < 3)
            {
                MessageBox.Show("Username minimal 3 karakter.");
                return;
            }

            
            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9]+(?: [a-zA-Z0-9]+)*$"))
            {
                MessageBox.Show("Username hanya boleh huruf, angka, dan spasi tunggal (tanpa karakter khusus atau spasi berlebih).");
                return;
            }

          
            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9]+@gmail\.com$"))
            {
                MessageBox.Show("Format email hanya boleh huruf/angka dan domain harus @gmail.com");
                return;
            }

         
            if (password.Length < 6)
            {
                MessageBox.Show("Password minimal 6 karakter.");
                return;
            }

            
            if (!Regex.IsMatch(Notelp, @"^[0-9]{10,13}$"))
            {
                MessageBox.Show("No Telp hanya boleh angka dan panjang 10–13 digit.");
                return;
            }

            bool success = dataAccess.RegisterUser(username, email, password, Notelp, "pelanggan");

            MessageBox.Show(success ? "Registrasi berhasil!" : "Username atau Email sudah terdaftar.");
        }
        private void textBox1Loguser_TextChanged(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2Passuser.UseSystemPasswordChar = !checkBox1.Checked;

        }

        private void checkBox2Show_CheckedChanged(object sender, EventArgs e)
        {
            textBox4Pass.UseSystemPasswordChar= !checkBox2Show.Checked;
        }
    }
}



