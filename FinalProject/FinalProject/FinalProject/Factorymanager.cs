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
    public partial class Factorymanager : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;
        public Factorymanager(string userName)
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

        
        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
            
        }

        private void guna2TileButton6_Click(object sender, EventArgs e)
        {
            InventoryAvailability mainForm = new InventoryAvailability(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton7_Click(object sender, EventArgs e)
        {
            InventoryReq mainForm = new InventoryReq(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;

        }

        private void guna2Button7_Click(object sender, EventArgs e)
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
            FactoryManagerPr mainForm = new FactoryManagerPr(currentUsername);
            mainForm.Show();
            this.Hide();

        }

        private void Factorymanager_Load(object sender, EventArgs e)
        {
            LoadEmployeeTasks(employeeId);

            LoadFeedbackIntoDGV(employeeId);
            FeedbackData();
            LoadEmployeeData();

            LoadRawMaterialsData();
            LoadItemNames();
            cmbOrderId.SelectedIndex = -1;
            cmbItemName.SelectedIndex = -1;
            // LoadOrderIdsForItem(itemName);
            RawmaterialsData();

            LoadToolsData();
            LoadItemNamesForTools();
            ToolsData();
            cmbItemNames.SelectedIndex = -1;
            cmbOrderIds.SelectedIndex = -1;
            cmbToolStatus.SelectedIndex = -1;
            dateTimePickerLastMaintenance.Value = DateTime.Now;





            CountEmployees();
            GetMostRecentMeetingForUser(currentUsername);
            CountSuppliers();
            //CountPastProjects();
            LoadTaskCounts(currentUsername);
            LoadLatestSalary(currentUsername);





        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {

        }

        //
        //
        //


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

        private void guna2Button10_Click(object sender, EventArgs e)
        {

        }


        //
        //
        //
        //raw materials 

        private void RawmaterialsData()
        {
            dgvRawmaterials.Columns["rawmaterialId"].HeaderText = "ID";
            dgvRawmaterials.Columns["rawmaterialName"].HeaderText = "Name";
            dgvRawmaterials.Columns["quantityNeeded"].HeaderText = "Quantity";
            dgvRawmaterials.Columns["unitPrice"].HeaderText = "Unit Price";
            dgvRawmaterials.Columns["orderId"].HeaderText = "Order ID";
            dgvRawmaterials.Columns["itemId"].HeaderText = "Item ID";
        }

        private string GenerateRawMaterialId()
        {
            string newId = "RM001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 rawmaterialId FROM Rawmaterials ORDER BY CAST(SUBSTRING(rawmaterialId, 3, LEN(rawmaterialId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();
                            int numericPart = int.Parse(lastId.Substring(2));
                            newId = "RM" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Raw Material ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }

        private void LoadRawMaterialsData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Rawmaterials";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvRawmaterials.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                      //  MessageBox.Show("No data found in the Raw Materials table.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadItemNames()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT itemName, itemId FROM Items";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    cmbItemName.DataSource = dataTable;
                    cmbItemName.DisplayMember = "itemName";
                    cmbItemName.ValueMember = "itemId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading item names: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadOrderIdsForItem(string itemName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(itemName))
                {
                    MessageBox.Show("Please select a valid Item Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                    SELECT DISTINCT o.orderId
                    FROM Orders o
                    INNER JOIN Items i ON o.orderId = i.orderId
                    WHERE i.itemName = @itemName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@itemName", itemName.Trim());

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        cmbOrderId.DataSource = dataTable;
                        cmbOrderId.DisplayMember = "orderId";
                        cmbOrderId.ValueMember = "orderId";
                    }
                    else
                    {
                        cmbOrderId.DataSource = null;
                        cmbOrderId.Items.Clear();
                        MessageBox.Show($"No Order IDs found for the selected Item Name: {itemName}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Order IDs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void cmbItemName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbItemName.SelectedItem != null)
            {
                string selectedItemName = (cmbItemName.SelectedItem as DataRowView)?["itemName"].ToString();
                if (!string.IsNullOrEmpty(selectedItemName))
                {
                    LoadOrderIdsForItem(selectedItemName);
                }
            }
        }

        private void addbtnraw_Click(object sender, EventArgs e)
        {
            if (cmbItemName.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an Item Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbItemName.Focus();
                return;
            }

            if (cmbOrderId.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an Order ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbOrderId.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRawMaterialName.Text))
            {
                MessageBox.Show("Material Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtRawMaterialName.Focus();
                return;
            }

            if (numericUpDownQuantityNeeded.Value <= 0)
            {
                MessageBox.Show("Quantity Needed must be greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDownQuantityNeeded.Focus();
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Unit Price must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUnitPrice.Focus();
                return;
            }

            try
            {
                string rawMaterialId = GenerateRawMaterialId();
                string orderId = cmbOrderId.SelectedValue.ToString(); // Corrected to use SelectedValue instead of SelectedItem.ToString()
                string itemId = cmbItemName.SelectedValue.ToString(); // Corrected to use SelectedValue instead of SelectedItem.ToString()

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Rawmaterials (rawmaterialId, rawmaterialName, quantityNeeded, unitPrice, orderId, itemId) " +
                                    "VALUES (@rawmaterialId, @rawmaterialName, @quantityNeeded, @unitPrice, @orderId, @itemId)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@rawmaterialId", rawMaterialId);
                    command.Parameters.AddWithValue("@rawmaterialName", txtRawMaterialName.Text.Trim());
                    command.Parameters.AddWithValue("@quantityNeeded", numericUpDownQuantityNeeded.Value);
                    command.Parameters.AddWithValue("@unitPrice", unitPrice);
                    command.Parameters.AddWithValue("@orderId", orderId); // Corrected to pass the correct value
                    command.Parameters.AddWithValue("@itemId", itemId); // Corrected to pass the correct value

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Raw Material added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRawMaterialsData();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add Raw Material. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void updbtnraw_Click(object sender, EventArgs e)
        {
            if (dgvRawmaterials.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a raw material from the list to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedRawMaterialId = dgvRawmaterials.SelectedRows[0].Cells["rawmaterialId"].Value.ToString();

            if (cmbOrderId.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an Order ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbOrderId.Focus();
                return;
            }

            if (cmbItemName.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an Item Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbItemName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRawMaterialName.Text))
            {
                MessageBox.Show("Material Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtRawMaterialName.Focus();
                return;
            }

            if (numericUpDownQuantityNeeded.Value <= 0)
            {
                MessageBox.Show("Quantity Needed must be greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDownQuantityNeeded.Focus();
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out decimal unitPrice) || unitPrice <= 0)
            {
                MessageBox.Show("Unit Price must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUnitPrice.Focus();
                return;
            }

            try
            {
                string orderId = cmbOrderId.SelectedValue.ToString(); // Corrected
                string itemId = cmbItemName.SelectedValue.ToString(); // Corrected

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Rawmaterials SET rawmaterialName = @rawmaterialName, " +
                                   "quantityNeeded = @quantityNeeded, unitPrice = @unitPrice, " +
                                   "orderId = @orderId, itemId = @itemId WHERE rawmaterialId = @rawmaterialId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@rawmaterialId", selectedRawMaterialId);
                    command.Parameters.AddWithValue("@rawmaterialName", txtRawMaterialName.Text.Trim());
                    command.Parameters.AddWithValue("@quantityNeeded", numericUpDownQuantityNeeded.Value);
                    command.Parameters.AddWithValue("@unitPrice", unitPrice);
                    command.Parameters.AddWithValue("@orderId", orderId); // Corrected to use SelectedValue
                    command.Parameters.AddWithValue("@itemId", itemId); // Corrected to use SelectedValue

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Raw Material updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRawMaterialsData();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update Raw Material. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database Error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private void remobtnraw_Click(object sender, EventArgs e)
        {
            if (dgvRawmaterials.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a raw material from the list to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedRawMaterialId = dgvRawmaterials.SelectedRows[0].Cells["rawmaterialId"].Value.ToString();

            DialogResult result = MessageBox.Show("Are you sure you want to delete this raw material?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Rawmaterials WHERE rawmaterialId = @rawmaterialId";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@rawmaterialId", selectedRawMaterialId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Raw Material deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadRawMaterialsData();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete Raw Material. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database Error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ClearInputs()
        {
            cmbOrderId.SelectedIndex = -1;
            cmbOrderId.SelectedItem = null;

            cmbItemName.SelectedIndex = -1;
            cmbItemName.SelectedItem = null;

            txtRawMaterialName.Clear(); 
            numericUpDownQuantityNeeded.Value = 0; 
            txtUnitPrice.Clear();
        }


        private void clrbtnraw_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void dgvRawmaterials_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvRawmaterials.Rows[e.RowIndex];

                try
                {
                    string orderId = row.Cells["orderId"].Value.ToString();
                    string itemId = row.Cells["itemId"].Value.ToString();

                    if (cmbOrderId.Items.Contains(orderId))
                    {
                        cmbOrderId.SelectedItem = orderId;
                    }
                    else
                    {
                        cmbOrderId.Text = orderId;  // Set the text in case the item is not in the combo
                    }

                    if (cmbItemName.Items.Cast<DataRowView>().Any(rowItem => rowItem["itemId"].ToString() == itemId))
                    {
                        cmbItemName.SelectedItem = cmbItemName.Items.Cast<DataRowView>().FirstOrDefault(rowItem => rowItem["itemId"].ToString() == itemId);
                    }
                    else
                    {
                        cmbItemName.Text = itemId;  // Set the text in case the item is not in the combo
                    }

                txtRawMaterialName.Text = row.Cells["RawMaterialName"].Value.ToString();
                numericUpDownQuantityNeeded.Value = Convert.ToDecimal(row.Cells["QuantityNeeded"].Value);
                txtUnitPrice.Text = row.Cells["UnitPrice"].Value.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cmbOrderId_SelectedIndexChanged_1(object sender, EventArgs e)
        {
           
        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {

        }

        //
        //
        //

        private void ToolsData()
        {
            dgvTools.Columns["toolId"].HeaderText = "ID";
            dgvTools.Columns["toolName"].HeaderText = "Name";
            dgvTools.Columns["quantityNeeded"].HeaderText = "Qty";
            dgvTools.Columns["toolStatus"].HeaderText = "Status";
            dgvTools.Columns["lastMaintenanceDate"].HeaderText = "Last Maintenance";
            dgvTools.Columns["orderId"].HeaderText = "Order ID";
            dgvTools.Columns["itemId"].HeaderText = "Item ID";

            dgvTools.Columns["orderId"].Width = 45;
            dgvTools.Columns["itemId"].Width = 45;
            dgvTools.Columns["toolId"].Width = 45;
            dgvTools.Columns["quantityNeeded"].Width = 35;  

        }

        private string GenerateToolId()
        {
            string newId = "T001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 toolId FROM Tools ORDER BY CAST(SUBSTRING(toolId, 2, LEN(toolId)) AS INT) DESC";

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
                MessageBox.Show($"Error generating Tool ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }

        private void LoadToolsData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Tools";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvTools.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                        // MessageBox.Show("No data found in the Tools table.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadItemNamesForTools()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT itemName, itemId FROM Items";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    cmbItemNames.DataSource = dataTable;
                    cmbItemNames.DisplayMember = "itemName";
                    cmbItemNames.ValueMember = "itemId";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading item names: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOrderIdsForTools(string itemName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(itemName))
                {
                    MessageBox.Show("Please select a valid Item Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                  SELECT DISTINCT o.orderId
                  FROM Orders o
                  INNER JOIN Items i ON o.orderId = i.orderId
                  WHERE i.itemName = @itemName";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@itemName", itemName.Trim());

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        cmbOrderIds.DataSource = dataTable;
                        cmbOrderIds.DisplayMember = "orderId";
                        cmbOrderIds.ValueMember = "orderId";
                    }
                    else
                    {
                        cmbOrderIds.DataSource = null;
                        cmbOrderIds.Items.Clear();
                        MessageBox.Show($"No Order IDs found for the selected Item Name: {itemName}.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Order IDs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbItemNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbItemNames.SelectedItem != null)
            {
                string selectedItemName = (cmbItemNames.SelectedItem as DataRowView)?["itemName"].ToString();
                if (!string.IsNullOrEmpty(selectedItemName))
                {
                    LoadOrderIdsForTools(selectedItemName);
                }
            }
        }
        private void addbtntool_Click(object sender, EventArgs e)
        {
            if (cmbItemNames.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an Item Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbItemNames.Focus();
                return;
            }

            if (cmbOrderIds.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an Order ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbOrderIds.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtToolName.Text))
            {
                MessageBox.Show("Tool Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtToolName.Focus();
                return;
            }

            if (numericUpDownQuantityNeededs.Value <= 0)
            {
                MessageBox.Show("Quantity Needed must be greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDownQuantityNeededs.Focus();
                return;
            }

            if (cmbToolStatus.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a Tool Status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbToolStatus.Focus();
                return;
            }

            DateTime lastMaintenanceDate = dateTimePickerLastMaintenance.Value;
            if (lastMaintenanceDate > DateTime.Now)
            {
                MessageBox.Show("The Maintenance Date cannot be in the future.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dateTimePickerLastMaintenance.Focus();
                return;
            }


            try
            {
                string toolId = GenerateToolId();
                string orderId = cmbOrderIds.SelectedValue.ToString();
                string itemId = cmbItemNames.SelectedValue.ToString();
                string toolStatus = cmbToolStatus.SelectedItem.ToString();  

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Tools (toolId, toolName, quantityNeeded, toolStatus, lastMaintenanceDate, orderId, itemId) " +
                                    "VALUES (@toolId, @toolName, @quantityNeeded, @toolStatus, @lastMaintenanceDate, @orderId, @itemId)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@toolId", toolId);
                    command.Parameters.AddWithValue("@toolName", txtToolName.Text.Trim());
                    command.Parameters.AddWithValue("@quantityNeeded", numericUpDownQuantityNeededs.Value);
                    command.Parameters.AddWithValue("@toolStatus", toolStatus);
                    command.Parameters.AddWithValue("@lastMaintenanceDate", lastMaintenanceDate); // You can use a specific date if required
                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@itemId", itemId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Tool added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadToolsData();
                        ClearInputss();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add Tool. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updbtntool_Click(object sender, EventArgs e)
        {
            if (dgvTools.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a tool from the list to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedToolId = dgvTools.SelectedRows[0].Cells["toolId"].Value.ToString();

            if (cmbOrderIds.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an Order ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbOrderIds.Focus();
                return;
            }

            if (cmbItemNames.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an Item Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbItemNames.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtToolName.Text))
            {
                MessageBox.Show("Tool Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtToolName.Focus();
                return;
            }

            if (numericUpDownQuantityNeededs.Value <= 0)
            {
                MessageBox.Show("Quantity Needed must be greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDownQuantityNeededs.Focus();
                return;
            }

            if (cmbToolStatus.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a Tool Status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbToolStatus.Focus();
                return;
            }

            DateTime lastMaintenanceDate = dateTimePickerLastMaintenance.Value;
            if (lastMaintenanceDate > DateTime.Now)
            {
                MessageBox.Show("The Maintenance Date cannot be in the future.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dateTimePickerLastMaintenance.Focus();
                return;
            }


            try
            {
                string orderId = cmbOrderIds.SelectedValue.ToString();
                string itemId = cmbItemNames.SelectedValue.ToString();
                string toolStatus = cmbToolStatus.SelectedItem.ToString();  // Get selected value from ComboBox

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Tools SET toolName = @toolName, quantityNeeded = @quantityNeeded, " +
                                   "toolStatus = @toolStatus, lastMaintenanceDate = @lastMaintenanceDate, " +
                                   "orderId = @orderId, itemId = @itemId WHERE toolId = @toolId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@toolId", selectedToolId);
                    command.Parameters.AddWithValue("@toolName", txtToolName.Text.Trim());
                    command.Parameters.AddWithValue("@quantityNeeded", numericUpDownQuantityNeededs.Value);
                    command.Parameters.AddWithValue("@toolStatus", toolStatus);
                    command.Parameters.AddWithValue("@lastMaintenanceDate", lastMaintenanceDate);
                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@itemId", itemId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Tool updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadToolsData();
                        ClearInputss();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update Tool. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database Error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rembtntool_Click(object sender, EventArgs e)
        {
            if (dgvTools.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a tool from the list to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedToolId = dgvTools.SelectedRows[0].Cells["toolId"].Value.ToString();

            DialogResult confirmResult = MessageBox.Show("Are you sure you want to delete this tool?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Tools WHERE toolId = @toolId";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@toolId", selectedToolId);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Tool deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadToolsData();
                            ClearInputss();
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete Tool. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database Error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void ClearInputss()
        {
            txtToolName.Clear();
            numericUpDownQuantityNeededs.Value = 0;
            cmbItemNames.SelectedItem = null;  
            cmbOrderIds.SelectedItem = null;   
            cmbToolStatus.SelectedItem = null;
            dateTimePickerLastMaintenance.Value = DateTime.Now;

        }
        private void clrbtntool_Click(object sender, EventArgs e)
        {
            ClearInputss();
        }

        private void dgvTools_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvTools.Rows[e.RowIndex];

                try
                {
                    string orderId = row.Cells["orderId"].Value.ToString();
                    string itemId = row.Cells["itemId"].Value.ToString();

                    if (cmbOrderIds.Items.Contains(orderId))
                    {
                        cmbOrderIds.SelectedItem = orderId;
                    }
                    else
                    {
                        cmbOrderIds.Text = orderId;  
                    }

                    if (cmbItemNames.Items.Cast<DataRowView>().Any(rowItem => rowItem["itemId"].ToString() == itemId))
                    {
                        cmbItemNames.SelectedItem = cmbItemNames.Items.Cast<DataRowView>().FirstOrDefault(rowItem => rowItem["itemId"].ToString() == itemId);
                    }
                    else
                    {
                        cmbItemNames.Text = itemId;  
                    }

                    txtToolName.Text = row.Cells["toolName"].Value.ToString();
                    numericUpDownQuantityNeededs.Value = Convert.ToDecimal(row.Cells["quantityNeeded"].Value);
                    cmbToolStatus.SelectedItem = row.Cells["toolStatus"].Value.ToString();
                    dateTimePickerLastMaintenance.Value = Convert.ToDateTime(row.Cells["lastMaintenanceDate"].Value);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
          //      guna2HtmlToolTip1.SetToolTip(guna2TextBox3, employeeCount.ToString() + " employees have registered in organization");


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
                  //      guna2HtmlToolTip3.SetToolTip(guna2TextBox6, "Meeting Title : " + meetingTitle);


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
                //    guna2HtmlToolTip3.SetToolTip(guna2TextBox1, supplierCount.ToString() + " suppliers have registered in the system");
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
                        //    guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending.\n " + overdueTaskCount + " of" + pendingTaskCount + " are overdue.");

                        }
                        else if (pendingTaskCount > 0 && overdueTaskCount <= 0)
                        {
                          //  guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending");

                        }
                        else if (pendingTaskCount <= 0)
                        {

                        //    guna2HtmlToolTip6.SetToolTip(guna2TextBox4, "You have no tasks to complete");
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
                      //      guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is not calculated yet");

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

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }
    }
    
}
