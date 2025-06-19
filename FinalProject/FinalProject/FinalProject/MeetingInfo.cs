using CrystalDecisions.CrystalReports.Engine;
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
using Microsoft.VisualBasic;


namespace FinalProject
{
    public partial class MeetingInfo : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;

        string loggedinuser = "";
        public MeetingInfo(string userName)
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

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            Payments mainForm = new Payments(loggedinuser);
            mainForm.Show();
            this.Hide();
        }

        private void MeetingInfo_Load(object sender, EventArgs e)
        {
            LoadMeetingData();
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            SECp form = new SECp(currentUsername);
            form.Show();
            this.Hide();
        }

        private void LoadMeetingData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT meetingId, meetingTitle, meetingDate, meetingParticipants , meetingSummary  FROM Meeting";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvMeetings.DataSource = dataTable;

                    dgvMeetings.Columns["meetingParticipants"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    //dgvMeeting.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    dgvMeetings.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                    dgvMeetings.Columns["meetingSummary"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    //dgvMeetings.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;








                    foreach (DataGridViewRow row in dgvMeetings.Rows)
                    {
                        var participants = row.Cells["meetingParticipants"].Value?.ToString();
                        if (!string.IsNullOrEmpty(participants))
                        {
                            string[] participantsArray = participants.Split(',');
                            row.Cells["meetingParticipants"].Value = string.Join(Environment.NewLine, participantsArray);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading meeting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvMeetings_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvMeetings.Rows[e.RowIndex];
                txtMeetingId.Text = selectedRow.Cells["meetingId"].Value.ToString();

                txtMeTitle.Text = selectedRow.Cells["meetingTitle"].Value.ToString();
                dtpInfo.Text = Convert.ToDateTime(selectedRow.Cells["meetingDate"].Value).ToString("yyyy-MM-dd"); // Display date in TextBox
                MeSummary.Text = selectedRow.Cells["meetingSummary"].Value.ToString();

                var participants = selectedRow.Cells["meetingParticipants"].Value?.ToString();
                if (!string.IsNullOrEmpty(participants))
                {
                    string[] participantArray = participants.Split(',');
                    participantsTextBox.Clear();
                    foreach (string participant in participantArray)
                    {
                        participantsTextBox.Text += participant.Trim() + Environment.NewLine;
                    }
                }
                else
                {
                    participantsTextBox.Clear();
                }
            }
        }

        private void dtpInfo_TextChanged(object sender, EventArgs e)
        {

        }

        private void addbtninfo_Click(object sender, EventArgs e)
        {
            string existingSummary = dgvMeetings.SelectedRows[0].Cells["meetingSummary"].Value.ToString();

            if (existingSummary == "")
            {
                // Check if meeting summary is empty
                if (string.IsNullOrWhiteSpace(MeSummary.Text))
                {
                    MessageBox.Show("Meeting summary cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    MeSummary.Focus();
                    return;
                }

                try
                {
                    // Check if a row is selected in the DataGridView
                    if (dgvMeetings.SelectedRows.Count > 0)
                    {
                        // Get the meetingId from the selected DataGridView row
                        string meetingId = dgvMeetings.SelectedRows[0].Cells["meetingId"].Value.ToString(); // Assuming meetingId column exists in your DataGridView

                        // Update the meeting summary
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string updateQuery = "UPDATE Meeting SET meetingSummary = @meetingSummary WHERE meetingId = @meetingId";

                            using (SqlCommand command = new SqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@meetingSummary", MeSummary.Text.Trim());
                                command.Parameters.AddWithValue("@meetingId", meetingId);

                                connection.Open();
                                int result = command.ExecuteNonQuery();

                                if (result > 0)
                                {
                                    if (MeSummary.Text == "")
                                    {
                                        MessageBox.Show("Meeting summary added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    }
                                    else
                                    {
                                        MessageBox.Show("Meeting summary added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    }
                                    LoadMeetingData(); // Refresh the DataGridView
                                    ClearMeetingFields(); // Clear the input fields
                                }
                                else
                                {
                                    MessageBox.Show("Failed to add meeting. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select a meeting from the list to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else
            {
                MessageBox.Show("Meeting information is already added.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }




        private void ClearMeetingFields()
        {
            txtMeTitle.Clear();
            dtpInfo.Clear();
            MeSummary.Clear();
            txtMeetingId.Clear();
            participantsTextBox.Clear();
        }
        private void SearchMeetingById(string meetingId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT meetingTitle, meetingDate, meetingParticipants, meetingSummary FROM Meeting WHERE meetingId = @meetingId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@meetingId", meetingId);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtMeTitle.Text = reader["meetingTitle"].ToString();
                            dtpInfo.Text = Convert.ToDateTime(reader["meetingDate"]).ToString("yyyy-MM-dd");
                            MeSummary.Text = reader["meetingSummary"].ToString();

                            string participants = reader["meetingParticipants"].ToString();
                            string[] participantArray = participants.Split(',');

                            participantsTextBox.Clear();
                            foreach (string participant in participantArray)
                            {
                                participantsTextBox.Text += participant.Trim() + Environment.NewLine;
                            }
                        }
                        else
                        {
                            MessageBox.Show("No meeting found with the given ID.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void searchbtninfo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMeetingId.Text))
            {
                SearchMeetingById(txtMeetingId.Text.Trim());
            }
            else
            {
                MessageBox.Show("Please enter a valid Meeting ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void updbtninfo_Click(object sender, EventArgs e)
        {

            // Check if meeting summary is empty
            if (string.IsNullOrWhiteSpace(MeSummary.Text))
            {
                MessageBox.Show("Meeting summary cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MeSummary.Focus();
                return;
            }

            try
            {
                // Check if a row is selected in the DataGridView
                if (dgvMeetings.SelectedRows.Count > 0)
                {
                    // Get the meetingId from the selected DataGridView row
                    string meetingId = dgvMeetings.SelectedRows[0].Cells["meetingId"].Value.ToString(); // Assuming meetingId column exists in your DataGridView

                    // Update the meeting summary
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string updateQuery = "UPDATE Meeting SET meetingSummary = @meetingSummary WHERE meetingId = @meetingId";

                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@meetingSummary", MeSummary.Text.Trim());
                            command.Parameters.AddWithValue("@meetingId", meetingId);

                            connection.Open();
                            int result = command.ExecuteNonQuery();

                            if (result > 0)
                            {
                                if (MeSummary.Text == "")
                                {
                                    MessageBox.Show("Meeting summary updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                }
                                else
                                {
                                    MessageBox.Show("Meeting summary updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                }
                                LoadMeetingData(); // Refresh the DataGridView
                                ClearMeetingFields(); // Clear the input fields
                            }
                            else
                            {
                                MessageBox.Show("Failed to add meeting. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a meeting from the list to update.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void rembtninfo_Click(object sender, EventArgs e)
        {
            if (dgvMeetings.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a meeting to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvMeetings.SelectedRows[0];
            string meetingId = selectedRow.Cells["meetingId"].Value.ToString();

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this meeting?",
                                                        "Confirm Deletion",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);

            if (dialogResult == DialogResult.No)
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Meeting WHERE meetingId = @meetingId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@meetingId", meetingId);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Meeting deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadMeetingData();
                            ClearMeetingFields();
                        }
                        else
                        {
                            MessageBox.Show("Meeting not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clrbtninfo_Click(object sender, EventArgs e)
        {
            ClearMeetingFields();
        }

        private void printbtninfo_Click(object sender, EventArgs e)
        {

            Form1 reportForm = new Form1();

            // Optionally, pass parameters or set a data source if needed
            reportForm.SetReportDataSource();  // Example function to set the report data source

            // Show the report form
            reportForm.ShowDialog();
        }

    }
}
