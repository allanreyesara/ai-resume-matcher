import Link from "next/link";
export default function HomePage() {
  return (
    <main className="bg-[var(--background)] text-[var(--foreground)] overflow-hidden pt-16">
      <section>
        <div className="grid max-w-screen-xl mx-auto px-6 py-14 lg:py-16 lg:grid-cols-12 lg:gap-10 items-center">
          <div className="mr-auto place-self-center lg:col-span-7">
            <h1 className="max-w-xl mb-6 text-4xl font-extrabold tracking-tight leading-tight md:text-5xl xl:text-6xl">
              Find the jobs that match your resume — instantly.
            </h1>

            <p className="max-w-lg mb-8 text-base opacity-90 md:text-lg lg:text-xl">
              Our AI analyzes your resume and connects you with real job opportunities
              based on skills, experience, and relevance — no guessing, no wasted applications.
            </p>

            <div className="flex flex-wrap items-center gap-4">
              <a href="/documents/match" className="inline-flex items-center justify-center px-6 py-3 text-base font-semibold rounded-lg bg-[var(--muted)] text-white hover:opacity-90 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/40 transition">
                Analyze Job Posting
              </a>

              <a
                href="/me"
                className="inline-flex items-center justify-center px-6 py-3 text-base font-medium rounded-lg border border-[var(--muted)] text-[var(--foreground)] hover:bg-[var(--muted)]/15 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/30 transition">
                Access Account
              </a>
            </div>
            <p className="mt-4 text-sm opacity-60">
              PDF & DOCX supported • Instant AI match score
            </p>
          </div>

          <div className="hidden lg:col-span-5 lg:flex lg:items-center lg:justify-end">
            <div>
              <div className="relative overflow-hidden rounded-[280px] shadow-lg">
                <img src="/HeroLanding.png" alt="AI Resume Matcher hero" className="max-w-[650px] h-auto" />
              </div>
            </div>
          </div>
        </div>
      </section>
      <section className="border-t border-[var(--muted)]/20 bg-[var(--surface)]/20">
      <div className="max-w-screen-xl mx-auto px-6 py-12">
        <div className="grid gap-10 md:grid-cols-3 text-center">
          
          <div className="flex flex-col items-center">
            <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-[var(--surface)] ring-1 ring-[var(--muted)]/25 shadow-sm">
              <svg className="h-6 w-6 text-[var(--foreground)]" viewBox="0 0 24 24" fill="none" aria-hidden="true">
                <path d="M10.5 18a7.5 7.5 0 1 1 0-15 7.5 7.5 0 0 1 0 15Z" stroke="currentColor" strokeWidth="2" />
                <path d="M16.5 16.5 21 21" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
              </svg>
            </div>
            <h3 className="text-sm font-semibold tracking-tight">Instant Analysis</h3>
            <p className="mt-2 text-sm opacity-75 max-w-xs">
              Get your match score in seconds.
            </p>
          </div>

          <div className="flex flex-col items-center">
            <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-[var(--surface)] ring-1 ring-[var(--muted)]/25 shadow-sm">
              <svg className="h-6 w-6 text-[var(--foreground)]" viewBox="0 0 24 24" fill="none" aria-hidden="true">
                <path d="M7 3h7l3 3v15H7V3Z" stroke="currentColor" strokeWidth="2" strokeLinejoin="round" />
                <path d="M14 3v4h4" stroke="currentColor" strokeWidth="2" strokeLinejoin="round" />
                <path d="m9 14 2 2 4-5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
              </svg>
            </div>
            <h3 className="text-sm font-semibold tracking-tight">Key Skills Comparison</h3>
            <p className="mt-2 text-sm opacity-75 max-w-xs">
              See how your skills align with the job requirements.
            </p>
          </div>

          <div className="flex flex-col items-center">
            <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-[var(--surface)] ring-1 ring-[var(--muted)]/25 shadow-sm">
              <svg className="h-6 w-6 text-[var(--foreground)]" viewBox="0 0 24 24" fill="none" aria-hidden="true">
                <path d="M12 21a9 9 0 1 1 9-9" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                <path d="M12 17a5 5 0 1 1 5-5" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                <path d="M12 13a1 1 0 1 0 0-2 1 1 0 0 0 0 2Z" fill="currentColor" />
                <path d="m20 4-7 7" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                <path d="m17 4h3v3" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
              </svg>
            </div>
            <h3 className="text-sm font-semibold tracking-tight">Job Fit Suggestions</h3>
            <p className="mt-2 text-sm opacity-75 max-w-xs">
              Receive tips to improve your resume.
            </p>
          </div>

        </div>
      </div>
    </section>
    <section id="services" aria-label="How it works">
        <div className="max-w-screen-xl mx-auto px-6 py-14 sm:py-16">
          <div className="max-w-2xl">
            <h2 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
              How it works
            </h2>
            <p className="mt-3 text-base sm:text-lg opacity-80">
              Three simple steps to get better matches and apply with confidence.
            </p>
          </div>

          <div className="mt-10 grid gap-6 lg:grid-cols-3">
            <div className="rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-6">
              <div className="flex items-center gap-3">
                <span className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-[var(--muted)] text-white text-sm font-bold">
                  1
                </span>
                <h3 className="font-semibold">Upload your resume</h3>
              </div>
              <p className="mt-3 text-sm opacity-80">
                Drop a PDF or DOCX. We extract your skills, experience, and keywords.
              </p>
            </div>
            <div className="rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-6">
              <div className="flex items-center gap-3">
                <span className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-[var(--muted)] text-white text-sm font-bold">
                  2
                </span>
                <h3 className="font-semibold">AI analyzes your profile</h3>
              </div>
              <p className="mt-3 text-sm opacity-80">
                We compare your resume to job requirements and calculate a match score.
              </p>
            </div>
            <div className="rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-6">
              <div className="flex items-center gap-3">
                <span className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-[var(--muted)] text-white text-sm font-bold">
                  3
                </span>
                <h3 className="font-semibold">Get ranked job matches</h3>
              </div>
              <p className="mt-3 text-sm opacity-80">
                See top matches, missing skills, and suggestions to improve your resume.
              </p>
            </div>
          </div>
        </div>
      </section>
      <section id="contact" aria-label="Call to action" className="border-t border-[var(--muted)]/20">
        <div className="max-w-screen-xl mx-auto px-6 py-14 sm:py-16">
          <div className="rounded-3xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-8 sm:p-10 flex flex-col lg:flex-row lg:items-center lg:justify-between gap-6">
            <div className="max-w-2xl">
              <h2 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
                Ready to find better job matches?
              </h2>
              <p className="mt-3 text-base sm:text-lg opacity-80">
                Upload your resume and get an instant AI match score, ranked opportunities,
                and actionable tips to improve your chances.
              </p>
            </div>

            <div className="flex flex-col sm:flex-row gap-3">
              <Link href="/" className="inline-flex items-center justify-center px-6 py-3 text-base font-semibold rounded-lg bg-[var(--muted)] text-white hover:opacity-90 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/40 transition" >
                Upload Resume
              </Link>
              <Link href="/" className="inline-flex items-center justify-center px-6 py-3 text-base font-medium rounded-lg border border-[var(--muted)] text-[var(--foreground)] hover:bg-[var(--muted)]/15 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/30 transition" >
                Sign in
              </Link>
            </div>
          </div>
          <div className="mt-8 flex flex-wrap gap-x-6 gap-y-2 text-sm opacity-70">
            <span>🔒 Privacy-first</span>
            <span>⚡ Results in seconds</span>
            <span>🎯 Skill-based ranking</span>
          </div>
        </div>
      </section>
    </main>
  );
}