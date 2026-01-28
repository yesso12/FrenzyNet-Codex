import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import Card from '../components/Card';
import PrimaryButton from '../components/PrimaryButton';
import { apiRequest } from '../services/api';
import { setToken } from '../services/auth';

export default function LoginPage() {
  const navigate = useNavigate();
  const [identifier, setIdentifier] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setLoading(true);
    try {
      const data = await apiRequest('/auth/login', {
        method: 'POST',
        body: JSON.stringify({ identifier, password })
      });
      setToken(data.token, { username: data.username, role: data.role });
      navigate('/dashboard');
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="mx-auto max-w-md">
      <Card title="Welcome back">
        <form className="space-y-4" onSubmit={handleSubmit}>
          <div>
            <label className="text-sm text-slate-300">Email or username</label>
            <input
              className="mt-2 w-full rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm"
              value={identifier}
              onChange={(event) => setIdentifier(event.target.value)}
              placeholder="player@example.com"
            />
          </div>
          <div>
            <label className="text-sm text-slate-300">Password</label>
            <input
              type="password"
              className="mt-2 w-full rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
            />
          </div>
          {error && <p className="text-sm text-red-400">{error}</p>}
          <PrimaryButton className="w-full" type="submit" disabled={loading}>
            {loading ? 'Signing in...' : 'Sign in'}
          </PrimaryButton>
        </form>
        <p className="mt-4 text-xs text-slate-400">
          Need an account?{' '}
          <Link className="text-accent" to="/register">
            Register
          </Link>
        </p>
      </Card>
    </div>
  );
}
