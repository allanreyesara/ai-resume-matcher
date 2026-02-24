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

      <main className="pt-20 min-h-screen bg-[var(--background)] text-[var(--foreground)]">
        {children}
      </main>

    </>
  );
}