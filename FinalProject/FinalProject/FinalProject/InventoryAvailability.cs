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
    public partial class InventoryAvailability : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        string loggedinuser = "";

        public InventoryAvailability(string userName)
        {
            InitializeComponent();
            this.Paint += RoundedForm_Paint;

            loggedinuser = userName;
            currentUsername = userName;
            LoadInventoryData();



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


        private void guna2TextBox35_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
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

            Factorymanager mainForm = new Factorymanager(loggedinuser);
            mainForm.Show();
            this.Hide();
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void guna2HtmlLabel15_Click(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button13_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel3_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel16_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel19_Click(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void guna2DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void InventoryAvailability_Load(object sender, EventArgs e)
        {
         //LoadDataIntoDGV();
            LoadSupplierIds();
            LoadInventoryData();

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

        private void LoadDataIntoDGV()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Inventory";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        dgv.DataSource = dataTable;
                    }
                    else
                    {
                        MessageBox.Show("No inventory data found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        private void dgvinventory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
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
        public void LoadInventoryData()
        {
            // Connection string (adjust as needed)

            // SQL query to get inventory where supplierId is NULL
            string query = "SELECT * FROM Inventory";

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
                    dgv.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur
                    MessageBox.Show("An error occurred: " + ex.Message);
                }
            }
        }
        private void updatebtn_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an inventory item to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgv.SelectedRows[0];
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

        private void supcmbbox_SelectedIndexChanged(object sender, EventArgs e)
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
    }

}
