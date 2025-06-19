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
    public partial class EmpAttendance : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;

        string loggedinuser = "";
        public EmpAttendance(string userName)
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
            AssistantAccountant mainForm = new AssistantAccountant(loggedinuser);
            mainForm.Show();
            this.Hide();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Login mainForm = new Login();
            mainForm.Show();
            this.Hide();
        }

        private void guna2Button2_Click_1(object sender, EventArgs e)
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

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void EmpAttendance_Load(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            AAccountantP mainForm = new AAccountantP(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void guna2DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dtpAttendanceDate.Value.Date;

            LoadAttendanceRecordsForDate(selectedDate);
        }

        private void LoadAttendanceRecordsForDate(DateTime attendanceDate)
        {
            try
            {
                if (dgvAttendance.Columns.Count == 0)
                {
                    dgvAttendance.Columns.Add("employeeId", "Employee ID");
                    dgvAttendance.Columns.Add("attendanceStatus", "Attendance Status");
                    dgvAttendance.Columns.Add("attendanceDate", "Attendance Date");  // Add column for Date
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT employeeId, attendanceStatus, attendanceDate FROM EmployeeAttendance WHERE attendanceDate = @attendanceDate";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@attendanceDate", attendanceDate.Date);  // Ensure only the date part is considered

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    dgvAttendance.Rows.Clear();

                    while (reader.Read())
                    {
                        string employeeId = reader["employeeId"].ToString();
                        string attendanceStatus = reader["attendanceStatus"].ToString();
                        DateTime attendanceDateFromDb = Convert.ToDateTime(reader["attendanceDate"]);  // Convert to DateTime

                        dgvAttendance.Rows.Add(employeeId, attendanceStatus, attendanceDateFromDb.ToString("yyyy-MM-dd")); // Add formatted date to the row
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attendance records: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
