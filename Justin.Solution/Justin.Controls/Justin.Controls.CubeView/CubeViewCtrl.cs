﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Justin.FrameWork.WinForm.FormUI;
using Microsoft.AnalysisServices.AdomdClient;

namespace Justin.Controls.CubeView
{
    public partial class CubeViewCtrl : JUserControl
    {
        public CubeViewCtrl()
        {
            InitializeComponent();
        }
        public static string DefaultConnStr { get; set; }
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
        CubeOperate co;
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


        private void CubeViewCtrl_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtConnectionString.Text))
                this.txtConnectionString.Text = CubeViewCtrl.DefaultConnStr;


        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            co = new CubeOperate(this.ConnStr);
            BindCategroy();
        }

        #region 服务器连接信息

        private void BindCategroy()
        {
            TreeNode tvCategroyInfo = new TreeNode("Categroy");
            tvCategroyInfo.Name = "Categroy";
            tvCategroyInfo.SelectedImageKey = tvCategroyInfo.ImageKey = "Categroy";
            tvCategroyInfo.Nodes.Add("Cubes_", "Cubes", "Cubes", "Cubes");
            tvCategroyInfo.Nodes.Add("Dimensions_", "Dimensions", "Dims", "Dims");
            tvServerInfo.Nodes.Add(tvCategroyInfo);

            BindServerCubes(tvCategroyInfo.Nodes["Cubes_"], co.Cubes);
            BindServerDimensions(tvCategroyInfo.Nodes["Dimensions_"], co.Dimensions);
        }
        private void BindServerCubes(TreeNode cubeNodeRoot, IEnumerable<CubeDef> cubes)
        {
            cubeNodeRoot.Nodes.Clear();
            if (cubes == null) return;
            foreach (var item in cubes)
            {
                string name = item.Name;//.Replace("$", "");
                string caption = item.Caption;//.Replace("$", "");
                TreeNode tempNode = new TreeNode(caption);
                tempNode.Name = name;
                tempNode.SelectedImageKey = tempNode.ImageKey = "Cube";
                tempNode.ToolTipText = string.Format("Name:[{0}]Caption:[{1}]", item.Name, item.Caption);
                cubeNodeRoot.Nodes.Add(tempNode);
            }
        }
        private void BindServerDimensions(TreeNode dimensionNodeRoot, IEnumerable<CubeDef> dimensions)
        {
            dimensionNodeRoot.Nodes.Clear();
            if (dimensions == null) return;
            foreach (var item in dimensions)
            {
                string name = item.Name.Replace("$", "");
                string caption = item.Caption.Replace("$", "");
                TreeNode tempNode = new TreeNode(caption);
                tempNode.Name = name;
                tempNode.SelectedImageKey = tempNode.ImageKey = "Dim";
                tempNode.ToolTipText = string.Format("Name:[{0}]Caption:[{1}]", item.Name, item.Caption);
                dimensionNodeRoot.Nodes.Add(tempNode);
            }
        }
        #endregion

        #region 加载单个Cube信息

        private void BindCubeInfo(string cubeName)
        {
            CubeDef cubeDef = co.GetCube(cubeName);
            tvCubeInfo.Nodes.Clear();
            tvCubeInfo.Nodes.Add("Cube_", "Cube", "Cube", "Cube");
            tvCubeInfo.Nodes[0].Nodes.Add("Measures_", "Measures", "Measure", "Measure");

            BindMeasuresForCube(co.GetMeasures(cubeName));
            BindDimensionsForCube(cubeDef);

        }
        private TreeNode CubeNode
        {
            get
            {
                return tvCubeInfo.Nodes[0];
            }
        }
        private TreeNode MeasuresRoot
        {
            get
            {
                return tvCubeInfo.Nodes[0].Nodes["Measures_"];
            }
        }
        private void BindMeasuresForCube(IEnumerable<Measure> measures)
        {
            MeasuresRoot.Nodes.Clear();

            if (measures == null) return;

            foreach (var item in measures)
            {
                string name = item.Name;//.Replace("$", "");
                string caption = item.Caption;//.Replace("$", "");
                TreeNode tempNode = new TreeNode(caption);
                tempNode.Name = name;
                tempNode.Tag = item;
                tempNode.SelectedImageKey = tempNode.ImageKey = "Measure";
                tempNode.ToolTipText = string.Format("Name:[{0}]Caption:[{1}]", item.Name, item.Caption);
                MeasuresRoot.Nodes.Add(tempNode);
            }
        }

        private void BindDimensionsForCube(CubeDef cubeDef)
        {
            IEnumerable<Dimension> dimensions = cubeDef.Dimensions.Cast<Dimension>();
            if (dimensions == null) return;
            for (int i = CubeNode.Nodes.Count - 1; i >= 0; i--)
            {
                if (!CubeNode.Nodes[i].Name.Equals("Measures_", StringComparison.CurrentCultureIgnoreCase))
                {
                    CubeNode.Nodes.RemoveAt(i);
                }
            }

            foreach (var item in dimensions)
            {
                if (item.Name.Equals("Measures") || item.Caption.Equals("Measures")) continue;
                string name = item.Name.Replace("$", "");
                string caption = item.Caption.Replace("$", "");
                TreeNode tempNode = new TreeNode(caption);
                tempNode.Name = name;
                tempNode.SelectedImageKey = tempNode.ImageKey = "Dim";
                BindHierarchies(tempNode, item.Hierarchies);
                CubeNode.Nodes.Add(tempNode);
            }
        }
        private void BindHierarchies(TreeNode dimNode, HierarchyCollection hierarchies)
        {
            if (hierarchies == null || hierarchies.Count == 0) return;

            foreach (var hierarchy in hierarchies)
            {
                string name = hierarchy.Name.Replace("$", "");
                string caption = hierarchy.Caption.Replace("$", "");
                TreeNode tempNode = new TreeNode(caption);
                tempNode.Name = name;
                tempNode.SelectedImageKey = tempNode.ImageKey = hierarchy.Levels.Count > 2 ? "Hie" : "Level";
                BindLevels(tempNode, hierarchy.Levels);
                dimNode.Nodes.Add(tempNode);
            }
        }
        private void BindLevels(TreeNode root, LevelCollection levels)
        {
            if (levels == null || levels.Count == 0) return;

            foreach (var level in levels)
            {
                string name = level.Name.Replace("$", "");
                string caption = level.Caption.Replace("$", "");
                TreeNode tempNode = new TreeNode(caption);
                tempNode.Name = name;
                tempNode.SelectedImageKey = tempNode.ImageKey = "Level";
                tempNode.Tag = level;
                root.Nodes.Add(tempNode);
            }
        }

        private void ExpendMembers(TreeNode root)
        {
            if (!root.ImageKey.Equals("Level") && !root.ImageKey.Equals("Member")) return;
            if (root.Nodes.Count > 0) return;
            MemberCollection members = null;
            if (root.ImageKey.Equals("Level"))
            {
                Level level = root.Tag as Level;
                members = level.GetMembers();


            }
            else if (root.ImageKey.Equals("Member"))
            {
                Member member = root.Tag as Member;
                members = member.GetChildren();
            }
            else
            {
                return;
            }
            if (members == null || members.Count <= 0) return;
            foreach (var member in members)
            {
                string name = member.Name.Replace("$", "");
                string caption = member.Caption.Replace("$", "");
                TreeNode tempNode = new TreeNode(caption);
                tempNode.Name = name;
                tempNode.SelectedImageKey = tempNode.ImageKey = "Member";
                tempNode.Tag = member;

                root.Nodes.Add(tempNode);
            }

            root.Expand();
        }




        #endregion

        #region Treeview 操作

        private void tvServerInfo_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvServerInfo.SelectedNode = e.Node;
        }
        private void tvServerInfo_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvServerInfo.SelectedNode.Parent != null && tvServerInfo.SelectedNode.Parent.Name.Equals("Cubes_"))
            {
                BindCubeInfo(tvServerInfo.SelectedNode.Name);
            }
            if (string.IsNullOrEmpty(tvServerInfo.SelectedNode.Name))
            {
                MessageBox.Show("节点Name不能为空");
            }
        }
        private void tvServerInfo_ItemDrag(object sender, ItemDragEventArgs e)
        {
            //DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void browerCubeInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvServerInfo.SelectedNode.Parent != null && tvServerInfo.SelectedNode.Parent.Name.Equals("Cubes_"))
            {
                BindCubeInfo(tvServerInfo.SelectedNode.Name);
            }
        }

        private void tvCubeInfo_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            tvCubeInfo.SelectedNode = e.Node;
        }
        private void tvCubeInfo_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (string.IsNullOrEmpty(tvCubeInfo.SelectedNode.Name))
            {
                MessageBox.Show("节点Name不能为空");
            }
        }
        private void tvCubeInfo_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move | DragDropEffects.Copy);
        }

        private void tvCubeInfo_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && (e.Node.ImageKey == "Level" || e.Node.ImageKey == "Member"))
            {
                ExpendMembers(e.Node);
            }
        }
        #endregion




    }
}
