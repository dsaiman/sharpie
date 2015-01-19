using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TwistedFate
{
    internal class Use
    {
        public static void UseQCombo(Obj_AI_Hero target)
        {
            var pred = TF.Q.GetPrediction(target);
            {
                if (pred.Hitchance > HitChance.VeryHigh && TF.Q.IsReady() &&
                    target.Distance(ObjectManager.Player) < TF.Q.Range)
                {
                    TF.Q.Cast(pred.UnitPosition);
                }
                else if (pred.Hitchance == HitChance.VeryHigh && target.Distance(ObjectManager.Player) > 100 &&
                         target.Distance(ObjectManager.Player) < TF.Q.Range)
                {
                    TF.Q.Cast(pred.UnitPosition);
                }
            }
        }

        public static void UseQLaneClear()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, TF.Q.Range);
            if (allMinions == null || ObjectManager.Player.ManaPercentage() < 20)
            {
                return;
            }

            foreach (var minion in allMinions)
            {
                var minionAround = MinionManager.GetMinions(minion.Position, 100f);

                if (minion.Health < TF.Q.GetDamage(minion) && TF.Q.IsReady() && minionAround.Count > 0)
                {
                    TF.Q.Cast(TF.GetBestQPosition(minion, minion.Position.To2D()));
                }
            }
        }

        public static void UseQHarass()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, TF.Q.Range);
            if (allMinions == null || ObjectManager.Player.ManaPercentage() < 20)
            {
                return;
            }

            foreach (var minion in allMinions)
            {
                var minionAround = MinionManager.GetMinions(minion.Position, 100f);

                if (minion.Health < TF.Q.GetDamage(minion) &&
                    minion.Distance(ObjectManager.Player) > ObjectManager.Player.AttackRange && TF.Q.IsReady() &&
                    minionAround.Count > 0)
                {
                    TF.Q.Cast(TF.GetBestQPosition(minion, minion.Position.To2D()));
                }
            }
        }

        public static void UseQHarass(Obj_AI_Hero target)
        {
            UseQHarass();
            if (target.Distance(ObjectManager.Player) < TF.Q.Range && TF.Q.IsReady())
            {
                var pred = TF.Q.GetPrediction(target);
                if (pred.Hitchance > HitChance.High && ObjectManager.Player.ManaPercentage() > 20)
                {
                    TF.Q.Cast(TF.GetBestQPosition(target, pred.UnitPosition.To2D()));
                }
            }
        }

        public static void UseWCombo(Obj_AI_Hero target)
        {
            if (CardSelector.Status == SelectStatus.Ready)
            {
                if (ObjectManager.Player.CountEnemiesInRange(TF.Q.Range) > 2)
                {
                    CardSelector.StartSelecting(Cards.Red);
                }
                else
                {
                    CardSelector.StartSelecting(Cards.Yellow);
                }
            }
        }

        public static void UseWLaneClear()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, TF.W.Range);
            if (allMinions == null)
            {
                return;
            }

            foreach (var minion in allMinions)
            {
                var minionAround = MinionManager.GetMinions(minion.Position, 100f);
                switch (CardSelector.Status)
                {
                    case SelectStatus.Ready:
                        if (ObjectManager.Player.ManaPercentage() < 50)
                        {
                            CardSelector.StartSelecting(Cards.Blue);
                        }
                        else if (minionAround.Count > 1)
                        {
                            CardSelector.StartSelecting(Cards.Red);
                        }
                        break;
                    case SelectStatus.Selected:
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                        break;
                }
            }
        }

        public static void UseWHarass()
        {
            if (CardSelector.Status == SelectStatus.Ready)
            {
                CardSelector.StartSelecting(Cards.Blue);
            }
        }

        public static void UseWHarass(Obj_AI_Hero target)
        {
            switch (CardSelector.Status)
            {
                case SelectStatus.Ready:
                    if (target.Distance(ObjectManager.Player.Position) <= TF.W.Range + 200f)
                    {
                        CardSelector.StartSelecting(Cards.Yellow);
                    }
                    else if (ObjectManager.Player.ManaPercentage() < 50f)
                    {
                        CardSelector.StartSelecting(Cards.Blue);
                    }
                    else if (target.Distance(ObjectManager.Player.Position) > TF.W.Range + 200f)
                    {
                        CardSelector.StartSelecting(Cards.Red);
                    }
                    break;

                case SelectStatus.Selected:
                {
                    var wName = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.W).Name;
                    var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, TF.W.Range);
                    foreach (var minion in allMinions)
                    {
                        if (wName == "redcardlock" && minion.Distance(target) < 100f)
                        {
                            ObjectManager.Player.IssueOrder(GameObjectOrder.AutoAttack, minion);
                        }
                    }
                }
                    break;
            }
        }

        public static void UseIgnite(Obj_AI_Hero target)
        {
            if (TF.IgniteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell(TF.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= ObjectManager.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                {
                    ObjectManager.Player.Spellbook.CastSpell(TF.IgniteSlot, target);
                }
            }
        }

        public static void UseItems(Obj_AI_Hero target)
        {
            //BOTRK and Cutlass
            if (Items.CanUseItem(3153))
            {
                Items.UseItem(3153, target);
            }
            if (Items.CanUseItem(3144))
            {
                Items.UseItem(3144, target);
            }
            
            //DFG
            if (Items.CanUseItem(3128))
            {
                Items.UseItem(3128, target);
            }

            //Gunblade
            if (Items.CanUseItem(3146))
            {
                Items.UseItem(3146, target);
            }



        }

        public static void UseSmiteOnChamp(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(TF.W.Range) && TF.smiteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell((TF.smiteSlot)) == SpellState.Ready &&
                (TF.GetSmiteType() == "s5_summonersmiteplayerganker" || TF.GetSmiteType() == "s5_summonersmiteduel"))
            {
                ObjectManager.Player.Spellbook.CastSpell(TF.smiteSlot, target);
            }
        }
    }
}