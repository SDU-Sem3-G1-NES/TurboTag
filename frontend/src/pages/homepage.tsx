import React, { useEffect, useState } from 'react'
import { LessonClient, LessonDto, LessonFilter } from '../api/apiClient'
import { Col, Input, Row, Spin } from 'antd'
import LibraryItem from '../components/library/libraryItem'
import { LoadingOutlined } from '@ant-design/icons'

const HomePage: React.FC = () => {
  const [loading, setLoading] = useState<boolean>(true)

  const lessonClient = new LessonClient()

  const [ownerLessons, setOwnerLessons] = useState<LessonDto[]>([])
  const [search, setSearch] = useState<string>('')

  const getUserOwnedLessons = async () => {
    try {
      const filter = new LessonFilter({
        ownerId: parseInt(localStorage.getItem('userId') ?? '0', 10)
      })
      const data = await lessonClient.getAllLessons(filter)
      setOwnerLessons(Array.isArray(data) ? data : data.items || [])
    } catch (error) {
      console.error('Error fetching lesson data:', error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    getUserOwnedLessons()
  }, [])

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
        <>
          <Row gutter={[16, 16]} style={{ width: '75%' }}>
            <Col span={24}>
              <h1 style={{ textAlign: 'left' }}>Your Uploads</h1>
            </Col>
          </Row>
          <Row gutter={[16, 16]} style={{ width: '75%' }}>
            {ownerLessons.map((lesson: LessonDto) => (
              <Col key={lesson.mongoId} span={12}>
                <LibraryItem lesson={lesson} />
              </Col>
            ))}
          </Row>
        </>
      )}
    </div>
  )
}

export default HomePage
