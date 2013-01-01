using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Xps.Packaging;
using CodeReason.Reports;
using LinearProgramming.Model;
using LinearProgramming.Solver;

namespace LinearProgramming.LPReport
{
    /// <summary>
    /// Interaction logic for LPReportWindow.xaml
    /// </summary>
    public partial class LPReportWindow
    {
        private readonly LPModel _model;
        private bool _firstActivated = true;

        public LPReportWindow(LPModel innerModel)
        {
            InitializeComponent();
            _model = new LPModel(innerModel);
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            if (!_firstActivated) return;

            _firstActivated = false;

            try
            {
                var reportDocument = new ReportDocument();

                String appStartPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

                var reader =
                    new StreamReader(new FileStream(appStartPath + @"\LPReport\ReportTemplate\StandardTemplate.xaml", FileMode.Open,
                                                    FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory,appStartPath + @"\LPReport\ReportTemplate");
                reader.Close();

                var data = new ReportData();
                data.ReportDocumentValues.Add("PrintDate", DateTime.Now); // print date is now


                List<LPConstraint> constraint = _model.GetConstraint;
                string strConstraints = constraint.Aggregate("",
                                                             (current, variable) =>
                                                             current +
                                                             String.Format("{0}{1}", variable.ToString(),
                                                                           Environment.NewLine));


                data.ReportDocumentValues.Add("ModelGoal", _model.GoalKind.ToString() + " : " + _model.Objective);
                // print date is now
                data.ReportDocumentValues.Add("ModelConstraints", strConstraints); // print date is now


                IModelSolver solver = new MicrosoftSolverFoundation(_model);
                solver.TrySolve();


                string result = solver.GetResult();

                result = result.Substring(result.IndexOf("===Solution Details===", System.StringComparison.Ordinal));

                data.ReportDocumentValues.Add("ReportSec", result);

                //TABLE Goes Here


                DateTime dateTimeStart = DateTime.Now; // start time measure here

                XpsDocument xps = reportDocument.CreateXpsDocument(data);
                documentViewer.Document = xps.GetFixedDocumentSequence();

                // show the elapsed time in window title
                Title += string.Format(" - Generated in {0}ms", (DateTime.Now - dateTimeStart).TotalMilliseconds);
            }
            catch (Exception)
            {
            }
        }
    }
}