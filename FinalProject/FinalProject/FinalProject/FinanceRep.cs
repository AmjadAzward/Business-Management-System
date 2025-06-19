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
using CrystalDecisions.ReportAppServer.ReportDefModel;
using CrystalDecisions.Shared;
using ReportDocument = CrystalDecisions.CrystalReports.Engine.ReportDocument;
using ReportObject = CrystalDecisions.CrystalReports.Engine.ReportObject;
using SubreportObject = CrystalDecisions.CrystalReports.Engine.SubreportObject;


namespace FinalProject
{
    public partial class FinanceRep : Form
    {
        public FinanceRep()
        {
            InitializeComponent();
        }







        public void ApplyFiltersToReport(ReportDocument report)
        {
            try
            {
                // Retrieve Main Report Parameters using ParameterDiscreteValue
                int year = GetParameterValue<int>(report, "Year");
                int month = GetParameterValue<int>(report, "Month");
                int week = GetParameterValue<int>(report, "Week");
                int day = GetParameterValue<int>(report, "Day");

                // Apply Filter to Main Report
                ApplyFilterToMainReport(report, year, month, week, day);

                // Apply Filter to Subreport 1 (ClientPayments)
                ApplyFilterToSubreport(
                    report.Subreports["Client"], // Replace with your subreport name
                    "ClientPayments.finalCost",
                    year,
                    month,
                    week,
                    day
                );

                // Apply Filter to Subreport 2 (OrganizationalPayment)
                ApplyFilterToSubreport(
                    report.Subreports["Organization"], // Replace with your subreport name
                    "OrganizationalPayment.typeCost",
                    year,
                    month,
                    week,
                    day
                );

                // Refresh the main report
                report.Refresh();
                Console.WriteLine("Filters applied successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        private T GetParameterValue<T>(ReportDocument report, string parameterName)
        {
            try
            {
                ParameterFieldDefinitions parameterFields = report.DataDefinition.ParameterFields;
                ParameterFieldDefinition parameterField = parameterFields[parameterName];
                ParameterValues currentValues = parameterField.CurrentValues;

                if (currentValues.Count > 0 && currentValues[0] is ParameterDiscreteValue discreteValue)
                {
                    return (T)Convert.ChangeType(discreteValue.Value, typeof(T));
                }
                else
                {
                    throw new Exception($"Parameter '{parameterName}' does not have a discrete value.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving parameter '{parameterName}': {ex.Message}");
            }
        }

        private void ApplyFilterToMainReport(ReportDocument report, int year, int month, int week, int day)
        {
            try
            {
                string mainSelectionFormula = $"Year({{ClientPayments.finalCost}}) = {year} AND Month({{ClientPayments.finalCost}}) = {month}";
                report.RecordSelectionFormula = mainSelectionFormula;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying filter to main report: {ex.Message}");
            }
        }

        private void ApplyFilterToSubreport(ReportDocument subreport, string fieldName, int year, int month, int week, int day)
        {
            try
            {
                // Construct Record Selection Formula for Subreport
                string selectionFormula = $"Year({{{fieldName}}}) = {year} AND Month({{{fieldName}}}) = {month}";

                // Uncomment the following lines for Week or Day filtering if needed
                // selectionFormula += $" AND Week({{{fieldName}}}) = {week}";
                // selectionFormula += $" AND Day({{{fieldName}}}) = {day}";

                // Apply the Record Selection Formula
                subreport.RecordSelectionFormula = selectionFormula;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying filter to subreport: {ex.Message}");
            }
        }




        private void crystalReportViewer1_Load(object sender, EventArgs e)
            {

            }



        } }

