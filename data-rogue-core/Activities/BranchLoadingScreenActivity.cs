using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Systems.Interfaces;
using System;

namespace data_rogue_core.Activities
{
    public class BranchLoadingScreenActivity : StaticTextActivity
    {
        private IEntityEngine _engine;

        public BranchLoadingScreenActivity(ISystemContainer systemContainer) : base(systemContainer.ActivitySystem, "Generating branch...", false, null)
        {
            _engine = systemContainer.EntityEngine;

            _displayEntity = systemContainer.EntityEngine.New("branchGenerationTracker",
                    new Appearance { Color = System.Drawing.Color.White, Glyph = '@' },
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

        public override ActivityType Type => base.Type;

        public override bool RendersEntireSpace => base.RendersEntireSpace;

        public override bool AcceptsInput => base.AcceptsInput;
    }
}
