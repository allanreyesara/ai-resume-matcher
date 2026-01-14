"use client";

import { useState } from "react";

export default function LogOutButton() {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const logout = async () => {
        setLoading(true);
        setError(null);

        try {
            const token = sessionStorage.getItem("accessToken");
            const res = await fetch("http://localhost:5162/auth/logout", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { Authorization: `Bearer ${token}` } : {}),
                },
                body: JSON.stringify({}),
                credentials: "include",
            });

            if (!res.ok) {
                setError("Logout failed. Please try again.");
            }
        }finally{
                sessionStorage.removeItem("accessToken");
                window.location.href = "/login";
            }
    }

    return (
        <li><button type="button" onClick={logout} disabled={loading} className="inline-flex items-center justify-center px-6 py-3 text-base font-medium rounded-lg border border-[var(--muted)] text-[var(--foreground)] hover:bg-[var(--muted)]/15 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/30 transition">Log Out</button></li>
    );
}