﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Justin.Core;
using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Shell;

namespace Justin.Toolbox
{
    public partial class Explorer : JForm
    {
        public Explorer()
        {
            InitializeComponent();
        }

        private void Explorer_Load(object sender, EventArgs e)
        {
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            explorerBrowser1.NavigateLogLocation(NavigationLogDirection.Backward);
        }

        private void buttonForward_Click(object sender, EventArgs e)
        {
            explorerBrowser1.NavigateLogLocation(NavigationLogDirection.Forward);
        }

        private void buttonLocation_Click(object sender, EventArgs e)
        {
            explorerBrowser1.Navigate(ShellFileSystemFolder.FromFolderPath(textBoxPath.Text));
        }
    }
}
