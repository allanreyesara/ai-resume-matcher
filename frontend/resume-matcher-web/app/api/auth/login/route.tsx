import { BACKEND_URL } from "@/lib/config";
import { NextResponse } from "next/server";

export async function POST(req: Request) {
  const body = await req.json();

  const apiRes = await fetch(`${BACKEND_URL}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    credentials: "include",
  });

  const text = await apiRes.text(); 
  const res = new NextResponse(text, { status: apiRes.status });

  // set-cookie
  const setCookies = apiRes.headers.getSetCookie?.() ?? [];
  if (setCookies.length > 0) {
    for (const c of setCookies) res.headers.append("set-cookie", c);
  } else {
    const single = apiRes.headers.get("set-cookie");
    if (single) res.headers.set("set-cookie", single);
  }

  const ct = apiRes.headers.get("content-type");
  if (ct) res.headers.set("content-type", ct);

  return res;
}