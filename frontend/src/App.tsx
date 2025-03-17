import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { useState } from 'react'
import { AdminClient } from './api/apiClient.ts'


function App() {
  const [count, setCount] = useState(0)
  const adminClient = new AdminClient()
  const [testString, setTestString] = useState<string | null>(null)

  const getTestString = (callback: () => void) => {
    console.log('getTestString')
    adminClient.getAllUsers().then((response) => {
      console.log('response', response)
      setTestString(response[0].email ?? null)
      callback()
    })
  }

  return (
    <div>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>count is {count}</button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">Click on the Vite and React logos to learn more</p>
      <button onClick={() => getTestString(() => console.log('weather updated'))}>
        Test
      </button>
      <p>{testString}</p>
    </div>
  )
}

export default App
