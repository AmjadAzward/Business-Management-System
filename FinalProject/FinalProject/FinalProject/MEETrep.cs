using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;

namespace FinalProject
{
    public partial class MEETrep : Form
    {
        public MEETrep(string meetingId)
        {
            InitializeComponent();
            crystalReportViewer1 = new CrystalReportViewer();
            this.Controls.Add(crystalReportViewer1);
            //load();
        }


        public void SetReportSource(ReportDocument report)
        {
            crystalReportViewer1.ReportSource = report;
            crystalReportViewer1.Refresh();
        }


        private void load()
        {
            try
            {
                ReportDocument reportDocument = new ReportDocument();
                string ReportPath = Path.Combine(Application.StartupPath, "CrystalReport1.rpt");

                if (File.Exists(ReportPath))
                {
                    reportDocument.Load(ReportPath);

                    // Securely retrieve credentials
                    string username = ""; // Retrieve from config
                    string password = ""; // Retrieve from config
                    string serverName = "Ilma_A";
                    string databaseName = "finalPJS";

                    reportDocument.SetDatabaseLogon(username, password, serverName, databaseName);

                    crystalReportViewer1.ReportSource = reportDocument;
                }
                else
                {
                    MessageBox.Show("Report file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // Log and display error
                File.AppendAllText("error.log", $"{DateTime.Now}: {ex.Message}\n");
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            crystalReportViewer1.Refresh();

        }

        private void CrystalReport11_InitReport(object sender, EventArgs e)
        {

        }

        private void CrystalReport11_InitReport_1(object sender, EventArgs e)
        {
            crystalReportViewer1.Refresh();

        }
    }
}
