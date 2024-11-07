using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public partial class DetailFolderForm : Form
{
    private List<string> variables;
    private List<string> selectedFolderSavePaths;
    private List<string> selectedFileIntroPaths;
    private DataGridView dataGridView;
    private Button deleteButton;
    private Button addButton;
    private CheckBox addAudioCheckBox = new CheckBox();

    public List<string> UpdatedVariables { get; private set; }
    public List<string> UpdatedFolderSavePaths { get; private set; }
    public List<string> UpdatedFileIntroPaths { get; private set; }
    public bool AddAudioCheckBox { get; private set; }

    public DetailFolderForm(List<string> variables, List<string> selectedFolderSavePaths, List<string> selectedFileIntroPaths, bool addAudioCheckBox = true)
    {
        InitializeComponent();
        this.variables = new List<string>(variables);
        this.selectedFolderSavePaths = new List<string>(selectedFolderSavePaths);
        this.selectedFileIntroPaths = new List<string>(selectedFileIntroPaths);
        this.addAudioCheckBox.Checked = addAudioCheckBox;
        this.UpdatedVariables = new List<string>(variables);
        this.UpdatedFolderSavePaths = new List<string>(selectedFolderSavePaths);
        this.UpdatedFileIntroPaths = new List<string>(selectedFileIntroPaths);
        this.AddAudioCheckBox = addAudioCheckBox;
        InitializeForm();
    }

    private void InitializeForm()
    {
        this.dataGridView = new DataGridView();
        this.dataGridView.Location = new System.Drawing.Point(25, 5);
        this.dataGridView.Size = new System.Drawing.Size(1200, 400); // Adjust width to fit new columns
        this.dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridView.AllowUserToAddRows = false;
        this.dataGridView.Columns.Add("AudioPath", "Audio Path");
        this.dataGridView.Columns.Add("SavePath", "Save Path");

        // Button column to select save path
        this.dataGridView.Columns.Add(new DataGridViewButtonColumn
        {
            Name = "SelectSavePath",
            HeaderText = "Select Save Path",
            Text = "Select",
            UseColumnTextForButtonValue = true
        });

        // Column for Intro Path
        this.dataGridView.Columns.Add("IntroPath", "Intro File Path");

        // Button column to select intro file
        this.dataGridView.Columns.Add(new DataGridViewButtonColumn
        {
            Name = "SelectIntroPath",
            HeaderText = "Select Intro File",
            Text = "Select",
            UseColumnTextForButtonValue = true
        });

        // Adjust column widths
        this.dataGridView.Columns["AudioPath"].Width = 350;
        this.dataGridView.Columns["SavePath"].Width = 350;
        this.dataGridView.Columns["SelectSavePath"].Width = 50;
        this.dataGridView.Columns["IntroPath"].Width = 350;
        this.dataGridView.Columns["SelectIntroPath"].Width = 50;
        this.dataGridView.CellContentClick += DataGridView_CellContentClick;
        this.Controls.Add(this.dataGridView);

        for (int i = 0; i < selectedFolderSavePaths.Count; i++)
        {
            this.dataGridView.Rows.Add(variables[i], selectedFolderSavePaths[i], "Select", selectedFileIntroPaths[i], "Select");
        }

        this.deleteButton = new Button();
        this.deleteButton.Text = "Delete";
        this.deleteButton.Location = new System.Drawing.Point(25, 420);
        this.deleteButton.Click += DeleteButton_Click;
        this.Controls.Add(this.deleteButton);

        this.addButton = new Button();
        this.addButton.Text = "Add";
        this.addButton.Location = new System.Drawing.Point(115, 420);
        this.addButton.Click += AddButton_Click;
        this.Controls.Add(this.addButton);

        // Add CheckBox "Add audio file"
        this.addAudioCheckBox = new CheckBox();
        this.addAudioCheckBox.Text = "Add audio file";
        this.addAudioCheckBox.Location = new System.Drawing.Point(215, 420);
        this.addAudioCheckBox.Checked = this.AddAudioCheckBox;
        this.addAudioCheckBox.CheckedChanged += (sender, e) =>
        {
            this.AddAudioCheckBox = this.addAudioCheckBox.Checked;
        };
        this.Controls.Add(this.addAudioCheckBox);

        this.FormClosing += DetailImageForm_FormClosing;
    }

    private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == dataGridView.Columns["SelectSavePath"].Index && e.RowIndex >= 0)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = folderBrowserDialog.SelectedPath;
                    dataGridView.Rows[e.RowIndex].Cells["SavePath"].Value = selectedFolder;
                }
            }
        }
        else if (e.ColumnIndex == dataGridView.Columns["SelectIntroPath"].Index && e.RowIndex >= 0)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Intro Files|*.mp4;*.avi;*.mov;*.mkv"; // Adjust filter for intro files
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = openFileDialog.FileName;
                    dataGridView.Rows[e.RowIndex].Cells["IntroPath"].Value = selectedFile;
                }
            }
        }
    }

    private void DeleteButton_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in dataGridView.SelectedRows)
        {
            if (!row.IsNewRow)
            {
                dataGridView.Rows.Remove(row);
            }
        }
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
        if (addAudioCheckBox.Checked)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "Audio Files|*.mp3;*.wav;*.flac;*.aac;*.ogg;*.wma";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] selectedFiles = openFileDialog.FileNames;
                    foreach (string file in selectedFiles)
                    {
                        dataGridView.Rows.Add(file, "", "", "", "Select");
                    }
                }
            }
        }
        else
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFolder = folderBrowserDialog.SelectedPath;
                    dataGridView.Rows.Add("", selectedFolder, "Select", "", "Select");
                }
            }
        }
    }

    private void DetailImageForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        UpdatedVariables = new List<string>();
        UpdatedFolderSavePaths = new List<string>();
        UpdatedFileIntroPaths = new List<string>();

        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (!row.IsNewRow)
            {
                UpdatedVariables.Add(row.Cells["AudioPath"].Value?.ToString());
                UpdatedFolderSavePaths.Add(row.Cells["SavePath"].Value?.ToString());
                UpdatedFileIntroPaths.Add(row.Cells["IntroPath"].Value?.ToString());
            }
        }
    }
}
