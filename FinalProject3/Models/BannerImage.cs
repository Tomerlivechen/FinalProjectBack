

namespace FinalProject3.Models
{
    public static class BannerImage
    {
        public static string GetBannerImageLink()
        {
            Random random = new Random();
            var rand = random.Next(0, 7);
            switch (rand)
            {
                case 0:
                    return "";
                    break;
                case 1:
                    return "";
                    break;
                case 2:
                    return "";
                    break;
                case 3:
                    return "";
                    break;
                case 4:
                    return "";
                    break;
                case 5:
                    return "";
                    break;
                case 6:
                    return "";
                    break;
                default:
                    return "";

            }
        }
    }
}
