import React, { useEffect, useState } from 'react'
import { LessonClient, LessonDto } from '../api/apiClient'
import { Input, Row, Col, Spin } from 'antd'
import LibraryItem from '../components/library/libraryItem'
import { LoadingOutlined } from '@ant-design/icons'

const Library: React.FC = () => {
  const [loading, setLoading] = useState<boolean>(true)

  const lessonClient = new LessonClient()
  const [lessons, setLessons] = useState<LessonDto[]>([])
  const [search, setSearch] = useState<string>('')

  const fetchUploadData = async () => {
    try {
      const data = await lessonClient.getAllLessons()
      setLessons(Array.isArray(data) ? data : data.items || [])
    } catch (error) {
      console.error('Error fetching lesson data:', error)
    } finally {
      setLoading(false)
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
      {loading ? (
        <Spin
          indicator={<LoadingOutlined />}
          size="large"
          style={{ color: 'black', marginTop: 24 }}
        />
      ) : (
        <Row gutter={[16, 16]} style={{ width: '75%' }}>
          {filteredLessons.map((lesson: LessonDto) => (
            <Col key={lesson.mongoId} span={12}>
              <LibraryItem lesson={lesson} />
            </Col>
          ))}
        </Row>
      )}
    </div>
  )
}

export default Library
