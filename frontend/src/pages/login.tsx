import React, { useState } from 'react'
import { Button, Form, Input, Checkbox, Modal, Alert } from 'antd'
import { LoginClient, UserCredentialsDto } from '../api/apiClient.ts'
import { useNavigate, Navigate } from 'react-router-dom'
import { MailOutlined, LockOutlined } from '@ant-design/icons'
import { AxiosError } from 'axios'

const LoginPage: React.FC = () => {
  const navigate = useNavigate()
  const [form] = Form.useForm()
  const loginClient = new LoginClient()
  const [isModalVisible, setIsModalVisible] = useState(false)
  const [formError, setFormError] = useState<string | null>(null)

  if (localStorage.getItem('authToken')) {
    return <Navigate to="/" replace />
  }

  const handleLogin = async (values: { email: string; password: string; remember: boolean }) => {
    try {
      const userCredentials = new UserCredentialsDto({
        email: values.email,
        password: values.password
      })

      const response = await loginClient.login(userCredentials)
      localStorage.setItem('authToken', response.accessToken ?? 'undefined')
      if (values.remember) {
        localStorage.setItem('refreshToken', response.refreshToken ?? 'undefined')
      } else {
        sessionStorage.setItem('refreshToken', response.refreshToken ?? 'undefined')
        localStorage.removeItem('refreshToken')
      }
      localStorage.setItem('userId', response.userId?.toString() ?? 'undefined')
      localStorage.setItem('userName', response.name ?? 'undefined')
      navigate('/')
    } catch (error) {
      if (error instanceof AxiosError && error.response?.status === 401) {
        setFormError('Login failed: Invalid email or password')
      } else {
        const msg = error instanceof Error ? error.message : 'Unexpected error'
        setFormError('Login failed: ' + msg)
      }
    }
  }

  const handleForgotPassword = () => {
    setIsModalVisible(true)
  }

  return (
    <div className="centered-container">
      <Modal
        title="Forgot Password?"
        open={isModalVisible}
        onOk={() => setIsModalVisible(false)}
        onCancel={() => setIsModalVisible(false)}
        centered
        footer={null}
      >
        <p>Please contact the administrator to reset your password.</p>
      </Modal>
      <div className="form-container" style={{ padding: 30, maxWidth: 350 }}>
        <h1>Log in</h1>
        {formError ? (
          <Alert
            message={formError}
            type="error"
            showIcon
            style={{ marginBottom: 24, marginTop: 24, padding: '4px 11px' }}
          />
        ) : (
          <div style={{ height: 22 }}></div>
        )}
        <Form
          className="login-form"
          form={form}
          onFinish={handleLogin}
          layout="vertical"
          validateTrigger="onSubmit"
        >
          <Form.Item
            name="email"
            rules={[
              { required: true, message: 'Email is required' },
              { type: 'email', message: 'Invalid email format' }
            ]}
          >
            <Input prefix={<MailOutlined />} placeholder={'email'} maxLength={100} />
          </Form.Item>
          <Form.Item name="password" rules={[{ required: true, message: 'Password is required' }]}>
            <Input.Password prefix={<LockOutlined />} placeholder={'password'} maxLength={100} />
          </Form.Item>
          <div
            style={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              height: 16
            }}
          >
            <Form.Item noStyle name="remember" valuePropName="checked" initialValue={false}>
              <Checkbox>Remember me</Checkbox>
            </Form.Item>
            <Button
              type="link"
              onClick={handleForgotPassword}
              style={{ padding: 0, lineHeight: '22px' }}
            >
              Forgot password?
            </Button>
          </div>
          <Button type="primary" htmlType="submit" style={{ width: '100%', marginTop: 24 }}>
            Log in
          </Button>
        </Form>
      </div>
    </div>
  )
}

export default LoginPage
