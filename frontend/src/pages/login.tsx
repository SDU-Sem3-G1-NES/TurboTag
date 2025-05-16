import React, { useState } from 'react'
import { Button, Form, Input } from 'antd'
import {
  LoginClient,
  UserCredentialsDto
} from '../api/apiClient.ts'
import { useNavigate } from 'react-router-dom'

const LoginPage: React.FC = () => {
  const navigate = useNavigate()
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const loginClient = new LoginClient();
  const userCredentials = new UserCredentialsDto({
    email: email,
    password: password
  });
  const handleLogin = async () => {
    try {
      const response = await loginClient.login(userCredentials);
      localStorage.setItem("authToken", response.accessToken);
      localStorage.setItem("refreshToken", response.refreshToken);
      localStorage.setItem("userId", response.userId);
      localStorage.setItem("userName", response.name);
      navigate('/')
    } catch (error) {
      console.error('Login failed:', error);
      // Handle login error (e.g., show notification)
    }
    console.log('Login submitted'); 
  };

  return (
    <div className="centered-container">
      <h1 className="title">Log in</h1>
      <div className="form-container">
        <Form className="login-form" onFinish={handleLogin}>
          <div className="form-group">
            <label htmlFor="email">Email</label>
            <Input type="text" maxLength={100} value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          <div className="form-group">
            <label htmlFor="password">Password</label>
            <Input type="password" maxLength={100} value={password}   onChange={(e) => setPassword(e.target.value)} required />
          </div>
          <div className="upload-button-wrapper">
          <Button type="primary"
                  htmlType="submit"
                  className="upload-btn">Log in</Button>
          </div>
        </Form>
      </div>
    </div>
  );
};

export default LoginPage;