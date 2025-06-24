using Npgsql;
using ProjectPBOSewaAlatCamping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjectPBOSewaAlatCamping.dataAccess
{
    public class DataAccess
    {
        private readonly string connectionString = "Host=localhost;Port=5432;Database=SEWAALATCAMPING;Username=postgres;Password=bayuaji";

        public bool RegisterUser(string username,string email, string password, string Notelp, string role )
            {
            if (!Regex.IsMatch(username, @"^[a-zA-Z]+$"))
            {
                return false;
            }

            // Validasi email sederhana
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return false;
            }

            // Validasi password minimal 6 karakter
            if (password.Length < 6)
            {
                return false;
            }

            // Validasi nomor telepon hanya angka dan panjang 10–13 digit
            if (!Regex.IsMatch(Notelp, @"^[0-9]{10,13}$"))
            {
                return false;
            }
            using var conn = new Npgsql.NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new Npgsql.NpgsqlCommand("INSERT INTO users (username, email, password, role, no_telp) VALUES (@u, @e, @p, @r, @n)", conn);
            cmd.Parameters.AddWithValue("u", username);
            cmd.Parameters.AddWithValue("e", email);
            cmd.Parameters.AddWithValue("p", password);
            cmd.Parameters.AddWithValue("n", Notelp);
            cmd.Parameters.AddWithValue("r", role);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Npgsql.PostgresException)
            {
                return false;
            }
        }

        public string LoginUser(string usernameOrEmail, string password)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT role FROM users WHERE (username = @u OR email = @u) AND password = @p", conn);
            cmd.Parameters.AddWithValue("u", usernameOrEmail);
            cmd.Parameters.AddWithValue("p", password);

            var role = cmd.ExecuteScalar()?.ToString();
            return role;
        }

        public bool CekUserTerdaftar(string nama, string email)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT COUNT(*) FROM users WHERE username = @nama AND email = @email";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nama", nama);
            cmd.Parameters.AddWithValue("@email", email);

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }

        public user LoginUserReturnUser(string usernameOrEmail, string password)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            SELECT username, email, role 
            FROM users 
            WHERE 
                (username = @userOrEmail OR email = @userOrEmail) 
                AND password = @password
            LIMIT 1;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userOrEmail", usernameOrEmail);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new user
                            {
                                Nama = reader.IsDBNull(0) ? "" : reader.GetString(0),
                                Email = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Role = reader.IsDBNull(2) ? "pelanggan" : reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

    }
}
