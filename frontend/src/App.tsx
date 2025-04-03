import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import { useState } from 'react'
import { LessonClient } from './api/apiClient.ts'
import { FileClient } from './api/apiClient.ts'

function App() {
  const [count, setCount] = useState(0)
  const fileClient = new FileClient()
  const lessonClient = new LessonClient()
  const [testString, setTestString] = useState<string | null>(null)
  const [imageUrl, setImageUrl] = useState<string | null>(null)

  const getTestString = (callback: () => void) => {
    console.log('getTestString')
    lessonClient.getLessonByObjectId('67e852b07eba9a8b9ee2c14c').then((data) => {
      setTestString(data.lessonDetails?.title ?? null)
      callback()
    })
  }
  const fetchImage = async () => {
    const response = await fileClient.getFileById('67e852b07eba9a8b9ee2c14d')
    const blob = response.data
    const url = URL.createObjectURL(blob)
    setImageUrl(url)
  }
  return (
    <div>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a>
          <img
            src={'http://localhost:5088/File/GetFileById?id=67e852b07eba9a8b9ee2c14d'}
            alt="docker compose up :)"
            height="100"
          />
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
      <button onClick={() => getTestString(fetchImage)}>Test</button>
      <p>{testString}</p>
      {imageUrl && <img src={imageUrl} alt="No shrek, something wrong :(" />}
    </div>
  )
}

export default App
