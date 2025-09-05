📁 Job Search Tracker
A scalable, maintainable C# console application that integrates with the Adzuna API to fetch job listings, track API usage, and support future multi-provider expansion. Built with clean architecture principles, ethical scraping boundaries, and robust error handling.

🚀 Features
- 🔍 Modular job search logic with constructor injection and explicit dependency management
- 📊 Persistent API usage tracking via JSON for accountability
- 🧼 Encoding-safe response parsing with charset validation
- 🧩 Scaffolded delay logic for multi-page and multi-provider support
- 🛠️ Console output polish for readability and debugging

🧠 Architecture Highlights
- Extensibility: Easily add new job providers or filters without touching core logic
- Error Handling: Graceful fallback for encoding issues, nullability warnings, and API failures
- Tracking: Usage metrics stored in ApiUsage/ApiUsage.json for transparency

📦 Setup
- Clone the repo
git clone https://github.com/your-username/job-search-tracker.git
cd job-search-tracker
- Add your Adzuna API credentials to appsettings.Development.json
{
  "Adzuna": {
    "AppId": "your-app-id",
    "AppKey": "your-app-key"
  }
}
- Run the project
dotnet run

🧪 Example Output
[✔] Fetched 25 listings from Adzuna
[ℹ] Saved usage stats to ApiUsage.json
[⚠] Skipped 3 listings due to encoding issues

🛡️ Ethical Scraping
This project respects robots.txt, rate limits, and API terms of service. All scraping logic is modular and compliant by design.

📚 Future Plans
- Add filtering by location, salary, and keywords
- Integrate additional job APIs (e.g., Indeed, Reed)
- Export listings to CSV or Markdown
- Add unit tests and CI pipeline