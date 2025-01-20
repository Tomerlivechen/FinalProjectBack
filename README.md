
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
  - [User Management](#user-management)  
  - [Social Actions](#social-actions)  
  - [Chat](#chat)  
  - [Comments](#comments)  
  - [Images](#images)  
  - [Notifications](#notifications)  
  - [Posts](#posts)  
  - [Social Groups](#social-groups)  
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
###Hosted Website
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

### User Management  
- **Register a New User**: `POST /api/Auth/register`  
- **Get All Users**: `GET /api/Auth/GetUsers`  
- **Inactivate/Reactivate User by ID**: `DELETE /api/Auth/ToggleInactivationById/{userId}`  

### Social Actions  
- **Follow/Unfollow Users**:  
  - `PUT /api/Auth/follow`  
  - `PUT /api/Auth/unfollow`  
- **Block/Unblock Users**:  
  - `PUT /api/Auth/block`  
  - `PUT /api/Auth/unblock`  

### Chat  
- **Create a Chat**: `POST /api/Chat`  
- **Send a Message**: `POST /api/Chat/Message`  

### Comments  
- **Create, Edit, or Delete Comments**:  
  - `POST /api/Comments`  
  - `PUT /api/Comments/{id}`  
  - `DELETE /api/Comments/{id}`  

### Images  
- **Upload an Image**: `POST /api/Image`  
- **Manage Images**:  
  - Toggle Privacy: `PUT /api/Image/togglePrivatebyId/{imageId}`  

### Notifications  
- **Fetch All Notifications**: `GET /api/Notification`  

### Posts  
- **Create a Post**: `POST /api/Posts`  
- **Get Posts**: `GET /api/Posts`  

### Social Groups  
- **Create a Group**: `POST /api/SocialGroup`  

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
This project is distributed for educational purposes as part of a course. It is not intended for production use.  

---

## Author  
[Dr. Tomer Chen](https://github.com/Tomerlivechen)  
