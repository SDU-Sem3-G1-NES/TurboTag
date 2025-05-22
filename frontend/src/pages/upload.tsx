import React, { useState } from 'react'
import Tags from '../components/tags'
import {
  UploadClient,
  LessonClient,
  UploadDto,
  LessonDto,
  LessonDetailsDto,
  FileMetadataDto,
  FileClient,
  UploadChunkDto,
  FinaliseUploadDto
} from '../api/apiClient.ts'
import { Button, Form, Input, notification, Progress, Upload, UploadProps } from 'antd'
import { UploadOutlined } from '@ant-design/icons'
import TextArea from 'antd/es/input/TextArea'

const UploadPage: React.FC = () => {
  const [uploading, setUploading] = useState<boolean>(false)
  const [uploadProgress, setUploadProgress] = useState<number>(0)
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)
  const [tags, setTags] = useState<string[]>([])
  const uploadClient = new UploadClient()
  const fileClient = new FileClient()
  const lessonClient = new LessonClient()
  const CHUNK_SIZE = 1048576 * 15 // 15MB Chunk size

  const ownerId = Number(localStorage.getItem('userId'))
  const ownerName = localStorage.getItem('userName')

  const handleFileChange: UploadProps['beforeUpload'] = (file) => {
    setFile(file)
    return false // prevent auto-upload
  }

  const getFileDuration = (file: File): Promise<number | null> => {
    return new Promise((resolve, reject) => {
      if (!file.type.startsWith('audio/') && !file.type.startsWith('video/')) {
        reject(new Error('Unsupported file type. Only audio and video files are allowed.'))
        return
      }

      const mediaElement = document.createElement(
        file.type.startsWith('audio/') ? 'audio' : 'video'
      )
      const url = URL.createObjectURL(file)

      mediaElement.src = url

      mediaElement.onloadedmetadata = () => {
        resolve(mediaElement.duration)
        URL.revokeObjectURL(url)
      }

      mediaElement.onerror = () => {
        URL.revokeObjectURL(url)
        reject(new Error('Unable to load media file for duration calculation'))
      }
    })
  }

  const blobToBase64 = (blob: Blob) =>
    new Promise<string>((resolve, reject) => {
      const reader = new FileReader()
      reader.onloadend = () => {
        const result = reader.result as string
        if (result.includes(',')) {
          resolve(result.split(',')[1])
        } else {
          reject(new Error('Invalid base64 format'))
        }
      }
      reader.onerror = reject
      reader.readAsDataURL(blob)
    })

  const handleChunkedUpload = async (file: File) => {
    const uploadId = crypto.randomUUID()
    const totalChunks = Math.ceil(file.size / CHUNK_SIZE)

    setUploading(true)

    for (let i = 0; i < totalChunks; i++) {
      const chunk = file.slice(i * CHUNK_SIZE, (i + 1) * CHUNK_SIZE)
      const base64Chunk = await blobToBase64(chunk)

      const uploadChunkDto = new UploadChunkDto()
      uploadChunkDto.init({
        chunk: base64Chunk,
        uploadId: uploadId,
        chunkNumber: i
      })

      await fileClient.uploadChunk(uploadChunkDto)

      setUploadProgress(Math.round(((i + 1) / totalChunks) * 100))
    }

    const finalizeUploadDto = new FinaliseUploadDto()
    finalizeUploadDto.init({
      uploadId: uploadId,
      fileName: file.name
    })

    const response = await fileClient['instance'].post('/File/FinalizeUpload', finalizeUploadDto)
    const fileId = response.data.fileId as string
    const thumbnailId = response.data.thumbnailId as string

    setUploading(false)
    return { fileId, thumbnailId }
  }

  const handleSubmit = async () => {
    if (!file) return

    try {
      const { fileId, thumbnailId } = await handleChunkedUpload(file)
      const duration = await getFileDuration(file)

      const uploadDTO = new UploadDto()
      uploadDTO.init({
        id: null,
        ownerId: ownerId,
        date: new Date(),
        type: file.type,
        libraryId: 1
      })

      const uploadID = await uploadClient.addUpload(uploadDTO)

      const lessonDetailsDTO = new LessonDetailsDto()
      lessonDetailsDTO.init({
        id: uploadID,
        title: title,
        description: description,
        tags: tags,
        thumbnailId: thumbnailId
      })

      const fileMetadataDTO = new FileMetadataDto()
      fileMetadataDTO.init({
        id: fileId,
        fileType: file.type,
        fileName: file.name,
        fileSize: file.size,
        duration: duration === null ? null : Math.round(duration),
        date: new Date(),
        checksum: null
      })

      const fileMetadataArray = Array.isArray(fileMetadataDTO) ? fileMetadataDTO : [fileMetadataDTO]

      const lessonDTO = new LessonDto()
      lessonDTO.init({
        uploadId: uploadID,
        lessonDetails: lessonDetailsDTO,
        fileMetadata: fileMetadataArray,
        ownerId: ownerId,
        ownerName: ownerName
      })

      await lessonClient.addLesson(lessonDTO)

      notification.success({
        message: 'Upload successful',
        description: 'Your lecture and file have been uploaded successfully',
        placement: 'topRight',
        duration: 2
      })

      console.log('Upload successful')
    } catch (error) {
      notification.error({
        message: 'Upload failed',
        description: 'Your lecture could not be uploaded',
        placement: 'topRight',
        duration: 2
      })

      console.error('Upload failed', error)
    }
  }

  return (
    <div className="form-container">
      <Form layout="vertical" onFinish={handleSubmit}>
        <Form.Item
          label="Title"
          name="title"
          rules={[{ required: true, message: 'Please input your title!' }]}
        >
          <Input
            maxLength={100}
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Enter Title Here"
          />
        </Form.Item>

        <Form.Item
          label="Upload File"
          name="file"
          rules={[{ required: true, message: 'Please upload a file!' }]}
        >
          <Upload.Dragger beforeUpload={handleFileChange} accept=".mp4,.mov,.avi,.wmv" maxCount={1}>
            <p className="ant-upload-drag-icon">
              <UploadOutlined />
            </p>
            <p className="ant-upload-text">Click or drag file to this area to upload</p>
            <p className="ant-upload-hint">Supported file formats: mp4, mov, avi, wmv</p>
          </Upload.Dragger>
        </Form.Item>

        <Form.Item
          label="Description"
          name="description"
          rules={[{ required: true, message: 'Please input your description!' }]}
        >
          <TextArea
            maxLength={100}
            value={title}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Optional short description"
          />
        </Form.Item>

        <Form.Item label="Tags" name="tags">
          <Tags tags={tags} setTags={setTags} />
        </Form.Item>

        <Form.Item>
          <div className="upload-button-wrapper">
            {uploading ? (
              <Progress percent={uploadProgress} status="active" />
            ) : (
              <Button
                type="primary"
                htmlType="submit"
                className="upload-btn"
                icon={<UploadOutlined />}
              >
                Submit
              </Button>
            )}
          </div>
        </Form.Item>
      </Form>
    </div>
  )
}

export default UploadPage
