using System;
using System.Windows.Forms;

namespace WinFormsApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Bước đăng nhập
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return; // Kết thúc chương trình nếu đăng nhập không thành công
                }
            }

            // Kiểm tra xem tệp lưu đường dẫn có tồn tại không
            if (File.Exists("pathProshow.txt"))
            {
                Application.Run(new Form1());
            }
            else
            {
                Application.Run(new PathProshowInputForm());
            }

        }
    }
}
