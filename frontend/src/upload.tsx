import React, { useState } from 'react'
import Tags from './tags'
import { UploadClient, UploadDTO, UploadDetailsDTO, FileMetadataDTO } from './api/apiClient.ts'

const Upload: React.FC = () => {
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)
  const [tags, setTags] = useState<string[]>([])
  const uploadClient = new UploadClient()

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault()
    if (!file) return

    const fileMetadata = new FileMetadataDTO()
    fileMetadata.fileName = file.name
    fileMetadata.fileSize = file.size
    fileMetadata.duration = null
    fileMetadata.date = new Date().toISOString()
    fileMetadata.checkSum = ''

    const uploadDetails = new UploadDetailsDTO()
    uploadDetails.init({id})
    uploadDetails.id = 0
    uploadDetails.description = description
    uploadDetails.title = title
    uploadDetails.tags = tags

    const uploadDTO = new UploadDTO()
    uploadDTO.id = 0
    uploadDTO.ownerId = 1
    uploadDTO.libraryId = 1
    uploadDTO.details = uploadDetails
    uploadDTO.fileMetadata = fileMetadata

    try {
      await uploadClient.storeUpload(uploadDTO)
      console.log('Upload successful')
    } catch (error) {
      console.error('Upload failed', error)
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h1>Upload Lecture</h1>

      <label htmlFor="title">Title</label>
      <input
        id="title"
        type="text"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
      />
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
