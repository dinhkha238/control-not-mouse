namespace WinFormsApp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    // private Button openImage;
    private Button button7;
    private Button generateSlideButton;
    private Button showStylesButton;
    private Button settingsButton;
    private Button reviewFolderAudioButton;
    private Button openImage;

    // private Button openFileButton;

    private List<string[]> selectedFileImagePaths = new List<string[]>();
    private List<string[]> selectedFileAudioPaths = new List<string[]>();
    private List<string> selectedFolderAudioPaths = new List<string>();
    private List<string> selectedFolderSavePaths = new List<string>();


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
        this.openImage = new System.Windows.Forms.Button();
        this.SuspendLayout();

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
        // openImage
        // 
        this.openImage.Location = new System.Drawing.Point(250, 100);
        this.openImage.Name = "openImage";
        this.openImage.Size = new System.Drawing.Size(120, 50);
        this.openImage.TabIndex = 2;
        this.openImage.Text = "Open Image";
        this.openImage.UseVisualStyleBackColor = true;
        this.openImage.Click += new System.EventHandler(this.openImage_Click);

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
        this.Controls.Add(this.openImage);
        // this.Controls.Add(this.button7);
        this.Controls.Add(this.generateSlideButton);
        this.Controls.Add(this.showStylesButton);
        this.Controls.Add(this.settingsButton);
        this.Controls.Add(this.reviewFolderAudioButton);
        this.Name = "Form1";
        this.Text = "Sắp giàu rùii";
        this.ResumeLayout(false);
    }

    #endregion
}
