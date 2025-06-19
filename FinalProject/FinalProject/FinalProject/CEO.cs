using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Guna.UI2.WinForms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportSource;



namespace FinalProject
{
    public partial class CEO : Form
    {

        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;

        public CEO(string userName)
        {
            InitializeComponent();    
            this.Paint += RoundedForm_Paint;

            currentUsername = userName;



        }

        private void guna2TextBox1_Enter(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_Enter(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_Enter(object sender, EventArgs e)
        {

        }
        private void guna2TextBox4_Enter(object sender, EventArgs e)
        {

        }
        private void guna2TextBox5_Enter(object sender, EventArgs e)
        {

        }
        private void guna2TextBox6_Enter(object sender, EventArgs e)
        {

        }
        private void guna2TextBox7_Enter(object sender, EventArgs e)
        {
 
        }
        private void guna2TextBox8_Enter(object sender, EventArgs e)
        {
       
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
    


    private void Login_Load(object sender, EventArgs e)
        {
            LoadData();
        LoadSupplierData();


            LoadSalesData();

            LoadFeedbackData();


            GetNullResponseCount();
            //GetMostRecentMeeting(currentUsername);
            GetMostRecentMeetingForUser(currentUsername);
            CountEmployees();
            CountSuppliers();
            CountPastProjects();

        }

        private void guna2TextBox9_Enter(object sender, EventArgs e)
        {
         
        }

        private void guna2TextBox9_Leave(object sender, EventArgs e)
        {
         
        }

        private void guna2TextBox33_Enter(object sender, EventArgs e)
        {
         
        }

        private void guna2TextBox33_Leave(object sender, EventArgs e)
        {
      
        }

        private void guna2TextBox26_Enter(object sender, EventArgs e)
        {
        
        }

        private void guna2TextBox26_Leave(object sender, EventArgs e)
        {
         
        }

        private void guna2TextBox19_Enter(object sender, EventArgs e)
        {
        
        }

        private void guna2TextBox19_Leave(object sender, EventArgs e)
        {
       
        }

        private void guna2TextBox36_Enter(object sender, EventArgs e)
        {
        
        }

        private void guna2TextBox36_Leave(object sender, EventArgs e)
        {
          
        }




        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label01_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void guna2TileButton1_Click(object sender, EventArgs e)
        {
            
        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void guna2TileButton1_Click_1(object sender, EventArgs e)
        {
          
        }
     
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_MouseDown(object sender, MouseEventArgs e)
        {
                        
        }

        private void guna2Button3_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel3_DoubleClick(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel9_Click(object sender, EventArgs e)
        {

        }

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel14_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button12_Click(object sender, EventArgs e)
        {

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

        private void guna2TileButton6_Click(object sender, EventArgs e)
        {
            SalaryView mainForm = new SalaryView(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton7_Click(object sender, EventArgs e)
        {
            ClientView mainForm = new ClientView(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton8_Click(object sender, EventArgs e)
        {
            OrganizationalView mainForm = new OrganizationalView(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {
            CEOp mainForm = new CEOp(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2HtmlLabel23_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;

        }

        private void guna2HtmlLabel24_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox16_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox9_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel46_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox19_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox36_Leave_1(object sender, EventArgs e)
        {

        }

        private void guna2TextBox33_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox26_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox36_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel18_Click(object sender, EventArgs e)
        {

        }

        private void searchbox_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void ClearInputFields()
        {
            searchbox.Clear();
            fnamebox.Clear();
            lnamebox.Clear();
            emailbox.Clear();
            phonenobox.Clear();
        }

        private void searchbtn_Click(object sender, EventArgs e)
        {
            string searchText = searchbox.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                ClearInputFields(); // Clear textboxes if search is empty
                return;
            }

            string query = @"SELECT employeeId, firstName, lastName, email, phoneNumber 
                     FROM Employee
                     WHERE LOWER(employeeId) = LOWER(@searchText)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@searchText", searchText);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            fnamebox.Text = reader["firstName"].ToString();
                            lnamebox.Text = reader["lastName"].ToString();
                            emailbox.Text = reader["email"].ToString();
                            phonenobox.Text = reader["phoneNumber"].ToString();
                            LoadEmployeeImage(searchText);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No employee found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        ClearInputFields(); // Clear textboxes if no match found
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        public void LoadEmployeeImage(string employeeId)
        {
            string imagePath = Path.Combine("C:\\Users\\USER\\OneDrive\\Pictures\\project", employeeId + ".png");

            if (File.Exists(imagePath))
            {
                pictureBoxProfile.Image = Image.FromFile(imagePath);
            }
            else
            {
                // Set a default image if the employee's image is not found
                string defaultImagePath = "C:\\Users\\USER\\Desktop\\photos\\image-recognition (1).png";

                if (File.Exists(defaultImagePath))
                {
                    pictureBoxProfile.Image = Image.FromFile(defaultImagePath);
                }
                else
                {
                    pictureBoxProfile.Image = null; // Optionally, set it to null if the default image isn't found
                   // MessageBox.Show("Image not found, and default image is also missing.");
                }
            }
        }


        private void Eupdate_Click(object sender, EventArgs e)
        {
            // Get values from the textboxes
            string employeeId = searchbox.Text.Trim();
            string firstName = fnamebox.Text.Trim();
            string lastName = lnamebox.Text.Trim();
            string email = emailbox.Text.Trim();
            string phoneNumber = phonenobox.Text.Trim();

            // Validate that all fields are filled in
            if (string.IsNullOrEmpty(employeeId))
            {
                MessageBox.Show("Please enter the Employee ID.");
                searchbox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(firstName))
            {
                MessageBox.Show("Please enter the First Name.");
                fnamebox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Please enter the Last Name.");
                lnamebox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid Email.");
                emailbox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Please enter the Phone Number.");
                phonenobox.Focus();
                return;
            }

            // SQL Query to update the employee record
            string query = @"UPDATE Employee 
                     SET firstName = @firstName, 
                         lastName = @lastName, 
                         email = @email, 
                         phoneNumber = @phoneNumber
                     WHERE employeeId = @employeeId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee record updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputFields(); // Clear input fields after successful update
                    }
                    else
                    {
                        MessageBox.Show("Error: Record not updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Method to validate email format using regular expression
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void Eremove_Click(object sender, EventArgs e)
        {
            // Get the employeeId of the selected row
            string employeeId = searchbox.Text;

            // Confirm deletion
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this employee?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.No)
            {
                return; // If the user selects No, exit the method without deleting
            }

            // SQL queries for deleting from each related table
            List<string> tablesToDeleteFrom = new List<string>
    {
        "CEO", "Storekeeper", "FactoryManager", "TechnicalOfficer", "Designer",
        "COO", "SalesExecutive", "Secretary", "TeamLeader", "QuantitySurveyor",
        "Accountant", "AssistantAccountant", "ProductionManager", "Administration", "CustomerRelation"
    };

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Delete from all related tables
                        foreach (string table in tablesToDeleteFrom)
                        {
                            string query = $"DELETE FROM {table} WHERE employeeId = @employeeId";
                            SqlCommand cmd = new SqlCommand(query, conn, transaction);
                            cmd.Parameters.AddWithValue("@employeeId", employeeId);
                            cmd.ExecuteNonQuery();
                        }

                        // Now delete from Employee table
                        string deleteEmployeeQuery = @"DELETE FROM Employee WHERE employeeId = @employeeId";
                        SqlCommand deleteEmployeeCmd = new SqlCommand(deleteEmployeeQuery, conn, transaction);
                        deleteEmployeeCmd.Parameters.AddWithValue("@employeeId", employeeId);
                        deleteEmployeeCmd.ExecuteNonQuery();

                        // Commit the transaction
                        transaction.Commit();

                        MessageBox.Show("Employee and all related data deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearInputFields();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // Rollback if something fails
                        MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {

            {
                try
                {
                    // Get the Project ID from a TextBox
                    string projectId = projectidText.Text.Trim(); // Replace with your TextBox control name

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
                            projectidText.Text = reader["projectId"].ToString();
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
                            ClearProjectInputs();
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

        private void ClearProjectInputs()
        {
            // Clear all the relevant TextBoxes and controls for the project form
            projectidText.Text = string.Empty;
            PROJECTBOX.Text = string.Empty;
            submissiondate.Text = string.Empty;
            labourcost.Text = string.Empty;
            materialcost.Text = string.Empty;
            totalcost.Text = string.Empty;
            CLIENTBOX.Text = string.Empty;
            ORDERBOX.Text = string.Empty;
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

                projectidText.Text = selectedRow.Cells["projectId"].Value.ToString();
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


        private void LoadSupplierData()
        {
            // SQL query to fetch the data from the Supplier table
            string query = "SELECT TOP (1000) [supplierId], [supplierName], [contactPerson], [supplierPhoneNo], [supplierEmail] FROM Supplier";

            // Create a connection to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Create a data adapter and fill the DataTable
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Bind the DataTable to the DataGridView
                dgvsup.DataSource = dt;
            }
        }


        private void dgvsup_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is not on a header row
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvsup.Rows[e.RowIndex];

                // Populating supplier details in the respective controls
                Supsearh.Text = selectedRow.Cells["supplierId"].Value.ToString();
                supname.Text = selectedRow.Cells["supplierName"].Value.ToString();
                supcontact.Text = selectedRow.Cells["contactPerson"].Value.ToString();
                supphone.Text = selectedRow.Cells["supplierPhoneNo"].Value.ToString();
                supmail.Text = selectedRow.Cells["supplierEmail"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }


        private void ClearSupplierInputs()
        {
            // Clear all the relevant TextBoxes and controls for the supplier form
            Supsearh.Text = string.Empty;
            supname.Text = string.Empty;
            supcontact.Text = string.Empty;
            supcontact.Text = string.Empty;
            supmail.Text = string.Empty;
        }


        private void guna2Button12_Click_1(object sender, EventArgs e)
        {
            try
            {
                string supplierId = this.Supsearh.Text.Trim(); 

                // Validate input
                if (string.IsNullOrEmpty(supplierId))
                {
                    MessageBox.Show("Please enter a Supplier ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // SQL query to fetch supplier details by Supplier ID
                string query = "SELECT * FROM Supplier WHERE supplierId = @supplierId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@supplierId", supplierId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Load data into controls
                        this.Supsearh.Text = reader["supplierId"].ToString();
                        this.supname.Text = reader["supplierName"].ToString();
                        this.supcontact.Text = reader["contactPerson"].ToString();
                        this.supmail.Text = reader["supplierEmail"].ToString();
                        this.supphone.Text = reader["supplierPhoneNo"].ToString();


                        MessageBox.Show("Supplier details loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No supplier found with the given Supplier ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ClearProjectInputs();
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

        private void dgvsup_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the click is not on a header row
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvsup.Rows[e.RowIndex];

                // Populating supplier details in the respective controls
                Supsearh.Text = selectedRow.Cells["supplierId"].Value.ToString();
                supname.Text = selectedRow.Cells["supplierName"].Value.ToString();
                supcontact.Text = selectedRow.Cells["contactPerson"].Value.ToString();
                supphone.Text = selectedRow.Cells["supplierPhoneNo"].Value.ToString();
                supmail.Text = selectedRow.Cells["supplierEmail"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {
            ProjectReport reportForm = new ProjectReport();

            reportForm.SetReportDataSource();  

            // Show the report form
            reportForm.ShowDialog();

        }


        private void guna2HtmlLabel34_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox21_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }
        private void LoadSalesData()
        {
            string query = "SELECT [salesId], [salesAmount], [salesDate], [salesStatus], [clientId], [orderId], [clientPaymentId] FROM [finalPJS].[dbo].[Sales]";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvSales.DataSource = dt;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PopulateSalesFields(SqlDataReader reader)
        {
            salesIdTextBox.Text = reader["salesId"].ToString();
            salesAmountTextBox.Text = reader["salesAmount"].ToString();
            salesDateTextBox.Text = Convert.ToDateTime(reader["salesDate"]).ToString("yyyy-MM-dd");
            salesStatusComboBox.Text = reader["salesStatus"].ToString();
            clientIdTextBox.Text = reader["clientId"].ToString();
            orderIdTextBox.Text = reader["orderId"].ToString();
            clientPaymentIdTextBox.Text = reader["clientPaymentId"].ToString();
        }

        private void ClearSalesInputs()
        {
            salesIdTextBox.Text = string.Empty;
            salesAmountTextBox.Text = string.Empty;
            salesDateTextBox.Text = string.Empty;
            salesStatusComboBox.Text = string.Empty;
            clientIdTextBox.Text = string.Empty;
            orderIdTextBox.Text = string.Empty;
            clientPaymentIdTextBox.Text = string.Empty;
        }

        private bool ValidateInput(string input, string fieldName)
        {
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show($"Please enter a valid {fieldName}.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void FetchSalesDetails(string salesId)
        {
            string query = "SELECT * FROM Sales WHERE salesId = @salesId";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@salesId", SqlDbType.NVarChar).Value = salesId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PopulateSalesFields(reader);
                            MessageBox.Show("Sales details loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"No sales record found with the ID: {salesId}", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ClearSalesInputs();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void guna2Button13_Click(object sender, EventArgs e)
        {
            string salesId = salesIdTextBox.Text.Trim();

            if (ValidateInput(salesId, "Sales ID"))
            {
                FetchSalesDetails(salesId);
            }
        }

        private void dgvSales_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvSales.Rows[e.RowIndex];

                salesIdTextBox.Text = selectedRow.Cells["salesId"].Value.ToString();
                salesAmountTextBox.Text = selectedRow.Cells["salesAmount"].Value.ToString();
                salesDateTextBox.Text = Convert.ToDateTime(selectedRow.Cells["salesDate"].Value).ToString("yyyy-MM-dd");
                salesStatusComboBox.Text = selectedRow.Cells["salesStatus"].Value.ToString();
                clientIdTextBox.Text = selectedRow.Cells["clientId"].Value.ToString();
                orderIdTextBox.Text = selectedRow.Cells["orderId"].Value.ToString();
                clientPaymentIdTextBox.Text = selectedRow.Cells["clientPaymentId"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }
        private void FetchFeedbackDetails(string feedbackId)
        {
            string query = "SELECT * FROM Feedback WHERE feedbackId = @feedbackId";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@feedbackId", SqlDbType.NVarChar).Value = feedbackId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PopulateFeedbackFields(reader);
                            MessageBox.Show("Feedback details loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show($"No feedback record found with the ID: {feedbackId}", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ClearFeedbackInputs();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateFeedbackFields(SqlDataReader reader)
        {
            feedbackIdTextBox.Text = reader["feedbackId"].ToString();
            feedbackTextBox.Text = reader["feedback"].ToString();
            feedbackTypeComboBox.Text = reader["feedbackType"].ToString();
            responseTextBox.Text = reader["response"].ToString();
            employeeIdTextBox.Text = reader["employeeId"].ToString();
        }

        private void ClearFeedbackInputs()
        {
            feedbackIdTextBox.Text = string.Empty;
            feedbackTextBox.Text = string.Empty;
            feedbackTypeComboBox.Text = string.Empty;
            responseTextBox.Text = string.Empty;
            employeeIdTextBox.Text = string.Empty;
        }




        private void LoadFeedbackData()
        {
            string query = "SELECT [feedbackId], [feedback], [feedbackType], [response], [employeeId] FROM [finalPJS].[dbo].[Feedback]";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvFeedback.DataSource = dt;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void dgvFeedback_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvFeedback.Rows[e.RowIndex];

                feedbackIdTextBox.Text = selectedRow.Cells["feedbackId"].Value.ToString();
                feedbackTextBox.Text = selectedRow.Cells["feedback"].Value.ToString();
                feedbackTypeComboBox.Text = selectedRow.Cells["feedbackType"].Value.ToString();
                responseTextBox.Text = selectedRow.Cells["response"].Value.ToString();
                employeeIdTextBox.Text = selectedRow.Cells["employeeId"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }

        }

        private void guna2Button10_Click(object sender, EventArgs e)
        {
            string feedbackId = feedbackIdTextBox.Text.Trim();

            if (ValidateInput(feedbackId, "Feedback ID"))
            {
                FetchFeedbackDetails(feedbackId);
            }
        }

        private void LoadEmployeeNameById(string employeeId)
        {
            string query = "SELECT firstName, lastName FROM Employee WHERE employeeId = @employeeId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@employeeId", employeeId);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string firstName = reader["firstName"].ToString();
                        string lastName = reader["lastName"].ToString();
                        txtEmployeeName.Text = $"{firstName} {lastName}";
                    }
                    else
                    {
                        txtEmployeeName.Clear();
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database error: {sqlEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void employeeIdTextBox_TextChanged(object sender, EventArgs e)
        {
            string employeeId = employeeIdTextBox.Text.Trim();
            LoadEmployeeNameById(employeeId);
        }

        private void guna2Button16_Click(object sender, EventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrEmpty(responseTextBox.Text.Trim()))
            {
                MessageBox.Show("Please add response", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

          

            try
            {
                // SQL query for updating feedback details
                string query = @"UPDATE Feedback 
                         SET feedback = @feedback, 
                             feedbackType = @feedbackType, 
                             response = @response, 
                             employeeId = @employeeId 
                         WHERE feedbackId = @feedbackId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@feedbackId", feedbackIdTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@feedback", feedbackTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@feedbackType", feedbackTypeComboBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@response", responseTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@employeeId", employeeIdTextBox.Text.Trim());

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Response added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadFeedbackData(); // Refresh the DataGridView after updating
                            ClearFeedbackInputs11();
                        }
                        else
                        {
                            MessageBox.Show("No feedback record found with the provided ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFeedbackInputs11()
        {
            // Clear all relevant TextBoxes and controls for the feedback form
            feedbackIdTextBox.Text = string.Empty;
            feedbackTextBox.Text = string.Empty;
            feedbackTypeComboBox.Text = string.Empty; // Reset ComboBox selection
            responseTextBox.Text = string.Empty;
            employeeIdTextBox.Text = string.Empty;
            txtEmployeeName.Text = string.Empty;

        }

        private void feedbackTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvSales_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvSales.Rows[e.RowIndex];

                salesIdTextBox.Text = selectedRow.Cells["salesId"].Value.ToString();
                salesAmountTextBox.Text = selectedRow.Cells["salesAmount"].Value.ToString();
                salesDateTextBox.Text = Convert.ToDateTime(selectedRow.Cells["salesDate"].Value).ToString("yyyy-MM-dd");
                salesStatusComboBox.Text = selectedRow.Cells["salesStatus"].Value.ToString();
                clientIdTextBox.Text = selectedRow.Cells["clientId"].Value.ToString();
                orderIdTextBox.Text = selectedRow.Cells["orderId"].Value.ToString();
                clientPaymentIdTextBox.Text = selectedRow.Cells["clientPaymentId"].Value.ToString();
            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }
        private int GetNullResponseCount()
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM [finalPJS].[dbo].[Feedback] WHERE [response] IS NULL";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    count = (int)command.ExecuteScalar();
                    feed.Text = count.ToString();
                   // guna2HtmlToolTip1.SetToolTip(feed, "You have "+count+" feedbacks to respond");
                  //  guna2HtmlToolTip2.SetToolTip(guna2Button8, "You have " + count + " feedbacks to respond");


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return count;
        }
        private void guna2TextBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {

        }

        private void feed_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlToolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        public void GetMostRecentMeeting(string userName)
        {

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
                        guna2TextBox6.Text = "You have a meeting on "+meetingDate;
                  //      guna2HtmlToolTip3.SetToolTip(guna2TextBox6, "Meeting Title : "+meetingTitle);


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
                guna2TextBox7.Text =  employeeCount.ToString();
            //    guna2HtmlToolTip3.SetToolTip(guna2TextBox7,  employeeCount.ToString()+" employees have registered in organization");


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
                    guna2TextBox3.Text = supplierCount.ToString();
                  //  guna2HtmlToolTip5.SetToolTip(guna2TextBox3, supplierCount.ToString() + " suppliers have registered in the system");
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
                    guna2TextBox1.Text = pastProjectCount.ToString();
                  //  guna2HtmlToolTip6.SetToolTip(guna2TextBox1,"Company currently works on "+ pastProjectCount.ToString() + " projects");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void guna2TextBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TileButton5_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlToolTip4_Popup(object sender, PopupEventArgs e)
        {

        }

        private void guna2TextBox7_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel43_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel50_Click(object sender, EventArgs e)
        {

        }
    }
}
