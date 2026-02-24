# AI Resume Matcher

**AI Resume Matcher** is a full-stack web application that uses artificial intelligence to analyze resumes and match candidates with relevant job opportunities based on skills, experience, and overall profile alignment.

The project is designed as a real-world, production-oriented system, built incrementally with a clear separation between frontend and backend concerns.

> 🚧 **MVP completed — active development continues**

---

## Project Overview

The goal of AI Resume Matcher is to reduce friction in job searching by:

- Analyzing resumes using AI techniques  
- Comparing candidate profiles against job requirements  
- Producing match scores, summaries, and actionable insights  

The system is intentionally built with production architecture in mind, focusing on scalability, reliability, and clarity rather than demo-style shortcuts.

---

## Architecture

The project follows a **full-stack, service-oriented architecture** split into two layers.

### Frontend

- Built with **Next.js (App Router)** and **Tailwind CSS**
- Uses design tokens via CSS variables for consistent theming
- Implements the landing page, resume management UI, and matching workflow
- Handles authenticated requests and client-side state

### Backend

- Built with **.NET 8** and **ASP.NET Core Web API**
- Uses **Entity Framework Core** with **PostgreSQL (Supabase)**
- JWT-based authentication and protected endpoints
- Implements the full AI matching pipeline

Backend pipeline includes:

1. Resume text extraction  
2. Text normalization and chunking  
3. Embedding generation via OpenAI  
4. Vector similarity search  
5. LLM-based scoring and reasoning  
6. Match summarization  

---

## Tech Stack

### Frontend

- Next.js (App Router)
- TypeScript
- Tailwind CSS
- CSS variables (design tokens)

### Backend

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL (Supabase)
- OpenAI API (embeddings + LLM scoring)
- JWT Authentication

---

## Current Status

✔ Landing page and UI implemented  
✔ Resume upload and storage pipeline  
✔ Full AI matching pipeline working end-to-end  
✔ Semantic scoring and summaries generated via LLM  
✔ Protected API endpoints  

The application is now a functional MVP capable of performing real resume-to-job analysis.

---

## Planned Improvements

- Job description auto-cleaning (noise reduction)
- Match history and dashboard
- Recruiter-side job posting support
- Improved explainability of match results
- Deployment pipeline and monitoring

---

## Development Philosophy

- Real-world architecture over demo shortcuts  
- Strong separation of concerns  
- Maintainability and clarity first  
- Incremental delivery of working features  
- AI used as a reasoning layer, not a gimmick  

This repository showcases practical full-stack AI engineering and system design.

---

## License

MIT License
