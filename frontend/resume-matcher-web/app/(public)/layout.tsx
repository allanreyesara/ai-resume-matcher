import Link from "next/link";
import Header from "@/components/Header";

import React from "react";

export default function PublicLayout({ 
  children,
}: {
  children: React.ReactNode;
}) { 
  return (
    <>
      <header>
        <Header />
      </header>

      <main className="pt-20 min-h-screen bg-[var(--background)] text-[var(--foreground)] overflow-x-hidden">
        {children}
      </main>

      <footer className="border-t text-center py-4 text-sm opacity-80">
        <p>© {new Date().getFullYear()} ResumeMatcher</p>
      </footer>
    </>
  );
}