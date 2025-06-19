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
using static System.Net.WebRequestMethods;

namespace FinalProject
{
    public partial class Administartion : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;


        public Administartion(string userName)
        {
            InitializeComponent();
            this.Paint += RoundedForm_Paint;

            currentUsername = userName;
            string loggedinUsername;

            employeeId = GetEmployeeId(currentUsername);

            if (!string.IsNullOrEmpty(employeeId))
            {
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


        private void guna2TextBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel12_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel24_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
            AdministrationP mainForm = new AdministrationP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel16_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel13_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

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

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;
        }

        private void guna2ImageButton3_Click_1(object sender, EventArgs e)
        {
            AdministrationP mainForm = new AdministrationP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void Administartion_Load(object sender, EventArgs e)
        {
            TaskData();
            LoadDataIntoDGVTasks();
            PopulateAssigneeComboBox();
            LoadData();
            LoadEmployeeIds();
            LoadAttendanceRecordsForDate(DateTime.Now);
            dtpAttendanceDate.Value = DateTime.Now;
            dtpTaskDate.Value = DateTime.Now;
            cmbEmployeeId.SelectedItem = null; // Clear employee selection (set to null)

            LoadFeedbackIntoDGV(employeeId);
            FeedbackData();
            LoadEmployeeData();



            CountEmployees();
            GetMostRecentMeetingForUser(currentUsername);
            CountSuppliers();
          //  CountPastProjects();
          LoadLatestSalary(currentUsername);
            LoadTaskCounts(currentUsername);



        }

        private void guna2TextBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void TaskData()
        {

        }

        private string GenerateTaskId()
        {
            string newId = "T001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 taskId FROM Task ORDER BY CAST(SUBSTRING(taskId, 2, LEN(taskId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();

                            int numericPart = int.Parse(lastId.Substring(1));
                            newId = "T" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Task ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }


        private void LoadDataIntoDGVTasks()
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
                    Task";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvTask.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No tasks found in the Task table.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateAssigneeComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT 
                                employeeId, 
                                (firstName + ' ' + lastName) AS EmployeeName 
                             FROM 
                                Employee";

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    cmbAssignee.DataSource = dt;
                    cmbAssignee.DisplayMember = "EmployeeName";
                    cmbAssignee.ValueMember = "employeeId";
                    cmbAssignee.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error populating assignee list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ClearInputs()
        {
            txtTaskName.Clear();
            cmbAssignee.SelectedIndex = -1;   // Reset Assignee ComboBox (no selection)
            cmbStatus.SelectedIndex = -1;     // Reset Status ComboBox (no selection)
            cmbPriority.SelectedIndex = -1;   // Reset Priority ComboBox (no selection)
            dtpTaskDate.Value = DateTime.Now;
        }

        private void addbtntsk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTaskName.Text.Trim()))
            {
                MessageBox.Show("Task Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTaskName.Focus();
                return;
            }

            if (cmbAssignee.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an Assignee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbAssignee.Focus();
                return;
            }

            if (cmbPriority.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Priority.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbPriority.Focus();
                return;
            }

            if (cmbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbStatus.Focus();
                return;
            }

            if (dtpTaskDate.Value.Date <= DateTime.Now.Date)
            {
                MessageBox.Show("The task date must be in the future. Please select a date higher than today's date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtpTaskDate.Focus();
                return;
            }

            try
            {
                string taskId = GenerateTaskId();
                string employeeId = cmbAssignee.SelectedValue.ToString(); // Get the selected employee's ID
                string taskName = txtTaskName.Text.Trim();
                string status = cmbStatus.SelectedItem.ToString();
                string priority = cmbPriority.SelectedItem.ToString();
                string deadline = dtpTaskDate.Value.ToString("yyyy-MM-dd");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                    INSERT INTO Task (taskId, taskName, deadline, status, priority, assignee) 
                    VALUES (@taskId, @taskName, @deadline, @status, @priority, @employeeId)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@taskId", taskId);
                        command.Parameters.AddWithValue("@taskName", taskName);
                        command.Parameters.AddWithValue("@deadline", deadline);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@priority", priority);
                        command.Parameters.AddWithValue("@employeeId", employeeId);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show($"Task added successfully! Task ID: {taskId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDGVTasks(); // Refresh the task list
                            ClearInputs();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add task. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updbtntsk_Click(object sender, EventArgs e)
        {
            if (dgvTask.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a task to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string taskId = dgvTask.SelectedRows[0].Cells["taskId"].Value.ToString();

            if (string.IsNullOrEmpty(txtTaskName.Text.Trim()))
            {
                MessageBox.Show("Task Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTaskName.Focus();
                return;
            }

            if (cmbAssignee.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an Assignee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbAssignee.Focus();
                return;
            }

            if (cmbPriority.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Priority.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbPriority.Focus();
                return;
            }

            if (cmbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbStatus.Focus();
                return;
            }

            if (dtpTaskDate.Value.Date <= DateTime.Now.Date)
            {
                MessageBox.Show("Please select the current date", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtpTaskDate.Focus();
                return;
            }

            try
            {
                string employeeId = cmbAssignee.SelectedValue.ToString();  // Get the selected employee's ID
                string taskName = txtTaskName.Text.Trim();
                string status = cmbStatus.SelectedItem.ToString();
                string priority = cmbPriority.SelectedItem.ToString();
                string deadline = dtpTaskDate.Value.ToString("yyyy-MM-dd");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
            UPDATE Task
            SET taskName = @taskName, deadline = @deadline, status = @status, priority = @priority, assignee = @employeeId
            WHERE taskId = @taskId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@taskId", taskId);
                        command.Parameters.AddWithValue("@taskName", taskName);
                        command.Parameters.AddWithValue("@deadline", deadline);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@priority", priority);
                        command.Parameters.AddWithValue("@employeeId", employeeId);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show($"Task updated successfully! Task ID: {taskId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDGVTasks();
                            ClearInputs();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update task. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void delbtntsk_Click(object sender, EventArgs e)
        {
            if (dgvTask.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a task to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string taskId = dgvTask.SelectedRows[0].Cells["taskId"].Value.ToString();

            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete Task ID: {taskId}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Task WHERE taskId = @taskId";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@taskId", taskId);

                            connection.Open();
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show($"Task deleted successfully! Task ID: {taskId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadDataIntoDGVTasks();
                                ClearInputs();
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete task. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvTask_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)

            {
                DataGridViewRow selectedRow = dgvTask.Rows[e.RowIndex];

                txtTaskName.Text = selectedRow.Cells["taskName"].Value.ToString();
                string assigneeId = selectedRow.Cells["assignee"].Value.ToString();

                cmbAssignee.SelectedValue = assigneeId; dtpTaskDate.Value = Convert.ToDateTime(selectedRow.Cells["deadline"].Value);
                cmbStatus.SelectedItem = selectedRow.Cells["status"].Value.ToString();
                cmbPriority.SelectedItem = selectedRow.Cells["priority"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }

        //
        private void LoadEmployeeIds()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT employeeId FROM Employee";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);


                    cmbEmployeeId.DisplayMember = "employeeId";
                    cmbEmployeeId.ValueMember = "employeeId";
                    cmbEmployeeId.DataSource = dataTable;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employee IDs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEmployeeName(string employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT firstName + ' ' + lastName AS fullName FROM Employee WHERE employeeId = @employeeId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        txtEmployeeName.Text = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employee name: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbEmployeeId_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbEmployeeId.SelectedValue != null && cmbEmployeeId.SelectedValue.ToString() != "0")
            {
                string selectedEmployeeId = cmbEmployeeId.SelectedValue.ToString();
                LoadEmployeeName(selectedEmployeeId);
            }
            else
            {
                txtEmployeeName.Clear();
            }
        }

        private void addbtnatt_Click(object sender, EventArgs e)
        {
            string employeeId = cmbEmployeeId.SelectedValue.ToString();
            string attendanceStatus = cmbAttendanceStatus.SelectedItem?.ToString().Trim();
            DateTime attendanceDate = dtpAttendanceDate.Value.Date;


            if (attendanceDate.Date != DateTime.Now.Date)
            {
                MessageBox.Show("Attendance date in be in the future or past.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpAttendanceDate.Focus(); // Focus on Attendance Date picker
                return;
            }

            if (string.IsNullOrEmpty(employeeId) || employeeId == "0")
            {
                MessageBox.Show("Please select an employee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEmployeeId.Focus(); // Focus on Employee dropdown
                return;
            }


            if (string.IsNullOrEmpty(attendanceStatus))
            {
                MessageBox.Show("Please select an attendance status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbAttendanceStatus.Focus(); // Focus on Attendance Status ComboBox
                return;
            }

            if (AttendanceExists(employeeId, attendanceDate))
            {
                MessageBox.Show("Attendance for this employee has already been marked for today.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO EmployeeAttendance (attendanceDate, attendanceStatus, employeeId) VALUES (@attendanceDate, @attendanceStatus, @employeeId)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@attendanceDate", attendanceDate);
                    command.Parameters.AddWithValue("@attendanceStatus", attendanceStatus);
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    connection.Open();
                    command.ExecuteNonQuery();

                    MessageBox.Show("Attendance record added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAttendanceRecordsForDate(attendanceDate);
                    ClearForm();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding attendance record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AttendanceExists(string employeeId, DateTime attendanceDate)
        {
            bool exists = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM EmployeeAttendance WHERE employeeId = @employeeId AND attendanceDate = @attendanceDate";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@attendanceDate", attendanceDate);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        exists = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking attendance: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return exists;
        }

        private void dtpAttendanceDate_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dtpAttendanceDate.Value.Date;

            LoadAttendanceRecordsForDate(selectedDate);
        }

        private void LoadAttendanceRecordsForDate(DateTime attendanceDate)
        {
            try
            {
                if (dgvAttendance.Columns.Count == 0)
                {
                    dgvAttendance.Columns.Add("employeeId", "Employee ID");
                    dgvAttendance.Columns.Add("attendanceStatus", "Attendance Status");
                    dgvAttendance.Columns.Add("attendanceDate", "Attendance Date");  // Add column for Date
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT employeeId, attendanceStatus, attendanceDate FROM EmployeeAttendance WHERE attendanceDate = @attendanceDate";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@attendanceDate", attendanceDate.Date);  // Ensure only the date part is considered

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    dgvAttendance.Rows.Clear();

                    while (reader.Read())
                    {
                        string employeeId = reader["employeeId"].ToString();
                        string attendanceStatus = reader["attendanceStatus"].ToString();
                        DateTime attendanceDateFromDb = Convert.ToDateTime(reader["attendanceDate"]);  // Convert to DateTime

                        dgvAttendance.Rows.Add(employeeId, attendanceStatus, attendanceDateFromDb.ToString("yyyy-MM-dd")); // Add formatted date to the row
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attendance records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clrbtnatt_Click(object sender, EventArgs e)
        {
            ClearForm();
        }
        private void ClearForm()
        {
            cmbEmployeeId.SelectedItem = null; // Clear employee selection (set to null)
            cmbAttendanceStatus.SelectedItem = null;
            dtpAttendanceDate.Value = DateTime.Now; // Set current date
            txtEmployeeName.Clear(); // Clear employee name (if you have this textbox)
        }

        private void updbtnatt_Click(object sender, EventArgs e)
        {
            string employeeId = cmbEmployeeId.SelectedValue.ToString();
            string attendanceStatus = cmbAttendanceStatus.SelectedItem?.ToString().Trim();
            DateTime attendanceDate = dtpAttendanceDate.Value;

            if (string.IsNullOrEmpty(employeeId) || employeeId == "0")
            {
                MessageBox.Show("Please select an employee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEmployeeId.Focus();
                return;
            }

            if (string.IsNullOrEmpty(attendanceStatus))
            {
                MessageBox.Show("Please select an attendance status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbAttendanceStatus.Focus();
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE EmployeeAttendance SET attendanceStatus = @attendanceStatus WHERE employeeId = @employeeId AND attendanceDate = @attendanceDate";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@attendanceStatus", attendanceStatus);
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@attendanceDate", attendanceDate);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Attendance record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAttendanceRecordsForDate(attendanceDate); // Refresh data grid
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("No record found to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating attendance record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void delbtnatt_Click(object sender, EventArgs e)
        {
            string employeeId = cmbEmployeeId.SelectedValue.ToString();
            DateTime attendanceDate = dtpAttendanceDate.Value;

            if (string.IsNullOrEmpty(employeeId) || employeeId == "0")
            {
                MessageBox.Show("Please select an employee.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEmployeeId.Focus();
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM EmployeeAttendance WHERE employeeId = @employeeId AND attendanceDate = @attendanceDate";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@employeeId", employeeId);
                    command.Parameters.AddWithValue("@attendanceDate", attendanceDate);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Attendance record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAttendanceRecordsForDate(attendanceDate); // Refresh data grid
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("No record found to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting attendance record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvAttendance_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvAttendance.Rows[e.RowIndex];

                string employeeId = row.Cells["employeeId"].Value.ToString();
                DateTime attendanceDate = Convert.ToDateTime(row.Cells["attendanceDate"].Value);
                string attendanceStatus = row.Cells["attendanceStatus"].Value.ToString();

                cmbEmployeeId.SelectedValue = employeeId;
                dtpAttendanceDate.Value = attendanceDate;

                cmbAttendanceStatus.SelectedItem = attendanceStatus;

            }
        }

        private void cmbAttendanceStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {

        }

        private string GenerateFeedbackId()
        {
            string newId = "F001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Query to get the numerically highest feedbackId
                    string query = "SELECT TOP 1 feedbackId FROM Feedback ORDER BY CAST(SUBSTRING(feedbackId, 2, LEN(feedbackId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();

                            // Extract the numeric part and increment it
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

        private void cmbAssignee_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel44_Click(object sender, EventArgs e)
        {

        }

        private void txtEmpId_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {

            {
                try
                {
                    // Get the Project ID from a TextBox
                    string projectId = projectid.Text.Trim(); // Replace with your TextBox control name

                    // Validate input
                    if (string.IsNullOrEmpty(projectId))
                    {
                        MessageBox.Show("Please enter a Project ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // SQL query to fetch project details by ID
                    string query = "SELECT * FROM Project WHERE projectId = @projectId";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@projectId", projectId);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // Load data into controls
                            projectid.Text = reader["projectId"].ToString();
                            PROJECTBOX.Text = reader["projectName"].ToString();
                            submissiondate.Text = Convert.ToDateTime(reader["projectSubmissionDate"]).ToString("yyyy-MM-dd");

                            labourcost.Text = reader["projectLabourCost"].ToString();
                            materialcost.Text = reader["projectMaterialCost"].ToString();
                            totalcost.Text = reader["projectTotalCost"].ToString();
                            CLIENTBOX.Text = reader["clientId"].ToString();
                            ORDERBOX.Text = reader["orderId"].ToString();

                            MessageBox.Show("Project details loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No project found with the given Project ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        reader.Close();
                    }
                }
                catch (SqlException sqlEx)
                {
                    // Handle SQL-specific errors
                    MessageBox.Show($"SQL Error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    // Handle general errors
                    MessageBox.Show($"Error: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ClearInputs1()
        {
            // Clear all text boxes
            PROJECTBOX.Clear();
            projectid.Clear();
            labourcost.Clear();
            materialcost.Clear();
            totalcost.Clear();
            CLIENTBOX.Clear();
            ORDERBOX.Clear();

            submissiondate.Clear(); // Set to no selection
        }

        private void LoadData()
        {
            // Define your connection string

            // SQL query to fetch the data
            string query = "SELECT TOP (1000) [projectId], [projectName], [projectSubmissionDate], " +
                           "[projectLabourCost], [projectMaterialCost], [projectTotalCost], " +
                           "[clientId], [orderId] FROM [finalPJS].[dbo].[Project]";

            // Create a connection to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Create a data adapter and fill the DataTable
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Bind the DataTable to the DataGridView
                dgv1.DataSource = dt;
            }
        }
        private void dgv1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is not on a header row
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgv1.Rows[e.RowIndex];

                // Assuming you have TextBoxes, ComboBox, or other controls to display the data
                // Populating project details in the respective controls

                projectid.Text = selectedRow.Cells["projectId"].Value.ToString();
                PROJECTBOX.Text = selectedRow.Cells["projectName"].Value.ToString();
                submissiondate.Text = Convert.ToDateTime(selectedRow.Cells["projectSubmissionDate"].Value).ToString("yyyy-MM-dd");

                labourcost.Text = selectedRow.Cells["projectLabourCost"].Value.ToString();
                materialcost.Text = selectedRow.Cells["projectMaterialCost"].Value.ToString();
                materialcost.Text = selectedRow.Cells["projectTotalCost"].Value.ToString();
                CLIENTBOX.Text = selectedRow.Cells["clientId"].Value.ToString();
                ORDERBOX.Text = selectedRow.Cells["orderId"].Value.ToString();
                totalcost.Text = selectedRow.Cells["projectTotalCost"].Value.ToString();

            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }

        private void projectid_TextChanged(object sender, EventArgs e)
        {

        }

        private void PROJECTBOX_TextChanged(object sender, EventArgs e)
        {

        }

        private void materialcost_TextChanged(object sender, EventArgs e)
        {

        }

        private void totalcost_TextChanged(object sender, EventArgs e)
        {

        }

        private void submissiondate_TextChanged(object sender, EventArgs e)
        {

        }

        private void ORDERBOX_TextChanged(object sender, EventArgs e)
        {

        }

        private void CLIENTBOX_TextChanged(object sender, EventArgs e)
        {

        }

        private void labourcost_TextChanged(object sender, EventArgs e)
        {

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
                guna2TextBox4.Text = employeeCount.ToString();
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
                        //   guna2HtmlToolTip3.SetToolTip(guna2TextBox6, "Meeting Title : " + meetingTitle);


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
                                                                            //  guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is Rs" + finalAmount.ToString() + ".00");

                        }
                        else
                        {
                            guna2TextBox2.Text = "Rs 0.00";
                            // guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is not calculated yet");

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
                        guna2TextBox10.Text = pendingTaskCount.ToString();



                        if (pendingTaskCount > 0 && overdueTaskCount > 0)
                        {
                            //guna2HtmlToolTip6.SetToolTip(guna2TextBox5, pendingTaskCount + " tasks are pending.\n " + overdueTaskCount + " of" + pendingTaskCount + " are overdue.");

                        }
                        else if (pendingTaskCount > 0 && overdueTaskCount <= 0)
                        {
                            //guna2HtmlToolTip6.SetToolTip(guna2TextBox5, pendingTaskCount + " tasks are pending");

                        }
                        else if (pendingTaskCount <= 0)
                        {

                            // guna2HtmlToolTip6.SetToolTip(guna2TextBox5, "You have no tasks to complete");
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


        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TileButton5_Click(object sender, EventArgs e)
        {

        }
    }
}

