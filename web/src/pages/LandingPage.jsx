import React from 'react';
import { Link } from 'react-router-dom';
import PrimaryButton from '../components/PrimaryButton';

export default function LandingPage() {
  return (
    <div className="space-y-16">
      <section className="rounded-3xl border border-violet-800/30 bg-gradient-to-br from-violet-950 via-slate-950 to-slate-950 p-10 text-slate-100">
        <p className="text-sm uppercase tracking-[0.3em] text-violet-300">Frenzy VPN Services</p>
        <h1 className="mt-4 text-4xl font-semibold text-white md:text-5xl">
          Secure, reliable gaming VPN built for low latency.
        </h1>
        <p className="mt-4 max-w-2xl text-sm text-slate-300 md:text-base">
          Protect your connection, optimize routing, and keep your gaming sessions stable with a
          trusted WireGuard-powered platform.
        </p>
        <div className="mt-6 flex flex-wrap gap-3">
          <Link to="/register">
            <PrimaryButton>Get started</PrimaryButton>
          </Link>
          <Link
            to="/login"
            className="rounded-lg border border-violet-700/50 px-4 py-2 text-sm text-violet-200"
          >
            Sign in
          </Link>
        </div>
      </section>

      <section className="grid gap-6 md:grid-cols-3">
        {[
          {
            title: 'Performance first',
            description: 'Optimized WireGuard tunnels for consistent ping and throughput.'
          },
          {
            title: 'Trusted security',
            description: 'Modern cryptography with audited logging and device-level controls.'
          },
          {
            title: 'Always in control',
            description: 'Provision or revoke devices instantly from your dashboard.'
          }
        ].map((item) => (
          <div
            key={item.title}
            className="rounded-2xl border border-slate-800 bg-slate-900/60 p-6 text-sm text-slate-300"
          >
            <h3 className="text-base font-semibold text-white">{item.title}</h3>
            <p className="mt-2">{item.description}</p>
          </div>
        ))}
      </section>

      <section className="rounded-2xl border border-slate-800 bg-slate-900/60 p-6 text-sm text-slate-300">
        <h2 className="text-lg font-semibold text-white">Built for trust</h2>
        <p className="mt-2">
          Frenzy VPN Services prioritizes reliability, transparency, and abuse prevention. Review
          our Terms of Service and Acceptable Use Policy before subscribing.
        </p>
        <div className="mt-4 flex gap-4">
          <Link to="/terms" className="text-violet-300 hover:text-violet-200">
            Terms of Service
          </Link>
          <Link to="/acceptable-use" className="text-violet-300 hover:text-violet-200">
            Acceptable Use Policy
          </Link>
        </div>
      </section>
    </div>
  );
}
