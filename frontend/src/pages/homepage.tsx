import React from 'react'
import { Col, Input, Row, Spin } from 'antd'
import { LoadingOutlined } from '@ant-design/icons'
import LibraryItem from '../components/library/libraryItem'
import { useHomePageState } from './hooks/useHomepageState'

const HomePage: React.FC = () => {
  const { ownerLessons, loading, search, setSearch, handleSearch } = useHomePageState()

  return (
    <div style={{ width: '100%', display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <Input.Search
        placeholder="Search by title or description"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        onSearch={(value) => handleSearch(value)} // when user presses Enter or clicks icon
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
          <Row gutter={[16, 16]} style={{ width: '75%' }}>
            <Col span={24}>
              <h1 style={{ textAlign: 'left' }}>Your Uploads</h1>
            </Col>
          </Row>
          <Row gutter={[16, 16]} style={{ width: '75%' }}>
            {ownerLessons.map((lesson) => (
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
