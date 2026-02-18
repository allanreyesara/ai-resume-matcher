"use client";

import { BACKEND_URL } from "@/lib/config";
import { useState, FormEvent } from "react";

export default function RegisterForm() {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);
        setLoading(true);

        const formData = new FormData(e.currentTarget);
        const fullName = formData.get("fullName") as string ?? "";
        const email = formData.get("email") as string ?? "";
        const password = formData.get("password") as string ?? "";
        const confirmPassword = formData.get("confirmPassword") as string ?? "";

        if (password !== confirmPassword) {
            setError("Passwords do not match.");
            setLoading(false);
            return;
        }

        try{
            const res = await fetch(`${BACKEND_URL}/auth/register`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                body: JSON.stringify({ fullName, email, password }),
            });
            if (!res.ok) {
                if (res.status === 400) {
                    throw new Error("Invalid registration data");
                }
                throw new Error("Registration failed");
            }

            const data = await res.json();

            window.location.href = "/login";

            console.log({fullName, email, password});
        } catch (err) {
            setError("Registration failed. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <main>
            <section>
                <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto md:h-screen lg:py-0">
                    <a href="#" className="flex items-center mb-6 text-2xl font-semibold">
                        <img className="w-22 h-22 mr-2" src="/AI Logo.png" alt="logo" />
                        Resume Matcher AI    
                    </a>
                    
                    <div className="w-full rounded-lg shadow md:mt-0 sm:max-w-md xl:p-0 bg-[var(--surface)] border border-default">
                        <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
                            <h1 className="text-xl font-bold leading-tight tracking-tight md:text-2xl">
                                Create your account
                            </h1>

                            {error && (<p className="text-sm text-red-700 bg-red-100 border border-red-200 rounded-lg px-3 py-2">{error}</p>)}

                            <form className="space-y-4 md:space-y-6" onSubmit={handleSubmit}>
                                <div>
                                    <label htmlFor="fullName" className="block mb-2 text-sm font-medium">Full Name</label>
                                    <input type="text" name="fullName" id="fullName" className="bg-[var(--background)] border border-[var(--muted)] text-[var(--foreground)] placeholder-gray-500 rounded-lg focus:ring-2 focus:ring-[var(--muted)] focus:border-[var(--muted)] block w-full p-2.5" placeholder="Full Name" required disabled={loading} autoComplete="name"/>
                                </div>
                                <div>
                                    <label htmlFor="email" className="block mb-2 text-sm font-medium">Your email</label>
                                    <input type="email" name="email" id="email" className="bg-[var(--background)] border border-[var(--muted)] text-[var(--foreground)] placeholder-gray-500 rounded-lg focus:ring-2 focus:ring-[var(--muted)] focus:border-[var(--muted)] block w-full p-2.5" placeholder="name@ycompany.com" required disabled={loading} autoComplete="email"/>
                                </div>
                                <div>
                                    <label htmlFor="password" className="block mb-2 text-sm font-medium">Password</label>
                                    <input type="password" name="password" id="password" className="bg-[var(--background)] border border-[var(--muted)] text-[var(--foreground)] placeholder-gray-500 rounded-lg focus:ring-2 focus:ring-[var(--muted)] focus:border-[var(--muted)] block w-full p-2.5" placeholder="••••••••" required disabled={loading} autoComplete="new-password"/>
                                </div>
                                <div>
                                    <label htmlFor="confirmPassword" className="block mb-2 text-sm font-medium">Confirm Password</label>
                                    <input type="password" name="confirmPassword" id="confirmPassword" className="bg-[var(--background)] border border-[var(--muted)] text-[var(--foreground)] placeholder-gray-500 rounded-lg focus:ring-2 focus:ring-[var(--muted)] focus:border-[var(--muted)] block w-full p-2.5" placeholder="Confirm Password" required disabled={loading} autoComplete="new-password"/>
                                </div>
                                <button type="submit" disabled={loading} className="w-full bg-[var(--muted)] text-white font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:opacity-90 focus:outline-none focus:ring-2 focus:ring-[var(--muted)] transition disabled:opacity-60" >
                                {loading ? "Creating Account..." : "Create Account"}
                                </button>

                                <p className="text-sm font-light">
                                    Already have an account?{" "}
                                    <a href="/login" className="font-medium hover:underline">Sign in</a>
                                </p>
                            </form>
                        </div>
                    </div>
                </div>
            </section>
        </main>
    );
}
