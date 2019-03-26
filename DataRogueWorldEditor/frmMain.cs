using System;
using System.Windows.Forms;
using data_rogue_core;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems;
using data_rogue_core.Systems.Interfaces;
using DataRogueWorldEditor.Editors;
using WeifenLuo.WinFormsUI.Docking;

namespace DataRogueWorldEditor
{
    public partial class frmMain : Form
    {
        public ISystemContainer SystemContainer;

        private DockPanel dockPanel;
        private IEntityEngine EntityEngineSystem { get; } = new EntityEngine(new FolderDataProvider());

        public frmMain()
        {
            InitializeComponent();

            SystemContainer = new SystemContainer(new FolderDataProvider(), new NullDataProvider());

            SystemContainer.CreateSystems("EDITOR");

            SystemContainer.EntityEngine.Initialise(SystemContainer);

            dockPanel = new DockPanel();
            dockPanel.Dock = DockStyle.Fill;
            Controls.Add(dockPanel);
            dockPanel.BringToFront();
        }

        private void OpenMapEditor(string filename)
        {
            var frmMapEditor = new frmMapEditor(SystemContainer, filename);

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
