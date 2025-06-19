using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject
{
    public partial class InventoryReq : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        string loggedinuser = "";

        public InventoryReq(string userName)
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
            Factorymanager mainForm = new Factorymanager(loggedinuser);
            mainForm.Show();
            this.Hide();
        }
        private void LoadShortageData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // SQL query to fetch all shortage data
                    string query = "SELECT productId, productName, requiredQty, availableQty FROM dbo.shortage";  // Modify the query if needed

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    // Set the DataGridView's data source to the fetched data
                    dgvShortage.DataSource = dt;  // Assuming dgvShortage is your DataGridView control
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading shortage data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void InventoryReq_Load(object sender, EventArgs e)
        {
            LoadInventoryData1();
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            FactoryManagerPr mainForm = new FactoryManagerPr(currentUsername);
            mainForm.Show();
            this.Hide();
        }


        //
        //
        //

        private void ClearInputs()
        {
            guna2NumericUpDown2.Value = 0; // Set to default value, or you can set to any other number you prefer

            // Clear other controls like ComboBoxes and TextBoxes
            Requiredtext.Clear(); // Clear TextBox
            Shortage.Clear();
            txttotal.Clear();// Clear TextBox


        }


        private void cmbInventoryType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "";
            ClearInputs();

            // Determine the category selected in the ComboBox.
            if (cmbInventoryType.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a category (Tool, Raw Material, Item).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Adjust the SQL query based on the selected category.
                switch (cmbInventoryType.SelectedItem.ToString().ToLower())
                {
                    case "tool":
                        query = @"
                    SELECT DISTINCT TRIM(UPPER(toolName)) AS ProductName 
                    FROM Tools";
                        break;

                    case "raw material":
                        query = @"
                    SELECT DISTINCT TRIM(UPPER(rawmaterialName)) AS ProductName 
                    FROM Rawmaterials";
                        break;

                    case "item":
                        query = @"
                    SELECT DISTINCT TRIM(UPPER(itemName)) AS ProductName 
                    FROM Items";
                        break;

                    default:
                        MessageBox.Show("Invalid category selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    // Clear existing items in ComboBox
                    cmbProductName.Items.Clear();

                    // Add each product name to the ComboBox
                    while (reader.Read())
                    {
                        cmbProductName.Items.Add(reader["ProductName"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product names: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void LoadProductDetails(string productId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT productName, requiredQty, availableQty FROM dbo.shortage WHERE productId = @productId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productId", productId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string productName = reader["productName"].ToString(); // Assuming this is already in uppercase
                        int requiredQty = Convert.ToInt32(reader["requiredQty"]);
                        int availableQty = Convert.ToInt32(reader["availableQty"]);

                        // Check if productName belongs to any of the three tables
                        string itemType = GetProductType(productName);

                        // Update ComboBox1 with the itemType
                        if (itemType != null)
                        {
                            cmbInventoryType.SelectedItem = itemType;
                        }
                        else
                        {
                            MessageBox.Show("Product not found in any relevant tables.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Optionally, update other controls with this data
                        cmbProductName.SelectedItem = productName;
                        // txtRequiredQty.Text = requiredQty.ToString();
                        // txtAvailableQty.Text = availableQty.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Product details not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL specific exceptions
                MessageBox.Show($"SQL Error: {sqlEx.Message}\n\n{sqlEx.StackTrace}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException invOpEx)
            {
                // Handle InvalidOperationException specific exceptions
                MessageBox.Show($"Invalid Operation: {invOpEx.Message}\n\n{invOpEx.StackTrace}", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\n{ex.StackTrace}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to check which table the productName belongs to
        private string GetProductType(string productName)
        {
            string itemType = cmbInventoryType.SelectedItem.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Check in Rawmaterials table (using UPPER function for case-insensitive comparison)
                    string rawMaterialsQuery = "SELECT COUNT(*) FROM dbo.Rawmaterials WHERE UPPER(rawmaterialName) = @productName";
                    SqlCommand command = new SqlCommand(rawMaterialsQuery, connection);
                    command.Parameters.AddWithValue("@productName", productName);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    if (count > 0)
                    {
                        itemType = "Raw material";
                    }
                    else
                    {
                        // Check in Tools table (using UPPER function for case-insensitive comparison)
                        string toolsQuery = "SELECT COUNT(*) FROM dbo.Tools WHERE UPPER(toolName) = @productName";
                        command = new SqlCommand(toolsQuery, connection);
                        command.Parameters.AddWithValue("@productName", productName);

                        count = Convert.ToInt32(command.ExecuteScalar());

                        if (count > 0)
                        {
                            itemType = "Tool";
                        }
                        else
                        {
                            // Check in Items table (using UPPER function for case-insensitive comparison)
                            string itemsQuery = "SELECT COUNT(*) FROM dbo.Items WHERE UPPER(itemName) = @productName";
                            command = new SqlCommand(itemsQuery, connection);
                            command.Parameters.AddWithValue("@productName", productName);

                            count = Convert.ToInt32(command.ExecuteScalar());

                            if (count > 0)
                            {
                                itemType = "Item";
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL Error: {sqlEx.Message}\n\n{sqlEx.StackTrace}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\n{ex.StackTrace}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return itemType;
        }

        private void LoadInventoryData()
        {
            string connectionString = "YourConnectionStringHere";
            string query = "SELECT inventoryId, supplierId, inventoryType, productName, availableQty, productAddedDate, requiredQty FROM Inventory";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvShortage.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            LoadInventoryData1();
        }

        public void LoadInventoryData1()
        {
            // Connection string (adjust as needed)

            // SQL query to get inventory where supplierId is NULL
            string query = "SELECT * FROM Inventory WHERE supplierId IS NULL";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the database connection
                    connection.Open();

                    // Create a SqlDataAdapter to execute the query and fill the data
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                    // Create a DataTable to hold the query result
                    DataTable dataTable = new DataTable();

                    // Fill the DataTable with data from the database
                    dataAdapter.Fill(dataTable);

                    // Bind the DataTable to the DataGridView
                    dgvShortage.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }

        private void dgvinventory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            // Ensure that the first column (0 index) was clicked
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                try
                {
                    // Get the inventoryId from the first column of the clicked row
                    string inventoryId = dgvShortage.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();

                    // SQL query to fetch the shortage data for the selected inventoryId
                    string query = @"
                select*from shortage

                WHERE inventoryId = @inventoryId";

                    // Database connection
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            // Add parameters to avoid SQL injection
                            command.Parameters.AddWithValue("@inventoryId", inventoryId);

                            // Execute the query and get the result
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Populate relevant fields with the data from the Shortage table
                                    cmbInventoryType.SelectedItem = reader["type"].ToString();
                                    cmbProductName.SelectedItem = reader["productName"].ToString();
                                    Requiredtext.Text = reader["requiredQty"].ToString();
                                    AvailableQtyTextBox.Text = reader["availableQty"].ToString();
                                    guna2NumericUpDown2.Value = Convert.ToDecimal(reader["extraQty"]); // Correct assignment for NumericUpDown
                                    txttotal.Text = reader["totalQty"].ToString();



                                }
                                else
                                {
                                    MessageBox.Show("No shortage data found for the selected inventory.", "Data Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database error: {sqlEx.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
              }
            } 

        private void cmbProductName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string query = "";
            string productName = cmbProductName.SelectedItem.ToString().Trim().ToUpper(); // Get selected product name

            if (string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Please select a product.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Determine the category selected and adjust the query accordingly
                switch (cmbInventoryType.SelectedItem.ToString().ToLower())
                {
                    case "tool":
                        query = @"
                    SELECT SUM(quantityNeeded) AS TotalQuantity
                    FROM Tools
                    WHERE TRIM(UPPER(toolName)) = @productName";
                        break;

                    case "raw material":
                        query = @"
                    SELECT SUM(quantityNeeded) AS TotalQuantity
                    FROM Rawmaterials
                    WHERE TRIM(UPPER(rawmaterialName)) = @productName";
                        break;

                    case "item":
                        query = @"
                    SELECT SUM(itemQuantity) AS TotalQuantity
                    FROM Items
                    WHERE TRIM(UPPER(itemName)) = @productName";
                        break;

                    default:
                        MessageBox.Show("Invalid category selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productName", productName);  // Avoid SQL injection

                    connection.Open();

                    var result = command.ExecuteScalar();

                    // Display the sum of quantities in the txtQuantityNeeded TextBox
                    if (result != DBNull.Value)
                    {
                        Requiredtext.Text = result.ToString();
                        CheckInventoryAndCalculateShortage();
                    }
                    else
                    {
                        Requiredtext.Text = "0";  // No matching products found
                        CheckInventoryAndCalculateShortage();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product quantity: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2HtmlLabel7_Click(object sender, EventArgs e)
        {

        }
        // This method checks if the selected product is in the inventory and calculates shortage if necessary
        private void CheckInventoryAndCalculateShortage()
        {
            string selectedProductName = cmbProductName.SelectedItem.ToString().Trim(); // Get selected product name from ComboBox
            decimal requiredQty = Convert.ToDecimal(Requiredtext.Text); // Get the required quantity from the text field

            if (string.IsNullOrEmpty(selectedProductName))
            {
                MessageBox.Show("Please select a product from the list.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // SQL query to get the sum of available quantity for the selected product
                string query = @"
        SELECT SUM(availableQty) AS totalAvailableQty
        FROM [finalPJS].[dbo].[Inventory]
        WHERE UPPER(TRIM(productName)) = UPPER(TRIM(@productName))";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productName", selectedProductName); // Avoid SQL injection

                    connection.Open();

                    var result = command.ExecuteScalar(); // Execute query and get the result

                    // If the product exists in the inventory
                    if (result != DBNull.Value)
                    {
                        decimal totalAvailableQty = Convert.ToDecimal(result);

                        // Set the total available quantity in the text box
                        AvailableQtyTextBox.Text = totalAvailableQty.ToString("0.##");

                        // Calculate shortage or set to 0 if there is no shortage
                        decimal shortage = (totalAvailableQty < requiredQty) ? requiredQty - totalAvailableQty : 0;

                        // Set the shortage value in the text field
                        Shortage.Text = shortage.ToString("0.##");
                    }
                    else
                    {
                        // If the product is not found in the inventory
                        AvailableQtyTextBox.Text = "0"; // No available quantity
                        Shortage.Text = requiredQty.ToString("0.##"); // Set shortage to required quantity
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking inventory and calculating shortage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Shortage_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }



        private void UpdateShortageAndTotal()
        {
            // Get the value from NumericUpDown
            decimal quantityNeeded = guna2NumericUpDown2.Value;

            // Get the current shortage value from the txtShortage (if it already has some value)
            decimal shortage = decimal.Parse(Shortage.Text);



            // Update the txtShortage text box
            Shortage.Text = shortage.ToString();

            // Calculate the total value (assuming it's the same as shortage or based on your calculation logic)
            decimal total = shortage + quantityNeeded;  // You can modify this logic based on how the total is calculated.

            // Update the txtTotal text box
            txttotal.Text = total.ToString();
        }

        private void guna2NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            UpdateShortageAndTotal();
        }

        private void Requiredtext_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private decimal GetTotalAvailableQty(string selectedProductName)
        {
            decimal totalAvailableQty = 0; // Variable to store total available quantity

            if (string.IsNullOrEmpty(selectedProductName))
            {
                MessageBox.Show("Please select a product from the list.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return totalAvailableQty;
            }

            try
            {
                // SQL query to get the sum of available quantity for the selected product
                string query = @"
            SELECT SUM(availableQty) AS totalAvailableQty
            FROM [finalPJS].[dbo].[Inventory]
            WHERE UPPER(TRIM(productName)) = UPPER(TRIM(@productName))";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productName", selectedProductName); // Avoid SQL injection

                    connection.Open();

                    var result = command.ExecuteScalar(); // Execute query and get the result

                    // If the product exists in the inventory
                    if (result != DBNull.Value)
                    {
                        totalAvailableQty = Convert.ToDecimal(result);
                    }
                    else
                    {
                        // If the product is not found in the inventory, set total available quantity to 0
                        totalAvailableQty = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking inventory and retrieving availability: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return totalAvailableQty;
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

        private void DeleteInventory()
        {
            try
            {
                // Confirm with the user before deleting
                var confirmResult = MessageBox.Show("Are you sure you want to delete this inventory record?",
                                                     "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.Yes)
                {
                    // SQL query to delete from both Inventory and Shortage tables using a transaction
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // Begin transaction
                        using (SqlTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                string inventoryId = dgvShortage.SelectedRows[0].Cells[0].Value.ToString().Trim();

                               
                                // Delete from Shortage table
                                string deleteShortageQuery = "DELETE FROM Shortage WHERE inventoryId = @inventoryId";
                                using (SqlCommand command = new SqlCommand(deleteShortageQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@inventoryId", inventoryId);
                                    command.ExecuteNonQuery();
                                }


                                // Delete from Inventory table
                                string deleteInventoryQuery = "DELETE FROM Inventory WHERE inventoryId = @inventoryId";
                                using (SqlCommand command = new SqlCommand(deleteInventoryQuery, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@inventoryId", inventoryId);
                                    command.ExecuteNonQuery();
                                }


                                // Commit the transaction
                                transaction.Commit();

                                // Inform the user and refresh the DataGridView
                                MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadInventoryData1(); // You can call a method to refresh your DataGridView
                            }
                            catch (Exception ex)
                            {
                                // Rollback the transaction on failure
                                transaction.Rollback();
                                MessageBox.Show($"Error deleting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void ClearInputs1()
        {
            // Clear textboxes
            cmbInventoryType.SelectedIndex = -1; // Reset ComboBox selection
            cmbProductName.SelectedIndex = -1;  // Reset ComboBox selection
            AvailableQtyTextBox.Clear();
            Requiredtext.Clear();
            txttotal.Clear();

            // Reset numeric up-down control
            guna2NumericUpDown2.Value = 0;

            // Optionally, reset other controls like checkboxes, date pickers, etc.
            // Example: dateTimePicker1.Value = DateTime.Now; (if you have any date pickers)

            // If you are using any error labels or status messages, reset them
           // lblStatus.Text = string.Empty;
        }

        private void InsertIntoInventory()
        {
            try
            {
                // Retrieve values from text fields
                string inventoryId = GenerateInventoryId();
                string inventoryType = string.IsNullOrWhiteSpace(cmbInventoryType.Text) ? null : cmbInventoryType.Text.Trim();
                string productName = string.IsNullOrWhiteSpace(cmbProductName.Text) ? null : cmbProductName.Text.Trim();
                int? availableQty = string.IsNullOrWhiteSpace(AvailableQtyTextBox.Text) ? (int?)null : int.Parse(AvailableQtyTextBox.Text.Trim());
                int? requiredQty = string.IsNullOrWhiteSpace(txttotal.Text) ? (int?)null : int.Parse(txttotal.Text.Trim());

                // Validate required fields
                if (string.IsNullOrEmpty(inventoryId) || string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(inventoryType))
                {
                    MessageBox.Show("Please fill in all required fields (Inventory ID, Product Name, Type).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // SQL query to insert data into the Inventory table
                string query = @"
        INSERT INTO Inventory (inventoryId, supplierId, inventoryType, productName, availableQty, productAddedDate, requiredQty)
        VALUES (@inventoryId, @supplierId, @inventoryType, @productName, @availableQty, GETDATE(), @requiredQty)";

                // Database connection and transaction
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand command = new SqlCommand(query, connection, transaction))
                            {
                                // Add parameters to avoid SQL injection
                                command.Parameters.AddWithValue("@inventoryId", inventoryId);
                                command.Parameters.AddWithValue("@supplierId", DBNull.Value); // Replace with actual supplier ID if available
                                command.Parameters.AddWithValue("@inventoryType", (object)inventoryType ?? DBNull.Value);
                                command.Parameters.AddWithValue("@productName", (object)productName ?? DBNull.Value);
                                command.Parameters.AddWithValue("@availableQty", (object)availableQty ?? DBNull.Value);
                                command.Parameters.AddWithValue("@requiredQty", (object)requiredQty ?? DBNull.Value);

                                // Execute the query
                                command.ExecuteNonQuery();
                            }

                            // Insert into Shortage table
                            if (availableQty.HasValue && requiredQty.HasValue)
                            {
                                InsertIntoShortage(inventoryId, inventoryType, productName, availableQty.Value, requiredQty.Value, transaction, connection);
                            }

                            // Commit the transaction
                            transaction.Commit();

                            MessageBox.Show("Inventory and shortage records added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch
                        {
                            // Rollback the transaction on failure
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertIntoShortage(string inventoryId, string type, string productName, int availableQty, int requiredQty, SqlTransaction transaction, SqlConnection connection)
        {
            try
            {
                // Calculate extraQty and totalQty
                int extraQty = availableQty > requiredQty ? availableQty - requiredQty : 0;
                int totalQty = requiredQty + extraQty;

                // SQL query to insert data into the Shortage table
                string query = @"
        INSERT INTO Shortage (inventoryId, type, productName, requiredQty, availableQty, extraQty, totalQty)
        VALUES (@inventoryId, @type, @productName, @requiredQty, @availableQty, @extraQty, @totalQty)";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    // Add parameters to avoid SQL injection
                    command.Parameters.AddWithValue("@inventoryId", inventoryId);
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@productName", productName);
                    command.Parameters.AddWithValue("@requiredQty", requiredQty);
                    command.Parameters.AddWithValue("@availableQty", availableQty);
                    command.Parameters.AddWithValue("@extraQty", extraQty);
                    command.Parameters.AddWithValue("@totalQty", totalQty);

                    // Execute the query
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                throw; // Let the calling method handle the exception
            }
        }


        private void UpdateInventoryAndShortage()
        {
            try
            {
                // Retrieve values from text fields
                string inventoryId = dgvShortage.SelectedRows[0].Cells[0].Value.ToString().Trim();
               // string supplierId = string.IsNullOrWhiteSpace(txtSupplierId.Text) ? null : txtSupplierId.Text.Trim();
                string inventoryType = string.IsNullOrWhiteSpace(cmbInventoryType.Text) ? null : cmbInventoryType.Text.Trim();
                string productName = string.IsNullOrWhiteSpace(cmbProductName.Text) ? null : cmbProductName.Text.Trim();
                int? availableQty = string.IsNullOrWhiteSpace(AvailableQtyTextBox.Text) ? (int?)null : int.Parse(AvailableQtyTextBox.Text.Trim());
                int? requiredQty = string.IsNullOrWhiteSpace(txttotal.Text) ? (int?)null : int.Parse(txttotal.Text.Trim());
              //  DateTime productAddedDate = dtpProductAddedDate.Value;

                // Validate required fields
                if (string.IsNullOrEmpty(inventoryId))
                {
                    MessageBox.Show("Inventory ID is required for updating.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // SQL query to update the Inventory and Shortage tables
                string query = @"
        -- Update Inventory Table
        UPDATE Inventory
        SET
            inventoryType = @inventoryType,
            productName = @productName,
            availableQty = @availableQty,
            
            requiredQty = @requiredQty
        WHERE inventoryId = @inventoryId;

        -- Update Shortage Table
        UPDATE Shortage
        SET availableQty = @availableQty,
            extraQty = CASE WHEN @availableQty > @requiredQty THEN @availableQty - @requiredQty ELSE 0 END,
            totalQty = @requiredQty + CASE WHEN @availableQty > @requiredQty THEN @availableQty - @requiredQty ELSE 0 END
        WHERE inventoryId = @inventoryId;";

                // Database connection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@inventoryId", inventoryId);
                     //   command.Parameters.AddWithValue("@supplierId", (object)supplierId ?? DBNull.Value);
                        command.Parameters.AddWithValue("@inventoryType", (object)inventoryType ?? DBNull.Value);
                        command.Parameters.AddWithValue("@productName", (object)productName ?? DBNull.Value);
                        command.Parameters.AddWithValue("@availableQty", (object)availableQty ?? DBNull.Value);
                     //   command.Parameters.AddWithValue("@productAddedDate", productAddedDate);
                        command.Parameters.AddWithValue("@requiredQty", (object)requiredQty ?? DBNull.Value);

                        // Execute the query
                        int rowsAffected = command.ExecuteNonQuery();

                        // Check if any rows were updated
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Inventory and Shortage records updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadInventoryData1();
                        }
                        else
                        {
                            MessageBox.Show("No record found with the specified Inventory ID.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "SQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void addbtninv_Click(object sender, EventArgs e)
        {
            InsertIntoInventory();
            LoadInventoryData1();
            ClearInputs1();
        }

        private void guna2HtmlLabel8_Click(object sender, EventArgs e)
        {

        }

        private void updbtninv_Click(object sender, EventArgs e)
        {
            UpdateInventoryAndShortage();
            ClearInputs1();

        }

        private void rembtninv_Click(object sender, EventArgs e)
        {

            DeleteInventory();
            ClearInputs1();



        }

        private void clrbtninv_Click(object sender, EventArgs e)
        {
            ClearInputs1();
        }

        private void txttotal_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel16_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel19_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void AvailableQtyTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
