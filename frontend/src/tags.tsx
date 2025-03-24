import { TagsInput } from 'react-tag-input-component'

interface TagsProps {
  tags: string[]
  setTags: React.Dispatch<React.SetStateAction<string[]>>
}

const Tags: React.FC<TagsProps> = ({ tags, setTags }) => {
  return (
    <div>
      <h1>Add Tags</h1>
      <TagsInput value={tags} onChange={setTags} name="tags" placeHolder="Enter Tags" />
      <em>Press enter or comma to add new tag</em>
    </div>
  )
}

export default Tags
