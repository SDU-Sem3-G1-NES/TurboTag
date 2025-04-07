import React, { useState } from 'react'
import Tags from '../components/tags'
import { UploadClient, UploadDto } from '../api/apiClient.ts'
import { Button, Form, Input, notification } from 'antd';
import { NotificationPlacement } from 'antd/es/notification/interface'
import Context from '@ant-design/icons/lib/components/Context';

const Upload: React.FC = () => {
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)
  const [tags, setTags] = useState<string[]>([])
  const uploadClient = new UploadClient()

  const openNotification = (placement: NotificationPlacement) => {
    notification.info({
      message: `Notification ${placement}`,
      description: <Context.Consumer>{() => `Hello, User!`}</Context.Consumer>,
      placement,
    });
  };

  const handleSubmit = async () => {
    if (!file) return

    const uploadDTO = new UploadDto()
    uploadDTO.init({
      id: 1,
      ownerId: 1,
      date: new Date(),
      type: file.type,
      libraryId: 1,
    })

    try {
      await uploadClient.addUpload(uploadDTO)

      openNotification('topRight');

      notification.success({
        message: 'Upload successful',
        description: 'Your lecture has been uploaded',
        placement: 'topRight',
        duration: 2,
      });

      console.log('Upload successful')
    } catch (error) {
      notification.error({
        message: 'Upload failed',
        description: 'Your lecture could not be uploaded',
        placement: 'topRight',
        duration: 2,
      });

      console.error('Upload failed', error)
    }
  }

  return (
    <Form
      name="basic"
      labelCol={{ span: 8 }}
      wrapperCol={{ span: 16 }}
      onFinish={handleSubmit}
      autoComplete="off"
    >
      <h1>Upload Lecture</h1>

      <Form.Item
        label="Title"
        name="title"
        rules={[{ required: true, message: 'Please input your title!' }]}
      >
        <Input
          id="title"
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
      </Form.Item>

      <Form.Item
        label="Description"
        name="description"
        rules={[{ required: true, message: 'Please input your description!' }]}
      >
        <Input
          id="description"
          type="text"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
        />
      </Form.Item>

      <Form.Item
        label="File"
        name="file"
        rules={[{ required: true, message: 'Please upload a file!' }]}
      >
        <Input
          id="file"
          type="file"
          onChange={(e) => setFile(e.target.files ? e.target.files[0] : null)}
        />
      </Form.Item>

      <Tags tags={tags} setTags={setTags} />

      <br />
      <Form.Item label={null}>
        <Button type="primary" htmlType="submit">
          Submit
        </Button>
      </Form.Item>
    </Form>
  )
}

export default Upload