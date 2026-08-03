namespace Kanban.Infrastructure.Security;

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Kanban.Api";
    public string Audience { get; set; } = "Kanban.Client";
    public int ExpirationMinutes { get; set; } = 480;
}
