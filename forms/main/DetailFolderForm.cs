using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public partial class DetailFolderForm : Form
{
    private List<string> variables;
    private DataGridView dataGridView;
    private Button deleteButton;
    private Button addButton;
    private List<string> selectedFolderSavePaths;

    public List<string> UpdatedVariables { get; private set; }
    public List<string> UpdatedFolderSavePaths { get; private set; }

    public DetailFolderForm(List<string> variables, List<string> selectedFolderSavePaths)
    {
        InitializeComponent();
        this.variables = new List<string>(variables);
        this.selectedFolderSavePaths = new List<string>(selectedFolderSavePaths);
        this.UpdatedVariables = new List<string>(variables); // Initialize with a copy of the input list
        this.UpdatedFolderSavePaths = new List<string>(selectedFolderSavePaths);
        InitializeForm();
    }

    private void InitializeForm()
    {
        this.dataGridView = new DataGridView();
        this.dataGridView.Location = new System.Drawing.Point(10, 10);
        this.dataGridView.Size = new System.Drawing.Size(750, 200);
        this.dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dataGridView.AllowUserToAddRows = false;
        this.dataGridView.Columns.Add("AudioPath", "Audio Path");
        this.dataGridView.Columns.Add("SavePath", "Save Path");
        this.dataGridView.Columns.Add(new DataGridViewButtonColumn
        {
            Name = "SelectSavePath",
            HeaderText = "Select Save Path",
            Text = "Select",
            UseColumnTextForButtonValue = true
        });

        // Adjust column widths
        this.dataGridView.Columns["AudioPath"].Width = 300;
        this.dataGridView.Columns["SavePath"].Width = 300;
        this.dataGridView.Columns["SelectSavePath"].Width = 100;
        this.dataGridView.CellContentClick += DataGridView_CellContentClick;
        this.Controls.Add(this.dataGridView);

        for (int i = 0; i < selectedFolderSavePaths.Count; i++)
        {
            this.dataGridView.Rows.Add(variables[i], selectedFolderSavePaths[i], "Select");
        }

        this.deleteButton = new Button();
        this.deleteButton.Text = "Delete";
        this.deleteButton.Location = new System.Drawing.Point(10, 220);
        this.deleteButton.Click += DeleteButton_Click;
        this.Controls.Add(this.deleteButton);

        this.addButton = new Button();
        this.addButton.Text = "Add";
        this.addButton.Location = new System.Drawing.Point(100, 220);
        this.addButton.Click += AddButton_Click;
        this.Controls.Add(this.addButton);

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
        using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;
                dataGridView.Rows.Add(selectedFolder, "", "Select");
            }
        }
    }

    private void DetailImageForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        UpdatedVariables = new List<string>();
        UpdatedFolderSavePaths = new List<string>();
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (!row.IsNewRow)
            {
                UpdatedVariables.Add(row.Cells["AudioPath"].Value.ToString());
                UpdatedFolderSavePaths.Add(row.Cells["SavePath"].Value.ToString());
            }
        }

    }

}