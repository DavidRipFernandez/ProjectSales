export const EmptyState = ({ message }: { message: string }) => (
  <div className="flex flex-col items-center justify-center rounded-lg border border-dashed border-slate-300 bg-white p-8 text-center text-sm text-slate-500">
    {message}
  </div>
);
