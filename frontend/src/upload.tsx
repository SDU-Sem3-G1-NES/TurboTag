import React, { useState } from 'react';

const Upload: React.FC = () => {
  const [title, setTitle] = useState<string>('');
  const [description, setDescription] = useState<string>('');
  const [file, setFile] = useState<File | null>(null);
  const [tags, setTags] = useState<string[]>([]);
  const [tagInput, setTagInput] = useState<string>('');

  const handleAddTag = () => {
    if (tagInput.trim() !== '') {
      setTags([...tags, tagInput]);
      setTagInput('');
    }
  };

  const handleRemoveTag = (index: number) => {
    setTags(tags.filter((_, i) => i !== index));
  };

  const handleSubmit = (event: React.FormEvent) => {
    event.preventDefault();
    const data = {
      title,
      description,
      file,
      tags
    };
    console.log(data);
    // Tutaj wyślij dane do backendu
  };

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
      <br/>
      
      <label htmlFor="description">Description</label>
      
      <input
        type="text"
        id="description"
        name="description"
        value={description}
        onChange={(e) => setDescription(e.target.value)}
      />
      <br/>
      
      <label htmlFor="file">File</label>
      
      <input
        type="file"
        id="file"
        name="file"
        onChange={(e) => setFile(e.target.files ? e.target.files[0] : null)}
        required
      />
      <br/>
      
      <ul>
        {tags.map((tag, index) => (
          <li key={index}>
            {tag}
            <button type="button" onClick={() => handleRemoveTag(index)}>Remove</button>
          </li>
        ))}
      </ul>
      <br/>
      
      <input
        type="text"
        value={tagInput}
        onChange={(e) => setTagInput(e.target.value)}
      />
      <button type="button" onClick={handleAddTag}>Add Tag</button>
      
      <br/>
      <button type="submit">Upload</button>
    </form>
  );
};

export default Upload;
