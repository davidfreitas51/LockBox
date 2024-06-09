# LockBox

LockBox is a password manager that communicates with a REST API to handle user and password CRUD operations. Sessions are stored in cookies and JWT tokens, while passwords are encrypted using Hash and AES, in addition to email verification via code. Both the website and API are part of the project.

## Technologies
Some of the technologies adopted in the whole project are:
- C# / .NET
- ASP.NET Core
- API REST
- HTML5 and CSS3
- SQL Server
- EF Core

### Patterns:
- MVC for the website
- DDD for the API
- Unity of works
- Independency Injection
- SOLID and Clean Code principles

### Safety Measures:
- JWT Tokens
- Password Hashing for user credentials
- User accounts encrypted with AES
- Filters Implemented
- Cookie-Based Sessions
- ASP.NET Identity-based users

## Installation

1. In your Visual Studio or preferred IDE, pull the repository. 
2. Run the "LockBoxAPI" project
3. Run the "LockBox" project

You can do this by configuring multi-project startup
If any doubts, check the demonstration video 

## Usage

LockBox simplifies the management of passwords by providing a secure platform for storing and accessing sensitive login information. Here's how to use it effectively:

1. **Register/Login**: Begin by creating a new account or logging in to an existing one. You may also use the recruiter account if you prefer not to create a personal account.

2. **Email Verification**: To validate your account, LockBox verifies user email address via code. This ensures that only valid users can access the account and manage passwords.

3. **Managing Passwords**: Once in, users can create, view, edit and delete their saved credentials as needed. The interface offers intuitive controls for managing passwords and accounts, also offering a safe password generator.

4. **Enhanced Security**: LockBox employs various security measures to safeguard user data. This includes JWT tokens for session management, password hashing to protect user credentials, and AES encryption for encrypting stored passwords. Additionally, filters are implemented to enhance security and streamline data retrieval.


## Demonstration

Here is a video demonstration of the application's use: [Watch the video](https://youtu.be/ArwHsVdXbE0)
