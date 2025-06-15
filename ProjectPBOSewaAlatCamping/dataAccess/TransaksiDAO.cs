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
      SELECT 
    t.id AS ""ID Transaksi"",
    t.tanggal AS ""Tanggal"",
    t.nama_pelanggan AS ""Nama"",
    t.metodepembayaran AS ""Metode"",
    t.status AS ""Status"",
    t.bukti_transfer AS ""BuktiPembayaran"",
    STRING_AGG(
        a.nama || ' x' || dt.jumlah || ' (' || dt.durasisewa || ' hari)', 
        ', '
    ) AS ""Detail Alat"",
    SUM(dt.jumlah * dt.durasisewa * dt.harga_satuan) AS ""Total Harga""
FROM transaksi t
JOIN detail_transaksi dt ON t.id = dt.id_transaksi
JOIN alat a ON a.id = dt.id_alat
WHERE t.bukti_transfer IS NOT NULL
GROUP BY 
    t.id, 
    t.tanggal,
    t.nama_pelanggan,
    t.metodepembayaran,
    t.status,
    t.bukti_transfer
ORDER BY t.tanggal DESC;
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
    t.id AS ""ID Transaksi"",
    t.tanggal AS ""Tanggal"",
    t.nama_pelanggan AS ""Nama Pelanggan"",
    t.metodepembayaran AS ""Metode Pembayaran"",
    t.status AS ""Status"",
    t.bukti_transfer AS ""BuktiPembayaran"",
    STRING_AGG(a.nama, ', ') AS ""Daftar Alat""
    FROM transaksi t
    JOIN detail_transaksi dt ON t.id = dt.id_transaksi
    JOIN alat a ON a.id = dt.id_alat
    GROUP BY t.id
    ORDER BY t.tanggal DESC;
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
                        // ✅ Perhatikan tambahan: status & bukti_transfer
                        string queryTransaksi = @"
                    INSERT INTO transaksi (tanggal, total_harga, nama_pelanggan, metodepembayaran, status, bukti_transfer)
                    VALUES (@tanggal, @total_harga, @nama_pelanggan, @metodepembayaran, @status, @bukti_transfer)
                    RETURNING id;
                ";

                        using var insertTransaksiCmd = new NpgsqlCommand(queryTransaksi, conn, transaction);
                        insertTransaksiCmd.Parameters.AddWithValue("@tanggal", transaksi.Tanggal);
                        insertTransaksiCmd.Parameters.AddWithValue("@total_harga", transaksi.TotalHarga);
                        insertTransaksiCmd.Parameters.AddWithValue("@nama_pelanggan", transaksi.NamaPelanggan);
                        insertTransaksiCmd.Parameters.AddWithValue("@metodepembayaran", transaksi.MetodePembayaran);
                        insertTransaksiCmd.Parameters.AddWithValue("@status", transaksi.Status ?? "Menunggu Konfirmasi");

                        // ❗Jika tidak ada bukti, kirimkan DBNull
                        if (transaksi.BuktiPembayaran != null)
                            insertTransaksiCmd.Parameters.AddWithValue("@bukti_transfer", transaksi.BuktiPembayaran);
                        else
                            insertTransaksiCmd.Parameters.AddWithValue("@bukti_transfer", DBNull.Value);

                        int idTransaksi = Convert.ToInt32(insertTransaksiCmd.ExecuteScalar());

                        // ✅ Simpan semua detail alat
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




    }


}
