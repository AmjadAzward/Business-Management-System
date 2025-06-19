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
    public partial class SalesRep : Form
    {
        public SalesRep()
        {
            InitializeComponent();
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
            reportDocument.Load(@"C:\Users\amjad\Documents\Final projects datas\FinalProject\FinalProject\FinalProject\CrystalReport7.rpt");

            // Set the selection formula based on the user's input
            string selectionFormula = "";

            // Default values for Day, Month, and Year
            int defaultDay = DateTime.Now.Day;
            int defaultMonth = DateTime.Now.Month;
            int defaultYear = DateTime.Now.Year;
            int defaultWeek = DateTime.Now.DayOfYear / 7 + 1; // A rough estimate of the week number, based on day of the year

            // Set parameters based on the selected report type
            switch (selectedReportType)
            {
                case "day":
                    // Modify the selection formula to compare just the day part of the productAddedDate
                    selectionFormula = "Day({Sales.salesDate}) = {?Day}";
                    // Set the Day parameter to the default value (current day number)
                    reportDocument.SetParameterValue("Day", defaultDay);
                    break;
                case "month":
                    selectionFormula = "Year({Sales.salesDate}) = {?Year} AND Month({Sales.salesDate}) = {?Month}";
                    // Set the Year and Month parameters to the default values
                    reportDocument.SetParameterValue("Year", defaultYear);
                    reportDocument.SetParameterValue("Month", defaultMonth);
                    break;
                case "week":
                    selectionFormula = "Year({Sales.salesDate}) = {?Year} AND DatePart('ww', {Sales.salesDate}) = {?Week}";
                    // Set the Year and Week parameters to the default values
                    reportDocument.SetParameterValue("Year", defaultYear);
                    reportDocument.SetParameterValue("Week", defaultWeek);
                    break;
            }

            // Apply the selection formula to the report
            reportDocument.RecordSelectionFormula = selectionFormula;

            // Set the report source and refresh
            crystalReportViewer1.ReportSource = reportDocument;
            crystalReportViewer1.RefreshReport();
        }

    }
}
