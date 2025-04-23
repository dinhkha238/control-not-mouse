using System;
using System.Windows.Forms;

namespace WinFormsApp.Forms
{
    public partial class ProgressForm : Form
    {
        public bool IsPaused { get; private set; } = false;
        public bool IsCancelled { get; private set; } = false;

        public ProgressForm()
        {
            InitializeComponent();
        }

        public void SetStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetStatus), status);
                return;
            }

            labelStatus.Text = status;
        }
        public void DisableCancelButton()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(DisableCancelButton));
            }
            else
            {
                btnCancel.Enabled = false;
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsCancelled = true;
            Close(); // Optional: close the form if you want
        }
    }
}
