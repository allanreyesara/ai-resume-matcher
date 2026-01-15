"use client";

import { useEffect, useState } from "react";
import LogOutButton from "@/app/(auth)/logout/logOutButton";

export default function Header() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);

    useEffect(() => {
        const token = sessionStorage.getItem("accessToken");
        setIsLoggedIn(!!token);

        const onStorage = () => {
            const t = sessionStorage.getItem("accessToken");
            setIsLoggedIn(!!t);
        };
        window.addEventListener("storage", onStorage);

        return () => {
            window.removeEventListener("storage", onStorage);
        };
    }, []);
    
    return (
        <nav className="fixed w-full z-20 top-0 start-0 border-b border-default bg-[var(--surface)] text-[var(--foreground)]">
          <div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto px-4">
            <a href="/" className="flex items-center space-x-3 rtl:space-x-reverse">
              <img src="https://i.ibb.co/VYbDctLY/Chat-GPT-Image-12-ene-2026-19-10-03-removebg-preview.png" className="h-20 w-20 object-contain" alt="Resume Matcher logo" />
              <span className="self-center text-xl font-semibold whitespace-nowrap">
                Resume Matcher
              </span>
            </a>
            <div className="items-center justify-between hidden w-full md:flex md:w-auto md:order-1" id="navbar-user">
              <ul className="font-medium flex flex-col p-4 md:p-0 mt-4 border border-default rounded-base bg-[var(--surface)] md:flex-row md:space-x-8 rtl:space-x-reverse md:mt-0 md:border-0">
                <li><a href="/" className="block py-2 px-3 hover:opacity-80">Home</a></li>
                <li><a href="/" className="block py-2 px-3 hover:opacity-80">About</a></li>
                <li><a href="/" className="block py-2 px-3 hover:opacity-80">Services</a></li>
                <li><a href="/" className="block py-2 px-3 hover:opacity-80">Contact</a></li>
                <li><a href="/me" className="block py-2 px-3 hover:opacity-80">Profile</a></li>
                {isLoggedIn ? <LogOutButton /> : <li><a href="/login" className="block py-2 px-3 hover:opacity-80">Login</a></li>}
              </ul>
            </div>
          </div>
        </nav>
    );
}