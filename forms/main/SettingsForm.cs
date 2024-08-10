using System;
using System.Text.Json;
using System.Windows.Forms;

public partial class SettingsForm : Form
{
    public SettingsForm()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        // Lấy danh sách các file trong thư mục
        string folderPath = @"groups"; // Thay đổi đường dẫn đến thư mục của bạn
        string[] files = System.IO.Directory.GetFiles(folderPath);

        this.selectFolderButton = new System.Windows.Forms.Button();
        this.groupStyleComboBox = new System.Windows.Forms.ComboBox();
        this.folderPathLabel = new System.Windows.Forms.Label();
        this.folderPathTextBox = new System.Windows.Forms.TextBox();
        this.groupStyleLabel = new System.Windows.Forms.Label();
        // this.groupStyleTextBox = new System.Windows.Forms.TextBox();
        this.saveButton = new System.Windows.Forms.Button();
        this.SuspendLayout();

        // 
        // selectFolderButton
        // 
        this.selectFolderButton.Location = new System.Drawing.Point(50, 50);
        this.selectFolderButton.Name = "selectFolderButton";
        this.selectFolderButton.Size = new System.Drawing.Size(150, 30);
        this.selectFolderButton.TabIndex = 0;
        this.selectFolderButton.Text = "Select Folder";
        this.selectFolderButton.UseVisualStyleBackColor = true;
        this.selectFolderButton.Click += new System.EventHandler(this.SelectFolderButton_Click);


        // 
        // folderPathLabel
        // 
        this.folderPathLabel.AutoSize = true;
        this.folderPathLabel.Location = new System.Drawing.Point(50, 90);
        this.folderPathLabel.Name = "folderPathLabel";
        this.folderPathLabel.Size = new System.Drawing.Size(80, 17);
        this.folderPathLabel.TabIndex = 2;
        this.folderPathLabel.Text = "Folder Image Path";

        // 
        // folderPathTextBox
        // 
        this.folderPathTextBox.Location = new System.Drawing.Point(50, 110);
        this.folderPathTextBox.Name = "folderPathTextBox";
        this.folderPathTextBox.Size = new System.Drawing.Size(300, 22);
        this.folderPathTextBox.TabIndex = 3;

        // 
        // groupStyleLabel
        // 
        this.groupStyleLabel.AutoSize = true;
        this.groupStyleLabel.Location = new System.Drawing.Point(50, 150);
        this.groupStyleLabel.Name = "groupStyleLabel";
        this.groupStyleLabel.Size = new System.Drawing.Size(80, 17);
        this.groupStyleLabel.TabIndex = 4;
        this.groupStyleLabel.Text = "Group Style";

        // 
        // groupStyleComboBox
        // 
        this.groupStyleComboBox.FormattingEnabled = true;
        this.groupStyleComboBox.Location = new System.Drawing.Point(50, 170);
        this.groupStyleComboBox.Name = "groupStyleComboBox";
        this.groupStyleComboBox.Size = new System.Drawing.Size(150, 24);
        this.groupStyleComboBox.TabIndex = 1;


        // Thêm tên file vào ComboBox
        foreach (string file in files)
        {
            groupStyleComboBox.Items.Add(System.IO.Path.GetFileName(file));
        }
        // // 
        // // groupStyleTextBox
        // // 
        // this.groupStyleTextBox.Location = new System.Drawing.Point(50, 200);
        // this.groupStyleTextBox.Name = "groupStyleTextBox";
        // this.groupStyleTextBox.Size = new System.Drawing.Size(150, 22);
        // this.groupStyleTextBox.TabIndex = 5;

        // 
        // saveButton
        // 
        this.saveButton.Location = new System.Drawing.Point(50, 230);
        this.saveButton.Name = "saveButton";
        this.saveButton.Size = new System.Drawing.Size(150, 30);
        this.saveButton.TabIndex = 6;
        this.saveButton.Text = "Save";
        this.saveButton.UseVisualStyleBackColor = true;
        this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);

        // 
        // SettingsForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(500, 300);
        this.Controls.Add(this.saveButton);
        // this.Controls.Add(this.groupStyleTextBox);
        this.Controls.Add(this.groupStyleLabel);
        this.Controls.Add(this.folderPathTextBox);
        this.Controls.Add(this.folderPathLabel);
        this.Controls.Add(this.groupStyleComboBox);
        this.Controls.Add(this.selectFolderButton);
        this.Name = "SettingsForm";
        this.Text = "Settings";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.Button selectFolderButton;
    private System.Windows.Forms.ComboBox groupStyleComboBox;
    private System.Windows.Forms.Label folderPathLabel;
    private System.Windows.Forms.TextBox folderPathTextBox;
    private System.Windows.Forms.Label groupStyleLabel;
    // private System.Windows.Forms.TextBox groupStyleTextBox;
    private System.Windows.Forms.Button saveButton;

    private void SelectFolderButton_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // Hiển thị đường dẫn folder được chọn trong TextBox
                folderPathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        // Tạo đối tượng SettingsData
        var settingsData = new SettingsData
        {
            FolderImage = folderPathTextBox.Text,
            GroupStyle = groupStyleComboBox.SelectedItem?.ToString() ?? string.Empty
        };

        // Chuyển đổi đối tượng thành JSON
        string jsonString = JsonSerializer.Serialize(settingsData);

        // Lưu JSON vào tệp
        File.WriteAllText("settings.json", jsonString);

        // Hiển thị thông báo lưu thành công
        MessageBox.Show("Settings saved successfully!");
    }
    private void LoadSettings()
    {
        // Kiểm tra xem tệp settings.json có tồn tại không
        if (File.Exists("settings.json"))
        {
            // Đọc nội dung của tệp
            string jsonString = File.ReadAllText("settings.json");

            // Chuyển đổi JSON thành đối tượng SettingsData
            var settingsData = JsonSerializer.Deserialize<SettingsData>(jsonString);

            // Truyền giá trị vào TextBox và ComboBox
            folderPathTextBox.Text = settingsData.FolderImage;
            groupStyleComboBox.SelectedItem = settingsData.GroupStyle;
        }
    }
}