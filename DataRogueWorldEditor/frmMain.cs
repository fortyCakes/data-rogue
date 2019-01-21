﻿using System;
using System.Windows.Forms;
using data_rogue_core;
using data_rogue_core.Behaviours;
using data_rogue_core.EntityEngine;
using data_rogue_core.EventSystem;
using data_rogue_core.Systems;
using DataRogueWorldEditor.Editors;
using WeifenLuo.WinFormsUI.Docking;

namespace DataRogueWorldEditor
{
    public partial class frmMain : Form
    {
        public PrototypeSystem PrototypeSystem { get; }

        private IPositionSystem PositionSystem;
        private IEventSystem EventSystem;
        private DockPanel dockPanel;
        private IEntityEngine EntityEngineSystem { get; } = new EntityEngine(new FolderEntityLoader());

        public frmMain()
        {
            InitializeComponent();

            PositionSystem = new PositionSystem();
            EventSystem = new EventSystem();
            
            var behaviourFactory = new BehaviourFactory(PositionSystem, EventSystem, new RNG("whatever"));

            EntityEngineSystem.Initialise(behaviourFactory);
            
            PrototypeSystem = new PrototypeSystem(EntityEngineSystem, null, behaviourFactory);
            EntityEngineSystem.Register(PrototypeSystem);

            dockPanel = new DockPanel();
            dockPanel.Dock = DockStyle.Fill;
            Controls.Add(dockPanel);
            dockPanel.BringToFront();
        }

        private void OpenMapEditor(string filename)
        {
            var frmMapEditor = new frmMapEditor(filename, EntityEngineSystem, PrototypeSystem);

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
