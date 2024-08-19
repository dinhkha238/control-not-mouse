using System;
using dotenv.net;
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
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Username and Password are required");
                return;
            }


            bool isValid = await ValidateCredentials(username, password);
            if (isValid)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password");
            }
        }

        private async Task<bool> ValidateCredentials(string inputUsername, string inputPassword)
        {
            // Đọc chuỗi kết nối đến MongoDB từ biến môi trường
            DotEnv.Load();
            var credentialPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
            var spreadsheetId = Environment.GetEnvironmentVariable("SPREADSHEET_ID");

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
            string range = "Data!A:B";

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Nhận kết quả
            ValueRange response = request.Execute();
            IList<IList<object>> values = response.Values;

            if (values != null && values.Count > 1)
            {
                bool isValid = false;

                // Bỏ qua hàng đầu tiên (label)
                foreach (var row in values.Skip(1))
                {
                    if (row.Count >= 2) // Đảm bảo có đủ 2 cột
                    {
                        string username = row[0].ToString();
                        string password = row[1].ToString();

                        if (username == inputUsername && password == inputPassword)
                        {
                            isValid = true;
                            break;
                        }
                    }
                }

                if (isValid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
