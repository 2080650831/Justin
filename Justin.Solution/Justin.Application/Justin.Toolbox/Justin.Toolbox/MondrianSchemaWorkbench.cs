﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using Justin.BI.DBLibrary.TestDataGenerate;
using Justin.FrameWork.Helper;
using Justin.FrameWork.WinForm.FormUI.SharpCodeTextEditor;
using Justin.FrameWork.Settings;
using Justin.FrameWork.WinForm.FormUI.PropertyGrid;
using Justin.Core;
using Justin.FrameWork.WinForm.Models;

namespace Justin.Toolbox
{
    public partial class MondrianSchemaWorkbench : JForm, IFile
    {
        public MondrianSchemaWorkbench()
        {
            InitializeComponent();
            this.schemaViewerCtrl1.FileChanged += this.OnFileChanged;
        }
        public MondrianSchemaWorkbench(string[] args)
            : this()
        {
            if (args != null)
            {
                schemaViewerCtrl1.SchemaFileName = args[0];
                this.FileName = args[0];

            }
        }

        #region 继承


        protected override string GetPersistString()
        {
            return string.Format("{1}{0}{2}", Constants.Splitor, GetType().ToString(), this.FileName);
        }

        protected override string FileName
        {
            get
            {
                return schemaViewerCtrl1.FileName;
            }
            set
            {
                schemaViewerCtrl1.FileName = value;
                base.FileName = value;

            }
        }
        public override void SaveFile(string fileName)
        {
            base.SaveFile(fileName);
           // this.jEditorCtrl1.Save(fileName);
        }
        #endregion
    }



}
