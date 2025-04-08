import React, { useState } from 'react'
import Tags from '../components/tags'
import { UploadClient, UploadDto, LessonDto, LessonDetailsDto, FileMetadataDto, AddUploadRequestDto, FileClient } from '../api/apiClient.ts'
import { Button, Form, Input, notification } from 'antd';



const Upload: React.FC = () => {
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)
  const [tags, setTags] = useState<string[]>([])
  const uploadClient = new UploadClient()
  const fileClient = new FileClient()

  const getFileDuration = (file: File): Promise<number | null> => {
    return new Promise((resolve, reject) => {
      if (file.type.startsWith('audio/') || file.type.startsWith('video/')) {
        const mediaElement = document.createElement(file.type.startsWith('audio/') ? 'audio' : 'video');
        const url = URL.createObjectURL(file);

        mediaElement.src = url;

        mediaElement.onloadedmetadata = () => {
          resolve(mediaElement.duration); 
          URL.revokeObjectURL(url);
        };

        mediaElement.onerror = () => {
          URL.revokeObjectURL(url);
          reject(new Error('Unable to load media file for duration calculation'));
        };
      } else {
        resolve(null); 
      }
    });
  };





  const handleSubmit = async () => {
    if (!file) return

    const duration = await getFileDuration(file)

    const formData = new FormData();
    formData.append("file", file, file.name);



    const uploadDTO = new UploadDto()
    uploadDTO.init({
      id: 1,
      ownerId: 1,
      date: new Date(),
      type: file.type,
      libraryId: 1,
    })

    const lessonDetailsDTO = new LessonDetailsDto()
    lessonDetailsDTO.init({
      title : title,
      description : description,
      tags : tags,
    })

    const fileMetadataArray: FileMetadataDto[] = [];
    
    const fileMetadataDTO = new FileMetadataDto()
    fileMetadataDTO.init({
      id : 1,
      fileType : file.type,
      fileName : file.name,
      fileSize : file.size,
      duration : duration,
      date: new Date(),
      checksum: null,
    })

    fileMetadataArray.push(fileMetadataDTO);
    
    const lessonDTO = new LessonDto()
    lessonDTO.init({
      uploadId: 1,
      lessonDetails : lessonDetailsDTO,
      fileMetadata : fileMetadataArray,
      ownerId: 1,
    })
    
    const request = new AddUploadRequestDto()
    request.init({
      uploadDto: uploadDTO,
      lessonDto: lessonDTO,
    })

    try {


      const fileId = await fileClient.uploadFile(formData);
      
      await uploadClient.addUpload(request)
      
      notification.success({
        message: 'Upload successful',
        description: 'Your lecture and file have been uploaded successfully' +
          '\nFile ID: ' + fileId,
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