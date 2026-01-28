const API_BASE = import.meta.env.VITE_API_BASE_URL || '/api';

export async function apiRequest(path, options = {}) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers || {})
    },
    ...options
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || 'Request failed');
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export async function apiRequestBlob(path, options = {}) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: {
      ...(options.headers || {})
    },
    ...options
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || 'Request failed');
  }

  return response.blob();
}

export { API_BASE };
