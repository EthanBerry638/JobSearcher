using System.Security.Cryptography;
using System.Text.Json;
using Helpers;

namespace ApiUsage
{
    public static class ApiUsageTracker
    {
        private static readonly string? FilePath = Path.Combine("ApiUsage", "ApiUsage.json");

        public static void Increment(string provider, string endpoint)
        {
            provider = provider.Trim().ToLowerInvariant();
            endpoint = endpoint.Trim().ToLowerInvariant();
            var today = DateTime.Now.ToString("yyyy-MM-dd");

            var usage = LoadUsage();

            if (!usage.ContainsKey(today))
                usage[today] = new Dictionary<string, Dictionary<string, int>>();

            if (!usage[today].ContainsKey(provider))
                usage[today][provider] = new Dictionary<string, int>();

            if (!usage[today][provider].ContainsKey(endpoint))
                usage[today][provider][endpoint] = 0;

            usage[today][provider][endpoint]++;
            Console.WriteLine($"✅ Usage tracked: {provider} → {endpoint} on {today}");
            SaveUsage(usage);
            int count = GetTodayCount("adzuna");
            Console.WriteLine($"\n📅 Requests made today to Adzuna: {count}");
            Helper.Pause(1000);
        }

        private static Dictionary<string, Dictionary<string, Dictionary<string, int>>> LoadUsage()
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("📂 Usage file not found. Starting fresh.");
                return new();
            }

            try
            {
                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, int>>>>(json)
                       ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to load usage file: {ex.Message}");
                return new();
            }
        }

        private static void SaveUsage(Dictionary<string, Dictionary<string, Dictionary<string, int>>> usage)
        {
            try
            {
                var json = JsonSerializer.Serialize(usage, new JsonSerializerOptions { WriteIndented = true });

                Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                File.WriteAllText(FilePath!, json);

                Console.WriteLine($"✅ Saved usage to {Path.GetFullPath(FilePath!)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Save failed: {ex}");
            }
        }

        public static int GetTodayCount(string provider)
        {
            provider = provider.Trim().ToLowerInvariant();
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var usage = LoadUsage();

            if (usage.ContainsKey(today) && usage[today].ContainsKey(provider))
                return usage[today][provider].Values.Sum();

            return 0;
        }

        public static int GetCountForDate(string provider, string date)
        {
            provider = provider.Trim().ToLowerInvariant();
            var usage = LoadUsage();

            if (usage.ContainsKey(date) && usage[date].ContainsKey(provider))
                return usage[date][provider].Values.Sum();

            return 0;
        }
    }
}
