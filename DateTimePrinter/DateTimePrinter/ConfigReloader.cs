using System.Text.Json;

namespace DateTimePrinter
{
    public class ConfigReloader
    {
        private readonly ConfigLoader _loader;

        public ConfigReloader(ConfigLoader loader)
        {
            _loader = loader;
        }

        public void ReloadConfig(ref Config config)
        {
            try
            {
                config = _loader.LoadConfig();
                Console.WriteLine("Config reloaded");
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Config reload error: invalid JSON");
                Console.WriteLine(ex.Message);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Config reload error: file is unavailable");
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Config reload error: " + ex.Message);
            }
        }
    }
}
