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

namespace FinalProject
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.Paint += RoundedForm_Paint;

        }

        private void RoundedForm_Paint(object sender, PaintEventArgs e)
        {

            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                int radius = 30; 
                int diameter = radius * 2;

                GraphicsPath path = new GraphicsPath();

                Rectangle topLeftArc = new Rectangle(0, 0, diameter, diameter);
                Rectangle topRightArc = new Rectangle(this.Width - diameter, 0, diameter, diameter);
                Rectangle bottomRightArc = new Rectangle(this.Width - diameter, this.Height - diameter, diameter, diameter);
                Rectangle bottomLeftArc = new Rectangle(0, this.Height - diameter, diameter, diameter);

                path.AddArc(topLeftArc, 180, 90);  // Top-left corner
                path.AddArc(topRightArc, 270, 90); // Top-right corner
                path.AddArc(bottomRightArc, 0, 90); // Bottom-right corner
                path.AddArc(bottomLeftArc, 90, 90); // Bottom-left corner

                path.CloseFigure(); 

                this.Region = new Region(path); 
            }
        }

        private void guna2HtmlLabel5_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void guna2HtmlLabel5_Click_1(object sender, EventArgs e)
        {

            Register mainForm = new Register();
            mainForm.Show();
            this.Hide();
        }




        private string currentUsername;
        private void guna2Button7_Click(object sender, EventArgs e)
        {

            if (txtUserName.Text.Trim() == "")
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUserName.Focus();
                return;
            }

            if (txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Password is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Focus();
                return;
            }

            try
            {
                string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT userName, position FROM Employee WHERE userName = @userName AND password = @password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userName", txtUserName.Text.Trim());
                        command.Parameters.AddWithValue("@password", txtPassword.Text.Trim());

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read(); 
                            currentUsername = reader["userName"].ToString(); 
                            string position = reader["position"].ToString();
                            MessageBox.Show("Welcome! " + position + " dashboard.", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            OpenDashboard(position , currentUsername);

                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtUserName.Clear();
                            txtPassword.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void OpenDashboard(string position, string userName)
        {
            if (string.IsNullOrEmpty(position))
            {
                MessageBox.Show("Position not found. Cannot open dashboard.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            switch (position.Trim())
            {
                case "Accountant":
                    Accountant accountantDashboard = new Accountant(userName);
                    accountantDashboard.Show();
                    break;
                case "Administration":
                    Administartion adminDashboard = new Administartion(userName);
                    adminDashboard.Show();
                    break;
                case "Assistant Accountant":
                    AssistantAccountant assistantAccountantDashboard = new AssistantAccountant(userName);
                    assistantAccountantDashboard.Show();
                    break;
                case "COO":
                    COO cooDashboard = new COO(userName);
                    cooDashboard.Show();
                    break;
                case "Designer":
                    Designer designerDashboard = new Designer(userName);
                    designerDashboard.Show();
                    break;
                case "Factory Manager":
                    Factorymanager factoryManagerDashboard = new Factorymanager(userName);
                    factoryManagerDashboard.Show();
                    break;
                case "Sales Executive":
                    SalesExecutive salesExecutiveDashboard = new SalesExecutive(userName);
                    salesExecutiveDashboard.Show();
                    break;
                case "Secretary":
                    Payments secretaryDashboard = new Payments(userName);
                    secretaryDashboard.Show();
                    break;
                case "Storekeeper":
                    Storekeeper storekeeperDashboard = new Storekeeper(userName);
                    storekeeperDashboard.Show();
                    break;
                case "Team Leader":
                    Teamleader teamLeaderDashboard = new Teamleader(userName);
                    teamLeaderDashboard.Show();
                    break;
                case "Technical Officer":
                    TechnicalOfficer technicalOfficerDashboard = new TechnicalOfficer(userName);
                    technicalOfficerDashboard.Show();
                    break;
                case "CEO":
                    CEO ceoDashboard = new CEO(userName);
                    ceoDashboard.Show();
                    break;
                case "Quantity Surveyor":
                    QuantitySurveyor quantitySurveyorDashboard = new QuantitySurveyor(userName);
                    quantitySurveyorDashboard.Show();
                    break;
                case "Production Manager":
                    Productionmanager productionManagerDashboard = new Productionmanager(userName);
                    productionManagerDashboard.Show();
                    break;
                case "Customer Relations":
                    CustomerRelations customer = new CustomerRelations(userName);
                    customer.Show();
                    break;
                default:
                    MessageBox.Show("Position not recognized. Cannot open dashboard.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            this.Hide();
        }



        private void Login_Load(object sender, EventArgs e)
        {
            guna2ImageButton1.HoverState.ImageSize = guna2ImageButton1.ImageSize; // Keep the original size
            guna2ImageButton1.HoverState.Parent = null;
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel13_Click(object sender, EventArgs e)
        {

        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
          
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void guna2ImageButton1_MouseEnter(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = '\0';
        }

        private void guna2ImageButton1_MouseLeave(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = '*';
        }
    }
}
