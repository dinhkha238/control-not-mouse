using System;
using dotenv.net;
using MongoDB.Bson;
using MongoDB.Driver;

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

        private async Task<bool> ValidateCredentials(string username, string password)
        {

            // Tạo một filter để tìm kiếm tài liệu
            var filter = Builders<BsonDocument>.Filter.Eq("username", username) & Builders<BsonDocument>.Filter.Eq("password", password);
            // Kiểm tra sự tồn tại của tài liệu
            var count = await collection.CountDocumentsAsync(filter);
            if (count == 1)
            {
                return true;
            }
            return false;
        }

    }
}
