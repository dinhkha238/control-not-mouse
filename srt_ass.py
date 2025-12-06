import os
import re
import sys
import subprocess
from concurrent.futures import ThreadPoolExecutor
from datetime import timedelta

# ======= Cấu hình =======
max_threads = 6  # Tùy CPU của bạn


# ======= Hỗ trợ thời gian =======
def seconds_to_ass_time(sec):
    td = timedelta(seconds=sec)
    total_seconds = int(td.total_seconds())
    ms = int((sec - total_seconds) * 100)
    h = total_seconds // 3600
    m = (total_seconds % 3600) // 60
    s = total_seconds % 60
    return f"{h:d}:{m:02d}:{s:02d}.{ms:02d}"


# ======= Đọc TXT =======
def parse_txt_blocks(txt_path):
    with open(txt_path, "r", encoding="utf-8") as f:
        lines = f.read().split("\n\n")  # Mỗi đoạn cách nhau 1 dòng trống
    blocks = []
    for line in lines:
        line = line.strip()
        if line:
            blocks.append({"text": line})
    return blocks


# ======= Tạo ASS highlight =======
def create_highlight_ass(text, output_file, display_duration=9):
    marked = re.findall(r"\[(.+?)\]", text)
    if not marked:
        return

    events = []
    for mark in marked:
        total_ms = int(display_duration * 1000)
        reveal_ms = int(total_ms * 0.167)  # 1/6 thời gian: xuất hiện
        hold_ms = int(total_ms * 0.667)  # 2/3 thời gian: giữ nguyên
        hide_ms = total_ms - reveal_ms - hold_ms

        start_display = seconds_to_ass_time(0)
        end_display = seconds_to_ass_time(display_duration)

        text_line = (
            f"Dialogue: 0,{start_display},{end_display},Highlight,,0,0,0,,"
            f"{{\\an2\\clip(0,900,0,1080)"
            f"\\t(0,{reveal_ms},\\clip(0,900,1920,1080))"
            f"\\t({reveal_ms + hold_ms},{reveal_ms + hold_ms + hide_ms},\\clip(0,900,0,1080))"
            f"\\fad(50,50)}}{mark}"
        )
        events.append(text_line)

    ass_header = """[Script Info]
Title: Highlight
ScriptType: v4.00+
PlayResX: 1920
PlayResY: 1080
WrapStyle: 2
ScaledBorderAndShadow: yes

[V4+ Styles]
Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding
Style: Highlight,Arial,60,&H00FFFFFF,&H000000FF,&H33000000,&HAA000000,-1,0,0,0,100,100,0,0,3,6,0,2,40,40,48,1

[Events]
Format: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text
"""

    with open(output_file, "w", encoding="utf-8") as f:
        f.write(ass_header + "\n".join(events))


# ======= GPU encode video + sub =======
def add_sub_to_video(video_path, ass_path, output_path):
    safe_ass = ass_path.replace("\\", "/")
    ass_filter = f"ass='{safe_ass}'"

    cmd = [
        "ffmpeg",
        "-y",
        "-hwaccel",
        "cuda",
        "-i",
        video_path,
        "-vf",
        ass_filter,
        "-r",
        "30",
        "-c:v",
        "h264_nvenc",
        "-preset",
        "p4",
        "-b:v",
        "5M",
        "-cq",
        "19",
        "-c:a",
        "copy",
        output_path,
    ]

    print(
        f"🚀 GPU Encoding with SUB: {os.path.basename(video_path)} → {os.path.basename(output_path)}"
    )
    try:
        subprocess.run(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=True)
        print(f"✅ Done: {os.path.basename(output_path)}")
    except subprocess.CalledProcessError as e:
        print(f"❌ Lỗi khi xử lý {os.path.basename(video_path)}")
        print(e.stderr.decode("utf-8", errors="ignore"))


# ======= GPU convert video to 30fps =======
def convert_to_30fps(video_path, output_path):
    cmd = [
        "ffmpeg",
        "-y",
        "-hwaccel",
        "cuda",
        "-i",
        video_path,
        "-r",
        "30",
        "-c:v",
        "h264_nvenc",
        "-preset",
        "p4",
        "-b:v",
        "5M",
        "-cq",
        "19",
        "-c:a",
        "copy",
        output_path,
    ]

    print(
        f"⚙️ GPU Convert FPS: {os.path.basename(video_path)} → {os.path.basename(output_path)}"
    )
    try:
        subprocess.run(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, check=True)
        print(f"✅ Done: {os.path.basename(output_path)}")
    except subprocess.CalledProcessError as e:
        print(f"❌ Lỗi khi xử lý {os.path.basename(video_path)}")
        print(e.stderr.decode("utf-8", errors="ignore"))


# ======= Main =======
def main():
    if len(sys.argv) < 4:
        print("❌ Thiếu tham số. Cú pháp:")
        print(
            "python script.py <input_txt hoặc ''> <input_videos_folder> <output_folder>"
        )
        return

    input_txt = sys.argv[1].strip()
    input_videos_folder = sys.argv[2].strip()
    output_folder = sys.argv[3].strip()

    os.makedirs(output_folder, exist_ok=True)

    videos = sorted(
        [
            f
            for f in os.listdir(input_videos_folder)
            if f.lower().endswith((".mp4", ".mov", ".mkv"))
        ]
    )

    if not videos:
        print("❌ Không tìm thấy video trong thư mục đầu vào.")
        return

    # 🟢 Nếu KHÔNG có file TXT → chỉ convert sang 30fps
    if not input_txt or not os.path.exists(input_txt):
        print("⚠️ Không có file TXT → chỉ convert sang 30fps (GPU).")
        with ThreadPoolExecutor(max_workers=max_threads) as executor:
            for i, video in enumerate(videos):
                video_path = os.path.join(input_videos_folder, video)
                output_path = os.path.join(output_folder, f"converted_{i+1:03}.mp4")
                executor.submit(convert_to_30fps, video_path, output_path)
        return

    # 🟢 Có file TXT → xử lý highlight
    blocks = parse_txt_blocks(input_txt)
    blocks = [b for b in blocks if "[" in b["text"] and "]" in b["text"]]
    if not blocks:
        print("⚠️ File TXT không có highlight, chuyển sang convert FPS.")
        with ThreadPoolExecutor(max_workers=max_threads) as executor:
            for i, video in enumerate(videos):
                video_path = os.path.join(input_videos_folder, video)
                output_path = os.path.join(output_folder, f"converted_{i+1:03}.mp4")
                executor.submit(convert_to_30fps, video_path, output_path)
        return

    print(f"📘 Tổng highlight: {len(blocks)}, 🎬 Tổng video: {len(videos)}")

    base_dir = os.path.dirname(os.path.abspath(__file__))
    mapping_file = os.path.join(base_dir, "txt_folder", "mapping.txt")

    with open(mapping_file, "w", encoding="utf-8") as f:
        pass

    with ThreadPoolExecutor(max_workers=max_threads) as executor:
        for i, block in enumerate(blocks):
            vid_index = i % len(videos)
            video_path = os.path.join(input_videos_folder, videos[vid_index])
            ass_path = os.path.join(output_folder, f"sub_{i+1:03}.ass")
            output_path = os.path.join(output_folder, f"clip{i+1}.mp4")

            create_highlight_ass(block["text"], ass_path)
            marked = re.findall(r"\[(.+?)\]", block["text"])
            with open(mapping_file, "a", encoding="utf-8") as f:
                f.write(f"{marked[0]}=clip{i+1}.mp4\n")

            executor.submit(add_sub_to_video, video_path, ass_path, output_path)

    print("✅ Hoàn tất tất cả video!")


if __name__ == "__main__":
    main()
