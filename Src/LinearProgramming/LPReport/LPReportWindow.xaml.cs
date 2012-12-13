using System;
using System.IO;
using System.Windows.Xps.Packaging;
using CodeReason.Reports;

namespace LinearProgramming.LPReport
{
    /// <summary>
    /// Interaction logic for LPReportWindow.xaml
    /// </summary>
    public partial class LPReportWindow
    {
        private bool _firstActivated = true;

        public LPReportWindow()
        {
            InitializeComponent();
        }

        private void WindowActivated(object sender, EventArgs e)
        {
            if (!_firstActivated) return;

            _firstActivated = false;

            try
            {
                var reportDocument = new ReportDocument();

                var reader =
                    new StreamReader(new FileStream(@"LPReport\ReportTemplate\StandardTemplate.xaml", FileMode.Open,
                                                    FileAccess.Read));
                reportDocument.XamlData = reader.ReadToEnd();
                reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"LPReport\ReportTemplate");
                reader.Close();

                var data = new ReportData();
                data.ReportDocumentValues.Add("PrintDate", DateTime.Now); // print date is now

                //TABLE Goes Here


                DateTime dateTimeStart = DateTime.Now; // start time measure here

                XpsDocument xps = reportDocument.CreateXpsDocument(data);
                documentViewer.Document = xps.GetFixedDocumentSequence();

                // show the elapsed time in window title
                Title += string.Format(" - Generated in {0}ms", (DateTime.Now - dateTimeStart).TotalMilliseconds);
            }
            catch (Exception exp)
            {
            }
        }
    }
}