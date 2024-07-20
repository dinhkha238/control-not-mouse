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
    const int WM_SETTEXT = 0x000C;
    const int WM_SYSCOMMAND = 0x0112;
    const int SC_MAXIMIZE = 0xF030;
    const int BM_CLICK = 0x00F5;
    const int WM_LBUTTONDBLCLK = 0x0203;
    const int WM_LBUTTONDOWN = 0x0201;
    const int WM_LBUTTONUP = 0x0202;

    const byte VK_MENU = 0x12; // Alt key
    const byte B_KEY = 0x42;   // B key
    const uint KEYEVENTF_KEYUP = 0x0002;

    const string PROSHOW_TITLE = "ProShow Producer - I Love You - ProShow Slideshow *"; // Cập nhật tiêu đề cửa sổ nếu cần thiết
    const string PROSHOW_NEW_TITLE = "ProShow Producer - I Love You"; // Cập nhật tiêu đề cửa sổ nếu cần thiết

    const string PATH_IMAGE = @"C:\Users\Dinh Kha\Desktop\image-test\80cac51934362ebb6b4b129cb7cecc77.jpg"; // Đường dẫn đến file ảnh
    const string PATH_AUDIO = @"C:\Users\Dinh Kha\Desktop\a2.mp3"; // Đường dẫn đến file ảnh
    string PROSHOW_PATH = "C:\\Program Files (x86)\\Photodex\\ProShow Producer\\proshow.exe"; // Thay đường dẫn bằng đường dẫn cài đặt ProShow Producer 9 trên máy của bạn

    public Form1()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        Process notepad = Process.Start("notepad.exe");
        notepad.WaitForInputIdle();

        Thread.Sleep(500); // Đợi một chút để Notepad mở hoàn toàn

        IntPtr notepadHandle = FindWindow("Notepad", null);
        IntPtr editHandle = FindWindowEx(notepadHandle, IntPtr.Zero, "Edit", null);

        SendMessage(editHandle, WM_SETTEXT, IntPtr.Zero, "Hello");
    }
    private void button2_Click(object sender, EventArgs e)
    {
        IntPtr notepadHandle = FindWindow("Notepad", null);
        if (notepadHandle != IntPtr.Zero)
        {
            SendMessage(notepadHandle, WM_SYSCOMMAND, (IntPtr)SC_MAXIMIZE, null);
        }
    }
    private void button3_Click(object sender, EventArgs e)
    {
        IntPtr proShowHandle = FindWindow(null, PROSHOW_TITLE); // Cập nhật tiêu đề cửa sổ nếu cần thiết
        if (proShowHandle != IntPtr.Zero)
        {
            SetForegroundWindow(proShowHandle);

            // nhấn alt + B
            keybd_event(VK_MENU, 0, 0, UIntPtr.Zero); // Alt down
            keybd_event(B_KEY, 0, 0, UIntPtr.Zero);   // B down
            keybd_event(B_KEY, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // B up
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Alt up

        }
        else
        {
            IntPtr proShowNewHandle = FindWindow(null, PROSHOW_NEW_TITLE); // Cập nhật tiêu đề cửa sổ nếu cần thiết
            SetForegroundWindow(proShowNewHandle);
            if (proShowNewHandle == IntPtr.Zero)
            {
                Process.Start(PROSHOW_PATH);
                Thread.Sleep(10000); // Đợi một chút để ProShow mở hoàn toàn
            }
            // nhấn alt + B
            keybd_event(VK_MENU, 0, 0, UIntPtr.Zero); // Alt down
            keybd_event(B_KEY, 0, 0, UIntPtr.Zero);   // B down
            keybd_event(B_KEY, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // B up
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Alt up
        }

    }
    private void button4_Click(object sender, EventArgs e)
    {
        IntPtr proShowHandle = FindWindow(null, PROSHOW_TITLE); // Cập nhật tiêu đề cửa sổ nếu cần thiết
        if (proShowHandle != IntPtr.Zero)
        {
            EnumChildWindows(proShowHandle, (hwnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hwnd, className, className.Capacity);
                if (className.ToString() == "Button")
                {
                    StringBuilder windowText = new StringBuilder(256);
                    GetWindowText(hwnd, windowText, windowText.Capacity);
                    if (windowText.ToString() == "Add Blank")
                    {
                        // Dùng PostMessage để click nút
                        PostMessage(hwnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);

                        return false; // Stop enumerating
                    }
                }
                return true; // Continue enumerating
            }, IntPtr.Zero);
        }


    }
    private void button5_Click(object sender, EventArgs e)
    {
        int length_audio = GetAudioFileLength(PATH_AUDIO);
        int path_audio;
        if (length_audio % 5 == 0)
        {
            path_audio = length_audio / 5;
        }
        else
        {
            path_audio = length_audio / 5 + 1;
        }
        IntPtr proShowHandle = FindWindow(null, PROSHOW_TITLE); // Cập nhật tiêu đề cửa sổ nếu cần thiết
        int lengt_selectedFilePaths = selectedFilePaths.Length;

        if (proShowHandle != IntPtr.Zero)
        {
            for (int i = 0; i < path_audio; i++)
            {
                SetForegroundWindow(proShowHandle);

                // Gửi tổ hợp phím Ctrl+L
                keybd_event(0x11, 0, 0, UIntPtr.Zero); // Ctrl down
                keybd_event(0x4C, 0, 0, UIntPtr.Zero); // L down
                keybd_event(0x4C, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // L up
                keybd_event(0x11, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Ctrl up

                Thread.Sleep(500); // Đợi một chút để hộp thoại mở

                // Gửi tổ hợp phím Ctrl+Shift+I
                keybd_event(0x11, 0, 0, UIntPtr.Zero); // Ctrl down
                keybd_event(0x10, 0, 0, UIntPtr.Zero); // Shift down
                keybd_event(0x49, 0, 0, UIntPtr.Zero); // I down
                keybd_event(0x49, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // I up
                keybd_event(0x10, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Shift up
                keybd_event(0x11, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Ctrl up

                Thread.Sleep(500); // Đợi một chút để hộp thoại mở

                IntPtr openImageHandle = FindWindow(null, "Open Image File"); // Cập nhật tiêu đề cửa sổ nếu cần thiết

                // Truyền biến PATH_IMAGE vào hộp thoại bằng class
                EnumChildWindows(openImageHandle, (hwnd, lParam) =>
                {
                    StringBuilder className = new StringBuilder(256);
                    GetClassName(hwnd, className, className.Capacity);
                    if (className.ToString() == "Edit")
                    {
                        SendMessage(hwnd, WM_SETTEXT, IntPtr.Zero, selectedFilePaths[lengt_selectedFilePaths - 1 - (i % lengt_selectedFilePaths)]);
                        return false; // Stop enumerating
                    }
                    return true; // Continue enumerating
                }, IntPtr.Zero);


                // // Truyền biến PATH_IMAGE vào hộp thoại
                SendKeys.SendWait("{ENTER}"); // Gửi phím Enter để mở ảnh

                Thread.Sleep(1000); // Đợi một chút để hộp thoại mở

                SendKeys.SendWait("{ENTER}"); // Gửi phím Enter để mở ảnh

                Thread.Sleep(1000); // Đợi một chút để menu mở

                // Gửi tổ hợp phím Alt+B
                keybd_event(VK_MENU, 0, 0, UIntPtr.Zero); // Alt down
                keybd_event(B_KEY, 0, 0, UIntPtr.Zero);   // B down
                keybd_event(B_KEY, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // B up
                keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Alt up

                Thread.Sleep(1000); // Đợi một chút để menu mở

            }
            // nhấn phím delete
            keybd_event(0x2E, 0, 0, UIntPtr.Zero); // Delete down
            keybd_event(0x2E, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Delete up
        }

    }
    private void button6_Click(object sender, EventArgs e)
    {


        IntPtr proShowHandle = FindWindow(null, PROSHOW_TITLE); // Cập nhật tiêu đề cửa sổ nếu cần thiết
        if (proShowHandle != IntPtr.Zero)
        {
            SetForegroundWindow(proShowHandle);

            // Tổ hợp phím Ctrl+M
            keybd_event(0x11, 0, 0, UIntPtr.Zero); // Ctrl down
            keybd_event(0x4D, 0, 0, UIntPtr.Zero); // M down
            keybd_event(0x4D, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // M up
            keybd_event(0x11, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Ctrl up 

            Thread.Sleep(500); // Đợi một chút để hộp thoại mở

            // Vị trí mà bạn muốn di chuyển chuột đến (ví dụ: x = 500, y = 500)
            int x1 = 100;
            int y1 = 257;
            int x2 = 120;
            int y2 = 267;

            // Di chuyển chuột đến vị trí đã chỉ định trong cửa sổ showOptionHandle
            SetCursorPos(x1, y1);

            // Gửi sự kiện nhấp chuột trái xuống và lên để mô phỏng nhấp chuột
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x1, y1, 0, 0);

            Thread.Sleep(1000); // Đợi một chút để hộp thoại mở

            // Di chuyển chuột đến vị trí đã chỉ định trong cửa sổ showOptionHandle
            SetCursorPos(x2, y2);

            // Gửi sự kiện nhấp chuột trái xuống và lên để mô phỏng nhấp chuột
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x2, y2, 0, 0);

            Thread.Sleep(1000); // Đợi một chút để hộp thoại mở

            // Truyền biến PATH_IMAGE vào hộp thoại
            IntPtr openAudioHandle = FindWindow(null, "Open Audio File"); // Cập nhật tiêu đề cửa sổ nếu cần thiết

            // Truyền biến PATH_IMAGE vào hộp thoại bằng class
            EnumChildWindows(openAudioHandle, (hwnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hwnd, className, className.Capacity);
                if (className.ToString() == "Edit")
                {
                    SendMessage(hwnd, WM_SETTEXT, IntPtr.Zero, PATH_AUDIO);
                    return false; // Stop enumerating
                }
                return true; // Continue enumerating
            }, IntPtr.Zero);

            SendKeys.SendWait("{ENTER}"); // Gửi phím Enter để mở ảnh

            Thread.Sleep(1000); // Đợi một chút để hộp thoại mở

            SendKeys.SendWait("{ENTER}"); // Gửi phím Enter để mở ảnh

        }
    }

    private void button7_Click(object sender, EventArgs e)
    {

        IntPtr proShowHandle = FindWindow(null, PROSHOW_TITLE); // Cập nhật tiêu đề cửa sổ nếu cần thiết
        if (proShowHandle != IntPtr.Zero)
        {
            SetForegroundWindow(proShowHandle);

            // Nhấn tổ hợp phím alt+f3
            keybd_event(VK_MENU, 0, 0, UIntPtr.Zero); // Alt down
            keybd_event(0x72, 0, 0, UIntPtr.Zero); // F3 down
            keybd_event(0x72, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // F3 up
            keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero); // Alt up

            Thread.Sleep(500); // Đợi một chút để hộp thoại mở

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

            Thread.Sleep(1000); // Đợi một chút để hộp thoại mở

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

    private void OpenFileButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Multiselect = true; // Cho phép chọn nhiều tệp
            openFileDialog.Filter = "All files (*.*)|*.*"; // Bộ lọc tệp (tất cả các tệp)

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePaths = openFileDialog.FileNames; // Lưu các đường dẫn của các tệp đã chọn
            }
        }
    }
    static int GetAudioFileLength(string filePath)
    {
        var audioFileReader = new AudioFileReader(filePath);
        TimeSpan duration = audioFileReader.TotalTime;
        int[] timeArray = { duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds };
        return timeArray[0] * 60 * 60 + timeArray[1] * 60 + timeArray[2];
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
}
