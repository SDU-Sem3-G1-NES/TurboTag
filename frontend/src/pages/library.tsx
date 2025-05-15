import React, { useEffect, useState } from 'react'
import { LessonClient, LessonDto } from '../api/apiClient'
import { Flex, Input } from 'antd'
import LibraryItem from '../components/library/libraryItem'

const Library: React.FC = () => {
  const lessonClient = new LessonClient()
  const [lessons, setLessons] = useState<LessonDto[]>([])
  const [search, setSearch] = useState<string>('')

  const fetchUploadData = async () => {
    try {
      const data = await lessonClient.getAllLessons()
      setLessons(Array.isArray(data) ? data : data.items || [])
    } catch (error) {
      console.error('Error fetching lesson data:', error)
    }
  }

  useEffect(() => {
    fetchUploadData()
  }, [])

  const filteredLessons = lessons.filter((lesson) => {
    const title = lesson.lessonDetails?.title?.toLowerCase() || ''
    const description = lesson.lessonDetails?.description?.toLowerCase() || ''
    const tags = (lesson.lessonDetails?.tags || []).join(' ').toLowerCase()
    const searchTerm = search.toLowerCase()
    return (
      title.includes(searchTerm) || description.includes(searchTerm) || tags.includes(searchTerm)
    )
  })

  return (
    <div style={{ width: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <Input.Search
        placeholder="Search by title, description, or tag..."
        onChange={(e) => setSearch(e.target.value)}
        style={{ width: '50%', marginBottom: 24 }}
      />
      <Flex
        style={{
          width: '75%',
          height: '100%',
          flexDirection: 'row',
          flexWrap: 'wrap',
          justifyContent: 'space-between',
          gap: '16px'
        }}
      >
        {filteredLessons.map((lesson: LessonDto) => (
          <div key={lesson.mongoId} style={{ width: '48%' }}>
            <LibraryItem lesson={lesson} />
          </div>
        ))}
      </Flex>
    </div>
  )
}

export default Library
