﻿using FinalProject3.Auth;
using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Mapping;
using FinalProject3.Models;
using FinalProject32.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace FinalProject3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(FP3Context context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IJwtTokenService jwtTokenService) : Controller
{
    private readonly FP3Context _context = context;
    /// Registers a new user and signs them in automatically.
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AppUserRegister register)
    {
        if (register.PermissionLevel == "Admin")
        {
            return Unauthorized();
        }
        if (ModelState.IsValid)
        {
            AppUser user = register.RegisterToUser();
            var result = await userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: true);
                AppUserLogin userLogin = new(register.Email, register.Password);
                await LogIn(userLogin);
                return Ok(result);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Register Failed", error.Description);
            }
        }

        return BadRequest(ModelState);
    }
    /// Retrieves a list of all users for authorized requests.
    [HttpGet("GetUsers")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AppUserDisplay>>> GetUsers()
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
        var users = await userManager.Users.ToListAsync();
        var usersDisplay = new List<AppUserDisplay>();
        foreach (var user in users)
        {
            var display = await user.UsertoDisplay(_context, currentUser);
            if (display is not null)
            {
                usersDisplay.Add(display);
            }
        }
        currentUser.LastActive = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");
        await userManager.UpdateAsync(currentUser);
        return Ok(usersDisplay);
    }
    /// Toggles a user's activation status by their ID. Only admins are allowed to perform this action.

    [HttpDelete("ToggleInactivationById/{userId}")]
    [Authorize]
    public async Task<ActionResult> DeleteUser(string userId)
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
        if (currentUser.PermissionLevel != "Admin")
        {
            return Unauthorized();
        }
        var userToDelete = await userManager.FindByIdAsync(userId);
        if (userToDelete is null)
        {
            return NotFound();
        }
        if (userToDelete.PermissionLevel == "Admin")
        {
            return Unauthorized();
        }
        if (userToDelete.PermissionLevel != "InActive")
        {
            userToDelete.PermissionLevel = "InActive";
        }
        else
        {
            userToDelete.PermissionLevel = "User";
        }
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return Problem(ex.Message);
        }

        return Ok(userToDelete);
    }

    /// Retrieves a list of users the specified user is following.

    [HttpGet("GetFollowing/{userId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<AppUserDisplay>>> GetFollowing(string userId)
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
        var user = await _context.Users.Include(u => u.Following).Where(u => u.Id == userId).FirstOrDefaultAsync();
        if (user is null)
        {
            return NotFound("User not found.");
        }
        var following = user.Following;
        if (following is null)
        {
            return Ok();
        }

        var followingDsplay = new List<AppUserDisplay>();
        foreach (var follower in following)
        {
            var display = await follower.UsertoDisplay(_context, currentUser);
            if (display is not null)
            {
                followingDsplay.Add(display);
            }
        }


        return Ok(followingDsplay);
    }

    /// Retrieves a user by their email address.
    [HttpGet("userByEmail/{userEmail}")]
    public async Task<ActionResult<AppUserDisplay>> GetUserByEmail(string userEmail)
    {
        var currentUser = await userManager.FindByEmailAsync(userEmail);
        if (currentUser is null)
        {
            return NotFound($"user {userEmail} not found ");
        }
        var currentUserDisplay = await currentUser.UsertoDisplay(_context, currentUser);
        return Ok(currentUserDisplay);
    }

    /// Generates a password reset token for a user identified by their email.
    [HttpGet("ResetPassword/{userEmail}")]
    public async Task<ActionResult<string>> GetUserPassword(string userEmail)
    {
        var currentUser = await userManager.FindByEmailAsync(userEmail);
        if (currentUser is null)
        {
            return NotFound($"user {userEmail} not found ");
        }
        var token = await userManager.GeneratePasswordResetTokenAsync(currentUser);
        return Ok(token);
    }

    /// Resets a user's password using a token.
    [HttpPut("SetNewPassword")]
    public async Task<ActionResult<string>> SetNewPassword(ReNewPasswordDTO passwordDTO)
    {
        var currentUser = await userManager.FindByEmailAsync(passwordDTO.userEmail);
        if (currentUser is null)
        {
            return NotFound($"user {passwordDTO.userEmail} not found ");
        }
        var result = await userManager.ResetPasswordAsync(currentUser, passwordDTO.token, passwordDTO.newPassword);
        if (result.Succeeded)
        {
           return Ok(result);
        }
        else
        {
            return BadRequest("Password reset error");
        }
    }


    /// Retrieves the social groups a user belongs to.
    [HttpGet("GroupsByUser/{userId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SocialGroupCard>>> GetGroups(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null)
        {
            return Unauthorized();
        }
        var user = await _context.Users.Include(u => u.SocialGroups).ThenInclude(sg=> sg.GroupCreator).Where(u => u.Id == userId).FirstOrDefaultAsync();
        if (user is null)
        {
            return NotFound("User not found.");
        }
        var groups = user.SocialGroups;
        if (groups is null)
        {
            return Ok();
        }
        var groupCards = new List<SocialGroupCard>();
        foreach (var group in groups)
        {
            var Card = await group.ToCard(currentUserId, _context);
            if (Card is not null)
            {
                groupCards.Add(Card);
            }
        }



        return Ok(groupCards);
    }

    /// Retrieves a user's details by their ID.
    [HttpGet("ById/{userId}")]
    [Authorize]
    public async Task<ActionResult<AppUserDisplay>> GetUser(string userId)
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
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is not null)
        {
            var usersDisplay = await user.UsertoDisplay(_context, currentUser);
            return Ok(usersDisplay);
        }

            return NotFound("User Not Found");
        
    }
    /// Retrieves the full details of a user, including following and posts.
    [HttpGet("GetFullUser/{userId}")]
    [Authorize]
    public async Task<ActionResult<AppUser>> GetFullUser(string userId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = await userManager.Users.Include(u => u.Following).Include(u => u.Posts).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is not null)
        {
            return Ok(user);
        }

        return NotFound("User Not Found");

    }
    /// Retrieves the IDs of users the current user is following.
    [HttpGet("GetFollowingIds")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<string>>> GetFollowingId( )
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = await userManager.Users.Include(u => u.Following).FirstOrDefaultAsync(u => u.Id == currentUserId);
        if (user is null)
        {
            return Unauthorized();
        }
        var following = user.Following.Select(u => u.Id).ToList();
        return Ok(following);
        

    }

    /// Authenticates and logs in a user.
    [HttpPost("login")]
    public async Task<IActionResult> LogIn([FromBody] AppUserLogin login)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync(login.Email);
            if (user != null && user.UserName != null)
            {
                if (user.PermissionLevel == "InActive")
                {
                    return Unauthorized("Your User Has Been Inactivated");
                }
                var result = await signInManager.PasswordSignInAsync(user.UserName, login.Password, isPersistent: true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    

                    if (user != null)
                    {
                        var token = await jwtTokenService.CreateToken(user);
                        return Ok(new { token });
                    }
                }
                return Unauthorized();
            }
        }
        return Unauthorized();
    }
    /// Validates the provided JWT token.
   [HttpGet("validateToken")]
   [Authorize]
    public async Task<IActionResult> ValidateToken()
    {
            return Ok();
    }



    /// Logs out the current user and redirects to the homepage.
   
  [HttpGet("Logout")]
    public async Task<IActionResult> LogOut()
    {
        await signInManager.SignOutAsync();
        return Redirect("/");
    }
    /// Manages user details and updates if necessary.
    [HttpPut("manage")]
    [Authorize]
    public async Task<ActionResult<AppUserDisplay>> Manage([FromBody] AppUserEdit manageView)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null)
        {
            return Unauthorized();
        }
        var currentUser = await userManager.FindByIdAsync(currentUserId);
        
        bool changed = false;
        if (currentUser is null)
        {
            return Unauthorized();
        }
        if (!string.IsNullOrEmpty(manageView.oldPassword)  && !string.IsNullOrEmpty(manageView.newPassword) )
        {
            var passwordChange = await userManager.ChangePasswordAsync(currentUser, manageView.oldPassword, manageView.newPassword);
            if (passwordChange.Succeeded)
            {
                changed = true;
            }
            else
            {
                foreach (var error in passwordChange.Errors)
                {
                    ModelState.AddModelError("Password error", error.Description);
                    return View(manageView);
                }
            }
        }
        if (manageView.userName != currentUser.UserName)
        {
            currentUser.UserName = manageView.userName;
            changed = true;
        }
        if (manageView.prefix != currentUser.Prefix)
        {
            currentUser.Prefix = manageView.prefix;
            changed = true;
        }
        if (manageView.first_Name != currentUser.Last_Name)
        {
            currentUser.First_Name = manageView.first_Name;
            changed = true;
        }
        if (manageView.last_Name != currentUser.Last_Name)
        {
            currentUser.Last_Name = manageView.last_Name;
            changed = true;
        }
        if (manageView.pronouns != currentUser.Pronouns)
        {
            currentUser.Pronouns = manageView.pronouns;
            changed = true;
        }
        if (manageView.imageURL != currentUser.ImageURL)
        {
            currentUser.ImageURL = manageView.imageURL;
            changed = true;
        }
        if (manageView.bio != currentUser.Bio)
        {
            currentUser.Bio = manageView.bio;
            changed = true;
        }
        if (manageView.banerImageURL != currentUser.BanerImageURL)
        {
            currentUser.BanerImageURL = manageView.banerImageURL;
            changed = true;
        }
        if (manageView.hideEmail != currentUser.HideEmail)
        {
            currentUser.HideEmail = manageView.hideEmail;
            changed = true;
        }
        if (manageView.hideName != currentUser.HideName)
        {
            currentUser.HideName = manageView.hideName;
            changed = true;
        }
        if (manageView.hideBlocked != currentUser.HideBlocked)
        {
            currentUser.HideBlocked = manageView.hideBlocked;
            changed = true;
        }
        if (manageView.permissionLevel != currentUser.PermissionLevel)
        {
            if (manageView.permissionLevel == "User" || manageView.permissionLevel == "PowerUser")
            {
                currentUser.PermissionLevel = manageView.permissionLevel;
                changed = true;
            }
        }
        if (changed)
        {
            await userManager.UpdateAsync(currentUser);
            var returnUser = await currentUser.UsertoDisplay(_context, currentUser);
            return Ok(returnUser);
        }
        ModelState.AddModelError("No Changes", "No changes made");
        var returnEmptyUser = await currentUser.UsertoDisplay(_context, currentUser);
        return Ok(returnEmptyUser);
    }
    /// Adds a user to the current user's following list.
    [HttpPut("follow")]
    [Authorize]
    public async Task<IActionResult> Follow([FromBody] AppUserIdRequest followId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(new { ModelState, userId });
        }

        var user = await userManager.FindByIdAsync(userId);
        var follow = await userManager.FindByIdAsync(followId.Id);

        if (user is null || follow is null)
        {
            return BadRequest("User not found");
        }
        user.Following.Add(follow);
        user.FollowingId ??= [];
        user.FollowingId.Add(follow.Id.ToString());

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest("Failed to update user following list.");
        }
        return Ok(user.FollowingId);
    }
    /// Removes a user from the current user's following list.
    [HttpPut("unfollow")]
    [Authorize]
    public async Task<IActionResult> UnFollow([FromBody] AppUserIdRequest unfollowId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.Include(u => u.Following).FirstOrDefaultAsync(u => u.Id == userId);
        var unfollow = await userManager.FindByIdAsync(unfollowId.Id);

        if (user is null || unfollow is null)
        {
            return BadRequest("User not found");
        }
        user.FollowingId.Remove(unfollow.Id);
        user.Following.Remove(unfollow);
        var result = await userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        if (!result.Succeeded)
        {
            return BadRequest("Failed to update user following list.");
        }
        return Ok(user.FollowingId);
    }

    /// Adds a user to the current user's blocked list.
    [HttpPut("block")]
    [Authorize]
    public async Task<IActionResult> Block([FromBody] AppUserIdRequest toBlock)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var blockedUser = await userManager.FindByIdAsync(toBlock.Id);
        if (blockedUser is null || blockedUser.PermissionLevel == "Admin")
        {
            return Unauthorized();
        }


        var user = await _context.Users.Include(u => u.Blocked).FirstOrDefaultAsync(u => u.Id == userId);
        var BlockUser = await userManager.FindByIdAsync(toBlock.Id);

        if (user is null || BlockUser is null)
        {
            return BadRequest("User not found");
        }
        user.BlockedId.Add(toBlock.Id);
        user.Blocked.Add(BlockUser);
        var result = await userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        if (!result.Succeeded)
        {
            return BadRequest("Failed to update user block list.");
        }
        return Ok(user.BlockedId);
    }
    /// Removes a user to the current user's blocked list.
    [HttpPut("unblock")]
    [Authorize]
    public async Task<IActionResult> UnBlock([FromBody] AppUserIdRequest unBlock)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.Include(u => u.Blocked).FirstOrDefaultAsync(u => u.Id == userId);
        var BlockedUser = await userManager.FindByIdAsync(unBlock.Id);

        if (user is null || BlockedUser is null)
        {
            return BadRequest("User not found");
        }
        var deleteBlockedId = user.BlockedId.Remove(unBlock.Id);
        var deleteBlocked = user.Blocked.Remove(BlockedUser);
        var result = await userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();
        if (!result.Succeeded && deleteBlockedId && deleteBlocked)
        {
            return BadRequest("Failed to update user block list.");
        }
        return Ok(user.BlockedId);
    }


}

