"use client";

import { useEffect, useState } from "react";
import { ChevronDown } from "lucide-react";
import LogOutButton from "@/app/(auth)/logout/logOutButton";

export default function Header() {
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [open, setOpen] = useState(false);

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
                <li className="relative">
                  {isLoggedIn ? (
                    <>
                      <button onClick={() => setOpen(!open)} className="cursor-pointer flex items-center gap-1 py-2 px-3 hover:opacity-80">
                        <span>Profile</span>
                        <ChevronDown size={16} className={`transition-transform ${open ? "rotate-180" : ""}`}/>
                      </button>

                      {open && (
                        <div className="absolute right-0 mt-2 w-44 rounded-md shadow-lh border border-default bg-[var(--surface)]">
                          <a href="/me" className="block px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-400">
                            My Profile
                          </a>
                          <a href="/documents/upload" className="block px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-400">
                            Upload Resume
                          </a>
                          <a href="/documents" className="block px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-400">
                            My Resumes
                          </a>
                          <div className="block px-4 py-2 text-sm hover:bg-gray-100 dark:hover:bg-gray-400" onClick={() => setOpen(false)}>
                            <LogOutButton />
                          </div>
                        </div>
                      )}
                    </>
                  ): (<a href="/login" className="block py-2 px-3 hover:opacity-40">
                    Login
                    </a>
                  )}
                </li>
              </ul>
            </div>
          </div>
        </nav>
    );
}