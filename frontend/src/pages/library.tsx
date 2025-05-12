import React, { useEffect, useState } from 'react';
import { LessonClient, LessonDto } from '../api/apiClient';
import { Flex } from 'antd';
import LibraryItem from '../components/library/libraryItem';

const Library: React.FC = () => {
    const lessonClient = new LessonClient();
    const [lessons, setLessons] = useState<LessonDto[]>([]);
    
    const fetchUploadData = async () => {
        try {
            const data = await lessonClient.getAllLessons();
            setLessons(Array.isArray(data) ? data : data.items || []);
        } catch (error) {
            console.error('Error fetching lesson data:', error);
        }
    };
    useEffect(() => {
        fetchUploadData();
    }, []);
    return (
        <Flex style={{width: '75%', height: '100%', flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-around'}}>
            {Array.isArray(lessons) && lessons.map((lesson: any) => (
            <LibraryItem
                key={lesson.mongoId}
                lesson={lesson}
            />
            ))}
        </Flex>
    );
};

export default Library;