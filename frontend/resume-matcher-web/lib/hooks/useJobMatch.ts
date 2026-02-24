"use client";

import { useCallback, useState } from "react";
import { apiFetch } from "@/lib/apiFetch";
import { BACKEND_URL } from "@/lib/config";
import type { MatchResultDto, RunMatchParams } from "@/lib/contracts/match";

export function useJobMatch() {
  const [running, setRunning] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [result, setResult] = useState<MatchResultDto | null>(null);

  const clear = useCallback(() => {
    setError(null);
    setResult(null);
  }, []);

  const run = useCallback(async (params: RunMatchParams) => {
    setRunning(true);
    setError(null);
    setResult(null);

    try {
      const res = await apiFetch(`${BACKEND_URL}/jobs/${params.documentId}/match`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          jobText: params.jobText.trim(),
          topK: params.topK,
          useLlm: params.useLlm,
        }),
      });

      if (!res.ok) throw new Error(await res.text());

      const data = (await res.json()) as MatchResultDto;
      setResult(data);
      return data;
    } catch (e: any) {
      const msg = e?.message ?? "Failed to run match";
      setError(msg);
      throw new Error(msg);
      return null;
    } finally {
      setRunning(false);
    }
  }, []);

  return { running, error, result, run, clear };
}