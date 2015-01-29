using System;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;

namespace Jarvan4
{
    internal class Jarvan4
    {
        #region Load

        public const string CharName = "JarvanIV";

        public static Obj_AI_Hero Target;

        public Jarvan4()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            try
            {
                Game.PrintChat("<font color=\"#00BFFF\">Jarvan IV# -</font> Loaded!");
                JMenu.AddMenu();
                J.SetSkillShots();
                J.GetSmiteSlot();

                Drawing.OnDraw += OnDraw;
                Game.OnGameUpdate += OnGameUpdate;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                Interrupter.OnPossibleToInterrupt += OnPossibleInterrupt;
                AntiGapcloser.OnEnemyGapcloser += OnGapCloser;
            }

            catch
            {
                Game.PrintChat("Oops. Something went wrong with JarvanIV");
            }
        }

        #endregion

        private static void OnGameUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || MenuGUI.IsChatOpen || ObjectManager.Player.IsChannelingImportantSpell() ||
                ObjectManager.Player.IsRecalling())
            {
                return;
            }

            if (JMenu.Config.Item("EQR").GetValue<KeyBind>().Active)
            {
                var fullRange = J.Spells[SpellSlot.E].Range + J.Spells[SpellSlot.R].Range - 60;
                Target = TargetSelector.GetTarget(fullRange, TargetSelector.DamageType.Physical);
                if (Target.IsValidTarget())
                {
                    Use.UseEQRCombo(Target);
                    J.Combo(Target);
                }
                else
                {
                    J.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }

            if (JMenu.Config.Item("EQFlash").GetValue<KeyBind>().Active)
            {
                var fullRange = J.Spells[SpellSlot.E].Range + 380;
                Target = TargetSelector.GetTarget(fullRange, TargetSelector.DamageType.Physical);
                if (Target.IsValidTarget())
                {
                    Use.UseEQFlashCombo(Target);
                    J.Combo(Target);
                }
                else
                {
                    J.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }

            if (JMenu.Config.Item("Flee").GetValue<KeyBind>().Active)
            {
                J.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                Use.UseEQFlee();
            }


            switch (J.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Target = TargetSelector.GetTarget(
                        J.Spells[SpellSlot.E].Range + J.Spells[SpellSlot.W].Range, TargetSelector.DamageType.Physical);
                    if (Target.IsValidTarget())
                    {
                        var objAiBase = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(obj => obj.Name.Contains("JarvanIVWall"));
                        if (
                            objAiBase != null && objAiBase.CountEnemiesInRange(325) < 1)
                        {
                            J.Spells[SpellSlot.R].Cast();
                        }

                        J.Combo(Target);
                    }
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Target = TargetSelector.GetTarget(J.Spells[SpellSlot.Q].Range, TargetSelector.DamageType.Physical);
                    if (Target.IsValidTarget())
                    {
                        J.Mixed(Target);
                    }
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    J.LaneClear();
                    if (JMenu.Config.Item("JungleMode").GetValue<bool>())
                    {
                        J.JungleClear();
                    }
                    break;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (JMenu.Config.Item("drawQ").GetValue<bool>())
            {
                Render.Circle.DrawCircle(J.Player.Position, J.Spells[SpellSlot.Q].Range, Color.DarkRed);
            }
            if (JMenu.Config.Item("drawE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(J.Player.Position, J.Spells[SpellSlot.E].Range, Color.DeepPink);
            }
            if (JMenu.Config.Item("drawR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(J.Player.Position, J.Spells[SpellSlot.R].Range, Color.GreenYellow);
            }
            if (JMenu.Config.Item("drawEQR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(
                    J.Player.Position, J.Spells[SpellSlot.E].Range + J.Spells[SpellSlot.R].Range - 60, Color.DarkBlue);
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (J.Player.IsDead || J.Player.InFountain())
            {
                return;
            }


            if (sender.IsMe && args.SData.Name.Contains("Martial") && args.Target.Type == GameObjectType.obj_AI_Hero)
            {
                OutgoingDamage.PassiveUp = false;
                Utility.DelayAction.Add(6000, () => OutgoingDamage.PassiveUp = true);
            }

            //if (JMenu.Config.Item("TowerTrap").GetValue<bool>() && sender.Distance(J.Player) < 1500f && sender.IsAlly &&
            //    sender.Type == GameObjectType.obj_AI_Turret && args.Target.Type == GameObjectType.obj_AI_Hero)
            //{
            //    var target = (Obj_AI_Hero) args.Target;
            //    if (J.Spells[SpellSlot.R].IsReady() && target.IsValidTarget() &&
            //        target.Distance(J.Player) < J.Spells[SpellSlot.R].Range)
            //    {
            //        J.Spells[SpellSlot.R].Cast(target);
            //        if (J.Spells[SpellSlot.Q].IsReady() && J.Spells[SpellSlot.E].IsReady())
            //        {
            //            var castPos = J.Player.Position.Extend(target.Position, J.Spells[SpellSlot.E].Range);
            //            J.Spells[SpellSlot.E].Cast(castPos);
            //            Use.LastE = Environment.TickCount;
            //            if (Environment.TickCount - Use.LastE >= 500)
            //            {
            //                J.Spells[SpellSlot.Q].Cast(
            //                    ObjectManager.Get<Obj_AI_Base>().First(obj => obj.Name == "Beacon").Position);
            //            }
            //        }
            //    }
            //}

            if (JMenu.Config.Item("WIncoming").GetValue<bool>() && sender.IsEnemy &&
                sender.Type == GameObjectType.obj_AI_Hero && args.Target.IsMe && J.Spells[SpellSlot.W].IsReady() &&
                sender.Distance(J.Player.Position) < J.Spells[SpellSlot.W].Range)
            {
                J.Spells[SpellSlot.W].Cast();
            }
        }

        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (!JMenu.Config.Item("WGap").GetValue<bool>() || !gapcloser.Sender.IsValidTarget() ||
                !(ObjectManager.Player.Distance(gapcloser.Sender.Position) <= J.Spells[SpellSlot.W].Range))
            {
                return;
            }
            if (gapcloser.End.Distance(J.Player.Position) < J.Spells[SpellSlot.W].Range)
            {
                J.Spells[SpellSlot.W].Cast();
            }
        }

        private static void OnPossibleInterrupt(Obj_AI_Hero sender, InterruptableSpell spell)
        {
            if (sender.IsAlly || !JMenu.Config.Item("EQInterrupt").GetValue<bool>() ||
                !JMenu.Config.Item("Interr" + spell.Slot + sender.ChampionName).GetValue<bool>())
            {
                return;
            }
            if (J.Spells[SpellSlot.E].IsReady() && J.Spells[SpellSlot.Q].IsReady() &&
                sender.Distance(ObjectManager.Player) < J.Spells[SpellSlot.E].Range)
            {
                Use.UseEQCombo(sender);
            }
        }
    }
}