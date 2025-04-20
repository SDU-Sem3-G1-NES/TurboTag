import React, { useState } from 'react'
import Tags from '../components/tags'
import { FileMetadataDto, UploadDto, UploadDetailsDto } from '../api/apiClient.ts'
import { Button, Form, Input, Typography, Upload, notification } from 'antd'
import { UploadOutlined } from '@ant-design/icons'
import type { UploadProps } from 'antd'

const { TextArea } = Input

const UploadPage: React.FC = () => {
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)
  const [tags, setTags] = useState<string[]>([])

  const handleFileChange: UploadProps['beforeUpload'] = (file) => {
    setFile(file)
    return false // prevent auto-upload
  }

  const handleSubmit = async () => {
    if (!file) return

    const fileMetadata = new FileMetadataDto({
      checkSum: '1234',
      fileSize: file.size,
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
      // await uploadClient.storeUpload(uploadDTO)

      notification.success({
        message: 'Upload successful',
        description: 'Your lecture has been uploaded',
        placement: 'topRight',
        duration: 2
      })
    } catch (error) {
      notification.error({
        message: 'Upload failed',
        description: 'Your lecture could not be uploaded',
        placement: 'topRight',
        duration: 2
      })
    }
  }

  return (
    <div className="form-container">
      <Typography.Title level={3}>Upload Lecture</Typography.Title>

      <Form layout="vertical" onFinish={handleSubmit}>
        <Form.Item label="Title" required>
          <TextArea
            maxLength={100}
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Enter Title Here"
          />
        </Form.Item>

        <Form.Item label="Upload File" required>
          <Upload.Dragger beforeUpload={handleFileChange} accept=".mp4,.mov,.avi,.wmv" maxCount={1}>
            <p className="ant-upload-drag-icon">
              <UploadOutlined />
            </p>
            <p className="ant-upload-text">Click or drag file to this area to upload</p>
            <p className="ant-upload-hint">Supported file formats: mp4, mov, avi, wmv</p>
          </Upload.Dragger>
        </Form.Item>

        <Form.Item label="Description">
          <Input
            maxLength={100}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Optional short description"
          />
        </Form.Item>

        <Form.Item label="Tags">
          <Tags tags={tags} setTags={setTags} />
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" icon={<UploadOutlined />}>
            Upload
          </Button>
        </Form.Item>
      </Form>
    </div>
  )
}

export default UploadPage
