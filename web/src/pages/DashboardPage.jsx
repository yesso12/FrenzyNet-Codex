import { useEffect, useState } from "react"
import Layout from "../components/Layout"
import Card from "../components/Card"
import PrimaryButton from "../components/PrimaryButton"
import api from "../services/api"

export default function DashboardPage() {
  const [devices, setDevices] = useState([])

  useEffect(() => {
    api.get("/devices").then(r => setDevices(r.data))
  }, [])

  return (
    <Layout>
      <Card>
        <h1 className="text-xl mb-4">Devices</h1>
        <ul>
          {devices.map(d => <li key={d.id}>{d.name}</li>)}
        </ul>
        <PrimaryButton onClick={() => alert("Add device UI next")}>
          Add Device
        </PrimaryButton>
      </Card>
    </Layout>
  )
}
