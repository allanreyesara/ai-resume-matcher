export default function ProfileSkeleton() {
    return (
        <main className="min-h-screen flex items-center justify-center" style={{background: "var(--background)", color: "var(--foreground)",}}>
            <div className="w-full max-w-md rounded-2xl p-6 flex flex-col gap-4 border" style={{background: "var(--surface)", borderColor: "var(--muted)",}}>
                {/* Avatar / header */}
                <div className="h-32 w-full rounded-xl animate-pulse" style={{ background: "var(--background)" }}/>

                {/* Name */}
                <div className="h-4 w-1/2 rounded animate-pulse" style={{ background: "var(--background)" }}/>

                {/* Email */}
                <div className="h-4 w-full rounded animate-pulse" style={{ background: "var(--background)" }}/>

                {/* Extra line */}
                <div className="h-4 w-full rounded animate-pulse" style={{ background: "var(--background)" }}/>
            </div>
            
        </main>
);
}
