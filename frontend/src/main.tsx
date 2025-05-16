import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Layout, Menu } from 'antd'
import App from './App.tsx'
import Upload from './pages/upload.tsx'
import './index.css'
import { BrowserRouter as Router, Route, Routes, useNavigate } from 'react-router-dom'
import { ReactNotifications } from 'react-notifications-component'
import 'react-notifications-component/dist/theme.css'
import { Content, Footer, Header } from 'antd/es/layout/layout'
import logo from './assets/logo.png'
import './App.css'
import Library from './pages/library.tsx'

const AppLayout = () => {
  const navigate = useNavigate()

  const items = [
    {
      key: 'home',
      label: 'Home',
      onClick: () => navigate('/')
    },
    {
      key: 'upload',
      label: 'Upload',
      onClick: () => navigate('/upload')
    },
    {
      key: 'library',
      label: 'Library',
      onClick: () => navigate('/library')
    }
  ]

  return (
    <StrictMode>
      <Layout className="layout">
        <ReactNotifications />
        <Header className="header">
          <div className="menu-container">
            <Menu items={items} mode="horizontal" />
          </div>
          <div className="logo-container">
            <img src={logo} alt="SpeedAdmin" className="logo" />
          </div>
        </Header>
        <Content className="content">
          <Routes>
            <Route path="/" element={<App />} />
            <Route path="/upload" element={<Upload />} />
            <Route path="/library" element={<Library />} />
          </Routes>
        </Content>
        <Footer style={{ textAlign: 'center' }}>Group 1 Turbo Tag</Footer>
      </Layout>
    </StrictMode>
  )
}

createRoot(document.getElementById('root')!).render(
  <Router>
    <AppLayout />
  </Router>
)
