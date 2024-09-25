using System;
using System.Management;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace WinFormsApp
{

    public partial class LoginForm : Form
    {
        // Chuỗi kết nối đến MongoDB

        public LoginForm()
        {
            InitializeComponent();
            SetSerialNumber();
        }

        // Lấy Serial Number từ Win32_OperatingSystem và set vào TextBox
        private void SetSerialNumber()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                string productId = "";

                foreach (ManagementObject os in searcher.Get())
                {
                    // Lấy Product ID
                    productId = os["SerialNumber"].ToString();
                }

                // Set giá trị vào TextBox
                textBox1.Text = productId;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lấy Serial Number: " + ex.Message);
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string serialNumber = textBox1.Text;
            if (string.IsNullOrEmpty(serialNumber))
            {
                MessageBox.Show("Serial Number is required");
                return;
            }
            bool isValid = await ValidateCredentials(serialNumber);
            if (isValid)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid Serial Number");
            }
        }

        private async Task<bool> ValidateCredentials(string serialNumber)
        {
            var credentialPath = "key.json";
            var spreadsheetId = "1zrrCVQfsTMnchwCLhk1DJLANT1BAmyd6i1d75p0iw0c";

            // Đường dẫn tới tệp JSON bạn đã tải về
            string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            string ApplicationName = "Google Sheets API .NET Quickstart";

            GoogleCredential credential;

            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }
            // Tạo dịch vụ Google Sheets API.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            // Phạm vi của dữ liệu mà bạn muốn đọc (ví dụ: "Sheet1!A1:C10")
            string range = "Data!A2:A";

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Nhận kết quả
            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;

            if (values != null && values.Count >= 1)
            {
                foreach (var row in values)
                {
                    if (row.Count >= 1) // Đảm bảo có đủ 1 cột
                    {
                        string inputSerialNumber = row[0].ToString();
                        if (inputSerialNumber == serialNumber)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

    }
}
