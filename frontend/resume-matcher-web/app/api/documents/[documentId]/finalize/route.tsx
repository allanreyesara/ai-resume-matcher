import { NextResponse } from "next/server";

export async function POST(
  req: Request,
  ctx: { params: Promise<{ documentId: string }> }
) {
  const { documentId } = await ctx.params;

  const body = await req.json();
  const auth = req.headers.get("authorization") ?? "";

  const apiRes = await fetch(`http://localhost:5162/documents/${documentId}/finalize`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(auth ? { Authorization: auth } : {}),
    },
    body: JSON.stringify(body),
  });

  const text = await apiRes.text();
  const res = new NextResponse(text, { status: apiRes.status });

  const ct = apiRes.headers.get("content-type");
  if (ct) res.headers.set("content-type", ct);

  return res;
}
