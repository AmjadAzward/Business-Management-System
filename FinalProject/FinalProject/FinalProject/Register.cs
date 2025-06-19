using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProject
{
    public partial class Register : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";

        public Register()
        {
            InitializeComponent();
            this.Paint += RoundedForm_Paint;

        }

        private void RoundedForm_Paint(object sender, PaintEventArgs e)
        {

            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                int radius = 30; 
                int diameter = radius * 2;

                GraphicsPath path = new GraphicsPath();

                Rectangle topLeftArc = new Rectangle(0, 0, diameter, diameter);
                Rectangle topRightArc = new Rectangle(this.Width - diameter, 0, diameter, diameter);
                Rectangle bottomRightArc = new Rectangle(this.Width - diameter, this.Height - diameter, diameter, diameter);
                Rectangle bottomLeftArc = new Rectangle(0, this.Height - diameter, diameter, diameter);

                path.AddArc(topLeftArc, 180, 90);  
                path.AddArc(topRightArc, 270, 90); // Top-right corner
                path.AddArc(bottomRightArc, 0, 90); // Bottom-right corner
                path.AddArc(bottomLeftArc, 90, 90); // Bottom-left corner

                path.CloseFigure(); 

                this.Region = new Region(path); 
            }
        }
        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {
            Login mainForm = new Login();
            mainForm.Show();
            this.Hide();
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }
        private string GenerateEmployeeId()
        {
           string newId = "EMP001"; 

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 employeeId FROM Employee ORDER BY CAST(SUBSTRING(employeeId, 4, LEN(employeeId)) AS INT) DESC ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();
                            int numericPart = int.Parse(lastId.Substring(3));
                            newId = "EMP" + (numericPart + 1).ToString("D3"); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Employee ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }

        private bool CheckUserNameUnique(string userName)
        {
            using (SqlConnection connection = new SqlConnection("Server=AmjadAzward\\SQLEXPRESS;Database=finalPJS;Trusted_Connection=True;"))
            {
                string query = "SELECT COUNT(*) FROM Employee WHERE userName = @userName";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@userName", userName);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                connection.Close();

                return count == 0; 
            }
        }


        private bool CheckEmailUnique(string email)
        {
            using (SqlConnection connection = new SqlConnection("Server=AmjadAzward\\SQLEXPRESS;Database=finalPJS;Trusted_Connection=True;"))
            {
                string query = "SELECT COUNT(*) FROM Employee WHERE email = @Email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                connection.Close();

                return count == 0;
            }
        }


        private bool CheckPhoneNumberUnique(string phoneNumber)
        {
            using (SqlConnection connection = new SqlConnection("Server=AmjadAzward\\SQLEXPRESS;Database=finalPJS;Trusted_Connection=True;"))
            {
                string query = "SELECT COUNT(*) FROM Employee WHERE phoneNumber = @PhoneNumber";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                connection.Close();

                return count == 0; 
            }
        }


        private void regbtn_Click(object sender, EventArgs e)
        {

            if (cmbPosition.SelectedItem == null)
            {
                MessageBox.Show("Please select a Position.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbPosition.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtFirstName.Text.Trim()) || !txtFirstName.Text.All(char.IsLetter))
            {
                MessageBox.Show("First Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFirstName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtLastName.Text.Trim()) || !txtLastName.Text.All(char.IsLetter))
            {
                MessageBox.Show("Last Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtLastName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUserName.Focus();
                return;
            }

            if (txtUserName.Text.Length > 50)
            {
                MessageBox.Show("Username cannot exceed 50 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUserName.Focus();
                return;
            }

            bool isUserNameUnique = CheckUserNameUnique(txtUserName.Text.Trim());
            if (!isUserNameUnique)
            {
                MessageBox.Show("Username already exists. Please choose another.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUserName.Focus();
                return;
            }
            
            string emailRegexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (string.IsNullOrEmpty(txtEmail.Text.Trim()) || !Regex.IsMatch(txtEmail.Text.Trim(), emailRegexPattern))
            {
                MessageBox.Show("A valid Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return;
            }

            bool isEmailUnique = CheckEmailUnique(txtEmail.Text.Trim());
            if (!isEmailUnique)
            {
                MessageBox.Show("Email address already exists. Please use another.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtPhoneNumber.Text.Trim()) || !long.TryParse(txtPhoneNumber.Text.Trim(), out _) || txtPhoneNumber.Text.Trim().Length != 10)
            {
                MessageBox.Show("A valid Phone Number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPhoneNumber.Focus();
                return;
            }

            bool isPhoneNumberUnique = CheckPhoneNumberUnique(txtPhoneNumber.Text.Trim());
            if (!isPhoneNumberUnique)
            {
                MessageBox.Show("Phone Number already exists. Please use another.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPhoneNumber.Focus();
                return;
            }

            string passwordRegexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,50}$";

            if (string.IsNullOrEmpty(txtPassword.Text.Trim()) || txtPassword.Text.Length < 8 || !Regex.IsMatch(txtPassword.Text, passwordRegexPattern))
            {
                MessageBox.Show("Password must be 8-50 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Focus();
                return;
            }

            if (txtPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConfirmPassword.Focus();
                return;
            }

            string employeeId = GenerateEmployeeId();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Employee (employeeId, firstName, lastName, userName, email, phoneNumber, password, position) " +
                                   "VALUES (@employeeId, @firstName, @lastName, @userName, @email, @phoneNumber, @password, @position)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@employeeId", employeeId);
                        command.Parameters.AddWithValue("@firstName", txtFirstName.Text.Trim());
                        command.Parameters.AddWithValue("@lastName", txtLastName.Text.Trim());
                        command.Parameters.AddWithValue("@userName", txtUserName.Text.Trim());
                        command.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        command.Parameters.AddWithValue("@phoneNumber", txtPhoneNumber.Text.Trim());
                        command.Parameters.AddWithValue("@password", txtPassword.Text.Trim());
                        command.Parameters.AddWithValue("@position", cmbPosition.SelectedItem.ToString());

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            this.Hide();
                            Login loginForm = new Login();
                            loginForm.Show();
                        }
                        else
                        {
                            MessageBox.Show("Registration failed. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          
        }
    }
}
