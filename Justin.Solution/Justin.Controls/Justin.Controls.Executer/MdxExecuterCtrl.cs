﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices.AdomdClient;
using ICSharpCode.TextEditor.Document;
using Justin.FrameWork.Extensions;
using Justin.FrameWork.Helper;
using System.IO;
using Justin.FrameWork.WinForm.FormUI;
using Justin.FrameWork.WinForm.Models;
namespace Justin.Controls.Executer
{
    public partial class MdxExecuterCtrl : JUserControl, IFile
    {
        public MdxExecuterCtrl()
        {
            InitializeComponent();
            this.LoadAction = (fileName) =>
            {
                this.txtMdx.LoadFile(fileName);
            };
            this.SaveAction = (fileName) =>
            {
                this.txtMdx.SaveFile(fileName);
            };
        }

        public static string DefaultConnStr { get; set; }
        public AdomdConnection Connection
        {
            get
            {
                AdomdConnection connection = null;
                try
                {
                    connection = new AdomdConnection(txtConnectionString.Text);
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    this.ShowMessage("连接OLAP成功:" + txtConnectionString.Text);
                }
                catch (Exception ex)
                {
                    this.ShowMessage("连接OLAP失败", ex.ToString());
                }

                return connection;
            }
        }

        private void btnConnectOLAP_Click(object sender, EventArgs e)
        {
            var conn = Connection;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            try
            {
                gvMdxresult.DataSource = null;
                string mdx = txtMdx.Text;
                if (txtMdx.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected)
                {
                    mdx = txtMdx.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
                }
                CellSet cst = MdxHelper.ExecuteCellSet(Connection, mdx);


                bool useFormattedValue = sender == this.btnExecute;
                DataTable dt = cst.ToDataTable(useFormattedValue);
                gvMdxresult.DataSource = dt;
                ShowResult(dt);
            }
            catch (Exception ex)
            {
                this.ShowMessage(string.Format("Mdx查询出错{0},", ex.ToString()));
            }
        }

        private void MdxExecuterCtrl_Load(object sender, EventArgs e)
        {
            txtMdx.Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy("TSQL");
            txtMdx.Encoding = Encoding.GetEncoding("GB2312");
            this.SetToolTipsForButton(new ToolTip());
        }

        public override string ConnStr
        {
            get
            {
                return txtConnectionString.Text;
            }
            set
            {
                txtConnectionString.Text = value;
            }
        }

        private void btnExecuteDataSet_Click(object sender, EventArgs e)
        {
            try
            {
                gvMdxresult.DataSource = null;
                string mdx = txtMdx.Text;
                if (txtMdx.ActiveTextAreaControl.TextArea.SelectionManager.HasSomethingSelected)
                {
                    mdx = txtMdx.ActiveTextAreaControl.TextArea.SelectionManager.SelectedText;
                }

                DataTable dt = MdxHelper.ExecuteDataTable(Connection, mdx);
                gvMdxresult.DataSource = dt;
                ShowResult(dt);
            }
            catch (Exception ex)
            {
                this.ShowMessage(string.Format("Mdx查询出错{0},", ex.ToString()));
            }
        }

        public string Extension
        {
            get { return ".mdx"; }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.txtConnectionString.Text = "Provider=mondrian;Data Source=http://localhost:8080/mondrian_mssql/xmla;Initial Catalog=gtp;";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.gvMdxresult.DataSource = null;
            this.txtResult.Text = "";
        }

        private void ShowResult(DataTable dt)
        {
            txtResult.Text = string.Format("查询结果:{0}行/{1}列,", dt == null ? 0 : dt.Rows.Count, dt == null ? 0 : dt.Columns.Count);
        }

        private void btnDefaultConnStr_Click(object sender, EventArgs e)
        {
            this.txtConnectionString.Text = DefaultConnStr;
        }

    }
}
