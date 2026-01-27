import { useState } from "react"
import Layout from "../components/Layout"
import Card from "../components/Card"
import PrimaryButton from "../components/PrimaryButton"
import { login } from "../services/auth"

export default function LoginPage({ onSwitch }) {
  const [usernameOrEmail, setU] = useState("")
  const [password, setP] = useState("")

  const submit = async () => {
    const token = await login(usernameOrEmail, password)
    if (token) location.reload()
  }

  return (
    <Layout>
      <Card>
        <h1 className="text-xl mb-4">Login</h1>
        <input className="w-full mb-2 p-2 border" placeholder="Username or Email" onChange={e => setU(e.target.value)} />
        <input className="w-full mb-4 p-2 border" type="password" placeholder="Password" onChange={e => setP(e.target.value)} />
        <PrimaryButton onClick={submit}>Login</PrimaryButton>
        <button className="mt-4 text-sm underline" onClick={onSwitch}>Register</button>
      </Card>
    </Layout>
  )
}
