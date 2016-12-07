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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }
        private DataTable dtOriginal;
        private DataTable dtSchool;
        private DataTable dtClass;
        private DataTable dtCurrent;
        private void tsmiImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel文件(*.xls)|*.xls";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dtOriginal = ExcelHelper.ImportExceltoDt(ofd.FileName, 0, 1);
                DataView dv = dtOriginal.DefaultView;
                dtOriginal= dv.ToTable(true, new string[] { "报名序号","姓名","性别","身份证号","班级","学校" });
                dtCurrent = dtOriginal;
                InitDGV(dtCurrent);
                InitCboSearchSchool();
                grpSearch.Enabled = true;
                rdoName.Checked = true;
                grpPrint.Enabled = true;
            }
        }

        private void InitDGV(DataTable dt)
        {
            dataGridView1.DataSource = dt;
            chkSelectAll.Checked = false;
            if (dt!=null)
            {
                lblCount.Text = "当前学生人数：" + dt.Rows.Count;
            }
            else
            {
                lblCount.Text = "当前学生人数：0";
            }
        }

        private void cboSearchSchool_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchSchool.SelectedIndex>0)
            {
                dtSchool = (from tb in dtOriginal.AsEnumerable()
                                  where tb.Field<string>("学校") == cboSearchSchool.Text
                                  select tb).CopyToDataTable();
                dtCurrent = dtSchool;
                InitDGV(dtCurrent);
            }
            else
            {
                InitDGV(dtOriginal);

            }
            InitCboSearchClass();
        }
        private void InitCboSearchSchool()
        {
            var schoolNames = from tb in dtOriginal.AsEnumerable()
                              orderby tb.Field<string>("班级") descending
                              group tb by tb.Field<string>("学校") into x
                              select new { 学校 = x.Key };
            List<string> listSchool = new List<string>();
            listSchool.Add("全部");
            foreach (var item in schoolNames)
            {
                listSchool.Add(item.学校);
            }
            cboSearchSchool.Items.AddRange(listSchool.ToArray());
            cboSearchSchool.SelectedIndex = 0;
        }
        private void InitCboSearchClass()
        {
            if (cboSearchSchool.SelectedIndex>0)
            {
                var classNames = from tb in dtOriginal.AsEnumerable()
                                 where tb.Field<string>("学校") == cboSearchSchool.Text
                                 orderby tb.Field<string>("班级")
                                 group tb by tb.Field<string>("班级") into x
                                 select new { 班级 = x.Key };
                List<string> listClass = new List<string>();
                cboSearchClass.Items.Clear();
                listClass.Add("全部");
                foreach (var item in classNames)
                {
                    listClass.Add(item.班级);
                }
                cboSearchClass.Items.AddRange(listClass.ToArray());
                cboSearchClass.SelectedIndex = 0;
                cboSearchClass.Enabled = true;
            }
            else
            {
                cboSearchClass.Items.Clear();
                cboSearchClass.Text = "";
                cboSearchClass.Enabled = false;
            }
        }

        private void cboSearchClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboSearchClass.SelectedIndex>0)
            {
                dtClass = (from tb in dtOriginal.AsEnumerable()
                           where tb.Field<string>("学校") == cboSearchSchool.Text && tb.Field<string>("班级") == cboSearchClass.Text
                           select tb).CopyToDataTable();
                dtCurrent = dtClass;
                InitDGV(dtCurrent);
            }
            else
            {
                InitDGV(dtSchool);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (rdoName.Checked)
            {
                var temp = from tb in dtOriginal.AsEnumerable()
                           where tb.Field<string>("姓名").Contains(txtSearchContent.Text)
                           select tb;
                if (temp.Count()>0)
                {
                    dtCurrent = temp.CopyToDataTable();
                }
                else
                {
                    dtCurrent = null;
                }
            }
            else
            {
                var temp = from tb in dtOriginal.AsEnumerable()
                           where tb.Field<string>("报名序号").Contains(txtSearchContent.Text)
                           select tb;
                if (temp.Count() > 0)
                {
                    dtCurrent = temp.CopyToDataTable();
                }
                else
                {
                    dtCurrent = null;
                }
            }
            InitDGV(dtCurrent);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectAll.Checked)
            {
                dataGridView1.SelectAll();
            }
            else
            {
                dataGridView1.ClearSelection();
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            chkSelectAll.Checked = false;
            if (dataGridView1.SelectedRows.Count==dataGridView1.Rows.Count)
            {
                chkSelectAll.Checked = true;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataTable table = dtOriginal.Clone();
            table.Columns.Add("条形码", typeof(Byte[]));
            for (int i = dataGridView1.SelectedRows.Count - 1; i >= 0; i--)
            {
                DataRow dr = table.NewRow();
                string strBMXH=dataGridView1.SelectedRows[i].Cells["报名序号"].Value.ToString();
                dr["报名序号"] = strBMXH;
                dr["姓名"] = dataGridView1.SelectedRows[i].Cells["姓名"].Value.ToString();
                dr["性别"] = dataGridView1.SelectedRows[i].Cells["性别"].Value.ToString();
                dr["身份证号"] = dataGridView1.SelectedRows[i].Cells["身份证号"].Value.ToString();
                dr["班级"] = dataGridView1.SelectedRows[i].Cells["班级"].Value.ToString();
                dr["学校"] = dataGridView1.SelectedRows[i].Cells["学校"].Value.ToString();
                Bitmap bmpTemp= Common.GetCode39(strBMXH);
                byte[] bmpByte = Common.Bitmap2Byte(bmpTemp);
                dr["条形码"] = bmpByte;
                table.Rows.Add(dr);
            }
            frmPrint frm = new frmPrint();
            frm.dtSource = table;
            frm.ShowDialog();
        }
    }
}
