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


namespace FinalProject
{
    public partial class Form5 : Form
    {
        public Form5()
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



        public void GenerateReport()
        {
            // Prompt the user for the report type using an InputBox
            string userInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the report type: Day, Month, or Week",
                "Select Report Type",
                "Day"); // Default value is "Day"

            // Validate the user's input
            string selectedReportType = userInput.Trim().ToLower();
            if (selectedReportType != "day" && selectedReportType != "month" && selectedReportType != "week")
            {
                MessageBox.Show("Invalid input. Please enter 'Day', 'Month', or 'Week'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method if the input is invalid
            }

            // Create a new report document
            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(@"C:\Users\amjad\Documents\Final projects datas\FinalProject\FinalProject\FinalProject\CrystalReport3.rpt");

            // Set the selection formula based on the user's input
            string selectionFormula = "";

            switch (selectedReportType)
            {
                case "day":
                    selectionFormula = "{Inventory.productAddedDate} = {?Day}"; // Replace {?Day} with user input
                    break;
                case "month":
                    selectionFormula = "Year({Inventory.productAddedDate}) = {?Year} AND Month({Inventory.productAddedDate}) = {?Month}"; // Replace {?Year} and {?Month} with user input
                    break;
                case "week":
                    selectionFormula = "Year({Inventory.productAddedDate}) = {?Year} AND DatePart('ww', {Inventory.productAddedDate}) = {?Week}"; // Replace {?Year} and {?Week} with user input
                    break;
            }

            // Apply the selection formula to the report
            reportDocument.RecordSelectionFormula = selectionFormula;

            // Set parameters (replace with actual data as needed)
            reportDocument.SetParameterValue("Day", DateTime.Now);  // Example for setting the Day parameter
            reportDocument.SetParameterValue("Year", DateTime.Now.Year);  // Example for setting the Year parameter
            reportDocument.SetParameterValue("Month", DateTime.Now.Month);  // Example for setting the Month parameter
            reportDocument.SetParameterValue("Week", 1);  // Example for setting the Week parameter

            // Set the report source and refresh
            crystalReportViewer1.ReportSource = reportDocument;
            crystalReportViewer1.RefreshReport();
        }



    }
}
