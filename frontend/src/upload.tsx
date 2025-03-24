import React, { useState } from 'react'
import Tags from './tags'
import { UploadClient, UploadDto, UploadDetailsDto, FileMetadataDto } from './api/apiClient.ts'
import { Store } from 'react-notifications-component'

const Upload: React.FC = () => {
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)
  const [tags, setTags] = useState<string[]>([])
  const uploadClient = new UploadClient()

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
      date: new Date()})
    

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

      Store.addNotification({
        title: 'Upload successful',
        message: 'Your lecture has been uploaded',
        type: 'success',
        insert: 'top',
        container: 'top-right',
        animationIn: ['animate__animated', 'animate__fadeIn'],
        animationOut: ['animate__animated', 'animate__fadeOut'],
        dismiss: {
          duration: 2000,
          onScreen: true
        }
      })
      console.log('Upload successful')
    } catch (error) {
      Store.addNotification({
        title: 'Upload failed',
        message: 'Your lecture could not be uploaded',
        type: 'danger',
        insert: 'top',
        container: 'top-right',
        animationIn: ['animate__animated', 'animate__fadeIn'],
        animationOut: ['animate__animated', 'animate__fadeOut'],
        dismiss: {
          duration: 2000,
          onScreen: true
        }
      })
      console.error('Upload failed', error)
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h1>Upload Lecture</h1>

      <label htmlFor="title">Title</label>
      <input id="title" type="text" value={title} onChange={(e) => setTitle(e.target.value)} />
      <br />

      <label htmlFor="description">Description</label>
      <input
        id="description"
        type="text"
        value={description}
        onChange={(e) => setDescription(e.target.value)}
      />
      <br />

      <label htmlFor="file">File</label>
      <input
        id="file"
        type="file"
        onChange={(e) => setFile(e.target.files ? e.target.files[0] : null)}
      />
      <br />
      <Tags tags={tags} setTags={setTags} />

      <br />
      <button type="submit">Upload</button>
    </form>
  )
}

export default Upload
