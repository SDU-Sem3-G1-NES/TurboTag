import React, { useState } from 'react'
import Tags from '../components/tags'
import {
  UploadClient,
  UploadDto,
  LessonDto,
  LessonDetailsDto,
  FileMetadataDto,
  AddUploadRequestDto,
  FileClient,
  UploadChunkDto,
  FinaliseUploadDto, 
  GenerationClient,
} from '../api/apiClient.ts'
import { Button, Form, Input, notification, Progress, Upload, UploadProps, Spin } from 'antd'
import { UploadOutlined } from '@ant-design/icons'
import TextArea from 'antd/es/input/TextArea'

const UploadPage: React.FC = () => {
  const [uploading, setUploading] = useState<boolean>(false)
  const [generating, setGenerating] = useState<boolean>(false)
  const [uploadProgress, setUploadProgress] = useState<number>(0)
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)
  const [tags, setTags] = useState<string[]>([])
  const uploadClient = new UploadClient()
  const fileClient = new FileClient()
  const contentGenerationClient = new GenerationClient()
  const CHUNK_SIZE = 1048576 * 15 // 15MB Chunk size
  const [form] = Form.useForm();

  const ownerId = Number(localStorage.getItem('userId'))

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
    const fileId = response.data as string

    setUploading(false)
    return fileId
  }
  
  const generateContention = async (/*file: File*/) => {
    setGenerating(true)
    const text = "One bright morning in Beijing, President Xi Jinping woke up craving something sweet and cold. 'Today,' he declared, 'I want bing chilling!' With great excitement, he summoned his personal chef and requested the finest ice cream in all of China.\n\nHowever, the chef looked worried. 'President Xi, we have run out of milk and sugar!'\n\nDetermined not to be defeated, Xi Jinping put on his casual jacket, sunglasses, and set out on his own to find bing chilling. As he walked through the streets, people gathered and waved, surprised to see their leader casually strolling around.\n\nEventually, he came across a small, colorful ice cream cart with a sign that read 'Bing Chilling – The Coolest Treat in Town!' Behind the cart stood none other than John Cena, holding a cone and speaking fluent Mandarin.\n\n'你想要冰淇淋吗？' John asked with a smile.\n\n'当然!' Xi Jinping replied, laughing.\n\nThey sat on a nearby bench, enjoying their bing chilling together while the crowd snapped selfies and laughed at the surreal moment. That day, the phrase 'Xi Jinping loves bing chilling' went viral across the internet.\n\nFrom that day on, every Sunday became 'Bing Chilling Day' in China, a day where everyone—from top leaders to children—would enjoy ice cream and remember the day diplomacy was served in a cone."

    const result = await contentGenerationClient.generate(text);

    if (result != null){
      const tagsList = (result.tags ?? "")
        .split(",")
        .map(tag => tag.trim())
        .filter(tag => tag !== "");

      setTags(tagsList)
      
      const description = result.description ?? ""
      setDescription(description)
      form.setFieldsValue({ description });
      
      notification.success({
        message: 'Content Generation successful',
        description: 'Your lecture content has been generated successfully',
        placement: 'topRight',
        duration: 2
      })
      console.log('Content Generation successful')
    }
    else {
      notification.error({
        message: 'Content Generation failed',
        description: 'Your lecture content could not be generated',
        placement: 'topRight',
        duration: 2
      })
      console.error('Content Generation failed')
    }
    
    setGenerating(false)
  }

  const handleSubmit = async () => {
    if (!file) return

    try {
      const fileId = await handleChunkedUpload(file)
      const duration = await getFileDuration(file)

      const uploadDTO = new UploadDto()
      uploadDTO.init({
        id: 0,
        ownerId: ownerId,
        date: new Date(),
        type: file.type,
        libraryId: 1
      })

      const lessonDetailsDTO = new LessonDetailsDto()
      lessonDetailsDTO.init({
        id: 0,
        title: title,
        description: description,
        tags: tags
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
        uploadId: fileId,
        lessonDetails: lessonDetailsDTO,
        fileMetadata: fileMetadataArray,
        ownerId: ownerId
      })

      const request = new AddUploadRequestDto()
      request.init({
        uploadDto: uploadDTO,
        lessonDto: lessonDTO
      })

      await uploadClient.addUpload(request)

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
      <Form form={form}
            layout="vertical" 
            onFinish={handleSubmit}
            initialValues={{ description: description }}>
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
            disabled
          />
        </Form.Item>

        <Form.Item label="Tags" name="tags">
          <Tags tags={tags} setTags={setTags} />
        </Form.Item>
        
        <Form.Item label="Content Generation" name="contentGeneration">
          <div className="upload-button-wrapper">
            {generating ? (
              <Spin tip="Loading..." />
            ) : (
              //Need to put it in generateContention function after implentation of mp3 to text 
              /*file as File*/
              <Button
                type="primary"
                className="upload-btn"
                icon={<UploadOutlined />}
                onClick={() => generateContention()}
              >
                
              </Button>
            )
            }
          </div>
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
