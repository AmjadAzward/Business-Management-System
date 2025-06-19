using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace FinalProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void SetReportDataSource()
        {
            try
            {
                // Create an instance of your Crystal Report
                ReportDocument reportDocument = new ReportDocument();

                // Load the Crystal Report file
                reportDocument.Load(@"C:\Users\amjad\Documents\Final projects datas\FinalProject\FinalProject\FinalProject\CrystalReport1.rpt");
                reportDocument.SetDatabaseLogon("","","Ilma_A","finalPJS");
                // Optionally, set the data source for the report (e.g., a dataset, datatable, or other data)
                // Example: reportDocument.SetDataSource(yourDataSource);

                // Set the report source for the Crystal Report Viewer
                crystalReportViewer1.ReportSource = reportDocument;
                crystalReportViewer1.RefreshReport();

            }
            catch (LogOnException ex)
            {
                // Handle logon exception silently or log it
                // You can log the error or silently fail without showing the message box
                Console.WriteLine("Hi " );
                // Optionally, show a custom message or take any other action
            }
            catch (Exception ex)
            {
                // Handle other exceptions (if needed)
                Console.WriteLine("An error occurred: " + ex.Message);
            }

        }
        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void crystalReportViewer1_Load_1(object sender, EventArgs e)
        {

        }
    }
}
