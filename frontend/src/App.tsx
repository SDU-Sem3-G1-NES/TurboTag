import React from "react";
import { Layout, Input, Upload, Button, Typography } from "antd";
import { UploadOutlined, InboxOutlined } from "@ant-design/icons";
import "./App.css";
import logo from "./assets/logo.png";

const { Header, Content } = Layout;
const { TextArea } = Input;
const { Dragger } = Upload;

const App: React.FC = () => {
  const uploadProps = {
    name: "file",
    multiple: false,
    accept: ".mp4,.mov,.avi,.wmv",
    beforeUpload: () => false,
  };

  return (
    <Layout className="layout">
      <Header className="header">
        <img src={logo} alt="SpeedAdmin" className="logo" />
      </Header>

      <Content className="content">
        <div className="form-container">
          <Typography.Title level={5}>Title</Typography.Title>
          <TextArea rows={1} maxLength={100} placeholder="Enter Title Here" />

          <div className="upload-box">
            <Dragger {...uploadProps}>
              <p className="upload-icon">
                <InboxOutlined style={{ fontSize: 40, color: "#1890ff" }} />
              </p>
              <p>Click or drag file to this area to upload</p>
              <p className="hint">Supported file formats: mp4, mov, avi, wmv</p>
            </Dragger>
          </div>

          <Typography.Title level={5}>Tags</Typography.Title>
          <Input placeholder="Feel free to edit your AI generated tags" maxLength={100} />

          <div className="upload-button-wrapper">
            <Button
              type="primary"
              icon={<UploadOutlined />}
              className="upload-btn"
            >
              Upload
            </Button>
          </div>
        </div>
      </Content>
    </Layout>
  );
};

export default App;
