﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Justin.BI.DBLibrary.TestDataGenerate;
using Justin.BI.DBLibrary.Utility;
using Justin.Core;
using Justin.FrameWork.Helper;
using Justin.FrameWork.Settings;
using Justin.Log;

namespace Justin.Toolbox.Tools
{
    public delegate void AsyncDelegate();
    public partial class SqlExecuteor : JDBForm
    {
        public SqlExecuteor()
        {
            InitializeComponent();
        }
        /// <summary>
        ///     
        /// </summary>
        /// <param name="args" type="string[]">
        ///     <para>
        ///           0:fileName
        ///           1:ConnStr
        ///     </para>
        /// </param>
        public SqlExecuteor(string[] args):this()
        {
            if (args != null)
            {
                this.FileName = args[0]; ;
                this.ConnStr = args.Length > 1 ? args[1] : "";
            }
        }

        #region 继承
        protected override string ConnStr
        {
            get
            {
                return this.sqlExecuterCtrl1.ConnStr;
            }
            set
            {
                this.sqlExecuterCtrl1.ConnStr = value;
                base.ConnStr = value;
            }
        }
        protected override string GetPersistString()
        {
            return string.Format("{1}{0}{2}{0}{3}", Constants.Splitor, GetType().ToString(), this.FileName, this.ConnStr);
        }
        protected override bool IsFile { get { return true; } }

        protected override string FileName
        {
            get
            {
                return sqlExecuterCtrl1.FileName;
            }
            set
            {
                sqlExecuterCtrl1.FileName = value;
                base.FileName = value;

            }
        }

        #endregion



    }
}
