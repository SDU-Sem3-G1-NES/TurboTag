import React, { useEffect, useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { FileClient, LessonClient, LessonDto } from "../api/apiClient";
import { Card, Spin, Typography, Tag } from "antd";
import { LoadingOutlined } from "@ant-design/icons";

const { Title, Paragraph } = Typography;

const LessonPage: React.FC = () => {
    const [loading, setLoading] = useState(true);
    const lessonCLient = useMemo(() => new LessonClient(), []);
    const fileClient = useMemo(() => new FileClient(), []);
    const { uploadId } = useParams<{ uploadId: string }>();
    const [lesson, setLesson] = useState<LessonDto>();
    const [file, setFile] = useState<string>();

    useEffect(() => {
        const fetchLesson = async () => {
            try {
                const data = await lessonCLient.getLessonByUploadId(Number(uploadId));

                const fileId = data.fileMetadata?.[0]?.id ?? undefined;
                //const fileData = await fileClient.getFileById(fileId);
                setLesson(data);
            } catch (error) {
                console.error("Error fetching lesson:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchLesson();
    }, [uploadId]);

    if (loading) {
        return (
            <div style={{ display: "flex", justifyContent: "center", alignItems: "center", height: "60vh" }}>
                <Spin indicator={<LoadingOutlined style={{ fontSize: 48 }} spin />} />
            </div>
        );
    }

    if (!lesson) {
        return (
            <div style={{ textAlign: "center", marginTop: 40 }}>
                <Title level={3}>404 Lesson not found</Title>
            </div>
        );
    }

    return (
        <div style={{ display: "flex", justifyContent: "center", marginTop: 20}}>
            <Card style={{ width: 1000 }}>
                <Card style={{ height: 500, backgroundColor: "lightGray" }}>
                </Card>
                <div style={{ marginTop: 20 }}>
                    <Title level={3}>{lesson.lessonDetails?.title}</Title>
                    {lesson.lessonDetails?.tags?.map((tag, index) => (
                    <Tag color="blue" key={index} style={{ marginRight: 4 }}>
                        {tag}
                    </Tag>
                    ))}
                </div>
                <Card style={{ height: 100, backgroundColor: "lightGray", marginTop: 20 }}>
                    <Title level={5}>Description</Title>
                    <Paragraph>{lesson.lessonDetails?.description}</Paragraph>
                </Card>
            </Card>
        </div>  
    );
};

export default LessonPage;