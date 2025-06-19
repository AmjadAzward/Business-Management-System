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
    public partial class ClientViewcoo : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        string loggedinuser = "";
        public ClientViewcoo(string userName)
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
            COO mainForm = new COO(loggedinuser);
            mainForm.Show();
            this.Hide();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Login mainForm = new Login();
            mainForm.Show();
            this.Hide();
        }

        private void ClientViewcoo_Load(object sender, EventArgs e)
        {
            LoadClientPaymentsData();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            COOp mainForm = new COOp(currentUsername);
            mainForm.Show();
            this.Hide();
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

        private void dgvClientPayments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
