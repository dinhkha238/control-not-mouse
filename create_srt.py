import sys
from datetime import timedelta
import re
import pysrt
import os


# ========================== Các hàm hỗ trợ ==========================
def smart_split_sentences(text):
    abbreviations = [
        "Mr.",
        "Mrs.",
        "Ms.",
        "Dr.",
        "Prof.",
        "J.",
        "A.",
        "B.",
        "C.",
        "D.",
        "E.",
        "U.S.",
    ]
    protected_spans = [(m.start(), m.end()) for m in re.finditer(r"\[[^\]]+\]", text)]

    def inside_protected(idx):
        return any(s <= idx < e for s, e in protected_spans)

    sentences = []
    buffer = ""
    i = 0
    while i < len(text):
        ch = text[i]
        buffer += ch
        if ch in ".?!":
            if inside_protected(i):
                i += 1
                continue
            if any(text[i - len(abbr) + 1 : i + 1] == abbr for abbr in abbreviations):
                i += 1
                continue
            if i + 1 < len(text) and text[i + 1].islower():
                i += 1
                continue
            sentences.append(buffer.strip())
            buffer = ""
        i += 1
    if buffer.strip():
        sentences.append(buffer.strip())
    return sentences


def split_txt_into_segments(txt_path):
    with open(txt_path, "r", encoding="utf-8") as f:
        raw = f.read().strip()
    segments = [seg.strip() for seg in raw.split("\n\n") if seg.strip()]
    results = [smart_split_sentences(seg) for seg in segments]
    return results


def get_middle_word(sentence):
    words = sentence.split()
    if not words:
        return None, 0, 0
    return words[len(words) // 2], len(words) // 2, len(words)


def map_marked_words_to_timestamps(goc_segments, srt_file):
    srt_subs = pysrt.open(srt_file, encoding="utf-8")
    dich_segments = [
        {
            "start": sub.start,
            "end": sub.end,
            "sentences": smart_split_sentences(sub.text.replace("\n", " ")),
        }
        for sub in srt_subs
    ]

    results = []

    def time_to_seconds(t):
        return t.hours * 3600 + t.minutes * 60 + t.seconds + t.milliseconds / 1000

    for seg_idx, goc_sentences in enumerate(goc_segments):
        if seg_idx >= len(dich_segments):
            break
        dich_sentences = dich_segments[seg_idx]["sentences"]
        start_sec = time_to_seconds(dich_segments[seg_idx]["start"])
        end_sec = time_to_seconds(dich_segments[seg_idx]["end"])
        segment_duration = end_sec - start_sec
        total_words = sum(len(s.split()) for s in dich_sentences)
        if total_words == 0:
            total_words = 1

        for sent_idx, sentence in enumerate(goc_sentences):
            if sent_idx >= len(dich_sentences):
                break
            marked_words = re.findall(r"\[([^\]]+)\]", sentence)
            if not marked_words:
                continue
            dich_sentence = dich_sentences[sent_idx]
            middle_word, middle_idx, sentence_len = get_middle_word(dich_sentence)
            words_before = (
                sum(len(s.split()) for s in dich_sentences[:sent_idx]) + middle_idx
            )
            time_offset_sec = (words_before / total_words) * segment_duration
            word_time = timedelta(seconds=start_sec + time_offset_sec)
            for mw in marked_words:
                results.append(
                    {
                        "goc_sentence": sentence,
                        "marked_word": mw,
                        "dich_sentence": dich_sentence,
                        "middle_word": middle_word,
                        "start_time": dich_segments[seg_idx]["start"],
                        "end_time": dich_segments[seg_idx]["end"],
                        "Time": word_time,
                    }
                )
    return results


def write_results_to_srt(results, output_srt):
    with open(output_srt, "w", encoding="utf-8") as f:
        for idx, item in enumerate(results, start=1):
            text = item["marked_word"]
            t = item["Time"]
            total = t.total_seconds()
            hours = int(total // 3600)
            minutes = int((total % 3600) // 60)
            seconds = total % 60
            timestamp = f"{hours}:{minutes:02}:{seconds:09.6f}"
            f.write(f"{idx}\n")
            f.write(f"{timestamp}\n")
            f.write(f"{text}\n\n")


# ========================== Hàm chính ==========================
def process_txt_and_srt_list(txt_file, srt_list_file, path_highlight):
    goc_segments = split_txt_into_segments(txt_file)

    # Tạo thư mục highlight nếu chưa có
    os.makedirs(path_highlight, exist_ok=True)

    # File log
    highlight_file = os.path.join(path_highlight, "highlights.txt")

    # Ghi mới hoàn toàn mỗi lần chạy
    with open(highlight_file, "w", encoding="utf-8") as highlight:

        with open(srt_list_file, "r", encoding="utf-8") as f:
            srt_files = [line.strip() for line in f if line.strip()]

        for srt_file in srt_files:
            if not os.path.exists(srt_file):
                continue

            results = map_marked_words_to_timestamps(goc_segments, srt_file)
            base_name = os.path.splitext(os.path.basename(srt_file))[0]

            output_srt = os.path.join(path_highlight, f"{base_name}_output.srt")
            write_results_to_srt(results, output_srt)

            highlight.write(output_srt + "\n")


# ========================== CMD ENTRY ==========================
if __name__ == "__main__":
    if len(sys.argv) != 4:
        print("Cách dùng: python script.py <txt_file> <srt_list_file> <path_highlight>")
        sys.exit(1)

    txt_file = sys.argv[1]
    srt_list_file = sys.argv[2]
    path_highlight = sys.argv[3]
    process_txt_and_srt_list(txt_file, srt_list_file, path_highlight)
