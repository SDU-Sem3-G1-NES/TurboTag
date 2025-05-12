import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Layout, Menu } from 'antd'
import App from './App.tsx'
import Upload from './pages/upload.tsx'
import Login from './pages/login.tsx'
import './index.css'
import { BrowserRouter as Router, Route, Routes, useNavigate } from 'react-router-dom'
import { ReactNotifications } from 'react-notifications-component'
import 'react-notifications-component/dist/theme.css'
import { Content, Footer, Header } from 'antd/es/layout/layout'
import logo from './assets/logo.png'
import avatar from './assets/avatar.png'
import './App.css'
import { Button } from 'antd'

const AppLayout = () => {
  const navigate = useNavigate()

  const items = [
    {
      key: 'library',
      label: 'Home',
      onClick: () => navigate('/')
    },
    {
      key: 'upload',
      label: 'Upload',
      onClick: () => navigate('/upload')
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
          { 1 ? (
          <div className="user-container"> 
              <img src={avatar} alt="Ueser" className="user-avatar" />
              <Button type="primary" onClick={() => navigate('/login')}className="logout-button">Log out</Button>
          </div>
          ) : (<a></a>)}
        </Header>
        <Content className="content">
          <Routes>
            <Route path="/" element={<App />} />
            <Route path="/upload" element={<Upload />} />
            <Route path="/login" element={<Login />} />
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
