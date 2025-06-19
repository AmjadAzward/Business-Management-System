using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalProject
{
    public partial class InventoryReport : Form
    {
        public InventoryReport()
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
                reportDocument.Load(@"C:\Users\amjad\Documents\Final projects datas\FinalProject\FinalProject\FinalProject\CrystalReport3.rpt");
               
                crystalReportViewer1.ReportSource = reportDocument;
                crystalReportViewer1.RefreshReport();

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
    }
}
