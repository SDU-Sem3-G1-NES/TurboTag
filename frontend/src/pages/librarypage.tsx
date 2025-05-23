import React from 'react'
import { Col, Input, Pagination, Row, Spin } from 'antd'
import { LoadingOutlined } from '@ant-design/icons'
import { useLibraryPageState } from './hooks/useLibrarypageState'
import LessonCard from '../components/lessonCard'
import TagSelectFilter from '../components/Filters/TagSelectFilter'
import UploaderSelectFilter from '../components/Filters/UploaderSelectFilter'
import { useNavigate } from 'react-router-dom'

const LibraryPage: React.FC = () => {
  const {
    lessons,
    loading,
    search,
    setSearch,
    handleSearch,
    reload,
    selectedTags,
    setSelectedTags,
    selectedUploaderIds,
    setSelectedUploaderIds,
    page,
    setPage,
    totalCount,
    totalPages
  } = useLibraryPageState()

  const navigate = useNavigate()
  return (
    <div
      style={{
        width: '100%',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        overflow: 'hidden'
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
            <TagSelectFilter selectedTags={selectedTags} setSelectedTags={setSelectedTags} />
          </Col>
          <Col flex="auto">
            <UploaderSelectFilter
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
          <Row gutter={[16, 16]} style={{ width: '75%' }}>
            {lessons.map((lesson) => (
              <Col key={lesson.mongoId} span={12}>
                <LessonCard lesson={lesson} onStarToggled={reload} onClick={() => navigate(`/lesson/${lesson.uploadId}`)} />
              </Col>
            ))}
          </Row>
          {totalPages > 1 && (
            <Pagination
              current={page}
              total={totalCount}
              pageSize={10}
              onChange={(newPage) => setPage(newPage)}
              style={{ marginTop: 24 }}
              showSizeChanger={false}
            />
          )}
        </>
      )}
    </div>
  )
}

export default LibraryPage
