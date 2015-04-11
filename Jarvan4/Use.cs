﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jarvan4
{
    internal class Use
    {

        public static int LastQ;
        public static int LastE;
        public static bool UsedE = false;
        public static bool UsedQ = false;
        public static bool R1Up = false;

        public static SharpDX.Vector3 EPosition;

        #region Combo

        public static void UseQCombo(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }
            var pred = J.Spells[SpellSlot.Q].GetPrediction(target);
            if (pred.Hitchance < HitChance.High)
            {
                return;
            }
            J.Spells[SpellSlot.Q].Cast(pred.CastPosition);
            LastQ = Environment.TickCount;
        }

        public static void UseECombo(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.E].IsReady())
            {
                return;
            }
            var pred = J.Spells[SpellSlot.E].GetPrediction(target);
            var castPosition = pred.UnitPosition;
            if (pred.Hitchance < HitChance.High)
            {
                return;
            }
            EPosition = J.Player.ServerPosition.Extend(
                castPosition,
                Math.Min(J.Player.Distance(castPosition) + J.Spells[SpellSlot.E].Width / 2, J.Spells[SpellSlot.E].Range));
            J.Spells[SpellSlot.E].Cast(EPosition);
            LastE = Environment.TickCount;
        }

        public static void UseEQCombo(Obj_AI_Hero target)
        {
            if (J.Spells[SpellSlot.Q].IsReady() && J.Spells[SpellSlot.E].IsReady())
            {
                UseECombo(target);
            }
            if (UsedE && J.Spells[SpellSlot.Q].IsReady())
            {
                var eqRectangle = new Geometry.Polygon.Rectangle(
                    J.Player.ServerPosition, EPosition, 180);
                if ((eqRectangle.IsInside(target.ServerPosition)))
                {
                    J.Spells[SpellSlot.Q].Cast(EPosition);
                }
            }
        }

        public static void UseEQ(Obj_AI_Hero target)
        {
            if (J.Spells[SpellSlot.E].IsReady() && J.Spells[SpellSlot.Q].IsReady())
            {
                EPosition = J.Player.ServerPosition.Extend(target.ServerPosition, J.Spells[SpellSlot.E].Range + 100f);
                J.Spells[SpellSlot.E].Cast(EPosition);
                LastE = Environment.TickCount;
            }
            if (UsedE && J.Spells[SpellSlot.Q].IsReady())
            {
                J.Spells[SpellSlot.Q].Cast(EPosition);
            }
        }

        public static void UseEQFlashCombo(Obj_AI_Hero target)
        {

            if (J.Spells[SpellSlot.E].IsReady() && J.Spells[SpellSlot.Q].IsReady())
            {
                EPosition = J.Player.ServerPosition.Extend(target.ServerPosition, J.Spells[SpellSlot.E].Range);
                J.Spells[SpellSlot.E].Cast(EPosition);
                LastE = Environment.TickCount;
            }
            if (J.Spells[SpellSlot.Q].IsReady() && UsedE)
            {
                    J.Spells[SpellSlot.Q].Cast(EPosition);
                    LastQ = Environment.TickCount;
            }
            if (J.Player.Spellbook.CanUseSpell(J.FlashSlot) == SpellState.Ready && Environment.TickCount - LastQ >= 50 && target.Distance(J.Player.Position) <= 350)
            {
                J.Player.Spellbook.CastSpell(J.FlashSlot, target.Position);
            }
        }

        public static void UseEQRCombo(Obj_AI_Hero target)
        {
            UseEQ(target);
            UseRCombo(target);
        }

        public static void UseWCombo(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.W].IsReady())
            {
                return;
            }
            J.Spells[SpellSlot.W].Cast();
        }

        public static void UseRCombo(Obj_AI_Hero target)
        {
            if (J.Spells[SpellSlot.R].IsReady())
            {
                J.Spells[SpellSlot.R].Cast(target);
            }
        }

        public static bool GapUsed(Obj_AI_Hero target)
        {
            if ((from spell in target.Spellbook.Spells
                from gapcloser in AntiGapcloser.Spells
                where
                    spell.Name.ToLower() == gapcloser.SpellName &&
                    target.Spellbook.CanUseSpell(spell.Slot) != SpellState.Cooldown &&
                    JMenu.Config.Item("waitGap" + spell.Slot + target.ChampionName).GetValue<bool>()
                select spell).Any())
            {
                return false;
            }
            return true;
        }

        #endregion

        #region Mixed and LC

        public static void UseEQFlee()
        {
            if (J.Spells[SpellSlot.E].IsReady() && J.Spells[SpellSlot.Q].IsReady())
            {
                EPosition = J.Player.ServerPosition.Extend(Game.CursorPos, J.Spells[SpellSlot.E].Range + 100f);
                J.Spells[SpellSlot.E].Cast(EPosition);
                LastE = Environment.TickCount;
            }
            if (J.Spells[SpellSlot.Q].IsReady() && UsedE)
            {
                    J.Spells[SpellSlot.Q].Cast(EPosition);
            }
        }

        public static void UseQMixed(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }
            var pred = J.Spells[SpellSlot.Q].GetPrediction(target);
            if (pred.Hitchance >= HitChance.High)
            {
                J.Spells[SpellSlot.Q].Cast(pred.CastPosition);
            }
        }

        public static void UseQLC()
        {
            if (!J.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }

            var allMinions = MinionManager.GetMinions(J.Player.ServerPosition, J.Spells[SpellSlot.Q].Range);
            if (allMinions.Count < 3)
            {
                return;
            }
            var qMinionsPositions = MinionManager.GetMinionsPredictedPositions(
                allMinions, J.Spells[SpellSlot.Q].Delay, J.Spells[SpellSlot.Q].Width, J.Spells[SpellSlot.Q].Speed,
                J.Player.ServerPosition, J.Spells[SpellSlot.Q].Range, false, SkillshotType.SkillshotLine);
            var bestQFarm = MinionManager.GetBestLineFarmLocation(
                qMinionsPositions, J.Spells[SpellSlot.Q].Width, J.Spells[SpellSlot.Q].Range);
            if (bestQFarm.MinionsHit > 2)
            {
                J.Spells[SpellSlot.Q].Cast(bestQFarm.Position);
            }
        }

        #endregion

        #region CommonUse

        public static void UseIgnite(Obj_AI_Hero target)
        {
            if (J.IgniteSlot != SpellSlot.Unknown && J.Player.Spellbook.CanUseSpell(J.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= J.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) ||
                    OutgoingDamage.ComboDamage(target) >= target.Health)
                {
                    J.Player.Spellbook.CastSpell(J.IgniteSlot, target);
                }
            }
        }

        public static void UseHydra(Obj_AI_Hero target)
        {
            if (Items.CanUseItem(3074) && target.Distance(J.Player) < 420)
            {
                Items.UseItem(3074);
            }
            if (Items.CanUseItem(3077) && target.Distance(J.Player) < 420)
            {
                Items.UseItem(3077);
            }
        }

        public static void UseHydraLc()
        {
            var minions = MinionManager.GetMinions(J.Player.ServerPosition, 420).ToArray();
            if (minions.Length > 1)
            {
                if (Items.CanUseItem(3074))
                {
                    Items.UseItem(3074);
                }
                if (Items.CanUseItem(3077))
                {
                    Items.UseItem(3077);
                }
            }
        }

        public static void UseComboItems(Obj_AI_Hero target)
        {
            //BOTRK and Cutlass
            if ((!J.Spells[SpellSlot.E].IsReady() && !J.Spells[SpellSlot.Q].IsReady() &&
                 target.Distance(J.Player) > J.Player.AttackRange + target.BoundingRadius) ||
                J.Player.HealthPercent < 40)
            {
                if (Items.CanUseItem(3153))
                {
                    Items.UseItem(3153, target);
                }
                if (Items.CanUseItem(3144))
                {
                    Items.UseItem(3144, target);
                }
            }
            //Ghostblade
            if (Items.CanUseItem(3142))
            {
                Items.UseItem(3142);
            }

            //Hydra and Tiamat
            if (Items.CanUseItem(3074) && target.Distance(J.Player) < 420)
            {
                Items.UseItem(3074);
            }
            if (Items.CanUseItem(3077) && target.Distance(J.Player) < 420)
            {
                Items.UseItem(3077);
            }

            //Locket
            if (Items.CanUseItem(3190) &&
                (J.Player.CountEnemiesInRange(600f) > 1 && J.Player.CountAlliesInRange(600f) > 1) ||
                J.Player.CountEnemiesInRange(600f) > 2)
            {
                Items.UseItem(3190);
            }

            //Randuin
            if (Items.CanUseItem(3143) &&
                (J.Player.CountEnemiesInRange(500f) > 1 ||
                 (target.Distance(J.Player) > J.Player.AttackRange && !J.Spells[SpellSlot.E].IsReady() &&
                  target.Distance(J.Player) < 500f)))
            {
                Items.UseItem(3143);
            }
        }

        public static void UseSmiteOnChamp(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(J.Spells[SpellSlot.E].Range) && J.SmiteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell((J.SmiteSlot)) == SpellState.Ready &&
                (J.GetSmiteType() == "s5_summonersmiteplayerganker" || J.GetSmiteType() == "s5_summonersmiteduel"))
            {
                ObjectManager.Player.Spellbook.CastSpell(J.SmiteSlot, target);
            }
        }

        #endregion
    }
}