import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Layout, Menu } from 'antd'
import App from './App.tsx'
import Upload from './pages/upload.tsx'
import Login from './pages/login.tsx'
import ProtectedRoute from './components/ProtectedRoute'
import './index.css'
import { BrowserRouter as Router, Route, Routes, useNavigate } from 'react-router-dom'
import { ReactNotifications } from 'react-notifications-component'
import 'react-notifications-component/dist/theme.css'
import { Content, Footer, Header } from 'antd/es/layout/layout'
import logo from './assets/logo.png'
import avatar from './assets/avatar.png'
import './App.css'
import { Button } from 'antd'
import { LogoutOutlined, LoginOutlined } from '@ant-design/icons';
import { LoginClient } from './api/apiClient.ts'

const AppLayout = () => {
  const navigate = useNavigate()
  const loginClient = new LoginClient()
  const userName = localStorage.getItem('userName');
  
  const handleLogout = async () => {
    await loginClient.logout()
    localStorage.removeItem("authToken")
    localStorage.removeItem("refreshToken")
    localStorage.removeItem('userId')
    localStorage.removeItem('userName')
    navigate('/login')
  }
  
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
            { userName &&
            <Menu items={items} mode="horizontal" /> }
          </div>
          <div className="logo-container">
            <img src={logo} alt="SpeedAdmin" className="logo" />
          </div>
          { userName ? (
            <div className="user-container">
              <img src={avatar} alt="User" className="user-avatar" />
              <span className="user-name">{userName}</span>
              <Button
                type="text"
                icon={<LogoutOutlined style={{ fontSize: '32px' }} />}
                onClick={handleLogout}
                className="auth-buttons"
                title="Log out"
              />
            </div>
          ) : (<Button
            type="text"
            icon={<LoginOutlined style={{ fontSize: '32px' }} />}
            onClick={handleLogout}
            className="auth-buttons"
            title="Log in"
          />) }
        </Header>
        <Content className="content">
          <Routes>
            <Route path="/" element={
              <ProtectedRoute>
                <App />
              </ProtectedRoute>
            } />
            <Route path="/upload" element={
              <ProtectedRoute>
                <Upload />
              </ProtectedRoute>
            } />
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
