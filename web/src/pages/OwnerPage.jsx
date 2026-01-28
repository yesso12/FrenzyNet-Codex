import React, { useState } from 'react';
import Card from '../components/Card';
import { apiRequest } from '../services/api';
import { authHeaders, getProfile } from '../services/auth';

export default function OwnerPage() {
  const profile = getProfile();
  const [userId, setUserId] = useState('');
  const [role, setRole] = useState('admin');
  const [message, setMessage] = useState('');

  const handleUpdate = async (event) => {
    event.preventDefault();
    setMessage('');
    try {
      await apiRequest(`/owner/users/${userId}/role`, {
        method: 'PATCH',
        headers: authHeaders(),
        body: JSON.stringify({ role })
      });
      setMessage('Role updated.');
    } catch (err) {
      setMessage(err.message);
    }
  };

  if (profile?.role !== 'owner') {
    return (
      <div className="mx-auto max-w-3xl">
        <Card title="Owner Console">
          <p className="text-sm text-slate-400">Owner access required.</p>
        </Card>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-3xl">
      <Card title="Owner Console">
        <form className="space-y-4 text-sm text-slate-300" onSubmit={handleUpdate}>
          <div>
            <label className="text-xs uppercase tracking-wide text-slate-500">User ID</label>
            <input
              className="mt-2 w-full rounded-lg border border-slate-700 bg-slate-950 px-3 py-2"
              value={userId}
              onChange={(event) => setUserId(event.target.value)}
              placeholder="User UUID"
            />
          </div>
          <div>
            <label className="text-xs uppercase tracking-wide text-slate-500">Role</label>
            <select
              className="mt-2 w-full rounded-lg border border-slate-700 bg-slate-950 px-3 py-2"
              value={role}
              onChange={(event) => setRole(event.target.value)}
            >
              <option value="admin">admin</option>
              <option value="user">user</option>
              <option value="owner">owner</option>
            </select>
          </div>
          <button className="rounded-lg bg-violet-600 px-4 py-2 text-sm text-white" type="submit">
            Update role
          </button>
          {message && <p className="text-xs text-slate-400">{message}</p>}
        </form>
      </Card>
    </div>
  );
}
