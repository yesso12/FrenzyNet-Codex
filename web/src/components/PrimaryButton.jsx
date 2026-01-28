import React from 'react';

export default function PrimaryButton({ children, className = '', ...props }) {
  return (
    <button
      className={`rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-white transition hover:bg-primaryHover ${className}`}
      {...props}
    >
      {children}
    </button>
  );
}
