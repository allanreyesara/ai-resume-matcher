# AI Resume Matcher

**AI Resume Matcher** is a full-stack web application that uses artificial intelligence to analyze resumes and match candidates with relevant job opportunities based on skills, experience, and overall profile alignment.

The project is designed as a real-world, production-oriented system, built incrementally with a clear separation between frontend and backend concerns.

> 🚧 **Work in progress** — the project is under active development and evolving continuously.

---

## Project Overview

The goal of AI Resume Matcher is to reduce the friction in job searching by:
- Analyzing resumes using AI techniques
- Comparing candidate profiles against job requirements
- Providing ranked job matches and actionable insights

This repository reflects an iterative development approach, prioritizing solid architecture, clarity, and scalability over premature optimization.

---

## Architecture

The project is split into two main parts:

### Frontend
- Built with **Next.js (App Router)** and **Tailwind CSS**
- Focused on user experience, clarity, and responsive design
- Uses CSS variables as design tokens for consistent theming
- Currently implements the public landing page and UI foundations

### Backend
- Built with **.NET 8** and **ASP.NET Core Web API**
- Uses **Entity Framework Core** and **PostgreSQL (Supabase)**
- JWT-based authentication in progress
- Designed to support resume processing, AI matching, and secure APIs

---

## Tech Stack

### Frontend
- Next.js (App Router)
- TypeScript
- Tailwind CSS
- CSS Variables (design tokens)

### Backend
- .NET 8
- ASP.NET Core
- Entity Framework Core
- PostgreSQL (Supabase)
- JWT Authentication

---

## Current Status

- Frontend landing page implemented
- Shared design system in place
- Backend infrastructure and database configured
- Authentication flow under development
- Core AI matching features planned and scoped

---

## Planned Features

- User authentication and authorization
- Resume upload and parsing
- AI-powered resume-to-job matching
- Match scoring and ranked job recommendations
- User dashboard and history
- Secure and documented REST API

---

## Development Philosophy

- Incremental, real-world development
- Clear separation of concerns
- Readability and maintainability over shortcuts
- No mock features or placeholder business logic

This repository is meant to showcase practical full-stack engineering rather than a polished, finished product.

---

## License

MIT License
