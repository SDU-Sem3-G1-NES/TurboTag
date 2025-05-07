import React, { useState } from 'react';
import Tags from '../components/tags';
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
} from '../api/apiClient.ts';
import { Button, Form, Input, notification,Progress } from 'antd';

const Upload: React.FC = () => {
  const [uploading, setUploading] = useState<boolean>(false);
  const [uploadProgress, setUploadProgress] = useState<number>(0);
  const [title, setTitle] = useState<string>('');
  const [description, setDescription] = useState<string>('');
  const [file, setFile] = useState<File | null>(null);
  const [tags, setTags] = useState<string[]>([]);
  const uploadClient = new UploadClient();
  const fileClient = new FileClient();
  const CHUNK_SIZE = 1048576 * 15; // 15MB Chunk size

  const getFileDuration = (file: File): Promise<number | null> => {
    return new Promise((resolve, reject) => {
      if (!file.type.startsWith('audio/') && !file.type.startsWith('video/')) {
        reject(new Error('Unsupported file type. Only audio and video files are allowed.'));
        return;
      }

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

      resolve(null);
    });
  };

  const blobToBase64 = (blob: Blob) =>
    new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      reader.onloadend = () => {
        const result = reader.result as string;
        if (result.includes(',')) {
          resolve(result.split(',')[1]);
        } else {
          reject(new Error('Invalid base64 format'));
        }
      };
      reader.onerror = reject;
      reader.readAsDataURL(blob);
    });

  const handleChunkedUpload = async (file: File) => {
    const uploadId = Date.now().toString();
    const totalChunks = Math.ceil(file.size / CHUNK_SIZE);
    
    setUploading(true);
    
    for (let i = 0; i < totalChunks; i++) {
      const chunk = file.slice(i * CHUNK_SIZE, (i + 1) * CHUNK_SIZE);
      const base64Chunk = await blobToBase64(chunk);

      const uploadChunkDto = new UploadChunkDto();
      uploadChunkDto.init({
        chunk: base64Chunk,
        uploadId: uploadId,
        chunkNumber: i,
      });

      await fileClient.uploadChunk(uploadChunkDto);

      setUploadProgress(Math.round(((i + 1) / totalChunks) * 100));
    }

    const finalizeUploadDto = new FinaliseUploadDto();
    finalizeUploadDto.init({
      uploadId: uploadId,
      fileName: file.name,
    });

    const fileId = await fileClient.finalizeUpload(finalizeUploadDto);
    
    setUploading(false);
    return fileId;
  };

  const handleSubmit = async () => {
    if (!file) return;

    try {
      const fileId = await handleChunkedUpload(file);
      const duration = await getFileDuration(file);

      const uploadDTO = new UploadDto();
      uploadDTO.init({
        id: 0,
        ownerId: 1,
        date: new Date(),
        type: file.type,
        libraryId: 1,
      });

      const lessonDetailsDTO = new LessonDetailsDto();
      lessonDetailsDTO.init({
        title: title,
        description: description,
        tags: tags,
      });

      const fileMetadataDTO = new FileMetadataDto();
      fileMetadataDTO.init({
        id: 0,
        fileType: file.type,
        fileName: file.name,
        fileSize: file.size,
        duration: duration,
        date: new Date(),
        checksum: null,
      });

      const lessonDTO = new LessonDto();
      lessonDTO.init({
        uploadId: fileId,
        lessonDetails: lessonDetailsDTO,
        fileMetadata: fileMetadataDTO,
        ownerId: 1,
      });

      const request = new AddUploadRequestDto();
      request.init({
        uploadDto: uploadDTO,
        lessonDto: lessonDTO,
      });

      await uploadClient.addUpload(request);

      notification.success({
        message: 'Upload successful',
        description: 'Your lecture and file have been uploaded successfully',
        placement: 'topRight',
        duration: 2,
      });

      console.log('Upload successful');
    } catch (error) {
      notification.error({
        message: 'Upload failed',
        description: 'Your lecture could not be uploaded',
        placement: 'topRight',
        duration: 2,
      });

      console.error('Upload failed', error);
    }
  };

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
        <Input id="title" type="text" value={title} onChange={(e) => setTitle(e.target.value)} />
      </Form.Item>

      <Form.Item
        label="Description"
        name="description"
        rules={[{ required: true, message: 'Please input your description!' }]}
      >
        <Input id="description" type="text" value={description} onChange={(e) => setDescription(e.target.value)} />
      </Form.Item>

      <Form.Item
        label="File"
        name="file"
        rules={[{ required: true, message: 'Please upload a file!' }]}
      >
        <Input id="file" type="file" onChange={(e) => setFile(e.target.files ? e.target.files[0] : null)} />
      </Form.Item>

      <Tags tags={tags} setTags={setTags} />

      <br />
      <Form.Item label={null}>
        {uploading ? (
          <Progress percent={uploadProgress} status="active" />
        ) : (
          <Button type="primary" htmlType="submit">
            Submit
          </Button>
        )}
        
      </Form.Item>
    </Form>
  );
};

export default Upload;