using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FinalProject3.Mapping
{
    public static class AppImageExtensionMethod
    {
        public static AppImageDisplay ToDisplay(this AppImage image, FP3Context _context)
        {
            AppImageDisplay imageDisplay = new();
            imageDisplay.Id = image.Id;
            imageDisplay.Url = image.Url;
            imageDisplay.Title = image.Title;
            imageDisplay.Public = image.Public;
            if (image.user is not null)
            {
                imageDisplay.userId = image.user.Id;
            }
            return imageDisplay;
        }
    }
}
