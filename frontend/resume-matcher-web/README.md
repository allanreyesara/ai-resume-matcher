AI Resume Matcher – Frontend

Frontend web application for AI Resume Matcher, a platform that uses artificial intelligence to analyze resumes and match candidates with relevant job opportunities based on skills, experience, and relevance.

Built with Next.js (App Router) and Tailwind CSS, focused on performance, clean UI, and scalability.

Features

Modern landing page with clear value proposition

AI-focused user experience for resume analysis and job matching

Fully responsive design

Design system based on CSS variables

Clean, component-based architecture

Ready for authentication and backend integration

Tech Stack

Framework: Next.js (App Router)

Language: TypeScript

Styling: Tailwind CSS

Design Tokens: CSS Variables (globals.css)

Icons: Inline SVG

Assets: Static images from public/

Project Structure

frontend/
└── resume-matcher-web/
    ├── app/
    │   ├── (public)/        # Public routes (landing page)
    │   │   ├── page.tsx
    │   │   └── layout.tsx
    │   ├── (auth)/          # Auth-related routes (planned)
    │   ├── layout.tsx       # Root layout (html / body)
    │   └── globals.css      # Global styles and CSS variables
    ├── public/
    │   ├── HeroLanding.png
    │   └── *.svg
    ├── package.json
    ├── tailwind.config.js
    └── tsconfig.json

Design System

The UI uses CSS variables to keep a consistent and easily maintainable theme:

:root {
  --background: #f4f1e9;
  --foreground: #111827;
  --surface: #ffffff;
  --muted: #5b82c4;
}

These tokens are consumed directly in Tailwind classes:
bg-[var(--background)]
text-[var(--foreground)]
border-[var(--muted)]

Getting Started
Install dependencies

npm install

Run the development server

npm run dev

Open in browser

http://localhost:3000


Current Pages

Landing Page

Hero section with AI illustration

Primary CTAs (Upload Resume / Access Account)

Feature highlights:

Instant Analysis

Key Skills Comparison

Job Fit Suggestions

Backend Integration (Planned)

The frontend is designed to integrate with the AI Resume Matcher API, built with:

.NET 8

JWT Authentication

Resume parsing and AI matching

Secure REST endpoints

Roadmap

Authentication (Login / Register)

Resume upload flow

AI match score visualization

Job recommendations dashboard

User profile and history

Dark mode support

Notes

This project prioritizes clean UI architecture, modern React patterns, and practical Tailwind usage over visual complexity or unnecessary abstractions.

License

MIT License
