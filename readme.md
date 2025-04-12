
# Kandidat: Syaikhul S R

## Sertifikat HackerRank
![Sertifikat HackerRank - .NET](/Docs/sertifikat%20HCR.png)

## Dokumentasi Postman

Berikut adalah langkah-langkah untuk menjalankan dan menguji aplikasi menggunakan Postman:

### Langkah-langkah Persiapan

1. **Atur Koneksi Database**  
   Ubah konfigurasi koneksi database pada file: `/data/AppDbContext.cs`

2. **Hapus Migrasi Sebelumnya (Jika Ada)**  
   Hapus seluruh isi folder `/migrations` untuk menghindari konflik migrasi.

3. **Inisialisasi Migrasi dan Database**  
   Jalankan perintah berikut pada terminal:

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

4. **Jalankan Aplikasi**  
   Jalankan aplikasi menggunakan perintah:

   ```bash
   dotnet run
   ```

5. **Import Dokumentasi Postman**
   - Buka aplikasi Postman  
   - Klik tombol **Import**  
   - Pilih file koleksi dokumentasi API (format `.json`) yang tersedia dalam proyek

---

Pastikan seluruh dependensi telah terinstal dan environment sudah dikonfigurasi dengan benar sebelum menjalankan aplikasi.
