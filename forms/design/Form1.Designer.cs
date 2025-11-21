namespace WinFormsApp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private Button button7;
    private Button generateSlideButton;
    private Button showStylesButton;
    private Button settingsButton;
    private Button reviewFolderAudioButton;
    private System.Windows.Forms.Button dropdownButton;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;




    // private Button openFileButton;

    private List<string[]> selectedFileImagePaths = new List<string[]>();
    private List<string[]> selectedFileAudioPaths = new List<string[]>();
    private List<string> selectedFolderAudioPaths = new List<string>();
    private List<string> selectedFolderSavePaths = new List<string>();
    private List<string> selectedFileIntroPaths = new List<string>();
    private List<string> selectedFileSrtPaths = new List<string>();
    private List<string> highlightFiles = new List<string>();
    private int optionSelectImage = -1;
    private bool addAudioCheckBox = true;
    private string backgroundMusicPath = "";
    private string orgSrtPath = "";

    private string path_image_animation = @"image_animation";
    private string path_image_animation_cutted = @"image_animation_cutted";
    private string path_video_converted = @"video_converted";
    private string path_video_subbed = @"video_subbed";
    private string path_highlight = @"highlight";
    private string path_txt_folder = @"txt_folder";

    private string mappingFile = @"txt_folder\mapping.txt";
    private string path_image_to_video = "";
    private string name_ffmpeg = "";
    private TabControl tabControl;
    private TabPage tabPageMedia;
    private TabPage tabPageStyles;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        // Khởi tạo các thành phần giao diện
        this.tabControl = new System.Windows.Forms.TabControl();
        this.tabPageMedia = new System.Windows.Forms.TabPage();
        this.tabPageStyles = new System.Windows.Forms.TabPage();
        this.dropdownButton = new System.Windows.Forms.Button();
        this.reviewFolderAudioButton = new System.Windows.Forms.Button();
        this.generateSlideButton = new System.Windows.Forms.Button();
        this.showStylesButton = new System.Windows.Forms.Button();
        this.settingsButton = new System.Windows.Forms.Button();
        this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
        this.SuspendLayout();

        // 
        // tabControl
        // 
        this.tabControl.Controls.Add(this.tabPageMedia);
        this.tabControl.Controls.Add(this.tabPageStyles);
        this.tabControl.Location = new System.Drawing.Point(20, 20);
        this.tabControl.Name = "tabControl";
        this.tabControl.Size = new System.Drawing.Size(710, 100);
        this.tabControl.TabIndex = 1;
        this.tabControl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.tabControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 245);
        this.tabControl.ForeColor = System.Drawing.Color.FromArgb(30, 30, 30);

        // 
        // tabPageMedia
        // 
        this.tabPageMedia.Controls.Add(this.reviewFolderAudioButton);
        this.tabPageMedia.Controls.Add(this.dropdownButton);
        this.tabPageMedia.Controls.Add(this.generateSlideButton);
        this.tabPageMedia.Name = "tabPageMedia";
        this.tabPageMedia.Text = "Media";
        this.tabPageMedia.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);

        // 
        // tabPageStyles
        // 
        this.tabPageStyles.Controls.Add(this.showStylesButton);
        this.tabPageStyles.Controls.Add(this.settingsButton);
        this.tabPageStyles.Name = "tabPageStyles";
        this.tabPageStyles.Text = "Styles & Settings";
        this.tabPageStyles.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);

        // 
        // reviewFolderAudioButton
        // 
        this.reviewFolderAudioButton.Location = new System.Drawing.Point(150, 15);
        this.reviewFolderAudioButton.Name = "reviewFolderAudioButton";
        this.reviewFolderAudioButton.Size = new System.Drawing.Size(120, 40);
        this.reviewFolderAudioButton.TabIndex = 2;
        this.reviewFolderAudioButton.Text = "\uD83C\uDFB5 Audio";
        this.reviewFolderAudioButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.reviewFolderAudioButton.FlatAppearance.BorderSize = 2;
        this.reviewFolderAudioButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(45, 85, 205);
        this.reviewFolderAudioButton.BackColor = System.Drawing.Color.FromArgb(65, 105, 225);
        this.reviewFolderAudioButton.ForeColor = System.Drawing.Color.White;
        this.reviewFolderAudioButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
        this.reviewFolderAudioButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(45, 85, 205);
        this.reviewFolderAudioButton.Click += new System.EventHandler(this.reviewFolderAudioButton_Click);

        // 
        // dropdownButton
        // 
        this.dropdownButton.Location = new System.Drawing.Point(295, 15);
        this.dropdownButton.Name = "dropdownButton";
        this.dropdownButton.Size = new System.Drawing.Size(120, 40);
        this.dropdownButton.TabIndex = 3;
        this.dropdownButton.Text = "\uD83D\uDDBC Image";
        this.dropdownButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.dropdownButton.FlatAppearance.BorderSize = 2;
        this.dropdownButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(127, 92, 199);
        this.dropdownButton.BackColor = System.Drawing.Color.FromArgb(147, 112, 219);
        this.dropdownButton.ForeColor = System.Drawing.Color.White;
        this.dropdownButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
        this.dropdownButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(127, 92, 199);
        this.dropdownButton.Click += new System.EventHandler(this.dropdownButton_Click);

        // 
        // generateSlideButton
        // 
        this.generateSlideButton.Location = new System.Drawing.Point(440, 15);
        this.generateSlideButton.Name = "generateSlideButton";
        this.generateSlideButton.Size = new System.Drawing.Size(120, 40);
        this.generateSlideButton.TabIndex = 4;
        this.generateSlideButton.Text = "\u25B6\uFE0F Run";
        this.generateSlideButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.generateSlideButton.FlatAppearance.BorderSize = 2;
        this.generateSlideButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(40, 159, 93);
        this.generateSlideButton.BackColor = System.Drawing.Color.FromArgb(60, 179, 113);
        this.generateSlideButton.ForeColor = System.Drawing.Color.White;
        this.generateSlideButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
        this.generateSlideButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(40, 159, 93);
        this.generateSlideButton.Click += new System.EventHandler(this.generateSlideButton_Click);

        // 
        // showStylesButton
        // 
        this.showStylesButton.Location = new System.Drawing.Point(225, 15);
        this.showStylesButton.Name = "showStylesButton";
        this.showStylesButton.Size = new System.Drawing.Size(120, 40);
        this.showStylesButton.TabIndex = 5;
        this.showStylesButton.Text = "\uD83C\uDFA8 Styles";
        this.showStylesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.showStylesButton.FlatAppearance.BorderSize = 2;
        this.showStylesButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(235, 120, 0);
        this.showStylesButton.BackColor = System.Drawing.Color.FromArgb(255, 140, 0);
        this.showStylesButton.ForeColor = System.Drawing.Color.White;
        this.showStylesButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
        this.showStylesButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(235, 120, 0);
        this.showStylesButton.Click += new System.EventHandler(this.showStylesButton_Click);

        // 
        // settingsButton
        // 
        this.settingsButton.Location = new System.Drawing.Point(365, 15);
        this.settingsButton.Name = "settingsButton";
        this.settingsButton.Size = new System.Drawing.Size(120, 40);
        this.settingsButton.TabIndex = 6;
        this.settingsButton.Text = "\u2699 Settings";
        this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.settingsButton.FlatAppearance.BorderSize = 2;
        this.settingsButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(188, 143, 143);
        this.settingsButton.BackColor = System.Drawing.Color.FromArgb(139, 69, 19);
        this.settingsButton.ForeColor = System.Drawing.Color.White;
        this.settingsButton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
        this.settingsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(205, 133, 63);
        this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);

        // 
        // contextMenuStrip
        // 
        this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        new System.Windows.Forms.ToolStripMenuItem("Segment", null, (sender, e) => this.openImageSegment_Click(sender, e, 0)) { Checked = optionSelectImage == 0 },
        new System.Windows.Forms.ToolStripMenuItem("Full", null, (sender, e) => this.openImageSegment_Click(sender, e, 1)) { Checked = optionSelectImage == 1 }
    });
        this.contextMenuStrip.Name = "contextMenuStrip";
        this.contextMenuStrip.Size = new System.Drawing.Size(150, 50);

        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(750, 400);
        this.Controls.AddRange(new Control[] {
        this.tabControl,
    });
        this.Name = "Form1";
        this.Text = "Tool Video Promax";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.BackColor = System.Drawing.Color.FromArgb(220, 240, 255); // Gradient không hỗ trợ trực tiếp, sử dụng màu nền tạm thời
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion
}
