﻿using BearLib;
using BLTWrapper;
using data_rogue_core.Activities;
using data_rogue_core.Components;
using data_rogue_core.Controls;
using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Renderers;
using data_rogue_core.Renderers.ConsoleRenderers;
using data_rogue_core.Systems;
using OpenTK.Input;
using RLNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace data_rogue_core.IOSystems.BLTTiles
{

    public class BLTTilesIOSystem : IIOSystem
    {
        public static IOSystemConfiguration DefaultConfiguration = new IOSystemConfiguration
        {
            InitialHeight = 25,
            InitialWidth = 40,
            TileHeight = 32,
            TileWidth = 32,
            WindowTitle = "data-rogue window title",
            MapConfigurations = new List<MapConfiguration> { new MapConfiguration { Position = new Rectangle(0, 0, 40 * TILE_SPACING, 25 * TILE_SPACING) } },
            StatsConfigurations = new List<StatsConfiguration> { new StatsConfiguration { Position = new Rectangle(2, 2, 40 * TILE_SPACING - 2, 25 * TILE_SPACING - 2), Displays = new List<InfoDisplay> {
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "Health,HP", BackColor = Color.Red},
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "AuraFighter,Aura", BackColor = Color.Gold},
                new InfoDisplay { ControlType = typeof(ComponentCounter), Parameters = "TiltFighter,Tilt", BackColor = Color.Purple},
                new InfoDisplay { ControlType =  typeof(Spacer) },
                new InfoDisplay { ControlType =  typeof(HoveredEntityDisplayBox), Parameters = "Health,HP;AuraFighter,Aura;TiltFighter,Tilt" }

            } } },
            MessageConfigurations = new List<MessageConfiguration> { new MessageConfiguration {
                Position = new Rectangle(2, 15 * TILE_SPACING, 40 * TILE_SPACING, 10 * TILE_SPACING - 2),
                NumberOfMessages = 15} }
        };

        private BLTSpriteLoader _spriteLoader;
        private bool isClosed;
        private GameLoopEventHandler _update;
        private GameLoopEventHandler _render;
        private KeyCombination _keyCombination;
        public IOSystemConfiguration Configuration { get; set; }
        private ISpriteManager _spriteManager;
        public const int TILE_SPACING = 8;

        private readonly List<int> MOUSE_EVENTS = new List<int> { BLT.TK_MOUSE_LEFT, BLT.TK_MOUSE_RIGHT, BLT.TK_MOUSE_LEFT | BLT.TK_KEY_RELEASED, BLT.TK_MOUSE_RIGHT | BLT.TK_KEY_RELEASED };
        private bool _leftMouseDown;
        private bool _rightMouseDown;
        private int _mouseY;
        private int _mouseX;
        private bool _mouseActive;
        private bool _leftClick;
        private bool _rightClick;

        public BLTTilesIOSystem(IOSystemConfiguration ioSystemConfiguration)
        { 
            _spriteLoader = new BLTSpriteLoader();

            Configuration = ioSystemConfiguration;
        }

        public IRendererFactory RendererFactory { get; private set; }

        public void Close()
        {
            isClosed = true;
            BLT.Close();
        }

        public void Run()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            do
            {
                var delta = stopwatch.ElapsedMilliseconds - elapsedMs;
                elapsedMs = stopwatch.ElapsedMilliseconds;
                var eventArgs = new GameLoopEventArgs(delta);
                CheckInput();
                _update(null, eventArgs);
                _render(null, eventArgs);
                
            } while (!isClosed);
        }

        private void CheckInput()
        {
            while (BLT.HasInput() && !isClosed)
            {
                var input = BLT.Read();

                if (input == BLT.TK_CLOSE)
                {
                    isClosed = true;
                }

                if (IsMouseMove(input))
                {
                    _mouseActive = true;
                    _mouseX = BLT.State(BLT.TK_MOUSE_X);
                    _mouseY = BLT.State(BLT.TK_MOUSE_Y);
                }
                if (IsClickEvent(input))
                {
                    _mouseActive = true;
                }
                else
                {
                    ResolveKeyboardInput(input);
                }
                
                SetMouseButtons(input);
            }
        }

        private bool IsMouseMove(int input)
        {
            return input == BLT.TK_MOUSE_MOVE;
        }

        private void SetMouseButtons(int input)
        {
            var leftMouseCurrentlyDown = BLT.State(BLT.TK_MOUSE_LEFT) == 1;
            var rightMouseCurrentlyDown = BLT.State(BLT.TK_MOUSE_RIGHT) == 1;

            _leftClick = _leftMouseDown && !leftMouseCurrentlyDown;
            _rightClick = _rightMouseDown && !rightMouseCurrentlyDown;

            _leftMouseDown = leftMouseCurrentlyDown;
            _rightMouseDown = rightMouseCurrentlyDown;
        }

        private bool IsClickEvent(int input)
        {
            return MOUSE_EVENTS.Contains(input);
        }

        private void ResolveKeyboardInput(int input)
        {
            var key = BLTKeyConverter.FromBLTInput(input);

            _keyCombination = new KeyCombination
            {
                Key = key,
                Ctrl = BLT.State(BLT.TK_CONTROL) != 0,
                Shift = BLT.State(BLT.TK_SHIFT) != 0,
                Alt = BLT.State(BLT.TK_ALT) != 0,
            };
        }

        public void Draw()
        {
            BLT.Refresh();
        }

        public KeyCombination GetKeyPress()
        {
            var ret = _keyCombination;
            _keyCombination = null;
            return ret;
        }

        public MouseData GetMouseData()
        {
            var mouse = new MouseData
            {
                IsLeftClick = _leftClick,
                IsRightClick = _rightClick,
                X = _mouseX,
                Y = _mouseY,
                MouseActive = _mouseActive
            };

            ResetMouseFlags();

            return mouse;
        }

        private void ResetMouseFlags()
        {
            _mouseActive = false;
            _leftClick = false;
            _rightClick = false;
        }

        public void Initialise(GameLoopEventHandler onUpdate, GameLoopEventHandler onRender, IEntityDataProvider graphicsDataProvider)
        {
            BLT.Open();

            _update = onUpdate;
            _render = onRender;

            var config = $"window: size={Configuration.InitialWidth * TILE_SPACING}x{Configuration.InitialHeight * TILE_SPACING}, cellsize=4x4, title='{Configuration.WindowTitle}'";
        
            BLT.Set(config);

            BLT.Set("input: precise-mouse=false, filter=[keyboard,mouse+];");

            BLT.Set("text font: Images/Tileset/Andux_sleipnir_8x12_tf.png, codepage=437, size=8x12, spacing=2x3;");
            BLT.Set("textLarge font: Images/Tileset/Andux_sleipnir_8x12_tf.png, codepage=437, size=8x12, resize=16x24, resize-filter=nearest, spacing=4x6;");
            BLT.Set("textXLarge font: Images/Tileset/Andux_sleipnir_8x12_tf.png, codepage=437, size=8x12, resize=32x48, resize-filter=nearest, spacing=8x12;");

            
            _spriteManager = SetUpSpriteManager(graphicsDataProvider);

            List<IDataRogueControlRenderer> controlRenderers = BLTControlRenderer.DefaultControlRenderers;

            controlRenderers.AddRange(Configuration.AdditionalControlRenderers);

            BLTTilesUnifiedRenderer unifiedRenderer = new BLTTilesUnifiedRenderer(controlRenderers, _spriteManager, Configuration);
            var renderers = new Dictionary<ActivityType, IRenderer>()
            {
                {ActivityType.Gameplay, unifiedRenderer},
                {ActivityType.Menu, unifiedRenderer},
                {ActivityType.StaticDisplay, unifiedRenderer},
                {ActivityType.Form, unifiedRenderer },
                {ActivityType.Targeting, unifiedRenderer },
                {ActivityType.Information, unifiedRenderer }
            };

            RendererFactory = new RendererFactory(renderers);

            BLT.Refresh();
        }

        private ISpriteManager SetUpSpriteManager(IEntityDataProvider graphicsDataProvider)
        {
            var spriteManager = new BLTSpriteManager();

            var graphicsData = graphicsDataProvider.GetData();

            foreach (var spriteCollection in graphicsData)
            {
                var entity = EntitySerializer.DeserializeOutsideEngine(spriteCollection);

                foreach (SpriteSheet sheet in entity.Components)
                {
                    switch(sheet.Type)
                    {
                        case "SingleSprite":
                            spriteManager.Add(_spriteLoader.LoadSingleSprite(sheet.Name, sheet.Path, sheet.SpriteWidth, sheet.SpriteHeight, sheet.Scaling, TILE_SPACING));
                            break;
                        case "TilesetBox":
                            spriteManager.Add(_spriteLoader.LoadTileset_BoxType(sheet.Name, sheet.Path, sheet.SpriteWidth, sheet.SpriteHeight, sheet.Scaling, TILE_SPACING));
                            break;
                        case "TilesetWall":
                            spriteManager.Add(_spriteLoader.LoadTileset_WallType(sheet.Name, sheet.Path, sheet.SpriteWidth, sheet.SpriteHeight, sheet.Scaling, TILE_SPACING));
                            break;
                        case "FourDirection":
                            spriteManager.Add(_spriteLoader.LoadFourDirectionSprite(sheet.Name, sheet.Path, sheet.SpriteWidth, sheet.SpriteHeight, sheet.Scaling, TILE_SPACING));
                            break;
                        default:
                            throw new Exception($"Unknown sprite sheet type '{sheet.Type}' in SetUpSpriteManager");
                    }
                }
            }

            return spriteManager;
        }
    }
}
