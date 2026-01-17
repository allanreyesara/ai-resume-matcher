import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

const protectedRoutes = ["/me"];
const publicRoutes = ["/", "/login", "/signup"];

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  const isProtectedRoute = protectedRoutes.some((r) => pathname.startsWith(r));
  const isPublicRoute = publicRoutes.includes(pathname);

  const token = request.cookies.get("refresh_token")?.value;

  const isAuthenticated = Boolean(token);

  if (isProtectedRoute && !isAuthenticated) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!api|_next/static|_next/image|.*\\.png$).*)"],
};