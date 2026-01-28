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
          <Link to="/" className="text-xl font-semibold text-white">
            Frenzy VPN Services
          </Link>
          <nav className="flex items-center gap-4 text-sm text-slate-300">
            {profile ? (
              <>
                <Link to="/dashboard" className="hover:text-white">
                  Dashboard
                </Link>
                <Link to="/account" className="hover:text-white">
                  Account
                </Link>
                {(profile.role === 'admin' || profile.role === 'owner') && (
                  <Link to="/admin" className="hover:text-white">
                    Admin
                  </Link>
                )}
                {profile.role === 'owner' && (
                  <Link to="/owner" className="hover:text-white">
                    Owner
                  </Link>
                )}
                <span className="text-slate-400">{profile.username}</span>
                <button
                  className="rounded-full border border-violet-600/50 px-4 py-2 text-xs uppercase tracking-wide"
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
          <span>© {new Date().getFullYear()} Frenzy VPN Services</span>
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
