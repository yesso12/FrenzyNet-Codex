import { useState } from "react"
import LoginPage from "./LoginPage"
import RegisterPage from "./RegisterPage"
import DashboardPage from "./DashboardPage"

export default function App() {
  const [page, setPage] = useState("login")
  const token = localStorage.getItem("token")

  if (token) return <DashboardPage />

  return page === "login"
    ? <LoginPage onSwitch={() => setPage("register")} />
    : <RegisterPage onSwitch={() => setPage("login")} />
}
