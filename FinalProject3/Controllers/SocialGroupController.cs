using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Mapping;
using FinalProject3.Models;
using FinalProject32.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;



namespace FinalProject3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocialGroupController(FP3Context context, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly FP3Context _context = context;
        /// Retrieves a list of social groups and returns a card representation of each group.
        /// The response will only include groups the current user has access to.
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SocialGroupCard>>> GetGroups()

        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var SG = await _context.Group.Include(g => g.Members).ToListAsync();

            var SGCards = new List<SocialGroupCard>();
            foreach (var group in SG)
            {
               var card = await group.ToCard(currentUserId, _context);
                if (card is not null)
                {
                    SGCards.Add(card);
                }

            }
            return Ok(SGCards);
        }

        /// Retrieves detailed information about a specific group based on its ID.
        /// Only accessible to authenticated users.
        [HttpGet("ById/{GroupId}")]
        [Authorize]
        public async Task<ActionResult<SocialGroupDisplay>> GetGroupbyId(string GroupId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var fullGroup = await _context.Group.Include(sg => sg.groupAdmin).Include(sg=> sg.Members).FirstOrDefaultAsync(g => g.Id == GroupId);
            if (fullGroup is null)
            {
                return NotFound();
            }
            var displayGroup = await fullGroup.ToDisplay(currentUserId, _context);

            return Ok(displayGroup);
        }


        /// Retrieves the list of members for a specific social group based on its ID.
        /// The response includes basic display information for each member.
        [HttpGet("GetMembers/{GroupId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppUserDisplay>>> GetMembersByGroupbyId(string GroupId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            if (currentUser is null)
            {
                return NotFound();    
            }
            var group = await _context.Group.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == GroupId);
            if (group is null)
            {
                return NotFound("Gruop not found");
            }
                var members = group.Members.ToList();
            var memberUsersDisplay = new List<AppUserDisplay>();

            foreach (var member in members)
            {
                var memberDisplay = await member.UsertoDisplay(_context, currentUser);
                if (memberDisplay is not null)
                {
                    memberUsersDisplay.Add(memberDisplay);
                }
            }
            return Ok(memberUsersDisplay);
            
                
        }
        /// Creates a new social group based on the provided data.
        /// The new group is saved to the database and a card representation of the group is returned.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SocialGroup>> PostGroup(SocialGroupNew groupNew) 
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var group= await groupNew.CreateGroup(currentUserId, userManager);
            await _context.Group.AddAsync(group);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Conflict(ex);
            }

            return Ok(await group.ToDisplay(currentUserId, _context));
        }
        /// Adds a new member to a specific social group by its ID.
        /// If the user is already a member, a conflict error is returned.
        [HttpPut("AddMember/{groupId}")]
        [Authorize]
        public async Task<ActionResult> AddMember(string groupId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            var currentUser = await userManager.FindByIdAsync(currentUserId);
            if (currentUser is null)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var group = await _context.Group.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == groupId);
            if (group is null)
            {
                return NotFound("Group Not Found");
            }
            var user = await userManager.FindByIdAsync(currentUserId);
            if (user is null) 
            {
                return NotFound("User Not Found");
            }
            if (group.Members.Any(m => m.Id == currentUserId))
            {
                return Conflict("User is already a member of this group.");
            }
            user.SocialGroups.Add(group);
            group.Members.Add(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
               return Conflict(ex);
            }
            return Ok();
        }



        /// Removes a specific member from a social group by its ID.
        /// The user to be removed is identified by the provided user ID.
        [HttpPut("RemoveMember/{groupId}")]
        [Authorize]
        public async Task<ActionResult> RemoveMember(string groupId,[FromBody] IdInput userIdInput) 
        {

            string UserId = userIdInput.id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var group = await _context.Group.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == groupId);
            if (group is null)
            {
                return NotFound("Group Not Found");
            }
            var user = await userManager.FindByIdAsync(UserId);
            if (user is null)
            {
                return NotFound("User Not Found");
            }
            if (!group.Members.Any(m => m.Id == UserId))
            {
                return Conflict("User not a member of this group.");
            }
            group.Members.Remove(group.Members.First(m => m.Id == UserId));
            if (user.SocialGroups.Contains(group))
            {
                user.SocialGroups.Remove(group);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Conflict(ex);
            }
            return Ok();
        }
        /// Edits the details of an existing social group. Only the group admin can make changes.
        [HttpPut("EditGroup")]
        [Authorize]
        public async Task<ActionResult> EditGroup(SocialGroupEdit editGroup)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var group = await _context.Group.FindAsync(editGroup.Id);
            if (group is null)
            {
                return NotFound("Group Not Found");
            }
            if (currentUserId != group.AdminId)
            {
                return Unauthorized();
            }
            await group.UpdateGroup(editGroup, userManager);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Conflict(ex);
            }
            return Ok();
        }
        /// Deletes a social group based on its ID. The group is marked as deleted rather than permanently removed.
        /// Only the group admin with "Admin" permission can delete a group.
        [HttpDelete("{groupId}")]
        [Authorize]
        public async Task<ActionResult> DeleteGroup(string groupId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserPremission = User.FindFirstValue("PermissionLevel");
            var group = await _context.Group.FindAsync(groupId);
            if (currentUserId is null)
            {
                return Unauthorized();
            }
            if (group is null)
            {
                return NotFound("Group Not Found");
            }
            if (currentUserId != group.AdminId || currentUserPremission != "Admin")
            {
                return Unauthorized();
            }
            group.Name = "__$DeletedGroup$__";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Problem(ex.Message);
            }

            return Ok();

        }

    }
}
