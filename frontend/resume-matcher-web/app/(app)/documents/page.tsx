"use client";   

import { apiFetch } from "@/lib/apiFetch";
import { BACKEND_URL } from "@/lib/config";
import {  useEffect, useMemo, useState } from "react";
import { fetchUserDocuments, type UserDocument, type DocumentKind } from "@/lib/documents";

function formatDate(iso: string) {
    const date = new Date(iso);
    return Number.isNaN(date.getTime()) ? iso : date.toLocaleDateString();
}

function kindBadge(kind: DocumentKind) {
    const base =  "inline-flex items-center rounded-full px-2.5 py-1 text-xs font-semibold border";
    switch (kind) {
        case "Resume":
            return (
                <span className={`${base} border-indigo-200 bg-indigo-50 text-indigo-700`}>
                    Resume
                </span>
            );
        case "CoverLetter":
            return (
                <span className={`${base} border-amber-200 bg-amber-50 text-amber-700`}>
                    Cover Letter
                </span>
            );
        default:
            return (
                <span className={`${base} border-slate-200 bg-slate-50 text-slate-700`}>
                    Other
                </span>
            );
    }
}

export default function DocumentsPage() {
    const [documents, setDocuments] = useState<UserDocument[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [busyId, setBusyId] = useState<string | null>(null);

    const canUseStorage = useMemo(() => typeof window !== "undefined", []);

    async function load(){
        try {
            setLoading(true);
            setError(null);
            const data = await fetchUserDocuments();
            setDocuments(data);
        } catch (e: any) {
            setDocuments([]);
            setError(e?.message ?? "Failed to load documents");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        load();
    }, []);
    
    async function onDownload(documentId: string) {
        try {
            setBusyId(documentId);
            setError(null);

            const res = await apiFetch(`${BACKEND_URL}/documents/${documentId}/download-url`, {
                method: "GET",
                cache: "no-store",
            });

            if (!res.ok) throw new Error(await res.text());

            const data = (await res.json()) as { signedDownloadUrl: string};

                
            if (!data?.signedDownloadUrl) throw new Error("Invalid response from server");

            window.open(data.signedDownloadUrl, "_blank", "noopener,noreferrer");
        } catch (e: any) {
            setError(e?.message ?? "Failed to download document");
        } finally {
            setBusyId(null);
        }
    }

    async function onSetDefault(documentId: string) {
        try {
            setBusyId(documentId);
            setError(null);

            const res = await apiFetch(`${BACKEND_URL}/documents/${documentId}/set-default`, {
                method: "POST",
            });

            if (!res.ok && res.status !== 204) throw new Error(await res.text());

            await load();
        } catch (e: any) {
            setError(e?.message ?? "Failed to set default document");
        } finally {
            setBusyId(null);
        }
    }

    async function onDelete(documentId: string) {
        const ok = confirm("Are you sure you want to delete this document? This action cannot be undone.");
        if (!ok) return;
        try {
            setBusyId(documentId);
            setError(null);

            const res = await apiFetch(`${BACKEND_URL}/documents/${documentId}`, {
                method: "DELETE",
            });

            if (!res.ok && res.status !== 204) throw new Error(await res.text());

            setDocuments(prev => prev.filter(d => d.id !== documentId));

            await load();
        } catch (e: any) {
            setError(e?.message ?? "Failed to delete document");
        } finally {
            setBusyId(null);
        }
    }


    return (
        <main>
            <section>
            <div className="flex flex-col items-center justify-start px-6 py-10 mx-auto lg:py-12 max-w-6xl">
                <div className="w-full text-center">
                <h1 className="text-3xl md:text-4xl font-extrabold tracking-tight">
                    Your documents
                </h1>
                <p className="mt-2 text-sm opacity-80">
                    Manage your resumes and cover letters.
                </p>
                </div>

                {/* Main Card */}
                <div className="w-full rounded-2xl shadow md:mt-8 bg-[var(--surface)] border border-default">
                {/* Header actions */}
                <div className="p-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                    <div className="flex items-center gap-3">
                    <span className="text-sm font-semibold">Documents</span>
                    {!loading && (
                        <span className="text-xs opacity-70">({documents.length})</span>
                    )}
                    </div>

                    <div className="flex gap-3 flex-wrap">
            
                    <button
                        onClick={load}
                        className="border border-default font-medium rounded-lg text-sm px-5 py-2.5 hover:bg-[var(--background)] transition"
                    >
                        Refresh
                    </button>
                    </div>
                </div>

                <div className="border-t border-default" />

                {/* Error */}
                {error && (
                    <div className="px-6 pt-6">
                    <div className="text-red-700 text-sm font-medium flex items-center gap-1">
                        <span>⚠️</span>
                        {error}
                    </div>
                    </div>
                )}

                {/* Content */}
                <div className="p-6">
                    {loading ? (
                    <div className="space-y-3">
                        <div className="h-14 rounded-lg bg-black/5 animate-pulse" />
                        <div className="h-14 rounded-lg bg-black/5 animate-pulse" />
                        <div className="h-14 rounded-lg bg-black/5 animate-pulse" />
                    </div>
                    ) : documents.length === 0 ? (
                    <div className="rounded-lg border border-default bg-[var(--background)] p-10 text-center">
                        <h2 className="text-lg font-semibold">No documents yet</h2>
                        <p className="opacity-80 mt-2">
                        Upload your first resume to start matching jobs.
                        </p>

                        <div className="mt-6 flex justify-center">
                        <a
                            href="/documents/upload"
                            className="bg-[var(--muted)] text-white font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:opacity-90 focus:outline-none focus:ring-2 focus:ring-[var(--muted)] transition"
                        >
                            Upload document
                        </a>
                        </div>
                    </div>
                    ) : (
                    <div className="space-y-4">
                        {documents.map((doc) => (
                        <div
                            key={doc.id}
                            className={`rounded-lg border border-default bg-[var(--background)] p-5 ${
                            doc.isDefault ? "ring-2 ring-[var(--muted)]/40" : ""
                            }`}
                        >
                            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                            {/* Left */}
                            <div className="min-w-0">
                                <div className="flex items-center gap-3 flex-wrap">
                                <p className="font-semibold truncate max-w-[520px]">
                                    {doc.originalFileName}
                                </p>

                                {kindBadge(doc.kind)}

                                {doc.isDefault && (
                                    <span className="inline-flex items-center rounded-full px-2.5 py-1 text-xs font-semibold border border-emerald-200 bg-emerald-50 text-emerald-700">
                                    Default
                                    </span>
                                )}
                                </div>

                                <p className="text-sm opacity-80 mt-1">
                                Uploaded: {formatDate(doc.uploadedAt)}
                                </p>
                            </div>

                            {/* Actions */}
                            <div className="flex gap-3 flex-wrap">
                                <button
                                onClick={() => onDownload(doc.id)}
                                disabled={busyId === doc.id}
                                className="border border-default font-medium rounded-lg text-sm px-5 py-2.5 hover:bg-[var(--surface)] transition disabled:opacity-60"
                                >
                                {busyId === doc.id ? "…" : "Download"}
                                </button>

                                {!doc.isDefault && (
                                <button
                                    onClick={() => onSetDefault(doc.id)}
                                    disabled={busyId === doc.id}
                                    className="bg-[var(--muted)] text-white font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:opacity-90 focus:outline-none focus:ring-2 focus:ring-[var(--muted)] transition disabled:opacity-60"
                                >
                                    {busyId === doc.id ? "…" : "Set default"}
                                </button>
                                )}

                                <button
                                onClick={() => onDelete(doc.id)}
                                disabled={busyId === doc.id}
                                className="bg-red-600 text-white font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-400 transition disabled:opacity-60"
                                >
                                {busyId === doc.id ? "…" : "Delete"}
                                </button>
                            </div>
                            </div>
                        </div>
                        ))}
                    </div>
                    )}
                </div>
                </div>
            </div>
            </section>
        </main>
    );
}