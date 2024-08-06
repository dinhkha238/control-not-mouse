using System;
using System.IO;
using System.Windows.Forms;
using WinFormsApp;

public class StylesForm : Form
{
    private ListBox listBox;
    private Button saveButton;
    private TextBox nameTextBox;
    private Label nameLabel;
    private Form1 form1;

    public StylesForm(Form1 form1)
    {
        InitializeComponent();
        LoadStyles();
        this.form1 = form1;
    }

    private void InitializeComponent()
    {
        this.listBox = new System.Windows.Forms.ListBox();
        this.saveButton = new System.Windows.Forms.Button();
        this.nameTextBox = new System.Windows.Forms.TextBox();
        this.nameLabel = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // listBox
        // 
        this.listBox.FormattingEnabled = true;
        this.listBox.Location = new System.Drawing.Point(12, 12);
        this.listBox.Name = "listBox";
        this.listBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
        this.listBox.Size = new System.Drawing.Size(260, 199);
        this.listBox.TabIndex = 0;
        // 
        // nameTextBox
        // 
        this.nameTextBox.Location = new System.Drawing.Point(12, 230);
        this.nameTextBox.Name = "nameTextBox";
        this.nameTextBox.Size = new System.Drawing.Size(179, 20);
        this.nameTextBox.TabIndex = 1;
        // 
        // nameLabel
        // 
        this.nameLabel.AutoSize = true;
        this.nameLabel.Location = new System.Drawing.Point(12, 215);
        this.nameLabel.Name = "nameLabel";
        this.nameLabel.Size = new System.Drawing.Size(35, 13);
        this.nameLabel.TabIndex = 2;
        this.nameLabel.Text = "Name";
        // 
        // saveButton
        // 
        this.saveButton.Location = new System.Drawing.Point(197, 229);
        this.saveButton.Name = "saveButton";
        this.saveButton.Size = new System.Drawing.Size(75, 23);
        this.saveButton.TabIndex = 3;
        this.saveButton.Text = "Save";
        this.saveButton.UseVisualStyleBackColor = true;
        this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
        // 
        // StylesForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(284, 288);
        this.Controls.Add(this.saveButton);
        this.Controls.Add(this.nameLabel);
        this.Controls.Add(this.nameTextBox);
        this.Controls.Add(this.listBox);
        this.Name = "StylesForm";
        this.Text = "StylesForm";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void LoadStyles()
    {
        string stylesFolder = "styles"; // Replace with the actual path to the styles folder
        if (Directory.Exists(stylesFolder))
        {
            string[] files = Directory.GetFiles(stylesFolder);
            listBox.Items.AddRange(files);
        }
        else
        {
            MessageBox.Show("Styles folder not found.");
        }
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        string name = nameTextBox.Text;
        if (string.IsNullOrEmpty(name))
        {
            MessageBox.Show("Please enter a name.");
            return;
        }

        string groupFolder = "groups"; // Replace with the actual path to the group folder
        if (!Directory.Exists(groupFolder))
        {
            Directory.CreateDirectory(groupFolder);
        }

        string savePath = Path.Combine(groupFolder, name + ".txt");
        using (StreamWriter writer = new StreamWriter(savePath))
        {
            foreach (var item in listBox.SelectedItems)
            {
                writer.WriteLine(item.ToString());
            }
        }
        // Gọi phương thức UpdateComboBox của Form1 để cập nhật ComboBox
        form1.UpdateComboBoxes();
        MessageBox.Show("Selected items saved.");
    }
}