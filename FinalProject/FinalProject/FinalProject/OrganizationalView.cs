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
    public partial class OrganizationalView : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;
        string loggedinuser = "";
        public OrganizationalView(string userName)
        {
            InitializeComponent();
            this.Paint += RoundedForm_Paint;

            loggedinuser = userName;
            currentUsername = userName;

        }

        private void RoundedForm_Paint(object sender, PaintEventArgs e)
        {

            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                int radius = 30; // Larger radius for a more pronounced curve
                int diameter = radius * 2;

                GraphicsPath path = new GraphicsPath();

                Rectangle topLeftArc = new Rectangle(0, 0, diameter, diameter);
                Rectangle topRightArc = new Rectangle(this.Width - diameter, 0, diameter, diameter);
                Rectangle bottomRightArc = new Rectangle(this.Width - diameter, this.Height - diameter, diameter, diameter);
                Rectangle bottomLeftArc = new Rectangle(0, this.Height - diameter, diameter, diameter);

                // Adding arcs for rounded corners
                path.AddArc(topLeftArc, 180, 90);  // Top-left corner
                path.AddArc(topRightArc, 270, 90); // Top-right corner
                path.AddArc(bottomRightArc, 0, 90); // Bottom-right corner
                path.AddArc(bottomLeftArc, 90, 90); // Bottom-left corner

                path.CloseFigure(); // Close the shape to form a complete region

                this.Region = new Region(path); // Apply the rounded shape to the form
            }
        }
        private void OrganizationalView_Load(object sender, EventArgs e)
        {
            LoadDataIntoDataGridView();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
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

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {

            CEO mainForm = new CEO(loggedinuser);
            mainForm.Show();
            this.Hide();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            CEOp mainForm = new CEOp(currentUsername);
            mainForm.Show();
            this.Hide();
        }
        private void LoadDataIntoDataGridView()
        {
            try
            {

                string query = "SELECT * FROM [finalPJS].[dbo].[OrganizationalPayment];";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2DataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
