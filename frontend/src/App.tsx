import {useEffect, useState} from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import api from './api/api';

function App() {
  const [count, setCount] = useState(0)
    const [weather, setWeather] = useState<string | null>(null)
    useEffect(() => {
        console.log('useEffect');
        api.get('/weatherforecast'
        ).then(response => {
            console.log('response');
            setWeather(JSON.stringify(response.data, null, 2));
        }
        ).catch(error => {
            console.log('error');
        });
    }, []);

  return (
    <>
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
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>
        <p>
            If there's text below... it's working! - Nick
        </p>
        <p>{weather}</p>
    </>
  )
}

export default App
