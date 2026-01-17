import InfoRow from "@/components/InfoRow";

export default function ProfileCard({ me }: { me: { id: string; email: string; fullName: string; createdAt: string; } }) {
    return(
        <div className="w-full max-w-5xl mx-auto mt-16 bg-[var(--surface)] border border-default rounded-3xl shadow-xl overflow-hidden">
            <div className="grid grid-cols-1 md:grid-cols-3">
                <div className="bg-gradient-to-br from-indigo-600 to-blue-500 p-10 text-white flex flex-col items-center justify-center">
                    <div className="h-36 w-36 rounded-full bg-white/20 flex items-center justify-center text-6xl font-bold mb-6">
                        {me.fullName.charAt(0).toUpperCase()}
                    </div>
                    <h1 className="text-3xl font-bold text-center">{me.fullName}</h1>
                    <p className="opacity-90 mt-1 text-sm">{me.email}</p>
                    </div>
                    <div className="md:col-span-2 p-10 flex flex-col justify-between">
                        <div>
                            <h2 className="text-2xl font-semibold mb-6">Profile Overview</h2>

                            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-8">
                            <div className="bg-[var(--background)] rounded-xl p-4">
                                <InfoRow
                                label="Member since"
                                value={new Date(me.createdAt).toLocaleDateString()}
                                />
                            </div>

                            <div className="bg-[var(--background)] rounded-xl p-4">
                                <InfoRow label="Account status" value="Active" />
                            </div>
                        </div>
                    </div>
                    <div className="flex flex-col sm:flex-row gap-4">
                        <button className="flex-1 py-4 rounded-xl text-lg font-semibold bg-indigo-600 text-white hover:bg-indigo-700 transition shadow">
                        Upload resume
                        </button>

                        <button className="flex-1 py-4 rounded-xl text-lg font-semibold border border-default hover:bg-[var(--muted)]/10 transition">
                        View matches
                        </button>
                    </div>
                </div>
            </div>
        </div>

    );
}