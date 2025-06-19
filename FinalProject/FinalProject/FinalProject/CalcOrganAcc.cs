using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject
{
    public partial class CalcOrganAcc : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;

        string loggedinuser = "";
        public CalcOrganAcc(string userName)
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

        private void CalcOrganAcc_Load(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {

            Accountant mainForm = new Accountant(loggedinuser);
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

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            AccountantP mainForm = new AccountantP(currentUsername);
            mainForm.Show();
            this.Hide();
        }


        private void LoadDataIntoDataGridView()
        {
            try
            {

                string query = "SELECT * FROM [finalPJS].[dbo].[OrganizationalPayment];";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateOrganizationalId()
        {
            string newId = "ORG001"; // Default ID in case no records exist

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Query to get the last OrganizationalId from the OrganizationalPayment table
                    string query = @"
                SELECT TOP 1 orgPaymentId 
                FROM OrganizationalPayment 
                WHERE orgPaymentId LIKE 'ORG%' 
                ORDER BY CAST(SUBSTRING(orgPaymentId, 4, LEN(orgPaymentId) - 3) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Get the last ID from the result
                            string lastId = result.ToString();

                            // Extract the numeric part of the last ID and increment it by 1
                            int numericPart = int.Parse(lastId.Substring(3)); // Extract the numeric part after 'ORG'

                            // Generate a new ID by incrementing the numeric part
                            newId = "ORG" + (numericPart + 1).ToString("D3"); // Format to always have 3 digits (e.g., ORG001, ORG002, ORG003)
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Organizational ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }

        private void clear4()
        {
            type.SelectedIndex = -1;
            comboOrg.SelectedIndex = -1;
            date.Value = DateTime.Now;
            amt.Clear();
        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {
            // Input validations
            if (type.SelectedItem == null)
            {
                MessageBox.Show("Please select an Organizational Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                type.Focus();
                return;
            }

            if (!decimal.TryParse(amt.Text.Trim(), out decimal typeCost) || typeCost <= 0)
            {
                MessageBox.Show("Cost must be a valid positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                amt.Focus();
                return;
            }

            if (date.Value > DateTime.Now)
            {
                MessageBox.Show("Payment date cannot be in the future.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                date.Focus();
                return;
            }

            if (comboOrg.SelectedItem == null)
            {
                MessageBox.Show("Please select a Payment Status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboOrg.Focus();
                return;
            }

            // Get selected values
            string orgType = type.SelectedItem.ToString();
            string orgBillPaymentStatus = comboOrg.SelectedItem.ToString();
            DateTime orgPaymentDate = date.Value;

            // Database insertion
            try
            {

                string query = @"
            INSERT INTO [finalPJS].[dbo].[OrganizationalPayment] 
            ([orgPaymentId], [orgType], [typeCost], [orgPaymentDate], [orgBillPaymentStatus])
            VALUES (@orgPaymentId,@orgType, @typeCost, @orgPaymentDate, @orgBillPaymentStatus);";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@orgPaymentId", GenerateOrganizationalId());

                        command.Parameters.AddWithValue("@orgType", orgType);
                        command.Parameters.AddWithValue("@typeCost", typeCost);
                        command.Parameters.AddWithValue("@orgPaymentDate", orgPaymentDate);
                        command.Parameters.AddWithValue("@orgBillPaymentStatus", orgBillPaymentStatus);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Organizational payment added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Optionally reset controls after successful insertion
                            clear4();
                            LoadDataIntoDataGridView();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add organizational payment. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                type.SelectedItem = row.Cells["orgType"].Value.ToString();
                amt.Text = row.Cells["typeCost"].Value.ToString();
                date.Value = DateTime.Parse(row.Cells["orgPaymentDate"].Value.ToString());
                comboOrg.SelectedItem = row.Cells["orgBillPaymentStatus"].Value.ToString();
            }
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            // Input validations
            if (type.SelectedItem == null)
            {
                MessageBox.Show("Please select an Organizational Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                type.Focus();
                return;
            }

            if (!decimal.TryParse(amt.Text.Trim(), out decimal typeCost) || typeCost <= 0)
            {
                MessageBox.Show("Cost must be a valid positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                amt.Focus();
                return;
            }

            if (date.Value > DateTime.Now)
            {
                MessageBox.Show("Payment date cannot be in the future.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                date.Focus();
                return;
            }

            if (comboOrg.SelectedItem == null)
            {
                MessageBox.Show("Please select a Payment Status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboOrg.Focus();
                return;
            }

            // Get the PaymentID from the first row of the DataGridView
            string paymentID = dataGridView1.Rows[0].Cells["orgPaymentId"].Value.ToString();

            // Get selected values
            string orgType = type.SelectedItem.ToString();
            string orgBillPaymentStatus = comboOrg.SelectedItem.ToString();
            DateTime orgPaymentDate = date.Value;

            try
            {

                string query = @"
            UPDATE [finalPJS].[dbo].[OrganizationalPayment]
            SET orgType = @orgType, typeCost = @typeCost, orgPaymentDate = @orgPaymentDate, orgBillPaymentStatus = @orgBillPaymentStatus
            WHERE orgPaymentId = @PaymentID;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaymentID", paymentID);
                        command.Parameters.AddWithValue("@orgType", orgType);
                        command.Parameters.AddWithValue("@typeCost", typeCost);
                        command.Parameters.AddWithValue("@orgPaymentDate", orgPaymentDate);
                        command.Parameters.AddWithValue("@orgBillPaymentStatus", orgBillPaymentStatus);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDataGridView();
                            clear4();
                        }

                        else
                        {
                            MessageBox.Show("Failed to update the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {
            // Get the PaymentID from the first row of the DataGridView
            string paymentID = dataGridView1.Rows[0].Cells["orgPaymentId"].Value.ToString();

            try
            {

                string query = "DELETE FROM [finalPJS].[dbo].[OrganizationalPayment] WHERE orgPaymentId = @PaymentID;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PaymentID", paymentID);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDataGridView();
                            clear4();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the record.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            clear4();
        }

        private void dgv1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                type.SelectedItem = row.Cells["orgType"].Value.ToString();
                amt.Text = row.Cells["typeCost"].Value.ToString();
                date.Value = DateTime.Parse(row.Cells["orgPaymentDate"].Value.ToString());
                comboOrg.SelectedItem = row.Cells["orgBillPaymentStatus"].Value.ToString();
            }
        }
    }
}
