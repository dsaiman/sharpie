using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jarvan4
{
    class Use
    {
        #region Combo

        public static void UseQCombo(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }

            var pred = J.Spells[SpellSlot.Q].GetPrediction(target);
            if (pred.Hitchance >= HitChance.High)
                J.Spells[SpellSlot.Q].Cast(pred.CastPosition);
        }

        public static void UseECombo(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.E].IsReady())
            {
                return;
            }

            var pred = J.Spells[SpellSlot.E].GetPrediction(target);
            var castPosition = pred.UnitPosition;
            if (pred.Hitchance <= HitChance.High)
            {
                return;
            }
            J.Spells[SpellSlot.E].Cast(
                J.Player.Position.Extend(
                    castPosition, J.Player.Distance(castPosition) + J.Spells[SpellSlot.E].Width/2 ));
        }

        public static void UseEQCombo(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.E].IsReady() || !J.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }
            UseECombo(target);
            if (ObjectManager.Get<Obj_AI_Base>().Count(obj => obj.Name == "Beacon" && obj.Distance(J.Player) < 900f) < 1)
            {
                return;
            }
            var eqRectangle = new Geometry.Polygon.Rectangle(J.Player.Position, ObjectManager.Get<Obj_AI_Base>().First(obj => obj.Name=="Beacon").Position, 180);
            if (eqRectangle.IsInside(target.Position))
            {
                J.Spells[SpellSlot.Q].Cast(ObjectManager.Get<Obj_AI_Base>().First(obj => obj.Name=="Beacon").Position);
            }
        }

        public static void UseEQ(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.E].IsReady() || !J.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }
            J.Spells[SpellSlot.E].Cast(J.Player.Position.Extend(target.Position, J.Spells[SpellSlot.E].Range));
            UseQCombo(target);
        }

        public static void UseEQFlashCombo(Obj_AI_Hero target)
        {
            if (J.Player.Spellbook.CanUseSpell(J.FlashSlot) != SpellState.Ready)
            {
                return;
            }
            if (J.Spells[SpellSlot.E].IsReady())
            {
                J.Spells[SpellSlot.E].Cast(J.Player.Position.Extend(target.Position, J.Spells[SpellSlot.E].Range));
            }
            if (ObjectManager.Get<Obj_AI_Base>().Count(obj => obj.Name == "Beacon" && obj.Distance(J.Player) < 900f) < 1)
            {
                return;
            }
            if (J.Spells[SpellSlot.Q].IsReady())
            {
                J.Spells[SpellSlot.Q].Cast(ObjectManager.Get<Obj_AI_Base>().First(obj => obj.Name == "Beacon").Position);
            }
            Utility.DelayAction.Add(750, () => J.Player.Spellbook.CastSpell(J.FlashSlot, target.Position));
        }

        public static void UseEQRCombo(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.R].IsReady())
            {
                return;
            }
            UseEQ(target);
            J.Spells[SpellSlot.R].Cast(target);
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
            if (!J.Spells[SpellSlot.R].IsReady())
            {
                return;
            }
            J.Spells[SpellSlot.R].Cast(target);
        }

        public static bool GapUsed(Obj_AI_Hero target)
        {
            if ((from spell in target.Spellbook.Spells from gapcloser in AntiGapcloser.Spells where spell.Name.ToLower() == gapcloser.SpellName && 
                     target.Spellbook.CanUseSpell(spell.Slot) != SpellState.Cooldown &&
                     JMenu.Config.Item("waitGap" + spell.Slot + target.ChampionName).GetValue<bool>()
                     select spell).Any())
                return false;
            return true;
        }
        #endregion

        #region Mixed and LC
        public static void UseQMixed(Obj_AI_Hero target)
        {
            if (!J.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }

            var pred = J.Spells[SpellSlot.Q].GetPrediction(target);
            if (pred.Hitchance >= HitChance.High)
                J.Spells[SpellSlot.Q].Cast(pred.CastPosition);
        }

        public static void UseQLC()
        {
            if (!J.Spells[SpellSlot.Q].IsReady())
            {
                return;
            }

            var allMinions = MinionManager.GetMinions(J.Player.Position, J.Spells[SpellSlot.Q].Range);
            if (allMinions.Count < 3)
            {
                return;
            }
            var qMinionsPositions = MinionManager.GetMinionsPredictedPositions(
                allMinions, J.Spells[SpellSlot.Q].Delay, J.Spells[SpellSlot.Q].Width, J.Spells[SpellSlot.Q].Speed,
                J.Player.Position, J.Spells[SpellSlot.Q].Range, false, SkillshotType.SkillshotLine);
            var bestQFarm = MinionManager.GetBestLineFarmLocation(
                qMinionsPositions, J.Spells[SpellSlot.Q].Width, J.Spells[SpellSlot.Q].Range);
            if (bestQFarm.MinionsHit > 2)
                J.Spells[SpellSlot.Q].Cast(bestQFarm.Position);
        }
        #endregion

        #region CommonUse
        public static void UseIgnite(Obj_AI_Hero target)
        {
            if (J.IgniteSlot != SpellSlot.Unknown &&
                J.Player.Spellbook.CanUseSpell(J.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= J.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite) || OutgoingDamage.ComboDamage(target) >= target.Health)
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
            var minions = MinionManager.GetMinions(J.Player.Position, 420).ToArray();
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
            if ((!J.Spells[SpellSlot.E].IsReady() &&
                !J.Spells[SpellSlot.Q].IsReady() &&
                 target.Distance(J.Player) > J.Player.AttackRange + target.BoundingRadius) ||
                J.Player.HealthPercentage() < 40)
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
            if (Items.CanUseItem(3143) && (J.Player.CountEnemiesInRange(500f) > 1 || 
                (target.Distance(J.Player) > J.Player.AttackRange && !J.Spells[SpellSlot.E].IsReady() && target.Distance(J.Player) < 500f)))
            {
                Items.UseItem(3143);
            }

        }

        public static void UseSmiteOnChamp(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(J.Spells[SpellSlot.E].Range) && J.SmiteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell((J.SmiteSlot)) == SpellState.Ready &&
                (J.GetSmiteType() == "s5_summonersmiteplayerganker" ||
                 J.GetSmiteType() == "s5_summonersmiteduel"))
            {
                ObjectManager.Player.Spellbook.CastSpell(J.SmiteSlot, target);
            }
        }
        #endregion
    }
}
