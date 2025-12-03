# Restaurant Reservation Application 🍽️

A modern ASP.NET Core application for managing restaurant reservations, built with .NET 8.

## Features 🚀

- **User Management 👤**
  - User registration and authentication
  - Role-based authorization (Admin / Customer)
  - JWT Authentication & Profile management

- **Reservation System 📅**
  - Table booking for registered users
  - Guest reservations (without account)
  - Automatic reservation status updates (if it is outdated)
  - Table availability tracking

- **Admin Panel 🔧**
  - Manage reservations, menus, and tables
  - Update reservation status (e.g., Confirmed, Pending)
  - Clear, user-friendly dashboard views

- **Menu Management 🍕🍹**
  - Organize food & drink items
  - Categorize menus
  - Display dietary information, pricing, etc.

## Tech Stack 💻

- **Backend:**
  - ASP.NET Core 8 (Razor Pages)
  - Entity Framework Core 9
  - SQLite database
  - Identity Framework with JWT Bearer Authentication

- **Frontend:**
  - Razor Pages
  - Bootstrap 5 & Font Awesome
  - HTML5, CSS3, and JavaScript
 
## 📸 Screenshots

### 🏠 Home Page
<img src="screenshots/homepage.png" alt="Home Page" width="800"/>


### 📅 Reservation Pages
<p>This page is for guests.</p>
<img src="screenshots/guestreservation.png" alt="Guest Reservation Page" width="800"/>
<p>This page is for users.</p>
<img src="screenshots/userreservation.png" alt="User Reservation Page" width="800"/>

### 👤 User Dashboard
<img src="screenshots/loginpage.png" alt="login Page" width="800"/>
<p>Here users can see their own reservations and cancel them.</p>
<img src="screenshots/myreservations.png" alt="My Reservations Page" width="800"/>

### 🍽️ Menus
<img src="screenshots/allmenus.png" alt="Menus Page" width="800"/>
<img src="screenshots/menu.png" alt="My Reservations Page" width="800"/>

### 👤 Admin Dashboard
<img src="screenshots/adminpanel.png" alt="Admin Dashboard" width="800"/>
<p>Admin can authorize a user or delete their account.</p>
<img src="screenshots/adminusers.png" alt="Admin Users" width="800"/>
<p>Admin can confirm a reservation and delete them.</p>
<img src="screenshots/adminreservations.png" alt="Admin reservations page" width="800"/>
<p>Admin can filter reservations to see only pending, outdated ones.</p>
<img src="screenshots/filteredreservations.png" alt="Admin filtered reservations" width="800"/>
<img src="screenshots/foods.png" alt="Admin foods" width="800"/>
<img src="screenshots/drinks.png" alt="Admin drinks" width="800"/>
<img src="screenshots/adminmenus.png" alt="Admin menus" width="800"/>
<p>Admin can check a table's occupancy, reservations and add, delete table.</p>
<img src="screenshots/tablesandpoccupancies.png" alt="Admin tables and table occupancies" width="800"/>

