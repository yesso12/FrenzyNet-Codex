import React from 'react';

export default function Card({ title, children, className = '' }) {
  return (
    <div className={`rounded-2xl border border-slate-800 bg-slate-900/60 p-6 shadow ${className}`}>
      {title && <h2 className="mb-4 text-lg font-semibold text-white">{title}</h2>}
      {children}
    </div>
  );
}
