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

namespace FinalProject
{
    public partial class QuantitySurveyor : Form
    {

        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;
        public QuantitySurveyor(string userName)
        {
            InitializeComponent();
            this.Paint += RoundedForm_Paint;

            currentUsername = userName;

            string loggedinUsername;


            employeeId = GetEmployeeId(currentUsername);

            if (!string.IsNullOrEmpty(employeeId))
            {
                LoadEmployeeTasks(employeeId);
                loggedinUsername = userName;
            }
            else
            {
                MessageBox.Show("Employee ID not found for the current user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetEmployeeId(string userName)
        {
            string empId = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT employeeId FROM Employee WHERE userName = @UserName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserName", userName);

                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        empId = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving employee ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return empId;
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

        private void LoadEmployeeTasks(string employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT 
                    taskId, 
                    taskName, 
                    deadline, 
                    status, 
                    priority, 
                    assignee
                FROM 
                    Task
                WHERE 
                    assignee = @EmployeeId";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.Add("@EmployeeId", SqlDbType.NVarChar).Value = employeeId;
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    acctask.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No tasks found for the specified employee.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void guna2TileButton7_Click(object sender, EventArgs e)
        {
            BudgetSubmit mainForm = new BudgetSubmit();
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton6_Click(object sender, EventArgs e)
        {
            CostCalculation mainForm = new CostCalculation();
            mainForm.Show();
            this.Hide();

        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;

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

        private void guna2ImageButton3_Click_1(object sender, EventArgs e)
        {
            QuantityP mainForm = new QuantityP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void QuantitySurveyor_Load(object sender, EventArgs e)
        {
            LoadEmployeeTasks(employeeId);

            LoadFeedbackIntoDGV(employeeId);
            FeedbackData();
            LoadEmployeeData();

            LoadProjectsData();
            LoadProjectIds();



            CountEmployees();
            GetMostRecentMeetingForUser(currentUsername);
            CountSuppliers();
          //  CountPastProjects();
            LoadTaskCounts(currentUsername);
            LoadLatestSalary(currentUsername);

        }

        private void acctask_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = acctask.Rows[e.RowIndex];

                accTaskNames.Text = selectedRow.Cells["taskName"].Value.ToString();
                accDeadline.Text = Convert.ToDateTime(selectedRow.Cells["deadline"].Value).ToString("yyyy-MM-dd");
                cmbaccStatuss.SelectedItem = selectedRow.Cells["status"].Value.ToString();
                accPriority.Text = selectedRow.Cells["priority"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }

        private void updbtnacc_Click(object sender, EventArgs e)
        {
            try
            {
                string taskId = acctask.CurrentRow.Cells["taskId"].Value.ToString();
                string taskName = accTaskNames.Text;
                string deadline = accDeadline.Text;
                string status = cmbaccStatuss.SelectedItem.ToString();
                string priority = accPriority.Text;

                string query = @"UPDATE Task SET taskName = @taskName,
                deadline = @deadline,
                status = @status,
                priority = @priority
                WHERE taskId = @taskId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@taskName", taskName);
                        command.Parameters.AddWithValue("@deadline", deadline);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@priority", priority);
                        command.Parameters.AddWithValue("@taskId", taskId);

                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Task updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadEmployeeTasks(employeeId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating task: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {

        }
        //
        //
        //
        //

        private string GenerateFeedbackId()
        {
            string newId = "F001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 feedbackId FROM Feedback ORDER BY CAST(SUBSTRING(feedbackId, 2, LEN(feedbackId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();

                            int numericPart = int.Parse(lastId.Substring(1));
                            newId = "F" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Feedback ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }





        private void LoadFeedbackIntoDGV(string employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT * 
                FROM Feedback 
                WHERE employeeId = @employeeId";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.Add("@employeeId", SqlDbType.NVarChar).Value = employeeId;

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvFeedback.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No feedbacks found for you.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void FeedbackData()
        {
            dgvFeedback.Columns["feedbackId"].HeaderText = "ID";
            dgvFeedback.Columns["feedback"].HeaderText = "Feedback";
            dgvFeedback.Columns["feedbackType"].HeaderText = "Type";
            dgvFeedback.Columns["response"].HeaderText = "Response";
            dgvFeedback.Columns["employeeId"].HeaderText = "Employee ID";


            dgvFeedback.Columns["employeeId"].DisplayIndex = 1;

            dgvFeedback.Columns["feedback"].Width = 150;
            dgvFeedback.Columns["feedbackId"].Width = 50;
            dgvFeedback.Columns["employeeId"].Width = 70;


        }


        private void ClearFeedbackForm()
        {
            txtFeedback.Text = string.Empty;
            cmbFeedbackType.SelectedIndex = -1;

        }

        private void LoadEmployeeData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT employeeId, firstName, lastName FROM Employee WHERE userName = @userName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.Add("@userName", SqlDbType.NVarChar).Value = currentUsername;

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        txtEmpId.Text = reader["employeeId"].ToString();

                        string firstName = reader["firstName"].ToString();
                        string lastName = reader["lastName"].ToString();
                        txtEmpName.Text = firstName + " " + lastName;

                    }
                    else
                    {
                        // MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employee data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sendc_Click(object sender, EventArgs e)
        {
            string feedbackId = GenerateFeedbackId();

            if (string.IsNullOrEmpty(txtFeedback.Text.Trim()))
            {
                MessageBox.Show("Feedback cannot be empty. Please enter feedback.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFeedback.Focus();
                return;
            }

            if (cmbFeedbackType.SelectedItem == null)
            {
                MessageBox.Show("Please select a feedback type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbFeedbackType.Focus();
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Feedback (feedbackId, feedback, feedbackType, employeeId) " +
                                   "VALUES (@feedbackId, @feedback, @feedbackType, @employeeId)";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@feedbackId", feedbackId);
                    command.Parameters.AddWithValue("@feedback", txtFeedback.Text.Trim());
                    command.Parameters.AddWithValue("@feedbackType", cmbFeedbackType.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@employeeId", txtEmpId.Text.Trim()); // Ensure employeeId is used


                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Feedback added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearFeedbackForm();
                        LoadFeedbackIntoDGV(employeeId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding feedback: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvFeedback_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvFeedback.Rows[e.RowIndex];

                string feedbackText = selectedRow.Cells["feedback"].Value.ToString();
                string employeeId = selectedRow.Cells["employeeId"].Value.ToString();
                string feedbackType = selectedRow.Cells["feedbackType"].Value.ToString();
                string employeeName = GetEmployeeNameById(employeeId);

                txtFeedback.Text = feedbackText;
                txtEmpId.Text = employeeId;
                cmbFeedbackType.SelectedItem = feedbackType;
                txtEmpName.Text = employeeName;
            }
        }

        private string GetEmployeeNameById(string employeeId)
        {
            string employeeName = string.Empty;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT firstName, lastName FROM Employee WHERE employeeId = @employeeId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string firstName = reader["firstName"].ToString();
                        string lastName = reader["lastName"].ToString();
                        employeeName = firstName + " " + lastName;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching employee name: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return employeeName;
        }


        //
        //
        //
        //

        private void LoadProjectIds()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT projectId FROM Project";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    cmbProjectId.DataSource = dataTable;
                    cmbProjectId.DisplayMember = "projectId";
                    cmbProjectId.ValueMember = "projectId";
                    cmbProjectId.SelectedIndex = -1; // No selection by default
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project IDs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbProjectId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProjectId.SelectedIndex < 0) return;

            try
            {
                string selectedProjectId = cmbProjectId.SelectedValue.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT p.projectName, p.clientId 
                             FROM Project p
                             WHERE p.projectId = @projectId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@projectId", selectedProjectId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        txtProjectName.Text = reader["projectName"].ToString();
                        txtClientId.Text = reader["clientId"].ToString(); // Updated to load client ID
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading project details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2HtmlLabel15_Click(object sender, EventArgs e)
        {

        }



        private void addpj_Click(object sender, EventArgs e)
        {
            if (cmbProjectId.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a valid Project ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbProjectId.Focus();
                return;
            }

            if (!decimal.TryParse(txtLabourCost.Text, out decimal labourCost) || labourCost < 0)
            {
                MessageBox.Show("Please enter a valid Labour Cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtLabourCost.Focus();
                return;
            }

            if (!decimal.TryParse(txtMaterialCost.Text, out decimal materialCost) || materialCost < 0)
            {
                MessageBox.Show("Please enter a valid Material Cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMaterialCost.Focus();
                return;
            }

            decimal totalCost = labourCost + materialCost;
            txtTotalCost.Text = totalCost.ToString("0.00");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string checkQuery = @"SELECT COUNT(*) FROM Project
                                  WHERE projectId = @projectId 
                                  AND (projectLabourCost IS NOT NULL AND projectLabourCost > 0)
                                  AND (projectMaterialCost IS NOT NULL AND projectMaterialCost > 0)
                                  AND (projectTotalCost IS NOT NULL AND projectTotalCost > 0)";

                    SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    checkCommand.Parameters.AddWithValue("@projectId", cmbProjectId.SelectedValue.ToString());

                    connection.Open();
                    int existingProjectCosts = (int)checkCommand.ExecuteScalar();

                    if (existingProjectCosts > 0)
                    {
                        MessageBox.Show("Project costs are already added for this project.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        string updateQuery = @"UPDATE Project
                                       SET projectLabourCost = @labourCost,
                                           projectMaterialCost = @materialCost,
                                           projectTotalCost = @totalCost
                                       WHERE projectId = @projectId";

                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@projectId", cmbProjectId.SelectedValue.ToString());
                        updateCommand.Parameters.AddWithValue("@labourCost", labourCost);
                        updateCommand.Parameters.AddWithValue("@materialCost", materialCost);
                        updateCommand.Parameters.AddWithValue("@totalCost", totalCost);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Project costs Added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearInputs(); // Clear the input fields after insertion
                            LoadProjectsData(); // Refresh the data grid view to show the updated data
                        }
                        else
                        {
                            MessageBox.Show("Failed to Add project costs. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating project costs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProjectsData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                             projectId, 
                             projectName, 
                             projectLabourCost, 
                             projectMaterialCost, 
                             projectTotalCost, 
                             clientId, 
                             orderId 
                             FROM Project";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvProjects.DataSource = dataTable;

                    dgvProjects.Columns["projectId"].HeaderText = "ID";
                    dgvProjects.Columns["projectId"].Width = 40;
                    dgvProjects.Columns["clientId"].Width = 40;
                    dgvProjects.Columns["orderId"].Width = 40;

                    dgvProjects.Columns["projectName"].HeaderText = "Name";
                    dgvProjects.Columns["projectLabourCost"].HeaderText = "Labour Cost";
                    dgvProjects.Columns["projectMaterialCost"].HeaderText = "Material Cost";
                    dgvProjects.Columns["projectTotalCost"].HeaderText = "Total Cost";
                    dgvProjects.Columns["clientId"].HeaderText = "Client ID";
                    dgvProjects.Columns["orderId"].HeaderText = "Order ID";

                    dgvProjects.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dgvProjects.Columns["projectName"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No data found in the Projects table.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtProjectName.Clear();  // Clears the Project Name text box.
            txtLabourCost.Clear();   // Clears the Labour Cost text box.
            txtMaterialCost.Clear(); // Clears the Material Cost text box.
            txtTotalCost.Clear();    // Clears the Total Cost text box.
            txtClientId.Clear();     // Clears the Client ID text box.

            cmbProjectId.SelectedIndex = -1; 

        }

        private void dgvProjects_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvProjects.Rows[e.RowIndex];

                cmbProjectId.SelectedValue = selectedRow.Cells["projectId"].Value.ToString();
                txtProjectName.Text = selectedRow.Cells["projectName"].Value.ToString();
                txtLabourCost.Text = selectedRow.Cells["projectLabourCost"].Value.ToString();
                txtMaterialCost.Text = selectedRow.Cells["projectMaterialCost"].Value.ToString();
                txtTotalCost.Text = selectedRow.Cells["projectTotalCost"].Value.ToString();
                txtClientId.Text = selectedRow.Cells["clientId"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }

        private void updpj_Click(object sender, EventArgs e)
        {
            if (cmbProjectId.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a valid Project ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbProjectId.Focus();
                return;
            }

            if (!decimal.TryParse(txtLabourCost.Text, out decimal labourCost) || labourCost < 0)
            {
                MessageBox.Show("Please enter a valid Labour Cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtLabourCost.Focus();
                return;
            }

            if (!decimal.TryParse(txtMaterialCost.Text, out decimal materialCost) || materialCost < 0)
            {
                MessageBox.Show("Please enter a valid Material Cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMaterialCost.Focus();
                return;
            }

            decimal totalCost = labourCost + materialCost;
            txtTotalCost.Text = totalCost.ToString("0.00");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Project
                             SET projectLabourCost = @labourCost,
                                 projectMaterialCost = @materialCost,
                                 projectTotalCost = @totalCost
                             WHERE projectId = @projectId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@projectId", cmbProjectId.SelectedValue.ToString());
                    command.Parameters.AddWithValue("@labourCost", labourCost);
                    command.Parameters.AddWithValue("@materialCost", materialCost);
                    command.Parameters.AddWithValue("@totalCost", totalCost);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Project costs updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputs(); // Clear the input fields after update
                        LoadProjectsData(); // Refresh the data grid view to show the updated data
                    }
                    else
                    {
                        MessageBox.Show("Failed to update project costs. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating project costs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rempj_Click(object sender, EventArgs e)
        {
            if (cmbProjectId.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a valid Project ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbProjectId.Focus();
                return;
            }

            // Display confirmation dialog
            var confirmResult = MessageBox.Show("Are you sure you want to delete the project costs?",
                                                "Confirm Deletion",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Warning);

            // Check user's choice based on the DialogResult
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = @"UPDATE Project
                                 SET projectLabourCost = NULL,
                                     projectMaterialCost = NULL,
                                     projectTotalCost = NULL
                                 WHERE projectId = @projectId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@projectId", cmbProjectId.SelectedValue.ToString());

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Project costs deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearInputs(); // Clear the input fields after deletion
                            LoadProjectsData(); // Refresh the data grid view to show the updated data
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete project costs. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting project costs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                //MessageBox.Show("Deletion canceled.", "Cancellation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }




        public void CountEmployees()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Step 1: SQL query to count the number of employees
                string countEmployeesQuery = "SELECT COUNT(*) FROM [finalPJS].[dbo].[Employee]";

                SqlCommand cmd = new SqlCommand(countEmployeesQuery, conn);

                // Step 2: Execute the query and get the count
                int employeeCount = (int)cmd.ExecuteScalar(); // ExecuteScalar returns the result of a single value query

                // Step 3: Set the count to the TextBox
                guna2TextBox3.Text = employeeCount.ToString();
             //   guna2HtmlToolTip1.SetToolTip(guna2TextBox3, employeeCount.ToString() + " employees have registered in organization");


            }
        }



        public void GetMostRecentMeetingForUser(string username)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Step 1: Get the full name of the user (FirstName + LastName)
                string getFullNameQuery = "SELECT firstName + ' ' + lastName AS fullName FROM [finalPJS].[dbo].[Employee] WHERE userName = @username";
                SqlCommand cmd = new SqlCommand(getFullNameQuery, conn);
                cmd.Parameters.AddWithValue("@username", username);
                object fullNameResult = cmd.ExecuteScalar();

                if (fullNameResult != null)
                {
                    string fullName = fullNameResult.ToString();

                    // Step 2: Get the most recent meeting where the user's full name is a participant
                    string getMostRecentMeetingQuery = @"
                SELECT TOP 1 m.[meetingId], m.[meetingTitle], m.[meetingDate], m.[meetingParticipants], m.[meetingSummary]
                FROM [finalPJS].[dbo].[Meeting] m
                WHERE CHARINDEX(@fullName, m.[meetingParticipants]) > 0
                AND m.[meetingDate] >= CAST(GETDATE() AS DATE) -- Ensures the meeting is today or in the future
                ORDER BY m.[meetingDate] DESC";

                    cmd = new SqlCommand(getMostRecentMeetingQuery, conn);
                    cmd.Parameters.AddWithValue("@fullName", fullName); // Search for the full name in meetingParticipants

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string meetingTitle = reader["meetingTitle"].ToString();
                        DateTime meetingDateTime = Convert.ToDateTime(reader["meetingDate"]);
                        string meetingDate = meetingDateTime.ToString("MM/dd/yyyy");
                        string meetingSummary = reader["meetingSummary"].ToString();

                        // Populate the TextBox with the meeting info
                        guna2TextBox6.Text = "You have a meeting on " + meetingDate;
                      //  guna2HtmlToolTip3.SetToolTip(guna2TextBox6, "Meeting Title : " + meetingTitle);


                    }
                    else
                    {
                        MessageBox.Show("No upcoming meetings");
                    }

                    reader.Close();
                }
                else
                {
                    MessageBox.Show("User not found.");
                }
            }
        }


        public void CountSuppliers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Step 1: SQL query to count the number of suppliers
                    string countSuppliersQuery = "SELECT COUNT(*) FROM [finalPJS].[dbo].[Supplier]";

                    SqlCommand cmd = new SqlCommand(countSuppliersQuery, conn);

                    // Step 2: Execute the query and get the count
                    int supplierCount = (int)cmd.ExecuteScalar(); // ExecuteScalar returns the result of a single value query

                    // Step 3: Set the count to the TextBox
                    guna2TextBox1.Text = supplierCount.ToString();
                  //  guna2HtmlToolTip3.SetToolTip(guna2TextBox1, supplierCount.ToString() + " suppliers have registered in the system");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        public void CountPastProjects()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Step 1: SQL query to count projects with past submission dates
                    string countProjectsQuery = "SELECT COUNT(*) FROM [finalPJS].[dbo].[Project] WHERE projectSubmissionDate >= CAST(GETDATE() AS DATE)";

                    SqlCommand cmd = new SqlCommand(countProjectsQuery, conn);

                    // Step 2: Execute the query and get the count
                    int pastProjectCount = (int)cmd.ExecuteScalar();

                    // Step 3: Set the count to the TextBox
                    guna2TextBox2.Text = pastProjectCount.ToString();
                 //   guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Company currently works on " + pastProjectCount.ToString() + " projects");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        public void LoadTaskCounts(string userName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Step 1: Get employeeId based on userName
                    string getEmployeeIdQuery = "SELECT employeeId FROM [finalPJS].[dbo].[Employee] WHERE userName = @userName";
                    SqlCommand cmd1 = new SqlCommand(getEmployeeIdQuery, conn);
                    cmd1.Parameters.AddWithValue("@userName", userName);
                    object employeeIdObj = cmd1.ExecuteScalar();

                    if (employeeIdObj != null)
                    {
                        string employeeId = (string)employeeIdObj;

                        // Step 2: Count tasks with Pending status
                        string pendingTaskQuery = "SELECT COUNT(*) FROM [finalPJS].[dbo].[Task] WHERE status = 'Pending' AND employeeId = @employeeId";
                        SqlCommand cmd2 = new SqlCommand(pendingTaskQuery, conn);
                        cmd2.Parameters.AddWithValue("@employeeId", employeeId);
                        int pendingTaskCount = (int)cmd2.ExecuteScalar();

                        // Step 3: Count overdue tasks with Pending status
                        string overdueTaskQuery = "SELECT COUNT(*) FROM [finalPJS].[dbo].[Task] WHERE status = 'Pending' AND deadline < CAST(GETDATE() AS DATE) AND employeeId = @employeeId";
                        SqlCommand cmd3 = new SqlCommand(overdueTaskQuery, conn);
                        cmd3.Parameters.AddWithValue("@employeeId", employeeId);
                        int overdueTaskCount = (int)cmd3.ExecuteScalar();

                        // Display results (assuming you have TextBoxes or Labels to show counts)
                        guna2TextBox5.Text = pendingTaskCount.ToString();



                        if (pendingTaskCount > 0 && overdueTaskCount > 0)
                        {
                 //           guna2HtmlToolTip6.SetToolTip(guna2TextBox5, pendingTaskCount + " tasks are pending.\n " + overdueTaskCount + " of" + pendingTaskCount + " are overdue.");

                        }
                        else if (pendingTaskCount > 0 && overdueTaskCount <= 0)
                        {
                   //         guna2HtmlToolTip6.SetToolTip(guna2TextBox5, pendingTaskCount + " tasks are pending");

                        }
                        else if (pendingTaskCount <= 0)
                        {

                     //       guna2HtmlToolTip6.SetToolTip(guna2TextBox5, "You have no tasks to complete");
                        }
                        else
                        {
                            MessageBox.Show("User not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        public void LoadLatestSalary(string userName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Step 1: Get employeeId based on userName
                    string getEmployeeIdQuery = "SELECT employeeId FROM [finalPJS].[dbo].[Employee] WHERE userName = @userName";
                    SqlCommand cmd1 = new SqlCommand(getEmployeeIdQuery, conn);
                    cmd1.Parameters.AddWithValue("@userName", userName);
                    object employeeIdObj = cmd1.ExecuteScalar();

                    if (employeeIdObj != null && employeeIdObj != DBNull.Value)
                    {
                        string employeeId = (string)employeeIdObj;

                        // Step 2: Get the latest salary's finalAmount
                        string latestSalaryQuery = @"
                    SELECT TOP 1 finalAmount 
                    FROM [finalPJS].[dbo].[SalaryPayments] 
                    WHERE employeeId = @employeeId 
                    ORDER BY salaryId DESC";

                        SqlCommand cmd2 = new SqlCommand(latestSalaryQuery, conn);
                        cmd2.Parameters.AddWithValue("@employeeId", employeeId);
                        object finalAmountObj = cmd2.ExecuteScalar();

                        if (finalAmountObj != null && finalAmountObj != DBNull.Value)
                        {
                            decimal finalAmount = Convert.ToDecimal(finalAmountObj);
                            guna2TextBox2.Text = finalAmount.ToString("C"); // Format as currency
                       //     guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is Rs" + finalAmount.ToString() + ".00");

                        }
                        else
                        {
                            guna2TextBox2.Text = "Rs 0.00";
                         //   guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is not calculated yet");

                        }
                    }
                    else
                    {
                        MessageBox.Show("User not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void clrpj_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

