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
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace FinalProject
{
    public partial class MeetingSchedule : Form
    {
        string connectionString = "Server=Ilma_A;Database=finalPJS;Trusted_Connection=True;";
        private string currentUsername;

        string loggedinuser = "";
        public MeetingSchedule(string userName)
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


        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
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

        private void MeetingSchedule_Load(object sender, EventArgs e)
        {
            LoadMeetingData();
            dtpMeetingDate.Value = DateTime.Now;

            LoadEmployeesToListBox();
        }

        private void LoadMeetingData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT meetingId, meetingTitle, meetingDate, meetingParticipants FROM Meeting";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvMeeting.DataSource = dataTable;

                    dgvMeeting.Columns["meetingParticipants"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    //dgvMeeting.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    dgvMeeting.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;



                    foreach (DataGridViewRow row in dgvMeeting.Rows)
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


        private string GenerateMeetingId()
        {
            string newId = "M001";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 meetingId FROM Meeting ORDER BY CAST(SUBSTRING(meetingId, 2, LEN(meetingId)) AS INT) DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string lastId = result.ToString();

                            int numericPart = int.Parse(lastId.Substring(1));
                            newId = "M" + (numericPart + 1).ToString("D3");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Meeting ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newId;
        }

        private void addbtnmet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMeetingTitle.Text.Trim()))
            {
                MessageBox.Show("Meeting Title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMeetingTitle.Focus();
                return;
            }

            if (dtpMeetingDate.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Meeting Date must not be in the past.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtpMeetingDate.Focus();
                return;
            }

            if (lstParticipants.Items.Count == 0)
            {
                MessageBox.Show("At least one participant is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string meetingId = GenerateMeetingId();
            string participants = GetParticipantsAsString();
            if (string.IsNullOrEmpty(participants)) return;



            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Meeting (meetingId, meetingTitle, meetingDate, meetingParticipants) " +
                                   "VALUES (@meetingId, @meetingTitle, @meetingDate, @meetingParticipants)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@meetingId", meetingId);
                        command.Parameters.AddWithValue("@meetingTitle", txtMeetingTitle.Text.Trim());
                        command.Parameters.AddWithValue("@meetingDate", dtpMeetingDate.Value.Date);
                        command.Parameters.AddWithValue("@meetingParticipants", participants);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Meeting added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadMeetingData();
                            ClearMeetingInputs();
                        }
                        else
                        {
                            MessageBox.Show("Failed to add meeting. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void updbtnmet_Click(object sender, EventArgs e)
        {
            if (dgvMeeting.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a meeting to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvMeeting.SelectedRows[0];
            string meetingId = selectedRow.Cells["meetingId"].Value.ToString();

            if (string.IsNullOrEmpty(txtMeetingTitle.Text.Trim()))
            {
                MessageBox.Show("Meeting Title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMeetingTitle.Focus();
                return;
            }

            if (dtpMeetingDate.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Meeting Date must not be in the past.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtpMeetingDate.Focus();
                return;
            }

            if (lstParticipants.Items.Count == 0)
            {
                MessageBox.Show("At least one participant is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string participants = GetParticipantsAsString();
            if (string.IsNullOrEmpty(participants)) return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Meeting SET meetingTitle = @meetingTitle, meetingDate = @meetingDate, " +
                                   "meetingParticipants = @meetingParticipants WHERE meetingId = @meetingId";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@meetingId", meetingId);
                        command.Parameters.AddWithValue("@meetingTitle", txtMeetingTitle.Text.Trim());
                        command.Parameters.AddWithValue("@meetingDate", dtpMeetingDate.Value.Date);
                        command.Parameters.AddWithValue("@meetingParticipants", participants);

                        connection.Open();
                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Meeting updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadMeetingData();
                            ClearMeetingInputs();
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

        private void rembtnmet_Click(object sender, EventArgs e)
        {
            if (dgvMeeting.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a meeting to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvMeeting.SelectedRows[0];
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
                            ClearMeetingInputs();
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

        private string GetParticipantsAsString()
        {
            List<string> selectedParticipants = new List<string>();

            foreach (var item in lstParticipants.SelectedItems)
            {
                string participant = item.ToString();
                selectedParticipants.Add(participant);
            }
            if (selectedParticipants.Count == 0)
            {
                MessageBox.Show("Please select at least one participant.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            return string.Join(",", selectedParticipants);
        }




        private void dgvMeeting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void ClearMeetingInputs()
        {
            txtMeetingTitle.Clear();
            dtpMeetingDate.Value = DateTime.Now;
            lstParticipants.Items.Clear();

        }

        private void clrbtnmet_Click(object sender, EventArgs e)
        {
            ClearMeetingInputs();
        }

        private void LoadEmployeesToListBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT CONCAT(firstname, ' ', lastname) AS fullName FROM Employee";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader;

                    connection.Open();
                    reader = command.ExecuteReader();

                    lstParticipants.Items.Clear();

                    while (reader.Read())
                    {
                        lstParticipants.Items.Add(reader["fullName"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading participants: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            SECp mainForm = new SECp(currentUsername);
            mainForm.Show();
            this.Hide();
        }

        private void dgvMeeting_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dgvMeeting.Rows[e.RowIndex];
                txtMeetingTitle.Text = selectedRow.Cells["meetingTitle"].Value.ToString();
                dtpMeetingDate.Value = Convert.ToDateTime(selectedRow.Cells["meetingDate"].Value);

                string participants = selectedRow.Cells["meetingParticipants"].Value?.ToString();

                if (!string.IsNullOrEmpty(participants))
                {
                    string[] participantArray = participants.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    lstParticipants.ClearSelected();

                    foreach (string participant in participantArray)
                    {
                        string trimmedParticipant = participant.Trim();
                        if (!lstParticipants.Items.Contains(trimmedParticipant))
                        {
                            lstParticipants.Items.Add(trimmedParticipant);
                        }
                    }

                    foreach (string participant in participantArray)
                    {
                        int index = lstParticipants.Items.IndexOf(participant.Trim());
                        if (index >= 0)
                        {
                            lstParticipants.SetSelected(index, true);  
                        }
                    }
                }
            }

        }

        private void lstParticipants_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
