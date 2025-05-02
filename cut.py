import math
from moviepy.video.io.ffmpeg_tools import ffmpeg_extract_subclip
from moviepy.video.io.VideoFileClip import VideoFileClip
import os
from concurrent.futures import ThreadPoolExecutor
import sys


# Hàm cắt video
def cut_video_segment(video_path, start_time, end_time, output_path):
    ffmpeg_extract_subclip(video_path, start_time, end_time, targetname=output_path)
    print(f"Đã cắt đoạn từ {start_time} đến {end_time} giây và lưu vào {output_path}")


def cut_video(input_video_path, output_directory, video_index):
    # Mở video để lấy độ dài
    with VideoFileClip(input_video_path) as video:
        video_duration = video.duration  # Độ dài video tính bằng giây

    # Sử dụng ThreadPoolExecutor để cắt video đa luồng
    segment_duration = 5.0  # Thời lượng mỗi phân đoạn (giây)
    with ThreadPoolExecutor() as executor:
        futures = []
        segment_index = 1
        num_segments = math.ceil(video_duration / segment_duration)

        for i in range(num_segments):
            start_time = i * segment_duration + 0.5
            end_time = start_time + segment_duration - 1
            output_path = os.path.join(
                output_directory, f"{video_index}-{segment_index}.mp4"
            )
            futures.append(
                executor.submit(
                    cut_video_segment,
                    input_video_path,
                    start_time,
                    end_time,
                    output_path,
                )
            )
            segment_index += 1

        # Đợi tất cả các luồng hoàn thành
        for future in futures:
            future.result()

    print(f"Hoàn thành cắt video {video_index} thành các đoạn 5 giây.")


# Lấy tham số từ dòng lệnh
if __name__ == "__main__":
    video_path = sys.argv[1]
    output_path = sys.argv[2]
    video_index = int(sys.argv[3])

    # Gọi hàm với các tham số từ dòng lệnh
    cut_video(video_path, output_path, video_index)
