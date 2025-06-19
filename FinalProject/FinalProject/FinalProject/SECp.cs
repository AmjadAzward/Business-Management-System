using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject
{
    public partial class SECp : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";

        string loggedinuser = "";
        public SECp(string userName)
        {
            InitializeComponent();
            this.Paint += RoundedForm_Paint;

            loggedinuser = userName;
            loadpage(userName);
        }

        private void LoadSecretaryData()
        {
            string employeeId = idbox.Text.Trim();

            if (string.IsNullOrEmpty(employeeId))
            {
                MessageBox.Show("Please enter an Employee ID to load data.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT typingSpeed, knownLanguages FROM Secretary WHERE employeeId = @employeeId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@employeeId", employeeId);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                guna2TextBox1.Text = reader["typingSpeed"].ToString();
                                guna2TextBox2.Text = reader["knownLanguages"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("No data found for the given Employee ID.", "Data Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading secretary data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool IsDuplicate(string columnName, string value)
        {
            string query = $"SELECT COUNT(*) FROM Employee WHERE {columnName} = @Value AND userName != @CurrentUserName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Value", value);
                        command.Parameters.AddWithValue("@CurrentUserName", loggedinuser);

                        int count = (int)command.ExecuteScalar();

                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while checking for duplicates: {ex.Message}");
                    return false;
                }
            }
        }

        public void loadpage(string userName)
        {
            string query = "SELECT employeeId, firstName, lastName, userName, email, phoneNumber, password FROM Employee WHERE userName = @Username ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", userName);

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            idbox.Text = reader["employeeId"].ToString();
                            fbox.Text = reader["firstName"].ToString();
                            lbox.Text = reader["lastName"].ToString();
                            ubox.Text = reader["userName"].ToString();
                            mailbox.Text = reader["email"].ToString();
                            phonebox.Text = reader["phoneNumber"].ToString();
                            pwbox.Text = reader["password"].ToString();
                            cpwbox.Text = reader["password"].ToString();

                            string imagePath = Path.Combine("C:\\Users\\USER\\OneDrive\\Pictures\\project", idbox.Text + ".png");
                            if (File.Exists(imagePath))
                            {
                                pictureBox.Image = Image.FromFile(imagePath);
                            }

                        }
                        else
                        {
                            MessageBox.Show("User data not found.");
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void RoundedForm_Paint(object sender, PaintEventArgs e)
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
            path.AddArc(topRightArc, 270, 90);
            path.AddArc(bottomRightArc, 0, 90);
            path.AddArc(bottomLeftArc, 90, 90);

            path.CloseFigure();

            this.Region = new Region(path);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
           "Are you sure you want to log out?",
           "Confirm Logout",
           MessageBoxButtons.YesNo,
           MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Login loginForm = new Login();
                loginForm.Show();
                this.Close();
            }
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {

            Payments mainForm = new Payments(loggedinuser);
            mainForm.Show();
            this.Hide();
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void SECp_Load(object sender, EventArgs e)
        {
            LoadSecretaryData();
        }

        private void btnBrowseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string employeeId = idbox.Text.Trim();

                    if (!string.IsNullOrEmpty(employeeId))
                    {
                        string selectedFilePath = openFileDialog.FileName;

                        string targetDirectory = @"C:\Users\USER\OneDrive\Pictures\project";

                        if (!Directory.Exists(targetDirectory))
                        {
                            Directory.CreateDirectory(targetDirectory);
                        }

                        string newFilePath = Path.Combine(targetDirectory, employeeId + ".png");

                        if (pictureBox.Image != null)
                        {
                            pictureBox.Image.Dispose();
                        }

                        File.Copy(selectedFilePath, newFilePath, true);
                        pictureBox.Image = new System.Drawing.Bitmap(newFilePath);
                        MessageBox.Show("Image uploaded and updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid employee ID.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading or renaming image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No file selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void updbtnprof_Click(object sender, EventArgs e)
        {
            if (IsDuplicate("userName", ubox.Text))
            {
                MessageBox.Show("The username is already taken.");
                return;
            }

            if (IsDuplicate("email", mailbox.Text))
            {
                MessageBox.Show("The email is already in use.");
                return;
            }

            if (IsDuplicate("phoneNumber", phonebox.Text))
            {
                MessageBox.Show("The phone number is already in use.");
                return;
            }

            if (string.IsNullOrEmpty(fbox.Text.Trim()) || !fbox.Text.All(char.IsLetter))
            {
                MessageBox.Show("First Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fbox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(lbox.Text.Trim()) || !lbox.Text.All(char.IsLetter))
            {
                MessageBox.Show("Last Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lbox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(ubox.Text.Trim()))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ubox.Focus();
                return;
            }

            if (ubox.Text.Length > 50)
            {
                MessageBox.Show("Username cannot exceed 50 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ubox.Focus();
                return;
            }

            string emailRegexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (string.IsNullOrEmpty(mailbox.Text.Trim()) || !Regex.IsMatch(mailbox.Text.Trim(), emailRegexPattern))
            {
                MessageBox.Show("A valid Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mailbox.Focus();
                return;
            }


            if (string.IsNullOrEmpty(phonebox.Text.Trim()) || !long.TryParse(phonebox.Text.Trim(), out _) || phonebox.Text.Trim().Length != 10)
            {
                MessageBox.Show("A valid Phone Number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                phonebox.Focus();
                return;
            }


            string passwordRegexPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,50}$";

            if (string.IsNullOrEmpty(pwbox.Text.Trim()) || pwbox.Text.Length < 8 || !Regex.IsMatch(pwbox.Text, passwordRegexPattern))
            {
                MessageBox.Show("Password must be 8-50 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pwbox.Focus();
                return;
            }

            if (pwbox.Text.Trim() != cpwbox.Text.Trim())
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cpwbox.Focus();
                return;
            }

            string query = "UPDATE Employee SET firstName = @FirstName, lastName = @LastName, email = @Email, phoneNumber = @PhoneNumber, password = @Password, userName = @user  WHERE employeeId = @eid";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", fbox.Text);
                        command.Parameters.AddWithValue("@LastName", lbox.Text);
                        command.Parameters.AddWithValue("@Email", mailbox.Text);
                        command.Parameters.AddWithValue("@PhoneNumber", phonebox.Text);
                        command.Parameters.AddWithValue("@Password", pwbox.Text);
                        command.Parameters.AddWithValue("@Username", ubox.Text);
                        command.Parameters.AddWithValue("@eid", idbox.Text);
                        command.Parameters.AddWithValue("@user", ubox.Text);



                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("User data updated successfully.");
                            loggedinuser = ubox.Text;
                        }
                        else
                        {
                            MessageBox.Show("No user data found to update.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            string typingSpeedText = guna2TextBox1.Text.Trim();
            string knownLanguages = guna2TextBox2.Text.Trim();
            string employeeId = idbox.Text.Trim();

            if (string.IsNullOrEmpty(employeeId))
            {
                MessageBox.Show("Employee ID is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                idbox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(typingSpeedText) || !int.TryParse(typingSpeedText, out int typingSpeed) || typingSpeed <= 0)
            {
                MessageBox.Show("Please enter a valid typing speed.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox1.Focus();
                return;
            }

            if (string.IsNullOrEmpty(knownLanguages))
            {
                MessageBox.Show("Known languages cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox2.Focus();
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string checkQuery = "SELECT COUNT(1) FROM Secretary WHERE employeeId = @employeeId";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@employeeId", employeeId);
                        int count = (int)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            string updateQuery = "UPDATE Secretary " +
                                                 "SET typingSpeed = @typingSpeed, knownLanguages = @knownLanguages " +
                                                 "WHERE employeeId = @employeeId";
                            using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@typingSpeed", typingSpeed);
                                updateCommand.Parameters.AddWithValue("@knownLanguages", knownLanguages);
                                updateCommand.Parameters.AddWithValue("@employeeId", employeeId);

                                updateCommand.ExecuteNonQuery();
                                MessageBox.Show("Secretary record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            string insertQuery = "INSERT INTO Secretary (employeeId, typingSpeed, knownLanguages) " +
                                                 "VALUES (@employeeId, @typingSpeed, @knownLanguages)";
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@employeeId", employeeId);
                                insertCommand.Parameters.AddWithValue("@typingSpeed", typingSpeed);
                                insertCommand.Parameters.AddWithValue("@knownLanguages", knownLanguages);

                                insertCommand.ExecuteNonQuery();
                                MessageBox.Show("Secretary record added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error performing operation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
