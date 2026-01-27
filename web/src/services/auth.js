import api from "./api"

export async function login(usernameOrEmail, password) {
  const r = await api.post("/auth/login", { usernameOrEmail, password })
  localStorage.setItem("token", r.data.token)
  return r.data.token
}

export async function register(username, email, password) {
  await api.post("/auth/register", { username, email, password })
}
