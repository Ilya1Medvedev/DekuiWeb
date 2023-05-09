namespace BakWeb.Options
{
    public class SendGridOptions
    {

        public string ApiKey { get; set; } = null!;
        public string FromEmail { get; set; } = null!;
        public string? CompanyName { get; set; }
        public SendGridTemplateOptions? Templates { get; set; }
    }

    public class SendGridTemplateOptions
    {
        public string? ReservationConfirmationTemplateId { get; set; }
        public string? AddProductConfirmationTemplateId { get; set; }

    }
}
