/*import React, { useState } from 'react'
import Tags from '../components/tags'
import { UploadClient, UploadDto, UploadDetailsDto, FileMetadataDto } from '../api/apiClient.ts'
import { Store } from 'react-notifications-component'
import { Button, Form, Input } from 'antd';
import { Divider, notification, Space } from 'antd';
import type { NotificationArgsProps } from 'antd';
import { CancelToken } from 'axios'
  
const Upload: React.FC = () => {
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)
  const [tags, setTags] = useState<string[]>([])
  const uploadClient = new UploadClient()


  const openNotification = (placement: NotificationPlacement) => {
    notification.info({
      message: `Notification ${placement}`,
      description: <Context.Consumer>{({ name }) => `Hello, ${name}!`}</Context.Consumer>,
      placement,
    });
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault()
    if (!file) return

    const fileMetadata = new FileMetadataDto({
      checkSum: '1234',
      id: 1,
      fileSize: 1000,
      fileName: file.name,
      fileType: file.type,
      duration: 100,
      date: new Date()
    })

    const uploadDetails = new UploadDetailsDto({
      id: 1,
      title: title,
      description: description,
      tags: tags
    })

    const uploadDTO = new UploadDto()
    uploadDTO.init({
      id: 1,
      ownerId: 1,
      libraryId: 1,
      details: uploadDetails,
      fileMetadata: fileMetadata
    })

    try {
      await uploadClient.storeUpload(uploadDTO)

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
      
      <Form.Item<string>
      label="Title"
      name="title"
      rules={[{ required: true, message: 'Please input your title!' }]}>
        <Input
          id="title"
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
      </Form.Item>
      

   
      <Form.Item<string>
      label="Description"
      name="description"
      rules={[{ required: true, message: 'Please input your description!' }]}>
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
      rules={[{ required: true, message: 'Please upload a file!' }]}>
        < Input
          id="file"
          type="file"
          onChange={(e) => setFile(e.target.files ? e.target.files[0] : null)}
        />
      </Form.Item>
      
      <Tags tags={tags} setTags={setTags} />
      
      <br/>
      <Form.Item label={null}>
        <Button type="primary" htmlType="submit">
          Submit
        </Button>
      </Form.Item>
    </Form>
  )
}

export default Upload
*/
