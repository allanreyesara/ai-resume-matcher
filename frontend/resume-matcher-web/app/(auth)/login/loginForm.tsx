"use client";

import { FormEvent, useState } from "react";

export default function LoginForm() {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);
        setLoading(true);

        const formData = new FormData(e.currentTarget);
        const email = formData.get("email") as string;
        const password = formData.get("password") as string;

        try {
            const res = await fetch("/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include",
                body: JSON.stringify({ email, password }),
            });
            if (!res.ok) {
                if (res.status === 401) {
                    throw new Error("Invalid credentials");
                }
                throw new Error("Login failed");
            }

            const data = await res.json();

            // Store the access token in session storage
            const accessToken = data.accessToken;
            sessionStorage.setItem("accessToken", accessToken);

            window.location.href = "/";
            console.log({email, password});
        } catch (err) {
            setError("Email or password is incorrect. Please try again.");
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
                                Sign in to your account
                            </h1>
                            <form className="space-y-4 md:space-y-6" onSubmit={handleSubmit}>
                                <div>
                                    <label htmlFor="email" className="block mb-2 text-sm font-medium">Your email</label>
                                    <input type="email" name="email" id="email" className="bg-[var(--background)] border border-[var(--muted)] text-[var(--foreground)] placeholder-gray-500 rounded-lg focus:ring-2 focus:ring-[var(--muted)] focus:border-[var(--muted)] block w-full p-2.5" placeholder="name@company.com" required />
                                </div>
                                <div>
                                    <label htmlFor="password" className="block mb-2 text-sm font-medium">Password</label>
                                    <input type="password" name="password" id="password" placeholder="••••••••" className="bg-[var(--background)] border border-[var(--muted)] text-[var(--foreground)] placeholder-gray-500 rounded-lg focus:ring-2 focus:ring-[var(--muted)] focus:border-[var(--muted)] block w-full p-2.5" required />
                                </div>
                                {error && (
                                    <div className="text-red-700 text-sm mt-2 font-medium flex items-center gap-1">
                                        <span>⚠️</span>
                                        {error}
                                    </div>
                                )}
                                {/* Future remember me and forgot password functionality */}
                                {/* <div className="flex items-center justify-between">
                                    <div className="flex items-start">
                                        <div className="flex items-center h-5">
                                            <input id="remember" aria-describedby="remember" type="checkbox" className="w-4 h-4 border border-gray-300 rounded bg-gray-50 focus:ring-3 focus:ring-primary-300 dark:bg-gray-700 dark:border-gray-600 dark:focus:ring-primary-600 dark:ring-offset-gray-800" />
                                        </div>
                                        <div className="ml-3 text-sm">
                                            <label htmlFor="remember" className="text-gray-500 dark:text-gray-300">Remember me</label>
                                        </div>
                                    </div>
                                    <a href="#" className="text-sm font-medium text-primary-600 hover:underline dark:text-primary-500">Forgot password?</a>
                                </div>
                                */}
                                <button type="submit" disabled={loading} className="w-full bg-[var(--muted)] text-white font-medium rounded-lg text-sm px-5 py-2.5 text-center hover:opacity-90 focus:outline-none focus:ring-2 focus:ring-[var(--muted)] transition">{loading ? "Signing in..." : "Sign in"}</button>
                                {loading && (
                                    <p className="text-sm text-[var(--muted)] mt-3 text-center">
                                        ⏳ First login may take a few seconds while the backend wakes up.
                                    </p>
                                )}
                                <p className="text-sm font-light">
                                    Don’t have an account yet? <a href="/register" className="font-medium hover:underline">Sign up</a>
                                </p>
                            </form>
                        </div>
                    </div>
                </div>
            </section>
        </main>

    );
}