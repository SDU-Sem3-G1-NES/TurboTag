import React from 'react'
import { Navigate } from 'react-router-dom'

const ProtectedRoute: React.FC<{ children: React.ReactElement }> = ({ children }) => {
  const isAuthenticated = !!localStorage.getItem('authToken')
  return isAuthenticated ? children : <Navigate to="/login" replace />
}

export default ProtectedRoute