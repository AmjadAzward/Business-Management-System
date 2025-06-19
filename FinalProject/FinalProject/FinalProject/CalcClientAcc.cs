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
    public partial class CalcClientAcc : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;

        string loggedinuser = "";
        public CalcClientAcc(string userName)
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

        private void CalcClientAcc_Load(object sender, EventArgs e)
        {
            LoadClientIds();  // Populate clientIdCombo
            LoadOrderIds();
            LoadClientPaymentsData();
        }

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            AccountantP mainForm = new AccountantP(currentUsername);
            mainForm.Show();
            this.Hide();
        }
        private void LoadClientIds()
        {
            // Connection string to your database

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT DISTINCT clientId FROM Project";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    // Clear the combo box before adding items
                    clientidcombo.SelectedIndex = -1; // Deselect any selected item

                    while (reader.Read())
                    {
                        string clientId = reader["clientId"].ToString();
                        clientidcombo.Items.Add(clientId);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading client IDs: " + ex.Message);
                }

            }
        }
        private void LoadOrderIds()
        {
            // Connection string to your database

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT DISTINCT orderId FROM Project";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    // Clear the combo box before adding items
                    ordercombo.Items.Clear();

                    while (reader.Read())
                    {
                        string orderId = reader["orderId"].ToString();
                        ordercombo.Items.Add(orderId);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading order IDs: " + ex.Message);
                }
            }
        }
        private void LoadClientName(string clientId)
        {
            // Connection string to your database

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT clientName FROM Client WHERE clientId = @clientId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@clientId", clientId);

                    var clientName = cmd.ExecuteScalar();

                    if (clientName != null)
                    {
                        clientbox.Text = clientName.ToString(); // Display client name in the textbox
                    }
                    else
                    {
                        MessageBox.Show("Client not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading client name: " + ex.Message);
                }
            }
        }
        private void LoadProjectIdFromOrderId(string orderId)
        {
            // Connection string to your database

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT projectId FROM Project WHERE orderId = @orderId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@orderId", orderId);

                    var result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        string projectId = result.ToString(); // Convert object to string
                        LoadProjectTotalCost(projectId); // Call method to load project total cost
                    }
                    else
                    {
                        MessageBox.Show("No project found for the selected order ID.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading project ID: " + ex.Message);
                }
            }
        }


        private void LoadProjectTotalCost(string projectId)
        {
            // Connection string to your database

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT projectTotalCost FROM Project WHERE projectId = @projectId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@projectId", projectId);

                    var projectTotalCost = cmd.ExecuteScalar();

                    if (projectTotalCost != null)
                    {

                        projectbox.Text = projectTotalCost.ToString(); // Display the total cost in projectBox
                    }
                    else
                    {
                        MessageBox.Show("Project not found or does not have a total cost.");
                        projectbox.Clear(); // Clear the text box if no result is found
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading project total cost: " + ex.Message);
                }
            }
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }
        private string GenerateClientPaymentId()
        {
            string newId = "CP001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 clientPaymentId FROM ClientPayments ORDER BY CAST(SUBSTRING(clientPaymentId, 3, LEN(clientPaymentId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();

                            // Extract numeric part and increment
                            int numericPart = int.Parse(lastId.Substring(2));
                            newId = "CP" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Client Payment ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }
        private void LoadClientPaymentsData()
        {
            // SQL query to fetch all records from the ClientPayments table
            string query = "SELECT clientPaymentId, projectCost, additionalCost, finalCost, clientPaymentStatus, clientPaymentDate, clientId, orderId FROM ClientPayments";

            // Using statement for resource management
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Create a DataAdapter to execute the query and fill the DataTable
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                    // Create a DataTable to hold the query results
                    DataTable dataTable = new DataTable();

                    // Fill the DataTable with the data from the database
                    dataAdapter.Fill(dataTable);

                    // Bind the DataTable to the DataGridView
                    dgvClientPayments.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    // Handle errors
                    MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(projectbox.Text) || !decimal.TryParse(projectbox.Text, out decimal projectCost))
            {
                MessageBox.Show("Please enter a valid project cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(additional.Text) || !decimal.TryParse(additional.Text, out decimal additionalCost))
            {
                MessageBox.Show("Please enter a valid additional cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Calculate final cost by adding projectCost and additionalCost
            decimal finalCost = projectCost + additionalCost;

            // Display the final cost in the textbox
            final1.Text = finalCost.ToString();

            // Validate that the client payment status is selected
            if (pstatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate that the client ID is selected
            if (clientidcombo.SelectedItem == null)
            {
                MessageBox.Show("Please select a client.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate that the order ID is selected
            if (ordercombo.SelectedItem == null)
            {
                MessageBox.Show("Please select an order.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate the payment date
            if (guna2DateTimePicker2.Value == null)
            {
                MessageBox.Show("Please select a valid payment date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // SQL query
            string query = "INSERT INTO ClientPayments (clientPaymentId, projectCost, additionalCost, finalCost, clientPaymentStatus, clientPaymentDate, clientId, orderId) " +
                           "VALUES (@clientPaymentId, @projectCost, @additionalCost, @finalCost, @clientPaymentStatus, @clientPaymentDate, @clientId, @orderId)";

            // Using statement for resource management
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters
                        command.Parameters.AddWithValue("@clientPaymentId", GenerateClientPaymentId());
                        command.Parameters.AddWithValue("@projectCost", projectCost);
                        command.Parameters.AddWithValue("@additionalCost", additionalCost);
                        command.Parameters.AddWithValue("@finalCost", finalCost);
                        command.Parameters.AddWithValue("@clientPaymentStatus", pstatus.SelectedItem);
                        command.Parameters.AddWithValue("@clientPaymentDate", guna2DateTimePicker2.Value);
                        command.Parameters.AddWithValue("@clientId", clientidcombo.SelectedItem);
                        command.Parameters.AddWithValue("@orderId", ordercombo.SelectedItem);

                        // Execute query
                        int rowsAffected = command.ExecuteNonQuery();

                        // Notify user
                        MessageBox.Show("Payment added successfully.");
                        LoadClientPaymentsData();
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }


        private void clientidcombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string clientId = clientidcombo.SelectedItem.ToString();
            LoadClientName(clientId);

        }

        private void ordercombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string orderId = ordercombo.SelectedItem.ToString();
            LoadProjectIdFromOrderId( orderId);
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {


            // Validate inputs
            if (string.IsNullOrEmpty(projectbox.Text) || !decimal.TryParse(projectbox.Text, out decimal projectCost))
            {
                MessageBox.Show("Please enter a valid project cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(additional.Text) || !decimal.TryParse(additional.Text, out decimal additionalCost))
            {
                MessageBox.Show("Please enter a valid additional cost.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Calculate final cost by adding projectCost and additionalCost
            decimal finalCost = projectCost + additionalCost;

            // Display the final cost in the textbox
            final1.Text = finalCost.ToString();

            // Validate that the client payment status is selected
            if (pstatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate that the client ID is selected
            if (clientidcombo.SelectedItem == null)
            {
                MessageBox.Show("Please select a client.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate that the order ID is selected
            if (ordercombo.SelectedItem == null)
            {
                MessageBox.Show("Please select an order.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate the payment date
            if (guna2DateTimePicker2.Value == null)
            {
                MessageBox.Show("Please select a valid payment date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // SQL query for updating the record
            string query = "UPDATE ClientPayments SET " +
                           "projectCost = @projectCost, " +
                           "additionalCost = @additionalCost, " +
                           "finalCost = @finalCost, " +
                           "clientPaymentStatus = @clientPaymentStatus, " +
                           "clientPaymentDate = @clientPaymentDate, " +
                           "clientId = @clientId, " +
                           "orderId = @orderId " +
                           "WHERE clientPaymentId = @clientPaymentId";

            // Using statement for resource management
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters
                        string clientPaymentId1 = dgvClientPayments.SelectedRows[0].Cells[0].Value.ToString();  // Assuming the first column contains clientPaymentId
                        command.Parameters.AddWithValue("@clientPaymentId", clientPaymentId1);

                        command.Parameters.AddWithValue("@projectCost", projectCost);
                        command.Parameters.AddWithValue("@additionalCost", additionalCost);
                        command.Parameters.AddWithValue("@finalCost", finalCost);
                        command.Parameters.AddWithValue("@clientPaymentStatus", pstatus.SelectedItem);
                        command.Parameters.AddWithValue("@clientPaymentDate", guna2DateTimePicker2.Value);
                        command.Parameters.AddWithValue("@clientId", clientidcombo.SelectedItem);
                        command.Parameters.AddWithValue("@orderId", ordercombo.SelectedItem);

                        // Execute query
                        int rowsAffected = command.ExecuteNonQuery();

                        // Notify user
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Payment record updated successfully.");
                            LoadClientPaymentsData();
                        }
                        else
                        {
                            MessageBox.Show("No record found with the provided Client Payment ID.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }

        }


        private void dgvClientPayments_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = dgvClientPayments.Rows[e.RowIndex];

                // Populate controls with data from the clicked row
                projectbox.Text = selectedRow.Cells["projectCost"].Value.ToString();
                additional.Text = selectedRow.Cells["additionalCost"].Value.ToString();
                final1.Text = selectedRow.Cells["finalCost"].Value.ToString();

                // Assuming pstatus is a ComboBox for payment status
                pstatus.SelectedItem = selectedRow.Cells["clientPaymentStatus"].Value.ToString();

                // Assuming clientidcombo is a ComboBox for client ID
                clientidcombo.SelectedItem = selectedRow.Cells["clientId"].Value.ToString();

                // Assuming ordercombo is a ComboBox for order ID
                ordercombo.SelectedItem = selectedRow.Cells["orderId"].Value.ToString();

                // Assuming clientPaymentDate is a DateTimePicker
                guna2DateTimePicker2.Value = Convert.ToDateTime(selectedRow.Cells["clientPaymentDate"].Value);
            }
            else
            {
                // If the clicked row is invalid, show a message
                MessageBox.Show("Please select a valid row.");
            }
        }
    }
}
