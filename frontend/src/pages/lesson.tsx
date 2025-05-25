import React, { useEffect, useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { FileClient, LessonClient, LessonDto } from "../api/apiClient";
import { Card, Spin, Typography, Tag, notification } from "antd";
import { LoadingOutlined } from "@ant-design/icons";
import VideoPlayer from "../components/videoPlayer";

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
                setLesson(data);

                const isGenerating =
                  !data.lessonDetails?.description ||
                  (data.lessonDetails?.tags?.length ?? 0) === 0;

                if (isGenerating) {
                    const interval = setInterval(async () => {
                        try {
                            const refreshed = await lessonCLient.getLessonByUploadId(Number(uploadId));

                            if (
                              refreshed.lessonDetails?.description &&
                              refreshed.lessonDetails.tags?.length > 0
                            ) {
                                setLesson(refreshed);
                                clearInterval(interval);

                                notification.success({
                                    message: "Content generation complete",
                                    description: "Tags and description are now available.",
                                    placement: "topRight",
                                    duration: 3
                                });
                            }
                        } catch (err) {
                            console.error("Error polling for updates:", err);
                        }
                    }, 5000);

                    return () => clearInterval(interval);
                }
            } catch (error) {
                console.error("Error fetching lesson:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchLesson();
    }, [uploadId]);

    const isGenerating =
      !lesson?.lessonDetails?.description ||
      (lesson.lessonDetails?.tags?.length ?? 0) === 0;

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
      <div style={{ display: "flex", justifyContent: "center", marginTop: 20 }}>
          <Card style={{ width: 1000 }}>
              <VideoPlayer videoId={lesson.fileMetadata?.[0]?.id ?? ""} />
              <div style={{ marginTop: 20 }}>
                  <Title level={3}>{lesson.lessonDetails?.title}</Title>
                  {isGenerating ? (
                    <Spin indicator={<LoadingOutlined />} style={{ marginLeft: 8 }} />
                  ) : (
                    lesson.lessonDetails?.tags?.map((tag, index) => (
                      <Tag color="blue" key={index} style={{ marginRight: 4 }}>
                          {tag}
                      </Tag>
                    ))
                  )}
              </div>
              <Card style={{ height: 100, backgroundColor: "lightGray", marginTop: 20 }}>
                  <Title level={5}>Description</Title>
                  {isGenerating ? (
                    <Spin indicator={<LoadingOutlined />} />
                  ) : (
                    <Paragraph>{lesson.lessonDetails?.description}</Paragraph>
                  )}
              </Card>
          </Card>
      </div>
    );
};

export default LessonPage;
