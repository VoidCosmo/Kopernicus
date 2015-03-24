// Material wrapper generated by shader translator tool
using System;
using System.Reflection;
using UnityEngine;

using Kopernicus.MaterialWrapper;

namespace Kopernicus
{
    namespace Configuration
    {
        public class PQSProjectionSurfaceQuadLoader : PQSProjectionSurfaceQuad
        {
            // Saturation, default = 1
            [ParserTarget("saturation", optional = true)]
            private NumericParser<float> saturationSetter
            {
                set { base.saturation = value.value; }
            }

            // Contrast, default = 1
            [ParserTarget("contrast", optional = true)]
            private NumericParser<float> contrastSetter
            {
                set { base.contrast = value.value; }
            }

            // Colour Unsaturation (A = Factor), default = (1,1,1,0)
            [ParserTarget("tintColor", optional = true)]
            private ColorParser tintColorSetter
            {
                set { base.tintColor = value.value; }
            }

            // Near Tiling, default = 1000
            [ParserTarget("texTiling", optional = true)]
            private NumericParser<float> texTilingSetter
            {
                set { base.texTiling = value.value; }
            }

            // Near Blend, default = 0.5
            [ParserTarget("texPower", optional = true)]
            private NumericParser<float> texPowerSetter
            {
                set { base.texPower = value.value; }
            }

            // Far Blend, default = 0.5
            [ParserTarget("multiPower", optional = true)]
            private NumericParser<float> multiPowerSetter
            {
                set { base.multiPower = value.value; }
            }

            // NearFar Start, default = 2000
            [ParserTarget("groundTexStart", optional = true)]
            private NumericParser<float> groundTexStartSetter
            {
                set { base.groundTexStart = value.value; }
            }

            // NearFar Start, default = 10000
            [ParserTarget("groundTexEnd", optional = true)]
            private NumericParser<float> groundTexEndSetter
            {
                set { base.groundTexEnd = value.value; }
            }

            // Steep Tiling, default = 1
            [ParserTarget("steepTiling", optional = true)]
            private NumericParser<float> steepTilingSetter
            {
                set { base.steepTiling = value.value; }
            }

            // Steep Blend, default = 1
            [ParserTarget("steepPower", optional = true)]
            private NumericParser<float> steepPowerSetter
            {
                set { base.steepPower = value.value; }
            }

            // Steep Fade Start, default = 20000
            [ParserTarget("steepTexStart", optional = true)]
            private NumericParser<float> steepTexStartSetter
            {
                set { base.steepTexStart = value.value; }
            }

            // Steep Fade End, default = 30000
            [ParserTarget("steepTexEnd", optional = true)]
            private NumericParser<float> steepTexEndSetter
            {
                set { base.steepTexEnd = value.value; }
            }

            // Deep ground, default = "white" {}
            [ParserTarget("deepTex", optional = true)]
            private Texture2DParser deepTexSetter
            {
                set { base.deepTex = value.value; }
            }

            [ParserTarget("deepTexScale", optional = true)]
            private Vector2Parser deepTexScaleSetter
            {
                set { base.deepTexScale = value.value; }
            }

            [ParserTarget("deepTexOffset", optional = true)]
            private Vector2Parser deepTexOffsetSetter
            {
                set { base.deepTexOffset = value.value; }
            }

            // Deep MT, default = "white" {}
            [ParserTarget("deepMultiTex", optional = true)]
            private Texture2DParser deepMultiTexSetter
            {
                set { base.deepMultiTex = value.value; }
            }

            [ParserTarget("deepMultiTexScale", optional = true)]
            private Vector2Parser deepMultiTexScaleSetter
            {
                set { base.deepMultiTexScale = value.value; }
            }

            [ParserTarget("deepMultiTexOffset", optional = true)]
            private Vector2Parser deepMultiTexOffsetSetter
            {
                set { base.deepMultiTexOffset = value.value; }
            }

            // Deep MT Tiling, default = 1
            [ParserTarget("deepMultiFactor", optional = true)]
            private NumericParser<float> deepMultiFactorSetter
            {
                set { base.deepMultiFactor = value.value; }
            }

            // Main Texture, default = "white" {}
            [ParserTarget("mainTex", optional = true)]
            private Texture2DParser mainTexSetter
            {
                set { base.mainTex = value.value; }
            }

            [ParserTarget("mainTexScale", optional = true)]
            private Vector2Parser mainTexScaleSetter
            {
                set { base.mainTexScale = value.value; }
            }

            [ParserTarget("mainTexOffset", optional = true)]
            private Vector2Parser mainTexOffsetSetter
            {
                set { base.mainTexOffset = value.value; }
            }

            // Main MT, default = "white" {}
            [ParserTarget("mainMultiTex", optional = true)]
            private Texture2DParser mainMultiTexSetter
            {
                set { base.mainMultiTex = value.value; }
            }

            [ParserTarget("mainMultiTexScale", optional = true)]
            private Vector2Parser mainMultiTexScaleSetter
            {
                set { base.mainMultiTexScale = value.value; }
            }

            [ParserTarget("mainMultiTexOffset", optional = true)]
            private Vector2Parser mainMultiTexOffsetSetter
            {
                set { base.mainMultiTexOffset = value.value; }
            }

            // Main MT Tiling, default = 1
            [ParserTarget("mainMultiFactor", optional = true)]
            private NumericParser<float> mainMultiFactorSetter
            {
                set { base.mainMultiFactor = value.value; }
            }

            // High Ground, default = "white" {}
            [ParserTarget("highTex", optional = true)]
            private Texture2DParser highTexSetter
            {
                set { base.highTex = value.value; }
            }

            [ParserTarget("highTexScale", optional = true)]
            private Vector2Parser highTexScaleSetter
            {
                set { base.highTexScale = value.value; }
            }

            [ParserTarget("highTexOffset", optional = true)]
            private Vector2Parser highTexOffsetSetter
            {
                set { base.highTexOffset = value.value; }
            }

            // High MT, default = "white" {}
            [ParserTarget("highMultiTex", optional = true)]
            private Texture2DParser highMultiTexSetter
            {
                set { base.highMultiTex = value.value; }
            }

            [ParserTarget("highMultiTexScale", optional = true)]
            private Vector2Parser highMultiTexScaleSetter
            {
                set { base.highMultiTexScale = value.value; }
            }

            [ParserTarget("highMultiTexOffset", optional = true)]
            private Vector2Parser highMultiTexOffsetSetter
            {
                set { base.highMultiTexOffset = value.value; }
            }

            // High MT Tiling, default = 1
            [ParserTarget("highMultiFactor", optional = true)]
            private NumericParser<float> highMultiFactorSetter
            {
                set { base.highMultiFactor = value.value; }
            }

            // Snow, default = "white" {}
            [ParserTarget("snowTex", optional = true)]
            private Texture2DParser snowTexSetter
            {
                set { base.snowTex = value.value; }
            }

            [ParserTarget("snowTexScale", optional = true)]
            private Vector2Parser snowTexScaleSetter
            {
                set { base.snowTexScale = value.value; }
            }

            [ParserTarget("snowTexOffset", optional = true)]
            private Vector2Parser snowTexOffsetSetter
            {
                set { base.snowTexOffset = value.value; }
            }

            // Snow MT, default = "white" {}
            [ParserTarget("snowMultiTex", optional = true)]
            private Texture2DParser snowMultiTexSetter
            {
                set { base.snowMultiTex = value.value; }
            }

            [ParserTarget("snowMultiTexScale", optional = true)]
            private Vector2Parser snowMultiTexScaleSetter
            {
                set { base.snowMultiTexScale = value.value; }
            }

            [ParserTarget("snowMultiTexOffset", optional = true)]
            private Vector2Parser snowMultiTexOffsetSetter
            {
                set { base.snowMultiTexOffset = value.value; }
            }

            // Snow MT Tiling, default = 1
            [ParserTarget("snowMultiFactor", optional = true)]
            private NumericParser<float> snowMultiFactorSetter
            {
                set { base.snowMultiFactor = value.value; }
            }

            // Steep Texture, default = "white" {}
            [ParserTarget("steepTex", optional = true)]
            private Texture2DParser steepTexSetter
            {
                set { base.steepTex = value.value; }
            }

            [ParserTarget("steepTexScale", optional = true)]
            private Vector2Parser steepTexScaleSetter
            {
                set { base.steepTexScale = value.value; }
            }

            [ParserTarget("steepTexOffset", optional = true)]
            private Vector2Parser steepTexOffsetSetter
            {
                set { base.steepTexOffset = value.value; }
            }

            // Deep Start, default = 0
            [ParserTarget("deepStart", optional = true)]
            private NumericParser<float> deepStartSetter
            {
                set { base.deepStart = value.value; }
            }

            // Deep End, default = 0.3
            [ParserTarget("deepEnd", optional = true)]
            private NumericParser<float> deepEndSetter
            {
                set { base.deepEnd = value.value; }
            }

            // Main lower boundary start, default = 0
            [ParserTarget("mainLoStart", optional = true)]
            private NumericParser<float> mainLoStartSetter
            {
                set { base.mainLoStart = value.value; }
            }

            // Main lower boundary end, default = 0.5
            [ParserTarget("mainLoEnd", optional = true)]
            private NumericParser<float> mainLoEndSetter
            {
                set { base.mainLoEnd = value.value; }
            }

            // Main upper boundary start, default = 0.3
            [ParserTarget("mainHiStart", optional = true)]
            private NumericParser<float> mainHiStartSetter
            {
                set { base.mainHiStart = value.value; }
            }

            // Main upper boundary end, default = 0.5
            [ParserTarget("mainHiEnd", optional = true)]
            private NumericParser<float> mainHiEndSetter
            {
                set { base.mainHiEnd = value.value; }
            }

            // High lower boundary start, default = 0.6
            [ParserTarget("hiLoStart", optional = true)]
            private NumericParser<float> hiLoStartSetter
            {
                set { base.hiLoStart = value.value; }
            }

            // High lower boundary end, default = 0.6
            [ParserTarget("hiLoEnd", optional = true)]
            private NumericParser<float> hiLoEndSetter
            {
                set { base.hiLoEnd = value.value; }
            }

            // High upper boundary start, default = 0.6
            [ParserTarget("hiHiStart", optional = true)]
            private NumericParser<float> hiHiStartSetter
            {
                set { base.hiHiStart = value.value; }
            }

            // High upper boundary end, default = 0.9
            [ParserTarget("hiHiEnd", optional = true)]
            private NumericParser<float> hiHiEndSetter
            {
                set { base.hiHiEnd = value.value; }
            }

            // Snow Start, default = 0.9
            [ParserTarget("snowStart", optional = true)]
            private NumericParser<float> snowStartSetter
            {
                set { base.snowStart = value.value; }
            }

            // Snow End, default = 1
            [ParserTarget("snowEnd", optional = true)]
            private NumericParser<float> snowEndSetter
            {
                set { base.snowEnd = value.value; }
            }

            // PlanetOpacity, default = 1
            [ParserTarget("planetOpacity", optional = true)]
            private NumericParser<float> planetOpacitySetter
            {
                set { base.planetOpacity = value.value; }
            }

            // Constructors
            public PQSProjectionSurfaceQuadLoader () : base() { }
            public PQSProjectionSurfaceQuadLoader (string contents) : base (contents) { }
            public PQSProjectionSurfaceQuadLoader (Material material) : base(material) { }
        }
    }
}
