namespace BakWeb.TerminalControllerClient.Models;

public class AddProductRequest
{
    public Guid ProductId { get; set; }
    public Guid MemberId { get; set; }
    public string? PhotoUrl { get; set; }
    public int UniqueCode { get; set; }

    public string? PhotoBase64 { get; set; }
}
