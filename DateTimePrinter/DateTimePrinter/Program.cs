using DateTimePrinter;

string configPath = "config.json";
var loader = new ConfigLoader(configPath);
Config config = loader.LoadConfig();

var reloader = new ConfigReloader(loader);

using var watcher = new FileSystemWatcher(Directory.GetCurrentDirectory(), configPath)
{
    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
    EnableRaisingEvents = true
};

watcher.Changed += OnConfigChanged;
watcher.Created += OnConfigCreated;

void OnConfigChanged(object sender, FileSystemEventArgs e) => reloader.ReloadConfig(ref config);
void OnConfigCreated(object sender, FileSystemEventArgs e) => reloader.ReloadConfig(ref config);

while (true)
{
    try
    {
        var local = config; //to avoid race condition
        string date = DateTime.UtcNow.ToString(local.DateFormat);
        Console.WriteLine($"{local.Message} | {date}");
        Thread.Sleep(local.IntervalInSeconds * 1000);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}



