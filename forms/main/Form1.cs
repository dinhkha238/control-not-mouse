using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

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
    const int WM_SETTEXT = 0x000C;
    const int WM_PASTE = 0x0302;
    // Các thông điệp ListBox
    const uint LB_GETCOUNT = 0x018B;  // Lấy số lượng mục trong ListBox
    const uint LB_SETCURSEL = 0x0186; // Đặt mục hiện tại trong ListBox
    const uint WM_LBUTTONDBLCLK = 0x0203; // Thông điệp double click chuột



    public Form1()
    {
        InitializeComponent();
    }
    private void button7_Click(object sender, EventArgs e, int totalFileExport)
    {
        // Đọc dữ liệu từ file settings.json
        string settingsFilePath = "settings.json";
        if (!File.Exists(settingsFilePath))
        {
            MessageBox.Show("Settings file not found.");
            return;
        }
        string settingsContent = File.ReadAllText(settingsFilePath);
        dynamic settings = JsonConvert.DeserializeObject(settingsContent);

        string defaultTitle = settings.DefaultTitle;
        var PROSHOW_TITLE = "";

        for (int index_final = 0; index_final < totalFileExport; index_final++)
        {
            PROSHOW_TITLE = defaultTitle + $" - combined_{index_final}.psh"; // Cập nhật tiêu đề cửa sổ nếu cần thiết

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $@"finals\combined_{index_final}.psh");
            string proShowPath = "";

            int timeout = 3600000; // 20 giây
            int intervalImport = 100; // Kiểm tra mỗi 100ms
            int interval = 100; // Kiểm tra mỗi 100ms
            int elapsed = 0;

            bool isFindWindow = false;

            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTimeOffset vietnamTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, vietnamTimeZone);
            long timestamp = vietnamTime.ToUnixTimeSeconds();

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

            // if (!System.IO.File.Exists(filePath))
            // {
            //     MessageBox.Show("Đường dẫn tới tệp .psh không đúng.");
            //     return;
            // }

            // Tạo một đối tượng ProcessStartInfo để khởi động ProShow với tệp .psh
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = proShowPath,
                Arguments = filePath,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Minimized // Hoặc Hidden nếu bạn muốn ẩn hoàn toàn cửa sổ

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

            while (elapsed < timeout)
            {
                proShowHandle = FindWindow(null, PROSHOW_TITLE); // Cập nhật tiêu đề cửa sổ nếu cần thiết
                if (proShowHandle != IntPtr.Zero)
                {
                    isFindWindow = true;
                    break;
                }
                // Chờ 100ms trước khi kiểm tra lại
                Thread.Sleep(intervalImport);
                elapsed += intervalImport;
            }
            if (!isFindWindow)
            {
                return;
            }
            isFindWindow = false;

            IntPtr renderVideoHandle = IntPtr.Zero;
            while (elapsed < timeout)
            {
                renderVideoHandle = FindWindow(null, "Publishing Formats");
                if (renderVideoHandle != IntPtr.Zero)
                {
                    isFindWindow = true;
                    break;
                }
                // Chờ 100ms trước khi kiểm tra lại
                Thread.Sleep(interval);
                elapsed += interval;
            }
            if (!isFindWindow)
            {
                return;
            }
            isFindWindow = false;

            Thread.Sleep(1000); // Đợi một chút để hộp thoại mở
            // Duyệt qua tất cả các cửa sổ con để tìm ListBox
            EnumChildWindows(renderVideoHandle, (hwnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hwnd, className, className.Capacity);

                if (className.ToString() == "ListBox")
                {

                    // Đếm số lượng phần tử trong ListBox
                    int itemCount = SendMessage(hwnd, LB_GETCOUNT, IntPtr.Zero, null);
                    if (itemCount == -1)
                    {
                        MessageBox.Show("Failed to get item count.");
                        return false; // Dừng lại
                    }

                    // Kiểm tra xem có ít nhất 4 phần tử không
                    if (itemCount >= 4)
                    {
                        // Đặt lựa chọn cho phần tử thứ 4 (chỉ số 3, vì chỉ số bắt đầu từ 0)
                        SendMessage(hwnd, LB_SETCURSEL, (IntPtr)3, null);
                        // Lấy tọa độ của ListBox
                        RECT rect = new RECT();
                        GetWindowRect(hwnd, out rect);

                        // Tính toán tọa độ cho phần tử thứ 4
                        int itemHeight = (rect.Bottom - rect.Top) / itemCount; // Giả định rằng tất cả các mục đều có cùng chiều cao
                        int x = rect.Left + 5; // Vị trí X (khoảng cách 5 pixel từ bên trái)
                        int y = rect.Top + itemHeight * 3 + (itemHeight / 2); // Vị trí Y (giữa phần tử thứ 4)

                        // Gửi thông điệp double click tới ListBox
                        PostMessage(hwnd, WM_LBUTTONDBLCLK, IntPtr.Zero, (IntPtr)((y << 16) | x));
                    }
                    else
                    {
                        MessageBox.Show("Not enough items in the ListBox.");
                    }

                    return false; // Dừng sau khi tìm thấy ListBox và thao tác
                }

                return true; // Tiếp tục nếu chưa tìm thấy
            }, IntPtr.Zero);

            elapsed = 0;
            IntPtr showOptionHandle = IntPtr.Zero;
            while (elapsed < timeout)
            {
                showOptionHandle = FindWindow(null, "Video for Web, Devices and Computers");
                if (showOptionHandle != IntPtr.Zero)
                {
                    isFindWindow = true;
                    break;
                }
                // Chờ 100ms trước khi kiểm tra lại
                Thread.Sleep(interval);
                elapsed += interval;
            }
            if (!isFindWindow)
            {
                return;
            }
            isFindWindow = false;

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

            elapsed = 0;
            IntPtr saveOptionHandle = IntPtr.Zero;
            while (elapsed < timeout)
            {
                saveOptionHandle = FindWindow(null, "Save Video File");
                if (saveOptionHandle != IntPtr.Zero)
                {
                    isFindWindow = true;
                    break;
                }
                // Chờ 100ms trước khi kiểm tra lại
                Thread.Sleep(interval);
                elapsed += interval;
            }
            if (!isFindWindow)
            {
                return;
            }
            isFindWindow = false;

            if (!Directory.Exists(path_image_animation))
            {
                Directory.CreateDirectory(path_image_animation);
            }
            path_image_animation = Path.GetFullPath(path_image_animation);
            string savePath = $@"{path_image_animation}\" + $"{index_final + "_" + timestamp}.mp4";
            path_image_to_video = savePath;

            Thread.Sleep(1000); // Đợi một chút để hộp thoại mở
            EnumChildWindows(saveOptionHandle, (hwnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hwnd, className, className.Capacity);
                if (className.ToString() == "Edit")
                {
                    // Clipboard.Clear();  // Xóa clipboard trước khi thao tác
                    // Clipboard.SetText(savePath);
                    // SendMessage(hwnd, WM_PASTE, IntPtr.Zero, null);
                    SendMessage(hwnd, WM_SETTEXT, IntPtr.Zero, savePath);
                    return false; // Stop enumerating
                }
                return true; // Continue enumerating
            }, IntPtr.Zero);

            Thread.Sleep(1000); // Đợi một chút để hộp thoại mở
            EnumChildWindows(saveOptionHandle, (hwnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hwnd, className, className.Capacity);
                if (className.ToString() == "Button")
                {
                    StringBuilder windowText = new StringBuilder(256);
                    GetWindowText(hwnd, windowText, windowText.Capacity);
                    if (windowText.ToString() == "&Save")
                    {
                        PostMessage(hwnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                        return false; // Stop enumerating
                    }
                }
                return true; // Continue enumerating
            }, IntPtr.Zero);

            elapsed = 0;
            IntPtr messageOptionHandle = IntPtr.Zero;
            while (elapsed < timeout)
            {
                showOptionHandle = FindWindow(null, "Message");
                if (showOptionHandle != IntPtr.Zero)
                {
                    isFindWindow = true;
                    break;
                }
                // Chờ 100ms trước khi kiểm tra lại
                Thread.Sleep(interval);
                elapsed += interval;
            }
            if (!isFindWindow)
            {
                return;
            }
            isFindWindow = false;

            Thread.Sleep(1000); // Đợi một chút để hộp thoại mở
            EnumChildWindows(showOptionHandle, (hwnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hwnd, className, className.Capacity);
                if (className.ToString() == "Button")
                {
                    StringBuilder windowText = new StringBuilder(256);
                    GetWindowText(hwnd, windowText, windowText.Capacity);
                    if (windowText.ToString() == "Ok")
                    {
                        PostMessage(hwnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                        return false; // Stop enumerating
                    }
                }
                return true; // Continue enumerating
            }, IntPtr.Zero);
        }
        Thread.Sleep(2000); // Đợi một chút để hộp thoại mở

        // Đóng cửa sổ PROSHOW_TITLE
        IntPtr proShowCloseHandle = FindWindow(null, PROSHOW_TITLE);
        if (proShowCloseHandle != IntPtr.Zero)
        {
            PostMessage(proShowCloseHandle, 0x0010, IntPtr.Zero, IntPtr.Zero);
        }
        Thread.Sleep(2000);
        ClearFolder("finals");

    }

    private void AddButtons(int buttonCount)
    {
        // Tạo Panel để chứa các Label và Button
        Panel panel = new Panel();
        panel.AutoScroll = true;
        panel.Location = new System.Drawing.Point(100, 300); // Vị trí của Panel
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
            // reviewButton.Enabled = selectedFileImagePaths.Count > i && selectedFileImagePaths[i].Length > 0;
            reviewButton.Click += (sender, e) => reviewFileImage(sender, e, reviewButton.TabIndex - 4);
            panel.Controls.Add(reviewButton);
        }
    }

    private void openImageSegment_Click(object sender, EventArgs e, int option)
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
        if (!addAudioCheckBox)
        {
            // Check if selectedFolderAudioPaths contains at least one folder
            if (selectedFolderAudioPaths.Count > 0)
            {
                // Dictionary to store folder path and its audio file count
                Dictionary<string, int> folderAudioFileCounts = new Dictionary<string, int>();

                foreach (var folderPath in selectedFolderAudioPaths)
                {
                    // Get the count of audio files in the folder
                    int audioFileCount = Directory.GetFiles(folderPath, "*.mp3").Length +
                                         Directory.GetFiles(folderPath, "*.wav").Length +
                                         Directory.GetFiles(folderPath, "*.flac").Length;

                    folderAudioFileCounts[folderPath] = audioFileCount;
                }

                if (selectedFolderAudioPaths.Count == 1)
                {
                    // Check if the single folder contains any audio files
                    if (folderAudioFileCounts.Values.First() <= 0)
                    {
                        MessageBox.Show("The folder does not contain any audio files.");
                        return;

                    }
                }
                else
                {
                    // Check if all folders have the same number of audio files
                    bool allFoldersHaveSameCount = folderAudioFileCounts.Values.Distinct().Count() == 1;

                    if (!allFoldersHaveSameCount)
                    {
                        MessageBox.Show("Folders contain different numbers of audio files.");
                        return;

                    }
                }
            }
            else
            {
                MessageBox.Show("Please select at least one audio folder.");
                return;
            }
        }

        optionSelectImage = option;

        // Làm nổi bật tùy chọn đã chọn
        foreach (ToolStripMenuItem item in contextMenuStrip.Items)
        {
            item.Checked = false; // Bỏ đánh dấu tất cả các tùy chọn
        }

        // Đánh dấu tùy chọn đã chọn
        if (optionSelectImage == 0)
        {
            ((ToolStripMenuItem)contextMenuStrip.Items[0]).Checked = true;
        }
        else if (optionSelectImage == 1)
        {
            ((ToolStripMenuItem)contextMenuStrip.Items[1]).Checked = true;
        }
        labelX.Visible = true;
        labelY.Visible = true;
        labelQuantity.Visible = true;
        labelQuantityVideo.Visible = true;
        textBoxX.Visible = true;
        textBoxY.Visible = true;
        textBoxQuantity.Visible = true;
        textBoxQuantityVideo.Visible = true;
        saveButton.Visible = true;
        labelSelectFolderSegment.Visible = true;
        openFolderSegmentButton.Visible = true;


        AddButtons(selectedFileAudioPaths[0].Length);
        // Khởi tạo phần tử cho selectedFileImagePaths
        for (int i = 0; i < selectedFileAudioPaths[0].Length; i++)
        {
            selectedFileImagePaths.Add([]);
        }
    }
    private void generateSlideButton_Click(object sender, EventArgs e)
    {
        //  Các phần tử còn lại (trừ phần tử videoFiles)
        string[] imageFiles = selectedFileImagePaths[0].Where(file => !IsVideoFile(file)).ToArray();

        if (imageFiles.Length != 0)
        {
            for (int number = 0; number < 1; number++)
            {
                int length_selectedFileAudioPaths = 1;
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


                string file3Path = @"files/extractedContent.txt";
                // clear content of file3Path
                System.IO.File.WriteAllText(file3Path, string.Empty);
                int index_cell = 0;

                int segment = 5000;
                for (int x = 0; x < length_selectedFileAudioPaths; x++)
                {
                    // Lấy length audio của file audio
                    int length_audio = 5000;

                    int length_att_in_selectedFileImagePaths = length_audio / segment;
                    // Lấy phần dư sau khi 
                    int remaining = length_audio - length_att_in_selectedFileImagePaths * segment;

                    string[] groupFileLines = System.IO.File.ReadAllLines(groupFilePath);
                    // Tạo đối tượng Random
                    Random random = new Random();


                    for (int i = 0; i < imageFiles.Length; i++)
                    {
                        // Lấy ngẫu nhiên một dòng từ groupFileLines
                        string selectedFile = groupFileLines[random.Next(groupFileLines.Length)];
                        string path_image;
                        path_image = "../../../../" + imageFiles[i];
                        string path_audio = "";
                        WriteCellToFile(selectedFile, ref index_cell, path_image, path_audio, length_audio, segment, i, length_att_in_selectedFileImagePaths, file3Path);
                    }
                }
                string file2Path = @"files/FileProShow_2.txt";
                using (StreamWriter writer1 = new StreamWriter(file2Path))
                {
                    writer1.WriteLine($"cells={index_cell}");
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
                if (!Directory.Exists(folderContainFileProShow))
                {
                    Directory.CreateDirectory(folderContainFileProShow);
                }
                string combinedFilePath = $"finals/combined_{number}.psh"; // Replace with your combined file path
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

            }
            button7_Click(sender, e, 1);
        }

        // Lấy thư mục hiện tại của chương trình (thư mục chứa code)
        string currentDirectory = Directory.GetCurrentDirectory();

        if (!Directory.Exists(path_image_animation_cutted))
        {
            Directory.CreateDirectory(path_image_animation_cutted);
        }
        path_image_animation_cutted = Path.GetFullPath(path_image_animation_cutted);

        if (imageFiles.Length != 0)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTimeOffset vietnamTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, vietnamTimeZone);
            long timestamp = vietnamTime.ToUnixTimeSeconds();

            ConvertTo30fps(path_image_to_video, $"{"output_30fps_" + timestamp}.mp4");

            CutVideo($"{"output_30fps_" + timestamp}.mp4", path_image_animation_cutted);

            DeleteFile($"{"output_30fps_" + timestamp}.mp4");
        }

        Thread.Sleep(1000);

        // Lọc các phần tử là file Video
        string[] videoFiles = selectedFileImagePaths[0].Where(IsVideoFile).ToArray();
        string[] imageToVideoFiles = Directory.GetFiles("image_animation_cutted", "*.mp4");

        imageToVideoFiles = imageToVideoFiles.Select(file => Path.Combine(currentDirectory, file)).ToArray();

        for (int index_audio = 0; index_audio < selectedFolderAudioPaths.Count(); index_audio++)
        {
            // Tính độ dài của file âm thanh
            double audioDuration = GetAudioFileLength(selectedFileAudioPaths[index_audio][0]) / 1000;

            // Mỗi video có độ dài là 5 giây, tính số lượng video cần ghép
            int totalVideosNeeded = (int)Math.Ceiling(audioDuration / 5.0);

            // Danh sách video cần ghép
            string[] videoPaths = GetVideoListForMerging(videoFiles, imageToVideoFiles, totalVideosNeeded);

            // Tạo đường dẫn cho file video_list.txt ngay trong thư mục hiện tại
            string listFilePath = Path.Combine(currentDirectory, "video_list.txt");

            using (StreamWriter writer = new StreamWriter(listFilePath))
            {
                foreach (var video in videoPaths)
                {
                    writer.WriteLine($"file '{video.Replace("\\", "/")}'");
                }
            }

            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTimeOffset vietnamTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, vietnamTimeZone);
            long timestamp = vietnamTime.ToUnixTimeSeconds();

            // Ghép các video đã chọn
            MergeVideosUsingListFile(listFilePath, Path.Combine(selectedFolderSavePaths[index_audio], $"{"output_" + timestamp}.mp4"));

            CombineVideoAndAudio(Path.Combine(selectedFolderSavePaths[index_audio], $"{"output_" + timestamp}.mp4"), selectedFileAudioPaths[index_audio][0], Path.Combine(selectedFolderSavePaths[index_audio], $"{"output_final_" + timestamp}.mp4"));

            DeleteFile(Path.Combine(selectedFolderSavePaths[index_audio], $"{"output_" + timestamp}.mp4"));

            if (selectedFileIntroPaths[index_audio] != "")
            {
                File.WriteAllText(listFilePath, $"file '{selectedFileIntroPaths[index_audio]}'\nfile '{Path.Combine(selectedFolderSavePaths[index_audio], $"{"output_final_" + timestamp}.mp4")}'");
                MergeVideosUsingListFileHaveAudio(listFilePath, Path.Combine(selectedFolderSavePaths[index_audio], $"{"intro_final_" + timestamp}.mp4"));
                DeleteFile(Path.Combine(selectedFolderSavePaths[index_audio], $"{"output_final_" + timestamp}.mp4"));
            }
        }
        DeleteAllFilesInFolder(path_image_animation);
        DeleteAllFilesInFolder(path_image_animation_cutted);

        MessageBox.Show("Done!");
    }
    public static void DeleteAllFilesInFolder(string folderPath)
    {
        try
        {
            if (Directory.Exists(folderPath))
            {
                // Lấy tất cả file trong thư mục
                string[] files = Directory.GetFiles(folderPath);

                foreach (string file in files)
                {
                    File.Delete(file);
                    Console.WriteLine($"Đã xóa file: {file}");
                }

                Console.WriteLine("Tất cả các file trong thư mục đã được xóa.");
            }
            else
            {
                Console.WriteLine("Thư mục không tồn tại.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khi xóa các file: " + ex.Message);
        }
    }
    public static void DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine("File đã được xóa thành công.");
            }
            else
            {
                Console.WriteLine("File không tồn tại.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Lỗi khi xóa file: " + ex.Message);
        }
    }
    public void ConvertTo30fps(string inputFile, string outputFile)
    {
        // Tạo lệnh FFmpeg để chuyển đổi video
        string arguments = $"-i \"{inputFile}\" -r 30 \"{outputFile}\"";
        name_ffmpeg = "CONVERTING TO 30FPS";
        RunFFmpegCommand(arguments);
    }

    // Hàm chọn danh sách video ghép theo tỉ lệ 3:1
    static string[] GetVideoListForMerging(string[] folder1Videos, string[] folder2Videos, int totalVideosNeeded)
    {
        var videoList = new List<string>();
        // Tạo đối tượng Random
        Random random = new Random();
        //
        int countImage = 0;
        for (int i = 0; i < totalVideosNeeded; i++)
        {
            if (countImage < 3)
            {
                videoList.Add(folder1Videos[random.Next(folder1Videos.Length)]);
                countImage++;
            }
            else
            {
                videoList.Add(folder2Videos[random.Next(folder2Videos.Length)]);
                countImage = 0;
            }
        }

        return videoList.ToArray();
    }
    static int GetAudioFileLength(string filePath)
    {
        var audioFileReader = new AudioFileReader(filePath);
        TimeSpan duration = audioFileReader.TotalTime;
        int[] timeArray = { duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds };
        return (timeArray[0] * 60 * 60 + timeArray[1] * 60 + timeArray[2]) * 1000 + timeArray[3];
    }
    static void WriteCellToFile(string style_file_name, ref int index, string path_image, string path_audio, int length_audio, float segment, int i, int length_att_in_selectedFileImagePaths, string outputFilePath)
    {
        var fileContent = "";
        if (IsVideoFile(path_image))
        {
            fileContent = System.IO.File.ReadAllText(@"styles\No style.pxs");
        }
        else
        {
            fileContent = System.IO.File.ReadAllText(style_file_name);
        }

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
            int video_length = 5000;
            if (IsVideoFile(path_image) && segment > video_length + 1000)
            {
                string soundFile = $"cell[{index}].sound.file={path_audio}";
                string soundLength = $"cell[{index}].sound.length={length_audio}";
                string soundStartTime = $"cell[{index}].sound.startTime={i * segment}";
                string soundEndTime, cellTime;

                soundEndTime = $"cell[{index}].sound.endTime={i * segment + video_length}";
                cellTime = $"cell[{index}].time={video_length - 1000}";

                extractedContent += Environment.NewLine + soundFile + Environment.NewLine + soundLength + Environment.NewLine + soundStartTime + Environment.NewLine + soundEndTime + Environment.NewLine + cellTime;
                // Ghi nội dung đã cắt vào một file khác
                // string outputFilePath = System.IO.Path.Combine("files", "extractedContent.txt");
                System.IO.File.AppendAllText(outputFilePath, extractedContent + Environment.NewLine);
                index++;

                string path_image_replace = @"../../../../" + RandomImage();


                WriteCellToFile(style_file_name, ref index, path_image_replace, path_audio, length_audio, (i * segment + video_length) / i, i, length_att_in_selectedFileImagePaths, outputFilePath);



            }
            else
            {
                if (path_audio != "")
                {
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
                }
                else
                {
                    string cellTime, transTime;
                    cellTime = $"cell[{index}].time={segment}";
                    transTime = $"cell[{index}].transTime={0}";
                    extractedContent += Environment.NewLine + cellTime + Environment.NewLine + transTime;
                }
                // Ghi nội dung đã cắt vào một file khác
                System.IO.File.AppendAllText(outputFilePath, extractedContent + Environment.NewLine);
                index++;
            }

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

    private void CombineVideoAndAudio(string videoFile, string audioFile, string outputFile)
    {
        string arguments = $"-i \"{videoFile}\" -i \"{audioFile}\" -c:v copy -c:a aac -b:a 128k -ar 44100 -ac 2 \"{outputFile}\"";
        name_ffmpeg = "COMBINING VIDEO AND AUDIO";
        RunFFmpegCommand(arguments);
    }

    // Hàm gọi FFmpeg để ghép video từ danh sách file
    private void MergeVideosUsingListFile(string listFilePath, string outputPath)
    {
        string arguments = $"-f concat -safe 0 -i \"{listFilePath}\" -c copy -an \"{outputPath}\"";
        name_ffmpeg = "MERGING VIDEO";

        RunFFmpegCommand(arguments);
    }
    private void MergeVideosUsingListFileHaveAudio(string listFilePath, string outputPath)
    {
        string arguments = $"-f concat -safe 0 -i \"{listFilePath}\" -c copy \"{outputPath}\"";
        name_ffmpeg = "MERGING INTRO AND VIDEO";

        RunFFmpegCommand(arguments);
    }

    static void CutVideo(string inputVideoPath, string outputFolder)
    {
        var psi = new ProcessStartInfo();
        psi.FileName = "python";  // Đảm bảo Python đã có trong PATH
        var script = @"cut.py";  // Đường dẫn đến script Python của bạn

        string videoIndex = GetRandomSixDigitNumber();

        // // Truyền tham số cho script Python
        psi.Arguments = $"{script} {inputVideoPath} {outputFolder} {videoIndex}";

        psi.UseShellExecute = false;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

        // Khởi chạy quá trình và đợi hoàn thành
        var process = Process.Start(psi);

        // Đọc đầu ra
        string output = process.StandardOutput.ReadToEnd();
        string errors = process.StandardError.ReadToEnd();

        process.WaitForExit();

        // Hiển thị đầu ra và lỗi (nếu có)
        Console.WriteLine(output);
        Console.WriteLine(errors);
    }
    public static string GetRandomSixDigitNumber()
    {
        Random random = new Random();
        int number = random.Next(100000, 1000000); // Tạo số ngẫu nhiên trong khoảng 100000 - 999999
        return number.ToString();
    }

    private void RunFFmpegCommand(string arguments)
    {
        Process ffmpegProcess = new Process();
        ffmpegProcess.StartInfo.FileName = "ffmpeg"; // Hoặc cung cấp đường dẫn đầy đủ nếu cần
        ffmpegProcess.StartInfo.Arguments = arguments;
        ffmpegProcess.StartInfo.RedirectStandardOutput = true;
        ffmpegProcess.StartInfo.RedirectStandardError = true;
        ffmpegProcess.StartInfo.UseShellExecute = false;
        ffmpegProcess.StartInfo.CreateNoWindow = true;

        // Đăng ký sự kiện để ghi log ra file
        ffmpegProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                LogToFile("ffmpeg_log.txt", "FFmpeg Log: " + e.Data);
            }
        };

        ffmpegProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                LogToFile("ffmpeg_log.txt", "FFmpeg Error: " + e.Data);
            }
        };

        ffmpegProcess.Start();

        // Bắt đầu đọc log
        ffmpegProcess.BeginOutputReadLine();
        ffmpegProcess.BeginErrorReadLine();

        ffmpegProcess.WaitForExit();

        LogToFile("ffmpeg_log.txt", "Video converted to 30fps successfully!");
    }
    private void LogToFile(string logFile, string message)
    {
        // Ghi đè nội dung file log bằng thông điệp mới
        File.WriteAllText(logFile, $"{name_ffmpeg}: {DateTime.Now}: {message}\n");
    }

    private void reviewFolderAudioButton_Click(object sender, EventArgs e)
    {
        // Thực hiện các thao tác với phần tử tại index
        List<string> variables = selectedFolderAudioPaths?.ToList() ?? new List<string>();
        List<string> variablesFileIntroPaths = selectedFileIntroPaths?.ToList() ?? new List<string>();
        List<string> variablesFolderSavePaths = selectedFolderSavePaths?.ToList() ?? new List<string>();
        bool variablesAddAudioCheckBox = addAudioCheckBox;
        DetailFolderForm detailImageForm = new DetailFolderForm(variables, variablesFolderSavePaths, variablesFileIntroPaths, variablesAddAudioCheckBox);

        // Show the form as a dialog
        detailImageForm.ShowDialog();



        // After the form is closed, get the updated variables
        List<string> updatedVariables = detailImageForm.UpdatedVariables;
        List<string> updatedFileIntroVariables = detailImageForm.UpdatedFileIntroPaths;
        List<string> updatedFolderSavePaths = detailImageForm.UpdatedFolderSavePaths;
        bool updatedAddAudioCheckBox = detailImageForm.AddAudioCheckBox;

        // Kiểm tra xem đã chọn đủ số lượng thư mục lưu chưa
        for (int i = 0; i < updatedVariables.Count; i++)
        {
            if (updatedFolderSavePaths[i] == "")
            {
                MessageBox.Show("Please select save path for segment " + (i + 1) + "!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        // Update your data in Form A
        selectedFolderAudioPaths = updatedVariables;
        selectedFileIntroPaths = updatedFileIntroVariables;
        selectedFolderSavePaths = updatedFolderSavePaths;
        addAudioCheckBox = updatedAddAudioCheckBox;

        selectedFileAudioPaths.Clear();

        foreach (var folderPath in selectedFolderAudioPaths)
        {
            if (!addAudioCheckBox)
            {
                string[] audioFiles = Directory.GetFiles(folderPath, "*.mp3").Concat(Directory.GetFiles(folderPath, "*.wav")).Concat(Directory.GetFiles(folderPath, "*.flac")).ToArray();
                Array.Sort(audioFiles, (file1, file2) =>
                {
                    // Lấy tên tệp từ đường dẫn đầy đủ
                    string fileName1 = Path.GetFileNameWithoutExtension(file1);
                    string fileName2 = Path.GetFileNameWithoutExtension(file2);

                    // Chuyển tên tệp sang số nguyên để so sánh
                    int number1 = int.Parse(fileName1);
                    int number2 = int.Parse(fileName2);

                    return number1.CompareTo(number2);
                });
                selectedFileAudioPaths.Add(audioFiles);
            }
            else
            {
                string[] audioFiles = [folderPath];
                selectedFileAudioPaths.Add(audioFiles);
            }
        }

    }
    private void selectedFileVideoPaths_Click(object sender, EventArgs e, int index)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Video files (*.mp4, *.avi, *.wmv)|*.mp4;*.avi;*.wmv"; // Bộ lọc tệp (các tệp video)
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Tạo một danh sách từ mảng tại chỉ mục index
                List<string> fileCurrentList = new List<string>(selectedFileImagePaths[index]);

                // Thêm tệp được chọn vào danh sách
                fileCurrentList.AddRange(openFileDialog.FileNames);

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
                selectedFileImagePaths[index] = openFileDialog.FileNames;
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
                        fileCurrentList.AddRange(selectedVideos);
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
    private void openFolderSegmentButton_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;
                string[] subDirectories = Directory.GetDirectories(selectedPath);

                // Biến kiểm tra xem có đủ các folder từ 1 đến n không
                bool isValid = true;

                // Kiểm tra các thư mục con
                for (int i = 1; i <= subDirectories.Length; i++)
                {
                    // Tạo đường dẫn folder cần kiểm tra
                    string expectedFolder = Path.Combine(selectedPath, i.ToString());

                    // Nếu thư mục tương ứng không tồn tại, đặt isValid thành false
                    if (!Directory.Exists(expectedFolder))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    // Nếu tất cả các thư mục đều tồn tại, sắp xếp và in ra
                    Array.Sort(subDirectories, (dir1, dir2) =>
                    {
                        string folderName1 = Path.GetFileName(dir1);
                        string folderName2 = Path.GetFileName(dir2);

                        int number1 = int.Parse(folderName1);
                        int number2 = int.Parse(folderName2);

                        return number1.CompareTo(number2);
                    });

                    // In ra các thư mục đã sắp xếp sử dụng vòng lặp for
                    for (int i = 0; i < subDirectories.Length; i++)
                    {
                        Console.WriteLine(subDirectories[i]);
                    }
                }
                else
                {
                    MessageBox.Show("The selected folder does not contain all the required subfolders.");
                    return;
                }

                int end;
                if (selectedFileAudioPaths[0].Length > subDirectories.Length)
                {
                    end = subDirectories.Length;
                }
                else
                {
                    end = selectedFileAudioPaths[0].Length;
                }


                for (int i = 0; i < end; i++)
                {
                    string[] imageFiles = Directory.GetFiles(subDirectories[i], "*.*")
                        .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                        .ToArray();
                    string[] videoFiles = Directory.GetFiles(subDirectories[i], "*.*")
                        .Where(file => file.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".avi", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".mov", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".flv", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) ||
                                       file.EndsWith(".webm", StringComparison.OrdinalIgnoreCase))
                        .ToArray();

                    if (imageFiles.Length <= 0 && videoFiles.Length <= 0)
                    {
                        MessageBox.Show("Segment " + (i + 1) + " does not contain any images and videos.");
                        return;
                    }

                    if (imageFiles.Length > 0)
                    {
                        selectedFileImagePaths[i] = imageFiles;
                        UpdateReviewButtonState(i);
                    }
                    else
                    {
                        selectedFileImagePaths[i] = new string[0];
                    }

                    if (videoFiles.Length > 0)
                    {
                        List<string> fileCurrentList = new List<string>(selectedFileImagePaths[i]);
                        fileCurrentList.AddRange(videoFiles);
                        selectedFileImagePaths[i] = fileCurrentList.ToArray();
                        UpdateReviewButtonState(i);
                    }

                }
            }
        }
    }

    public static bool IsVideoFile(string filePath)
    {
        // List of common video file extensions
        string[] videoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".mkv", ".webm" };

        // Get the file extension
        string fileExtension = Path.GetExtension(filePath).ToLower();

        // Check if the file extension is in the list of video extensions
        foreach (string extension in videoExtensions)
        {
            if (fileExtension == extension)
            {
                return true;
            }
        }

        return false;
    }

    public static string RandomImage()
    {
        // Read the settings.json file
        string settingsPath = "settings.json";
        string jsonContent = File.ReadAllText(settingsPath);
        JObject settings = JObject.Parse(jsonContent);

        // Get the FolderImage path from settings.json
        string folderImagePath = settings["FolderImage"].ToString();

        // List all image files in the FolderImage directory
        string[] imageFiles = Directory.GetFiles(folderImagePath, "*.*", SearchOption.TopDirectoryOnly)
                                        .Where(file => file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".png") || file.EndsWith(".bmp") || file.EndsWith(".gif"))
                                        .ToArray();

        // Check if there are any image files
        if (imageFiles.Length == 0)
        {
            throw new Exception("No image files found in the specified directory.");
        }

        // Select a random image file
        Random random = new Random();
        int randomIndex = random.Next(imageFiles.Length);
        string randomImagePath = imageFiles[randomIndex];

        return randomImagePath;
    }

    public void saveButtonRandomSegment(object sender, EventArgs e)
    {
        // Đọc giá trị từ ô nhập số
        if (!(this.Controls.Find("textBoxStartSegment", true).FirstOrDefault() is TextBox startSegment))
        {
            MessageBox.Show("Please enter a valid number.");
            return;
        }
        if (!(this.Controls.Find("textBoxEndSegment", true).FirstOrDefault() is TextBox endSegment))
        {
            MessageBox.Show("Please enter a valid number.");
            return;
        }
        if (!(this.Controls.Find("textBoxQuantityImageSegment", true).FirstOrDefault() is TextBox totalImage))
        {
            MessageBox.Show("Please enter a valid number.");
            return;
        }
        if (!(this.Controls.Find("textBoxQuantityVideoSegment", true).FirstOrDefault() is TextBox totalVideo))
        {
            MessageBox.Show("Please enter a valid number.");
            return;
        }

        int start, end;
        if (!int.TryParse(startSegment.Text, out start) || !int.TryParse(endSegment.Text, out end))
        {
            MessageBox.Show("Please enter valid numbers for start and end segments.");
            return;
        }

        for (int i = start - 1; i < end; i++)
        {
            TextBox textBox = this.Controls.Find("textBox" + (i + 1), true).FirstOrDefault() as TextBox;
            TextBox videoRandomTextBox = this.Controls.Find("videoRandomTextBox" + (i + 1), true).FirstOrDefault() as TextBox;
            if (textBox != null)
            {
                textBox.Text = totalImage.Text;
                if (textBox.Text != "")
                {
                    randomButton_Click(sender, e, i);
                }
            }
            if (videoRandomTextBox != null)
            {
                videoRandomTextBox.Text = totalVideo.Text;
                if (videoRandomTextBox.Text != "")
                {
                    videoRandomButton_Click(sender, e, i);
                }
            }
        }
    }
    private void ClearFolder(string folderPath)
    {
        string finalsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
        if (Directory.Exists(finalsFolderPath))
        {
            try
            {
                string[] files = Directory.GetFiles(finalsFolderPath);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while clearing the folder: " + ex.Message, "Error");
            }
        }
        else
        {
            MessageBox.Show("The folder does not exist.", "Error");
        }
    }
    private void dropdownButton_Click(object sender, EventArgs e)
    {
        contextMenuStrip.Show(dropdownButton, new Point(0, dropdownButton.Height));
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
