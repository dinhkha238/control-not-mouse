namespace WinFormsApp
{
    partial class LoginForm
    {
        // Các thành phần giao diện
        private System.ComponentModel.IContainer components = null;
        private Label label1;
        private TextBox textBox1;
        private Button button1;

        /// <summary>
        /// Giải phóng tài nguyên đang sử dụng.
        /// </summary>
        /// <param name="disposing">True nếu tài nguyên đã được quản lý cần giải phóng; ngược lại, False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Phương thức khởi tạo các thành phần giao diện.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // Label
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Serial Number của bạn là:";

            // TextBox
            this.textBox1.Location = new System.Drawing.Point(20, 50);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Enabled = false;
            this.textBox1.Size = new System.Drawing.Size(250, 23);
            this.textBox1.TabIndex = 1;

            // Button
            this.button1.Location = new System.Drawing.Point(100, 100);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 30);
            this.button1.TabIndex = 2;
            this.button1.Text = "Đăng nhập";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnLogin_Click);

            // Form settings
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 180);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
