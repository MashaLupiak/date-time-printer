using DateTimePrinter;
using System.Text.Json;

string configPath = "config.json";
Config config = LoadConfig();

using var watcher = new FileSystemWatcher(Directory.GetCurrentDirectory(), configPath)
{
    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
    EnableRaisingEvents = true
};

watcher.Changed += OnConfigChanged;
watcher.Created += OnConfigCreated;


while (true)
{
    try
    {
        var local = config; //to avoid race condition
        string date = DateTime.Now.ToString(local.DateFormat);
        Console.WriteLine($"{local.Message} | {date}");
        Thread.Sleep(local.IntervalInSeconds * 1000);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void OnConfigChanged(object sender, FileSystemEventArgs e) => ReloadConfig();
void OnConfigCreated(object sender, FileSystemEventArgs e) => ReloadConfig();

Config LoadConfig()
{
    string json = File.ReadAllText(configPath);
    var config = JsonSerializer.Deserialize<Config>(json);
    return config;
}

void ReloadConfig()
{
    try
    {
        config = LoadConfig(); 
        Console.WriteLine("Config reloaded");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Config reload error: " + ex.Message);
    }
}


