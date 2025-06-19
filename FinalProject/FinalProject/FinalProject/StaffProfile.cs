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
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject
{
    public partial class StaffProfile : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        string loggedinuser = "";
        public StaffProfile(string userName)
        {
            InitializeComponent();
            this.Paint += RoundedForm_Paint;

            loggedinuser = userName;
            currentUsername = userName;

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

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            COO mainForm = new COO(loggedinuser);
            mainForm.Show();
            this.Hide();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();

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

        private void StaffProfile_Load(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            COOp mainForm = new COOp(currentUsername);
            mainForm.Show();
            this.Hide();
        }
        private void ClearInputFields()
        {
            searchbox.Clear();
            fnamebox.Clear();
            lnamebox.Clear();
            emailbox.Clear();
            phonenobox.Clear();
        }
        private void searchbtn_Click(object sender, EventArgs e)
        {

            string searchText = searchbox.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                ClearInputFields(); // Clear textboxes if search is empty
                return;
            }

            string query = @"SELECT employeeId, firstName, lastName, email, phoneNumber 
                     FROM Employee
                     WHERE LOWER(employeeId) = LOWER(@searchText)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@searchText", searchText);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            fnamebox.Text = reader["firstName"].ToString();
                            lnamebox.Text = reader["lastName"].ToString();
                            emailbox.Text = reader["email"].ToString();
                            phonenobox.Text = reader["phoneNumber"].ToString();
                            LoadEmployeeImage(searchText);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No employee found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        ClearInputFields(); // Clear textboxes if no match found
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public void LoadEmployeeImage(string employeeId)
        {
            string imagePath = Path.Combine("C:\\Users\\USER\\OneDrive\\Pictures\\project", employeeId + ".png");

            if (File.Exists(imagePath))
            {
                pictureBoxProfile.Image = Image.FromFile(imagePath);
            }
            else
            {
                // Set a default image if the employee's image is not found
                string defaultImagePath = "C:\\Users\\USER\\Desktop\\photos\\image-recognition (1).png";

                if (File.Exists(defaultImagePath))
                {
                    pictureBoxProfile.Image = Image.FromFile(defaultImagePath);
                }
                else
                {
                    pictureBoxProfile.Image = null; // Optionally, set it to null if the default image isn't found
                                                    // MessageBox.Show("Image not found, and default image is also missing.");
                }
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            // Get values from the textboxes
            string employeeId = searchbox.Text.Trim();
            string firstName = fnamebox.Text.Trim();
            string lastName = lnamebox.Text.Trim();
            string email = emailbox.Text.Trim();
            string phoneNumber = phonenobox.Text.Trim();

            // Validate that all fields are filled in
            if (string.IsNullOrEmpty(employeeId))
            {
                MessageBox.Show("Please enter the Employee ID.");
                searchbox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Please enter the First Name.");
                fnamebox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Please enter the Last Name.");
                lnamebox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid Email.");
                emailbox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Please enter the Phone Number.");
                phonenobox.Focus();
                return;
            }

            // SQL Query to update the employee record
            string query = @"UPDATE Employee 
                     SET firstName = @firstName, 
                         lastName = @lastName, 
                         email = @email, 
                         phoneNumber = @phoneNumber
                     WHERE employeeId = @employeeId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputFields(); // Clear input fields after successful update
                    }
                    else
                    {
                        MessageBox.Show("Error: Record not updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Eremove_Click(object sender, EventArgs e)
        {
            // Get the employeeId of the selected row
            string employeeId = searchbox.Text;

            // Confirm deletion
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this employee?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.No)
            {
                return; // If the user selects No, exit the method without deleting
            }

            // SQL queries for deleting from each related table
            List<string> tablesToDeleteFrom = new List<string>
    {
        "CEO", "Storekeeper", "FactoryManager", "TechnicalOfficer", "Designer",
        "COO", "SalesExecutive", "Secretary", "TeamLeader", "QuantitySurveyor",
        "Accountant", "AssistantAccountant", "ProductionManager", "Administration", "CustomerRelation"
    };

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Delete from all related tables
                        foreach (string table in tablesToDeleteFrom)
                        {
                            string query = $"DELETE FROM {table} WHERE employeeId = @employeeId";
                            SqlCommand cmd = new SqlCommand(query, conn, transaction);
                            cmd.Parameters.AddWithValue("@employeeId", employeeId);
                            cmd.ExecuteNonQuery();
                        }

                        // Now delete from Employee table
                        string deleteEmployeeQuery = @"DELETE FROM Employee WHERE employeeId = @employeeId";
                        SqlCommand deleteEmployeeCmd = new SqlCommand(deleteEmployeeQuery, conn, transaction);
                        deleteEmployeeCmd.Parameters.AddWithValue("@employeeId", employeeId);
                        deleteEmployeeCmd.ExecuteNonQuery();

                        // Commit the transaction
                        transaction.Commit();

                        MessageBox.Show("Employee and all related data deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputFields();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // Rollback if something fails
                        MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
