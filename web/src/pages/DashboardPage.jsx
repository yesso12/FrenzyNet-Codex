import React, { useEffect, useState } from 'react';
import Card from '../components/Card';
import PrimaryButton from '../components/PrimaryButton';
import { apiRequest, apiRequestBlob } from '../services/api';
import { authHeaders, getProfile } from '../services/auth';

export default function DashboardPage() {
  const profile = getProfile();
  const [subscription, setSubscription] = useState(null);
  const [devices, setDevices] = useState([]);
  const [deviceName, setDeviceName] = useState('');
  const [config, setConfig] = useState('');
  const [selectedDevice, setSelectedDevice] = useState(null);
  const [qrUrl, setQrUrl] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const loadDevices = async () => {
    try {
      const data = await apiRequest('/devices', { headers: authHeaders() });
      setDevices(data);
    } catch (err) {
      setError(err.message);
    }
  };

  const loadProfile = async () => {
    try {
      const data = await apiRequest('/me', { headers: authHeaders() });
      setSubscription(data.subscription);
    } catch (err) {
      setError(err.message);
    }
  };

  useEffect(() => {
    loadDevices();
    loadProfile();
  }, []);

  useEffect(() => {
    return () => {
      if (qrUrl) {
        URL.revokeObjectURL(qrUrl);
      }
    };
  }, [qrUrl]);

  const handleCreate = async () => {
    if (!deviceName.trim()) {
      setError('Enter a device name.');
      return;
    }
    setLoading(true);
    setError('');
    try {
      const data = await apiRequest('/devices', {
        method: 'POST',
        headers: authHeaders(),
        body: JSON.stringify({ name: deviceName })
      });
      setConfig(data.config);
      setSelectedDevice({ id: data.id, name: deviceName });
      setDeviceName('');
      await loadDevices();
      await loadProfile();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleRevoke = async (deviceId, name) => {
    if (!confirm(`Revoke ${name}? This will disable the device.`)) {
      return;
    }
    setError('');
    try {
      await apiRequest(`/devices/${deviceId}`, {
        method: 'DELETE',
        headers: authHeaders()
      });
      await loadDevices();
      await loadProfile();
    } catch (err) {
      setError(err.message);
    }
  };

  const handleDownload = async (device) => {
    setError('');
    try {
      const blob = await apiRequestBlob(`/devices/${device.id}/config`, { headers: authHeaders() });
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `frenzynet-${device.name}.conf`;
      link.click();
      URL.revokeObjectURL(url);
    } catch (err) {
      setError(err.message);
    }
  };

  const handleShowQr = async (device) => {
    setError('');
    try {
      const blob = await apiRequestBlob(`/devices/${device.id}/qrcode`, { headers: authHeaders() });
      if (qrUrl) {
        URL.revokeObjectURL(qrUrl);
      }
      const url = URL.createObjectURL(blob);
      setQrUrl(url);
      setSelectedDevice(device);
    } catch (err) {
      setError(err.message);
    }
  };

  const closeQr = () => {
    if (qrUrl) {
      URL.revokeObjectURL(qrUrl);
    }
    setQrUrl('');
  };

  return (
    <div className="grid gap-8 lg:grid-cols-[2fr_1fr]">
      <Card title={`Welcome${profile ? `, ${profile.username}` : ''}`}>
        <div className="space-y-6">
          <div className="rounded-xl border border-slate-800 bg-slate-950/60 p-4">
            <p className="text-sm text-slate-300">
              Add a new device to generate a WireGuard configuration and QR code.
            </p>
            {subscription && (
              <p className="mt-2 text-xs text-slate-500">
                Plan: {subscription.planName} • Status: {subscription.status} • $
                {subscription.pricePerDevice.toFixed(2)} / device • {subscription.deviceCount}/
                {subscription.maxDevices} devices
              </p>
            )}
            <div className="mt-4 flex flex-col gap-3 sm:flex-row">
              <input
                className="flex-1 rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm"
                value={deviceName}
                onChange={(event) => setDeviceName(event.target.value)}
                placeholder="e.g. Gaming PC"
              />
              <PrimaryButton onClick={handleCreate} disabled={loading}>
                {loading ? 'Provisioning...' : 'Create device'}
              </PrimaryButton>
            </div>
          </div>

          {error && <p className="text-sm text-red-400">{error}</p>}

          <div>
            <h3 className="mb-3 text-sm font-semibold uppercase tracking-wide text-slate-400">
              Your devices
            </h3>
            <div className="space-y-3">
              {devices.map((device) => (
                <div
                  key={device.id}
                  className="flex flex-col gap-3 rounded-xl border border-slate-800 bg-slate-950/40 p-4 sm:flex-row sm:items-center sm:justify-between"
                >
                  <div>
                    <p className="font-medium text-white">{device.name}</p>
                    <p className="text-xs text-slate-500">IP: {device.ipAddress ?? 'pending'}</p>
                  </div>
                  <div className="flex flex-wrap items-center gap-3 text-xs">
                    <span
                      className={`rounded-full px-3 py-1 ${
                        device.revokedAt
                          ? 'bg-red-500/20 text-red-300'
                          : 'bg-emerald-500/20 text-emerald-300'
                      }`}
                    >
                      {device.revokedAt ? 'Revoked' : 'Active'}
                    </span>
                    {!device.revokedAt && (
                      <>
                        <button
                          className="rounded-full border border-slate-700 px-3 py-1 text-xs"
                          onClick={() => handleDownload(device)}
                        >
                          Download
                        </button>
                        <button
                          className="rounded-full border border-slate-700 px-3 py-1 text-xs"
                          onClick={() => handleShowQr(device)}
                        >
                          QR Code
                        </button>
                        <button
                          className="text-xs text-red-400 hover:text-red-300"
                          onClick={() => handleRevoke(device.id, device.name)}
                        >
                          Revoke
                        </button>
                      </>
                    )}
                  </div>
                </div>
              ))}
              {devices.length === 0 && (
                <p className="text-sm text-slate-500">No devices yet.</p>
              )}
            </div>
          </div>
        </div>
      </Card>

      <Card title="Latest configuration">
        {config ? (
          <div className="space-y-4">
            <div className="rounded-xl bg-slate-950/60 p-4 text-xs text-slate-300">
              <pre className="whitespace-pre-wrap">{config}</pre>
            </div>
            <div className="flex gap-3">
              <PrimaryButton onClick={() => navigator.clipboard.writeText(config)}>Copy</PrimaryButton>
            </div>
          </div>
        ) : (
          <p className="text-sm text-slate-500">Generate a device to see configuration details.</p>
        )}
      </Card>

      {qrUrl && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 p-6">
          <div className="w-full max-w-sm rounded-2xl border border-slate-800 bg-slate-950 p-6 text-center">
            <h3 className="text-lg font-semibold text-white">Scan QR code</h3>
            <p className="mt-1 text-xs text-slate-400">{selectedDevice?.name}</p>
            <img src={qrUrl} alt="WireGuard QR" className="mx-auto mt-4 h-48 w-48" />
            <button
              className="mt-6 w-full rounded-lg border border-slate-700 px-4 py-2 text-sm"
              onClick={closeQr}
            >
              Close
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
