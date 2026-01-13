
export default function PublicLayout({
  children,
}: {
  children: React.ReactNode;
}) {
    return (    
    <main>
        <section>
            <header>
                <nav className="fixed w-full z-20 top-0 start-0 border-b border-default bg-[var(--surface)] text-[var(--foreground)]">
                    <div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto px-4">
                        <a href="/" className="flex items-center space-x-3">
                        <img src="https://i.ibb.co/VYbDctLY/Chat-GPT-Image-12-ene-2026-19-10-03-removebg-preview.png" className="h-20 w-20 object-contain" alt="Resume Matcher logo" />
                        <span className="text-xl font-semibold whitespace-nowrap">
                            Resume Matcher
                        </span>
                        </a>
                        <div className="hidden md:flex absolute left-1/2 -translate-x-1/2" id="navbar-user">
                            <ul className="flex space-x-8 font-medium">
                                <li><a href="/" className="hover:opacity-80">Home</a></li>
                                <li><a href="#" className="hover:opacity-80">About</a></li>
                                <li><a href="#" className="hover:opacity-80">Services</a></li>
                                <li><a href="#" className="hover:opacity-80">Contact</a></li>
                            </ul>
                        </div>
                    </div>
                </nav>
            </header>
        </section>
        <section>
            {children}
        </section>
    </main>
    );
}