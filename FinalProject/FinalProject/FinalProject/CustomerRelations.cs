using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject
{
    public partial class CustomerRelations : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;


        public CustomerRelations(string userName)
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
        
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel15_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton3_Click_1(object sender, EventArgs e)
        {




        //    string userName = "";


           CustomerRelationsP mainForm = new CustomerRelationsP(currentUsername);
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
        private void CustomerRelations_Load(object sender, EventArgs e)
        {
            LoadDataIntoDGV();
            Clientdata();

            LoadOrdersIntoDGV();
            OrderData();
            LoadClientIdsIntoComboBox();

            LoadFeedbackIntoDGV(employeeId);
            FeedbackData();
            LoadEmployeeData();

            LoadEmployeeTasks(employeeId);


            CountEmployees();
            GetMostRecentMeetingForUser(currentUsername);
            CountSuppliers();
            //CountPastProjects();
            LoadTaskCounts(currentUsername);
            LoadLatestSalary(currentUsername);




        }

        //
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

                    custask.DataSource = dataTable;

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
//

        private void guna2Button5_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {
        }

        private void ClearInputs()
        {
            txtClientName.Clear();
            txtClientPhoneNo.Clear();
            txtClientAddress.Clear();
        }

        /// client
        private void Clientdata()
        {
            dgvClient.Columns["clientId"].HeaderText = "ID";
            dgvClient.Columns["clientName"].HeaderText = "Name";
            dgvClient.Columns["clientPhoneNo"].HeaderText = "Phone Number";
            dgvClient.Columns["clientAddress"].HeaderText = "Address";
        }


        private string GenerateClientId()
        {
            string newId = "C001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 clientId FROM Client ORDER BY CAST(SUBSTRING(clientId, 2, LEN(clientId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();

                            int numericPart = int.Parse(lastId.Substring(1));
                            newId = "C" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Client ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }

        private void LoadDataIntoDGV()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Client";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvClient.DataSource = dataTable;

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No data found in the Client table.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private bool CheckClientPhoneNumberUnique(string phoneNumber)
        {
            using (SqlConnection connection = new SqlConnection("Server=AmjadAzward\\SQLEXPRESS;Database=finalPJS;Trusted_Connection=True;"))
            {
                string query = "SELECT COUNT(*) FROM Client WHERE clientPhoneNo = @PhoneNumber";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                connection.Close();

                return count == 0;
            }
        }

        
        private void addbtncl_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtClientName.Text.Trim()))
            {
                MessageBox.Show("Client Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClientName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtClientPhoneNo.Text.Trim()) || !long.TryParse(txtClientPhoneNo.Text.Trim(), out _) || txtClientPhoneNo.Text.Trim().Length != 10)
            {
                MessageBox.Show("A valid Phone Number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClientPhoneNo.Focus();
                return;
            }

            bool isPhoneNumberUnique = CheckClientPhoneNumberUnique(txtClientPhoneNo.Text.Trim());
            if (!isPhoneNumberUnique)
            {
                MessageBox.Show("Phone Number already exists. Please use another.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClientPhoneNo.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtClientAddress.Text.Trim()))
            {
                MessageBox.Show("Client Address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClientAddress.Focus();
                return;
            }

            try
            {
                string clientId = GenerateClientId();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Client (clientId, clientName, clientPhoneNo, clientAddress) " +
                                   "VALUES (@clientId, @clientName, @clientPhoneNo, @clientAddress);";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@clientId", clientId);
                        command.Parameters.AddWithValue("@clientName", txtClientName.Text.Trim());
                        command.Parameters.AddWithValue("@clientPhoneNo", txtClientPhoneNo.Text.Trim());
                        command.Parameters.AddWithValue("@clientAddress", txtClientAddress.Text.Trim());

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Client added successfully! Client ID: " + clientId, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDGV();
                            ClearInputs();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add client. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2TextBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void updbtncl_Click(object sender, EventArgs e)
        {
            if (dgvClient.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a client to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvClient.SelectedRows[0];
            string clientId = selectedRow.Cells["clientId"].Value.ToString();
            string clientName = txtClientName.Text.Trim();
            string clientPhoneNo = txtClientPhoneNo.Text.Trim();
            string clientAddress = txtClientAddress.Text.Trim();

            if (string.IsNullOrEmpty(clientName))
            {
                MessageBox.Show("Client Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClientName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtClientPhoneNo.Text.Trim()) || !long.TryParse(txtClientPhoneNo.Text.Trim(), out _) || txtClientPhoneNo.Text.Trim().Length != 10)
            {
                MessageBox.Show("A valid Phone Number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClientPhoneNo.Focus();
                return;
            }

            bool isPhoneNumberUnique = CheckClientPhoneNumberUnique(txtClientPhoneNo.Text.Trim());
            if (!isPhoneNumberUnique)
            {
                MessageBox.Show("Phone Number already exists. Please use another.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClientPhoneNo.Focus();
                return;
            }
            if (string.IsNullOrEmpty(clientAddress))
            {
                MessageBox.Show("Client Address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClientAddress.Focus();
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Client SET clientName = @clientName, clientPhoneNo = @clientPhoneNo, clientAddress = @clientAddress WHERE clientId = @clientId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@clientId", clientId);
                    command.Parameters.AddWithValue("@clientName", clientName);
                    command.Parameters.AddWithValue("@clientPhoneNo", clientPhoneNo);
                    command.Parameters.AddWithValue("@clientAddress", clientAddress);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Client updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDGV();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Client not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: "+ ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rembtncl_Click(object sender, EventArgs e)
        {

            if (dgvClient.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a client to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvClient.SelectedRows[0];
            string clientId = selectedRow.Cells["clientId"].Value.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Client WHERE clientId = @clientId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@clientId", clientId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Client deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDGV();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Client not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvClient_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvClient.Rows[e.RowIndex];

                string clientName = selectedRow.Cells["clientName"].Value.ToString();
                string clientPhoneNo = selectedRow.Cells["clientPhoneNo"].Value.ToString();
                string clientAddress = selectedRow.Cells["clientAddress"].Value.ToString();

                txtClientName.Text = clientName;
                txtClientPhoneNo.Text = clientPhoneNo;
                txtClientAddress.Text = clientAddress;
            }
        }

        private void clrbtncl_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void guna2HtmlLabel13_Click(object sender, EventArgs e)
        {

        }


        //order

        private void dgvOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvOrders.Rows[e.RowIndex];

                cmbOrderType.SelectedItem = selectedRow.Cells["orderType"].Value.ToString();
                cmbOrderStatus.SelectedItem = selectedRow.Cells["orderStatus"].Value.ToString();
                cmbClientId.SelectedItem = selectedRow.Cells["clientId"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Please select a valid row.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadClientIdsIntoComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT clientId FROM Client";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        cmbClientId.Items.Add(reader["clientId"].ToString());
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Client IDs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2ComboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbClientId.SelectedItem == null)
                return;

            string selectedClientId = cmbClientId.SelectedItem.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT clientName FROM Client WHERE clientId = @clientId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@clientId", selectedClientId);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    connection.Close();

                    if (result != null)
                    {
                        txtOClientName.Text = result.ToString();
                    }
                    else
                    {
                        txtOClientName.Clear();
                        MessageBox.Show("Client Name not found for the selected Client ID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching Client Name: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

     
        private void OrderData()
        {
            dgvOrders.Columns["orderId"].HeaderText = "Order ID";
            dgvOrders.Columns["orderType"].HeaderText = "Order Type";
            dgvOrders.Columns["orderDate"].HeaderText = "Order Date";
            dgvOrders.Columns["orderStatus"].HeaderText = "Order Status";
            dgvOrders.Columns["clientId"].HeaderText = "Client ID";
        }

        private string GenerateOrderId()
        {
            string newId = "O001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 orderId FROM Orders ORDER BY orderId DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();
                            int numericPart = int.Parse(lastId.Substring(1));
                            newId = "O" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating Order ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }

        private void LoadOrdersIntoDGV()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Orders";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvOrders.DataSource = dataTable;

                    /*if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No data found in the Orders table.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    */
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearOrderInputs()
        {
            cmbClientId.SelectedIndex = -1;
            cmbOrderType.SelectedIndex = -1;
            cmbOrderStatus.SelectedIndex = -1;
            txtOClientName.Clear();
        }

      

        private void addbtnord_Click(object sender, EventArgs e)
        {
            if (cmbClientId.SelectedIndex == -1 || cmbOrderType.SelectedIndex == -1 || cmbOrderStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string orderId = GenerateOrderId();
                string orderDate = DateTime.Now.ToString("yyyy-MM-dd");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Orders (orderId, orderType, orderDate, orderStatus, clientId) " +
                                   "VALUES (@orderId, @orderType, @orderDate, @orderStatus, @clientId)";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@orderType", cmbOrderType.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@orderDate", orderDate);
                    command.Parameters.AddWithValue("@orderStatus", cmbOrderStatus.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@clientId", cmbClientId.SelectedItem.ToString());

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Order added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadOrdersIntoDGV();
                        ClearOrderInputs();
                    }
                    else
                    {
                        MessageBox.Show("Failed to add order.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clrbtnord_Click(object sender, EventArgs e)
        {
            ClearOrderInputs();
        }

        private void updbtnord_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an order to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvOrders.SelectedRows[0];
            string orderId = selectedRow.Cells["orderId"].Value.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Orders SET orderType = @orderType, orderStatus = @orderStatus, clientId = @clientId WHERE orderId = @orderId";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@orderId", orderId);
                    command.Parameters.AddWithValue("@orderType", cmbOrderType.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@orderStatus", cmbOrderStatus.SelectedItem.ToString());
                    command.Parameters.AddWithValue("@clientId", cmbClientId.SelectedItem.ToString());

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Order updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadOrdersIntoDGV();
                        ClearOrderInputs();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update order.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rembtnord_Click(object sender, EventArgs e)
        {
            if (dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an order to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvOrders.SelectedRows[0];
            string orderId = selectedRow.Cells["orderId"].Value.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Orders WHERE orderId = @orderId";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@orderId", orderId);

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Order deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadOrdersIntoDGV();
                        ClearOrderInputs();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete order.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

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

     

        private void guna2TextBox11_TextChanged(object sender, EventArgs e)
        {

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

       


        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void custask_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = custask.Rows[e.RowIndex];

                txtTaskNames.Text = selectedRow.Cells["taskName"].Value.ToString();
                txtDeadline.Text = Convert.ToDateTime(selectedRow.Cells["deadline"].Value).ToString("yyyy-MM-dd");
                cmbStatuss.SelectedItem = selectedRow.Cells["status"].Value.ToString();
                txtPriority.Text = selectedRow.Cells["priority"].Value.ToString(); 
            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }

        private void custaskupd_Click(object sender, EventArgs e)
        {
            try
            {
                string taskId = custask.CurrentRow.Cells["taskId"].Value.ToString(); 
                string taskName = txtTaskNames.Text;
                string deadline = txtDeadline.Text; 
                string status = cmbStatuss.SelectedItem.ToString();
                string priority = txtPriority.Text;

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
            txtTaskNames.Text = string.Empty; // Clear task name text box
            txtDeadline.Text = string.Empty; // Clear deadline text box
            cmbStatuss.SelectedIndex = -1; // Reset combo box selection
            txtPriority.Text = string.Empty; // Clear priority text box
        }

        private void guna2TileButton5_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// //////////////////////////////////////////////
        /// </summary>
      
        //dashboard



        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void empCount_TextChanged(object sender, EventArgs e)
        {

        }

        private void clientcount_TextChanged(object sender, EventArgs e)
        {

        }

        private void ordercount_TextChanged(object sender, EventArgs e)
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
  //                      guna2HtmlToolTip3.SetToolTip(guna2TextBox6, "Meeting Title : " + meetingTitle);


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
    //                guna2HtmlToolTip3.SetToolTip(guna2TextBox1, supplierCount.ToString() + " suppliers have registered in the system");
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
      //                      guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending.\n " + overdueTaskCount + " of" + pendingTaskCount + " are overdue.");

                        }
                        else if (pendingTaskCount > 0 && overdueTaskCount <= 0)
                        {
        //                    guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending");

                        }
                        else if (pendingTaskCount <= 0)
                        {

          //                  guna2HtmlToolTip6.SetToolTip(guna2TextBox4, "You have no tasks to complete");
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
            //                guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is Rs" + finalAmount.ToString() + ".00");

                        }
                        else
                        {
                            guna2TextBox2.Text = "Rs 0.00";
              //              guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is not calculated yet");

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
    

    
