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
    private System.Windows.Forms.TextBox textBoxX;
    private System.Windows.Forms.TextBox textBoxY;
    private System.Windows.Forms.TextBox textBoxQuantity;
    private System.Windows.Forms.TextBox textBoxQuantityVideo;
    private System.Windows.Forms.Button saveButton;
    private System.Windows.Forms.Label labelX;
    private System.Windows.Forms.Label labelY;
    private System.Windows.Forms.Label labelQuantity;
    private System.Windows.Forms.Label labelQuantityVideo;
    private System.Windows.Forms.Label labelSelectFolderSegment;
    private Button openFolderSegmentButton;
    private System.Windows.Forms.Button dropdownButton;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;




    // private Button openFileButton;

    private List<string[]> selectedFileImagePaths = new List<string[]>();
    private List<string[]> selectedFileAudioPaths = new List<string[]>();
    private List<string> selectedFolderAudioPaths = new List<string>();
    private List<string> selectedFolderSavePaths = new List<string>();
    private List<string> selectedFileIntroPaths = new List<string>();
    private int optionSelectImage = -1;
    private bool addAudioCheckBox = true;

    private string path_image_animation = @"image_animation";
    private string path_image_animation_cutted = @"image_animation_cutted";
    private string path_image_to_video = "";
    private string name_ffmpeg = "";

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
        // 
        this.button7 = new Button();
        this.generateSlideButton = new Button();
        this.showStylesButton = new System.Windows.Forms.Button();
        this.settingsButton = new System.Windows.Forms.Button();
        this.reviewFolderAudioButton = new System.Windows.Forms.Button();
        this.labelX = new System.Windows.Forms.Label();
        this.labelY = new System.Windows.Forms.Label();
        this.labelQuantity = new System.Windows.Forms.Label();
        this.labelQuantityVideo = new System.Windows.Forms.Label();
        this.textBoxX = new System.Windows.Forms.TextBox();
        this.textBoxY = new System.Windows.Forms.TextBox();
        this.textBoxQuantity = new System.Windows.Forms.TextBox();
        this.textBoxQuantityVideo = new System.Windows.Forms.TextBox();
        this.saveButton = new System.Windows.Forms.Button();
        this.labelSelectFolderSegment = new System.Windows.Forms.Label();
        this.openFolderSegmentButton = new System.Windows.Forms.Button();
        this.dropdownButton = new System.Windows.Forms.Button();
        this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
        this.SuspendLayout();

        // 
        // dropdownButton
        // 
        this.dropdownButton.Location = new System.Drawing.Point(250, 100);
        this.dropdownButton.Name = "dropdownButton";
        this.dropdownButton.Size = new System.Drawing.Size(120, 50);
        this.dropdownButton.TabIndex = 0;
        this.dropdownButton.Text = "Open Image";
        this.dropdownButton.UseVisualStyleBackColor = true;
        this.dropdownButton.Click += new System.EventHandler(this.dropdownButton_Click);

        // 
        // contextMenuStrip
        // 
        this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            new System.Windows.Forms.ToolStripMenuItem("Segment", null, (sender, e) => this.openImageSegment_Click(sender, e, 0)) { Checked = optionSelectImage == 0 },
            new System.Windows.Forms.ToolStripMenuItem("Full", null, (sender, e) => this.openImageSegment_Click(sender, e, 1)) { Checked = optionSelectImage == 1 }
        });
        this.contextMenuStrip.Name = "contextMenuStrip";
        this.contextMenuStrip.Size = new System.Drawing.Size(181, 70);

        // 
        // labelX
        // 
        this.labelX.AutoSize = true;
        this.labelX.Location = new System.Drawing.Point(100, 173);
        this.labelX.Name = "labelX";
        this.labelX.Size = new System.Drawing.Size(44, 13);
        this.labelX.TabIndex = 7;
        this.labelX.Text = "Ngẫu nhiên từ đoạn";
        this.labelX.Visible = false;

        // 
        // labelY
        // 
        this.labelY.AutoSize = true;
        this.labelY.Location = new System.Drawing.Point(280, 173);
        this.labelY.Name = "labelY";
        this.labelY.Size = new System.Drawing.Size(44, 13);
        this.labelY.TabIndex = 8;
        this.labelY.Text = "đến";
        this.labelY.Visible = false;

        // 
        // labelQuantity
        // 
        this.labelQuantity.AutoSize = true;
        this.labelQuantity.Location = new System.Drawing.Point(380, 173);
        this.labelQuantity.Name = "labelQuantity";
        this.labelQuantity.Size = new System.Drawing.Size(44, 13);
        this.labelQuantity.TabIndex = 9;
        this.labelQuantity.Text = "Số lượng ảnh";
        this.labelQuantity.Visible = false;

        // labelQuantityVideo
        this.labelQuantityVideo.AutoSize = true;
        this.labelQuantityVideo.Location = new System.Drawing.Point(530, 173);
        this.labelQuantityVideo.Name = "labelQuantityVideo";
        this.labelQuantityVideo.Size = new System.Drawing.Size(44, 13);
        this.labelQuantityVideo.TabIndex = 10;
        this.labelQuantityVideo.Text = "Số lượng video";
        this.labelQuantityVideo.Visible = false;

        // 
        // textBoxX
        // 
        this.textBoxX.Location = new System.Drawing.Point(230, 170);
        this.textBoxX.Name = "textBoxStartSegment";
        this.textBoxX.Size = new System.Drawing.Size(50, 20);
        this.textBoxX.TabIndex = 3;
        this.textBoxX.Visible = false;

        // 
        // textBoxY
        // 
        this.textBoxY.Location = new System.Drawing.Point(313, 170);
        this.textBoxY.Name = "textBoxEndSegment";
        this.textBoxY.Size = new System.Drawing.Size(50, 20);
        this.textBoxY.TabIndex = 4;
        this.textBoxY.Visible = false;

        // 
        // textBoxQuantity
        // 
        this.textBoxQuantity.Location = new System.Drawing.Point(470, 170);
        this.textBoxQuantity.Name = "textBoxQuantityImageSegment";
        this.textBoxQuantity.Size = new System.Drawing.Size(50, 20);
        this.textBoxQuantity.TabIndex = 5;
        this.textBoxQuantity.Visible = false;

        // textBoxQuantityVideo
        this.textBoxQuantityVideo.Location = new System.Drawing.Point(630, 170);
        this.textBoxQuantityVideo.Name = "textBoxQuantityVideoSegment";
        this.textBoxQuantityVideo.Size = new System.Drawing.Size(50, 20);
        this.textBoxQuantityVideo.TabIndex = 6;
        this.textBoxQuantityVideo.Visible = false;

        // 
        // saveButton
        // 
        this.saveButton.Location = new System.Drawing.Point(720, 170);
        this.saveButton.Name = "saveButton";
        this.saveButton.Size = new System.Drawing.Size(75, 23);
        this.saveButton.TabIndex = 7;
        this.saveButton.Text = "Save";
        this.saveButton.UseVisualStyleBackColor = true;
        this.saveButton.Visible = false;
        this.saveButton.Click += new System.EventHandler(this.saveButtonRandomSegment);

        // 
        // labelSelectFolderSegment
        // 
        this.labelSelectFolderSegment.Location = new System.Drawing.Point(100, 220);
        this.labelSelectFolderSegment.Name = "labelSelectFolderSegment";
        this.labelSelectFolderSegment.Size = new System.Drawing.Size(150, 40);
        this.labelSelectFolderSegment.TabIndex = 11;
        this.labelSelectFolderSegment.Text = "Chọn thư mục chứa ảnh của các đoạn";
        this.labelSelectFolderSegment.Visible = false;

        //  
        // openFolderSegmentButton
        // 
        this.openFolderSegmentButton.Location = new System.Drawing.Point(250, 220);
        this.openFolderSegmentButton.Name = "openFolderSegmentButton";
        this.openFolderSegmentButton.Size = new System.Drawing.Size(100, 40);
        this.openFolderSegmentButton.TabIndex = 4;
        this.openFolderSegmentButton.Text = "Open Folder";
        this.openFolderSegmentButton.UseVisualStyleBackColor = true;
        this.openFolderSegmentButton.Visible = false;
        this.openFolderSegmentButton.Click += new System.EventHandler(this.openFolderSegmentButton_Click);

        // 
        // reviewFolderAudioButton
        // 
        this.reviewFolderAudioButton.Location = new System.Drawing.Point(100, 100);
        this.reviewFolderAudioButton.Name = "reviewFolderAudioButton";
        this.reviewFolderAudioButton.Size = new System.Drawing.Size(120, 50);
        this.reviewFolderAudioButton.TabIndex = 5;
        this.reviewFolderAudioButton.Text = "Open Audio";
        this.reviewFolderAudioButton.UseVisualStyleBackColor = true;
        this.reviewFolderAudioButton.Click += new System.EventHandler(this.reviewFolderAudioButton_Click);


        // 
        // showStylesButton
        // 
        this.showStylesButton.Location = new System.Drawing.Point(400, 100);
        this.showStylesButton.Name = "showStylesButton";
        this.showStylesButton.Size = new System.Drawing.Size(120, 50);
        this.showStylesButton.TabIndex = 0;
        this.showStylesButton.Text = "Group Styles";
        this.showStylesButton.UseVisualStyleBackColor = true;
        this.showStylesButton.Click += new System.EventHandler(this.showStylesButton_Click);
        // 
        // openFileButton
        //
        this.generateSlideButton.Location = new System.Drawing.Point(550, 100);
        this.generateSlideButton.Name = "generateSlideButton";
        this.generateSlideButton.Size = new System.Drawing.Size(120, 50);
        this.generateSlideButton.TabIndex = 6;
        this.generateSlideButton.Text = "Generate and Save";
        this.generateSlideButton.UseVisualStyleBackColor = true;
        this.generateSlideButton.Click += new System.EventHandler(generateSlideButton_Click);

        // // 
        // // button7
        // // 
        // this.button7.Location = new System.Drawing.Point(700, 100);
        // this.button7.Name = "button7";
        // this.button7.Size = new System.Drawing.Size(120, 50);
        // this.button7.TabIndex = 7;
        // this.button7.Text = "Save to video";
        // this.button7.UseVisualStyleBackColor = true;
        // this.button7.Click += new System.EventHandler(this.button7_Click);


        // 
        // settingsButton
        // 
        this.settingsButton.Location = new System.Drawing.Point(700, 100); // Đặt vị trí cho nút Setting
        this.settingsButton.Name = "settingsButton";
        this.settingsButton.Size = new System.Drawing.Size(120, 50);
        this.settingsButton.TabIndex = 3;
        this.settingsButton.Text = "Setting";
        this.settingsButton.UseVisualStyleBackColor = true;
        this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.WindowState = FormWindowState.Maximized;
        // this.Controls.Add(this.button7);
        this.Controls.Add(this.generateSlideButton);
        this.Controls.Add(this.showStylesButton);
        this.Controls.Add(this.settingsButton);
        this.Controls.Add(this.reviewFolderAudioButton);
        this.Controls.Add(this.labelX);
        this.Controls.Add(this.labelY);
        this.Controls.Add(this.labelQuantity);
        this.Controls.Add(this.labelQuantityVideo);
        this.Controls.Add(this.textBoxX);
        this.Controls.Add(this.textBoxY);
        this.Controls.Add(this.textBoxQuantity);
        this.Controls.Add(this.textBoxQuantityVideo);
        this.Controls.Add(this.saveButton);
        this.Controls.Add(this.labelSelectFolderSegment);
        this.Controls.Add(this.openFolderSegmentButton);
        this.Controls.Add(this.dropdownButton);
        this.Name = "Form1";
        this.Text = "Sắp giàu rùii";
        this.ResumeLayout(false);
    }

    #endregion
}
