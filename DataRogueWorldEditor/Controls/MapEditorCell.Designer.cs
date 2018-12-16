namespace DataRogueWorldEditor.Controls
{
    partial class MapEditorCell
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblGlyph = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblGlyph
            // 
            this.lblGlyph.BackColor = System.Drawing.Color.Black;
            this.lblGlyph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGlyph.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGlyph.ForeColor = System.Drawing.Color.White;
            this.lblGlyph.Location = new System.Drawing.Point(0, 0);
            this.lblGlyph.Name = "lblGlyph";
            this.lblGlyph.Size = new System.Drawing.Size(16, 16);
            this.lblGlyph.TabIndex = 0;
            this.lblGlyph.Text = "?";
            this.lblGlyph.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblGlyph.Click += new System.EventHandler(this.lblGlyph_Click);
            // 
            // MapEditorCell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblGlyph);
            this.Name = "MapEditorCell";
            this.Size = new System.Drawing.Size(16, 16);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblGlyph;
    }
}
