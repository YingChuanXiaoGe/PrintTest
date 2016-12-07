using Microsoft.Reporting.WinForms;
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

namespace _2016中招理化生准考证
{
    public partial class frmPrint : Form
    {
        public DataTable dtSource;
        public frmPrint()
        {
            InitializeComponent();
        }

        private void frmPrint_Load(object sender, EventArgs e)
        {
            string picPath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), @"Photos\");
            reportViewer1.LocalReport.ReportPath = @"..\..\rpCardReport.rdlc";
            reportViewer1.LocalReport.EnableExternalImages = true;
            reportViewer1.LocalReport.SetParameters(new ReportParameter[] {
            new ReportParameter("CurrentPath",picPath)
            });
            ReportDataSource rds = new ReportDataSource("DataSet1", dtSource);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            reportViewer1.RefreshReport();
        }
    }
}
