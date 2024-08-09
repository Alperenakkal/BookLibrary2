namespace BookLibary.Api.Models
{
    public class Email
    {
        public string? Name { get; set; }
        public string EmailAddress { get; set; }
        public string? Phone { get; set; }

        public string? Message { get; set; }
        public string? HtmlContent { get; set; }
    }
}