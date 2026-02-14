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
        <button className="cursor-pointer"type="button" onClick={logout} disabled={loading}>Log Out</button>
        
    );
}