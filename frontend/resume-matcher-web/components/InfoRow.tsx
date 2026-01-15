export default function InfoRow({ label, value, mono }: { label: string; value: string; mono?: boolean }) {
  return (
    <div className="flex justify-between items-center rounded-lg px-4 py-2 border" style={{ background: "var(--background)", borderColor: "var(--muted)",}}>
      <span className="text-sm font-semibold opacity-70">{label } </span>
      <span className={`text-sm font-semibold ${mono ? "font-mono" : " "}`}>
        {value}
      </span>
    </div>
  );
}