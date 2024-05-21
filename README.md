# LockBox

LockBox is a password manager that communicates with a REST API to handle user and password CRUD operations.
Sessions are stored in cookies and JWT tokens, while passwords are encrypted using Hash and AES, in addition to
email verification via code. Both the website and API are part of the project.

## Technologies
Some of the technologies adopted in the whole are:
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
 
### Safety:
- JWT Tokens
- Password hash
- Filters
- Cookie-based sessions

The project follows SOLID principles and Clean Code, ensuring easy maintenance and scalability if needed.

## Installation

In your Visual Studio or preferred IDE, pull the repository. After that, run the "ClinicaDaMulher" project.

## Usage

The program facilitates the management of appointments, clients and appointment reasons, enabling creation, reading, editing, searching, sorting, and deletion of data as necessary. To schedule an appointment, it is necessary to first register reasons and clients.

## Demonstration

Here is a video demonstration of the application's use:
[Watch the video](https://youtu.be/6vFmhOXRRKM)
