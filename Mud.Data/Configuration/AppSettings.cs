namespace Mud.Data.Configuration
{
    public class AppSettings
    {
        public PostgresSettings PostgresSettings { get; set; }
        public AuthenticationSettings AuthenticationSettings { get; set; }
        public UrlSettings UrlSettings { get; set; }
    }
}
