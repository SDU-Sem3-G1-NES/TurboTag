import React, { useState } from 'react'
import { Checkbox, Col, Input, Row, Spin } from 'antd'
import { LoadingOutlined } from '@ant-design/icons'
import { useHomePageState } from './hooks/useHomepageState'
import LessonCard from '../components/lessonCard'

const HomePage: React.FC = () => {
  const { ownerLessons, starredLessons, loading, search, setSearch, handleSearch, reload } =
    useHomePageState()

  // Local state to track if showing all lessons for each section
  const [showAllOwner, setShowAllOwner] = useState(false)
  const [showAllStarred, setShowAllStarred] = useState(false)

  const renderSection = (
    title: string,
    lessons: typeof ownerLessons,
    showAll: boolean,
    setShowAll: React.Dispatch<React.SetStateAction<boolean>>
  ) => {
    // Limit to 4 lessons if showAll is false
    const displayedLessons = showAll ? lessons : lessons.slice(0, 4)

    return (
      <>
        <Row
          align="middle"
          gutter={[16, 16]}
          style={{ width: '75%', marginBottom: 8, justifyContent: 'space-between' }}>
          <Col>
            <h1 style={{ textAlign: 'left', margin: 0 }}>{title}</h1>
          </Col>
          <Col>
            <Checkbox checked={showAll} onChange={() => setShowAll(!showAll)}>
              Show All
            </Checkbox>
          </Col>
        </Row>
        <Row gutter={[16, 16]} style={{ width: '75%' }}>
          {displayedLessons.map((lesson) => (
            <Col key={lesson.mongoId} span={12}>
              <LessonCard lesson={lesson} onStarToggled={reload} />
            </Col>
          ))}
        </Row>
      </>
    )
  }

  return (
    <div
      style={{
        width: '100%',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        overflow: 'hidden' // No scroll
      }}
    >
      <Input.Search
        placeholder="Search by title or description"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        onSearch={(value) => handleSearch(value)}
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
          {renderSection('Your Uploads', ownerLessons, showAllOwner, setShowAllOwner)}
          {renderSection('Starred Uploads', starredLessons, showAllStarred, setShowAllStarred)}
        </>
      )}
    </div>
  )
}

export default HomePage
