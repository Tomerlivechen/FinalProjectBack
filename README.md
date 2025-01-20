
# Deinonychus Social Media Platform API Guide  


## Table of Contents  
- [Overview](#overview)  
- [Architecture](#architecture)  
  - [Backend](#backend)  
  - [Frontend](#frontend)  
- [Technologies](#technologies)  
  - [Backend Technologies](#backend-technologies)  
  - [Frontend Technologies](#frontend-technologies)  
- [Authentication](#authentication)  
- [API Endpoints](#api-endpoints)  
  - [Auth](#auth)  
    - [User Management](#user-management)  
    - [User Information](#user-information)  
    - [Authentication and Password Management](#authentication-and-password-management)  
    - [Social Actions](#social-actions)  
  - [Chat](#chat)  
  - [Comments](#comments)  
  - [Images](#image)  
  - [Notifications](#notification)  
  - [Posts](#posts)  
  - [Social Groups](#socialgroup)
- [Database Schema and Entity Relationships](#database-schema-and-entity-relationships)  
  - [Social Groups and Members](#social-groups-and-members)  
  - [User Following Relationships](#user-following-relationships)  
  - [Chats and Users](#chats-and-users)  
  - [Posts and Comments](#posts-and-comments)  
- [License](#license)  
- [Author](#author)  


## Overview  
This document provides an overview of the API endpoints for the Deinonychus Social Media Platform backend. Each section covers the routes, parameters, and expected input/output formats.  

Azure deployment URL:  
```
https://finalprojectapinew-dah9g5bcg9bycme4.southeastasia-01.azurewebsites.net  
```  
### Hosted Website
[Deinonychus](https://finalprojectfront-6dxw.onrender.com)

## Architecture

### Backend
- **Framework**: ASP.NET Core 7.0
- **Architecture Style**: RESTful APIs
- **Authentication**: JWT-based Authentication and Authorization
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core

### Frontend
- **Framework**: React with Vite
- **Styling**: Tailwind CSS and Bootstrap
- **State Management**: Context API and React Hooks

---

## Technologies

### Backend
- ASP.NET Core 7.0
- Entity Framework Core
- Microsoft SQL Server
- Identity Framework
- JWT for Authentication and Authorization

### Frontend
- React
- Vite
- Axios for API calls
- Cloudinary for media storage
- Tailwind CSS and Bootstrap for UI

---

## Authentication  
The platform uses JWT-based authentication to ensure secure access to resources. Registered users are assigned roles and permissions to manage posts, comments, and groups.  

---

## API Endpoints  
### Auth

### User Management
- **Register a New User**  
  `POST /api/Auth/register`
  
- **Get All Users**  
  `GET /api/Auth/GetUsers`
  
- **Inactivate or Reactivate User**  
  `DELETE /api/Auth/ToggleInactivationById/{userId}`

### User Information
- **Get User's Following List**  
  `GET /api/Auth/GetFollowing/{userId}`
  
- **Get User by Email**  
  `GET /api/Auth/userByEmail/{userEmail}`
  
- **Get User by ID**  
  `GET /api/Auth/ById/{userId}`

- **Get Full User Details**  
  `GET /api/Auth/GetFullUser/{userId}`
  
- **Get IDs of Users the Current User is Following**  
  `GET /api/Auth/GetFollowingIds`

- **Get Groups by User**  
  `GET /api/Auth/GroupsByUser/{userId}`

### Authentication and Password Management
- **Login**  
  `POST /api/Auth/login`
  
- **Validate Token**  
  `GET /api/Auth/validateToken`
  
- **Logout**  
  `GET /api/Auth/Logout`

- **Reset Password**  
  `GET /api/Auth/ResetPassword/{userEmail}`
  
- **Set New Password**  
  `PUT /api/Auth/SetNewPassword`

### Social Actions
- **Manage User**  
  `PUT /api/Auth/manage`
  
- **Follow a User**  
  `PUT /api/Auth/follow`
  
- **Unfollow a User**  
  `PUT /api/Auth/unfollow`
  
- **Block a User**  
  `PUT /api/Auth/block`
  
- **Unblock a User**  
  `PUT /api/Auth/unblock`

---

## Chat

- **Create a Chat**  
  `POST /api/Chat`

- **Get Chat by ID**  
  `GET /api/Chat/ByChatId/{ChatID}`
  
- **Get Chats Excluding Followed Users**  
  `GET /api/Chat/notFollowingChats`
  
- **Edit a Message**  
  `PUT /api/Chat/EditMessage/{MessageId}`
  
- **Delete a Message**  
  `DELETE /api/Chat/DeleteMessage/{MessageId}`
  
- **Send a Message**  
  `POST /api/Chat/Message`

---

## Comments

- **Get Comments by Post ID**  
  `GET /api/Comments/ByPPostId/{PostID}`
  
- **Get Comments by Parent Comment ID**  
  `GET /api/Comments/ByPCommentId/{CommentID}`
  
- **Get Comment by Comment ID**  
  `GET /api/Comments/ByCommentId/{CommentID}`
  
- **Get Comment by ID**  
  `GET /api/Comments/ById/{CommentID}`
  
- **Create a Comment**  
  `POST /api/Comments`
  
- **Edit a Comment**  
  `PUT /api/Comments/{id}`
  
- **Delete a Comment**  
  `DELETE /api/Comments/{id}`
  
- **Vote on a Comment**  
  `PUT /api/Comments/VoteById/{commentId}`
  
- **Remove Vote from a Comment**  
  `PUT /api/Comments/UnVoteById/{commentId}`

---

## Image

- **Upload an Image**  
  `POST /api/Image`
  
- **Get Images by User ID**  
  `GET /api/Image/byId/{UserId}`
  
- **Delete an Image**  
  `DELETE /api/Image/byId/{imageId}`
  
- **Edit Image Details**  
  `PUT /api/Image/byId/{imageId}`
  
- **Toggle Image Privacy**  
  `PUT /api/Image/togglePrivatebyId/{imageId}`

---

## Notification

- **Fetch All Notifications**  
  `GET /api/Notification`
  
- **Update Notifications**  
  `PUT /api/Notification`
  
- **Create a Notification**  
  `POST /api/Notification`
  
- **Get Notification by ID**  
  `GET /api/Notification/ByNotificationID/{notificationID}`
  
- **Update Specific Notification**  
  `PUT /api/Notification/Update/{notificationID}`

---

## Posts

- **Fetch All Posts**  
  `GET /api/Posts`
  
- **Create a Post**  
  `POST /api/Posts`
  
- **Get Post by ID**  
  `GET /api/Posts/ById/{id}`
  
- **Search Posts by Keyword**  
  `GET /api/Posts/ByKeyword/{KeyWord}`
  
- **Fetch Posts by Group**  
  `GET /api/Posts/ByGroup/{GroupId}`
  
- **Fetch Posts by Author**  
  `GET /api/Posts/ByAuthor/{AuthorId}`
  
- **Get Voted Posts**  
  `GET /api/Posts/ByVotedOn`
  
- **Fetch Downvoted Posts**  
  `GET /api/Posts/ByDownVote/{UserID}`
  
- **Get Full Post Details by ID**  
  `GET /api/Posts/FullById/{PostID}`
  
- **Edit a Post**  
  `PUT /api/Posts/{id}`
  
- **Delete a Post**  
  `DELETE /api/Posts/{id}`
  
- **Vote on a Post**  
  `PUT /api/Posts/VoteById/{PostId}`
  
- **Remove Vote from a Post**  
  `PUT /api/Posts/UnVoteById/{PostId}`

---

## SocialGroup

- **Fetch All Groups**  
  `GET /api/SocialGroup`
  
- **Create a Group**  
  `POST /api/SocialGroup`
  
- **Get Group by ID**  
  `GET /api/SocialGroup/ById/{GroupId}`
  
- **Get Members of a Group**  
  `GET /api/SocialGroup/GetMembers/{GroupId}`
  
- **Add a Member to a Group**  
  `PUT /api/SocialGroup/AddMember/{groupId}`
  
- **Remove a Member from a Group**  
  `PUT /api/SocialGroup/RemoveMember/{groupId}`
  
- **Edit Group Details**  
  `PUT /api/SocialGroup/EditGroup`
  
- **Delete a Group**  
  `DELETE /api/SocialGroup/{groupId}`

---

## Database Schema and Entity Relationships  

### Social Groups and Members  
- A **User** can create multiple **Social Groups**.  
- A **User** can be the **Group Admin** of multiple **Social Groups**.  
- A **User** can be a member of multiple **Social Groups**.  
- A **Social Group** can have many **Members**.  
- A **Social Group** has one **Creator**.  
- A **Social Group** has one **Group Admin**.  

### User Following Relationships  
- A **User** can follow multiple other **Users**.  
- A **User** can be followed by multiple other **Users**.  
- A **User** can block multiple other **Users**.  
- A **User** can be blocked by multiple other **Users**.  

### Chats and Users  
- A **Chat** includes multiple **Users**.  
- A **User** can participate in multiple **Chats**.  

### Posts and Comments  
- A **User** can create multiple **Posts**.  
- A **Post** can have one **Author**.  
- A **Post** can belong to one **Social Group**.  
- A **Post** can have multiple **Comments**.  
- A **Comment** can have one **Author**.  
- A **Comment** can have multiple **Replies**.  

---

## License  
This project is licensed under the [MIT License](LICENSE).  
This project was created for educational purposes as part of a course. It is not intended for production use yet.  

---

## Author  
[Dr. Tomer Chen](https://github.com/Tomerlivechen)  
