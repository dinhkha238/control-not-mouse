using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp
{
    public class MainForm : Form
    {
        private Button addButton;
        // Import các hàm từ user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);
        // Các hằng số và mã tin nhắn

        // Hằng số và delegaate
        const int GW_CHILD = 5;
        const int GW_HWNDNEXT = 2;

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        const int BM_CLICK = 0x00F5; // Mã tin nhắn để click chuột vào nút

        public MainForm()
        {
            Text = "Add Image to ProShow Producer";
            Width = 400;
            Height = 200;

            // // Label và TextBox để nhập đường dẫn ảnh
            // Label imagePathLabel = new Label();
            // imagePathLabel.Text = "Image Path:";
            // imagePathLabel.Location = new System.Drawing.Point(20, 20);
            // Controls.Add(imagePathLabel);

            // imagePathTextBox = new TextBox();
            // imagePathTextBox.Location = new System.Drawing.Point(120, 20);
            // imagePathTextBox.Width = 200;
            // Controls.Add(imagePathTextBox);

            // Nút để thêm ảnh vào ProShow Producer
            addButton = new Button();
            addButton.Text = "Add Image";
            addButton.Location = new System.Drawing.Point(120, 60);
            addButton.Click += new EventHandler(AddButton_Click);
            Controls.Add(addButton);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Process.Start("C:\\Program Files (x86)\\Photodex\\ProShow Producer\\proshow.exe");
                // Process.Start("C:\\Program Files (x86)\\Photodex\\ProShow Producer\\proshow.exe");
                // wait for 5 seconds
                // System.Threading.Thread.Sleep(5000);

                // Thông tin về cửa sổ chứa nút (ví dụ: Caption, Class)
                string windowCaption = "ProShow Producer - I Love You"; // Thay bằng Caption của cửa sổ chứa nút
                string buttonCaption = "Save"; // Thay bằng Caption của nút cần click

                // Tìm cửa sổ chứa nút
                IntPtr mainWindowHandle = FindWindow(null, windowCaption);

                if (mainWindowHandle == IntPtr.Zero)
                {
                    MessageBox.Show("Không tìm thấy cửa sổ chứa nút.");
                    return;
                }

                // Duyệt qua từng control con trong cửa sổ
                EnumChildWindows(mainWindowHandle, EnumerateChildWindows, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private bool EnumerateChildWindows(IntPtr hWnd, IntPtr lParam)
        {
            // Kiểm tra xem control có phải là button không
            StringBuilder className = new StringBuilder(256);
            GetClassName(hWnd, className, className.Capacity);
            if (className.ToString() == "Button")
            {
                // Lấy caption của button
                StringBuilder windowText = new StringBuilder(256);
                GetWindowText(hWnd, windowText, windowText.Capacity);

                // In ra thông tin của button
                Console.WriteLine($"Button Caption: {windowText.ToString()}");
                // In ra caption của button
                if (windowText.ToString() == " New")
                {
                    MessageBox.Show("Tìm thấy nút Save");
                    // Click vào button
                    SendMessage(hWnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                    return false;
                }
            }

            // Tiếp tục duyệt các control con khác
            return true;
        }
    }
}
