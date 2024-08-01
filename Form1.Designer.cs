﻿namespace WinFormsApp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private Button button3;
    private Button button7;
    private Button generateSlideButton;

    // private Button openFileButton;

    private List<string[]> selectedFileImagePaths = new List<string[]>();
    private List<string> selectedFileAudioPaths = new List<string>();


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
        // openFileButton
        //
        this.generateSlideButton.Location = new System.Drawing.Point(250, 100);
        this.generateSlideButton.Name = "generateSlideButton";
        this.generateSlideButton.Size = new System.Drawing.Size(120, 50);
        this.generateSlideButton.TabIndex = 6;
        this.generateSlideButton.Text = "Generate Slide";
        this.generateSlideButton.UseVisualStyleBackColor = true;
        this.generateSlideButton.Click += new System.EventHandler(generateSlideButton_Click);

        // 
        // button7
        // 
        this.button7.Location = new System.Drawing.Point(400, 100);
        this.button7.Name = "button7";
        this.button7.Size = new System.Drawing.Size(120, 50);
        this.button7.TabIndex = 7;
        this.button7.Text = "Save to video";
        this.button7.UseVisualStyleBackColor = true;
        this.button7.Click += new System.EventHandler(this.button7_Click);

        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.WindowState = FormWindowState.Maximized;
        this.Controls.Add(this.button3);
        this.Controls.Add(this.button7);
        this.Controls.Add(this.generateSlideButton);
        this.Name = "Form1";
        this.Text = "Notepad Example";
        this.ResumeLayout(false);
    }

    #endregion
}
