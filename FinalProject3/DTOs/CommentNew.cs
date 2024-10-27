namespace FinalProject3.DTOs
{
    public class CommentNew
    {
        public required string Id { get; set; }
        public string Link { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;

        public string ParentPostId { get; set; } = string.Empty;

        public string ParentCommentId { get; set; } = string.Empty;

        public string Datetime = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");

    }
}
