import { useState } from "react"
import Layout from "../components/Layout"
import Card from "../components/Card"
import PrimaryButton from "../components/PrimaryButton"
import { register } from "../services/auth"

export default function RegisterPage({ onSwitch }) {
  const [u, setU] = useState("")
  const [e, setE] = useState("")
  const [p, setP] = useState("")

  const submit = async () => {
    await register(u, e, p)
    onSwitch()
  }

  return (
    <Layout>
      <Card>
        <h1 className="text-xl mb-4">Register</h1>
        <input className="w-full mb-2 p-2 border" placeholder="Username" onChange={e => setU(e.target.value)} />
        <input className="w-full mb-2 p-2 border" placeholder="Email" onChange={e => setE(e.target.value)} />
        <input className="w-full mb-4 p-2 border" type="password" placeholder="Password" onChange={e => setP(e.target.value)} />
        <PrimaryButton onClick={submit}>Register</PrimaryButton>
        <button className="mt-4 text-sm underline" onClick={onSwitch}>Back to Login</button>
      </Card>
    </Layout>
  )
}
