using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Kayle
{
    internal class IncomingDamage
    {
        public static int WarmingUpStacks = 0, HeatedUpStacks = 0, HeatStacks = 0;
        public static bool StackResetDelay = false;

        public static bool MinionIsLethal(Obj_AI_Base sender, Obj_AI_Base target, GameObjectProcessSpellCastEventArgs args)
        {
            return target.Health <= sender.CalcDamage(target, Damage.DamageType.Physical, sender.BaseAttackDamage);
        }

        public static bool TowerIsLethal(Obj_AI_Base sender, Obj_AI_Base target, GameObjectProcessSpellCastEventArgs args)
        {
            return target.Health <= GetTowerDamage(sender, args);
        }

        public static bool TargetedHeroIsLethal(Obj_AI_Base sender, Obj_AI_Base target, GameObjectProcessSpellCastEventArgs args)
        {
            return target.Health <= GetTargetedHeroDamage(sender, target, args);
        }

        public static bool SkillshotHeroIsLethal(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            double incDmg = 0;
            var attackerHero = (Obj_AI_Hero)sender;
            SpellSlot spellSlot = attackerHero.GetSpellSlot(args.SData.Name);

            //if (SkillshotDetector.IsAboutToHit(ObjectManager.Player, 150))
            //{
            //    incDmg = attackerHero.GetSpellDamage(ObjectManager.Player, spellSlot);

            //}
            return ObjectManager.Player.Health <= incDmg;
        }

        private static bool TowerIsOuter(Obj_AI_Base sender)
        {
            return sender.InventoryItems.Any(t => t.DisplayName == "Penetrating Bullets");
        }

        private static bool TowerIsInhib(Obj_AI_Base sender)
        {
            return sender.InventoryItems.Any(t => t.DisplayName == "Lightning Rod");
        }

        private static double GetTargetedHeroDamage(Obj_AI_Base sender, Obj_AI_Base target, GameObjectProcessSpellCastEventArgs args)
        {
            double incDmg;
            var attackerHero = (Obj_AI_Hero)sender;
            SpellSlot spellSlot = attackerHero.GetSpellSlot(args.SData.Name);
            SpellSlot igniteSlot = attackerHero.GetSpellSlot("SummonerDot");

            if (igniteSlot != SpellSlot.Unknown && spellSlot == igniteSlot)
                incDmg = attackerHero.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            else if (spellSlot == SpellSlot.Item1 || spellSlot == SpellSlot.Item2 ||
                     spellSlot == SpellSlot.Item3 || spellSlot == SpellSlot.Item4 ||
                     spellSlot == SpellSlot.Item5 || spellSlot == SpellSlot.Item6)
            {
                incDmg = 200f;
                if (args.SData.Name.ToLower().Contains("bilgewater"))
                    incDmg = attackerHero.GetItemDamage(target, Damage.DamageItems.Bilgewater);
                if (args.SData.Name.ToLower().Contains("ruined"))
                    incDmg = attackerHero.GetItemDamage(target, Damage.DamageItems.Botrk);
                if (args.SData.Name.ToLower().Contains("deathfire"))
                    incDmg = attackerHero.GetItemDamage(target, Damage.DamageItems.Dfg);
                if (args.SData.Name.ToLower().Contains("hextech"))
                    incDmg = attackerHero.GetItemDamage(target, Damage.DamageItems.Hexgun);
                if (args.SData.Name.ToLower().Contains("hydra"))
                    incDmg = attackerHero.GetItemDamage(target, Damage.DamageItems.Hydra);
                if (args.SData.Name.ToLower().Contains("tiamat"))
                    incDmg = attackerHero.GetItemDamage(target, Damage.DamageItems.Tiamat);
            }
            else if (spellSlot == SpellSlot.Unknown)
                incDmg = attackerHero.GetAutoAttackDamage(target);
            else
                incDmg = attackerHero.GetSpellDamage(target, spellSlot);

            return incDmg;
        }

        private static double GetTowerDamage(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var towerDamage = sender.CalcDamage((Obj_AI_Base)args.Target, Damage.DamageType.Physical, sender.BaseAttackDamage);
            if (TowerIsOuter(sender))
            {
                towerDamage = towerDamage * (1 + 0.375f * WarmingUpStacks + 0.25f * HeatedUpStacks);
            }
            else if (TowerIsInhib(sender))
            {
                towerDamage = towerDamage * (1 + 0.0105f * HeatStacks);
            }
            return towerDamage;
        }

        private static void ResetTowerStacks()
        {
            HeatStacks = 0;
            HeatedUpStacks = 0;
            WarmingUpStacks = 0;
        }

        private static void ResetTowerWarming()
        {
            WarmingUpStacks = 0;
            HeatStacks = 0;
        }

        public static void ChargeOnTowerSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (ObjectManager.Player.IsDead || ObjectManager.Player.InFountain())
            {
                return;
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.obj_AI_Turret &&
                sender.Distance(ObjectManager.Player) < 2000f)
            {

                if (args.Target.IsMe)
                {
                    if (TowerIsOuter(sender))
                    {
                        if (WarmingUpStacks < 2)
                        {
                            WarmingUpStacks++;
                            //Console.WriteLine("Warming: " + WarmingUpStacks);
                        }
                        else if (HeatedUpStacks < 2)
                        {
                            HeatedUpStacks++;
                            //Console.WriteLine("Heated: " + HeatedUpStacks);
                        }
                    }
                    if (TowerIsInhib(sender))
                    {
                        if (HeatStacks < 120)
                        {
                            HeatStacks = HeatStacks + 6;
                            //Console.WriteLine("Heat: " + HeatStacks);
                        }
                    }

                }
                else if (args.Target.IsAlly && args.Target.Type == GameObjectType.obj_AI_Hero)
                {
                    ResetTowerWarming();
                }
                else
                {
                    ResetTowerStacks();
                }

            }
        }
    }


}
