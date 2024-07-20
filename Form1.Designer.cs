namespace WinFormsApp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private Button button1;
    private Button button2;
    private Button button3;
    private Button button4;
    private Button button5;
    private Button button6;
    private Button button7;


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
        this.button1 = new Button();
        this.button2 = new Button();
        this.button3 = new Button();
        this.button4 = new Button();
        this.button5 = new Button();
        this.button6 = new Button();
        this.button7 = new Button();
        this.SuspendLayout();

        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(100, 100);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(100, 50);
        this.button1.TabIndex = 0;
        this.button1.Text = "Open Notepad";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(this.button1_Click);


        // 
        // button2
        // 
        this.button2.Location = new System.Drawing.Point(200, 100);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(100, 50);
        this.button2.TabIndex = 1;
        this.button2.Text = "Maximize Notepad";
        this.button2.UseVisualStyleBackColor = true;
        this.button2.Click += new System.EventHandler(this.button2_Click);
        // 
        // button3
        // 
        this.button3.Location = new System.Drawing.Point(310, 100);
        this.button3.Name = "button3";
        this.button3.Size = new System.Drawing.Size(120, 50);
        this.button3.TabIndex = 2;
        this.button3.Text = "Open ProShow Producer 9";
        this.button3.UseVisualStyleBackColor = true;
        this.button3.Click += new System.EventHandler(this.button3_Click);
        // 
        // button4
        // 
        this.button4.Location = new System.Drawing.Point(100, 200);
        this.button4.Name = "button4";
        this.button4.Size = new System.Drawing.Size(120, 50);
        this.button4.TabIndex = 3;
        this.button4.Text = "Click Open in ProShow";
        this.button4.UseVisualStyleBackColor = true;
        this.button4.Click += new System.EventHandler(this.button4_Click);

        // 
        // button5
        // 
        this.button5.Location = new System.Drawing.Point(200, 200);
        this.button5.Name = "button5";
        this.button5.Size = new System.Drawing.Size(200, 23);
        this.button5.TabIndex = 4;
        this.button5.Text = "Add Image to Slide";
        this.button5.UseVisualStyleBackColor = true;
        this.button5.Click += new System.EventHandler(this.button5_Click);

        // 
        // button6
        // 
        this.button6.Location = new System.Drawing.Point(100, 300);
        this.button6.Name = "button6";
        this.button6.Size = new System.Drawing.Size(200, 23);
        this.button6.TabIndex = 5;
        this.button6.Text = "Add Audio to Slide";
        this.button6.UseVisualStyleBackColor = true;
        this.button6.Click += new System.EventHandler(this.button6_Click);

        // 
        // button7
        // 

        this.button7.Location = new System.Drawing.Point(100, 400);
        this.button7.Name = "button7";
        this.button7.Size = new System.Drawing.Size(200, 23);
        this.button7.TabIndex = 6;
        this.button7.Text = "Save to Slide";
        this.button7.UseVisualStyleBackColor = true;
        this.button7.Click += new System.EventHandler(this.button7_Click);


        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(555, 555);
        this.Controls.Add(this.button1);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.button3);
        this.Controls.Add(this.button4);
        this.Controls.Add(this.button5);
        this.Controls.Add(this.button6);
        this.Controls.Add(this.button7);
        this.Name = "Form1";
        this.Text = "Notepad Example";
        this.ResumeLayout(false);
    }

    #endregion
}
