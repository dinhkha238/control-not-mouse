public partial class DetailImageForm : Form
{
    private List<string> variables;
    private ListBox imageListBox;
    private ListBox videoListBox;
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
        // Label for Image ListBox
        Label imageLabel = new Label();
        imageLabel.Text = "Images";
        imageLabel.Location = new System.Drawing.Point(25, 5); // Position above imageListBox
        imageLabel.Size = new System.Drawing.Size(50, 20);
        this.Controls.Add(imageLabel);

        // ListBox for images
        this.imageListBox = new ListBox();
        this.imageListBox.Location = new System.Drawing.Point(25, 30); // Adjust position below the label
        this.imageListBox.Size = new System.Drawing.Size(450, 375);
        this.imageListBox.SelectionMode = SelectionMode.MultiExtended;
        this.Controls.Add(this.imageListBox);

        // Label for Video ListBox
        Label videoLabel = new Label();
        videoLabel.Text = "Videos";
        videoLabel.Location = new System.Drawing.Point(525, 5); // Position above videoListBox
        videoLabel.Size = new System.Drawing.Size(50, 20);
        this.Controls.Add(videoLabel);

        // ListBox for videos
        this.videoListBox = new ListBox();
        this.videoListBox.Location = new System.Drawing.Point(525, 30); // Adjust position below the label
        this.videoListBox.Size = new System.Drawing.Size(450, 375);
        this.videoListBox.SelectionMode = SelectionMode.MultiExtended;
        this.Controls.Add(this.videoListBox);

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
        UpdateListBoxes(); // Populate ListBoxes based on initial variables
    }

    private void DeleteButton_Click(object sender, EventArgs e)
    {
        var selectedImages = imageListBox.SelectedItems.Cast<string>().ToList();
        var selectedVideos = videoListBox.SelectedItems.Cast<string>().ToList();

        foreach (var item in selectedImages)
        {
            variables.Remove(item);
        }
        foreach (var item in selectedVideos)
        {
            variables.Remove(item);
        }

        UpdateCounts();
        UpdateListBoxes();
    }

    private void AddButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Multiselect = true;
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
                UpdateListBoxes();
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

    private void UpdateListBoxes()
    {
        var images = variables.Where(IsImageFile).ToList();
        var videos = variables.Where(IsVideoFile).ToList();

        imageListBox.DataSource = null;
        imageListBox.DataSource = images;
        imageListBox.SelectedIndex = -1; // Clear selection in imageListBox

        videoListBox.DataSource = null;
        videoListBox.DataSource = videos;
        videoListBox.SelectedIndex = -1; // Clear selection in videoListBox
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
