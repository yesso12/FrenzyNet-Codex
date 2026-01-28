const TOKEN_KEY = 'frenzynet.token';
const PROFILE_KEY = 'frenzynet.profile';

export function setToken(token, profile) {
  localStorage.setItem(TOKEN_KEY, token);
  localStorage.setItem(PROFILE_KEY, JSON.stringify(profile));
}

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function getProfile() {
  const value = localStorage.getItem(PROFILE_KEY);
  return value ? JSON.parse(value) : null;
}

export function clearToken() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(PROFILE_KEY);
}

export function authHeaders() {
  const token = getToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
}
