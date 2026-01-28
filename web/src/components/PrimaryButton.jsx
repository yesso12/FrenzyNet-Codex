import React from 'react';

export default function PrimaryButton({ children, className = '', ...props }) {
  return (
    <button
      className={`rounded-lg bg-accent px-4 py-2 text-sm font-semibold text-slate-900 transition hover:brightness-110 ${className}`}
      {...props}
    >
      {children}
    </button>
  );
}
