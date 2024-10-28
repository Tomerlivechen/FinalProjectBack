

namespace FinalProject3.Models
{
    public static class BannerImage
    {
        public static string GetBannerImageLink()
        {
            Random random = new Random();
            var rand = random.Next(0, 4);
            switch (rand)
            {
                case 0:
                    return "https://res.cloudinary.com/dhle9hj3n/image/upload/v1730138581/whoyx2qtyjve1sfmrbbg.jpg";
         
                case 1:
                    return "https://res.cloudinary.com/dhle9hj3n/image/upload/v1730138545/pondezm6ey0avk7mqeay.jpg";
          
                case 2:
                    return "https://res.cloudinary.com/dhle9hj3n/image/upload/v1730138422/llpq02dkjzh1njlrizrj.jpg";
              
                case 3:
                    return "https://res.cloudinary.com/dhle9hj3n/image/upload/v1730138360/gwmim3fyuudgfo4fvjfb.jpg";
                 
                default:
                    return "https://res.cloudinary.com/dhle9hj3n/image/upload/v1730138581/whoyx2qtyjve1sfmrbbg.jpg";

            }
        }
    }
}
