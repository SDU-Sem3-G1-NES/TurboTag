import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Layout, Menu } from 'antd'
import App from './App.tsx'
import './index.css'
import Upload from './pages/upload.tsx'
import { BrowserRouter as Router, Route, Routes, useNavigate } from 'react-router-dom'
import { ReactNotifications } from 'react-notifications-component'
import 'react-notifications-component/dist/theme.css'
import { Content, Header, Footer } from 'antd/es/layout/layout'

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
      <Layout>
        <ReactNotifications />
        <Header>
          <Menu items={items} mode="horizontal" />
        </Header>
        <Content>
          <Routes>
            <Route path="/" element={<App />} />
            <Route path="/upload" element={<Upload />} />
          </Routes>
        </Content>
        <Footer>Group 1 Turbo Tag</Footer>
      </Layout>
    </StrictMode>
  )
}

createRoot(document.getElementById('root')!).render(
  <Router>
    <AppLayout />
  </Router>
)
