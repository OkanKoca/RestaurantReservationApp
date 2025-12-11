# Restaurant Reservation Application 🍽️

A modern **restaurant reservation system** built with **ASP.NET Core 8 Web API**, **ASP.NET Core 8 MVC**  and **Entity Framework Core 9**.

The application covers the full flow:

- Guest can book a table without an account
- Registered users can manage their own reservations
- Admins manage tables, menus, users and reservations
- Plus **real-time notifications (SignalR)**, **caching (Redis)** and **async messaging (RabbitMQ)** integrations to mimic a production-ready system.

---

## Overview

This project is designed as a **full-stack .NET reservation system** that you can show in interviews as a realistic example of:

- Authentication & authorization (Identity + roles + JWT)
- CRUD operations with a relational data model (EF Core)
- Admin vs user vs guest flows
- Real-time communication (SignalR)
- Caching with Redis
- Asynchronous processing with RabbitMQ

---

## Core Features

### User Management 👤

- User registration & login via **ASP.NET Core Identity**
- Role-based authorization:
  - `Admin`
  - `Customer` / normal user
- **JWT authentication** support for API / future integrations:
  - Token generation on login
  - Token validation for protected endpoints

---

### Reservation System 📅

- Table booking for **registered users**
- **Guest reservations** without creating an account
- Reservation lifecycle:
  - `Pending`
  - `Confirmed`
  - `Outdated` (automatically set when reservation time is in the past)
- Table availability checks to prevent conflicting bookings
- Users can:
  - See all their reservations (past + upcoming)
  - Cancel upcoming reservations before the reservation date

---

### Admin Panel 🔧

- Central dashboard with key metrics:
  - Total reservations
  - Upcoming reservations
  - Outdated / pending reservations
- Reservation management:
  - Confirm / reject / delete reservations
  - Filter reservations by status (Pending, Confirmed, Outdated, etc.)
- User management:
  - List all users
  - Grant / revoke admin role
  - Delete user accounts
- All admin pages protected by **role-based authorization**

---

### Real-time Notifications (SignalR)

The application integrates **SignalR** to push real-time updates without page refresh:

- When a new reservation is created:
  - Admin dashboard can receive **instant notifications**
- When an admin changes a reservation’s status (e.g. Pending → Confirmed):
  - The relevant user can be **notified in real time**
- SignalR hubs are used to:
  - Separate admin and user channels
  - Broadcast events like *reservation created* / *status changed*

This demonstrates how to add **real-time capabilities** on top of a classical Razor Pages app.

---

### Caching (Redis)

The project uses **Redis** as a distributed cache to improve performance and reduce database load:

- Frequently read data such as:
  - Menus (foods & drinks)
  - Table definitions
- are cached in Redis behind a clean abstraction.

Typical behaviours:

- On read:
  - Check Redis cache first
  - Fallback to database if not present
- On write/update:
  - Update DB
  - Invalidate or refresh related cache keys

---

### Async Messaging (RabbitMQ)

The application integrates **RabbitMQ** for asynchronous, decoupled operations:

- When certain events happen (for example):
  - Reservation created
  - Reservation confirmed/cancelled
- The app publishes messages to RabbitMQ (e.g. `ReservationCreated`, `ReservationStatusChanged`).

A background worker / consumer can:

- Listen to these messages
- Trigger side effects such as:
  - Sending email notifications
  - Logging / auditing
  - Future integration with external services

---

## Tech Stack 💻

### Backend

- **.NET 8**
- **ASP.NET Core 8 (Razor Pages)**
- **Entity Framework Core 9**
- **ASP.NET Core Identity** (roles, users)
- **JWT Bearer Authentication**
- **SQLite** (development database)
- **SignalR** for real-time communication
- **Redis** for distributed caching
- **RabbitMQ** for messaging

### Frontend

- Razor Pages
- Bootstrap 5
- Font Awesome
- HTML5, CSS3, JavaScript

---
