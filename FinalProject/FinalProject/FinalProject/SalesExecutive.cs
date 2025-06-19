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
    public partial class SalesExecutive : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;
        public SalesExecutive(string userName)
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

        private void LoadOrderIds()
        {
            string query = "SELECT orderId FROM Orders";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    orderbox.Items.Clear();

                    while (reader.Read())
                    {
                        orderbox.Items.Add(reader["orderId"].ToString());
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
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

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
            SalesP mainForm = new SalesP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2TextBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel7_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Login mainForm = new Login();
            mainForm.Show();
            this.Hide();
        }

        private void SalesExecutive_Load(object sender, EventArgs e)
        {
            LoadEmployeeTasks(employeeId);

            LoadFeedbackIntoDGV(employeeId);
            FeedbackData();
            LoadEmployeeData();
            LoadOrderIds();


            LoadSalesData();



            CountEmployees();
            GetMostRecentMeetingForUser(currentUsername);
            CountSuppliers();
            //    CountPastProjects();
            LoadLatestSalary(currentUsername);
            LoadTaskCounts(currentUsername);
        }

        private void guna2Button8_Click(object sender, EventArgs e)
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
                        ClearInput();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating task: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearInput()
        {
            accTaskNames.Text = string.Empty; // Clear task name text box
            accDeadline.Text = string.Empty; // Clear deadline text box
            cmbaccStatuss.SelectedIndex = -1; // Reset combo box selection
            accPriority.Text = string.Empty; // Clear priority text box
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

        private void guna2Button2_Click_1(object sender, EventArgs e)
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

        public string ClientIdByName(string clientName)
        {
            string query = "SELECT TOP 1 [clientId] FROM [finalPJS].[dbo].[Client] WHERE [clientName] = @clientName";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@clientName", clientName);

                try
                {
                    conn.Open();
                    var result = cmd.ExecuteScalar();

                    // If result is not null or DBNull, return it as a string
                    return result != DBNull.Value ? result.ToString() : null;
                }
                catch (Exception ex)
                {
                    // Handle exceptions by returning null or a custom error message
                    MessageBox.Show("Error: " + ex.Message);
                    return null;
                }
            }

        }
        private string GenerateSalesId()
        {
            // Default ID if the table is empty
            string newId = "SL001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Query to fetch the last sales ID with prefix 'SL' in descending order
                    string query = @"
                SELECT TOP 1 salesId 
                FROM Sales 
                WHERE salesId LIKE 'SL%' 
                ORDER BY CAST(SUBSTRING(salesId, 3, LEN(salesId) - 2) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            // Extract the numeric part from the last ID
                            string lastId = result.ToString();
                            int numericPart = int.Parse(lastId.Substring(2)); // Extract the numeric part after 'SL'

                            // Generate a new ID by incrementing the numeric part
                            newId = "SL" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Show an error message in case of exceptions
                MessageBox.Show($"Error generating Sales ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }

        private void addbtn2_Click(object sender, EventArgs e)
        {
            string salid = GenerateSalesId();

            // Validate user input
            if (string.IsNullOrWhiteSpace(salesamt.Text) ||
                string.IsNullOrWhiteSpace(status.Text) ||
                string.IsNullOrWhiteSpace(clientbox.Text) ||
                string.IsNullOrWhiteSpace(orderbox.Text) ||
                string.IsNullOrWhiteSpace(paymentid.Text))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }
            if (datebox.Value <= DateTime.Now.Date)
            {
                MessageBox.Show("Sales date must be today or a future date.");
                return;
            }

            // Get values from input fields
            string salesAmount = salesamt.Text;
            DateTime salesDate = datebox.Value;
            string salesStatus = status.Text;
            string clientname = clientbox.Text;

            string clientId = ClientIdByName(clientname);
            string orderId = orderbox.Text;
            string clientPaymentId = paymentid.Text;

            // Check if a sales record already exists for this order
            string checkQuery = "SELECT COUNT(*) FROM [finalPJS].[dbo].[Sales] WHERE orderId = @orderId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@orderId", orderId);

                        int existingCount = (int)checkCommand.ExecuteScalar();
                        if (existingCount > 0)
                        {
                            MessageBox.Show("A sales record already exists for this order.");
                            return;
                        }
                    }

                    // Insert query
                    string insertQuery = @"
                INSERT INTO [finalPJS].[dbo].[Sales] 
                (salesId, salesAmount, salesDate, salesStatus, clientId, orderId, clientPaymentId)
                VALUES 
                (@salesid, @salesAmount, @salesDate, @salesStatus, @clientId, @orderId, @clientPaymentId)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Add parameters to the query
                        command.Parameters.AddWithValue("@salesid", salid);
                        command.Parameters.AddWithValue("@salesAmount", decimal.Parse(salesAmount));
                        command.Parameters.AddWithValue("@salesDate", salesDate);
                        command.Parameters.AddWithValue("@salesStatus", salesStatus);
                        command.Parameters.AddWithValue("@clientId", clientId);
                        command.Parameters.AddWithValue("@orderId", orderId);
                        command.Parameters.AddWithValue("@clientPaymentId", clientPaymentId);

                        int rowsAffected = command.ExecuteNonQuery();

                        // Confirm success
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sales record added successfully!");
                            clear1();
                            LoadSalesData();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add the sales record.");
                            clear1();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        public void clear1()
        {
            orderbox.SelectedIndex = -1;
            salesamt.Clear();
            status.SelectedIndex = -1;
            clientbox.Clear();
            paymentid.Clear();
            datebox.Value = DateTime.Now;
        }

        private void LoadSalesData()
        {
            string query = "SELECT * FROM [finalPJS].[dbo].[Sales]"; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable salesTable = new DataTable();
                        adapter.Fill(salesTable);

                        dgv10.DataSource = salesTable; 
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading sales data: {ex.Message}");
                }
            }
        }


        private void orderbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (orderbox.SelectedItem != null)
            {
                string selectedOrderId = orderbox.SelectedItem.ToString();

             
                string clientQuery = @"
            SELECT C.clientName
            FROM [finalPJS].[dbo].[Orders] O
            INNER JOIN [finalPJS].[dbo].[Client] C ON O.clientId = C.clientId
            WHERE O.orderId = @orderId";

                string paymentQuery = @"
            SELECT *
            FROM [finalPJS].[dbo].[ClientPayments]
            WHERE orderId = @orderId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (SqlCommand clientCommand = new SqlCommand(clientQuery, connection))
                        {
                            clientCommand.Parameters.AddWithValue("@orderId", selectedOrderId);

                            object clientResult = clientCommand.ExecuteScalar();

                            if (clientResult != null)
                            {
                                clientbox.Text = clientResult.ToString();
                            }
                           
                        }

                        using (SqlCommand paymentCommand = new SqlCommand(paymentQuery, connection))
                        {
                            paymentCommand.Parameters.AddWithValue("@orderId", selectedOrderId);

                            using (SqlDataReader reader = paymentCommand.ExecuteReader())
                            {
                                if (reader.Read()) 
                                {
                                    string paymentId = reader["clientPaymentId"].ToString();
                                    string paymentamt = reader["finalCost"].ToString();

                                    paymentid.Text = paymentId; 
                                    salesamt.Text = paymentamt;
                                }
                                
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }
        private string ClientNameById(string clientId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT clientName FROM Clients WHERE clientId = @clientId";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@clientId", clientId);
                        connection.Open();
                        return command.ExecuteScalar()?.ToString() ?? "Client not found";
                    }
                }
            }
            catch
            {
                return "Error retrieving client name";
            }
        }


        private void dgv10_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is not on a header row or column
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dgv10.Rows[e.RowIndex];

                // Load data into input fields
                salesamt.Text = selectedRow.Cells["salesAmount"].Value?.ToString();
                datebox.Value = Convert.ToDateTime(selectedRow.Cells["salesDate"].Value);
                status.Text = selectedRow.Cells["salesStatus"].Value?.ToString();
                clientbox.Text = ClientNameById(selectedRow.Cells["clientId"].Value?.ToString());
                orderbox.Text = selectedRow.Cells["orderId"].Value?.ToString();
                paymentid.Text = selectedRow.Cells["clientPaymentId"].Value?.ToString();
            }
        }

        private void updatebtn2_Click(object sender, EventArgs e)
        {
            // Validate user input
            if (string.IsNullOrWhiteSpace(salesamt.Text) ||
                string.IsNullOrWhiteSpace(status.Text) ||
                string.IsNullOrWhiteSpace(clientbox.Text) ||
                string.IsNullOrWhiteSpace(orderbox.Text) ||
                string.IsNullOrWhiteSpace(paymentid.Text))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            // Get values from input fields
            string salesAmount = salesamt.Text;
            DateTime salesDate = datebox.Value; // Replace with user input if needed
            string salesStatus = status.Text;
            string clientname = clientbox.Text;
            string clientId = ClientIdByName(clientname);
            string orderId = orderbox.Text;
            string clientPaymentId = paymentid.Text;

            // Retrieve the selected row
            DataGridViewRow selectedRow = dgv10.CurrentCell.OwningRow;

            // Validate that a row is selected
            if (selectedRow == null)
            {
                MessageBox.Show("No row selected.");
                return;
            }

            // Retrieve the salesId from the first column
            string salesId = selectedRow.Cells[0].Value.ToString();

            // Validate that salesId is not null or empty
            if (string.IsNullOrEmpty(salesId))
            {
                MessageBox.Show("Invalid sales record selected.");
                return;
            }

            // Ensure that only one sale corresponds to one order
            // Check if the orderId is already linked to another salesId
            string checkOrderQuery = @"
    SELECT COUNT(*)
    FROM [finalPJS].[dbo].[Sales]
    WHERE orderId = @orderId AND salesId != @salesId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if the orderId is already linked to another salesId
                    using (SqlCommand command = new SqlCommand(checkOrderQuery, connection))
                    {
                        command.Parameters.AddWithValue("@orderId", orderId);
                        command.Parameters.AddWithValue("@salesId", salesId);

                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("This order is already linked to another sale.");
                            return;
                        }

                        // Proceed with the update query if the order is not already linked
                        string updateQuery = @"
                UPDATE [finalPJS].[dbo].[Sales]
                SET salesAmount = @salesAmount,
                    salesDate = @salesDate,
                    salesStatus = @salesStatus,
                    clientId = @clientId,
                    orderId = @orderId,
                    clientPaymentId = @clientPaymentId
                WHERE salesId = @salesId;";

                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            // Add parameters to the query
                            updateCommand.Parameters.AddWithValue("@salesAmount", decimal.Parse(salesAmount));
                            updateCommand.Parameters.AddWithValue("@salesDate", salesDate);
                            updateCommand.Parameters.AddWithValue("@salesStatus", salesStatus);
                            updateCommand.Parameters.AddWithValue("@clientId", clientId);
                            updateCommand.Parameters.AddWithValue("@orderId", orderId);
                            updateCommand.Parameters.AddWithValue("@clientPaymentId", clientPaymentId);
                            updateCommand.Parameters.AddWithValue("@salesId", salesId);  // Use the selected salesId to identify the record

                            int rowsAffected = updateCommand.ExecuteNonQuery();

                            // Confirm success
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Sales record updated successfully!");
                                clear1();
                                LoadSalesData();  // Reload updated data
                            }
                            else
                            {
                                MessageBox.Show("Failed to update the sales record.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void removebtn2_Click(object sender, EventArgs e)
        {
            // Retrieve the selected row
            DataGridViewRow selectedRow = dgv10.CurrentCell.OwningRow;

            // Validate that a row is selected
            if (selectedRow == null)
            {
                MessageBox.Show("No row selected.");
                return;
            }

            // Retrieve the salesId from the first column
            string salesId = selectedRow.Cells[0].Value.ToString();

            // Validate that salesId is not null or empty
            if (string.IsNullOrEmpty(salesId))
            {
                MessageBox.Show("Invalid sales record selected.");
                return;
            }

            // Confirm deletion
            DialogResult confirmResult = MessageBox.Show("Are you sure you want to delete this sales record?", "Confirm Deletion", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.No)
            {
                return;
            }

            // Delete query
            string deleteQuery = @"
    DELETE FROM [finalPJS].[dbo].[Sales]
    WHERE salesId = @salesId;";

            // Execute delete query
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        // Add parameter to the query
                        command.Parameters.AddWithValue("@salesId", salesId);

                        int rowsAffected = command.ExecuteNonQuery();

                        // Confirm success
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sales record deleted successfully!");
                            LoadSalesData();  // Reload updated data
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the sales record.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
        }

        private void clearbtn2_Click(object sender, EventArgs e)
        {
            clear1();
        }

        private void dgv10_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is not on a header row or column
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dgv10.Rows[e.RowIndex];

                // Load data into input fields
                salesamt.Text = selectedRow.Cells["salesAmount"].Value?.ToString();
                datebox.Value = Convert.ToDateTime(selectedRow.Cells["salesDate"].Value);
                status.Text = selectedRow.Cells["salesStatus"].Value?.ToString();
                clientbox.Text = ClientNameById(selectedRow.Cells["clientId"].Value?.ToString());
                orderbox.Text = selectedRow.Cells["orderId"].Value?.ToString();
                paymentid.Text = selectedRow.Cells["clientPaymentId"].Value?.ToString();
            }
        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
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


//                guna2HtmlToolTip1.SetToolTip(guna2TextBox3, employeeCount.ToString() + " employees have registered in organization");


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
                   //     guna2HtmlToolTip3.SetToolTip(guna2TextBox6, "Meeting Title : " + meetingTitle);


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
              //      guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Company currently works on " + pastProjectCount.ToString() + " projects");
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
                            //guna2HtmlToolTip6.SetToolTip(guna2TextBox5, pendingTaskCount + " tasks are pending.\n " + overdueTaskCount + " of" + pendingTaskCount + " are overdue.");

                        }
                        else if (pendingTaskCount > 0 && overdueTaskCount <= 0)
                        {
                            //guna2HtmlToolTip6.SetToolTip(guna2TextBox5, pendingTaskCount + " tasks are pending");

                        }
                        else if (pendingTaskCount <= 0)
                        {

                            //na2HtmlToolTip6.SetToolTip(guna2TextBox5, "You have no tasks to complete");
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
                            //guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is Rs" + finalAmount.ToString() + ".00");

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

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {

            SalesRep reportForm = new SalesRep();

            reportForm.GenerateReport();

            // Show the report form
            reportForm.ShowDialog();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            SalesRep reportForm = new SalesRep();

            reportForm.GenerateReport();

            // Show the report form
            reportForm.ShowDialog();

        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }
    }
}

