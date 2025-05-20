import React from 'react'
import { Col, Input, Row, Spin } from 'antd'
import { LoadingOutlined } from '@ant-design/icons'
import { useHomePageState } from './hooks/useHomepageState'
import LessonCard from '../components/lessonCard'

const HomePage: React.FC = () => {
  const { ownerLessons, starredLessons, loading, search, setSearch, handleSearch, reload } =
    useHomePageState()

  // Slice lessons to max 4 items for display, no filtering or exclusion
  const sliceLessons = (lessons: typeof ownerLessons) => lessons.slice(0, 4)

  const renderSection = (title: string, lessons: typeof ownerLessons) => (
    <>
      <Row gutter={[16, 16]} style={{ width: '75%' }}>
        <Col span={24}>
          <h1 style={{ textAlign: 'left' }}>{title}</h1>
        </Col>
      </Row>
      <Row gutter={[16, 16]} style={{ width: '75%' }}>
        {sliceLessons(lessons).map((lesson) => (
          <Col key={lesson.mongoId} span={12}>
            <LessonCard lesson={lesson} onStarToggled={reload} />
          </Col>
        ))}
      </Row>
    </>
  )

  return (
    <div
      style={{
        width: '100%',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        overflow: 'hidden' // No scroll
      }}>
      <Input.Search
        placeholder="Search by title or description"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        onSearch={handleSearch}
        style={{ width: '50%', marginBottom: 24 }}
        allowClear
      />

      {loading ? (
        <Spin
          indicator={<LoadingOutlined />}
          size="large"
          style={{ color: 'black', marginTop: 24 }}
        />
      ) : (
        <>
          {renderSection('Your Uploads', ownerLessons)}
          {renderSection('Starred Uploads', starredLessons)}
        </>
      )}
    </div>
  )
}

export default HomePage
