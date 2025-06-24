using Npgsql;
using ProjectPBOSewaAlatCamping.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectPBOSewaAlatCamping.dataAccess
{
    public class TransaksiDAO
    {
        private readonly string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=bayuaji;Database=SEWAALATCAMPING";
        public DataTable AmbilDaftarTransaksiDenganBukti()
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
      WITH ranked_transaksi AS (
    SELECT 
        t.id,
        t.tanggal,
        t.nama_pelanggan,
        t.metodepembayaran,
        t.status,
        t.bukti_transfer,
        ROW_NUMBER() OVER (PARTITION BY t.nama_pelanggan ORDER BY t.tanggal DESC) AS rn
    FROM transaksi t
     )
SELECT 
    r.id AS ""ID Transaksi"",
    r.tanggal AS ""Tanggal"",
    r.nama_pelanggan AS ""Nama"",
    r.metodepembayaran AS ""Metode"",
    r.status AS ""Status"",
    r.bukti_transfer AS ""BuktiPembayaran"",
    STRING_AGG(a.nama || ' x' || dt.jumlah || ' (' || dt.durasisewa || ' hari)', ', ') AS ""Detail Alat"",
    SUM(dt.jumlah * dt.durasisewa * dt.harga_satuan) AS ""Total Harga""
    FROM ranked_transaksi r
    JOIN detail_transaksi dt ON dt.id_transaksi = r.id
    JOIN alat a ON a.id = dt.id_alat
    WHERE r.rn = 1
    GROUP BY r.id, r.tanggal, r.nama_pelanggan, r.metodepembayaran, r.status, r.bukti_transfer
    ORDER BY r.tanggal DESC;

    ";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(reader);

            return dt;
        }

        public DataTable AmbilHeaderTransaksi(int idTransaksi)
        {
            DataTable dt = new DataTable();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        id AS ""ID Transaksi"",
                        nama_pelanggan AS ""NamaPelanggan"",
                        tanggal AS ""Tanggal""
                    FROM transaksi
                    WHERE id = @idTransaksi
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idTransaksi", idTransaksi);
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public DataTable AmbilDetailTransaksi(int idTransaksi)
        {
            DataTable dt = new DataTable();
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                          SELECT 
    a.nama AS ""Nama Alat"",
    dt.jumlah AS ""Jumlah"",
    dt.durasisewa AS ""Durasi (Hari)"",
    dt.harga_satuan AS ""Harga Satuan"",
    (dt.jumlah * dt.durasisewa * dt.harga_satuan) AS ""Total_Harga""
FROM detail_transaksi dt
JOIN alat a ON a.id = dt.id_alat
WHERE dt.id_transaksi = @idTransaksi
                ";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@idTransaksi", idTransaksi);
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public bool SimpanTransaksi(Transaksi transaksi)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {

                        string queryTransaksi = @"INSERT INTO transaksi 
                        (tanggal, total_harga, nama_pelanggan, metodepembayaran, bukti_transfer, status, tanggal_akhir_sewa) 
                        VALUES 
                        (@tanggal, @total, @nama, @metode, @bukti, @status, @tglAkhir) 
                        RETURNING id;";

                        using var insertTransaksiCmd = new NpgsqlCommand(queryTransaksi, conn, transaction);
                        insertTransaksiCmd.Parameters.AddWithValue("@tanggal", transaksi.Tanggal);
                        insertTransaksiCmd.Parameters.AddWithValue("@total", transaksi.TotalHarga);
                        insertTransaksiCmd.Parameters.AddWithValue("@nama", transaksi.NamaPelanggan);
                        insertTransaksiCmd.Parameters.AddWithValue("@metode", transaksi.MetodePembayaran);
                        insertTransaksiCmd.Parameters.AddWithValue("@status", transaksi.Status ?? "Menunggu Konfirmasi");
                        insertTransaksiCmd.Parameters.AddWithValue("@tglAkhir", transaksi.TanggalAkhirSewa);



                        if (transaksi.BuktiPembayaran != null)
                            insertTransaksiCmd.Parameters.AddWithValue("@bukti", transaksi.BuktiPembayaran);
                        else
                            insertTransaksiCmd.Parameters.AddWithValue("@bukti", DBNull.Value);

                        int idTransaksi = Convert.ToInt32(insertTransaksiCmd.ExecuteScalar());

                        
                        foreach (var detail in transaksi.DetailItems)
                        {
                            using var insertDetailCmd = new NpgsqlCommand(@"
                        INSERT INTO detail_transaksi (id_transaksi, id_alat, jumlah, harga_satuan, durasisewa)
                        VALUES (@id_transaksi, @id_alat, @jumlah, @harga_satuan, @durasisewa)", conn, transaction);

                            insertDetailCmd.Parameters.AddWithValue("@id_transaksi", idTransaksi);
                            insertDetailCmd.Parameters.AddWithValue("@id_alat", detail.AlatId);
                            insertDetailCmd.Parameters.AddWithValue("@jumlah", detail.Jumlah);
                            insertDetailCmd.Parameters.AddWithValue("@harga_satuan", detail.HargaSatuan);
                            insertDetailCmd.Parameters.AddWithValue("@durasisewa", detail.DurasiSewa);

                            insertDetailCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error saat simpan transaksi: " + ex.Message);
                        try { transaction.Rollback(); } catch { }
                        return false;
                    }
                }
            }
        }

        public void UpdateStatusTransaksi(int idTransaksi, string statusBaru, string alasan)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"UPDATE transaksi SET status = @status WHERE id = @id";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", statusBaru);
            cmd.Parameters.AddWithValue("@id", idTransaksi);

            cmd.ExecuteNonQuery();

            if (!string.IsNullOrEmpty(alasan))
            {
                MessageBox.Show($"Transaksi #{idTransaksi} ditolak. Alasan: {alasan}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public DataTable AmbilSemuaTransaksiGabungAlat()
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
        SELECT 
            t.id AS ""ID Transaksi"",
            t.tanggal AS ""Tanggal"",
            t.nama_pelanggan AS ""Nama Pelanggan"",
            t.metodepembayaran AS ""Metode"",
            t.status AS ""Status"",
            t.total_harga AS ""Total Harga"",
            t.tanggal_akhir_sewa AS ""Tanggal Akhir Sewa"",
            STRING_AGG(a.nama || ' x' || dt.jumlah || ' (' || dt.durasisewa || ' hari)', ', ') AS ""Alat Disewa""
        FROM transaksi t
        JOIN detail_transaksi dt ON t.id = dt.id_transaksi
        JOIN alat a ON a.id = dt.id_alat
        GROUP BY 
            t.id, t.tanggal, t.nama_pelanggan, 
            t.metodepembayaran, t.status, t.total_harga, t.tanggal_akhir_sewa
        ORDER BY t.tanggal DESC;
    ";

            using var cmd = new NpgsqlCommand(query, conn);
            using var adapter = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }



        public DataTable AmbilTransaksiByNamaEmail(string nama, string email)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
        SELECT 
    t.id AS ""ID Transaksi"",
    t.tanggal AS ""Tanggal"",
    t.metodepembayaran AS ""Metode"",
    t.status AS ""Status"",
    SUM(dt.jumlah * dt.durasisewa * dt.harga_satuan) AS ""Total Harga"",
    t.tanggal_akhir_sewa AS ""Tanggal Akhir Sewa""
    FROM transaksi t
    JOIN detail_transaksi dt ON t.id = dt.id_transaksi
    WHERE t.nama_pelanggan = @nama
    GROUP BY t.id, t.tanggal, t.metodepembayaran, t.status, t.tanggal_akhir_sewa
    ORDER BY t.tanggal DESC;
    ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nama", nama);

            using var reader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);

            return dt;
        }


        public bool CekUserTerdaftar(string nama, string email)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = "SELECT COUNT(*) FROM \"users\" WHERE username = @nama AND email = @email";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nama", nama);
            cmd.Parameters.AddWithValue("@email", email);

            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }

        public bool PelangganPunyaTransaksi(string nama, string email)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            string query = @"
            SELECT COUNT(*) FROM transaksi
            WHERE nama_pelanggan = @nama AND email = @email
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nama", nama);
            cmd.Parameters.AddWithValue("@email", email);

            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }



    }


}
