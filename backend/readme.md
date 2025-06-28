# Expense Tracker API

A robust and scalable backend API for a modern expense tracking application, built with **ASP.NET Core** and designed for high performance and maintainability. This API provides comprehensive features for user management, expense tracking, data analysis, and AI-driven financial insights.

---

## 📚 Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [API Architecture](#api-architecture)
- [Security](#security)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation & Setup](#installation--setup)
- [API Documentation](#api-documentation)
- [Observability](#observability)
  - [Logging](#logging)
  - [Performance Monitoring](#performance-monitoring)
- [Testing](#testing)

---

## 🚀 Features

This API provides a complete backend solution with a rich set of features:

- **Full User Authentication**  
  Secure user registration, login, and session management using JWT Access & Refresh Token flow.

- **Expense Management**  
  Full CRUD operations with soft deletion to preserve historical data.

- **Dynamic Category Management**  
  Supports both predefined and user-defined categories for flexibility.

- **Advanced Querying**  
  Filtering (date range, category, payment method), searching, sorting, and pagination supported on all list endpoints.

- **Data-Rich Dashboard Endpoints**  
  - Monthly total expenses  
  - Category-wise breakdown (pie chart)
  - Historical spending trends (line chart)
  - Spending by day of the week  
  - Monthly comparisons  

- **AI-Powered Insights**  
  Integrates with AI services ( Google Gemini) to generate personalized financial summaries.

- **Real-Time Notifications**  
  Powered by SignalR to push live events like budget alerts or summaries to clients.

---

## 🛠️ Technology Stack

| Layer           | Technology           |
|----------------|----------------------|
| Framework      | ASP.NET Core 8       |
| Database       | PostgreSQL           |
| ORM            | Entity Framework Core|
| Auth           | JWT (Access + Refresh)|
| Logging        | Serilog              |
| Docs           | Swagger / Excel      |
-----------------------------------------

---

## 🧱 API Architecture

Built on a **clean N-Tier architecture** for maintainability and testability:

- **Repository Pattern**: Abstracts all data access logic.
- **Service Layer**: Contains all business logic.
- **Dependency Injection**: Promotes loose coupling throughout the application.
- **AutoMapper**: Transforms Entities ⇄ DTOs.
- **FluentValidation**: Elegant, fluent request validation.

---

## 🔐 Security

- **HTTPS/TLS 1.2+** enforced by default.
- **JWT Authentication** with short-lived access tokens and refresh tokens.
- **Role-Based Authorization** (currently a single "User" role, easily extendable).
- **BCrypt Password Hashing** for secure password storage.
- **CORS Policy**: Strictly allows only whitelisted origins.
- **Rate Limiting**: Default 1000 requests/hour per user to prevent abuse.

---

## 🚀 Getting Started

### ✅ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [PostgreSQL](https://www.postgresql.org/) (or Docker)
- IDE (e.g., Visual Studio, VS Code, Rider)

### ⚙️ Installation & Setup

1. **Clone the repository:**

   ```bash
   git clone <your-repository-url>
   cd ExpenseTracker.API
