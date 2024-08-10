namespace WinFormsApp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private Button button3;
    private Button button7;
    private Button generateSlideButton;
    private Button showStylesButton;
    private Button settingsButton;

    // private Button openFileButton;

    private List<string[]> selectedFileImagePaths = new List<string[]>();
    private List<string> selectedFileAudioPaths = new List<string>();
    // private List<string> selectedGroupPaths = new List<string>();


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
        this.button3 = new Button();
        this.button7 = new Button();
        this.generateSlideButton = new Button();
        this.showStylesButton = new System.Windows.Forms.Button();
        this.settingsButton = new System.Windows.Forms.Button();
        this.SuspendLayout();

        // 
        // button3
        // 
        this.button3.Location = new System.Drawing.Point(100, 100);
        this.button3.Name = "button3";
        this.button3.Size = new System.Drawing.Size(120, 50);
        this.button3.TabIndex = 2;
        this.button3.Text = "Open Audio";
        this.button3.UseVisualStyleBackColor = true;
        this.button3.Click += new System.EventHandler(this.OpenAudioButton_Click);

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
        // openFileButton
        //
        this.generateSlideButton.Location = new System.Drawing.Point(400, 100);
        this.generateSlideButton.Name = "generateSlideButton";
        this.generateSlideButton.Size = new System.Drawing.Size(120, 50);
        this.generateSlideButton.TabIndex = 6;
        this.generateSlideButton.Text = "Generate Slide";
        this.generateSlideButton.UseVisualStyleBackColor = true;
        this.generateSlideButton.Click += new System.EventHandler(generateSlideButton_Click);

        // 
        // button7
        // 
        this.button7.Location = new System.Drawing.Point(550, 100);
        this.button7.Name = "button7";
        this.button7.Size = new System.Drawing.Size(120, 50);
        this.button7.TabIndex = 7;
        this.button7.Text = "Save to video";
        this.button7.UseVisualStyleBackColor = true;
        this.button7.Click += new System.EventHandler(this.button7_Click);
        // 
        // showStylesButton
        // 
        this.showStylesButton.Location = new System.Drawing.Point(250, 100);
        this.showStylesButton.Name = "showStylesButton";
        this.showStylesButton.Size = new System.Drawing.Size(120, 50);
        this.showStylesButton.TabIndex = 0;
        this.showStylesButton.Text = "Group Styles";
        this.showStylesButton.UseVisualStyleBackColor = true;
        this.showStylesButton.Click += new System.EventHandler(this.showStylesButton_Click);
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.WindowState = FormWindowState.Maximized;
        this.Controls.Add(this.button3);
        this.Controls.Add(this.button7);
        this.Controls.Add(this.generateSlideButton);
        this.Controls.Add(this.showStylesButton);
        this.Controls.Add(this.settingsButton);
        this.Name = "Form1";
        this.Text = "Notepad Example";
        this.ResumeLayout(false);
    }

    #endregion
}
