﻿using System.Collections.Generic;
using System.Threading;
using Mapsui.Layers;
using Mapsui.Layers.Tiling;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.Utilities;

namespace Mapsui.Samples.Common.Maps
{
    public class AnimationsSample : ISample
    {
        public string Name => "Animated symbols";
        public string Category => "Special";

        public void Setup(IMapControl mapControl)
        {
            mapControl.Map = CreateMap(mapControl);
        }

        private static Timer? timer;

        public static Map CreateMap(IMapControl mapControl)
        {
            var map = new Map();
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(CreateLayer());

            // IMPORTANT:
            // This is a vary temporary timer that is needed to show how passive animation works.
            // In the eventual solution there render loop would be triggered a regular intervals.
            // On each interval it should check if a render update is needed. So, the state of the
            // animations should somehow be requestable at that point.
            timer = new Timer((s) => {
                var layer = (Layer)map.Layers[1];
                Animation.UpdateAnimations(layer.SymbolStyle, layer.Animations);
                mapControl.Refresh();
            }, null,16, 16);
            
            return map;
        }

        public static ILayer CreateLayer()
        {
            var style = new SymbolStyle() { Fill = new Brush(Color.Red), SymbolScale = 1, };

            var layer = new Layer("Points")
            {
                DataSource = new MemoryProvider<IFeature>(CreatePoints(style)),
            };
            layer.SymbolStyle = style;

            layer.Animations.AddRange(CreateAnimationsForSymbolStyle(style));

            Animation.Start(layer.Animations, 10000); // This should not be necessary

            return layer;

        }

        private static List<PointFeature> CreatePoints(IStyle style)
        {
            var result = new List<PointFeature>
            {
                CreatePoint(1000000, 1000000, style),
                CreatePoint(9000000, 1000000, style),
                CreatePoint(9000000, 9000000, style),
                CreatePoint(1000000, 9000000, style)
            };

            return result;
        }

        private static List<AnimationEntry<SymbolStyle>> CreateAnimationsForSymbolStyle(SymbolStyle style)
        {
            var animations = new List<AnimationEntry<SymbolStyle>>();

            var entry1 = new AnimationEntry<SymbolStyle>(
                start: style.SymbolScale,
                end: style.SymbolScale * 2,
                animationStart: 0,
                animationEnd: .5,
                easing: Easing.SinInOut,
                repeat: true,
                tick: (symbolStyle, e, v) => { style.SymbolScale = (double)((double)e.Start + ((double)e.End - (double)e.Start) * e.Easing.Ease(v)); },
                final: (symbolStyle, e) => { style.SymbolScale = (double)e.End; }
            );
            animations.Add(entry1);

            var entry2 = new AnimationEntry<SymbolStyle>(
                start: style.SymbolScale * 2,
                end: style.SymbolScale,
                animationStart: .5,
                animationEnd: 1,
                easing: Easing.SinInOut,
                repeat: true,
                tick: (symbolStyle, e, v) => { style.SymbolScale = (double)((double)e.Start + ((double)e.End - (double)e.Start) * e.Easing.Ease(v)); },
                final: (symbolStyle, e) => { style.SymbolScale = (double)e.End; }
            );
            animations.Add(entry2);

            var entry3 = new AnimationEntry<SymbolStyle>(
                start: style.Outline?.Color ?? Color.Gray,
                end: style.Outline?.Color ?? Color.Gray,
                animationStart: 0,
                animationEnd: 1,
                easing: Easing.Linear,
                tick: (symbolStyle, e, v) => {
                    var color = (Color)e.Start;
                    style.Fill ??= new Brush();
                    style.Fill.Color = new Color(color.R, color.G, (int)(v < 0.5 ? (1.0 - 2.0 * v) * 255 : ((v - 0.5) * 2.0) * 255));
                },
                final: (symbolStyle, e) => {
                    style.Fill ??= new Brush();
                    style.Fill.Color = e.End as Color;
                }
            );
            animations.Add(entry3);

            return animations;
        }

        private static PointFeature CreatePoint(double x, double y, IStyle style)
        {
            var result = new PointFeature(new MPoint(x, y));
            result.Styles.Add(style);
            return result;
        }
    }
}