using System;
using System.Collections.Generic;
using System.Linq;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;
using data_rogue_core.Menus.StaticMenus;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Forms.StaticForms
{

    public class MapInfoForm : Form
    {
        private ISystemContainer _systemContainer;

        public MapInfoForm(ISystemContainer systemContainer, IMap map, FormButtonSelected OnSelectCallback) : base(systemContainer.ActivitySystem, "Map Info", FormButton.Ok | FormButton.Cancel,
                null)
        {
            _systemContainer = systemContainer;

            Fields = MapInfoFields(map);

            base.OnSelectCallback = OnSelectCallback;
        }

        public Dictionary<string, FormData> MapInfoFields(IMap map)
        {
            var classNames = _systemContainer.EntityEngine.EntitiesWith<Class>(true).Select(e => (object)e.DescriptionName).ToList();

            var creationFields = new Dictionary<string, FormData>
            {
                {"Map Name", new TextFormData("Name", FormDataType.Text, map.MapKey.Key, 1)},
                {"Biomes", new TextFormData("Biomes", FormDataType.Text, string.Join(",", map.Biomes.Select(b=>b.Name)), 2)},
                {"VaultWeight", new TextFormData("VaultWeight", FormDataType.Text, map.VaultWeight.ToString(), 3)},
            };

            return creationFields;
          
        }
    }
}