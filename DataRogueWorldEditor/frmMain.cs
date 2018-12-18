﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using data_rogue_core;
using data_rogue_core.EntitySystem;
using DataRogueWorldEditor.Editors;
using WeifenLuo.WinFormsUI.Docking;

namespace DataRogueWorldEditor
{
    public partial class frmMain : Form
    {
        private DockPanel dockPanel;
        private IEntityEngine EntityEngineSystem { get; set; } = new EntityEngineSystem(new FolderEntityLoader());

        public frmMain()
        {
            InitializeComponent();

            EntityEngineSystem.Initialise();

            dockPanel = new DockPanel();
            dockPanel.Dock = DockStyle.Fill;
            Controls.Add(dockPanel);
            dockPanel.BringToFront();
        }

        private void OpenMapEditor(string filename)
        {
            var frmMapEditor = new frmMapEditor(filename, EntityEngineSystem);

            frmMapEditor.Show(dockPanel, DockState.Document);
        }

        private void itemSave_Click(object sender, EventArgs e)
        {
            var editor = dockPanel.ActiveContent as IEditor;

            if (editor != null)
            {
                editor.Save();
            }
        }

        private void itemOpenMap_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();

            open.Filter = "data-rogue map file|*.map";
            open.Title = "Open Map";

            open.ShowDialog();

            if (!string.IsNullOrEmpty(open.FileName))
            {
                OpenMapEditor(open.FileName);
            }
        }

        private void itemSaveAs_Click(object sender, EventArgs e)
        {
            var editor = dockPanel.ActiveContent as IEditor;

            if (editor != null)
            {
                editor.SaveAs();
            }
        }

        private void itemNewMap_Click(object sender, EventArgs e)
        {
            OpenMapEditor(null);
        }
    }
}
