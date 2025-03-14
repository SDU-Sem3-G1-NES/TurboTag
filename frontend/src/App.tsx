import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom'
import Upload from './upload'
import Clicker from './Clicker'


function App() {
  

  return (
    <Router>
      <nav>
        <ul>
          <li>
            <Link to="/">Home</Link>
          </li>
          <li>
            <Link to="/upload">Upload</Link>
          </li>
        </ul>
      </nav>
      <Routes>
        <Route path="/" element={<Clicker/>}
        />
        <Route path="/upload" element={<Upload />} />
      </Routes>
    </Router>
  )
}

export default App
