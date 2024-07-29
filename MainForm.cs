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
        // Định nghĩa các API từ kernel32.dll
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint GENERIC_WRITE = 0x40000000;
        private const uint CREATE_ALWAYS = 2;
        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

        public MainForm()
        {
            MessageBox.Show("Hello World!");
            // Bước 1: Mở tệp (CreateFile)
            string filePath = @"c:\Users\Dinh Kha\Desktop\ProShow Slideshow.psh";
            IntPtr hFile = CreateFile(filePath, GENERIC_WRITE, 0, IntPtr.Zero, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);
            if (hFile == IntPtr.Zero)
            {
                MessageBox.Show("Could not create file. Error: " + Marshal.GetLastWin32Error());
                return;
            }

            // Bước 2: Ghi vào tệp (WriteFile)
            string text = "Hello world!";
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            if (!WriteFile(hFile, bytes, (uint)bytes.Length, out uint bytesWritten, IntPtr.Zero))
            {
                MessageBox.Show("Could not write to file. Error: " + Marshal.GetLastWin32Error());
                CloseHandle(hFile);
                return;
            }

            MessageBox.Show($"Successfully written {bytesWritten} bytes to file.");

            // Bước 3: Đóng tệp (CloseHandle)
            if (!CloseHandle(hFile))
            {
                MessageBox.Show("Could not close file. Error: " + Marshal.GetLastWin32Error());
                return;
            }

            MessageBox.Show("File closed successfully.");
        }




    }
}
