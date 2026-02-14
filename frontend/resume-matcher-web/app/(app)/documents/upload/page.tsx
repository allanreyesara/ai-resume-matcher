"use client";

import {  useState } from "react";

type InitResponse = {
    documentId: string;
    bucket?: string;
    path: string;
    signedUploadUrl: string;
};

async function sha256(file: File): Promise<string> {
    const buf = await file.arrayBuffer();
    const hashBuffer = await crypto.subtle.digest("SHA-256", buf);
    const hashArray = Array.from(new Uint8Array(hashBuffer));
    return hashArray.map(b => b.toString(16).padStart(2, "0")).join("");
}

export default function UploadResumePage(){
    const [file, setFile] = useState<File | null>(null);
    const [setAsDefault, setSetAsDefault] = useState(true);

    const [isLoggedIn, setIsLoggedIn] = useState(true);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [status, setStatus] = useState<string>("");


    const validateFile = (file: File) => {
        const maxMb =10;
        const allowedExt = [".pdf", ".doc", ".docx", ".txt"];
        const ext = "." + (file.name.split(".").pop() ?? "").toLowerCase();


        if (!allowedExt.includes(ext)) throw new Error("Unsupported file type. Allowed: PDF, DOC, DOCX, TXT.");
        if (file.size > maxMb * 1024 * 1024) throw new Error(`File size exceeds ${maxMb}MB limit.`);
    };

    const handleSubmit = async () => {
        if (!file) return;
        setError(null);
        setStatus("");
        setLoading(true);

        try {
            validateFile(file);

            const accessToken = sessionStorage.getItem("accessToken");
            if (!accessToken) {
                setIsLoggedIn(false);
                throw new Error("You must be logged in to upload a resume.");
                
            }
                        

            setStatus("Requesting upload URL...");
            const initRes = await fetch("/api/documents/init", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${accessToken}`
                },
                body: JSON.stringify({
                    originalFileName: file.name,
                    mimeType: file.type || "application/octet-stream",
                    kind: 0, // resume
                    setAsDefault,
            }),     
        });

        const initRaw = await initRes.text();

        if (!initRes.ok) {
            throw new Error(initRaw || "Failed to get upload URL.");
        }

        const init = JSON.parse(initRaw) as InitResponse;

        setStatus("Uploading file...");
        const put = await fetch(init.signedUploadUrl, {
        method: "PUT",
        body: file,
        });

        if (!put.ok) {
        const errText = await put.text().catch(() => "");
        throw new Error(`Upload failed: ${errText || put.status}`);
        }

        setStatus("Calculating file hash...");
        const hash = await sha256(file);

        setStatus("Finalizing upload...");
        const finalizeRes = await fetch(`/api/documents/${init.documentId}/finalize`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${accessToken}`
            },
            body: JSON.stringify({
                sizeBytes: file.size,
                sha256Hash: hash,
            }),
        });

        const finalizeRaw = await finalizeRes.text();

        if (!finalizeRes.ok) {
            throw new Error(finalizeRaw || "Failed to finalize upload.");
        }

        setStatus("Upload successful!");
        setFile(null);
        } catch (err: any) {
            setError(err.message || "An unexpected error occurred.");
            setStatus("");
        } finally {
            setLoading(false);
        }
        
    };

    return (
  <main className="bg-[var(--background)] text-[var(--foreground)] pt-16">
    <section className="max-w-xl mx-auto px-6 py-14">

      <h1 className="text-2xl sm:text-3xl font-extrabold tracking-tight">
        Upload your resume
      </h1>

      <p className="mt-3 text-sm opacity-75">
        Supported formats: PDF, DOC, DOCX, TXT · Max 10MB
      </p>

      {/* Card */}
      <div className="mt-8 rounded-2xl bg-[var(--surface)]/25 border border-[var(--muted)]/20 p-6">

        {/* File input */}
        <input
          type="file"
          disabled={loading}
          accept=".pdf,.doc,.docx,.txt,application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,text/plain"
          onChange={(e) => setFile(e.target.files?.[0] ?? null)}
          className="block w-full text-sm file:mr-4 file:py-2 file:px-4 
                     file:rounded-lg file:border-0
                     file:bg-[var(--muted)] file:text-white
                     hover:file:opacity-90"
        />

        {/* Selected file */}
        {file && (
          <div className="mt-4 text-sm opacity-80">
            Selected: <span className="font-semibold">{file.name}</span>{" "}
            ({(file.size / (1024 * 1024)).toFixed(2)} MB)
          </div>
        )}

        {/* Checkbox */}
        <label className="flex items-center gap-2 mt-4 text-sm">
          <input
            type="checkbox"
            checked={setAsDefault}
            disabled={loading}
            onChange={(e) => setSetAsDefault(e.target.checked)}
            className="accent-[var(--muted)]"
          />
          Set as default resume
        </label>

        {/* Button */}
        <button
          onClick={handleSubmit}
          disabled={!file || loading}
          className="mt-6 w-full px-6 py-3 rounded-lg font-semibold
                     bg-[var(--muted)] text-white
                     hover:opacity-90 transition
                     disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {loading ? "Uploading..." : "Upload Resume"}
        </button>

        {/* Status */}
        {status && !error && (
          <p className="mt-4 text-sm opacity-75">{status}</p>
        )}

        {/* Error */}
        {error && (
            <div className="mt-5 rounded-lg border border-red-300 bg-red-50 p-4 text-sm text-red-700">
                <p className="font-medium">{error}</p>

                {isLoggedIn === false && (
                <a
                    href="/login"
                    className="inline-block mt-2 font-semibold text-red-700 underline hover:text-red-900"
                >
                    Go to sign in →
                </a>
                )}
            </div>
        )}



      </div>

    </section>
  </main>
);
};