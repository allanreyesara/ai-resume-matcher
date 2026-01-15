"use client";

import { apiFetch } from "@/lib/apiFetch";
import { useState, useEffect } from "react";
import ProfileSkeleton from "@/components/skeletons/ProfileSkeleton";
import ProfileCard from "@/components/skeletons/ProfileCard";

type MeResponse = {
        id: string;
        email: string;
        fullName: string;
        createdAt: string;
    }

export default function ProfileComponent() {
    const [me, setMe] = useState<MeResponse | null>(null);
    const [isLoading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
    let cancelled = false;  
    async function loadProfile() {
        try{
            const res = await apiFetch("http://localhost:5162/auth/me");

            if (!res.ok){
                throw new Error("Failed to fetch profile data");
            }

            const data = await res.json();
            if (!cancelled) {
                setMe(data);
            }
            
        } catch (err) {
            if (!cancelled) {
                setError("Not authorized");
            }
        } finally {
            if (!cancelled) {
                setLoading(false);
            }
        }
        }
        loadProfile();
        return () => {
            cancelled = true;   
        };
    }, []);

    const createdAtPretty = me?.createdAt ? new Date(me.createdAt).toLocaleDateString() : "";

    return (
        <main className="min-h-screen flex items-center justify-center" style={{background: "var(--background)", color: "var(--foreground)"}}>
            {/*Skeleton*/}
            {isLoading && <ProfileSkeleton />}
            {/* Error */}
            {!isLoading && error && (
                <div className="w-full max-w-md rounded-2xl p-6 border text-center" style={{ background: "var(--surface)", borderColor: "var(--muted)" }}>
                    <h2 className="text-xl font-bold mb-2">Session expired</h2>
                    <p className="opacity-80 mb-4">{error}</p>
                    <a href="/login" className="inline-block px-4 py-2 rounded-lg font-semibold text-white" style={{ background: "var(--muted)" }}>
                    Go to login
                    </a>
                </div>
            )} 

            {!isLoading && me && <ProfileCard me={me} />}
            
        </main>
    );
}