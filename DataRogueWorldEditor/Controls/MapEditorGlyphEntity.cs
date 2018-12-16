using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataRogueWorldEditor.Controls
{
    public partial class MapEditorGlyphEntity : UserControl
    {
        public char Glyph { get; set; }
        public string Entity { get; set; }

        public MapEditorGlyphEntity()
        {
            InitializeComponent();

            BindTextBoxes();
        }

        private void BindTextBoxes()
        {
            txtGlyph.DataBindings.Clear();
            txtGlyph.DataBindings.Add(nameof(txtGlyph.Text), this, nameof(Glyph), false, DataSourceUpdateMode.OnPropertyChanged);

            txtEntity.DataBindings.Clear();
            txtEntity.DataBindings.Add(nameof(txtEntity.Text), this, nameof(Entity), false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
