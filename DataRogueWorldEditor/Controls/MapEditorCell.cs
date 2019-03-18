using System;
using System.Drawing;
using System.Windows.Forms;

namespace DataRogueWorldEditor.Controls
{
    public partial class MapEditorCell : UserControl
    {
        public char Glyph { get; set; }
        public Color GlyphColor { get; set; }

        public MapEditorCell()
        {
            InitializeComponent();

            BindControls();
        }

        private void BindControls()
        {
            lblGlyph.DataBindings.Clear();
            lblGlyph.DataBindings.Add(nameof(lblGlyph.Text), this, nameof(Glyph), false, DataSourceUpdateMode.OnPropertyChanged);
            lblGlyph.DataBindings.Add(nameof(lblGlyph.ForeColor), this, nameof(GlyphColor), false, DataSourceUpdateMode.OnPropertyChanged);

        }

        private void lblGlyph_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }
    }
}
