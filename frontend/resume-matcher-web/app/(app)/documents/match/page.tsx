"use client";

import { useEffect, useMemo, useState } from "react";
import { fetchUserDocuments, type UserDocument } from "@/lib/documents";
import { useJobMatch } from "@/lib/hooks/useJobMatch";

export default function MatchesPage() {
  const [jobDescription, setJobDescription] = useState("");
  const [selectedResumeId, setSelectedResumeId] = useState<string>("");

  const [documents, setDocuments] = useState<UserDocument[]>([]);
  const [docsLoading, setDocsLoading] = useState(true);
  const [docsError, setDocsError] = useState<string | null>(null);

  const { running, error: matchError, result, run } = useJobMatch();

  const resumes = useMemo(
    () => documents.filter((d) => d.kind === "Resume"),
    [documents]
  );

  const canRun =
    !!selectedResumeId &&
    jobDescription.trim().length > 0 &&
    !docsLoading &&
    !running;

  useEffect(() => {
    let cancelled = false;

    (async () => {
      try {
        setDocsLoading(true);
        setDocsError(null);

        const data = await fetchUserDocuments();
        if (!data) return;
        if (cancelled) return;

        setDocuments(data);

        const defaultResume = data.find((d) => d.kind === "Resume" && d.isDefault);
        const firstResume = data.find((d) => d.kind === "Resume");

        setSelectedResumeId((defaultResume ?? firstResume)?.id ?? "");
      } catch (e: any) {
        if (!cancelled) setDocsError(e?.message ?? "Failed to load documents");
      } finally {
        if (!cancelled) setDocsLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, []);

  async function onRunMatch() {
    if (!canRun) return;

    await run({
      documentId: selectedResumeId,
      jobText: jobDescription,
      topK: 10,
      useLlm: true,
    });
  }

  return (
    <main className="bg-[var(--background)] text-[var(--foreground)] overflow-hidden pt-16">
      <section>
        <div className="max-w-screen-xl mx-auto px-6 py-10 lg:py-12">
          <div className="mb-8">
            <h1 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
              Resume Matcher
            </h1>
            <p className="mt-2 text-sm sm:text-base opacity-80">
              Paste a job description, choose a resume, and run an AI match.
            </p>
          </div>
          <div className="grid gap-6 lg:grid-cols-12">
            <div className="lg:col-span-7 rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-6">
              <div className="flex items-center justify-between gap-3">
                <h2 className="text-base sm:text-lg font-semibold tracking-tight">
                  Job Description
                </h2>
                <span className="text-xs opacity-60">
                  {jobDescription.trim().length} chars
                </span>
              </div>

              <p className="mt-2 text-sm opacity-75">
                Paste the job requirements you want to match against. 
              </p>
              <p className="mt-2 text-sm opacity-75">
                For the most accurate match, include technical requirements, responsibilities, and tools used in the role.
                Removing company culture or promotional text helps the AI focus on real skills and improves scoring accuracy.
              </p>

              <textarea
                value={jobDescription}
                onChange={(e) => setJobDescription(e.target.value)}
                rows={14}
                placeholder="Paste job description here..."
                className="mt-4 w-full resize-y rounded-xl border border-[var(--muted)]/20 bg-[var(--background)]/40 px-4 py-3 text-sm outline-none focus:ring-4 focus:ring-[var(--muted)]/30"
              />
            </div>

            {/* Resume Selector */}
            <div className="lg:col-span-5 rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-6">
              <h2 className="text-base sm:text-lg font-semibold tracking-tight">
                Select Resume
              </h2>
              <p className="mt-2 text-sm opacity-75">
                Choose which resume you want to evaluate.
              </p>

              <div className="mt-4">
                {docsLoading && (
                  <p className="text-sm opacity-75">Loading documents...</p>
                )}

                {docsError && (
                  <p className="text-sm text-red-400">{docsError}</p>
                )}

                {!docsLoading && resumes.length === 0 && (
                  <p className="text-sm opacity-75">No resumes uploaded yet.</p>
                )}

                {!docsLoading && resumes.length > 0 && (
                  <div className="space-y-2">
                    {resumes.map((r) => (
                      <label
                        key={r.id}
                        className="flex items-start gap-3 rounded-xl border border-[var(--muted)]/20 bg-[var(--background)]/30 px-4 py-3 hover:bg-[var(--muted)]/10 transition cursor-pointer"
                      >
                        <input
                          type="radio"
                          name="resume"
                          className="mt-1"
                          checked={selectedResumeId === r.id}
                          onChange={() => setSelectedResumeId(r.id)}
                        />
                        <div className="min-w-0">
                          <div className="flex flex-wrap items-center gap-2">
                            <span className="text-sm font-medium truncate max-w-[22rem]">
                              {r.originalFileName}
                            </span>
                            {r.isDefault && (
                              <span className="text-xs font-semibold px-2 py-0.5 rounded-full border border-[var(--muted)]/25 bg-[var(--surface)]/40">
                                Default
                              </span>
                            )}
                          </div>
                          <div className="mt-1 text-xs opacity-60">
                            Uploaded:{" "}
                            {new Date(r.uploadedAt).toLocaleDateString()}
                          </div>
                        </div>
                      </label>
                    ))}
                  </div>
                )}
              </div>

              <button
                onClick={onRunMatch}
                disabled={!canRun}
                className="mt-5 inline-flex w-full items-center justify-center px-6 py-3 text-base font-semibold rounded-lg bg-[var(--muted)] text-white hover:opacity-90 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/40 transition disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {running ? "Running analysis..." : "Run Match"}
              </button>

              {matchError && (
                <p className="mt-3 text-sm text-red-400">{matchError}</p>
              )}

              <p className="mt-3 text-xs opacity-60">
                Tip: include responsibilities + requirements for better results.
              </p>
            </div>
          </div>

          {/* Results (NO matches/chunks) */}
          {result && (
            <div className="mt-8 rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-6">
              <div className="flex flex-col sm:flex-row sm:items-end sm:justify-between gap-3">
                <div>
                  <h2 className="text-base sm:text-lg font-semibold tracking-tight">
                    Match Result
                  </h2>
                  <p className="mt-1 text-sm opacity-75">
                    Summary + overall score for this job vs resume.
                  </p>
                </div>

                <div className="text-sm opacity-75">
                  <span className="font-semibold opacity-100">Processing:</span>{" "}
                  {result.meta?.processingTimeMs ?? 0} ms
                </div>
              </div>

              <div className="mt-5 grid gap-6 lg:grid-cols-12">
                <div className="lg:col-span-4 rounded-2xl border border-[var(--muted)]/20 bg-[var(--background)]/30 p-5">
                  <div className="text-xs uppercase tracking-wide opacity-60">
                    Overall Score
                  </div>
                  <div className="mt-2 text-4xl font-extrabold tracking-tight">
                    {Math.round(result.overallScorePercent)}%
                  </div>
                  <div className="mt-3 text-xs opacity-60">
                    TopK: {result.topK} • LLM: {result.usedLlm ? "On" : "Off"} •
                    Version: {result.meta?.pipelineVersion ?? "-"}
                  </div>
                </div>

                <div className="lg:col-span-8 rounded-2xl border border-[var(--muted)]/20 bg-[var(--background)]/30 p-5">
                  <div className="text-xs uppercase tracking-wide opacity-60">
                    Summary
                  </div>
                  <p className="mt-2 text-sm opacity-90 whitespace-pre-wrap">
                    {result.summary ?? "No summary returned."}
                  </p>
                </div>
              </div>
            </div>
          )}
        </div>
      </section>
    </main>
  );
}