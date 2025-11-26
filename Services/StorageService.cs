using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using shreeji_packaging.Models;

namespace shreeji_packaging.Services
{
    public static class StorageService
    {
        public static readonly string StorageRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "storage");

        public static void EnsureCustomerDirectories(string customerName)
        {
            var customerDir = GetCustomerDirectory(customerName);
            var recordsDir = Path.Combine(customerDir, "records");
            Directory.CreateDirectory(recordsDir);
        }

        public static string[] ListCustomers()
        {
            if (!Directory.Exists(StorageRoot)) Directory.CreateDirectory(StorageRoot);
            return Directory.GetDirectories(StorageRoot).Select(Path.GetFileName).ToArray();
        }

        public static void CreateCustomer(string customerName)
        {
            EnsureCustomerDirectories(customerName);
        }

        public static void DeleteCustomer(string customerName)
        {
            var customerDir = GetCustomerDirectory(customerName);
            if (Directory.Exists(customerDir))
            {
                Directory.Delete(customerDir, true);
            }
        }

        public static void SaveRecord(string customerName, BoxRecord record)
        {
            EnsureCustomerDirectories(customerName);
            var recordsDir = Path.Combine(GetCustomerDirectory(customerName), "records");

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
            var filePath = Path.Combine(recordsDir, $"{timestamp}.json");

            var json = JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        public static List<BoxRecord> LoadRecords(string customerName)
        {
            var records = new List<BoxRecord>();
            var recordsDir = Path.Combine(GetCustomerDirectory(customerName), "records");
            if (!Directory.Exists(recordsDir)) return records;

            foreach (var file in Directory.GetFiles(recordsDir, "*.json").OrderBy(f => f))
            {
                try
                {
                    var json = File.ReadAllText(file, Encoding.UTF8);
                    var record = JsonSerializer.Deserialize<BoxRecord>(json);
                    if (record != null) records.Add(record);
                }
                catch
                {
                    // Skip unreadable files
                }
            }

            return records;
        }

        private static string GetCustomerDirectory(string customerName)
        {
            var safeName = MakeSafeName(customerName);
            return Path.Combine(StorageRoot, safeName);
        }

        public static string MakeSafeName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(name.Length);
            foreach (var ch in name)
            {
                builder.Append(invalid.Contains(ch) ? '_' : ch);
            }
            return builder.ToString().Trim();
        }

        public static void SaveInventory(Inventory inventory)
        {
            if (!Directory.Exists(StorageRoot)) Directory.CreateDirectory(StorageRoot);
            var inventoryPath = Path.Combine(StorageRoot, "inventory.json");
            var json = JsonSerializer.Serialize(inventory, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(inventoryPath, json, Encoding.UTF8);
        }

        public static Inventory LoadInventory()
        {
            var inventoryPath = Path.Combine(StorageRoot, "inventory.json");
            if (!File.Exists(inventoryPath))
            {
                return new Inventory();
            }

            try
            {
                var json = File.ReadAllText(inventoryPath, Encoding.UTF8);
                var inventory = JsonSerializer.Deserialize<Inventory>(json);
                return inventory ?? new Inventory();
            }
            catch
            {
                return new Inventory();
            }
        }

        // No encryption helpers needed for plain JSON storage
    }
}


