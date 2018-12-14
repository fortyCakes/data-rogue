using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataRogueWorldEditor.Editors;
using WeifenLuo.WinFormsUI.Docking;

namespace DataRogueWorldEditor
{
    public partial class frmMain : Form
    {
        private DockPanel dockPanel;

        public frmMain()
        {
            InitializeComponent();

            dockPanel = new DockPanel();
            dockPanel.Dock = DockStyle.Fill;
            Controls.Add(dockPanel);
            dockPanel.BringToFront();
        }

        private void OpenMapEditor()
        {
            var frmMapEditor = new frmMapEditor();

            frmMapEditor.Show(dockPanel, DockState.Document);
        }

        private void newMapEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenMapEditor();
        }
    }
}
