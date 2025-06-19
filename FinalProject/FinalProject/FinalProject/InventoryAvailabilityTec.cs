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
    public partial class InventoryAvailabilityTec : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        string loggedinuser = "";
        public InventoryAvailabilityTec(string userName)
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
            TechnicalOfficer mainForm = new TechnicalOfficer(loggedinuser);
            mainForm.Show();
            this.Hide();
        }

        private void InventoryAvailabilityTec_Load(object sender, EventArgs e)
        {
            LoadDataIntoDGV();
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            TechnicalOfficerP mainForm = new TechnicalOfficerP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        //
        //
        //

        private void LoadDataIntoDGV()
        {
            try
            {
                if (dgvinventorys == null)
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

                    dgvinventorys.DataSource = dataTable;

                    dgvinventorys.Columns["inventoryId"].Width = 55;
                    dgvinventorys.Columns["supplierId"].Width = 45;
                    dgvinventorys.Columns["availableQty"].Width = 25;
                    dgvinventorys.Columns["productAddedDate"].Width = 65;
                    dgvinventorys.Columns["inventoryType"].Width = 80;


                    dgvinventorys.Columns["inventoryId"].HeaderText = "Id";
                    dgvinventorys.Columns["supplierId"].HeaderText = "Sup Id";
                    dgvinventorys.Columns["productName"].HeaderText = "Name";
                    dgvinventorys.Columns["inventoryType"].HeaderText = "Type";
                    dgvinventorys.Columns["availableQty"].HeaderText = "Qty";
                    dgvinventorys.Columns["productAddedDate"].HeaderText = "Product Added Date";

                    dgvinventorys.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dgvinventorys.Columns["productName"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvinventorys.Columns["inventoryType"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;


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

        private void dgvinventorys_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow selectedRow = dgvinventorys.Rows[e.RowIndex];

                    string inventoryId = selectedRow.Cells["inventoryId"].Value.ToString();
                    string productName = selectedRow.Cells["productName"].Value.ToString();
                    string inventoryType = selectedRow.Cells["inventoryType"].Value.ToString();
                    string supplierId = selectedRow.Cells["supplierId"].Value.ToString();
                    string availableQty = selectedRow.Cells["availableQty"].Value.ToString();
                    string productAddedDate = Convert.ToDateTime(selectedRow.Cells["productAddedDate"].Value).ToString("yyyy-MM-dd");

                    productboxs.Text = productName;
                    typecombos.Text = inventoryType;
                    supcmbboxs.Text = supplierId;
                    qtyboxs.Text = availableQty;
                    dateboxs.Text = productAddedDate; // Assuming datebox is also a TextBox

                    LoadSupplierName(supplierId);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadSupplierName(string supplierId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT supplierName FROM Supplier WHERE supplierId = @supplierId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@supplierId", supplierId);

                    connection.Open();
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        txtSupplierNames.Text = result.ToString(); // Assuming txtSupplierName is the TextBox for supplier name
                    }
                    else
                    {
                        txtSupplierNames.Text = "Unknown"; // Default value if supplier not found
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading supplier name: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
