import React, { useState, useEffect } from 'react'
import { LibraryDTO, UploadDTO } from '../api/apiClient'
import { ContentLibraryClient } from '../api/apiClient'

const Library: React.FC = () => {
    const contentLibraryClient = new ContentLibraryClient()
    const [userLibraries, setUserLibraries] = useState<LibraryDTO[]>([])
    const [libaryUploads, setLibraryUploads] = useState<UploadDTO[]>([])
    const [selectedLibraryId, setSelectedLibraryId] = useState<string>('')
    const fetchUserLibraries = () => {
        const response = contentLibraryClient.getUserLibrariesById("mockId")
        response.then((response) => {
            setUserLibraries(response)  
        })
    }
    const fetchLibraryUploads = (libraryId: string) => {
        const response = contentLibraryClient.getLibraryUploadsById(libraryId)
        response.then((response) => {
            setLibraryUploads(response)  
        })
    }
    useEffect(() => {
      fetchUserLibraries()
  }, []) 
  return (
    <div>
    <p>Select a library to retrieve uploads from:</p>
    <select value={selectedLibraryId} onChange={(e) => setSelectedLibraryId(e.target.value)}>
        {userLibraries.map((library) => (
            <option key={library.id} value={library.id}>
                {library.name}
            </option>
        ))}
    </select>
    <div>
        <button onClick={() => fetchLibraryUploads(selectedLibraryId)}>Fetch Uploads</button>
        {libaryUploads.map((upload) => (
            <div >
              <h3>Upload ID: {upload.id}</h3>
              <p>Owner ID: {upload.ownerId}</p>
              <p>Library ID: {upload.libraryId}</p>
            </div>
        ))}
    </div>
    </div>
  )
}

export default Library
