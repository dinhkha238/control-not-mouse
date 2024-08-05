using System;
using System.IO;
using System.Windows.Forms;
using WinFormsApp;

public class PathProshowInputForm : Form
{
    private TextBox textBox;
    private Button saveButton;
    private Button openFileButton;
    private Label titleLabel;
    private OpenFileDialog openFileDialog;


    public PathProshowInputForm()
    {
        // Tiêu đề
        titleLabel = new Label
        {
            Text = "Vui lòng nhập đường dẫn đến tệp exe của Proshow Producer",
            Location = new System.Drawing.Point(10, 10),
            AutoSize = true
        };

        // TextBox để nhập đường dẫn
        textBox = new TextBox
        {
            Left = 10,
            Top = 41,
            Width = 300,
        };

        // Nút để mở hộp thoại chọn tệp
        openFileButton = new Button
        {
            Text = "Open Explorer",
            Location = new System.Drawing.Point(320, 40),
            Size = new System.Drawing.Size(100, 28)
        };
        openFileButton.Click += OpenFileExeButton_Click;

        // Nút Save để lưu đường dẫn
        saveButton = new Button
        {
            Text = "Save",
            Location = new System.Drawing.Point(170, 100),
            Size = new System.Drawing.Size(100, 30)
        };
        saveButton.Click += SaveButton_Click;

        // OpenFileDialog để chọn tệp
        openFileDialog = new OpenFileDialog();

        // Thêm các điều khiển vào form
        Controls.Add(titleLabel);
        Controls.Add(textBox);
        Controls.Add(openFileButton);
        Controls.Add(saveButton);
        // thêm kích thước window
        Size = new System.Drawing.Size(450, 300);

    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        string filePath = textBox.Text;
        if (string.IsNullOrEmpty(filePath))
        {
            MessageBox.Show("Please enter the path to the file.");
            return;
        }
        // Save the path to a file
        File.WriteAllText("pathProshow.txt", filePath);
        MessageBox.Show("Path saved successfully.");

        // Close this form and open MainForm
        this.Hide();
        Form1 mainForm = new Form1();
        mainForm.ShowDialog();
        this.Close();
    }
    private void OpenFileExeButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Exe files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = openFileDialog.FileName;
            }
        }
    }
}
