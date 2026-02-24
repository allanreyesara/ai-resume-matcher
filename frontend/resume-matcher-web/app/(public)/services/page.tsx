export default function ServicesPage() {
  return (
    <main className="bg-[var(--background)] text-[var(--foreground)] pt-16">
      <div className="max-w-screen-xl mx-auto px-6 min-w-0 [&_a]:no-underline">
        {/* Section 1 */}
        <section className="py-14 sm:py-16">
          <div className="max-w-3xl min-w-0">
            <h1 className="text-3xl sm:text-4xl font-extrabold tracking-tight">
              What the platform does
            </h1>
            <p className="mt-4 text-base sm:text-lg opacity-80 break-words [overflow-wrap:anywhere]">
              AI Resume Matcher analyzes your resume and job descriptions using
              semantic AI to reveal real alignment, skill gaps, and opportunities.
            </p>
          </div>

          <div className="mt-10 grid gap-6 lg:grid-cols-3 min-w-0">
            <div className="rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-7 min-w-0">
              <h3 className="font-semibold text-lg">Semantic Resume Analysis</h3>
              <p className="mt-3 text-sm opacity-80 break-words [overflow-wrap:anywhere]">
                Your resume is parsed into structured text and analyzed using
                embeddings to capture meaning, not just keywords.
              </p>
            </div>

            <div className="rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-7 min-w-0">
              <h3 className="font-semibold text-lg">AI Match Scoring</h3>
              <p className="mt-3 text-sm opacity-80 break-words [overflow-wrap:anywhere]">
                Job descriptions are compared against your profile using vector
                search and LLM reasoning to produce a realistic match percentage.
              </p>
            </div>

            <div className="rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-7 min-w-0">
              <h3 className="font-semibold text-lg">Evidence-Based Insights</h3>
              <p className="mt-3 text-sm opacity-80 break-words [overflow-wrap:anywhere]">
                Instead of generic feedback, the system explains exactly why a
                match is strong or weak and highlights missing requirements.
              </p>
            </div>
          </div>
        </section>

        {/* Section 2 */}
        <section className="py-14 sm:py-16">
          <div className="max-w-2xl min-w-0">
            <h2 className="text-2xl font-semibold tracking-tight">
              How the system works
            </h2>
            <p className="mt-3 text-sm sm:text-base opacity-80 break-words [overflow-wrap:anywhere]">
              The platform combines multiple AI techniques to simulate how a
              technical recruiter evaluates candidates.
            </p>
          </div>

          <div className="mt-10 grid gap-6 lg:grid-cols-2 min-w-0">
            {[
              {
                step: "Step 1",
                title: "Text extraction",
                desc: "Resume content is normalized and structured for AI processing.",
              },
              {
                step: "Step 2",
                title: "Semantic chunking",
                desc: "Both resume and job description are split into meaningful, contextual segments.",
              },
              {
                step: "Step 3",
                title: "Vector matching",
                desc: "Embeddings detect semantic similarity between candidate evidence and job requirements.",
              },
              {
                step: "Step 4",
                title: "LLM reasoning",
                desc: "AI evaluates evidence, identifies gaps, and generates the final score and summary.",
              },
            ].map((x) => (
              <div
                key={x.step}
                className="rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-7 min-w-0"
              >
                <div className="text-xs font-semibold uppercase tracking-wider opacity-60">
                  {x.step}
                </div>
                <h3 className="mt-2 text-lg font-semibold">{x.title}</h3>
                <p className="mt-2 text-sm opacity-80 break-words [overflow-wrap:anywhere]">
                  {x.desc}
                </p>
              </div>
            ))}
          </div>
        </section>

        {/* Section 3 */}
        <section className="pb-16">
          <div className="rounded-3xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-10 min-w-0">
            <h2 className="text-2xl font-semibold">Why this matters</h2>
            <p className="mt-4 text-sm sm:text-base opacity-80 max-w-3xl break-words [overflow-wrap:anywhere]">
              Job searching is often inefficient because candidates don’t know
              how their profile maps to requirements. By revealing alignment and
              skill gaps, the platform helps you prioritize learning,
              applications, and career decisions strategically.
            </p>
          </div>
        </section>
      </div>
    </main>
  );
}