import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import Card from '../components/Card';
import PrimaryButton from '../components/PrimaryButton';
import { apiRequest } from '../services/api';
import { setToken } from '../services/auth';

export default function RegisterPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [acceptTerms, setAcceptTerms] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setLoading(true);
    try {
      const data = await apiRequest('/auth/register', {
        method: 'POST',
        body: JSON.stringify({ email, username, password, acceptTerms })
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
      <Card title="Create your account">
        <form className="space-y-4" onSubmit={handleSubmit}>
          <div>
            <label className="text-sm text-slate-300">Email</label>
            <input
              className="mt-2 w-full rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="player@example.com"
            />
          </div>
          <div>
            <label className="text-sm text-slate-300">Username</label>
            <input
              className="mt-2 w-full rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              placeholder="FrenzyPlayer"
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
          <label className="flex items-start gap-2 text-xs text-slate-400">
            <input
              type="checkbox"
              className="mt-1"
              checked={acceptTerms}
              onChange={(event) => setAcceptTerms(event.target.checked)}
            />
            <span>
              I agree to the{' '}
              <Link className="text-violet-300" to="/terms">
                Terms of Service
              </Link>{' '}
              and{' '}
              <Link className="text-violet-300" to="/acceptable-use">
                Acceptable Use Policy
              </Link>.
            </span>
          </label>
          {error && <p className="text-sm text-red-400">{error}</p>}
          <PrimaryButton className="w-full" type="submit" disabled={loading}>
            {loading ? 'Creating...' : 'Create account'}
          </PrimaryButton>
        </form>
        <p className="mt-4 text-xs text-slate-400">
          Already have an account?{' '}
          <Link className="text-violet-300" to="/login">
            Log in
          </Link>
        </p>
      </Card>
    </div>
  );
}
