# AI Resume Matcher

**AI Resume Matcher** is a full-stack AI-powered web application that analyzes resumes and matches candidates with relevant job opportunities based on semantic understanding of skills, experience, and profile alignment — not just keyword matching.

🌐 **Live:** [resumematcher.app](https://resumematcher.app)

---

## What It Does

Most resume matchers rely on keyword overlap. This system uses **semantic embeddings + LLM reasoning** to evaluate how well a candidate's actual experience maps to what a role requires — producing a score, a summary, and actionable insights.

- Upload a resume → get matched against job descriptions
- Receive a semantic match score with LLM-generated reasoning
- Understand *why* you're a strong or weak match, not just whether you are

---

## Architecture

Full-stack, service-oriented — designed for production, not demo shortcuts.

```
Next.js (App Router)  →  ASP.NET Core Web API  →  PostgreSQL (Supabase)
                                    ↓
                         OpenAI Embeddings API
                                    ↓
                         Vector Similarity Search
                                    ↓
                         LLM Scoring + Reasoning
```

### Frontend
- **Next.js (App Router)** + **TypeScript** + **Tailwind CSS**
- Design tokens via CSS variables for consistent theming
- Handles resume management UI, matching workflow, and authenticated requests

### Backend
- **.NET 8 / ASP.NET Core Web API**
- **Entity Framework Core** + **PostgreSQL (Supabase)**
- JWT-based authentication on all protected endpoints

---

## AI Matching Pipeline

The core pipeline runs server-side and processes resumes in sequential stages:

```
1. Text Extraction
2. Normalization & Chunking
3. Embedding Generation  →  OpenAI text-embedding-3-small
4. Vector Similarity Search
5. LLM Scoring & Reasoning  →  GPT-4o
6. Match Summarization
```

**Why this approach:**

- **Chunking before embedding** — resume text is split into semantic sections before embedding to avoid losing signal from long documents that would exceed model context limits.
- **Two-stage evaluation** — vector similarity handles the initial candidate filtering efficiently; LLM scoring is reserved for the final reasoning pass where nuance matters. This keeps latency and token cost manageable.
- **LLM as reasoning layer, not retrieval** — the model doesn't search for matches; it explains them. Embeddings handle similarity, GPT-4o handles *why*.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Next.js 14 (App Router), TypeScript, Tailwind CSS |
| Backend | .NET 8, ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | PostgreSQL via Supabase |
| AI | OpenAI API — embeddings + GPT-4o |
| Auth | JWT (JSON Web Tokens) |

---

## Current Status

| Feature | Status |
|---------|--------|
| Landing page and UI | ✅ Done |
| Resume upload and storage | ✅ Done |
| Full AI matching pipeline | ✅ Done |
| Semantic scoring + LLM summaries | ✅ Done |
| Protected API endpoints | ✅ Done |
| Match history dashboard | 🔄 Planned |
| Recruiter-side job posting | 🔄 Planned |
| Improved match explainability | 🔄 Planned |

---

## Running Locally

```bash
# Backend (.NET 8)
cd backend/ResumeMatcher.Api
dotnet restore
dotnet run

# Frontend (Next.js)
cd frontend
npm install
npm run dev
```

> Copy `.env.example` to `.env` and fill in your OpenAI API key, Supabase connection string, and JWT secret before running.

---

## License

MIT — [Allan Araya Reyes](https://github.com/allanaraya) © 2026
