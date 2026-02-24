export default function AboutPage() {
  return (
    <main className="bg-[var(--background)] text-[var(--foreground)] pt-16 overflow-x-hidden">
      <section className="py-14 sm:py-16">
        <div className="max-w-screen-xl mx-auto px-6">
          <div className="grid gap-10 lg:grid-cols-12 items-start">
            <div className="lg:col-span-7">
              <h1 className="text-3xl sm:text-4xl md:text-5xl font-extrabold tracking-tight leading-tight">
                About AI Resume Matcher
              </h1>

              <p className="mt-4 text-base sm:text-lg opacity-85 max-w-2xl">
                AI Resume Matcher helps you understand how your experience aligns with real job requirements -
                using semantic analysis, not simple keyword scanning.
              </p>

              <p className="mt-4 text-sm sm:text-base opacity-80 max-w-2xl">
                Instead of guessing which roles are “a fit”, it compares your resume evidence against a job description and returns
                a match score plus clear gaps you can improve.
              </p>

              <div className="mt-6 flex flex-wrap gap-2 text-xs">
                <span className="px-3 py-1 rounded-full border border-[var(--muted)]/25 bg-[var(--surface)]/20">
                  Semantic embeddings
                </span>
                <span className="px-3 py-1 rounded-full border border-[var(--muted)]/25 bg-[var(--surface)]/20">
                  Evidence-based scoring
                </span>
                <span className="px-3 py-1 rounded-full border border-[var(--muted)]/25 bg-[var(--surface)]/20">
                  Skill gaps
                </span>
                <span className="px-3 py-1 rounded-full border border-[var(--muted)]/25 bg-[var(--surface)]/20">
                  LLM summary
                </span>
              </div>
            </div>

            <div className="lg:col-span-5">
              <div className="rounded-3xl bg-[var(--surface)]/20 border border-[var(--muted)]/20 p-7">
                <div className="text-xs uppercase tracking-wider opacity-60">In one sentence</div>
                <p className="mt-2 text-sm sm:text-base opacity-85">
                  A practical tool that turns your resume + a job description into a measurable fit score and actionable next steps.
                </p>

                <div className="mt-6 grid gap-3">
                  <div className="rounded-2xl border border-[var(--muted)]/20 bg-[var(--background)]/35 p-4">
                    <div className="text-xs opacity-60">Best for</div>
                    <div className="mt-1 text-sm font-semibold">Targeted applications & skill planning</div>
                  </div>
                  <div className="rounded-2xl border border-[var(--muted)]/20 bg-[var(--background)]/35 p-4">
                    <div className="text-xs opacity-60">Output</div>
                    <div className="mt-1 text-sm font-semibold">Score + summary + gaps (evidence-based)</div>
                  </div>
                  <div className="rounded-2xl border border-[var(--muted)]/20 bg-[var(--background)]/35 p-4">
                    <div className="text-xs opacity-60">Goal</div>
                    <div className="mt-1 text-sm font-semibold">Less guessing, more clarity</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div className="mt-12 rounded-3xl bg-[var(--surface)]/15 border border-[var(--muted)]/20 p-8">
            <div className="grid gap-6 lg:grid-cols-3">
              <div>
                <div className="text-xs uppercase tracking-wider opacity-60">The problem</div>
                <p className="mt-2 text-sm sm:text-base opacity-85">
                  Job ads are noisy and keyword-heavy. Candidates waste time applying without knowing what actually matches.
                </p>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wider opacity-60">The approach</div>
                <p className="mt-2 text-sm sm:text-base opacity-85">
                  Extract resume evidence, compare against job requirements semantically, and score based on what’s present vs missing.
                </p>
              </div>
              <div>
                <div className="text-xs uppercase tracking-wider opacity-60">The result</div>
                <p className="mt-2 text-sm sm:text-base opacity-85">
                  A realistic fit score and clear gaps so you can improve the resume or prioritize learning.
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="pb-14 sm:pb-16">
        <div className="max-w-screen-xl mx-auto px-6">
          <div className="max-w-2xl">
            <h2 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
              Design principles
            </h2>
            <p className="mt-3 text-sm sm:text-base opacity-80">
              The product is built to be simple, realistic, and useful - not a “magic AI” gimmick.
            </p>
          </div>

          <div className="mt-10 grid gap-6 md:grid-cols-2 lg:grid-cols-4">
            {[
              {
                title: "Evidence over hype",
                desc: "Scores must reflect what your resume actually contains - no invented experience.",
              },
              {
                title: "Useful outputs",
                desc: "Not just a number. You get a summary and the biggest gaps you can fix.",
              },
              {
                title: "Production mindset",
                desc: "Built like a real SaaS system: clear layers, APIs, and scalable patterns.",
              },
              {
                title: "Clarity first",
                desc: "UI stays clean so the results are easy to understand and act on.",
              },
            ].map((x) => (
              <div
                key={x.title}
                className="rounded-2xl bg-[var(--surface)]/20 border border-[var(--muted)]/20 p-6"
              >
                <h3 className="text-base font-semibold">{x.title}</h3>
                <p className="mt-2 text-sm opacity-80">{x.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>
      <section className="pb-14 sm:pb-16">
        <div className="max-w-screen-xl mx-auto px-6">
          <div className="max-w-2xl">
            <h2 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
              How it works
            </h2>
            <p className="mt-3 text-sm sm:text-base opacity-80">
              Under the hood, the pipeline mirrors how a technical reviewer checks fit — with structure and traceability.
            </p>
          </div>

          <div className="mt-10 grid gap-6 lg:grid-cols-12">
            <div className="lg:col-span-7 space-y-4">
              {[
                {
                  step: "Step 1",
                  title: "Extract & normalize",
                  desc: "Resume text is extracted, cleaned, and normalized so the model compares consistent signals.",
                },
                {
                  step: "Step 2",
                  title: "Chunk into evidence",
                  desc: "Both resume and job description are split into meaningful segments (skills, experience, responsibilities).",
                },
                {
                  step: "Step 3",
                  title: "Semantic match",
                  desc: "Embeddings + vector search find the best evidence alignments - beyond exact keyword matches.",
                },
                {
                  step: "Step 4",
                  title: "LLM scoring & summary",
                  desc: "An LLM scores relevance based on evidence and produces a short, actionable summary.",
                },
              ].map((x) => (
                <div
                  key={x.step}
                  className="rounded-2xl bg-[var(--surface)]/20 border border-[var(--muted)]/20 p-7"
                >
                  <div className="text-xs font-semibold uppercase tracking-wider opacity-60">
                    {x.step}
                  </div>
                  <h3 className="mt-2 text-lg font-semibold">{x.title}</h3>
                  <p className="mt-2 text-sm opacity-80">{x.desc}</p>
                </div>
              ))}
            </div>
            <div className="lg:col-span-5">
              <div className="rounded-3xl bg-[var(--surface)]/15 border border-[var(--muted)]/20 p-7">
                <h3 className="text-lg font-semibold">What your score means</h3>
                <p className="mt-3 text-sm opacity-80">
                  The percentage is an estimate of alignment based on the strongest evidence found across your resume.
                  It’s not a guarantee - it’s a signal to help you decide what to improve next.
                </p>

                <div className="mt-6 space-y-3">
                  <div className="rounded-2xl border border-[var(--muted)]/20 bg-[var(--background)]/35 p-4">
                    <div className="text-xs opacity-60">High score</div>
                    <div className="mt-1 text-sm font-semibold">
                      You have concrete evidence matching key requirements.
                    </div>
                  </div>
                  <div className="rounded-2xl border border-[var(--muted)]/20 bg-[var(--background)]/35 p-4">
                    <div className="text-xs opacity-60">Low score</div>
                    <div className="mt-1 text-sm font-semibold">
                      The job asks for signals that aren’t clearly present in your resume.
                    </div>
                  </div>
                  <div className="rounded-2xl border border-[var(--muted)]/20 bg-[var(--background)]/35 p-4">
                    <div className="text-xs opacity-60">Best action</div>
                    <div className="mt-1 text-sm font-semibold">
                      Add missing evidence or target a closer role.
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div className="mt-10 rounded-3xl bg-[var(--surface)]/15 border border-[var(--muted)]/20 p-8">
            <h3 className="text-lg font-semibold">Privacy & control</h3>
            <p className="mt-2 text-sm opacity-80 max-w-3xl">
              Your resume is analyzed to generate matches and insights. The system is designed to keep outputs practical and
              avoid hallucinating experience - it should only score what it can justify from resume evidence.
            </p>
          </div>
        </div>
      </section>
      <section className="pb-16">
        <div className="max-w-screen-xl mx-auto px-6">
          <div className="rounded-3xl bg-[var(--surface)]/20 border border-[var(--muted)]/20 p-8 sm:p-10 flex flex-col lg:flex-row lg:items-center lg:justify-between gap-6">
            <div className="max-w-2xl">
              <h2 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
                Ready to test your resume against a job post?
              </h2>
              <p className="mt-3 text-sm sm:text-base opacity-80">
                Upload your resume, paste a job description, and get an evidence-based score in seconds.
              </p>
            </div>

            <div className="flex flex-col sm:flex-row gap-3">
              <a
                href="/documents/upload"
                className="inline-flex items-center justify-center px-6 py-3 text-base font-semibold rounded-lg bg-[var(--muted)] text-white hover:opacity-90 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/40 transition"
              >
                Upload Resume
              </a>
              <a
                href="/documents/match"
                className="inline-flex items-center justify-center px-6 py-3 text-base font-medium rounded-lg border border-[var(--muted)] text-[var(--foreground)] hover:bg-[var(--muted)]/15 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/30 transition"
              >
                Analyze Job
              </a>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}