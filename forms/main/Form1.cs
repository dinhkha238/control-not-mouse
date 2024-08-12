using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave;
using Newtonsoft.Json;

namespace WinFormsApp;

public partial class Form1 : Form
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool EnumChildWindows(IntPtr hwndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

    public const int MOUSEEVENTF_LEFTDOWN = 0x02;
    public const int MOUSEEVENTF_LEFTUP = 0x04;
    delegate bool EnumChildProc(IntPtr hwnd, IntPtr lParam);
    const byte VK_MENU = 0x12; // Alt key
    const uint KEYEVENTF_KEYUP = 0x0002;
    const int BM_CLICK = 0x00F5;

    const string PROSHOW_TITLE = "ProShow Producer - I Love You - combined.psh"; // Cập nhật tiêu đề cửa sổ nếu cần thiết

    public Form1()
    {
        InitializeComponent();
    }
    private void button7_Click(object sender, EventArgs e)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"finals\combined.psh");
        string proShowPath = "";
        // Kiểm tra và đọc file pathProshow.txt
        if (File.Exists("pathProshow.txt"))
        {
            proShowPath = File.ReadAllText("pathProshow.txt");
        }
        // Kiểm tra xem tệp thực thi và tệp .psh có tồn tại không
        if (!System.IO.File.Exists(proShowPath))
        {
            MessageBox.Show("Đường dẫn tới ProShow không đúng.");
            return;
        }

        if (!System.IO.File.Exists(filePath))
        {
            MessageBox.Show("Đường dẫn tới tệp .psh không đúng.");
            return;
        }

        // Tạo một đối tượng ProcessStartInfo để khởi động ProShow với tệp .psh
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = proShowPath,
            Arguments = filePath,
            UseShellExecute = true
        };

        // Khởi động ProShow với tệp .psh
        try
        {
            Process process = Process.Start(startInfo);
            Console.WriteLine("ProShow đã được khởi động.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Đã xảy ra lỗi khi khởi động ProShow: " + ex.Message);
        }
        IntPtr proShowHandle = IntPtr.Zero;
        int timeout = 20000; // 20 giây
        int interval = 100; // Kiểm tra mỗi 100ms
        int elapsed = 0;
        while (elapsed < timeout)
        {
            proShowHandle = FindWindow(null, PROSHOW_TITLE); // Cập nhật tiêu đề cửa sổ nếu cần thiết
            if (proShowHandle != IntPtr.Zero)
            {
                break;
            }
            // Chờ 100ms trước khi kiểm tra lại
            Thread.Sleep(interval);
            elapsed += interval;
        }
        SetForegroundWindow(proShowHandle);
        Thread.Sleep(2000); // Đợi một chút để hộp thoại mở

        // Nhấn tổ hợp phím alt+f3
        keybd_event(VK_MENU, 0, 0, UIntPtr.Zero); // Alt down
        keybd_event(0x72, 0, 0, UIntPtr.Zero); // F3 down
        keybd_event(0x72, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // F3 up
        keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Alt up

        Thread.Sleep(2000); // Đợi một chút để hộp thoại mở

        EnumChildWindows(proShowHandle, (hwnd, lParam) =>
        {
            StringBuilder className = new StringBuilder(256);
            GetClassName(hwnd, className, className.Capacity);
            if (className.ToString() == "Button")
            {
                StringBuilder windowText = new StringBuilder(256);
                GetWindowText(hwnd, windowText, windowText.Capacity);
                if (windowText.ToString() == "Video")
                {
                    PostMessage(hwnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                    return false; // Stop enumerating
                }
            }
            return true; // Continue enumerating
        }, IntPtr.Zero);

        elapsed = 0;
        IntPtr showOptionHandle = IntPtr.Zero;
        while (elapsed < timeout)
        {
            showOptionHandle = FindWindow(null, "Video for Web, Devices and Computers");
            if (showOptionHandle != IntPtr.Zero)
            {
                break;
            }
            // Chờ 100ms trước khi kiểm tra lại
            Thread.Sleep(interval);
            elapsed += interval;
        }

        Thread.Sleep(1000); // Đợi một chút để hộp thoại mở
        EnumChildWindows(showOptionHandle, (hwnd, lParam) =>
        {
            StringBuilder className = new StringBuilder(256);
            GetClassName(hwnd, className, className.Capacity);
            if (className.ToString() == "Button")
            {
                StringBuilder windowText = new StringBuilder(256);
                GetWindowText(hwnd, windowText, windowText.Capacity);
                if (windowText.ToString() == "Create")
                {
                    PostMessage(hwnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                    return false; // Stop enumerating
                }
            }
            return true; // Continue enumerating
        }, IntPtr.Zero);
    }

    private void AddButtons(int buttonCount)
    {
        // Tạo Panel để chứa các Label và Button
        Panel panel = new Panel();
        panel.AutoScroll = true;
        panel.Location = new System.Drawing.Point(100, 200); // Vị trí của Panel
        panel.Size = new System.Drawing.Size(1000, 400); // Kích thước của Panel
        this.Controls.Add(panel);

        int maxRows = 8;
        int labelHeight = 20;
        int buttonHeight = 30;
        int spacing = 10;
        int buttonWidth = 100;
        int checkBoxWidth = 20;
        int textBoxWidth = 50;
        int numberLabelWidth = 30;

        for (int i = 0; i < buttonCount; i++) // Sử dụng buttonCount để xác định số hàng
        {
            Label sttLabel = new Label();
            Label numberLabel = new Label();
            Label label = new Label();
            Button button = new Button();
            Label randomLabel = new Label();
            CheckBox checkBox = new CheckBox();
            TextBox textBox = new TextBox();
            Label videoLabel = new Label();
            Button videoButton = new Button();
            Button reviewButton = new Button();
            Button randomButton = new Button();
            Label videoRandomLabel = new Label();
            CheckBox videoRandomCheckBox = new CheckBox();
            TextBox videoRandomTextBox = new TextBox();
            Button videoRandomButton = new Button();

            int x = 10;
            int y = (labelHeight + buttonHeight + spacing) * i;

            // Thiết lập nhãn "STT"
            sttLabel.Location = new System.Drawing.Point(x, y);
            sttLabel.Name = "sttLabel" + (i + 1);
            sttLabel.Size = new System.Drawing.Size(numberLabelWidth, labelHeight);
            sttLabel.Text = "STT";
            panel.Controls.Add(sttLabel);

            // Thiết lập nhãn số thứ tự
            numberLabel.Location = new System.Drawing.Point(x, y + labelHeight);
            numberLabel.Name = "numberLabel" + (i + 1);
            numberLabel.Size = new System.Drawing.Size(numberLabelWidth, labelHeight);
            numberLabel.Text = (i + 1).ToString();
            panel.Controls.Add(numberLabel);

            // Thiết lập nhãn
            label.Location = new System.Drawing.Point(x + numberLabelWidth + spacing, y);
            label.Name = "label" + (i + 1);
            label.Size = new System.Drawing.Size(buttonWidth, labelHeight);
            label.Text = "Select Image";
            label.Tag = "LabelSelectImageButton"; // Gán thuộc tính Tag để nhận diện label động
            panel.Controls.Add(label);

            // Thiết lập nút bấm
            button.Location = new System.Drawing.Point(x + numberLabelWidth + spacing, y + labelHeight);
            button.Name = "button" + (i + 1);
            button.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            button.TabIndex = i + 2; // Điều chỉnh TabIndex để tránh xung đột với các điều khiển hiện có
            button.Text = "Open";
            button.UseVisualStyleBackColor = true;
            button.Click += (sender, e) => selectedFileImagePaths_Click(sender, e, button.TabIndex - 2);
            button.Tag = "SelectImageButton"; // Gán thuộc tính Tag để nhận diện button động
            panel.Controls.Add(button);

            // Thiết lập nhãn "Chọn ảnh ngẫu nhiên"
            randomLabel.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing, y);
            randomLabel.Name = "randomLabel" + (i + 1);
            randomLabel.Size = new System.Drawing.Size(buttonWidth, labelHeight);
            randomLabel.Text = "Image Random";
            panel.Controls.Add(randomLabel);

            // Thiết lập checkbox
            checkBox.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing, y + labelHeight);
            checkBox.Name = "checkBox" + (i + 1);
            checkBox.Size = new System.Drawing.Size(checkBoxWidth, buttonHeight);
            checkBox.CheckedChanged += (sender, e) =>
            {
                button.Enabled = !checkBox.Checked;
                textBox.Enabled = checkBox.Checked;
                randomButton.Enabled = checkBox.Checked;
            };
            panel.Controls.Add(checkBox);

            // Thiết lập ô nhập số
            textBox.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing + checkBoxWidth + spacing - 10, y + labelHeight + 3);
            textBox.Name = "textBox" + (i + 1);
            textBox.Size = new System.Drawing.Size(30, 30);
            textBox.Enabled = false;
            panel.Controls.Add(textBox);

            // Thiết lập nút bấm "Random"
            randomButton.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing + checkBoxWidth + 35, y + labelHeight + 2);
            randomButton.Name = "randomButton" + (i + 1);
            randomButton.Size = new System.Drawing.Size(35, 25);
            randomButton.Text = "Lưu";
            randomButton.UseVisualStyleBackColor = true;
            randomButton.Enabled = false;
            randomButton.TabIndex = i + 5; // Điều chỉnh TabIndex để tránh xung đột với các điều khiển hiện có
            randomButton.Click += (sender, e) => randomButton_Click(sender, e, randomButton.TabIndex - 5);
            panel.Controls.Add(randomButton);

            // Thiết lập nhãn "Select Video"
            videoLabel.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing + checkBoxWidth + textBoxWidth + 3 * spacing, y);
            videoLabel.Name = "videoLabel" + (i + 1);
            videoLabel.Size = new System.Drawing.Size(buttonWidth, labelHeight);
            videoLabel.Text = "Select Video";
            panel.Controls.Add(videoLabel);

            // Thiết lập nút bấm "Select Video"
            videoButton.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing + checkBoxWidth + textBoxWidth + 3 * spacing, y + labelHeight);
            videoButton.Name = "videoButton" + (i + 1);
            videoButton.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            videoButton.TabIndex = i + 3; // Điều chỉnh TabIndex để tránh xung đột với các điều khiển hiện có
            videoButton.Text = "Open";
            videoButton.UseVisualStyleBackColor = true;
            videoButton.Click += (sender, e) => selectedFileVideoPaths_Click(sender, e, videoButton.TabIndex - 3);
            panel.Controls.Add(videoButton);

            // Thiết lập nhãn "Video Random"
            videoRandomLabel.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing + checkBoxWidth + textBoxWidth + buttonWidth + 4 * spacing, y);
            videoRandomLabel.Name = "videoRandomLabel" + (i + 1);
            videoRandomLabel.Size = new System.Drawing.Size(buttonWidth, labelHeight);
            videoRandomLabel.Text = "Video Random";
            panel.Controls.Add(videoRandomLabel);

            // Thiết lập checkbox "Video Random"
            videoRandomCheckBox.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing + checkBoxWidth + textBoxWidth + buttonWidth + 4 * spacing, y + labelHeight);
            videoRandomCheckBox.Name = "videoRandomCheckBox" + (i + 1);
            videoRandomCheckBox.Size = new System.Drawing.Size(checkBoxWidth, buttonHeight);
            videoRandomCheckBox.CheckedChanged += (sender, e) =>
            {
                videoButton.Enabled = !videoRandomCheckBox.Checked;
                videoRandomTextBox.Enabled = videoRandomCheckBox.Checked;
                videoRandomButton.Enabled = videoRandomCheckBox.Checked;
            };
            panel.Controls.Add(videoRandomCheckBox);

            // Thiết lập ô nhập số "Video Random"
            videoRandomTextBox.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing + checkBoxWidth + textBoxWidth + buttonWidth + 4 * spacing + checkBoxWidth + spacing - 10, y + labelHeight + 3);
            videoRandomTextBox.Name = "videoRandomTextBox" + (i + 1);
            videoRandomTextBox.Size = new System.Drawing.Size(30, 30);
            videoRandomTextBox.Enabled = false;
            panel.Controls.Add(videoRandomTextBox);

            // Thiết lập nút bấm "Video Random"
            videoRandomButton.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing + checkBoxWidth + textBoxWidth + buttonWidth + 4 * spacing + checkBoxWidth + 35, y + labelHeight + 2);
            videoRandomButton.Name = "videoRandomButton" + (i + 1);
            videoRandomButton.Size = new System.Drawing.Size(35, 25);
            videoRandomButton.Text = "Lưu";
            videoRandomButton.UseVisualStyleBackColor = true;
            videoRandomButton.Enabled = false;
            videoRandomButton.TabIndex = i + 6; // Điều chỉnh TabIndex để tránh xung đột với các điều khiển hiện có
            videoRandomButton.Click += (sender, e) => videoRandomButton_Click(sender, e, videoRandomButton.TabIndex - 6);
            panel.Controls.Add(videoRandomButton);

            // Thiết lập nút bấm "Review"
            reviewButton.Location = new System.Drawing.Point(x + numberLabelWidth + buttonWidth + 2 * spacing + checkBoxWidth + textBoxWidth + buttonWidth + 4 * spacing + checkBoxWidth + textBoxWidth + 2 * spacing + checkBoxWidth + 35, y + labelHeight);
            reviewButton.Name = "reviewButton" + (i + 1);
            reviewButton.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            reviewButton.TabIndex = i + 4; // Điều chỉnh TabIndex để tránh xung đột với các điều khiển hiện có
            reviewButton.Text = "Review";
            reviewButton.UseVisualStyleBackColor = true;
            reviewButton.Enabled = selectedFileImagePaths.Count > i && selectedFileImagePaths[i].Length > 0;
            reviewButton.Click += (sender, e) => reviewFileImage(sender, e, reviewButton.TabIndex - 4);
            panel.Controls.Add(reviewButton);
        }
    }


    private void OpenAudioButton_Click(object sender, EventArgs e)
    {
        // Tìm panel và xóa
        foreach (Control control in this.Controls)
        {
            if (control is Panel)
            {
                this.Controls.Remove(control);
                break;
            }
        }
        selectedFileImagePaths.Clear();

        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Multiselect = true; // Cho phép chọn nhiều tệp
            openFileDialog.Filter = "Audio files (*.wav, *.mp3, *.ogg)|*.wav;*.mp3;*.ogg"; // Bộ lọc tệp (các tệp âm thanh)

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFileAudioPaths = new List<string>(openFileDialog.FileNames); // Lưu các đường dẫn của các tệp đã chọn
                AddButtons(selectedFileAudioPaths.Count);
                // Khởi tạo phần tử cho selectedFileImagePaths
                for (int i = 0; i < selectedFileAudioPaths.Count; i++)
                {
                    selectedFileImagePaths.Add([]);
                }
            }
        }
    }
    private void generateSlideButton_Click(object sender, EventArgs e)
    {
        if (selectedFileAudioPaths.Count == 0)
        {
            MessageBox.Show("Please select audio files!");
            return;
        }
        for (int i = 0; i < selectedFileImagePaths.Count; i++)
        {
            if (selectedFileImagePaths[i].Length == 0)
            {
                MessageBox.Show($"Please select images for segment {i + 1}!");
                return;
            }
        }


        int length_selectedFileAudioPaths = selectedFileAudioPaths.Count;
        string settingsFilePath = "settings.json";
        if (!File.Exists(settingsFilePath))
        {
            MessageBox.Show("Settings file not found.");
            return;
        }
        string settingsContent = File.ReadAllText(settingsFilePath);
        dynamic settings = JsonConvert.DeserializeObject(settingsContent);
        string groupStyle = settings.GroupStyle;
        string groupFilePath = System.IO.Path.Combine("groups", groupStyle);
        if (!File.Exists(groupFilePath))
        {
            MessageBox.Show("Group file not found.");
            return;
        }

        string file2Path = @"files/FileProShow_2.txt";
        int totalCount = 0;

        foreach (var array in selectedFileImagePaths)
        {
            totalCount += array.Length;
        }
        using (StreamWriter writer1 = new StreamWriter(file2Path))
        {
            writer1.WriteLine($"cells={totalCount}");
            writer1.Close();
        }
        string file3Path = @"files/extractedContent.txt";
        // clear content of file3Path
        System.IO.File.WriteAllText(file3Path, string.Empty);
        int index_cell = 0;
        for (int x = 0; x < length_selectedFileAudioPaths; x++)
        {

            string[] att_in_selectedFileImagePaths = selectedFileImagePaths[x];
            int length_att_in_selectedFileImagePaths = att_in_selectedFileImagePaths.Length;
            string[] groupFileLines = System.IO.File.ReadAllLines(groupFilePath);
            // Tạo đối tượng Random
            Random random = new Random();
            // 

            for (int i = 0; i < length_att_in_selectedFileImagePaths; i++)
            {
                // Lấy ngẫu nhiên một dòng từ groupFileLines
                string selectedFile = groupFileLines[random.Next(groupFileLines.Length)];
                string path_image = "../../../../" + att_in_selectedFileImagePaths[i];
                string path_audio = "../../../../" + selectedFileAudioPaths[x];
                int length_audio = GetAudioFileLength(selectedFileAudioPaths[x]);
                float segment = length_audio / length_att_in_selectedFileImagePaths;
                WriteCellToFile(selectedFile, index_cell, path_image, path_audio, length_audio, segment, i, length_att_in_selectedFileImagePaths, file3Path);
                index_cell++;
            }
        }
        string file4Path = @"files/FileProShow_4.txt";
        using (StreamWriter writer1 = new StreamWriter(file4Path))
        {
            writer1.WriteLine($"modifierCount=0");
            writer1.Close();
        }

        string file1Path = @"files/FileProShow.txt";
        string folderContainFileProShow = Path.Combine(Directory.GetCurrentDirectory(), @"finals");
        if (Directory.Exists(folderContainFileProShow))
        {
            // // Delete all files in the folder
            // foreach (string file in Directory.GetFiles(folderContainFileProShow))
            // {
            //     File.Delete(file);
            // }
        }
        else
        {
            // Create the folder if it does not exist
            Directory.CreateDirectory(folderContainFileProShow);
        }
        string combinedFilePath = @"finals/combined.psh"; // Replace with your combined file path
        // Open the combined file for writing
        using (StreamWriter writer = new StreamWriter(combinedFilePath))
        {
            WriteFileContent(writer, file1Path);

            // Write the content of the first file
            WriteFileContent(writer, file2Path);

            // Write the content of the second file
            WriteFileContent(writer, file3Path);

            // Write the content of the third file
            WriteFileContent(writer, file4Path);
        }
        MessageBox.Show("Done!");
    }
    static int GetAudioFileLength(string filePath)
    {
        var audioFileReader = new AudioFileReader(filePath);
        TimeSpan duration = audioFileReader.TotalTime;
        int[] timeArray = { duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds };
        return (timeArray[0] * 60 * 60 + timeArray[1] * 60 + timeArray[2]) * 1000 + timeArray[3];
    }
    static void WriteCellToFile(string style_file_name, int index, string path_image, string path_audio, int length_audio, float segment, int i, int length_att_in_selectedFileImagePaths, string outputFilePath)
    {
        string fileContent = System.IO.File.ReadAllText(style_file_name);

        // Cắt nội dung từ dòng có "cells=" và kết thúc ở dòng "modifierCount="
        string startMarker = "cells=1";
        string endMarker = "modifierCount=";
        int startIndex = fileContent.IndexOf(startMarker);
        int endIndex = fileContent.IndexOf(endMarker);

        if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
        {
            // Bỏ qua dòng "cells=" và "modifierCount="
            startIndex += startMarker.Length;
            string extractedContent = fileContent.Substring(startIndex, endIndex - startIndex).Trim();

            // Loại bỏ dòng "modifierCount=" khỏi kết quả
            int endMarkerIndex = extractedContent.IndexOf(endMarker);
            if (endMarkerIndex != -1)
            {
                extractedContent = extractedContent.Substring(0, endMarkerIndex).Trim();
            }
            // Tìm kiếm và thêm dòng mới
            for (int n = 0; n <= 20; n++)
            {
                string insertImage = $"cell[0].images[{n}]";
                string searchPattern = $"cell[0].images[{n}].imageEnable=1";
                string searchPattern2 = $"cell[0].images[{n}].image=";
                string searchPattern3 = $"cell[0].images[{n}].videoVolume=100";


                if (extractedContent.Contains(searchPattern))
                {
                    if (!extractedContent.Contains(searchPattern2))
                    {
                        string newLine = $"{insertImage}.image={path_image}";
                        int insertIndex = extractedContent.IndexOf(searchPattern) + searchPattern.Length;
                        extractedContent = extractedContent.Insert(insertIndex, Environment.NewLine + newLine);
                    }
                    if (extractedContent.Contains(searchPattern3))
                    {
                        extractedContent = extractedContent.Replace(searchPattern3, $"{insertImage}.videoVolume=0");
                    }
                }
                else
                {
                    break;
                }
            }
            extractedContent = System.Text.RegularExpressions.Regex.Replace(extractedContent, @"cell\[0\]\.sound\.normalizeFrameSize=0\s*[\r\n]*", "");
            extractedContent = System.Text.RegularExpressions.Regex.Replace(extractedContent, @"cell\[0\]\.sound\.normalizeFilterSize=0\s*[\r\n]*", "");
            extractedContent = System.Text.RegularExpressions.Regex.Replace(extractedContent, @"cell\[0\]\.sound\.normalizePeakValue=0\s*[\r\n]*", "");
            extractedContent = System.Text.RegularExpressions.Regex.Replace(extractedContent, @"cell\[0\]\.sound\.normalizeMaxAmp=0\s*[\r\n]*", "");
            extractedContent = System.Text.RegularExpressions.Regex.Replace(extractedContent, @"cell\[0\]\.sound\.normalizeTargetRMS=0\s*[\r\n]*", "");
            extractedContent = System.Text.RegularExpressions.Regex.Replace(extractedContent, @"cell\[0\]\.sound\.normalizeCompressionFactor=0\s*[\r\n]*", "");
            extractedContent = System.Text.RegularExpressions.Regex.Replace(extractedContent, @"cell\[0\]\.sound\.normalizeInitialGainFade=0\s*[\r\n]*", "");
            extractedContent = System.Text.RegularExpressions.Regex.Replace(extractedContent, @"cell\[0\]\.sound\.normalizePaddingMode=0\s*[\r\n]*", "");

            // Thay thế "cell[0]" bằng "cell[index]"
            extractedContent = extractedContent.Replace("cell[0]", $"cell[{index}]");
            // Tìm và thay thế dòng chứa "cell[0].transTime" bằng "cell[0].transTime=1000"
            extractedContent = System.Text.RegularExpressions.Regex.Replace(extractedContent, $@"cell\[{index}\]\.transTime=\d+", $"cell[{index}].transTime=1000");

            string soundFile = $"cell[{index}].sound.file={path_audio}";
            string soundLength = $"cell[{index}].sound.length={length_audio}";
            string soundStartTime = $"cell[{index}].sound.startTime={i * segment}";
            string soundEndTime, cellTime;

            if (i == length_att_in_selectedFileImagePaths - 1)
            {
                soundEndTime = $"cell[{index}].sound.endTime={length_audio}";
                cellTime = $"cell[{index}].time={length_audio - i * segment - 1000}";
            }
            else
            {
                soundEndTime = $"cell[{index}].sound.endTime={(i + 1) * segment}";
                cellTime = $"cell[{index}].time={segment - 1000}";
            }

            extractedContent += Environment.NewLine + soundFile + Environment.NewLine + soundLength + Environment.NewLine + soundStartTime + Environment.NewLine + soundEndTime + Environment.NewLine + cellTime;
            // Ghi nội dung đã cắt vào một file khác
            // string outputFilePath = System.IO.Path.Combine("files", "extractedContent.txt");
            System.IO.File.AppendAllText(outputFilePath, extractedContent + Environment.NewLine);

        }
        else
        {
            MessageBox.Show("Không tìm thấy đoạn nội dung cần cắt.", "Lỗi");
        }
    }
    static void WriteFileContent(StreamWriter writer, string filePath)
    {
        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    writer.WriteLine(line);
                }
            }
        }
        else
        {
            Console.WriteLine($"File not found: {filePath}");
        }
    }
    private void showStylesButton_Click(object sender, EventArgs e)
    {
        StylesForm stylesForm = new StylesForm();
        stylesForm.ShowDialog();
    }
    private void reviewFileImage(object sender, EventArgs e, int index)
    {
        // Thực hiện các thao tác với phần tử tại index
        List<string> variables = selectedFileImagePaths[index]?.ToList() ?? new List<string>();
        DetailImageForm detailImageForm = new DetailImageForm(variables);

        // Show the form as a dialog
        detailImageForm.ShowDialog();

        // After the form is closed, get the updated variables
        List<string> updatedVariables = detailImageForm.UpdatedVariables;

        // Update your data in Form A
        selectedFileImagePaths[index] = updatedVariables.ToArray();
    }
    private void SettingsButton_Click(object sender, EventArgs e)
    {
        SettingsForm settingsForm = new SettingsForm();
        settingsForm.ShowDialog();
    }

    private void selectedFileVideoPaths_Click(object sender, EventArgs e, int index)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Video files (*.mp4, *.avi, *.wmv)|*.mp4;*.avi;*.wmv"; // Bộ lọc tệp (các tệp video)

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Tạo một danh sách từ mảng tại chỉ mục index
                List<string> fileCurrentList = new List<string>(selectedFileImagePaths[index]);

                // Thêm tệp được chọn vào danh sách
                fileCurrentList.Add(openFileDialog.FileName);

                // Cập nhật lại mảng tại chỉ mục index
                selectedFileImagePaths[index] = fileCurrentList.ToArray();

                UpdateReviewButtonState(index);
            }
        }

    }
    private void selectedFileImagePaths_Click(object sender, EventArgs e, int index)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Multiselect = true; // Cho phép chọn nhiều tệp
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp"; // Bộ lọc tệp (các tệp hình ảnh)

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFileImagePaths[index] = openFileDialog.FileNames.OrderBy(x => Guid.NewGuid()).ToArray(); // Lưu các đường dẫn của các tệp đã chọn và sắp xếp ngẫu nhiên
                UpdateReviewButtonState(index);
            }
        }
    }
    private void UpdateReviewButtonState(int index)
    {
        // Tìm nút "Review" tương ứng với index
        Button reviewButton = this.Controls.Find("reviewButton" + (index + 1), true).FirstOrDefault() as Button;
        if (reviewButton != null)
        {
            // Kiểm tra và cập nhật trạng thái của nút "Review"
            reviewButton.Enabled = selectedFileImagePaths.Count > index && selectedFileImagePaths[index].Length > 0;
        }
    }
    private void randomButton_Click(object sender, EventArgs e, int index)
    {
        string settingsFilePath = "settings.json";
        string folderImagePath = string.Empty;

        // lấy giá trị từ ô nhập số
        TextBox textBox = this.Controls.Find("textBox" + (index + 1), true).FirstOrDefault() as TextBox;
        if (textBox != null)
        {
            int number = 0;
            if (int.TryParse(textBox.Text, out number))
            {
                // Đọc file settings.json
                try
                {
                    string jsonContent = File.ReadAllText(settingsFilePath);
                    var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
                    if (jsonObject.TryGetValue("FolderImage", out folderImagePath))
                    {
                        string[] imageFiles = Directory.GetFiles(folderImagePath, "*.*", SearchOption.TopDirectoryOnly)
                                                       .ToArray();
                        if (imageFiles.Length < number)
                        {
                            MessageBox.Show("The folder does not contain enough images.");
                            return;
                        }
                        Random random = new Random();
                        string[] selectedImages = imageFiles.OrderBy(x => random.Next()).Take(number).ToArray();
                        selectedFileImagePaths[index] = selectedImages;
                        UpdateReviewButtonState(index);
                    }
                    else
                    {
                        MessageBox.Show("The FolderImage attribute is not found in the settings.json file.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number.");
            }
        }
        else
        {
            MessageBox.Show("The TextBox is not found.");
        }

    }

    private void videoRandomButton_Click(object sender, EventArgs e, int index)
    {
        string settingsFilePath = "settings.json";
        string folderVideoPath = string.Empty;

        // lấy giá trị từ ô nhập số
        TextBox textBox = this.Controls.Find("videoRandomTextBox" + (index + 1), true).FirstOrDefault() as TextBox;
        if (textBox != null)
        {
            int number = 0;
            if (int.TryParse(textBox.Text, out number))
            {
                // Đọc file settings.json
                try
                {
                    string jsonContent = File.ReadAllText(settingsFilePath);
                    var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
                    if (jsonObject.TryGetValue("FolderVideo", out folderVideoPath))
                    {
                        string[] videoFiles = Directory.GetFiles(folderVideoPath, "*.*", SearchOption.TopDirectoryOnly)
                                                       .ToArray();
                        if (videoFiles.Length < number)
                        {
                            MessageBox.Show("The folder does not contain enough videos.");
                            return;
                        }
                        Random random = new Random();
                        string[] selectedVideos = videoFiles.OrderBy(x => random.Next()).Take(number).ToArray();
                        List<string> fileCurrentList = new List<string>(selectedFileImagePaths[index]);
                        fileCurrentList.Add(selectedVideos[0]);
                        selectedFileImagePaths[index] = fileCurrentList.ToArray();
                        UpdateReviewButtonState(index);
                    }
                    else
                    {
                        MessageBox.Show("The FolderVideo attribute is not found in the settings.json file.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number.");
            }
        }
        else
        {
            MessageBox.Show("The TextBox is not found.");
        }
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
}
