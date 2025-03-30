import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import 'bootstrap/dist/css/bootstrap.css'
import App from './App.tsx'
import Upload from './upload'
import Library from './library'
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Router>
      <nav>
        <ul>
          <li>
            <Link to="/">Home</Link>
          </li>
          <li>
            <Link to="/upload">Upload</Link>
          </li>
          <li>
            <Link to="/library">Library</Link>
          </li>
        </ul>
      </nav>
      <Routes>
        <Route path="/" element={<App />} />
        <Route path="/upload" element={<Upload />} />
        <Route path="/library" element={<Library />} />
      </Routes>
    </Router>
  </StrictMode>
)
