import React, { useEffect, useState } from 'react';
import Card from '../components/Card';
import { apiRequest } from '../services/api';
import { authHeaders } from '../services/auth';

export default function AccountPage() {
  const [data, setData] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    const load = async () => {
      try {
        const result = await apiRequest('/me', { headers: authHeaders() });
        setData(result);
      } catch (err) {
        setError(err.message);
      }
    };
    load();
  }, []);

  return (
    <div className="mx-auto max-w-3xl">
      <Card title="Account & Subscription">
        {error && <p className="text-sm text-red-400">{error}</p>}
        {data && (
          <div className="space-y-4 text-sm text-slate-300">
            <div>
              <p className="text-xs uppercase tracking-wide text-slate-500">Subscription</p>
              <p className="mt-1 text-white">{data.subscription.planName}</p>
              <p className="text-xs text-slate-500">Status: {data.subscription.status}</p>
            </div>
            <div className="grid gap-4 sm:grid-cols-2">
              <div>
                <p className="text-xs uppercase tracking-wide text-slate-500">Devices</p>
                <p className="mt-1 text-white">
                  {data.subscription.deviceCount} / {data.subscription.maxDevices}
                </p>
              </div>
              <div>
                <p className="text-xs uppercase tracking-wide text-slate-500">Price per device</p>
                <p className="mt-1 text-white">${data.subscription.pricePerDevice.toFixed(2)}</p>
              </div>
            </div>
          </div>
        )}
      </Card>
    </div>
  );
}
