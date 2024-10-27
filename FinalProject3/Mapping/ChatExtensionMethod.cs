using FinalProject3.Data;
using FinalProject3.DTOs;
using FinalProject3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace FinalProject3.Mapping
{
    public static class ChatExtensionMethod
    {
        public static string NotificatioToWho(this Chat chat)
        {
            var message = chat.messages.Last();
            var user1 = message.UserId;
            string ToNotify = chat.User1Id == user1? user1 : chat.User2Id;
            return ToNotify;
        }


        public static async Task<Notification> CreatMessageNotification (this Message message, Chat chat, string UserId, FP3Context context)
        {
            var newNotification = new NotificationNew()
            {
                NotifierId = UserId,
                NotifiedId = getOtherUser(chat, UserId),
                Type = "Message",
                ReferenceId = chat.Id,
            };
            Notification notification = await newNotification.AddNotification(context);
            return notification;
        }



        public static string getOtherUser (this Chat chat, string UserId)
        {
            if (chat.User1Id == UserId)
            {
                return chat.User2Id;
            }
            if (chat.User2Id == UserId)
            {
                return chat.User1Id;
            }
            else
            {
                return "error";
            }
        }


        public static Message MakeMessage(this MessageNew newMessage, string userId, string userName) 
        {
            var setMessage = new Message()
            {
                ChatId = newMessage.ChatId,
                UserId = userId,
                UserName = userName,
                message = newMessage.message,
                Datetime = newMessage.Datetime,
                Id = Guid.NewGuid().ToString(),

            };
            return setMessage;

        }


        public static ChatDisplay ToDisplay(this Chat chat)
        {
            var chatDisplay = new ChatDisplay()
            {
                Id = chat.Id,
                User1Name = chat.User1Name,
                User1Id = chat.User1Id,
                User2Id = chat.User2Id,
                User2Name = chat.User2Name,
            };
            foreach (var message in chat.messages) {
                chatDisplay.messages.Add(message.ToDisplay());
            }
            return chatDisplay;
        }

        public static MessageDisplay ToDisplay(this Message Fullmessage)
        {
            var DisplayMessage = new MessageDisplay() {
                id = Fullmessage.Id,
                message = Fullmessage.message,
            ChatId = Fullmessage.ChatId,
            UserName= Fullmessage.UserName,
            UserId = Fullmessage.UserId,
            Datetime= Fullmessage.Datetime,
            };
            return DisplayMessage;
        }

        public static async Task<Chat?> MakeNewChat(this ChatNew chat, UserManager<AppUser> userManager)
        {
            var user1 = await userManager.FindByIdAsync(chat.User1Id);
            var user2 = await userManager.FindByIdAsync(chat.User2Id);
            if (user1 is null || user2 is null || user1.UserName is null || user2.UserName is null)
            {
                return null;
            }
            Chat? chatWithUser;
            foreach (var oldChat in user1.Chats)
            {
                if (oldChat.User1Id == user2.Id || oldChat.User2Id == user2.Id)
                {
                    chatWithUser = oldChat;
                    return chatWithUser;
                }
            }

            if (user1 is not null && user2 is not null)
            {
                var newChat = new Chat()
                {
                    User1Id = user1.Id,
                    User2Id = user2.Id,
                    User1Name = user1.UserName,
                    User2Name = user2.UserName,
                    messages = new List<Message>(),
                    Id = Guid.NewGuid().ToString()
                };
                newChat.Users.Add(user1);
                newChat.Users.Add(user2);
                return newChat;
            }
            return null;
        }

     

    }
}
