namespace DataRogueWorldEditor.Controls
{
    partial class MapEditorGlyphEntity
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
            this.txtGlyph = new System.Windows.Forms.TextBox();
            this.txtEntity = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtGlyph
            // 
            this.txtGlyph.Location = new System.Drawing.Point(4, 4);
            this.txtGlyph.MaxLength = 1;
            this.txtGlyph.Name = "txtGlyph";
            this.txtGlyph.Size = new System.Drawing.Size(29, 20);
            this.txtGlyph.TabIndex = 0;
            // 
            // txtEntity
            // 
            this.txtEntity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEntity.Location = new System.Drawing.Point(39, 4);
            this.txtEntity.Name = "txtEntity";
            this.txtEntity.Size = new System.Drawing.Size(152, 20);
            this.txtEntity.TabIndex = 1;
            // 
            // MapEditorGlyphEntity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtEntity);
            this.Controls.Add(this.txtGlyph);
            this.Name = "MapEditorGlyphEntity";
            this.Size = new System.Drawing.Size(194, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtGlyph;
        private System.Windows.Forms.TextBox txtEntity;
    }
}
