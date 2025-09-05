using System.Text.Json;

namespace ApiUsage
{
    public static class ApiUsageTracker
    {
        private static readonly string? FilePath = Path.Combine("ApiUsage", "ApiUsage.json");

        public static void Increment(string provider, string endpoint)
        {
            provider = provider.Trim().ToLowerInvariant();
            endpoint = endpoint.Trim().ToLowerInvariant();
            var usage = LoadUsage();

            if (!usage.ContainsKey(provider))
                usage[provider] = new Dictionary<string, int>();

            if (!usage[provider].ContainsKey(endpoint))
                usage[provider][endpoint] = 0;

            usage[provider][endpoint]++;
            /* Console.WriteLine($"🔍 Incrementing {provider} → {endpoint}");
            foreach (var kvp in usage)
            {
                Console.WriteLine($"Provider: {kvp.Key}");
                foreach (var ep in kvp.Value)
                {
                    Console.WriteLine($"  {ep.Key}: {ep.Value}");
                }
            } */
            Console.WriteLine($"✅ Usage tracked: {provider} → {endpoint}");
            SaveUsage(usage);
        }

        private static Dictionary<string, Dictionary<string, int>> LoadUsage()
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine("📂 Usage file not found. Starting fresh.");
                return new();
            }

            try
            {
                var json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json) ?? new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to load usage file: {ex.Message}");
                return new();
            }
        }

        private static void SaveUsage(Dictionary<string, Dictionary<string, int>> usage)
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
    }
}