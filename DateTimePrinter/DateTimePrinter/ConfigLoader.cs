using System.Text.Json;

namespace DateTimePrinter
{
    public class ConfigLoader
    {
        private readonly string _configPath;

        public ConfigLoader(string configPath)
        {
            _configPath = configPath;
        }

        public Config LoadConfig()
        {
            try
            {
                string json = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<Config>(json);

                if (config == null)
                    throw new Exception("Config file is empty");

                if (config.IntervalInSeconds <= 0)
                    throw new Exception("Interval must be > 0");

                if (string.IsNullOrWhiteSpace(config.DateFormat))
                    config.DateFormat = "yyyy-MM-dd HH:mm:ss";

                return config;
            }
            catch (JsonException ex)
            {
                throw new Exception("Config file contains invalid JSON", ex);
            }
            catch (IOException ex)
            {
                throw new Exception("Config file cannot be read (it may be in use)", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to read config file: " + ex.Message);
            }
        }
    }
}
