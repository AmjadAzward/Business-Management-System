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
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FinalProject
{
    public partial class Productionmanager : Form
    {

        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;
        public Productionmanager(string userName)
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


      


        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel23_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox16_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel14_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton3_Click_1(object sender, EventArgs e)
        {
            ProductionmanagerP mainForm = new ProductionmanagerP(currentUsername);
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

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;

        }


        private void tabPage3_Click(object sender, EventArgs e)
        {

        }


       
        private void namebox_TextChanged(object sender, EventArgs e)
        {

        }
        private void Productionmanager_Load(object sender, EventArgs e)
        {
            LoadOrderIds();
            LoadDataIntoDataGridView();
            cmbOrderId.SelectedIndex = -1;
            txtClientName.Clear();

            Load1();
            LoadProjectIds();
            LoadEmployeeNames();

            LoadEmployeeTasks(employeeId);

            LoadFeedbackIntoDGV(employeeId);
            FeedbackData();
            LoadEmployeeData();




            CountEmployees();
            GetMostRecentMeetingForUser(currentUsername);
            CountSuppliers();
            //CountPastProjects();
            LoadTaskCounts(currentUsername);
            LoadLatestSalary(currentUsername);

        }

        //
        //
        // item 

        private void LoadOrderIds()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT [orderId] FROM [finalPJS].[dbo].[Orders]";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                cmbOrderId.DisplayMember = "orderId";  // Display orderId in the ComboBox
                cmbOrderId.ValueMember = "orderId";    // Set orderId as the value member

                cmbOrderId.DataSource = dataTable;  // Bind the data source to ComboBox
            }
        }

        private void LoadDataIntoDataGridView()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT itemId, itemName, itemPrice, itemQuantity, description, totalCost, orderId, clientId FROM Items";

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();

                    dataAdapter.Fill(dataTable);

                    dgvitem.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private string GenerateItemId()
        {
            string newId = "ID001"; 

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 itemId FROM Items ORDER BY CAST(SUBSTRING(itemId, 3, LEN(itemId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();

                            int numericPart = int.Parse(lastId.Substring(2));  // Skipping "ID"
                            newId = "ID" + (numericPart + 1).ToString("D3");  // Format as ID001, ID002, etc.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Item ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }


        private void ordercombo_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cmbOrderId.SelectedIndex != -1)
            {
                string selectedOrderId = cmbOrderId.SelectedValue.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT [clientId] FROM [finalPJS].[dbo].[Orders] WHERE [orderId] = @orderId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@orderId", selectedOrderId);

                    connection.Open();
                    var result = command.ExecuteScalar();  // Execute the query and get the clientId

                    if (result != null)
                    {
                        txtClientName.Text = result.ToString();  
                    }
                }
            }
        }


        


        private void LoadClientId(string orderId)
        {
            try
            {
                string query = "SELECT clientId FROM Orders WHERE orderId = '" + orderId + "'";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string clientId = reader["clientId"].ToString();
                            txtClientName.Text = clientId; 
                        }
                    }
                    else
                    {
                        MessageBox.Show("Order not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading clientId: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void addbtn1_Click(object sender, EventArgs e)
        {
            if (cmbOrderId.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a valid Order ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtClientName.Text))
            {
                MessageBox.Show("Client Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtItemName.Text))
            {
                MessageBox.Show("Item Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal itemPrice) || itemPrice <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numericUpDownQuantity.Value <= 0)
            {
                MessageBox.Show("Quantity must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {

                string clientId=txtClientName.Text;

                

                decimal totalPrice = itemPrice * numericUpDownQuantity.Value;
                totalbox.Text = totalPrice.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Items (itemId, itemName, itemPrice, itemQuantity, description, totalCost, orderId, clientId) " +
                                   "VALUES (@itemId, @itemName, @itemPrice, @itemQuantity, @description, @totalCost, @orderId, @clientId)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@itemId", GenerateItemId());
                        command.Parameters.AddWithValue("@itemName", txtItemName.Text);
                        command.Parameters.AddWithValue("@itemPrice", itemPrice);
                        command.Parameters.AddWithValue("@itemQuantity", numericUpDownQuantity.Value);
                        command.Parameters.AddWithValue("@description", txtDescription.Text);
                        command.Parameters.AddWithValue("@totalCost", totalPrice);
                        command.Parameters.AddWithValue("@orderId", cmbOrderId.SelectedValue.ToString());
                        command.Parameters.AddWithValue("@clientId", clientId); 

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Item added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataIntoDataGridView();
                ClearFields();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void updbtnacc_Click(object sender, EventArgs e)
        {
            if (dgvitem.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtItemName.Text))
            {
                MessageBox.Show("Item Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Price is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out decimal itemPrice) || itemPrice <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numericUpDownQuantity.Value <= 0)
            {
                MessageBox.Show("Quantity must be greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                decimal totalPrice = itemPrice * numericUpDownQuantity.Value;

                string itemId = dgvitem.SelectedRows[0].Cells["itemId"].Value.ToString();

                string projectId = GetProjectIdForOrder(cmbOrderId.SelectedValue.ToString());
                if (string.IsNullOrEmpty(projectId))
                {
                    MessageBox.Show("No project found for the selected Order ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Items SET itemName = @itemName, itemPrice = @itemPrice, itemQuantity = @itemQuantity, description = @description, " +
                                   "totalCost = @totalCost, orderId = @orderId, clientId = @clientId, projectId = @projectId WHERE itemId = @itemId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@itemName", txtItemName.Text);
                        command.Parameters.AddWithValue("@itemPrice", itemPrice);
                        command.Parameters.AddWithValue("@itemQuantity", numericUpDownQuantity.Value);
                        command.Parameters.AddWithValue("@description", txtDescription.Text);
                        command.Parameters.AddWithValue("@totalCost", totalPrice);
                        command.Parameters.AddWithValue("@orderId", cmbOrderId.SelectedValue.ToString());
                        command.Parameters.AddWithValue("@clientId", txtClientName.Text);
                        command.Parameters.AddWithValue("@projectId", projectId);
                        command.Parameters.AddWithValue("@itemId", itemId);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Item updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataIntoDataGridView();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void removebtn1_Click(object sender, EventArgs e)
        {
            if (dgvitem.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string itemId = dgvitem.SelectedRows[0].Cells["itemId"].Value.ToString();

            DialogResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Items WHERE itemId = @itemId";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@itemId", itemId);

                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Item deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataIntoDataGridView();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearFields()
        {
            txtItemName.Clear();
            txtPrice.Clear();
            numericUpDownQuantity.Value = 0;
            txtDescription.Clear();
            txtClientName.Clear();
            cmbOrderId.SelectedIndex = -1;
            totalbox.Clear();
        }


        private void clearbtn1_Click(object sender, EventArgs e)
        {
            ClearFields();
        }


        private string GetProjectIdForOrder(string orderId)
        {
            string projectId = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT projectId FROM Orders WHERE orderId = @orderId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderId", orderId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        projectId = reader["projectId"].ToString();
                    }
                }
            }

            return projectId;
        }

        private void dgvitem_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvitem.Rows[e.RowIndex];

                txtItemName.Text = row.Cells["itemName"].Value.ToString();
                txtPrice.Text = row.Cells["itemPrice"].Value.ToString();
                numericUpDownQuantity.Value = Convert.ToDecimal(row.Cells["itemQuantity"].Value);
                txtDescription.Text = row.Cells["description"].Value.ToString();
                txtClientName.Text = row.Cells["clientId"].Value.ToString();
                cmbOrderId.SelectedValue = row.Cells["orderId"].Value.ToString();
                totalbox.Text = row.Cells["totalCost"].Value.ToString();
            }
        }

        private void dgvitem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void guna2Button14_Click(object sender, EventArgs e)
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


        //
        //
        //


        private string GenerateInstallationId()
        {
            string newId = "INS001"; // Default ID format for installations

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 installationId FROM Installation ORDER BY CAST(SUBSTRING(installationId, 4, LEN(installationId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString(); 

                            int numericPart = int.Parse(lastId.Substring(3)); // Remove 'INS' and parse the numeric part
                            newId = "INS" + (numericPart + 1).ToString("D3"); // Increment the numeric part and format it
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Installation ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId; 
        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {
            string ID = GenerateInstallationId();
            string installationAddress = address.Text.Trim();
            if (string.IsNullOrEmpty(installationAddress))
            {
                MessageBox.Show("Please enter the Installation Address.");
                address.Focus();
                return;
            }

            DateTime installationDate = date.Value;

            string installationTeam = team.Text.Trim();
            if (string.IsNullOrEmpty(installationTeam))
            {
                MessageBox.Show("Please enter the Installation Team.");
                team.Focus();
                return;
            }

            string installationStatus = istatus.Text.Trim();
            if (string.IsNullOrEmpty(installationStatus))
            {
                MessageBox.Show("Please select the Installation Status.");
                istatus.Focus();
                return;
            }

            string projectId = projectidCMBs.Text.Trim();
            if (string.IsNullOrEmpty(projectId))
            {
                MessageBox.Show("Please select a Project ID.");
                projectidCMBs.Focus();
                return;
            }

            string orderId = orderid.Text.Trim();
            if (string.IsNullOrEmpty(orderId))
            {
                MessageBox.Show("Please enter the Order ID.");
                orderid.Focus();
                return;
            }

            string checkQuery = "SELECT COUNT(*) FROM Installation WHERE projectId = @projectId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Check if the project already has an installation
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@projectId", projectId);

                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("This Project ID already has an Installation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // If no installation exists for the project, proceed with the insert
                    string insertQuery = @"INSERT INTO Installation 
                (installationId, installationAddress, installationDate, installationTeam, 
                 installationStatus, projectId, orderId)
                VALUES 
                (@installationId, @installationAddress, @installationDate, @installationTeam, 
                 @installationStatus, @projectId, @orderId)";

                    SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@installationId", ID);
                    cmd.Parameters.AddWithValue("@installationAddress", installationAddress);
                    cmd.Parameters.AddWithValue("@installationDate", installationDate);
                    cmd.Parameters.AddWithValue("@installationTeam", installationTeam);
                    cmd.Parameters.AddWithValue("@installationStatus", installationStatus);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@orderId", orderId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputFields();
                        Load1();
                    }
                    else
                    {
                        MessageBox.Show("Error: Record not added.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void ClearInputFields()
        {
            address.Clear();
            team.SelectedIndex = -1;
            istatus.SelectedIndex = -1;
            projectidCMBs.SelectedIndex = -1;
            orderid.Clear();
            date.Value = DateTime.Now;
            projectName.Clear();

        }

        private void LoadProjectIds()
        {
            string query = "SELECT projectId FROM Project";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    projectidCMBs.Items.Clear();

                    while (reader.Read())
                    {
                        projectidCMBs.Items.Add(reader["projectId"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading project IDs: " + ex.Message);
            }
        }

        private void projectidCMBs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (projectidCMBs.SelectedItem != null)
            {
                string selectedProjectId = projectidCMBs.SelectedItem.ToString();
                LoadProjectName(selectedProjectId);
            }
        }


        private void Load1()
        {
            try
            {
                string query = @"
            SELECT TOP (1000) 
                installationId,
                installationAddress,
                installationDate,
                installationTeam,
                installationStatus,
                projectId,
                orderId
            FROM Installation";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    dgvinstallation.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProjectName(string projectId)
        {
            string query = "SELECT projectName, orderId FROM Project WHERE projectId = @projectId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@projectId", projectId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string projectName1 = reader["projectName"].ToString();
                        string orderId = reader["orderId"].ToString();

                        projectName.Text = projectName1;  // Set projectName in the TextBox
                        orderid.Text = orderId;          // Set orderId in the TextBox
                    }
                    else
                    {
                        projectName.Text = "Project not found";
                        orderid.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }


        private void projectid_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        private void LoadEmployeeNames()
        {
            string query = "SELECT firstName, lastName FROM Employee";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    team.Items.Clear();

                    while (reader.Read())
                    {
                        string fullName = reader["firstName"].ToString() + " " + reader["lastName"].ToString();
                        team.Items.Add(fullName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }
        private void team_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dgvinstallation_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dgvinstallation_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvinstallation.Rows[e.RowIndex];

                string installationId = selectedRow.Cells["installationId"].Value.ToString();
                string installationAddress = selectedRow.Cells["installationAddress"].Value.ToString();
                string installationDate = selectedRow.Cells["installationDate"].Value.ToString();
                string installationTeam = selectedRow.Cells["installationTeam"].Value.ToString();
                string installationStatus = selectedRow.Cells["installationStatus"].Value.ToString();
                string projectId = selectedRow.Cells["projectId"].Value.ToString();
                string orderId = selectedRow.Cells["orderId"].Value.ToString();

                address.Text = installationAddress;
                team.Text = installationTeam;
                istatus.Text = installationStatus;
                orderid.Text = orderId;

                if (projectidCMBs.Items.Contains(projectId))
                {
                    projectidCMBs.SelectedItem = projectId;
                }
                else
                {
                    MessageBox.Show("Project ID not found.");
                }

                DateTime parsedDate;
                if (DateTime.TryParse(installationDate, out parsedDate))
                {
                    date.Value = parsedDate;
                }
                else
                {
                    MessageBox.Show("Invalid date format.");
                }
            }

        }

        private void iupdate_Click(object sender, EventArgs e)
        {
            if (dgvinstallation.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to update.");
                dgvinstallation.Focus();
                return;
            }

            // Get installationId from the first column of the selected row
            string installationId = dgvinstallation.SelectedRows[0].Cells[0].Value.ToString();

            string installationAddress = address.Text.Trim();
            if (string.IsNullOrEmpty(installationAddress))
            {
                MessageBox.Show("Please enter the Installation Address.");
                address.Focus();
                return;
            }

            string installationTeam = team.Text.Trim();
            if (string.IsNullOrEmpty(installationTeam))
            {
                MessageBox.Show("Please enter the Installation Team.");
                team.Focus();
                return;
            }

            string installationStatus = istatus.Text.Trim();
            if (string.IsNullOrEmpty(installationStatus))
            {
                MessageBox.Show("Please select the Installation Status.");
                istatus.Focus();
                return;
            }

            string projectId = projectidCMBs.Text.Trim();
            if (string.IsNullOrEmpty(projectId))
            {
                MessageBox.Show("Please select a Project ID.");
                projectidCMBs.Focus();
                return;
            }

            string orderId = orderid.Text.Trim();
            if (string.IsNullOrEmpty(orderId))
            {
                MessageBox.Show("Please enter the Order ID.");
                orderid.Focus();
                return;
            }

            // Check if the selected project ID already exists in the Installation table (excluding current installationId)
            string checkQuery = @"SELECT COUNT(*) FROM Installation 
                          WHERE projectId = @projectId AND installationId <> @installationId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Check for duplicate projectId
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@projectId", projectId);
                    checkCmd.Parameters.AddWithValue("@installationId", installationId);

                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("This Project ID is already associated with another Installation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Update query
                    string query = @"UPDATE Installation 
                             SET installationAddress = @installationAddress,
                                 installationDate = @installationDate,
                                 installationTeam = @installationTeam,
                                 installationStatus = @installationStatus,
                                 projectId = @projectId,
                                 orderId = @orderId
                             WHERE installationId = @installationId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@installationAddress", installationAddress);
                    cmd.Parameters.AddWithValue("@installationDate", date.Value);
                    cmd.Parameters.AddWithValue("@installationTeam", installationTeam);
                    cmd.Parameters.AddWithValue("@installationStatus", installationStatus);
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    cmd.Parameters.AddWithValue("@installationId", installationId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputFields();
                        Load1(); // Refresh the DataGridView
                    }
                    else
                    {
                        MessageBox.Show("Error: Record not updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show("SQL Error: " + sqlEx.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


        private void iremove_Click(object sender, EventArgs e)
        {
            if (dgvinstallation.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dgvinstallation.Focus();
                return;
            }

            // Retrieve the installationId from the selected row
            string installationId = dgvinstallation.SelectedRows[0].Cells[0].Value.ToString();

            DialogResult confirmResult = MessageBox.Show(
                "Are you sure you want to delete this record?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                string deleteQuery = @"DELETE FROM Installation WHERE installationId = @installationId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                        cmd.Parameters.AddWithValue("@installationId", installationId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Load1(); // Reload the data in the DataGridView
                        }
                        else
                        {
                            MessageBox.Show("Error: Record not deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show("SQL Error: " + sqlEx.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void iclear_Click(object sender, EventArgs e)
        {
            ClearInputFields();
        }

        private void tabPage5_Click(object sender, EventArgs e)
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
                guna2TextBox3.Text = employeeCount.ToString();
                //guna2HtmlToolTip1.SetToolTip(guna2TextBox3, employeeCount.ToString() + " employees have registered in organization");


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
                 //       guna2HtmlToolTip3.SetToolTip(guna2TextBox6, "Meeting Title : " + meetingTitle);


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
               //     guna2HtmlToolTip3.SetToolTip(guna2TextBox1, supplierCount.ToString() + " suppliers have registered in the system");
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
                            //guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending.\n " + overdueTaskCount + " of" + pendingTaskCount + " are overdue.");

                        }
                        else if (pendingTaskCount > 0 && overdueTaskCount <= 0)
                        {
                          //  guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending");

                        }
                        else if (pendingTaskCount <= 0)
                        {

                            //guna2HtmlToolTip6.SetToolTip(guna2TextBox4, "You have no tasks to complete");
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
                     //       guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is Rs" + finalAmount.ToString() + ".00");

                        }
                        else
                        {
                            guna2TextBox2.Text = "Rs 0.00";
                        //    guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is not calculated yet");

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
