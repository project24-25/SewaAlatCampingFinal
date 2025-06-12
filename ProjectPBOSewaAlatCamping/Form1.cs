using Npgsql;

namespace ProjectPBOSewaAlatCamping
{
    public partial class Form1 : Form
    {
        private readonly Database dbHelper = new Database();
        public Form1()
        {
            InitializeComponent();
        }
  
        private void button1_Click(object sender, EventArgs e)
        {
            string usernameOrEmail = textBox1Loguser.Text;
            string password = textBox2Passuser.Text;

            bool success = dbHelper.LoginUser(usernameOrEmail, password);

            MessageBox.Show(success ? "Login berhasil!" : "Username/Email atau Password salah.");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string username = textBox3Reg.Text;
            string email = textBox1Emailreg.Text; 
            string password = textBox4Pass.Text;

            bool success = dbHelper.RegisterUser(username, email, password);

            MessageBox.Show(success ? "Registrasi berhasil!" : "Username atau Email sudah terdaftar.");
        }
    }
}


public class Database
{
    private readonly string connectionString = "Host=localhost;Port=5432;Database=SEWAALATCAMPING;Username=postgres;Password=bayuaji";

    public bool RegisterUser(string username, string email, string password)
    {
        using var conn = new NpgsqlConnection(connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("INSERT INTO users (username, email, password) VALUES (@u, @e, @p)", conn);
        cmd.Parameters.AddWithValue("u", username);
        cmd.Parameters.AddWithValue("e", email);
        cmd.Parameters.AddWithValue("p", password); 
        try
        {
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (PostgresException)
        {
            return false; 
        }
    }


    public bool LoginUser(string usernameOrEmail, string password)
    {
        using var conn = new NpgsqlConnection(connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT COUNT (*) FROM users WHERE (username = @u OR email = @u) AND password = @p", conn);
        cmd.Parameters.AddWithValue("u", usernameOrEmail);
        cmd.Parameters.AddWithValue("p", password);

        var count = (long)cmd.ExecuteScalar();
        return count == 1;
    }
}



