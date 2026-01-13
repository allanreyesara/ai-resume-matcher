import Link from "next/link";

export default function PublicLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <>
      <header>
        <nav className="fixed w-full z-20 top-0 start-0 border-b border-default bg-[var(--surface)] text-[var(--foreground)]">
          <div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto px-4">
            <a href="/" className="flex items-center space-x-3 rtl:space-x-reverse">
              <img src="https://i.ibb.co/VYbDctLY/Chat-GPT-Image-12-ene-2026-19-10-03-removebg-preview.png" className="h-20 w-20 object-contain" alt="Resume Matcher logo" />
              <span className="self-center text-xl font-semibold whitespace-nowrap">
                Resume Matcher
              </span>
            </a>

            <div className="flex items-center md:order-2 space-x-3 md:space-x-0 rtl:space-x-reverse">
              <button type="button" className="h-9 w-9 rounded-full overflow-hidden ring-1 ring-default hover:ring-[var(--muted)] focus:outline-none focus:ring-2 focus:ring-[var(--muted)] flex items-center justify-center" id="user-menu-button"aria-expanded="false"
                data-dropdown-toggle="user-dropdown" data-dropdown-placement="bottom" >
                <span className="sr-only">Open user menu</span>
                <img className="h-full w-full object-cover" src="https://i.ibb.co/1fJXSmGR/abstract-user-flat-1.png" alt="user" />
              </button>
              <div className="z-50 hidden border border-default-medium rounded-base shadow-lg w-44 bg-[var(--surface)] text-[var(--foreground)]" id="user-dropdown">
                <div className="px-4 py-3 text-sm border-b border-default">
                  <span className="block font-medium">Joseph McFall</span>
                  <span className="block truncate opacity-80">name@flowbite.com</span>
                </div>
                <ul className="p-2 text-sm font-medium" aria-labelledby="user-menu-button">
                  <li>
                    <a href="#" className="inline-flex items-center w-full p-2 hover:bg-[var(--muted)]/20 rounded">
                      Dashboard
                    </a>
                  </li>
                  <li>
                    <a href="#" className="inline-flex items-center w-full p-2 hover:bg-[var(--muted)]/20 rounded">
                      Sign out
                    </a>
                  </li>
                </ul>
              </div>

              <button data-collapse-toggle="navbar-user" type="button" className="inline-flex items-center p-2 w-10 h-10 justify-center text-sm rounded-base md:hidden hover:bg-[var(--muted)]/20 focus:outline-none focus:ring-2 focus:ring-[var(--muted)]" aria-controls="navbar-user" aria-expanded="false" >
                <span className="sr-only">Open main menu</span>
                <svg className="w-6 h-6" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24">
                  <path stroke="currentColor" strokeLinecap="round" strokeWidth="2" d="M5 7h14M5 12h14M5 17h14"/>
                </svg>
              </button>
            </div>

            <div className="items-center justify-between hidden w-full md:flex md:w-auto md:order-1" id="navbar-user">
              <ul className="font-medium flex flex-col p-4 md:p-0 mt-4 border border-default rounded-base bg-[var(--surface)] md:flex-row md:space-x-8 rtl:space-x-reverse md:mt-0 md:border-0">
                <li><a href="#" className="block py-2 px-3 hover:opacity-80">Home</a></li>
                <li><a href="#" className="block py-2 px-3 hover:opacity-80">About</a></li>
                <li><a href="#" className="block py-2 px-3 hover:opacity-80">Services</a></li>
                <li><a href="#" className="block py-2 px-3 hover:opacity-80">Pricing</a></li>
                <li><a href="#" className="block py-2 px-3 hover:opacity-80">Contact</a></li>
              </ul>
            </div>
          </div>
        </nav>
      </header>

      <main className="pt-20 min-h-screen bg-[var(--background)] text-[var(--foreground)]">
        {children}
      </main>

      <footer className="border-t text-center py-4 text-sm opacity-80">
        <p>© {new Date().getFullYear()} ResumeMatcher</p>
      </footer>
    </>
  );
}