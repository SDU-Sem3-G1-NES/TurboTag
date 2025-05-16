import React, { useState } from 'react'
import { Button, Form, Input, message } from 'antd'
import {
  LoginClient,
  UserCredentialsDto
} from '../api/apiClient.ts'
import { useNavigate } from 'react-router-dom'

const LoginPage: React.FC = () => {
  const navigate = useNavigate()
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();
  const loginClient = new LoginClient();

  const handleLogin = async (values: { email: string; password: string }) => {
    try {
      setLoading(true);
      const userCredentials = new UserCredentialsDto({
        email: values.email,
        password: values.password
      });

      const response = await loginClient.login(userCredentials);
      localStorage.setItem("authToken", response.accessToken ?? "undefined");
      localStorage.setItem("refreshToken", response.refreshToken ?? "undefined");
      localStorage.setItem("userId", response.userId?.toString() ?? "undefined");
      localStorage.setItem("userName", response.name ?? "undefined");
      message.success('Login successful');
      navigate('/');
    } catch (error) {
      console.error('Login failed:', error);
      message.error('Login failed: Invalid email or password');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="centered-container">
      <div className="form-container">
        <h2 style={{ textAlign: 'left' }}>Log in</h2>
        <Form
          className="login-form"
          form={form}
          onFinish={handleLogin}
          layout="vertical"
        >
          <Form.Item name="email" label="Email" rules={[{ required: true, message: 'Email is required' }]}>
            <Input type="text" maxLength={100} />
          </Form.Item>
          <Form.Item name="password" label="Password" rules={[{ required: true, message: 'Password is required' }]}>
            <Input type="password" maxLength={100} />
          </Form.Item>
          <div className="upload-button-wrapper">
            <Button type="primary"
                    htmlType="submit"
                    loading={loading}
                    className="upload-btn">Log in</Button>
          </div>
        </Form>
      </div>
    </div>
  );
};

export default LoginPage;