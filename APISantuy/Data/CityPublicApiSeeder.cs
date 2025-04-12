using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

public class CityPublicApiSeeder
{
    private readonly AppDbContext _context;

    public CityPublicApiSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedCitiesFromPublicApiAsync()
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("https://alamat.thecloudalert.com/api/kabkota/get");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Gagal mengambil data dari API.");
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        var kabkotaData = JsonConvert.DeserializeObject<KabKotaResponse>(content);

        if (kabkotaData?.result == null)
        {
            Console.WriteLine("Data dari API kosong atau tidak sesuai format.");
            return;
        }

        try
        {
            // Clear existing data first for re-seeding
            Console.WriteLine("Menghapus data kota yang sudah ada...");
            _context.Cities.RemoveRange(_context.Cities);
            await _context.SaveChangesAsync();
            
            // Process all cities from the API
            var cities = kabkotaData.result.Select(item => new City
            {
                Name = item.text
            }).ToList();
            
            // Add all cities at once
            _context.Cities.AddRange(cities);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"Seeder kota selesai dijalankan. Total {cities.Count} kota berhasil disimpan.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding cities: {ex.Message}");
            Console.WriteLine(ex.InnerException?.Message ?? "");
        }
    }

}