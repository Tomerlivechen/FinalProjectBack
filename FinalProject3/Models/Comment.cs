namespace FinalProject3.Models
{
    public class Comment : Interaction
    {
        public  Post? ParentPost { get; set; }

        public Comment? ParentComment { get; set; }


    }
}
