using DataRogueWorldEditor.Controls;

namespace DataRogueWorldEditor.Editors
{
    partial class frmMapEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.toolStripContainer2 = new System.Windows.Forms.ToolStripContainer();
            this.lblCurrentTool = new System.Windows.Forms.Label();
            this.toolsEditorTools = new System.Windows.Forms.ToolStrip();
            this.btnSetCell = new System.Windows.Forms.ToolStripButton();
            this.btnClearCell = new System.Windows.Forms.ToolStripButton();
            this.btnSelectCell = new System.Windows.Forms.ToolStripButton();
            this.toolsScroll = new System.Windows.Forms.ToolStrip();
            this.btnScrollLeft = new System.Windows.Forms.ToolStripButton();
            this.btnScrollUp = new System.Windows.Forms.ToolStripButton();
            this.btnScrollDown = new System.Windows.Forms.ToolStripButton();
            this.btnScrollRight = new System.Windows.Forms.ToolStripButton();
            this.dgvGlyphs = new System.Windows.Forms.DataGridView();
            this.Glyph = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Entity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlMap = new System.Windows.Forms.Panel();
            this.lblMapEntities = new DataRogueWorldEditor.Controls.TransparentLabel();
            this.lblMap = new System.Windows.Forms.Label();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.txtMapKey = new System.Windows.Forms.TextBox();
            this.txtDefaultCell = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvCommands = new System.Windows.Forms.DataGridView();
            this.colCommandTypes = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSelectedCell = new System.Windows.Forms.Label();
            this.toolsVisible = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMap = new System.Windows.Forms.ToolStripButton();
            this.btnEntities = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.toolStripContainer2.ContentPanel.SuspendLayout();
            this.toolStripContainer2.LeftToolStripPanel.SuspendLayout();
            this.toolStripContainer2.SuspendLayout();
            this.toolsEditorTools.SuspendLayout();
            this.toolsScroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGlyphs)).BeginInit();
            this.pnlMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommands)).BeginInit();
            this.toolsVisible.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.splitContainer1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1246, 685);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1246, 710);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolsVisible);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer1.Size = new System.Drawing.Size(1246, 685);
            this.splitContainer1.SplitterDistance = 983;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pnlMap);
            this.splitContainer2.Size = new System.Drawing.Size(983, 685);
            this.splitContainer2.SplitterDistance = 224;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.toolStripContainer2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.AutoScroll = true;
            this.splitContainer3.Panel2.Controls.Add(this.dgvGlyphs);
            this.splitContainer3.Size = new System.Drawing.Size(224, 685);
            this.splitContainer3.SplitterDistance = 385;
            this.splitContainer3.TabIndex = 0;
            // 
            // toolStripContainer2
            // 
            // 
            // toolStripContainer2.ContentPanel
            // 
            this.toolStripContainer2.ContentPanel.Controls.Add(this.lblCurrentTool);
            this.toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(196, 356);
            this.toolStripContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer2.LeftToolStripPanel
            // 
            this.toolStripContainer2.LeftToolStripPanel.Controls.Add(this.toolsEditorTools);
            this.toolStripContainer2.LeftToolStripPanel.Controls.Add(this.toolsScroll);
            this.toolStripContainer2.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer2.Name = "toolStripContainer2";
            this.toolStripContainer2.Size = new System.Drawing.Size(220, 381);
            this.toolStripContainer2.TabIndex = 1;
            this.toolStripContainer2.Text = "toolStripContainer2";
            // 
            // lblCurrentTool
            // 
            this.lblCurrentTool.AutoSize = true;
            this.lblCurrentTool.Location = new System.Drawing.Point(3, 9);
            this.lblCurrentTool.Name = "lblCurrentTool";
            this.lblCurrentTool.Size = new System.Drawing.Size(68, 13);
            this.lblCurrentTool.TabIndex = 1;
            this.lblCurrentTool.Text = "Current Tool:";
            // 
            // toolsEditorTools
            // 
            this.toolsEditorTools.Dock = System.Windows.Forms.DockStyle.None;
            this.toolsEditorTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolsEditorTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSetCell,
            this.btnClearCell,
            this.btnSelectCell});
            this.toolsEditorTools.Location = new System.Drawing.Point(0, 3);
            this.toolsEditorTools.Name = "toolsEditorTools";
            this.toolsEditorTools.Size = new System.Drawing.Size(24, 71);
            this.toolsEditorTools.TabIndex = 0;
            // 
            // btnSetCell
            // 
            this.btnSetCell.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSetCell.Image = global::DataRogueWorldEditor.Properties.Resources.pencil;
            this.btnSetCell.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSetCell.Name = "btnSetCell";
            this.btnSetCell.Size = new System.Drawing.Size(22, 20);
            this.btnSetCell.Text = "toolStripButton1";
            this.btnSetCell.Click += new System.EventHandler(this.btnSetCell_Click);
            // 
            // btnClearCell
            // 
            this.btnClearCell.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClearCell.Image = global::DataRogueWorldEditor.Properties.Resources.eraser;
            this.btnClearCell.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClearCell.Name = "btnClearCell";
            this.btnClearCell.Size = new System.Drawing.Size(22, 20);
            this.btnClearCell.Text = "toolStripButton2";
            this.btnClearCell.Click += new System.EventHandler(this.btnClearCell_Click);
            // 
            // btnSelectCell
            // 
            this.btnSelectCell.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSelectCell.Image = global::DataRogueWorldEditor.Properties.Resources.cursor;
            this.btnSelectCell.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectCell.Name = "btnSelectCell";
            this.btnSelectCell.Size = new System.Drawing.Size(22, 20);
            this.btnSelectCell.Text = "toolStripButton1";
            this.btnSelectCell.Click += new System.EventHandler(this.btnSelectCell_Click);
            // 
            // toolsScroll
            // 
            this.toolsScroll.Dock = System.Windows.Forms.DockStyle.None;
            this.toolsScroll.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolsScroll.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnScrollLeft,
            this.btnScrollUp,
            this.btnScrollDown,
            this.btnScrollRight});
            this.toolsScroll.Location = new System.Drawing.Point(0, 125);
            this.toolsScroll.Name = "toolsScroll";
            this.toolsScroll.Size = new System.Drawing.Size(24, 94);
            this.toolsScroll.TabIndex = 2;
            // 
            // btnScrollLeft
            // 
            this.btnScrollLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnScrollLeft.Image = global::DataRogueWorldEditor.Properties.Resources.leftarrow;
            this.btnScrollLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScrollLeft.Name = "btnScrollLeft";
            this.btnScrollLeft.Size = new System.Drawing.Size(22, 20);
            this.btnScrollLeft.Text = "toolStripButton1";
            this.btnScrollLeft.Click += new System.EventHandler(this.btnScrollLeft_Click);
            // 
            // btnScrollUp
            // 
            this.btnScrollUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnScrollUp.Image = global::DataRogueWorldEditor.Properties.Resources.uparrow;
            this.btnScrollUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScrollUp.Name = "btnScrollUp";
            this.btnScrollUp.Size = new System.Drawing.Size(22, 20);
            this.btnScrollUp.Text = "toolStripButton2";
            this.btnScrollUp.Click += new System.EventHandler(this.btnScrollUp_Click);
            // 
            // btnScrollDown
            // 
            this.btnScrollDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnScrollDown.Image = global::DataRogueWorldEditor.Properties.Resources.downarrow;
            this.btnScrollDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScrollDown.Name = "btnScrollDown";
            this.btnScrollDown.Size = new System.Drawing.Size(22, 20);
            this.btnScrollDown.Text = "toolStripButton3";
            this.btnScrollDown.Click += new System.EventHandler(this.btnScrollDown_Click);
            // 
            // btnScrollRight
            // 
            this.btnScrollRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnScrollRight.Image = global::DataRogueWorldEditor.Properties.Resources.rightarrow;
            this.btnScrollRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScrollRight.Name = "btnScrollRight";
            this.btnScrollRight.Size = new System.Drawing.Size(22, 20);
            this.btnScrollRight.Text = "toolStripButton4";
            this.btnScrollRight.Click += new System.EventHandler(this.btnScrollRight_Click);
            // 
            // dgvGlyphs
            // 
            this.dgvGlyphs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGlyphs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Glyph,
            this.Entity});
            this.dgvGlyphs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGlyphs.Location = new System.Drawing.Point(0, 0);
            this.dgvGlyphs.Name = "dgvGlyphs";
            this.dgvGlyphs.Size = new System.Drawing.Size(220, 292);
            this.dgvGlyphs.TabIndex = 0;
            this.dgvGlyphs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGlyphs_DoubleClickCell);
            this.dgvGlyphs.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGlyphs_CellValueChanged);
            // 
            // Glyph
            // 
            this.Glyph.DataPropertyName = "Glyph";
            this.Glyph.HeaderText = "Glyph";
            this.Glyph.Name = "Glyph";
            this.Glyph.Width = 50;
            // 
            // Entity
            // 
            this.Entity.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Entity.DataPropertyName = "Entity";
            this.Entity.HeaderText = "Entity";
            this.Entity.Name = "Entity";
            // 
            // pnlMap
            // 
            this.pnlMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMap.Controls.Add(this.lblMapEntities);
            this.pnlMap.Controls.Add(this.lblMap);
            this.pnlMap.Location = new System.Drawing.Point(0, 0);
            this.pnlMap.Name = "pnlMap";
            this.pnlMap.Size = new System.Drawing.Size(751, 681);
            this.pnlMap.TabIndex = 1;
            // 
            // lblMapEntities
            // 
            this.lblMapEntities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMapEntities.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMapEntities.ForeColor = System.Drawing.Color.Gold;
            this.lblMapEntities.Location = new System.Drawing.Point(4, 2);
            this.lblMapEntities.Name = "lblMapEntities";
            this.lblMapEntities.Padding = new System.Windows.Forms.Padding(5);
            this.lblMapEntities.Size = new System.Drawing.Size(751, 681);
            this.lblMapEntities.TabIndex = 0;
            this.lblMapEntities.TabStop = false;
            this.lblMapEntities.Text = "█......\r\n█.....\r\n....█..\r\n.......";
            this.lblMapEntities.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lblMapEntities.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblMapEntities_MouseClick);
            this.lblMapEntities.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblMapEntities_MouseMove);
            // 
            // lblMap
            // 
            this.lblMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMap.BackColor = System.Drawing.Color.Black;
            this.lblMap.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMap.ForeColor = System.Drawing.Color.White;
            this.lblMap.Location = new System.Drawing.Point(0, 0);
            this.lblMap.Name = "lblMap";
            this.lblMap.Padding = new System.Windows.Forms.Padding(5);
            this.lblMap.Size = new System.Drawing.Size(751, 681);
            this.lblMap.TabIndex = 0;
            this.lblMap.Text = "█......\r\n█.....\r\n....█..\r\n.......";
            this.lblMap.Paint += new System.Windows.Forms.PaintEventHandler(this.lblMap_Paint);
            this.lblMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lblMap_MouseClick);
            this.lblMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblMap_MouseMove);
            // 
            // splitContainer4
            // 
            this.splitContainer4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.txtMapKey);
            this.splitContainer4.Panel1.Controls.Add(this.txtDefaultCell);
            this.splitContainer4.Panel1.Controls.Add(this.label2);
            this.splitContainer4.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.dgvCommands);
            this.splitContainer4.Panel2.Controls.Add(this.lblSelectedCell);
            this.splitContainer4.Size = new System.Drawing.Size(259, 685);
            this.splitContainer4.SplitterDistance = 85;
            this.splitContainer4.TabIndex = 0;
            // 
            // txtMapKey
            // 
            this.txtMapKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMapKey.Location = new System.Drawing.Point(84, 9);
            this.txtMapKey.Name = "txtMapKey";
            this.txtMapKey.Size = new System.Drawing.Size(161, 20);
            this.txtMapKey.TabIndex = 10;
            this.txtMapKey.TextChanged += new System.EventHandler(this.txtMapKey_TextChanged);
            // 
            // txtDefaultCell
            // 
            this.txtDefaultCell.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDefaultCell.Location = new System.Drawing.Point(84, 44);
            this.txtDefaultCell.Name = "txtDefaultCell";
            this.txtDefaultCell.Size = new System.Drawing.Size(161, 20);
            this.txtDefaultCell.TabIndex = 9;
            this.txtDefaultCell.TextChanged += new System.EventHandler(this.txtDefaultCell_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 47);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "DefaultCell";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "MapKey";
            // 
            // dgvCommands
            // 
            this.dgvCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCommands.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCommands.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCommandTypes,
            this.dataGridViewTextBoxColumn2});
            this.dgvCommands.Location = new System.Drawing.Point(-2, 148);
            this.dgvCommands.Name = "dgvCommands";
            this.dgvCommands.Size = new System.Drawing.Size(259, 446);
            this.dgvCommands.TabIndex = 1;
            // 
            // colCommandTypes
            // 
            this.colCommandTypes.DataPropertyName = "CommandType";
            this.colCommandTypes.HeaderText = "Command";
            this.colCommandTypes.Name = "colCommandTypes";
            this.colCommandTypes.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCommandTypes.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colCommandTypes.Width = 75;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Parameters";
            this.dataGridViewTextBoxColumn2.HeaderText = "Parameters";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // lblSelectedCell
            // 
            this.lblSelectedCell.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelectedCell.BackColor = System.Drawing.Color.White;
            this.lblSelectedCell.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedCell.Location = new System.Drawing.Point(0, 0);
            this.lblSelectedCell.Name = "lblSelectedCell";
            this.lblSelectedCell.Padding = new System.Windows.Forms.Padding(5);
            this.lblSelectedCell.Size = new System.Drawing.Size(257, 145);
            this.lblSelectedCell.TabIndex = 0;
            this.lblSelectedCell.Text = "X: 0, Y: 0\r\nCell:Wall\r\nPassable: True\r\nTransparent: True\r\nGlyph: .\r\nColor: #aaaaa" +
    "a\r\nZOrder: 0";
            // 
            // toolsVisible
            // 
            this.toolsVisible.Dock = System.Windows.Forms.DockStyle.None;
            this.toolsVisible.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolsVisible.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnSaveAs,
            this.toolStripButton1,
            this.btnMap,
            this.btnEntities});
            this.toolsVisible.Location = new System.Drawing.Point(6, 0);
            this.toolsVisible.Name = "toolsVisible";
            this.toolsVisible.Size = new System.Drawing.Size(101, 25);
            this.toolsVisible.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::DataRogueWorldEditor.Properties.Resources.save;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "toolStripButton2";
            this.btnSave.ToolTipText = "Save";
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveAs.Image = global::DataRogueWorldEditor.Properties.Resources.saveas;
            this.btnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(23, 22);
            this.btnSaveAs.Text = "toolStripButton3";
            this.btnSaveAs.ToolTipText = "Save As";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnMap
            // 
            this.btnMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMap.Image = global::DataRogueWorldEditor.Properties.Resources.scroll;
            this.btnMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMap.Name = "btnMap";
            this.btnMap.Size = new System.Drawing.Size(23, 22);
            this.btnMap.Text = "toolStripButton1";
            this.btnMap.ToolTipText = "Toggle Map";
            this.btnMap.Click += new System.EventHandler(this.btnMap_Click);
            // 
            // btnEntities
            // 
            this.btnEntities.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEntities.Image = global::DataRogueWorldEditor.Properties.Resources.entity;
            this.btnEntities.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEntities.Name = "btnEntities";
            this.btnEntities.Size = new System.Drawing.Size(23, 22);
            this.btnEntities.Text = "toolStripButton2";
            this.btnEntities.ToolTipText = "Toggle Entities";
            this.btnEntities.Click += new System.EventHandler(this.btnEnemies_Click);
            // 
            // frmMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1246, 710);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "frmMapEditor";
            this.Text = "frmMapEditor";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.toolStripContainer2.ContentPanel.ResumeLayout(false);
            this.toolStripContainer2.ContentPanel.PerformLayout();
            this.toolStripContainer2.LeftToolStripPanel.ResumeLayout(false);
            this.toolStripContainer2.LeftToolStripPanel.PerformLayout();
            this.toolStripContainer2.ResumeLayout(false);
            this.toolStripContainer2.PerformLayout();
            this.toolsEditorTools.ResumeLayout(false);
            this.toolsEditorTools.PerformLayout();
            this.toolsScroll.ResumeLayout(false);
            this.toolsScroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGlyphs)).EndInit();
            this.pnlMap.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCommands)).EndInit();
            this.toolsVisible.ResumeLayout(false);
            this.toolsVisible.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolsVisible;
        private System.Windows.Forms.ToolStripButton btnMap;
        private System.Windows.Forms.ToolStripButton btnEntities;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Panel pnlMap;
        private System.Windows.Forms.DataGridView dgvGlyphs;
        private System.Windows.Forms.DataGridViewTextBoxColumn Glyph;
        private System.Windows.Forms.DataGridViewTextBoxColumn Entity;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.TextBox txtMapKey;
        private System.Windows.Forms.TextBox txtDefaultCell;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMap;
        private System.Windows.Forms.Label lblSelectedCell;
        private System.Windows.Forms.ToolStripContainer toolStripContainer2;
        private System.Windows.Forms.Label lblCurrentTool;
        private System.Windows.Forms.ToolStrip toolsEditorTools;
        private System.Windows.Forms.ToolStripButton btnSetCell;
        private System.Windows.Forms.ToolStripButton btnClearCell;
        private System.Windows.Forms.ToolStripButton btnSelectCell;
        private System.Windows.Forms.ToolStrip toolsScroll;
        private System.Windows.Forms.ToolStripButton btnScrollLeft;
        private System.Windows.Forms.ToolStripButton btnScrollUp;
        private System.Windows.Forms.ToolStripButton btnScrollDown;
        private System.Windows.Forms.ToolStripButton btnScrollRight;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripButton1;
        private System.Windows.Forms.ToolStripButton btnSaveAs;
        private System.Windows.Forms.DataGridView dgvCommands;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCommandTypes;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private TransparentLabel lblMapEntities;
    }
}