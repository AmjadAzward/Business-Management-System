using Guna.UI2.WinForms;
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
    public partial class Designer : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;
        public Designer(string userName)
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

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
            DesignerP mainForm = new DesignerP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click_1(object sender, EventArgs e)
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

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;

        }

        private void guna2ImageButton3_Click_1(object sender, EventArgs e)
        {
            DesignerP mainForm = new DesignerP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void Designer_Load(object sender, EventArgs e)
        {
            LoadFeedbackIntoDGV(employeeId);
            LoadEmployeeData();
            FeedbackDatas();

            LoadEmployeeTasks(employeeId);

            LoadProjects();
            LoadDesigns();
            cmbProjectId.SelectedIndex = -1;

            CountEmployees();
            GetMostRecentMeetingForUser(currentUsername);
            CountSuppliers();
            //CountPastProjects();
            LoadTaskCounts(currentUsername);
            LoadLatestSalary(currentUsername);


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

        //
        //
        //
        

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

        private void accsend_Click(object sender, EventArgs e)
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

                            int numericPart = int.Parse(lastId.Substring(1)); // Assumes format "F<number>"
                            newId = "F" + (numericPart + 1).ToString("D3");   // Zero-padded to 3 digits
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
                WHERE employeeId = @employeeId"; // Filter by the employee ID

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.Add("@employeeId", SqlDbType.NVarChar).Value = employeeId;

                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    Feetbackdgv.DataSource = dataTable;

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



        private void FeedbackDatas()
        {
            Feetbackdgv.Columns["feedbackId"].HeaderText = "ID";
            Feetbackdgv.Columns["feedback"].HeaderText = "Feedback";
            Feetbackdgv.Columns["feedbackType"].HeaderText = "Type";
            Feetbackdgv.Columns["response"].HeaderText = "Response";
            Feetbackdgv.Columns["employeeId"].HeaderText = "Employee ID";


            Feetbackdgv.Columns["employeeId"].DisplayIndex = 1;

            Feetbackdgv.Columns["feedback"].Width = 150;
            Feetbackdgv.Columns["feedbackId"].Width = 50;
            Feetbackdgv.Columns["employeeId"].Width = 70;
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

        private void Feetbackdgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = Feetbackdgv.Rows[e.RowIndex];

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
        private void LoadProjects()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT projectId, clientId FROM Project"; // Adjust the query according to your table
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    cmbProjectId.DataSource = dataTable;
                    cmbProjectId.DisplayMember = "projectId"; // Display Project ID
                    cmbProjectId.ValueMember = "projectId"; // ValueMember is for selecting Project
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading projects: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbProjectId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProjectId.SelectedValue != null)
            {
                string selectedProjectId = cmbProjectId.SelectedValue.ToString();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "SELECT clientId FROM Project WHERE projectId = @projectId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@projectId", selectedProjectId);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            txtClientId.Text = reader["clientId"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading client ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*|PDF Files (*.pdf)|*.pdf|Image Files (*.jpg;*.png)|*.jpg;*.png", 
                Title = "Select Design File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtDesignFile.Text = openFileDialog.FileName; 
            }
        }

        private void LoadDesigns()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT designId, designName, description, designFile, desginedDate, review , clientId, projectId FROM Designs";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvDesigns.DataSource = dataTable;

                    dgvDesigns.Columns["designId"].HeaderText = "ID";
                    dgvDesigns.Columns["designName"].HeaderText = "Name";
                    dgvDesigns.Columns["description"].HeaderText = "Description";
                    dgvDesigns.Columns["designFile"].HeaderText = "File";
                    dgvDesigns.Columns["desginedDate"].HeaderText = "Designed Date";
                    dgvDesigns.Columns["review"].HeaderText = "Review";
                    dgvDesigns.Columns["clientId"].HeaderText = "Client ID";
                    dgvDesigns.Columns["projectId"].HeaderText = "Project ID";

                    dgvDesigns.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dgvDesigns.Columns["description"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvDesigns.Columns["designFile"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvDesigns.Columns["designName"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    dgvDesigns.Columns["desginedDate"].Width = 70;
                    dgvDesigns.Columns["clientId"].Width = 35;
                    dgvDesigns.Columns["projectId"].Width = 35;
                    dgvDesigns.Columns["designId"].Width = 35;
                    dgvDesigns.Columns["designFile"].Width = 70;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading designs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtDesignName.Clear();
            txtDescription.Clear();
            txtDesignFile.Clear();
            txtClientId.Clear();
            cmbProjectId.SelectedIndex = -1; // Clear ComboBox selection
        }

        private string GenerateDesignId()
        {
            string newId = "D001"; // Default design ID

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 designId FROM Designs ORDER BY designId DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();
                            int numericPart = int.Parse(lastId.Substring(1)); // Extract the number part from the last ID
                            newId = "D" + (numericPart + 1).ToString("D3"); // Increment the numeric part and return the new ID
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Design ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }


        private void addbtndes_Click(object sender, EventArgs e)
        {
            string designId = GenerateDesignId();
            string designName = txtDesignName.Text.Trim();
            string description = txtDescription.Text.Trim();
            string designFile = txtDesignFile.Text.Trim();
            string clientId = txtClientId.Text.Trim();
            string projectId = cmbProjectId.SelectedValue.ToString();
            string desginedDate = guna2DateTimePicker2.Value.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(designName))
            {
                MessageBox.Show("Design Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDesignName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(desginedDate)) // Changed from uploaddate to desginedDate
            {
                MessageBox.Show("Upload Date is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2DateTimePicker2.Focus(); // Focus on the date picker
                return;
            }

            if (string.IsNullOrEmpty(designFile))
            {
                MessageBox.Show("Design File is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDesignFile.Focus();
                return;
            }

            if (string.IsNullOrEmpty(clientId))
            {
                MessageBox.Show("Client ID is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClientId.Focus();
                return;
            }

            if (cmbProjectId.SelectedIndex == -1)
            {
                MessageBox.Show("Project ID must be selected.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbProjectId.Focus();
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Designs (designId, designName, description, designFile, clientId, projectId, desginedDate) " +
                                   "VALUES (@designId, @designName, @description, @designFile, @clientId, @projectId, @desginedDate)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@designId", designId);
                    command.Parameters.AddWithValue("@designName", designName);
                    command.Parameters.AddWithValue("@description", description);
                    command.Parameters.AddWithValue("@designFile", designFile);
                    command.Parameters.AddWithValue("@clientId", clientId);
                    command.Parameters.AddWithValue("@projectId", projectId);
                    command.Parameters.AddWithValue("@desginedDate", desginedDate);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show("Design added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadDesigns();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding design: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button9_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {

        }

        private void clrbtndes_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void dgvDesigns_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDesigns.Rows[e.RowIndex];
                string designId = row.Cells["designId"].Value.ToString();
                string designName = row.Cells["designName"].Value.ToString();
                string designFile = row.Cells["designFile"].Value.ToString();
                string clientId = row.Cells["clientId"].Value.ToString();
                string projectId = row.Cells["projectId"].Value.ToString();
                string desginedDate = row.Cells["desginedDate"].Value.ToString();
                string description = row.Cells["description"].Value.ToString();


                cmbProjectId.Text = projectId;
                txtClientId.Text = clientId;
                txtDesignName.Text = designName;
                txtDesignFile.Text = designFile;
                guna2DateTimePicker2.Text = desginedDate;
                txtDescription.Text = description;


            }
        }
        


        private void txtClientId_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void guna2DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void dgvDesigns_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDesigns.Columns[e.ColumnIndex].Name == "designFile")
            {
                string filePath = dgvDesigns.Rows[e.RowIndex].Cells["designFile"].Value.ToString();

                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        System.Diagnostics.Process.Start(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No file path available to open.", "No File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void delbtndes_Click(object sender, EventArgs e)
        {
            if (dgvDesigns.SelectedRows.Count > 0)
            {
                string designId = dgvDesigns.SelectedRows[0].Cells["designId"].Value.ToString();

                DialogResult result = MessageBox.Show($"Are you sure you want to delete Design ID: {designId}?",
                                                      "Delete Confirmation",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string query = "DELETE FROM Designs WHERE designId = @designId";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@designId", designId);

                            connection.Open();
                            command.ExecuteNonQuery();

                            MessageBox.Show("Design deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDesigns();
                            ClearForm();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting design: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void updbtndes_Click(object sender, EventArgs e)
        {
            if (dgvDesigns.SelectedRows.Count > 0)
            {
                string designId = dgvDesigns.SelectedRows[0].Cells["designId"].Value.ToString();
                string designName = txtDesignName.Text.Trim();
                string description = txtDescription.Text.Trim();
                string designFile = txtDesignFile.Text.Trim();
                string clientId = txtClientId.Text.Trim();
                string projectId = cmbProjectId.SelectedValue.ToString();
                string desginedDate = guna2DateTimePicker2.Value.ToString("yyyy-MM-dd");

                if (string.IsNullOrEmpty(designName) || string.IsNullOrEmpty(designFile) || string.IsNullOrEmpty(clientId))
                {
                    MessageBox.Show("All fields must be filled.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Designs SET designName = @designName, description = @description, designFile = @designFile, " +
                                       "clientId = @clientId, projectId = @projectId, desginedDate = @desginedDate WHERE designId = @designId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@designId", designId);
                        command.Parameters.AddWithValue("@designName", designName);
                        command.Parameters.AddWithValue("@description", description);
                        command.Parameters.AddWithValue("@designFile", designFile);
                        command.Parameters.AddWithValue("@clientId", clientId);
                        command.Parameters.AddWithValue("@projectId", projectId);
                        command.Parameters.AddWithValue("@desginedDate", desginedDate);

                        connection.Open();
                        command.ExecuteNonQuery();

                        MessageBox.Show("Design updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDesigns();
                        ClearForm();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating design: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            //    guna2HtmlToolTip1.SetToolTip(guna2TextBox3, employeeCount.ToString() + " employees have registered in organization");


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
                    //    guna2HtmlToolTip3.SetToolTip(guna2TextBox6, "Meeting Title : " + meetingTitle);


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
                   // guna2HtmlToolTip3.SetToolTip(guna2TextBox1, supplierCount.ToString() + " suppliers have registered in the system");
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
                        guna2TextBox4.Text = pendingTaskCount.ToString();



                        if (pendingTaskCount > 0 && overdueTaskCount > 0)
                        {
                           // guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending.\n " + overdueTaskCount + " of" + pendingTaskCount + " are overdue.");

                        }
                        else if (pendingTaskCount > 0 && overdueTaskCount <= 0)
                        {
                            //guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending");

                        }
                        else if (pendingTaskCount <= 0)
                        {

                          //  guna2HtmlToolTip6.SetToolTip(guna2TextBox4, "You have no tasks to complete");
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
                      //      guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is Rs" + finalAmount.ToString() + ".00");

                        }
                        else
                        {
                            guna2TextBox2.Text = "Rs 0.00";
                     //       guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is not calculated yet");

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



        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
