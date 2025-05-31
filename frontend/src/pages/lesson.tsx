import React, { useEffect, useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'
import { LessonClient, LessonDto } from '../api/apiClient'
import { Card, Spin, Typography, Tag, notification } from 'antd'
import { LoadingOutlined } from '@ant-design/icons'
import VideoPlayer from '../components/videoPlayer'

const { Title, Paragraph } = Typography

const LessonPage: React.FC = () => {
  const [loading, setLoading] = useState(true)
  const lessonClient = useMemo(() => new LessonClient(), [])
  const { uploadId } = useParams<{ uploadId: string }>()
  const [lesson, setLesson] = useState<LessonDto>()

  useEffect(() => {
    let intervalId: NodeJS.Timeout | null = null

    const fetchLesson = async () => {
      try {
        const data = await lessonClient.getLessonByUploadId(Number(uploadId))
        setLesson(data)

        const isGenerating =
          !data.lessonDetails?.description || (data.lessonDetails?.tags?.length ?? 0) === 0

        if (isGenerating) {
          intervalId = setInterval(async () => {
            try {
              const refreshed = await lessonClient.getLessonByUploadId(Number(uploadId))

              if (
                refreshed.lessonDetails?.description &&
                refreshed.lessonDetails.tags?.length > 0
              ) {
                setLesson(refreshed)
                if (intervalId) clearInterval(intervalId)

                notification.success({
                  message: 'Content generation complete',
                  description: 'Tags and description are now available.',
                  placement: 'topRight',
                  duration: 3
                })
              }
            } catch (err) {
              console.error('Error polling for updates:', err)
            }
          }, 5000)
        }
      } catch (error) {
        console.error('Error fetching lesson:', error)
      } finally {
        setLoading(false)
      }
    }

    fetchLesson()

    return () => {
      if (intervalId) clearInterval(intervalId)
    }
  }, [uploadId])

  const isGenerating =
    !lesson?.lessonDetails?.description || (lesson.lessonDetails?.tags?.length ?? 0) === 0

  if (loading) {
    return (
      <div
        style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '60vh' }}
      >
        <Spin indicator={<LoadingOutlined style={{ fontSize: 48 }} spin />} />
      </div>
    )
  }

  if (!lesson) {
    return (
      <div style={{ textAlign: 'center', marginTop: 40 }}>
        <Title level={3}>404 Lesson not found</Title>
      </div>
    )
  }

  return (
    <div
      style={{
        width: '80%',
        maxWidth: 1200,
        margin: '20px auto',
        display: 'flex',
        flexDirection: 'column',
        gap: 16
      }}
    >
      <div style={{ alignSelf: 'center', width: '100%' }}>
        <VideoPlayer videoId={lesson.fileMetadata?.[0]?.id ?? ''} />
      </div>

      <Card style={{ width: '100%' }}>
        <Title level={3} style={{ marginBottom: 8 }}>
          {lesson.lessonDetails?.title}
        </Title>
        {isGenerating ? (
          <Spin indicator={<LoadingOutlined />} style={{ marginBottom: 16 }} />
        ) : (
          <div style={{ marginBottom: 16 }}>
            {lesson.lessonDetails?.tags?.map((tag, i) => (
              <Tag color="blue" key={i} style={{ margin: '0 4px 4px 0' }}>
                {tag}
              </Tag>
            ))}
          </div>
        )}

        <Card
          style={{
            backgroundColor: 'lightGray',
            marginTop: 0,
            boxShadow: '0 4px 16px rgba(0,0,0,0.15)'
          }}
        >
          <Title level={5}>Summary</Title>
          {isGenerating ? (
            <Spin indicator={<LoadingOutlined />} />
          ) : (
            <Paragraph>{lesson.lessonDetails?.description}</Paragraph>
          )}
        </Card>
      </Card>
    </div>
  )
}

export default LessonPage
