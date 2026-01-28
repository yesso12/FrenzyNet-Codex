import React, { useEffect, useState } from 'react';
import Card from '../components/Card';
import { apiRequest } from '../services/api';
import { authHeaders, getProfile } from '../services/auth';

export default function AdminPage() {
  const profile = getProfile();
  const [users, setUsers] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    const load = async () => {
      try {
        const data = await apiRequest('/admin/users', { headers: authHeaders() });
        setUsers(data);
      } catch (err) {
        setError(err.message);
      }
    };

    if (profile?.role === 'admin' || profile?.role === 'owner') {
      load();
    }
  }, [profile]);

  return (
    <div className="mx-auto max-w-5xl">
      <Card title="Admin Console">
        {error && <p className="text-sm text-red-400">{error}</p>}
        <div className="space-y-3 text-sm text-slate-300">
          {users.map((user) => (
            <div key={user.id} className="rounded-xl border border-slate-800 bg-slate-950/50 p-4">
              <div className="flex flex-wrap items-center justify-between gap-2">
                <div>
                  <p className="text-white">{user.username}</p>
                  <p className="text-xs text-slate-500">{user.email}</p>
                </div>
                <span className="rounded-full bg-violet-500/20 px-3 py-1 text-xs text-violet-200">
                  {user.role}
                </span>
              </div>
              {user.subscription && (
                <p className="mt-2 text-xs text-slate-400">
                  Plan: {user.subscription.planName} • {user.subscription.deviceCount}/
                  {user.subscription.maxDevices} devices
                </p>
              )}
            </div>
          ))}
        </div>
      </Card>
    </div>
  );
}
