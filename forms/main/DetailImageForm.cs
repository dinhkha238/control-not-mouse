public partial class DetailImageForm : Form
{
    private List<string> variables;
    private ListBox listBox;
    private Button deleteButton;
    private Button addButton;

    public List<string> UpdatedVariables { get; private set; }

    public DetailImageForm(List<string> variables)
    {
        InitializeComponent();
        this.variables = new List<string>(variables);
        this.UpdatedVariables = new List<string>(variables); // Initialize with a copy of the input list
        InitializeForm();
    }

    private void InitializeForm()
    {
        this.listBox = new ListBox();
        this.listBox.Location = new System.Drawing.Point(10, 10);
        this.listBox.Size = new System.Drawing.Size(950, 200);
        this.listBox.SelectionMode = SelectionMode.MultiExtended;
        this.listBox.DataSource = variables;
        this.Controls.Add(this.listBox);

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

    private void DeleteButton_Click(object sender, EventArgs e)
    {
        var selectedItems = listBox.SelectedItems.Cast<string>().ToList();
        foreach (var item in selectedItems)
        {
            variables.Remove(item);
        }
        listBox.DataSource = null;
        listBox.DataSource = variables;
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Multiselect = true; // Cho phép chọn nhiều tệp
            openFileDialog.Filter = "Image and Video Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.mp4;*.avi;*.mov;*.wmv";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    variables.Add(file);
                }
                listBox.DataSource = null;
                listBox.DataSource = variables;
            }
        }
    }

    private void DetailImageForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        UpdatedVariables = new List<string>(variables); // Update the property before closing the form
    }
}
