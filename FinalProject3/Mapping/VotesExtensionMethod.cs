using FinalProject3.DTOs;
using FinalProject3.Models;

namespace FinalProject3.Mapping
{
    public static class VotesExtensionMethod
    {
        public static Votes CreatVote(this Votes vote, AppUser user, int value)
        {
            vote.Voter = user;
            if (value == 1)
            {
                vote.Voted = 1;
            }
            if (value == -1)
            {
                vote.Voted = -1;
            }
            return vote;
        }
    }
}
