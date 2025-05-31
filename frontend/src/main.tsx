import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Button, Layout, Menu } from 'antd'
import Upload from './pages/upload.tsx'
import Login from './pages/login.tsx'
import Admin from './pages/admin.tsx'
import HomePage from './pages/homepage.tsx'
import ProtectedRoute from './components/ProtectedRoute'
import './index.css'
import { BrowserRouter as Router, Route, Routes, useNavigate } from 'react-router-dom'
import { ReactNotifications } from 'react-notifications-component'
import 'react-notifications-component/dist/theme.css'
import { Content, Footer, Header } from 'antd/es/layout/layout'
import logo from './assets/logo.png'
import avatar from './assets/avatar.png'
import './App.css'
import { LoginOutlined, LogoutOutlined } from '@ant-design/icons'
import { LoginClient } from './api/apiClient.ts'
import Forbidden from './pages/forbidden.tsx'
import LibraryPage from './pages/librarypage.tsx'
import LessonPage from './pages/lesson.tsx'

const AppLayout = () => {
  const navigate = useNavigate()
  const loginClient = new LoginClient()
  const userName = localStorage.getItem('userName')
  const userType = localStorage.getItem('userType')

  const handleLogout = async () => {
    await loginClient.logout()
    localStorage.removeItem('authToken')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('userId')
    localStorage.removeItem('userName')
    localStorage.removeItem('userType')
    navigate('/login')
  }

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
    },
    ...(userType === 'speedadmin'
      ? [
          {
            key: 'admin',
            label: 'Admin',
            onClick: () => navigate('/admin')
          }
        ]
      : [])
  ]

  return (
    <StrictMode>
      <Layout className="layout">
        <ReactNotifications />
        <Header className="header">
          <div className="menu-container">
            {userName && <Menu items={items} mode="horizontal" />}
          </div>
          <div className="logo-container">
            <img src={logo} alt="SpeedAdmin" className="logo" />
          </div>
          {userName ? (
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
          ) : (
            <Button
              type="text"
              icon={<LoginOutlined style={{ fontSize: '32px' }} />}
              onClick={handleLogout}
              className="auth-buttons"
              title="Log in"
            />
          )}
        </Header>
        <Content className="content">
          <Routes>
            <Route
              path="/"
              element={
                <ProtectedRoute>
                  <HomePage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/upload"
              element={
                <ProtectedRoute>
                  <Upload />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin"
              element={
                <ProtectedRoute>
                  <Admin />
                </ProtectedRoute>
              }
            />
            <Route
              path="/library"
              element={
                <ProtectedRoute>
                  <LibraryPage />
                </ProtectedRoute>
              }
            />
            <Route path="/login" element={<Login />} />
            <Route path="/forbidden" element={<Forbidden />} />
            <Route path="/lesson/:uploadId" element={<LessonPage />} />
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
