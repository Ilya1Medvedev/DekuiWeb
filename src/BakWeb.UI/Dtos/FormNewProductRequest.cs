namespace BakWeb.Dtos
{
    public class FormNewProductRequest
    {
        public Guid FormId { get; set; }
        public string PageUrl { get; set; }
        public DateTime SubmissionDate { get; set; }

        public string Name { get; set; }
        public string Photo { get; set; }
        public string Size { get; set; }
    }
}
