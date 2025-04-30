# StaffTrack

**StaffTrack** is a simple internal tool built for tracking weekly reports submitted by staff and national service personnel in a tech firm. It replaces the use of Google Forms with a more modern and structured web-based system.

## Features

- User login and registration (admin-created accounts)
- Weekly report submission with file upload
- Department and staff role management
- Admin dashboard (coming soon)

## Tech Stack

- ASP.NET Core Web API
- Razor Pages (UI)
- Entity Framework Core
- SQL Server

## Getting Started

1. Clone the repo  
   `git clone https://github.com/maame-efua1/StaffTrack.git`

2. Update `appsettings.json` with your database connection string.

3. Run migrations and update the database:  
   `dotnet ef database update`

4. Start the app:  
   `dotnet run`

---

### Future Plans

- Admin dashboard for managing users and reports
- Email notifications for new account creation
- Report analytics

---

Feel free to fork, contribute, or raise issues!

