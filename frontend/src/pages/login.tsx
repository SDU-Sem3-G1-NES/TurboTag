import React from 'react';
import { Button, Form, Input } from 'antd'
const LoginPage: React.FC = () => {
  const handleLogin = (event: React.FormEvent) => {
    event.preventDefault();
    // Add login logic here
    console.log('Login submitted');
  };

  return (
    <div className="centered-container">
      <h1 className="title">Log in</h1>
      <div className="form-container">
        <Form className="login-form" onFinish={handleLogin}>
          <div className="form-group">
            <label htmlFor="username">Username</label>
            <Input type="text" id="username" name="username" required />
          </div>
          <div className="form-group">
            <label htmlFor="password">Password</label>
            <Input type="password" id="password" name="password" required />
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