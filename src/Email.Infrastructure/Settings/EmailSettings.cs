namespace Email.Infrastructure.Settings
{
    public class EmailSettings
    {
        public string ApiKey { get; init; } = string.Empty;

        public string ApiSecret { get; init; } = string.Empty;

        public string SenderEmail { get; init; } = string.Empty;
    }
}
