public partial class DetailImageForm : Form
{
    private List<string> variables;
    private ListBox listBox;
    private Button deleteButton;
    private Button addButton;
    private Label imageCountLabel;
    private Label videoCountLabel;

    private int imageCount = 0;
    private int videoCount = 0;

    public List<string> UpdatedVariables { get; private set; }

    public DetailImageForm(List<string> variables)
    {
        InitializeComponent();
        this.variables = new List<string>(variables);
        this.UpdatedVariables = new List<string>(variables); // Initialize with a copy of the input list
        InitializeForm();
        UpdateCounts(); // Update counts upon initialization
    }

    private void InitializeForm()
    {
        this.listBox = new ListBox();
        this.listBox.Location = new System.Drawing.Point(25, 5);
        this.listBox.Size = new System.Drawing.Size(950, 400);
        this.listBox.SelectionMode = SelectionMode.MultiExtended;
        this.listBox.DataSource = variables;
        this.Controls.Add(this.listBox);

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

        // Label for image count
        this.imageCountLabel = new Label();
        this.imageCountLabel.Text = "Images: 0";
        this.imageCountLabel.Size = new System.Drawing.Size(75, 20);
        this.imageCountLabel.Location = new System.Drawing.Point(215, 423);
        this.Controls.Add(this.imageCountLabel);

        // Label for video count
        this.videoCountLabel = new Label();
        this.videoCountLabel.Text = "Videos: 0";
        this.videoCountLabel.Location = new System.Drawing.Point(290, 423);
        this.Controls.Add(this.videoCountLabel);

        this.FormClosing += DetailImageForm_FormClosing;
    }

    private void DeleteButton_Click(object sender, EventArgs e)
    {
        var selectedItems = listBox.SelectedItems.Cast<string>().ToList();
        foreach (var item in selectedItems)
        {
            variables.Remove(item);
            UpdateCounts();
        }
        listBox.DataSource = null;
        listBox.DataSource = variables;
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Multiselect = true; // Allow multiple file selection
            openFileDialog.Filter = "Image and Video Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.mp4;*.avi;*.mov;*.wmv";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    variables.Add(file);
                    if (IsImageFile(file))
                        imageCount++;
                    else if (IsVideoFile(file))
                        videoCount++;
                }
                UpdateLabels();
                listBox.DataSource = null;
                listBox.DataSource = variables;
            }
        }
    }

    private void UpdateLabels()
    {
        imageCountLabel.Text = $"Images: {imageCount}";
        videoCountLabel.Text = $"Videos: {videoCount}";
    }

    private void UpdateCounts()
    {
        imageCount = variables.Count(IsImageFile);
        videoCount = variables.Count(IsVideoFile);
        UpdateLabels();
    }

    private bool IsImageFile(string file)
    {
        string extension = System.IO.Path.GetExtension(file).ToLower();
        return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif" || extension == ".bmp";
    }

    private bool IsVideoFile(string file)
    {
        string extension = System.IO.Path.GetExtension(file).ToLower();
        return extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".wmv";
    }

    private void DetailImageForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        UpdatedVariables = new List<string>(variables); // Update the property before closing the form
    }
}
