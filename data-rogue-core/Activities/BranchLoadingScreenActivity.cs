using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;
using Appearance = data_rogue_core.Components.Appearance;

namespace data_rogue_core.Activities
{
    public class BranchLoadingScreenActivity : StaticTextActivity
    {
        private IEntityEngine _engine;

        public BranchLoadingScreenActivity(ISystemContainer systemContainer) : base(systemContainer.ActivitySystem, "Generating branch...", false, null)
        {
            _engine = systemContainer.EntityEngine;

            _displayEntity = systemContainer.EntityEngine.New("branchGenerationTracker",
                    new Appearance { Color = Color.White, Glyph = '@' },
                    new SpriteAppearance { Bottom = "generic_person" },
                    new Animated(),
                    new Animation(),
                    new Description { Name = "Branch generation tracker", Detail = "Generating branch..." });

        }

        protected override void Close()
        {
            _engine.Destroy(_displayEntity);
            base.Close();
        }
    }
}
