"use client";

type MeResponse = {
  id: string;
  email: string;
  fullName: string;
  createdAt: string;
};

export default function ProfileCard({ me }: { me: MeResponse }) {
  const createdAtPretty = me?.createdAt
    ? new Date(me.createdAt).toLocaleDateString()
    : "";

  const initials =
    me?.fullName?.trim()?.[0]?.toUpperCase() ||
    me?.email?.trim()?.[0]?.toUpperCase() ||
    "U";

  return (
    <div className="w-[92%] max-w-6xl xl:max-w-7xl rounded-3xl shadow-lg overflow-hidden border border-black/10">
      <div className="grid grid-cols-1 md:grid-cols-[340px_1fr]">
        
        {/* LEFT PANEL */}
        <div className="bg-indigo-600 text-white p-10 flex flex-col justify-center items-center text-center">
          <div className="w-28 h-28 rounded-full bg-white/20 flex items-center justify-center text-5xl font-bold mb-6">
            {initials}
          </div>

          <div className="text-3xl font-extrabold leading-tight">
            {me.fullName}
          </div>
          <div className="opacity-90 mt-2">{me.email}</div>
        </div>

        {/* RIGHT PANEL */}
        <div className="bg-slate-400/60 p-10 flex flex-col justify-center items-center text-center">
          
          <h2 className="text-2xl font-bold mb-6 text-center md:text-left">
            Profile Overview
          </h2>

          {/* badges */}
          <div className="flex flex-col sm:flex-row gap-4 mb-8 justify-center">
            <div className="bg-white/70 rounded-xl px-6 py-3 border text-center">
              <span className="text-sm opacity-70 mr-2">Member since</span>
              <span className="font-semibold">{createdAtPretty}</span>
            </div>

            <div className="bg-white/70 rounded-xl px-6 py-3 border text-center">
              <span className="text-sm opacity-70 mr-2">Account status</span>
              <span className="font-semibold">Active</span>
            </div>
          </div>

          {/* BUTTONS */}
          <div className="flex flex-col sm:flex-row gap-4 justify-center w-full max-w-3xl">
            
            <a href="/documents/upload" className="flex-1 py-4 rounded-xl text-lg font-semibold bg-indigo-600 text-white hover:bg-indigo-700 transition shadow text-center">
              Upload Resume
            </a>

            <a href="/me/documents" className="flex-1 py-4 rounded-xl text-lg font-semibold bg-slate-700 text-white hover:bg-slate-800 transition shadow text-center">
              View resumes
            </a>

            <a href="/documents/match" className="flex-1 py-4 rounded-xl text-lg font-semibold bg-slate-700 text-white hover:bg-slate-800 transition shadow text-center">
              Match Jobs
            </a>

          </div>
        </div>
      </div>
    </div>
  );
}