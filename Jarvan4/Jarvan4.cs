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
                Game.OnUpdate += OnGameUpdate;
                Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
                Interrupter2.OnInterruptableTarget += OnPossibleInterrupt;
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

            if (Environment.TickCount - Use.LastE >= 280)
            {
                Use.UsedE = true;
                Utility.DelayAction.Add(8000, () => Use.UsedE = false);
            }

            if (JMenu.Config.Item("EQR").GetValue<KeyBind>().Active)
            {
                var fullRange = J.Spells[SpellSlot.E].Range + J.Spells[SpellSlot.R].Range;
                Target = TargetSelector.GetTarget(fullRange, TargetSelector.DamageType.Physical);
                if (Target.IsValidTarget())
                {
                    Use.UseEQRCombo(Target);
                    Orbwalking.Orbwalk(Target, Game.CursorPos);
                    J.Combo(Target);
                }
                else
                {
                    J.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }

            if (JMenu.Config.Item("EQFlash").GetValue<KeyBind>().Active)
            {
                var fullRange = J.Spells[SpellSlot.E].Range + 430;
                Target = TargetSelector.GetTarget(fullRange, TargetSelector.DamageType.Physical);
                if (Target.IsValidTarget())
                {
                    Use.UseEQFlashCombo(Target);
                    Orbwalking.Orbwalk(Target, Game.CursorPos);
                    J.Combo(Target);
                }
                else
                {
                    J.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }

            if (JMenu.Config.Item("Flee").GetValue<KeyBind>().Active)
            {
                Use.UseEQFlee(); 
                J.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }


            switch (J.Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Target = TargetSelector.GetTarget(
                        J.Spells[SpellSlot.E].Range + J.Spells[SpellSlot.W].Range + 600f, TargetSelector.DamageType.Physical);
                    if (Target.IsValidTarget())
                    {
                        //var objAiBase = ObjectManager.Get<Obj_AI_Base>().FirstOrDefault(obj => obj.Name.Contains("JarvanIVWall"));
                        //if (
                        //    objAiBase != null && objAiBase.CountEnemiesInRange(325) < 1)
                        //{
                        //    J.Spells[SpellSlot.R].Cast();
                        //}

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
                Render.Circle.DrawCircle(J.Player.ServerPosition, J.Spells[SpellSlot.Q].Range, Color.DarkRed);
            }
            if (JMenu.Config.Item("drawE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(J.Player.ServerPosition, J.Spells[SpellSlot.E].Range, Color.DeepPink);
            }
            if (JMenu.Config.Item("drawR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(J.Player.ServerPosition, J.Spells[SpellSlot.R].Range, Color.GreenYellow);
            }
            if (JMenu.Config.Item("drawEQR").GetValue<bool>())
            {
                Render.Circle.DrawCircle(
                    J.Player.ServerPosition, J.Spells[SpellSlot.E].Range + J.Spells[SpellSlot.R].Range - 60, Color.DarkBlue);
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
                sender.Distance(J.Player.ServerPosition) < J.Spells[SpellSlot.W].Range)
            {
                J.Spells[SpellSlot.W].Cast();
            }
        }

        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (!JMenu.Config.Item("WGap").GetValue<bool>() || !gapcloser.Sender.IsValidTarget() ||
                !(ObjectManager.Player.Distance(gapcloser.Sender.ServerPosition) <= J.Spells[SpellSlot.W].Range))
            {
                return;
            }
            if (gapcloser.End.Distance(J.Player.ServerPosition) < J.Spells[SpellSlot.W].Range)
            {
                J.Spells[SpellSlot.W].Cast();
            }
        }

        private static void OnPossibleInterrupt(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (sender.IsAlly || !JMenu.Config.Item("EQInterrupt").GetValue<bool>() || args.DangerLevel == Interrupter2.DangerLevel.Low)
            {
                return;
            }
            if (sender.Distance(ObjectManager.Player) < J.Spells[SpellSlot.E].Range && sender.IsValidTarget())
            {
                Use.UseEQCombo(sender);
            }
        }
    }
}