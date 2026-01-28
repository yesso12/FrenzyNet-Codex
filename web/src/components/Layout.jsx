import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { clearToken, getProfile } from '../services/auth';

export default function Layout({ children }) {
  const navigate = useNavigate();
  const profile = getProfile();

  const handleLogout = () => {
    clearToken();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100">
      <header className="border-b border-slate-800">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
          <Link to="/dashboard" className="text-xl font-semibold text-white">
            FrenzyNet VPN
          </Link>
          <nav className="flex items-center gap-4 text-sm text-slate-300">
            {profile ? (
              <>
                <span className="text-slate-400">{profile.username}</span>
                <button
                  className="rounded-full border border-slate-700 px-4 py-2 text-xs uppercase tracking-wide"
                  onClick={handleLogout}
                >
                  Log out
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="hover:text-white">
                  Login
                </Link>
                <Link to="/register" className="hover:text-white">
                  Register
                </Link>
              </>
            )}
          </nav>
        </div>
      </header>
      <main className="mx-auto max-w-6xl px-6 py-10">{children}</main>
      <footer className="border-t border-slate-800 py-6">
        <div className="mx-auto flex max-w-6xl flex-col items-center justify-between gap-2 px-6 text-xs text-slate-500 sm:flex-row">
          <span>© {new Date().getFullYear()} FrenzyNet</span>
          <div className="flex gap-4">
            <Link to="/terms" className="hover:text-slate-300">
              Terms
            </Link>
            <Link to="/acceptable-use" className="hover:text-slate-300">
              Acceptable Use
            </Link>
          </div>
        </div>
      </footer>
    </div>
  );
}
