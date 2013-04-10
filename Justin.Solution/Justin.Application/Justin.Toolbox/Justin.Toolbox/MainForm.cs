﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Justin.FrameWork.Settings;
using Justin.Toolbox.Tools;
using WeifenLuo.WinFormsUI.Docking;
using Justin.FrameWork.Extensions;
using Justin.BI.DBLibrary.TestDataGenerate;
using Justin.BI.DBLibrary.Utility;
using Justin.Core;

namespace Justin.Toolbox
{
    public partial class MainForm : WorkbenchXXX
    {
        #region 加载上次打开的文件

        /// <summary>
        /// 加载上次打开的文件
        /// </summary>
        public MainForm()
            : base()
        {
            InitializeComponent();
        }

        #endregion

        #region 右键直接打开文件

        public MainForm(string fileName)
            : base(fileName)
        {
            InitializeComponent();
        }

        #endregion

        bool forceClose = false;
        private OutPutWindow OutPutWin = new OutPutWindow();
        public DockPanel DockPanel
        {
            get
            {
                return this.dockPanel;
            }
        }

        #region Form事件

        private void MainForm_Load(object sender, EventArgs e)
        {
            //base.LoadForm();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
           // base.ClosingForm(sender, e);
            //if (!forceClose)
            //{
            //    e.Cancel = true;
            //    this.Hide();
            //}
            //else
            //{

            //    if (m_bSaveLayout && !specialFile)
            //    {
            //        string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            //        dockPanel.SaveAsXml(configFile);
            //    }
            //}
        }

        #endregion


        #region 函数 (业务相关)


        protected override IDockContent NewToolAccrodingPersistString(string[] parsedStrings)
        {
            if (parsedStrings[0] == typeof(TableConfigurator).ToString())
            {
                #region TableConfigurator

                if (parsedStrings.Length != 3)
                    return null;

                string tableName = parsedStrings[1];
                string fileName = JTools.GetFileName(tableName, FileType.TableConfig);

                if (!File.Exists(fileName))
                {
                    this.ShowMessage("文件不存在", string.Format("文件{0}不存在", fileName));
                }
                else
                {
                    JTable table = JTools.ReadTableSettingByFile(fileName);
                    TableConfigurator tableConfig = new TableConfigurator(table);
                    return tableConfig;
                }

                #endregion
            }
            else if (parsedStrings[0] == typeof(TestDataGenerator).ToString())
            {
                #region TestDataGenerator

                if (parsedStrings.Length != 2)
                    return null;

                TestDataGenerator testDataGenerator = new TestDataGenerator(parsedStrings[1]);
                return testDataGenerator;

                #endregion
            }
            else if (parsedStrings[0] == typeof(SqlExecuteor).ToString())
            {
                #region SqlExecuteor

                if (parsedStrings.Length != 3)
                    return null;

                string fileName = parsedStrings[1];

                if (!File.Exists(fileName))
                {
                    this.ShowMessage("文件不存在", string.Format("文件{0}不存在", fileName));
                }
                SqlExecuteor sqlExecutor = new SqlExecuteor(fileName, parsedStrings[2]);
                return sqlExecutor;

                #endregion
            }
            else if (parsedStrings[0] == typeof(MdxExecutor).ToString())
            {
                #region MdxExecutor

                if (parsedStrings.Length != 3)
                    return null;

                MdxExecutor executor = new MdxExecutor(parsedStrings[1], parsedStrings[2]);
                return executor;

                #endregion
            }
            else if (parsedStrings[0] == typeof(MondrianSchemaWorkbench).ToString())
            {
                #region MondrianSchemaWorkbench

                if (parsedStrings.Length != 3)
                    return null;

                MondrianSchemaWorkbench executor = new MondrianSchemaWorkbench(parsedStrings[1], parsedStrings[2]);
                return executor;

                #endregion
            }
            else if (parsedStrings[0] == typeof(JCodeCompiler).ToString())
            {
                #region JCodeCompiler

                //if (parsedStrings.Length != 3)
                //    return null;

                JCodeCompiler executor = new JCodeCompiler();
                return executor;

                #endregion
            }
            else if (parsedStrings[0] == typeof(JsonViewer).ToString())
            {
                #region JsonViewer

                JsonViewer executor = new JsonViewer();
                return executor;

                #endregion
            }
            else if (parsedStrings[0] == typeof(CodeSnippetMgr).ToString())
            {
                #region CodeSnippetMgr

                CodeSnippetMgr executor = new CodeSnippetMgr();
                return executor;

                #endregion
            }
            return null;
        }

        //右键打开不同类型的文件
        protected override void OpenFileAccordingToFile(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName).ToLower().TrimStart('.');
            if (FileType.TableConfig.GetAllowFileExtensions().Contains(fileExtension, true))
            {
                OpenTableConfigFile(fileName);
            }
            else if (FileType.SQL.GetAllowFileExtensions().Contains(fileExtension, true))
            {
                OpenSQLFile(fileName);
            }
            else if (FileType.MDX.GetAllowFileExtensions().Contains(fileExtension, true))
            {
                OpenMDXFile(fileName);
            }
            else
            {
                this.ShowMessage("不支持此文件类型", "不支持此文件类型");
            }
        }

        private void OpenTableConfigFile(string fileName)
        {
            JTable table = JTools.ReadTableSettingByFile(fileName);
            if (table != null)
            {
                TableConfigurator configForm = new TableConfigurator(table);
                configForm.MdiParent = this;
                configForm.Show();
            }
            else
            {
                this.ShowMessage("配置文件不可识别", "所读文件不符合表配置文件规则");
            }
        }
        private void OpenSQLFile(string fileName)
        {
            SqlExecuteor sqlExecutor = new SqlExecuteor(fileName, "");
            sqlExecutor.MdiParent = this;
            sqlExecutor.Show();
        }
        private void OpenMDXFile(string fileName)
        {
            string mdx = File.ReadAllText(fileName);
            MdxExecutor executor = new MdxExecutor("", mdx);
            executor.MdiParent = this;
            executor.Show();
        }

        #endregion

    }
}
