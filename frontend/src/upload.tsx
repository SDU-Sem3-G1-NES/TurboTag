import React, { useState } from 'react'
import Tags from './tags'

const Upload: React.FC = () => {
  const [title, setTitle] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [file, setFile] = useState<File | null>(null)

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault()
    const data = {
      title,
      description,
      file
    }
    console.log(data)
  }

  return (
    <form onSubmit={handleSubmit}>
      <h1>Upload Lecture</h1>

      <label htmlFor="title">Title</label>

      <input
        type="text"
        id="title"
        name="title"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        required
      />
      <br />

      <label htmlFor="description">Description</label>

      <input
        type="text"
        id="description"
        name="description"
        value={description}
        onChange={(e) => setDescription(e.target.value)}
      />
      <br />

      <label htmlFor="file">File</label>

      <input
        type="file"
        id="file"
        name="file"
        onChange={(e) => setFile(e.target.files ? e.target.files[0] : null)}
        required
      />
      <br />
      <Tags />

      <br />
      <button type="submit">Upload</button>
    </form>
  )
}

export default Upload
