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
    public partial class StaffSalaryAcc : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;

        string loggedinuser = "";
        public StaffSalaryAcc(string userName)
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

        private void guna2Button10_Click(object sender, EventArgs e)
        {

        }

        private void addbtnsal_Click(object sender, EventArgs e)
        {
            
            
        }

        private void StaffSalaryAcc_Load(object sender, EventArgs e)
        {
            LoadEmployeeIdsIntoComboBox();
            LoadSalaryData();
            LoadSalaryDataIntoDGV();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            AccountantP mainForm = new AccountantP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        //
        //
        //


        private void LoadEmployeeIdsIntoComboBox()
        {

            string query = "SELECT employeeId FROM Employee";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    employeecombo.Items.Clear();

                    while (reader.Read())
                    {
                        string employeeId = reader["employeeId"].ToString();
                        employeecombo.Items.Add(employeeId);
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database error: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void LoadEmployeeNameById(string employeeId)
        {
            string query = "SELECT firstName, lastName FROM Employee WHERE employeeId = @employeeId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string firstName = reader["firstName"].ToString();
                        string lastName = reader["lastName"].ToString();
                        txtEmployeeName.Text = $"{firstName} {lastName}";
                    }
                    else
                    {
                        txtEmployeeName.Clear();
                        MessageBox.Show("No employee found with the given ID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database error: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

  



        private void employeecombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string employeeId = employeecombo.SelectedItem.ToString();
            LoadEmployeeNameById(employeeId);


        }

        private void delbtnsal_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LoadSalaryData()
        {
        
        }



        private string GenerateSalaryId()
        {
            // Default ID if the table is empty
            string newId = "SAL001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Query to fetch the last salary ID with prefix 'SAL' in descending order
                    string query = @"
                SELECT TOP 1 SalaryId 
                FROM SalaryPayments
                WHERE SalaryId LIKE 'SAL%' 
                ORDER BY CAST(SUBSTRING(SalaryId, 4, LEN(SalaryId) - 3) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Extract the numeric part from the last ID
                            string lastId = result.ToString();
                            int numericPart = int.Parse(lastId.Substring(3)); // Extract the numeric part after 'SAL'

                            // Generate a new ID by incrementing the numeric part
                            newId = "SAL" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Show an error message in case of exceptions
                MessageBox.Show($"Error generating Salary ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }


        private async void guna2Button10_Click_1(object sender, EventArgs e)
        {
            string salaryId = GenerateSalaryId();

            // Basic validation for input fields
            if (string.IsNullOrEmpty(employeecombo.Text.Trim()))
            {
                MessageBox.Show("Employee ID cannot be empty. Please select an employee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                employeecombo.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtEmployeeName.Text.Trim()))
            {
                MessageBox.Show("Employee Name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmployeeName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(basictxt.Text.Trim()))
            {
                MessageBox.Show("Basic salary cannot be empty. Please enter the basic salary.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                basictxt.Focus();
                return;
            }

            if (string.IsNullOrEmpty(Paymentdate.Text.Trim()))
            {
                MessageBox.Show("Payment date cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Paymentdate.Focus();
                return;
            }

            if (string.IsNullOrEmpty(statuscombo.Text.Trim()))
            {
                MessageBox.Show("Please select the payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                statuscombo.Focus();
                return;
            }

            try
            {
                // Calculate attendance details
                DateTime selectedDate = Paymentdate.Value;
                int year = selectedDate.Year;
                int month = selectedDate.Month;
                string employeeId = employeecombo.Text;

                (int totalAttendance, int daysAbsent) = await Task.Run(() =>
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Query to fetch attendance and absent days
                        string attendanceQuery = @"
                    SELECT 
                        ISNULL(COUNT(*), 0) AS TotalAttendance, 
                        ISNULL(SUM(CASE WHEN attendanceStatus = 'Absent' THEN 1 ELSE 0 END), 0) AS DaysAbsent
                    FROM [finalPJS].[dbo].[EmployeeAttendance]
                    WHERE YEAR(attendanceDate) = @year
                      AND MONTH(attendanceDate) = @month
                      AND employeeId = @employeeId;";

                        using (SqlCommand command = new SqlCommand(attendanceQuery, connection))
                        {
                            command.Parameters.AddWithValue("@year", year);
                            command.Parameters.AddWithValue("@month", month);
                            command.Parameters.AddWithValue("@employeeId", employeeId);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int total = reader.GetInt32(0);
                                    int absent = reader.GetInt32(1);
                                    return (total, absent);
                                }
                            }
                        }
                    }
                    return (0, 0); // Return default values if no data is found
                });

                // Handle attendance results
                if (totalAttendance == 0 && daysAbsent == 0)
                {
                    MessageBox.Show(this, "No attendance records found for the selected employee.",
                                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Calculate bonus, holidays, and deductions
                int holidays = daysAbsent < 5 ? 5 - daysAbsent : 0;
                int extraDays = daysAbsent > 5 ? daysAbsent - 5 : 0;
                decimal bonus = holidays * 1000;
                decimal deduction = extraDays * 1100;

                holidaycount.Text = holidays.ToString();
                txtBonus.Text = bonus.ToString();
                txtDeduction.Text = deduction.ToString();

                // Insert salary record
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                INSERT INTO SalaryPayments (salaryId, basicSalary, availableLeaveCount, bonus, deduction, finalAmount, salaryPaymentStatus, employeeId)
                VALUES (@salaryId, @basicSalary, @availableLeaveCount, @bonus, @deduction, @finalAmount, @salaryPaymentStatus, @employeeId)";

                    SqlCommand command = new SqlCommand(query, connection);

                    decimal basicSalary = decimal.Parse(basictxt.Text.Trim());
                    decimal finalAmount = basicSalary + bonus - deduction;
                    finalamt.Text = finalAmount.ToString();

                    command.Parameters.AddWithValue("@salaryId", salaryId);
                    command.Parameters.AddWithValue("@basicSalary", basicSalary);
                    command.Parameters.AddWithValue("@availableLeaveCount", holidays);
                    command.Parameters.AddWithValue("@bonus", bonus);
                    command.Parameters.AddWithValue("@deduction", deduction);
                    command.Parameters.AddWithValue("@finalAmount", finalAmount);
                    command.Parameters.AddWithValue("@salaryPaymentStatus", statuscombo.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Salary record added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearinputs();
                        LoadSalaryDataIntoDGV();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSalaryDataIntoDGV()
        {
            string query = "SELECT * FROM SalaryPayments";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvSalary.DataSource = dataTable; // Bind data to DataGridView
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading salary data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearSalaryForm()
        {
            basictxt.Clear();
            txtBonus.Clear();
            txtDeduction.Clear();
            holidaycount.Clear();
            employeecombo.SelectedIndex=0;
            statuscombo.SelectedIndex = -1; // Reset the dropdown selection
         //   txtBasicSalary.Focus(); // Set focus to the first input field
        }






        private void clearinputs()
        {
            employeecombo.SelectedIndex = 0;
            txtBonus.Clear();
            txtDeduction.Clear();
            txtEmployeeName.Clear();
            basictxt.Clear();
            finalamt.Clear();
            holidaycount.Clear();
            statuscombo.SelectedIndex = -1;
            Paymentdate.Value = DateTime.Now;
        }


        private void paymentdate_ValueChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(employeecombo.Text))
            {
                MessageBox.Show(this, "Please select an employee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
               
            }
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {
            clearinputs();
        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private void Paymentdate_ValueChanged_1(object sender, EventArgs e)
        {
         
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {

        }

        private async void guna2ButtonUpdate_Click(object sender, EventArgs e)
        {
            // Basic validation for input fields
            if (string.IsNullOrEmpty(employeecombo.Text.Trim()))
            {
                MessageBox.Show("Employee ID cannot be empty. Please select an employee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                employeecombo.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtEmployeeName.Text.Trim()))
            {
                MessageBox.Show("Employee Name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmployeeName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(basictxt.Text.Trim()))
            {
                MessageBox.Show("Basic salary cannot be empty. Please enter the basic salary.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                basictxt.Focus();
                return;
            }

            if (string.IsNullOrEmpty(Paymentdate.Text.Trim()))
            {
                MessageBox.Show("Payment date cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Paymentdate.Focus();
                return;
            }

            if (string.IsNullOrEmpty(statuscombo.Text.Trim()))
            {
                MessageBox.Show("Please select the payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                statuscombo.Focus();
                return;
            }

            try
            {
                // Calculate attendance details
                DateTime selectedDate = Paymentdate.Value;
                int year = selectedDate.Year;
                int month = selectedDate.Month;
                string employeeId = employeecombo.Text;

                (int totalAttendance, int daysAbsent) = await Task.Run(() =>
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Query to fetch attendance and absent days
                        string attendanceQuery = @"
                SELECT 
                    ISNULL(COUNT(*), 0) AS TotalAttendance, 
                    ISNULL(SUM(CASE WHEN attendanceStatus = 'Absent' THEN 1 ELSE 0 END), 0) AS DaysAbsent
                FROM [finalPJS].[dbo].[EmployeeAttendance]
                WHERE YEAR(attendanceDate) = @year
                  AND MONTH(attendanceDate) = @month
                  AND employeeId = @employeeId;";

                        using (SqlCommand command = new SqlCommand(attendanceQuery, connection))
                        {
                            command.Parameters.AddWithValue("@year", year);
                            command.Parameters.AddWithValue("@month", month);
                            command.Parameters.AddWithValue("@employeeId", employeeId);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int total = reader.GetInt32(0);
                                    int absent = reader.GetInt32(1);
                                    return (total, absent);
                                }
                            }
                        }
                    }
                    return (0, 0); // Return default values if no data is found
                });

                // Handle attendance results
                if (totalAttendance == 0 && daysAbsent == 0)
                {
                    MessageBox.Show(this, "No attendance records found for the selected employee.",
                                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Calculate bonus, holidays, and deductions
                int holidays = daysAbsent < 5 ? 5 - daysAbsent : 0;
                int extraDays = daysAbsent > 5 ? daysAbsent - 5 : 0;
                decimal bonus = holidays * 1000;
                decimal deduction = extraDays * 1100;

                holidaycount.Text = holidays.ToString();
                txtBonus.Text = bonus.ToString();
                txtDeduction.Text = deduction.ToString();

                // Update salary record
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
            UPDATE SalaryPayments
            SET 
                basicSalary = @basicSalary,
                availableLeaveCount = @availableLeaveCount,
                bonus = @bonus,
                deduction = @deduction,
                finalAmount = @finalAmount,
                salaryPaymentStatus = @salaryPaymentStatus
            WHERE salaryId = @salaryId;";

                    SqlCommand command = new SqlCommand(query, connection);

                    decimal basicSalary = decimal.Parse(basictxt.Text.Trim());
                    decimal finalAmount = basicSalary + bonus - deduction;
                    finalamt.Text = finalAmount.ToString();
                    string salaryId = dgvSalary.SelectedRows.Count > 0 ? dgvSalary.SelectedRows[0].Cells[0].Value.ToString() : null;

                    if (salaryId == null)
                    {
                        MessageBox.Show("No salary record selected for update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    command.Parameters.AddWithValue("@salaryId", salaryId); // Ensure the salaryId is passed correctly
                    command.Parameters.AddWithValue("@basicSalary", basicSalary);
                    command.Parameters.AddWithValue("@availableLeaveCount", holidays);
                    command.Parameters.AddWithValue("@bonus", bonus);
                    command.Parameters.AddWithValue("@deduction", deduction);
                    command.Parameters.AddWithValue("@finalAmount", finalAmount);
                    command.Parameters.AddWithValue("@salaryPaymentStatus", statuscombo.SelectedItem.ToString());

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    // Provide feedback based on whether the update was successful
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Salary record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        clearinputs(); // Reset input fields after successful update
                        LoadSalaryDataIntoDGV(); // Reload the updated salary data into the DataGridView
                    }
                    else
                    {
                        MessageBox.Show("No record found to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // Check if a row is selected in the DataGridView
            if (dgvSalary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm deletion
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the selected salary record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Get the selected salaryId
                string salaryId = dgvSalary.SelectedRows[0].Cells[0].Value.ToString();

                // Delete salary record from the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM SalaryPayments WHERE salaryId = @salaryId;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@salaryId", salaryId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        connection.Close();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Salary record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadSalaryDataIntoDGV(); // Reload the DataGridView to reflect changes
                        }
                        else
                        {
                            MessageBox.Show("No record found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSalary_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvSalary_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is not on a header row
            if (e.RowIndex >= 0)
            {
                // Retrieve the clicked row
                DataGridViewRow clickedRow = dgvSalary.Rows[e.RowIndex];

                // Assuming 'salaryId' is in the first column (index 0)
                // string employeeName = clickedRow.Cells["employeeName"].Value?.ToString();
                decimal basicSalary = Convert.ToDecimal(clickedRow.Cells["basicSalary"].Value);
                int availableLeaveCount = Convert.ToInt32(clickedRow.Cells["availableLeaveCount"].Value);
                decimal bonus = Convert.ToDecimal(clickedRow.Cells["bonus"].Value);
                decimal deduction = Convert.ToDecimal(clickedRow.Cells["deduction"].Value);
                decimal finalAmount = Convert.ToDecimal(clickedRow.Cells["finalAmount"].Value);
                string salaryPaymentStatus = clickedRow.Cells["salaryPaymentStatus"].Value?.ToString();
                string employeeId = clickedRow.Cells["employeeId"].Value?.ToString();

                // Update TextBox controls with the retrieved values
                // txtEmployeeName.Text = employeeName;
                basictxt.Text = basicSalary.ToString();
                holidaycount.Text = availableLeaveCount.ToString();
                txtBonus.Text = bonus.ToString();
                txtDeduction.Text = deduction.ToString();
                finalamt.Text = finalAmount.ToString();
                statuscombo.SelectedItem = salaryPaymentStatus;
                employeecombo.SelectedItem = employeeId;
            }
        }
    }
}
