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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject
{
    public partial class COO : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        private string employeeId;
        public COO(string userName)
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

        private void updbtnacc_Click_1(object sender, EventArgs e)
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
        }

        private void guna2TileButton10_Click(object sender, EventArgs e)
        {
            StaffProfile mainForm = new StaffProfile(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton6_Click_1(object sender, EventArgs e)
        {
            SalaryViewcoo mainForm = new SalaryViewcoo(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton7_Click(object sender, EventArgs e)
        {
            ClientViewcoo mainForm = new ClientViewcoo(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton8_Click(object sender, EventArgs e)
        {
            OrganizationalViewcoo mainForm = new OrganizationalViewcoo(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton9_Click(object sender, EventArgs e)
        {
            StaffAttendance mainForm = new StaffAttendance(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2ImageButton3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel7_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Login mainForm = new Login();
            mainForm.Show();
            this.Hide();
        }

        private void guna2ImageButton3_Click_1(object sender, EventArgs e)
        {
            COOp mainForm = new COOp(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2CircleButton2_Click_1(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 0;

        }

        private void guna2CircleButton1_Click_2(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void guna2TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2TileButton6_Click(object sender, EventArgs e)
        {
            SalaryViewcoo mainForm = new SalaryViewcoo(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton7_Click_1(object sender, EventArgs e)
        {
            ClientViewcoo mainForm = new ClientViewcoo(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2TileButton8_Click_1(object sender, EventArgs e)
        {
            OrganizationalViewcoo mainForm = new OrganizationalViewcoo(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void COO_Load(object sender, EventArgs e)
        {
            LoadDataIntoDGV();
            SupplierData();
            LoadData();
            LoadFeedbackIntoDGV(employeeId);
            FeedbackData();
            LoadEmployeeData();
            LoadSalesData();
            LoadMeetingData();
            LoadInventoryData();

            //dashboard

            CountEmployees();
            GetMostRecentMeetingForUser(currentUsername);
            CountSuppliers();
            //CountPastProjects();
            LoadTaskCounts(currentUsername);
            LoadLatestSalary(currentUsername);

        }


        private void guna2Button2_Click_1(object sender, EventArgs e)
        {
            Login mainForm = new Login();
            mainForm.Show();
            this.Hide();
        }

        private void guna2DataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

       

        private void SupplierData()
        {
            dgvSupplier.Columns["supplierId"].HeaderText = "ID";
            dgvSupplier.Columns["supplierName"].HeaderText = "Name";
            dgvSupplier.Columns["supplierPhoneNo"].HeaderText = "Phone Number";
            dgvSupplier.Columns["supplierEmail"].HeaderText = "Email";
            dgvSupplier.Columns["contactPerson"].HeaderText = "Contact Person";
        }

        private string GenerateSupplierId()
        {
            string newId = "S001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 supplierId FROM Supplier ORDER BY CAST(SUBSTRING(supplierId, 2, LEN(supplierId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();

                            int numericPart = int.Parse(lastId.Substring(1));
                            newId = "S" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Supplier ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }

        private void LoadDataIntoDGV()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Supplier";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvSupplier.DataSource = dataTable;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool CheckSupplierPhoneNumberUnique(string phoneNumber)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Supplier WHERE supplierPhoneNo = @PhoneNumber";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                connection.Close();

                return count == 0;
            }
        }

        private bool CheckEmailUnique(string supplierEmail)
        {
            using (SqlConnection connection = new SqlConnection("Server=AmjadAzward\\SQLEXPRESS;Database=finalPJS;Trusted_Connection=True;"))
            {
                string query = "SELECT COUNT(*) FROM Supplier WHERE supplierEmail = @supplierEmail";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@supplierEmail", supplierEmail);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                connection.Close();

                return count == 0;
            }
        }


        private void ClearInputs()
        {
            namebox.Clear();
            phonebox.Clear();
            mailbox.Clear();
            contactpersonbox.Clear();
        }


        private void addbtnsup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(namebox.Text.Trim()))
            {
                MessageBox.Show("Supplier Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                namebox.Focus();
                return;
            }
            if (string.IsNullOrEmpty(phonebox.Text.Trim()) || !long.TryParse(phonebox.Text.Trim(), out _) || phonebox.Text.Trim().Length != 10)
            {
                MessageBox.Show("A valid Phone Number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                phonebox.Focus();
                return;
            }

            bool isPhoneNumberUnique = CheckSupplierPhoneNumberUnique(phonebox.Text.Trim());
            if (!isPhoneNumberUnique)
            {
                MessageBox.Show("Phone Number already exists. Please use another.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                phonebox.Focus();
                return;
            }

            string emailRegexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (string.IsNullOrEmpty(mailbox.Text.Trim()) || !Regex.IsMatch(mailbox.Text.Trim(), emailRegexPattern))
            {
                MessageBox.Show("A valid Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mailbox.Focus();
                return;
            }

            bool isEmailUnique = CheckEmailUnique(mailbox.Text.Trim());
            if (!isEmailUnique)
            {
                MessageBox.Show("Email address already exists. Please use another.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mailbox.Focus();
                return;
            }



            if (string.IsNullOrEmpty(contactpersonbox.Text.Trim()))
            {
                MessageBox.Show("Contact Person is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                contactpersonbox.Focus();
                return;
            }

            try
            {
                string supplierId = GenerateSupplierId();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Supplier (supplierId, supplierName, supplierPhoneNo, supplierEmail, contactPerson) " +
                                   "VALUES (@supplierId, @supplierName, @supplierPhoneNo, @supplierEmail, @contactPerson);";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@supplierId", supplierId);
                        command.Parameters.AddWithValue("@supplierName", namebox.Text.Trim());
                        command.Parameters.AddWithValue("@supplierPhoneNo", phonebox.Text.Trim());
                        command.Parameters.AddWithValue("@supplierEmail", mailbox.Text.Trim());
                        command.Parameters.AddWithValue("@contactPerson", contactpersonbox.Text.Trim());

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Supplier added successfully! Supplier ID: " + supplierId, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataIntoDGV();
                            ClearInputs();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add supplier. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rembtnsup_Click(object sender, EventArgs e)
        {
            if (dgvSupplier.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvSupplier.SelectedRows[0];
            string supplierId = selectedRow.Cells["supplierId"].Value.ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // SQL Query to delete related inventory records first and then the supplier
                    string query = @"
                BEGIN TRANSACTION;
                DELETE FROM Inventory WHERE supplierId = @supplierId;
                DELETE FROM Supplier WHERE supplierId = @supplierId;
                COMMIT TRANSACTION;";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@supplierId", supplierId);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Supplier records deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDGV(); // Refresh the DataGridView
                        ClearInputs();    // Clear input fields
                    }
                    else
                    {
                        MessageBox.Show("Supplier not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSupplier_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvSupplier.Rows[e.RowIndex];

                string supplierName = selectedRow.Cells["supplierName"].Value.ToString();
                string supplierPhoneNo = selectedRow.Cells["supplierPhoneNo"].Value.ToString();
                string supplierEmail = selectedRow.Cells["supplierEmail"].Value.ToString();
                string contactPerson = selectedRow.Cells["contactPerson"].Value.ToString();

                namebox.Text = supplierName;
                phonebox.Text = supplierPhoneNo;
                mailbox.Text = supplierEmail;
                contactpersonbox.Text = contactPerson;
            }

        }

        private bool CheckSupplierEmailUnique1(string email, string supplierId = null)
        {
            try
            {
                // If supplierId is not passed, fetch it from the selected row in the DataGridView
                if (string.IsNullOrEmpty(supplierId))
                {
                    // Ensure a row is selected
                    if (dgvSupplier.SelectedRows.Count > 0)
                    {
                        DataGridViewRow selectedRow = dgvSupplier.SelectedRows[0];
                        supplierId = selectedRow.Cells[0].Value.ToString(); // Assuming supplierId is in the first column
                    }
                    else
                    {
                        MessageBox.Show("Please select a supplier to check email uniqueness.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                // Continue with the existing logic to check if the email is unique
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Supplier WHERE supplierEmail = @supplierEmail";

                    // If a supplierId is provided, we exclude the current record
                    if (!string.IsNullOrEmpty(supplierId))
                    {
                        query += " AND supplierId != @supplierId"; // Exclude the current supplier
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@supplierEmail", email.Trim());

                        if (!string.IsNullOrEmpty(supplierId))
                        {
                            command.Parameters.AddWithValue("@supplierId", supplierId);
                        }

                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        return count == 0; // If count is 0, email is unique
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CheckSupplierPhoneNumberUnique1(string phoneNumber, string supplierId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(supplierId))
                {
                    if (dgvSupplier.SelectedRows.Count > 0)
                    {
                        DataGridViewRow selectedRow = dgvSupplier.SelectedRows[0];
                        supplierId = selectedRow.Cells[0].Value.ToString(); // Assuming supplierId is in the first column
                    }
                    else
                    {
                        MessageBox.Show("Please select a supplier to check phone number uniqueness.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM Supplier WHERE supplierPhoneNo = @supplierPhoneNo";

                    if (!string.IsNullOrEmpty(supplierId))
                    {
                        query += " AND supplierId != @supplierId"; // Exclude the current supplier
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@supplierPhoneNo", phoneNumber.Trim());

                        if (!string.IsNullOrEmpty(supplierId))
                        {
                            command.Parameters.AddWithValue("@supplierId", supplierId);
                        }

                        connection.Open();
                        int count = (int)command.ExecuteScalar();
                        return count == 0; // If count is 0, phone number is unique
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }



        private void clrbtnsup_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void updbtnsup_Click(object sender, EventArgs e)


        {
            if (string.IsNullOrEmpty(namebox.Text.Trim()))
            {
                MessageBox.Show("Supplier Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                namebox.Focus();
                return;
            }

            if (string.IsNullOrEmpty(phonebox.Text.Trim()) || !long.TryParse(phonebox.Text.Trim(), out _) || phonebox.Text.Trim().Length != 10)
            {
                MessageBox.Show("A valid Phone Number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                phonebox.Focus();
                return;
            }

            bool isPhoneNumberUnique = CheckSupplierPhoneNumberUnique1(phonebox.Text.Trim());
            if (!isPhoneNumberUnique)
            {
                MessageBox.Show("Phone Number already exists. Please use another.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                phonebox.Focus();
                return;
            }

            string emailRegexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (string.IsNullOrEmpty(mailbox.Text.Trim()) || !Regex.IsMatch(mailbox.Text.Trim(), emailRegexPattern))
            {
                MessageBox.Show("A valid Email is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mailbox.Focus();
                return;
            }


            bool isEmailUnique = CheckSupplierEmailUnique1(mailbox.Text.Trim());
            if (!isEmailUnique)
            {
                MessageBox.Show("Email already exists. Please use another.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mailbox.Focus();
                return;
            }
            if (string.IsNullOrEmpty(contactpersonbox.Text.Trim()))
            {
                MessageBox.Show("Contact Person is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                contactpersonbox.Focus();
                return;
            }

            if (dgvSupplier.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvSupplier.SelectedRows[0];
            string supplierId = selectedRow.Cells["supplierId"].Value.ToString(); // Get the selected supplier's ID

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Supplier SET supplierName = @supplierName, supplierPhoneNo = @supplierPhoneNo, " +
                                   "supplierEmail = @supplierEmail, contactPerson = @contactPerson WHERE supplierId = @supplierId;";
                    SqlCommand command = new SqlCommand(query, connection);

                    // Add parameters
                    command.Parameters.AddWithValue("@supplierId", supplierId);
                    command.Parameters.AddWithValue("@supplierName", namebox.Text.Trim());
                    command.Parameters.AddWithValue("@supplierPhoneNo", phonebox.Text.Trim());
                    command.Parameters.AddWithValue("@supplierEmail", mailbox.Text.Trim());
                    command.Parameters.AddWithValue("@contactPerson", contactpersonbox.Text.Trim());

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Supplier updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadDataIntoDGV();
                        ClearInputs();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update supplier. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void guna2Button14_Click(object sender, EventArgs e)
        {

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
                    string query = "SELECT TOP 1 feedbackId FROM Feedback ORDER BY CAST(SUBSTRING(feedbackId, 2, LEN(feedbackId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();

                            int numericPart = int.Parse(lastId.Substring(1));
                            newId = "F" + (numericPart + 1).ToString("D3");
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
                WHERE employeeId = @employeeId";

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

        private void guna2HtmlLabel47_Click(object sender, EventArgs e)
        {

        }

        private void cmbFeedbackType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtFeedback_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel6_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel7_Click_1(object sender, EventArgs e)
        {

        }

        private void txtEmpName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtEmpId_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel27_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel24_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

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
        private void dgvProjectss_CellContentClick(object sender, DataGridViewCellEventArgs e)
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

        private void guna2Button11_Click(object sender, EventArgs e)
        {
            // Check if any required field is empty
            if (string.IsNullOrWhiteSpace(PROJECTBOX.Text) ||
                string.IsNullOrWhiteSpace(projectid.Text) ||
                 string.IsNullOrWhiteSpace(submissiondate.Text) ||
                string.IsNullOrWhiteSpace(labourcost.Text) ||
                string.IsNullOrWhiteSpace(materialcost.Text) ||
                string.IsNullOrWhiteSpace(CLIENTBOX.Text) ||
                string.IsNullOrWhiteSpace(ORDERBOX.Text) ||
                string.IsNullOrWhiteSpace(totalcost.Text))
            {
                MessageBox.Show("Project is not initiated yet.cannot approve yet.",
                                "Validation Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                ClearInputs1();
                return; // Exit the function if validation fails
            }

            // All fields are filled, approve the project
            MessageBox.Show("Project Approved!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearInputs1();

            // Optionally, perform other actions after approval
        }


        private void guna2Button12_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the selected Project ID from the TextBox or ComboBox
                string projectid1 = projectid.Text;  // Ensure 'projectid' is the correct control reference

                // Check if the projectid is not empty
                if (string.IsNullOrEmpty(projectid1))
                {
                    MessageBox.Show("Please select a valid Project ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method if projectid is empty
                }

                string query = "DELETE FROM Project WHERE projectId = @projectid";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Create the SqlCommand and add the parameter
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@projectid", projectid);

                    // Execute the delete query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Check if a record was deleted
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("PROJECT REJECTED", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Optionally, refresh the combo box or any UI component.
                        // projectid.Clear(); // Example of clearing the TextBox or ComboBox
                    }
                    else
                    {
                        MessageBox.Show("No record found with the given Project ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private void REJECTBTN_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the selected Project ID from the TextBox or ComboBox
                string projectid1 = projectid.Text;  // Ensure 'projectid' is the correct control reference

                // Check if the projectid is not empty
                if (string.IsNullOrEmpty(projectid1))
                {
                    MessageBox.Show("Please select a valid Project ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method if projectid is empty
                }

                string query = "DELETE FROM Project WHERE projectId = @projectid";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Create the SqlCommand and add the parameter
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@projectid", projectid);

                    // Execute the delete query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Check if a record was deleted
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("PROJECT REJECTED", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Optionally, refresh the combo box or any UI component.
                        // projectid.Clear(); // Example of clearing the TextBox or ComboBox
                    }
                    else
                    {
                        MessageBox.Show("No record found with the given Project ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private void tabPage7_Click(object sender, EventArgs e)
        {

        }

        private void CLIENTBOX_TextChanged(object sender, EventArgs e)
        {

        }

        private void ORDERBOX_TextChanged(object sender, EventArgs e)
        {

        }

        private void PROJECTBOX_TextChanged(object sender, EventArgs e)
        {

        }

        private void materialcost_TextChanged(object sender, EventArgs e)
        {

        }

        private void labourcost_TextChanged(object sender, EventArgs e)
        {

        }

        private void totalcost_TextChanged(object sender, EventArgs e)
        {

        }

        private void projectid_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {

            try
            {
                // Get the Project ID from the TextBox
                string projectId1 = projectid.Text.Trim(); // Ensure the .Text property is used

                // Validate input
                if (string.IsNullOrEmpty(projectId1))
                {
                    MessageBox.Show("Please enter a valid Project ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // SQL query to delete the project
                string query = "DELETE FROM Project WHERE projectId = @projectId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add parameter for project ID
                        cmd.Parameters.AddWithValue("@projectId", projectId1);

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Project deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearInputs1();
                            LoadData();

                            // Optionally, clear the form or refresh the UI
                            projectid.Clear();
                        }
                        else
                        {
                            MessageBox.Show("No project found with the given Project ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
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

        //search
        private void guna2Button10_Click(object sender, EventArgs e)
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

        private void submissiondate_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoadSalesData()
        {
            string query = "SELECT TOP (1000) [salesId], [salesAmount], [salesDate], [salesStatus], [clientId], [orderId], [clientPaymentId] FROM [finalPJS].[dbo].[Sales]";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvsales.DataSource = dt;
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

        private void dgvsales_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvsales.Rows[e.RowIndex];

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


        private void LoadMeetingData()
        {
            // SQL query to fetch meeting data
            string query = "SELECT  [meetingId], [meetingTitle], [meetingDate], [meetingParticipants], [meetingSummary], [employeeId] FROM [finalPJS].[dbo].[Meeting]";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))  // Open connection to the database
                {
                    conn.Open();

                    // Create a SqlDataAdapter to fill a DataTable with data
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();

                    // Fill the DataTable with data
                    da.Fill(dt);

                    // Check if any data was returned
                    if (dt.Rows.Count > 0)
                    {
                        // Bind the data to the DataGridView
                        dgvMeetings.DataSource = dt;
                    }
                    else
                    {
                        MessageBox.Show("No meeting data found.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (SqlException ex)
            {
                // Handle SQL-specific exceptions
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                MessageBox.Show($"Error: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void guna2Button17_Click(object sender, EventArgs e)
        {
            string meetingId = meetingIdTextBox.Text.Trim();

            if (ValidateInput1(meetingId, "Meeting ID"))
            {
                FetchMeetingDetails(meetingId);
            }
        }
        private bool ValidateInput1(string input, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show($"{fieldName} cannot be empty. Please provide a valid {fieldName}.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // Method to fetch meeting details
        private void FetchMeetingDetails(string meetingId)
        {
            try
            {
                string query = @"SELECT meetingId, meetingTitle, meetingDate, meetingParticipants, meetingSummary
                         FROM Meeting
                         WHERE meetingId = @meetingId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@meetingId", meetingId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                meetingIdTextBox.Text = reader["meetingId"].ToString();
                                meetingTitleTextBox.Text = reader["meetingTitle"].ToString();
                                meetingDatePicker.Value = Convert.ToDateTime(reader["meetingDate"]);
                                meetingParticipantsListBox.Text = reader["meetingSummary"].ToString();

                                // Assuming meetingParticipants is a comma-separated string of participants
                                meetingParticipantsListBox.Items.Clear();
                                string[] participants = reader["meetingParticipants"].ToString().Split(',');
                                foreach (var participant in participants)
                                {
                                    meetingParticipantsListBox.Items.Add(participant);
                                }

                                MessageBox.Show("Meeting details loaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No meeting record found with the given Meeting ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                ClearMeetingInputs();
                            }
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

        // Method to clear meeting-related input fields
        private void ClearMeetingInputs()
        {
            meetingIdTextBox.Text = string.Empty;
            meetingTitleTextBox.Text = string.Empty;
            meetingDatePicker.Value = DateTime.Now; // Reset the DateTimePicker to current date
            summary.Text = string.Empty;
            meetingParticipantsListBox.Items.Clear();
        }

        

        private void dgvMeetings_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvMeetings.Rows[e.RowIndex];

                meetingIdTextBox.Text = selectedRow.Cells["meetingId"].Value.ToString();
                meetingTitleTextBox.Text = selectedRow.Cells["meetingTitle"].Value.ToString();
                meetingDatePicker.Value = Convert.ToDateTime(selectedRow.Cells["meetingDate"].Value);
                summary.Text = selectedRow.Cells["meetingSummary"].Value.ToString();
                // Assuming meetingParticipants is a comma-separated string of participants
                meetingParticipantsListBox.Items.Clear();
                string[] participants = selectedRow.Cells["meetingParticipants"].Value.ToString().Split(',');
                foreach (var participant in participants)
                {
                    meetingParticipantsListBox.Items.Add(participant);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid row.", "Row Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void dgvsales_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvsales.Rows[e.RowIndex];

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
        private void LoadInventoryData()
        {
            // SQL query to load inventory data where requiredQty is not null
            string query = @"
        SELECT  
            [inventoryId], 
            [inventoryType], 
            [productName], 
            [availableQty], 
            [requiredQty]
        FROM [finalPJS].[dbo].[Inventory]
        WHERE [requiredQty] IS NOT NULL";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))  // Open connection to the database
                {
                    conn.Open();

                    // Create a SqlDataAdapter to fill a DataTable with data
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();

                    // Fill the DataTable with data
                    da.Fill(dt);

                    // Check if any data was returned
                    if (dt.Rows.Count > 0)
                    {
                        // Bind the data to the DataGridView
                        guna2DataGridView3.DataSource = dt;
                    }
                    else
                    {
                        MessageBox.Show("No inventory data found where requiredQty is not NULL.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (SqlException ex)
            {
                // Handle SQL-specific exceptions
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                MessageBox.Show($"Error: {ex.Message}", "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void guna2TextBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2DataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked row index is valid (>= 0)
            if (e.RowIndex >= 0)
            {
                // Get the selected row from the DataGridView
                DataGridViewRow selectedRow = guna2DataGridView3.Rows[e.RowIndex];

                // Access individual cells by column names and display or process data
                string inventoryType = selectedRow.Cells["inventoryType"].Value.ToString();
                string productName = selectedRow.Cells["productName"].Value.ToString();
                string requiredQty = selectedRow.Cells["requiredQty"].Value.ToString();

                // Example: Populate TextBoxes or Labels with data
                type.Text = inventoryType;
                name.Text = productName;
                qty.Text = requiredQty;

            }
            else
            {
                MessageBox.Show("Please select a valid row.");
            }
        }




        //dashboard


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
           //     guna2HtmlToolTip1.SetToolTip(guna2TextBox3, employeeCount.ToString() + " employees have registered in organization");


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
                    guna2TextBox12.Text = supplierCount.ToString();
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
                        guna2TextBox10.Text = pendingTaskCount.ToString();



                        if (pendingTaskCount > 0 && overdueTaskCount > 0)
                        {
                       //     guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending.\n " + overdueTaskCount + " of" + pendingTaskCount + " are overdue.");

                        }
                        else if (pendingTaskCount > 0 && overdueTaskCount <= 0)
                        {
                         //   guna2HtmlToolTip6.SetToolTip(guna2TextBox4, pendingTaskCount + " tasks are pending");

                        }
                        else if (pendingTaskCount <= 0 )
                        {
                            
                             //   guna2HtmlToolTip6.SetToolTip(guna2TextBox4,"You have no tasks to complete");  
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
                            guna2HtmlToolTip4.SetToolTip(guna2TextBox2, "Your this month salary is Rs" + finalAmount.ToString() + ".00");

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







        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {

        }

        private void guna2TileButton1_Click(object sender, EventArgs e)
        {

        }

        private void guna2TextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel33_Click(object sender, EventArgs e)
        {

        }
    }


}



