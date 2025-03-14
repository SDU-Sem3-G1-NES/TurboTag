import { useState } from "react";
import { TagsInput } from "react-tag-input-component";


const Tags = ()=> {
  const [selected, setSelected] = useState(["tag"]);
  
  return (
    <div>
      <h1>Add Tags</h1>
      <TagsInput
        value={selected}
        onChange={setSelected}
        name="tags"
        placeHolder="Enter Tags"
      />
      <em>Press enter or comma to add new tag</em>
    </div>
  )
  
}

export default Tags;