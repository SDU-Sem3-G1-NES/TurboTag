import React, { useState } from 'react'
import { Card, Image, Tag, Typography } from 'antd'
import { LessonClient, LessonDto } from '../api/apiClient'
import { StarFilled, StarOutlined } from '@ant-design/icons'

interface LessonCardProps {
  lesson: LessonDto
  onStarToggled?: () => void
}

const LessonCard: React.FC<LessonCardProps> = ({ lesson, onStarToggled }) => {
  const [isStarred, setIsStarred] = useState<boolean>(lesson.isStarred ?? false)

  const userId = parseInt(localStorage.getItem('userId') ?? '0', 10)
  const lessonClient = new LessonClient()

  const toggleStar = async () => {
    try {
      if (isStarred) {
        await lessonClient.unstarLesson(userId, lesson.uploadId!)
      } else {
        await lessonClient.starLesson(userId, lesson.uploadId!)
      }
      setIsStarred(!isStarred)
      onStarToggled?.()
    } catch (err) {
      console.error('Failed to toggle star:', err)
    }
  }

  const limitDescription = (text: string | undefined, maxLength: number): string => {
    if (!text) return ''
    return text.length > maxLength ? `${text.substring(0, maxLength)}...` : text
  }

  return (
    <Card hoverable style={{ margin: 8, position: 'relative' }} bodyStyle={{ paddingRight: 32 }}>
      <div
        style={{
          position: 'absolute',
          top: 8,
          right: 8,
          fontSize: 20,
          cursor: 'pointer'
        }}
        onClick={toggleStar}
        title={isStarred ? 'Unstar this lesson' : 'Star this lesson'}>
        {isStarred ? <StarFilled style={{ color: '#fadb14' }} /> : <StarOutlined />}
      </div>

      <div style={{ display: 'flex', flexWrap: 'wrap', alignItems: 'center' }}>
        <Image
          src="https://placehold.co/100x100"
          preview={false}
          style={{ maxWidth: 100, maxHeight: 100, marginRight: 16 }}
        />
        <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
          <Typography.Text style={{ fontSize: 16, fontWeight: 'bold' }}>
            {lesson.lessonDetails?.title}
          </Typography.Text>
          <Typography.Text style={{ fontSize: 12 }}>
            {limitDescription(lesson.lessonDetails?.description ?? undefined, 50)}
          </Typography.Text>
          <div>
            {lesson.lessonDetails?.tags?.slice(0, 4).map((tag: string, index: number) => (
              <Tag color="blue" key={index} style={{ marginRight: 4 }}>
                {tag}
              </Tag>
            ))}
            {lesson.lessonDetails?.tags && lesson.lessonDetails.tags.length > 4 && (
              <Tag color="blue" style={{ marginRight: 4 }}>
                +{lesson.lessonDetails.tags.length - 4}
              </Tag>
            )}
          </div>
          <div>
            <Typography.Text
              style={{ fontSize: 10, fontStyle: 'italic', color: 'rgba(0, 0, 0, 0.45)' }}>
              {'Uploaded by ' + lesson.ownerName}
            </Typography.Text>
            <Typography.Text
              style={{ fontSize: 10, fontStyle: 'italic', color: 'rgba(0, 0, 0, 0.45)' }}>
              {lesson.fileMetadata?.[0]?.date
                ? '  at ' +
                  new Date(lesson.fileMetadata[0].date).toLocaleString('da-DK', {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit'
                  })
                : ' at Unknown Time'}
            </Typography.Text>
          </div>
        </div>
      </div>
    </Card>
  )
}

export default LessonCard
