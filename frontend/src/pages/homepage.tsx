import React, { useMemo, useState } from 'react'
import { Checkbox, Col, Input, Row, Spin } from 'antd'
import { LoadingOutlined } from '@ant-design/icons'
import { useHomePageState } from './hooks/useHomepageState'
import LessonCard from '../components/lessonCard'
import TagSelectFilter from '../components/Filters/TagSelectFilter'
import UploaderSelectFilter from '../components/Filters/UploaderSelectFilter'
import { useNavigate } from 'react-router-dom'

const HomePage: React.FC = () => {
  const ownerId = useMemo(() => parseInt(localStorage.getItem('userId') ?? '0', 10), [])

  const [showAllOwner, setShowAllOwner] = useState(true)
  const [showAllStarred, setShowAllStarred] = useState(true)
  const navigate = useNavigate()
  const {
    ownerLessons,
    starredLessons,
    loading,
    search,
    setSearch,
    handleSearch,
    reload,
    selectedTags,
    setSelectedTags,
    selectedUploaderIds,
    setSelectedUploaderIds
  } = useHomePageState(showAllOwner, showAllStarred)

  const renderSection = (
    title: string,
    lessons: typeof ownerLessons,
    limit: boolean,
    setLimit: React.Dispatch<React.SetStateAction<boolean>>
  ) => {
    const displayedLessons = limit ? lessons.slice(0, 4) : lessons

    return (
      <>
        <Row
          align="middle"
          gutter={[16, 16]}
          style={{ width: '75%', marginBottom: 8, justifyContent: 'space-between' }}
        >
          <Col>
            <h1 style={{ textAlign: 'left', margin: 0 }}>{title}</h1>
          </Col>
          <Col>
            {lessons.length > 4 && (
              <Checkbox checked={!limit} onChange={(e) => setLimit(!e.target.checked)}>
                Show All
              </Checkbox>
            )}
          </Col>
        </Row>
        <Row gutter={[16, 16]} style={{ width: '75%' }}>
          {displayedLessons.map((lesson) => (
            <Col key={lesson.mongoId} span={12}>
              <LessonCard lesson={lesson} onStarToggled={reload} onClick={() => navigate(`/lesson/${lesson.uploadId}`)}/>
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

      <div style={{ width: '50%', marginBottom: 24 }}>
        <Row gutter={8}>
          <Col flex="auto">
            <TagSelectFilter
              userId={ownerId}
              selectedTags={selectedTags}
              setSelectedTags={setSelectedTags}
            />
          </Col>
          <Col flex="auto">
            <UploaderSelectFilter
              userId={ownerId}
              selectedUploaders={selectedUploaderIds}
              setSelectedUploaders={setSelectedUploaderIds}
            />
          </Col>
        </Row>
      </div>

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
