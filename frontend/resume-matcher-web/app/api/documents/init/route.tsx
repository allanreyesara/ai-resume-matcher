import { NextResponse } from "next/server";

export async function POST(req: Request) {
  const body = await req.json();

  const auth = req.headers.get("authorization") ?? "";

  const apiRes = await fetch("http://localhost:5162/documents/init", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(auth ? { Authorization: auth } : {}),
    },
    body: JSON.stringify(body),
    credentials: "include",
  });

  const text = await apiRes.text();
  const res = new NextResponse(text, { status: apiRes.status });

  const setCookies = (apiRes.headers as any).getSetCookie?.() ?? [];
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