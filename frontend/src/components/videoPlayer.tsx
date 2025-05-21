import React, { useEffect, useMemo, useRef, useState } from 'react';
import { Card } from 'antd';
import { FileClient } from '../api/apiClient';

export interface VideoPlayerProps {
    videoId: string;
}

const VideoPlayer: React.FC<VideoPlayerProps> = ({ videoId }) => {
    const [videoSrc, setVideoSrc] = useState<string>();
    const videoRef = useRef<HTMLVideoElement>(null);
    const fileClient = useMemo(() => new FileClient(), []);

    useEffect(() => {
        const fetchVideo = async () => {
            try {
                const result = await fileClient.streamVideo(videoId);
                const blob = result.data;
                setVideoSrc(URL.createObjectURL(blob));
            } catch (error) {
                console.error('Error fetching video:', error);
            }
        };
        if (videoId) {
            fetchVideo();
        }
        return () => {
            if (videoSrc) URL.revokeObjectURL(videoSrc);
        };
    }, [videoId]);

    return (
        <Card style={{ boxShadow: '0 4px 16px rgba(0,0,0,0.15)' }}>
            <video
                ref={videoRef}
                src={videoSrc}
                controls
                style={{ width: '100%' }}
                preload="auto"
            />
        </Card>
    );
};

export default VideoPlayer;