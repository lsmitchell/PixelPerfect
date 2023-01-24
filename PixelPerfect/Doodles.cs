using System;
using System.Diagnostics;
using Dalamud.Configuration;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;
using System.Numerics;
using Num = System.Numerics;
using System.Collections.Generic;

namespace PixelPerfect
{
    public partial class PixelPerfect : IDalamudPlugin
    {
        private void DrawDoodles()
        {
            if (_cs.LocalPlayer == null) return;

            var actor = _cs.LocalPlayer;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Vector2(0, 0));
            ImGui.Begin("Canvas",
                ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
            ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
            
            if (_floorLines && _condition[ConditionFlag.BoundByDuty]) {

                float cardThickness = 5f;
                float intercardThickness = 2f;
                float cardMarkerThickness = 15f;
                float halfCardLength = 0.2f;
                float cardinalIndicatorDistance = 5.0f;

                float opacity = 0.8f;
                uint southColor = ImGui.GetColorU32(new Num.Vector4(0.2f, 0.6f, 0.8f, opacity));
                uint eastColor = ImGui.GetColorU32(new Num.Vector4(0.4f, 0.7f, 0.0f, opacity));
                uint northColor = ImGui.GetColorU32(new Num.Vector4(0.7f, 0.4f, 0.4f, opacity));
                uint westColor = ImGui.GetColorU32(new Num.Vector4(0.5f, 0.4f, 0.7f, opacity));
                float Ypos = 0f;

                _gui.WorldToScreen(new Num.Vector3(_arenaCenter, Ypos, _arenaCenter),
                    out Num.Vector2 floorStart);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter + _floorLineLength, Ypos, _arenaCenter),
                    out Num.Vector2 eastEnd);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter - _floorLineLength, Ypos, _arenaCenter),
                    out Num.Vector2 westEnd);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter, Ypos, _arenaCenter + _floorLineLength),
                    out Num.Vector2 southEnd);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter, Ypos, _arenaCenter - _floorLineLength),
                    out Num.Vector2 northEnd);

                _gui.WorldToScreen(new Num.Vector3(_arenaCenter + _floorLineLength, Ypos, _arenaCenter - _floorLineLength),
                    out Num.Vector2 neEnd);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter + _floorLineLength, Ypos, _arenaCenter + _floorLineLength),
                    out Num.Vector2 seEnd);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter - _floorLineLength, Ypos, _arenaCenter + _floorLineLength),
                    out Num.Vector2 swEnd);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter - _floorLineLength, Ypos, _arenaCenter - _floorLineLength),
                    out Num.Vector2 nwEnd);
                
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter - halfCardLength, Ypos, _arenaCenter - cardinalIndicatorDistance),
                    out Num.Vector2 nIndicatorStart);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter + halfCardLength, Ypos, _arenaCenter - cardinalIndicatorDistance),
                    out Num.Vector2 nIndicatorEnd);

                _gui.WorldToScreen(new Num.Vector3(_arenaCenter - halfCardLength, Ypos, _arenaCenter + cardinalIndicatorDistance),
                    out Num.Vector2 sIndicatorStart);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter + halfCardLength, Ypos, _arenaCenter + cardinalIndicatorDistance),
                    out Num.Vector2 sIndicatorEnd);
                
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter - cardinalIndicatorDistance, Ypos, _arenaCenter + halfCardLength),
                    out Num.Vector2 wIndicatorStart);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter - cardinalIndicatorDistance, Ypos, _arenaCenter - halfCardLength),
                    out Num.Vector2 wIndicatorEnd);
                
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter + cardinalIndicatorDistance, Ypos, _arenaCenter - halfCardLength),
                    out Num.Vector2 eIndicatorStart);
                _gui.WorldToScreen(new Num.Vector3(_arenaCenter + cardinalIndicatorDistance, Ypos, _arenaCenter + halfCardLength),
                    out Num.Vector2 eIndicatorEnd);

                ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();

                // cardinal lines
                windowDrawList.AddLine(new Num.Vector2(floorStart.X, floorStart.Y),
                    new Num.Vector2(eastEnd.X, eastEnd.Y),
                    eastColor, cardThickness);
                windowDrawList.AddLine(new Num.Vector2(floorStart.X, floorStart.Y),
                    new Num.Vector2(westEnd.X, westEnd.Y),
                    westColor, cardThickness);
                windowDrawList.AddLine(new Num.Vector2(floorStart.X, floorStart.Y),
                    new Num.Vector2(northEnd.X, northEnd.Y),
                    northColor, cardThickness);
                windowDrawList.AddLine(new Num.Vector2(floorStart.X, floorStart.Y),
                    new Num.Vector2(southEnd.X, southEnd.Y),
                    southColor, cardThickness);
                
                // cardinal indicators
                if (_floorBlips) {
                    windowDrawList.AddLine(new Num.Vector2(nIndicatorStart.X, nIndicatorStart.Y),
                        new Num.Vector2(nIndicatorEnd.X, nIndicatorEnd.Y),
                        northColor, cardMarkerThickness);
                    windowDrawList.AddLine(new Num.Vector2(sIndicatorStart.X, sIndicatorStart.Y),
                        new Num.Vector2(sIndicatorEnd.X, sIndicatorEnd.Y),
                        southColor, cardMarkerThickness);
                    windowDrawList.AddLine(new Num.Vector2(eIndicatorStart.X, eIndicatorStart.Y),
                        new Num.Vector2(eIndicatorEnd.X, eIndicatorEnd.Y),
                        eastColor, cardMarkerThickness);
                    windowDrawList.AddLine(new Num.Vector2(wIndicatorStart.X, wIndicatorStart.Y),
                        new Num.Vector2(wIndicatorEnd.X, wIndicatorEnd.Y),
                        westColor, cardMarkerThickness);
                }

                // intercard lines
                windowDrawList.AddLine(new Num.Vector2(floorStart.X, floorStart.Y), new Num.Vector2(seEnd.X, seEnd.Y),
                    eastColor, intercardThickness);
                windowDrawList.AddLine(new Num.Vector2(floorStart.X, floorStart.Y), new Num.Vector2(nwEnd.X, nwEnd.Y),
                    westColor, intercardThickness);
                windowDrawList.AddLine(new Num.Vector2(floorStart.X, floorStart.Y), new Num.Vector2(neEnd.X, neEnd.Y),
                    northColor, intercardThickness);
                windowDrawList.AddLine(new Num.Vector2(floorStart.X, floorStart.Y), new Num.Vector2(swEnd.X, swEnd.Y),
                    southColor, intercardThickness);
            }

            foreach (var doodle in doodleBag)
            {
                if (!doodle.Enabled) continue;

                if (!CheckJob(_cs.LocalPlayer.ClassJob.Id, doodle.JobsBool)) continue;

                if (doodle.Combat && !_condition[ConditionFlag.InCombat]) continue;

                if (doodle.Instance && !_condition[ConditionFlag.BoundByDuty]) continue;

                if (doodle.Type == 0)//Ring
                {
                    DrawRingWorld(_cs.LocalPlayer, doodle.Radius, doodle.Segments, doodle.Thickness, ImGui.GetColorU32(doodle.Colour),doodle.Offset,doodle.Vector);
                }
                if (doodle.Type == 1)//Line
                {
                    if (doodle.North)
                    {
                        //Get LinePos1
                        _gui.WorldToScreen(new Vector3(
                            actor.Position.X + doodle.Vector.W,//X1
                            actor.Position.Y,
                            actor.Position.Z + doodle.Vector.X//Y1
                            ), out Vector2 linePos1);

                        //Get LinePos2
                        _gui.WorldToScreen(new Vector3(
                            actor.Position.X + doodle.Vector.Y,//X2
                            actor.Position.Y,
                            actor.Position.Z + doodle.Vector.Z//Y2
                            ), out Vector2 linePos2);

                        ImGui.GetWindowDrawList().AddLine(new Vector2(linePos1.X, linePos1.Y), new Vector2(linePos2.X, linePos2.Y),
                        ImGui.GetColorU32(doodle.Colour), doodle.Thickness);
                    }
                    else
                    {
                        var sin = Math.Sin(-_cs.LocalPlayer.Rotation + Math.PI);
                        var cos = Math.Cos(-_cs.LocalPlayer.Rotation + Math.PI);
                        var xr1 = cos * (doodle.Vector.W) - sin * (doodle.Vector.X ) + actor.Position.X;
                        var yr1 = sin * (doodle.Vector.W ) + cos * (doodle.Vector.X) + actor.Position.Z;

                        var xr2 = cos * (doodle.Vector.Y) - sin * (doodle.Vector.Z) + actor.Position.X;
                        var yr2 = sin * (doodle.Vector.Y ) + cos * (doodle.Vector.Z ) + actor.Position.Z;

                        //Get LinePos1
                        _gui.WorldToScreen(new Vector3(
                            (float)xr1,//X1
                            actor.Position.Y,
                            (float)yr1//Y1
                            ), out Vector2 linePos1);

                        //Get LinePos2
                        _gui.WorldToScreen(new Vector3(
                            (float)xr2,//X2
                            actor.Position.Y,
                            (float)yr2//Y2
                            ), out Vector2 linePos2);

                        ImGui.GetWindowDrawList().AddLine(new Vector2(linePos1.X, linePos1.Y), new Vector2(linePos2.X, linePos2.Y),
                        ImGui.GetColorU32(doodle.Colour), doodle.Thickness);
                    }
                }

                if (doodle.Type == 2)//Dot
                {
                    var xOff = 0f;
                    var yOff = 0f;
                    if (doodle.Offset)
                    {
                        xOff = doodle.Vector.X;
                        yOff = doodle.Vector.Y;
                    }

                    _gui.WorldToScreen(
                         new Vector3(actor.Position.X+xOff, actor.Position.Y, actor.Position.Z+yOff),
                        out var pos);

                    if (doodle.Outline)
                    {
                        ImGui.GetWindowDrawList().AddCircle(
                            new Vector2(pos.X, pos.Y),
                            doodle.Radius + doodle.Thickness * 0.6f,
                            ImGui.GetColorU32(doodle.OutlineColour),
                            doodle.Segments, doodle.Thickness);
                    }

                    if (doodle.Filled)
                    {
                        ImGui.GetWindowDrawList().AddCircleFilled(new Vector2(pos.X, pos.Y), doodle.Radius, ImGui.GetColorU32(doodle.Colour), doodle.Segments);
                    }
                    else
                    {
                        ImGui.GetWindowDrawList().AddCircle(new Vector2(pos.X, pos.Y), doodle.Radius, ImGui.GetColorU32(doodle.Colour), doodle.Segments, doodle.Thickness);
                    }
                }
            }
            ImGui.End();
            ImGui.PopStyleVar();
        }
    }
}
