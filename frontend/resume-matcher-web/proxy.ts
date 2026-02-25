import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

const protectedRoutes = ["/me", "/documents"];
const publicRoutes = ["/", "/login", "/register", "/about", "/services"];

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  const isProtectedRoute = protectedRoutes.some((r) => pathname.startsWith(r));
  const isPublicRoute = publicRoutes.includes(pathname);

  const token = request.cookies.get("refresh_token")?.value;
  const isAuthenticated = Boolean(token);

  if (isProtectedRoute && !isAuthenticated) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  if (isAuthenticated && (pathname === "/login" || pathname === "/register")) {
    return NextResponse.redirect(new URL("/me", request.url));
  }

  if (isPublicRoute) {
    return NextResponse.next();
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|.*\\.(png|jpg|jpeg|gif|svg|ico)$).*)"],
};