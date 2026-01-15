import InfoRow from "@/components/InfoRow";

export default function ProfileCard({ me }: { me: { id: string; email: string; fullName: string; createdAt: string; } }) {
    return(
        <div className="flex flex-col items-center text-center mb-6">
            <div className="h-24 w-24 rounded-full flex items-center justify-center text-3xl font-bold mb-3" style={{ background: "var(--background)", color:"var(--muted)"}}>
                {me.fullName.charAt(0).toUpperCase()}
            </div>
            <h1 className="text-2xl font-bold">{me.fullName}</h1>
            <p className="opacity-80">{me.email}</p>

            <div className="flex flex-col gap-3 mb-6">
                <InfoRow label=" Member Since : " value={new Date(me.createdAt).toLocaleDateString()} />
            </div>
             <div className="flex flex-col gap-3">
                <button className="inline-flex items-center justify-center px-6 py-3 text-base font-medium rounded-lg border cursor-pointer bg-[var(--muted)] border-[var(--muted)] text-[var(--foreground)] hover:bg-[var(--muted)]/15 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/30 transition">
                Upload resume
                </button>

                <button className="inline-flex items-center justify-center px-6 py-3 text-base font-medium rounded-lg border cursor-pointer bg-[var(--muted)] border-[var(--muted)] text-[var(--foreground)] hover:bg-[var(--muted)]/15 focus:outline-none focus:ring-4 focus:ring-[var(--muted)]/30 transition">
                View matches
                </button>
            </div>
        </div>

    );
}