using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace TwistedFate
{
    internal class TwistedFate
    {
        public static Menu Config;
        public static Obj_AI_Hero Target;

        public TwistedFate()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            try
            {
                if (ObjectManager.Player.BaseSkinName != "TwistedFate")
                {
                    return;
                }

                Config = new Menu("Twisted Fate", "TwistedFate", true);

                //Orbwalker
                Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
                TF.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

                //Target Selector
                var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
                TargetSelector.AddToMenu(targetSelectorMenu);
                Config.AddSubMenu(targetSelectorMenu);

                //Utilities
                Config.AddSubMenu(new Menu("Utilities", "utils"));
                Config.SubMenu("utils").AddItem(new MenuItem("UseItems", "Use Items in Combo")).SetValue(true);
                Config.SubMenu("utils").AddItem(new MenuItem("StunOnGC", "Auto Stun on GapCloser")).SetValue(true);
                Config.SubMenu("utils").AddItem(new MenuItem("AutoY", "Select yellow card after R").SetValue(true));

                //Drawings
                Config.AddSubMenu(new Menu("Drawings", "draw"));
                Config.SubMenu("draw").AddItem(new MenuItem("DrawCombo", "Draw Combo Damage")).SetValue(true);
                Config.SubMenu("draw").AddItem(new MenuItem("drawQ", "Q Range").SetValue(true));
                Config.SubMenu("draw").AddItem(new MenuItem("drawW", "W Range").SetValue(true));
                Config.SubMenu("draw").AddItem(new MenuItem("drawR", "R Range (Minimap)").SetValue(true));
                Config.SubMenu("draw").AddItem(new MenuItem("DrawEnemy", "Draw Enemy HP in R range")).SetValue(true);


                Game.PrintChat("<font color=\"#00BFFF\">Twisted Fate# -</font> Loaded!");

                Config.AddToMainMenu();
                TF.GetSmiteSlot();
                TF.SetSkillShots();

                Drawing.OnDraw += OnDraw;
                Drawing.OnEndScene += DrawingOnOnEndScene;
                Game.OnGameUpdate += OnGameUpdate;

                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                AntiGapcloser.OnEnemyGapcloser += OnGapCloser;
            }
            catch
            {
                Game.PrintChat("Oops. Something went wrong with Twisted Fate");
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (TF.Orbwalker.ActiveMode.ToString() == "Combo")
            {
                Target = TargetSelector.GetTarget(TF.Q.Range, TargetSelector.DamageType.Magical);
                if (Target != null && Target.IsValidTarget())
                {
                    TF.Combo(Target);
                }
            }

            if (TF.Orbwalker.ActiveMode.ToString() == "Mixed")
            {
                Target = TargetSelector.GetTarget(TF.Q.Range, TargetSelector.DamageType.Magical);
                if (Target != null && Target.IsValidTarget())
                {
                    TF.Harass(Target);
                }
                else
                {
                    TF.Harass();
                }
            }

            if (TF.Orbwalker.ActiveMode.ToString() == "LaneClear")
            {
                TF.LaneClear();
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "gate" && Config.Item("AutoY").GetValue<bool>())
            {
                CardSelector.StartSelecting(Cards.Yellow);
            }
        }

        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (Config.Item("StunOnGC").GetValue<bool>() && gapcloser.Sender.IsValidTarget() &&
                ObjectManager.Player.Distance(gapcloser.Sender.Position) < TF.W.Range)
            {
                if (CardSelector.Status == SelectStatus.Ready)
                {
                    CardSelector.StartSelecting(Cards.Yellow);
                }
                if (CardSelector.Status == SelectStatus.Selected)
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AutoAttack, gapcloser.Sender);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("drawQ").GetValue<bool>())
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, TF.Q.Range, Color.DeepPink);
            }

            if (Config.Item("drawW").GetValue<bool>())
            {
                Drawing.DrawCircle(ObjectManager.Player.Position, TF.W.Range, Color.DeepSkyBlue);
            }

            if (Config.Item("DrawEnemy").GetValue<bool>())
            {
                float i = 0;
                foreach (var hero in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            hero =>
                                hero.IsEnemy && hero.Distance(ObjectManager.Player) < 5500 && !hero.IsDead &&
                                hero.IsVisible))
                {
                    var champion = hero.SkinName;
                    var percent = (int) (hero.Health / hero.MaxHealth * 100);
                    var color = Color.Red;
                    if (percent > 25)
                    {
                        color = Color.Orange;
                    }
                    if (percent > 50)
                    {
                        color = Color.Yellow;
                    }
                    if (percent > 75)
                    {
                        color = Color.Green;
                    }
                    Drawing.DrawText(Drawing.Width * 0.8f, Drawing.Height * 0.1f + i, color, champion);
                    Drawing.DrawText(Drawing.Width * 0.9f, Drawing.Height * 0.1f + i, color, percent.ToString());
                    i += 20f;
                }
            }
        }

        private static void DrawingOnOnEndScene(EventArgs args)
        {
            if (Config.Item("drawR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, TF.R.Range, Color.White);
            }
        }
    }
}