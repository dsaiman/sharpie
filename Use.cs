using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;


namespace Tryhardamere
{
    internal class Use
    {
        public static void UseQSmart()
        {
            if (!Trynda.Q.IsReady())
            {
                return;
            }
            if (Trynda.MyHpPerc() <= Tryhardamere.Config.Item("QonHp").GetValue<Slider>().Value)
            {
                Trynda.Q.Cast();
            }
        }

        public static void UseWSmart(Obj_AI_Hero target)
        {
            if (!Trynda.W.IsReady())
            {
                return;
            }
            var trueAARange = Trynda.Player.AttackRange + target.BoundingRadius;
            if (Trynda.Player.Distance(target) > trueAARange && Trynda.Player.Distance(target) < Trynda.W.Range)
            {
                if ((OutgoingDamage.TimeToReach(target) > 1.7f || OutgoingDamage.TimeToReach(target) < 0.0f) &&
                    !target.IsFacing(Trynda.Player))
                {
                    Trynda.W.Cast();
                }
            }
        }

        public static void UseESmart(Obj_AI_Hero target)
        {
            if (!Trynda.E.IsReady())
            {
                return;
            }
            var trueAARange = Trynda.Player.AttackRange + target.BoundingRadius;
            var trueERange = target.BoundingRadius + Trynda.E.Range;
            if (Trynda.Player.Distance(target) > trueAARange && Trynda.Player.Distance(target) < trueERange)
            {
                if (OutgoingDamage.TimeToReach(target) > 1.7f || OutgoingDamage.TimeToReach(target) < 0.0f)
                {
                    Trynda.E.Cast(
                        Trynda.Player.Position.To2D()
                            .Extend(target.Position.To2D(), target.Distance3D(Trynda.Player) + 200));
                }
            }
        }

        public static void UseRSmart()
        {
            if (!Trynda.R.IsReady() || Trynda.Player.HasBuff("Recall") || ObjectManager.Player.InFountain())
            {
                return;
            }

            if ((Trynda.MyHpPerc() <= Tryhardamere.Config.Item("RonHp").GetValue<Slider>().Value) &&
                (Utility.CountEnemysInRange(900) > 0))
            {
                Trynda.R.Cast();
            }
        }

        public static void UseWTrade(Obj_AI_Hero target)
        {
            if (target.Distance(Trynda.Player) < 250 && Trynda.W.IsReady())
            {
                Trynda.W.Cast();
            }
        }

        public static void UseIgnite(Obj_AI_Hero target)
        {
            if (Trynda.IgniteSlot != SpellSlot.Unknown &&
                Trynda.Player.Spellbook.CanUseSpell(Trynda.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= Trynda.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                {
                    Trynda.Player.Spellbook.CastSpell(Trynda.IgniteSlot, target);
                }
            }
        }

        public static void UseHydra(Obj_AI_Hero target)
        {
            if (Items.CanUseItem(3074) && target.Distance(Trynda.Player) < 420)
            {
                Items.UseItem(3074);
            }
            if (Items.CanUseItem(3077) && target.Distance(Trynda.Player) < 420)
            {
                Items.UseItem(3077);
            }
        }

        public static void UseHydraLc()
        {
            var minions = MinionManager.GetMinions(Trynda.Player.Position, 420).ToArray();
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
            if ((!Trynda.W.IsReady() &&
                 target.Distance(Trynda.Player) > Trynda.Player.AttackRange + target.BoundingRadius) ||
                Trynda.MyHpPerc() < 40)
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
            if (Items.CanUseItem(3074) && target.Distance(Trynda.Player) < 420)
            {
                Items.UseItem(3074);
            }
            if (Items.CanUseItem(3077) && target.Distance(Trynda.Player) < 420)
            {
                Items.UseItem(3077);
            }
        }
    }
}