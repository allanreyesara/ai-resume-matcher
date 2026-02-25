import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

const protectedRoutes = ["/me", "/documents"];
const authPages = ["/login", "/register"];

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (
    pathname.startsWith("/_next") ||
    pathname.startsWith("/api") ||
    pathname === "/favicon.ico" ||
    /\.(.*)$/.test(pathname)
  ) {
    return NextResponse.next();
  }


  const sessionCookie =
    request.cookies.get("access_token")?.value ||
    request.cookies.get("session")?.value ||
    null;

  const isAuthenticated = Boolean(sessionCookie);

  if (isAuthenticated && authPages.includes(pathname)) {
    return NextResponse.redirect(new URL("/me", request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/:path*"],
};