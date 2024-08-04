using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave;

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
    // Phương thức công khai để cập nhật ComboBox
    public void UpdateComboBoxes()
    {
        // Lấy danh sách các file trong thư mục
        string folderPath = @"groups"; // Thay đổi đường dẫn đến thư mục của bạn
        string[] files = System.IO.Directory.GetFiles(folderPath);

        // Lặp qua tất cả các điều khiển trong form
        foreach (Control control in this.Controls)
        {
            if (control is ComboBox comboBox)
            {
                // Xóa các mục cũ trong ComboBox
                comboBox.Items.Clear();

                // Thêm tên file vào ComboBox
                foreach (string file in files)
                {
                    comboBox.Items.Add(System.IO.Path.GetFileName(file));
                }
            }
        }
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

        // Đợi một chút để hộp thoại mở
        Thread.Sleep(7000);

        IntPtr proShowHandle = FindWindow(null, PROSHOW_TITLE); // Cập nhật tiêu đề cửa sổ nếu cần thiết
        if (proShowHandle != IntPtr.Zero)
        {
            SetForegroundWindow(proShowHandle);

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

            Thread.Sleep(3000); // Đợi một chút để hộp thoại mở

            IntPtr showOptionHandle = FindWindow(null, "Video for Web, Devices and Computers");
            if (showOptionHandle != IntPtr.Zero)
            {
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

        }
    }

    private void OpenImageButton_Click(object sender, EventArgs e, int index)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Multiselect = true; // Cho phép chọn nhiều tệp
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif"; // Bộ lọc tệp (các tệp hình ảnh)

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (index >= selectedFileImagePaths.Count)
                {
                    selectedFileImagePaths.Add(new string[index - selectedFileImagePaths.Count + 1]);
                }
                selectedFileImagePaths[index] = openFileDialog.FileNames; // Lưu các đường dẫn của các tệp đã chọn
            }
        }
    }

    private void AddButtons(int buttonCount)
    {
        int buttonsPerRow = 10; // Số nút bấm mỗi hàng
        int buttonWidth = 90; // Chiều rộng của nút bấm
        int buttonHeight = 30; // Chiều cao của nút bấm
        int labelHeight = 20; // Chiều cao của nhãn
        int comboBoxHeight = 30; // Chiều cao của ComboBox

        int horizontalSpacing = 20; // Khoảng cách ngang giữa các nút bấm
        int verticalSpacing = 60; // Khoảng cách dọc giữa các hàng nút bấm (bao gồm cả khoảng cách cho nhãn)
        int startX = 100; // Vị trí bắt đầu theo trục X
        int startY = 200; // Vị trí bắt đầu theo trục Y

        // Lấy danh sách các file trong thư mục
        string folderPath = @"groups"; // Thay đổi đường dẫn đến thư mục của bạn
        string[] files = System.IO.Directory.GetFiles(folderPath);
        // Tạo các nút bấm động và nhãn
        for (int i = 0; i < buttonCount; i++)
        {
            Button button = new Button();
            Label label = new Label();
            ComboBox comboBox = new ComboBox();

            int row = i / buttonsPerRow; // Xác định hàng
            int col = i % buttonsPerRow; // Xác định cột

            int x = startX + (col * (buttonWidth + horizontalSpacing));
            int y = startY + (row * verticalSpacing);

            // Thiết lập nhãn
            label.Location = new System.Drawing.Point(x, y);
            label.Name = "label" + (i + 1);
            label.Size = new System.Drawing.Size(buttonWidth, labelHeight);
            label.Text = "Đoạn " + (i + 1);
            label.Tag = "LabelSelectImageButton"; // Gán thuộc tính Tag để nhận diện label động
            this.Controls.Add(label);

            // Thiết lập nút bấm
            button.Location = new System.Drawing.Point(x, y + labelHeight);
            button.Name = "button" + (i + 1);
            button.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            button.TabIndex = i + 2; // Điều chỉnh TabIndex để tránh xung đột với các điều khiển hiện có
            button.Text = "Select Image";
            button.UseVisualStyleBackColor = true;
            button.Click += (sender, e) => OpenImageButton_Click(sender, e, button.TabIndex - 2);
            button.Tag = "SelectImageButton"; // Gán thuộc tính Tag để nhận diện button động
            this.Controls.Add(button);

            // Thiết lập ComboBox
            comboBox.Location = new System.Drawing.Point(x, y + labelHeight + buttonHeight);
            comboBox.Name = "comboBox" + (i + 1);
            comboBox.Size = new System.Drawing.Size(buttonWidth, comboBoxHeight);

            // Xóa các mục cũ trong ComboBox
            comboBox.Items.Clear();

            // Thêm tên file vào ComboBox
            foreach (string file in files)
            {
                comboBox.Items.Add(System.IO.Path.GetFileName(file));
            }
            // Gán sự kiện SelectedIndexChanged cho ComboBox
            comboBox.SelectedIndexChanged += (sender, e) =>
            {
                ComboBox cb = sender as ComboBox;
                if (cb != null && cb.SelectedItem != null)
                {
                    selectedGroupPaths.Add(cb.SelectedItem.ToString());
                }
            };
            this.Controls.Add(comboBox);
        }
    }
    private void OpenAudioButton_Click(object sender, EventArgs e)
    {
        foreach (Control control in this.Controls)
        {
            if (control.Tag != null && (control.Tag.ToString() == "SelectImageButton"))
            {
                this.Controls.Remove(control);
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
        if (selectedFileImagePaths.Count == 0)
        {
            MessageBox.Show("Please select image files!");
            return;
        }
        int length_selectedFileAudioPaths = selectedFileAudioPaths.Count;

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
        {
            for (int x = 0; x < length_selectedFileAudioPaths; x++)
            {

                string[] att_in_selectedFileImagePaths = selectedFileImagePaths[x];
                int length_att_in_selectedFileImagePaths = att_in_selectedFileImagePaths.Length;
                // Đọc tất cả các dòng từ file group/selectedGroupPaths[x]
                string groupFilePath = System.IO.Path.Combine("groups", selectedGroupPaths[x]);
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

                if (extractedContent.Contains(searchPattern))
                {
                    if (!extractedContent.Contains(searchPattern2))
                    {
                        string newLine = $"{insertImage}.image={path_image}";
                        int insertIndex = extractedContent.IndexOf(searchPattern) + searchPattern.Length;
                        extractedContent = extractedContent.Insert(insertIndex, Environment.NewLine + newLine);
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
        StylesForm stylesForm = new StylesForm(this);
        stylesForm.ShowDialog();
    }


    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
}
