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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FinalProject
{
    public partial class Storekeeper : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;
        public Storekeeper(string userName)
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
            StorekeeperP mainForm = new StorekeeperP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
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
            StorekeeperP mainForm = new StorekeeperP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        //
        private void Storekeeper_Load(object sender, EventArgs e)
        {
            LoadSupplierIds();
            LoadDataIntoDGV();
            datebox.Value = DateTime.Now;

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
                        //    guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending");

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
                        //    guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is Rs" + finalAmount.ToString() + ".00");

                        }
                        else
                        {
                            guna2TextBox2.Text = "Rs 0.00";
                          //  guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is not calculated yet");

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


        private void supcmbbox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (supcmbbox.SelectedItem != null)
                {
                    string selectedSupplierId = supcmbbox.SelectedItem.ToString();

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "SELECT supplierName FROM Supplier WHERE supplierId = @supplierId";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@supplierId", selectedSupplierId);

                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();

                            if (reader.Read())
                            {
                                supnamebox.Text = reader["supplierName"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Supplier not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadSupplierIds()
        {
            try
            {
                supcmbbox.Items.Clear();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT supplierId FROM Supplier"; 

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                supcmbbox.Items.Add(reader["supplierId"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateInventoryId()
        {
            string newId = "INV001"; 

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 inventoryId " +
                                   "FROM Inventory " +
                                   "ORDER BY CAST(SUBSTRING(inventoryId, 4, LEN(inventoryId) - 3) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            string lastId = result.ToString();

                            int numericPart = int.Parse(lastId.Substring(3)); 
                            newId = "INV" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Inventory ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }


        private void ClearInputs()
        {
            supcmbbox.SelectedIndex = -1; 
            typecombo.SelectedIndex = -1; 
            supnamebox.Clear(); 
            productbox.Clear(); 
            qtybox.Value = qtybox.Minimum; 
            datebox.Value = DateTime.Now;

        }


        private void addbtn_Click(object sender, EventArgs e)
        {
            string inventoryId = GenerateInventoryId();


            if (typecombo.SelectedItem == null)
            {
                MessageBox.Show("Please select a type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(productbox.Text.Trim()))
            {
                MessageBox.Show("Please enter a product name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (qtybox.Value <= 0)
            {
                MessageBox.Show("Please enter a quantity greater than 0.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (datebox.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Please select a valid date (not in the past).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (supcmbbox.SelectedItem == null)
            {
                MessageBox.Show("Please select a supplier.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Collect data from input controls
                    string supplierId = supcmbbox.SelectedItem.ToString();
                    string inventoryType = typecombo.SelectedItem.ToString();
                    string productName = productbox.Text.Trim();
                    int availableQty = (int)qtybox.Value;
                    DateTime productAddedDate = datebox.Value;

                    while (true)
                    {
                        try
                        {
                            // SQL Query to insert data into the database
                            string query = "INSERT INTO Inventory (inventoryId, supplierId, inventoryType, productName, availableQty, productAddedDate) " +
                                           "VALUES (@inventoryId, @supplierId, @inventoryType, @productName, @availableQty, @productAddedDate);";

                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@inventoryId", inventoryId);
                            cmd.Parameters.AddWithValue("@supplierId", supplierId);
                            cmd.Parameters.AddWithValue("@inventoryType", inventoryType);
                            cmd.Parameters.AddWithValue("@productName", productName);
                            cmd.Parameters.AddWithValue("@availableQty", availableQty);
                            cmd.Parameters.AddWithValue("@productAddedDate", productAddedDate);

                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show($"Inventory added successfully! Inventory ID: {inventoryId}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                // Exit the loop after successful insertion
                                LoadDataIntoDGV();
                                ClearInputs();

                                return;
                            }
                        }
                        catch (SqlException ex) when (ex.Number == 2627) // Handle duplicate key error
                        {
                            inventoryId = GenerateInventoryId(); // Generate a new ID and retry
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearbtn_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }
        private void LoadDataIntoDGV()
        {
            try
            {
                if (dgvinventory == null)
                {
                    MessageBox.Show("DataGridView is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Inventory";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvinventory.DataSource = dataTable;

                    dgvinventory.Columns["inventoryId"].Width = 55;
                    dgvinventory.Columns["supplierId"].Width = 45;
                    dgvinventory.Columns["availableQty"].Width = 25;
                    dgvinventory.Columns["productAddedDate"].Width =65;
                    dgvinventory.Columns["inventoryType"].Width =80;


                    dgvinventory.Columns["inventoryId"].HeaderText = "Id";
                    dgvinventory.Columns["supplierId"].HeaderText = "Sup Id";
                    dgvinventory.Columns["productName"].HeaderText = "Name";
                    dgvinventory.Columns["inventoryType"].HeaderText = "Type";
                    dgvinventory.Columns["availableQty"].HeaderText = "Qty";
                    dgvinventory.Columns["productAddedDate"].HeaderText = "Product Added Date";

                    dgvinventory.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dgvinventory.Columns["productName"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvinventory.Columns["inventoryType"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;


                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No data found in the Inventory table.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updatebtn_Click(object sender, EventArgs e)
        {
            if (dgvinventory.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an inventory item to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvinventory.SelectedRows[0];
            string inventoryId = selectedRow.Cells["inventoryId"].Value.ToString();

            // Validate inputs
            if (typecombo.SelectedItem == null || string.IsNullOrEmpty(productbox.Text.Trim()) || qtybox.Value <= 0 ||
                datebox.Value.Date < DateTime.Now.Date || supcmbbox.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Collect data from input controls
                    string supplierId = supcmbbox.SelectedItem.ToString();
                    string inventoryType = typecombo.SelectedItem.ToString();
                    string productName = productbox.Text.Trim();
                    int availableQty = (int)qtybox.Value;
                    DateTime productAddedDate = datebox.Value;

                    // SQL Query to update the inventory record
                    string query = "UPDATE Inventory " +
                                   "SET supplierId = @supplierId, inventoryType = @inventoryType, productName = @productName, " +
                                   "availableQty = @availableQty, productAddedDate = @productAddedDate " +
                                   "WHERE inventoryId = @inventoryId";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@supplierId", supplierId);
                    cmd.Parameters.AddWithValue("@inventoryType", inventoryType);
                    cmd.Parameters.AddWithValue("@productName", productName);
                    cmd.Parameters.AddWithValue("@availableQty", availableQty);
                    cmd.Parameters.AddWithValue("@productAddedDate", productAddedDate);
                    cmd.Parameters.AddWithValue("@inventoryId", inventoryId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Inventory updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDGV(); // Refresh the DataGridView
                        ClearInputs(); // Clear input fields
                    }
                    else
                    {
                        MessageBox.Show("Failed to update inventory. Inventory item not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (dgvinventory.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an inventory item to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvinventory.SelectedRows[0];
            string inventoryId = selectedRow.Cells["inventoryId"].Value.ToString();

            var confirmResult = MessageBox.Show("Are you sure you want to delete this inventory item?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmResult == DialogResult.No)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "DELETE FROM Inventory WHERE inventoryId = @inventoryId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@inventoryId", inventoryId);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        dgvinventory.Rows.Remove(selectedRow);
                        MessageBox.Show("Inventory item deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadDataIntoDGV();  
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Inventory item not found in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void dgvinventory_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvinventory.Rows[e.RowIndex];

                string inventoryId = selectedRow.Cells["inventoryId"].Value.ToString();
                string productName = selectedRow.Cells["productName"].Value.ToString();
                string inventoryType = selectedRow.Cells["inventoryType"].Value.ToString();
                string supplierId = selectedRow.Cells["supplierId"].Value.ToString();
                int availableQty = Convert.ToInt32(selectedRow.Cells["availableQty"].Value);
                DateTime productAddedDate = Convert.ToDateTime(selectedRow.Cells["productAddedDate"].Value);

                productbox.Text = productName;
                typecombo.Text = inventoryType;
                supcmbbox.Text = supplierId;
                qtybox.Text = availableQty.ToString();
                datebox.Value = productAddedDate;
            }
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

        private void acctask_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
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
        private void guna2Button8_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button7_Click(object sender, EventArgs e)
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

        private void guna2Button13_Click(object sender, EventArgs e)
        {
            InvR reportForm = new InvR();

            reportForm.GenerateReport();

            // Show the report form
            reportForm.ShowDialog();
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }
    }
}

    
