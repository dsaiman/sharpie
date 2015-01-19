using LeagueSharp;
using LeagueSharp.Common;

namespace Kayle
{
    internal class Use
    {
        public static void UseIgnite(Obj_AI_Hero target)
        {
            if (K.IgniteSlot != SpellSlot.Unknown && K.Player.Spellbook.CanUseSpell(K.IgniteSlot) == SpellState.Ready)
            {
                if (target.Health <= K.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite))
                {
                    K.Player.Spellbook.CastSpell(K.IgniteSlot, target);
                }
            }
        }

        public static void UseSmiteOnChamp(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(K.E.Range) && K.smiteSlot != SpellSlot.Unknown &&
                ObjectManager.Player.Spellbook.CanUseSpell((K.smiteSlot)) == SpellState.Ready &&
                (K.GetSmiteType() == "s5_summonersmiteplayerganker" || K.GetSmiteType() == "s5_summonersmiteduel"))
            {
                ObjectManager.Player.Spellbook.CastSpell(K.smiteSlot, target);
            }
        }

        public static void UseWSustain(Obj_AI_Hero target)
        {
            if ((ObjectManager.Player.MaxHealth - ObjectManager.Player.Health < K.W.GetDamage(target) &&
                 (ObjectManager.Player.CountEnemysInRange(2000f) < 1) ||
                ObjectManager.Player.HealthPercentage() < 40) && ObjectManager.Player.ManaPercentage() > 50 )
            {
                K.W.Cast(target);
            }
        }

        public static void UseRManual(Obj_AI_Hero target)
        {
            if (!K.R.IsReady() || K.Player.HasBuff("Recall") || ObjectManager.Player.InFountain())
            {
                return;
            }

            if ((target.HealthPercentage() <= KMenu.Config.Item("RonHp").GetValue<Slider>().Value) &&
                (target.CountEnemysInRange(900f) > 0))
            {
                K.R.Cast();
            }
        }

        public static void UseQCombo(Obj_AI_Hero target)
        {
            if (OutgoingDamage.IsEscaping(target) ||
                (OutgoingDamage.SheenProcable() && target.Distance(ObjectManager.Player) <= K.E.Range) || 
                K.Q.GetDamage(target) <= target.Health && ObjectManager.Player.Mana > 90f)
            {
                K.Q.Cast(target);
            }
        }

        public static void UseWCombo(Obj_AI_Hero target)
        {
            if (OutgoingDamage.IsEscaping(target) ||
                (OutgoingDamage.SheenProcable() && target.Distance(ObjectManager.Player) <= K.E.Range)
                && ObjectManager.Player.Mana > 90f)
            {
                K.W.Cast(ObjectManager.Player);
            }
        }

        public static void UseEMixed(Obj_AI_Hero target)
        {
            var allMinions = MinionManager.GetMinions(target.Position, 150f);
            if (allMinions.Count > 0)
            {
                foreach (var minion in allMinions)
                {
                    if (minion.Distance(ObjectManager.Player) <= K.E.Range)
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
            }

        }

        public static void UseELaneClear()
        {
            var allMinions = MinionManager.GetMinions(ObjectManager.Player.Position, K.E.Range);
            foreach (var minion in allMinions)
            {
                var minionAround = MinionManager.GetMinions(minion.Position, 150f);
                if (minionAround.Count > 1)
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AutoAttack, minion);
            }
        }

        public static void UseComboItems(Obj_AI_Hero target)
        {
            //BOTRK and Cutlass
            if (OutgoingDamage.IsEscaping(target) ||
                ObjectManager.Player.HealthPercentage() < 40)
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

            //DFG
            if (Items.CanUseItem(3128))
            {
                Items.UseItem(3128, target);
            }

            //Gunblade
            if (OutgoingDamage.IsEscaping(target) && Items.CanUseItem(3146))
            {
                Items.UseItem(3146, target);
            }

        }



    }
}