import React from 'react';
import { Card, Image, Tag, Typography } from 'antd';
import { LessonDto } from '../../api/apiClient';

interface LibraryItemProps {
    lesson: LessonDto;
}

const LibraryItem: React.FC<LibraryItemProps> = ({ lesson }) => {
    const limitDescription = (text: string | undefined, maxLength: number): string => {
    if (!text) return '';
    return text.length > maxLength ? `${text.substring(0, maxLength)}...` : text;
    };
    
    return (
        <Card
            hoverable
            style={{
                width: "40%",
                margin: 8
            }}
        >
            <div style={{ display: 'flex', alignItems: 'center' }}>
                <Image
                    src="https://placehold.co/100x100"
                    preview={false}
                    style={{
                        width: 100,
                        height: 100,
                        marginRight: 16,
                    }}
                />
                <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
                    <Typography.Text style={{ fontSize: 16, fontWeight: 'bold' }}>
                        {lesson.lessonDetails?.title}
                    </Typography.Text>
                    <Typography.Text style={{ fontSize: 12 }}>
                        {limitDescription(lesson.lessonDetails?.description ?? undefined, 50)}
                    </Typography.Text>
                    <div>
                        {lesson.lessonDetails?.tags?.slice(0, 4).map((tag, index) => (
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
                    <Typography.Text style={{ fontSize: 10, fontStyle: 'italic', color: 'rgba(0, 0, 0, 0.45)' }}>
                        {lesson.fileMetadata?.[0]?.date
                            ? 'Uploaded: ' +
                            new Date(lesson.fileMetadata[0].date).toLocaleString('da-DK', {
                                year: 'numeric',
                                month: '2-digit',
                                day: '2-digit',
                                hour: '2-digit',
                                minute: '2-digit',
                            })
                            : 'N/A'}
                    </Typography.Text>
                </div>
            </div>
        </Card> 
    );
};

export default LibraryItem;