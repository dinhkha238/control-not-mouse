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
        int horizontalSpacing = 20; // Khoảng cách ngang giữa các nút bấm
        int verticalSpacing = 60; // Khoảng cách dọc giữa các hàng nút bấm (bao gồm cả khoảng cách cho nhãn)
        int startX = 100; // Vị trí bắt đầu theo trục X
        int startY = 200; // Vị trí bắt đầu theo trục Y

        // Tạo các nút bấm động và nhãn
        for (int i = 0; i < buttonCount; i++)
        {
            Button button = new Button();
            Label label = new Label();

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
        string file3Path = @"files/FileProShow_3.txt";
        int index_cell = 0;
        using (StreamWriter writer1 = new StreamWriter(file3Path))
        {
            for (int x = 0; x < length_selectedFileAudioPaths; x++)
            {
                int length_audio = GetAudioFileLength(selectedFileAudioPaths[x]);

                string[] att_in_selectedFileImagePaths = selectedFileImagePaths[x];
                int length_att_in_selectedFileImagePaths = att_in_selectedFileImagePaths.Length;
                float segment = length_audio / length_att_in_selectedFileImagePaths;

                for (int i = 0; i < length_att_in_selectedFileImagePaths; i++)
                {
                    Cell cell = new Cell();
                    cell.images[0].image = "../../../../" + att_in_selectedFileImagePaths[i];
                    cell.images[0].name = "Image" + index_cell;
                    cell.images[0].objectId = index_cell;
                    cell.images[1].image = "../../../../" + att_in_selectedFileImagePaths[i];
                    cell.images[1].name = "Image" + index_cell;
                    cell.images[1].objectId = index_cell;
                    cell.sound.file = "../../../../" + selectedFileAudioPaths[x];
                    cell.sound.length = length_audio;
                    cell.sound.startTime = i * segment;
                    if (i == length_att_in_selectedFileImagePaths - 1)
                    {
                        cell.sound.endTime = length_audio;
                        cell.time = length_audio - i * segment - cell.transTime;
                    }
                    else
                    {
                        cell.sound.endTime = (i + 1) * segment;
                        cell.time = segment - cell.transTime;
                    }
                    WriteCellToFile(cell, index_cell, writer1);
                    index_cell++;
                }
            }
            writer1.Close();
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
    static void WriteCellToFile(Cell cell, int index, StreamWriter writer)
    {

        writer.WriteLine($"cell[{index}].imageEnable={cell.imageEnable}");
        writer.WriteLine($"cell[{index}].notes={cell.notes}");
        writer.WriteLine($"cell[{index}].slideStyleFileName={cell.slideStyleFileName}");
        writer.WriteLine($"cell[{index}].nrOfImages={cell.nrOfImages}");

        for (int i = 0; i < cell.images.Length; i++)
        {
            var image = cell.images[i];
            writer.WriteLine($"cell[{index}].images[{i}].image={image.image}");
            writer.WriteLine($"cell[{index}].images[{i}].imageEnable={image.imageEnable}");
            writer.WriteLine($"cell[{index}].images[{i}].name={image.name}");
            writer.WriteLine($"cell[{index}].images[{i}].notes={image.notes}");
            if (i == 1)
            {
                writer.WriteLine($"cell[{index}].images[{i}].fromStyle={image.fromStyle}");
            }
            writer.WriteLine($"cell[{index}].images[{i}].templateImageId={image.templateImageId}");
            writer.WriteLine($"cell[{index}].images[{i}].replaceableTemplate={image.replaceableTemplate}");
            writer.WriteLine($"cell[{index}].images[{i}].sizeMode={image.sizeMode}");
            if (i == 1)
            {
                writer.WriteLine($"cell[{index}].images[{i}].colorize={image.colorize}");
            }
            writer.WriteLine($"cell[{index}].images[{i}].colorizeColor={image.colorizeColor}");
            writer.WriteLine($"cell[{index}].images[{i}].colorizeStrength={image.colorizeStrength}");
            if (i == 0)
            {
                writer.WriteLine($"cell[{index}].images[{i}].outline={image.outline}");
            }
            writer.WriteLine($"cell[{index}].images[{i}].outlineColor={image.outlineColor}");
            if (i == 0)
            {
                writer.WriteLine($"cell[{index}].images[{i}].shadow={image.shadow}");

            }
            writer.WriteLine($"cell[{index}].images[{i}].aspectX={image.aspectX}");
            writer.WriteLine($"cell[{index}].images[{i}].aspectY={image.aspectY}");
            writer.WriteLine($"cell[{index}].images[{i}].videoVolume={image.videoVolume}");
            writer.WriteLine($"cell[{index}].images[{i}].objectId={image.objectId}");
            writer.WriteLine($"cell[{index}].images[{i}].videoSpeed={image.videoSpeed}");
            writer.WriteLine($"cell[{index}].images[{i}].useTransitionIn={image.useTransitionIn}");
            writer.WriteLine($"cell[{index}].images[{i}].useTransitionOut={image.useTransitionOut}");
            writer.WriteLine($"cell[{index}].images[{i}].outlineSize={image.outlineSize}");
            writer.WriteLine($"cell[{index}].images[{i}].shadowSize={image.shadowSize}");
            writer.WriteLine($"cell[{index}].images[{i}].shadowOpacity={image.shadowOpacity}");
            writer.WriteLine($"cell[{index}].images[{i}].maskChannel={image.maskChannel}");
            writer.WriteLine($"cell[{index}].images[{i}].filterName={image.filterName}");
            writer.WriteLine($"cell[{index}].images[{i}].savedFilterName={image.savedFilterName}");
            writer.WriteLine($"cell[{index}].images[{i}].motionFilterName={image.motionFilterName}");
            writer.WriteLine($"cell[{index}].images[{i}].motionFilterNameIn={image.motionFilterNameIn}");
            writer.WriteLine($"cell[{index}].images[{i}].motionFilterNameOut={image.motionFilterNameOut}");
            writer.WriteLine($"cell[{index}].images[{i}].nrOfKeyframes={image.nrOfKeyframes}");

            for (int j = 0; j < image.keyframes.Length; j++)
            {
                var keyframe = image.keyframes[j];
                if (j == 1)
                {
                    writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].timestamp={keyframe.timestamp}");
                }
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].timeSegment={keyframe.timeSegment}");
                if (j == 1)
                {
                    writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].segmentTimestamp={keyframe.segmentTimestamp}");
                }
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].attributeMask={keyframe.attributeMask}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].offsetX={keyframe.offsetX}");
                if (i == 1)
                {
                    writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].offsetY={keyframe.offsetY}");
                }
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].zoomX={keyframe.zoomX}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].zoomY={keyframe.zoomY}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].panAccelType={keyframe.panAccelType}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].zoomXAccelType={keyframe.zoomXAccelType}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].zoomYAccelType={keyframe.zoomYAccelType}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].rotationAccelType={keyframe.rotationAccelType}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].tiltVAccelType={keyframe.tiltVAccelType}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].tiltHAccelType={keyframe.tiltHAccelType}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].motionSmoothness={keyframe.motionSmoothness}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].lockAR={keyframe.lockAR}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].transparency={keyframe.transparency}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].audioFade={keyframe.audioFade}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].colorizeColor={keyframe.colorizeColor}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].colorizeStrength={keyframe.colorizeStrength}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].shadowOffsetX={keyframe.shadowOffsetX}");
                writer.WriteLine($"cell[{index}].images[{i}].keyframes[{j}].shadowOffsetY={keyframe.shadowOffsetY}");

            }
        }

        writer.WriteLine($"cell[{index}].background={cell.background}");
        writer.WriteLine($"cell[{index}].bgDefault={cell.bgDefault}");
        writer.WriteLine($"cell[{index}].bgSizeMode={cell.bgSizeMode}");
        writer.WriteLine($"cell[{index}].bgColorizeColor={cell.bgColorizeColor}");
        if (cell.sound.file != null)
        {
            writer.WriteLine($"cell[{index}].sound.file={cell.sound.file}");
        }
        if (cell.sound.length != 0)
        {
            writer.WriteLine($"cell[{index}].sound.length={cell.sound.length}");
        }
        writer.WriteLine($"cell[{index}].sound.useDefault={cell.sound.useDefault}");
        writer.WriteLine($"cell[{index}].sound.volume={cell.sound.volume}");
        writer.WriteLine($"cell[{index}].sound.startTime={cell.sound.startTime}");
        writer.WriteLine($"cell[{index}].sound.endTime={cell.sound.endTime}");
        writer.WriteLine($"cell[{index}].sound.fadeIn={cell.sound.fadeIn}");
        writer.WriteLine($"cell[{index}].sound.fadeOut={cell.sound.fadeOut}");
        writer.WriteLine($"cell[{index}].sound.async={cell.sound.async}");
        writer.WriteLine($"cell[{index}].sound.musicUseDefault={cell.sound.musicUseDefault}");
        writer.WriteLine($"cell[{index}].sound.musicVolume={cell.sound.musicVolume}");
        writer.WriteLine($"cell[{index}].sound.musicFadeIn={cell.sound.musicFadeIn}");
        writer.WriteLine($"cell[{index}].sound.musicFadeOut={cell.sound.musicFadeOut}");
        writer.WriteLine($"cell[{index}].sound.normalizeCustom={cell.sound.normalizeCustom}");
        writer.WriteLine($"cell[{index}].sound.normalizePreset={cell.sound.normalizePreset}");
        writer.WriteLine($"cell[{index}].musicVolumeOffset={cell.musicVolumeOffset}");
        writer.WriteLine($"cell[{index}].time={cell.time}");
        writer.WriteLine($"cell[{index}].transId={cell.transId}");
        writer.WriteLine($"cell[{index}].transTime={cell.transTime}");
        writer.WriteLine($"cell[{index}].includeGlobalCaptions={cell.includeGlobalCaptions}");

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

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
}
