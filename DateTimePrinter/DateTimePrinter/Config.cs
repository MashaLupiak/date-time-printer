namespace DateTimePrinter
{
    public class Config
    {
        public string Message { get; set; } = "";
        public string DateFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public int IntervalInSeconds { get; set; } = 5;
    }
}
