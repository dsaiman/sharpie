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
    internal class IncomingDamage
    {
        public static int WarmingUpStacks = 0, HeatedUpStacks = 0, HeatStacks = 0;
        public static bool StackResetDelay = false;

        public static bool MinionIsLethal(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //Console.WriteLine("Damage from Minion: " + sender.GetAutoAttackDamage(ObjectManager.Player));
            return ObjectManager.Player.Health <= sender.GetAutoAttackDamage(ObjectManager.Player);
        }

        public static bool TowerIsLethal(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //Console.WriteLine("Damage from Tower: " + GetTowerDamage(sender));
            return ObjectManager.Player.Health <= GetTowerDamage(sender);
        }

        public static bool HeroIsLethal(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            double incDmg;
            var attackerHero = (Obj_AI_Hero) sender;
            SpellSlot spellSlot = attackerHero.GetSpellSlot(args.SData.Name);
            SpellSlot igniteSlot = attackerHero.GetSpellSlot("SummonerDot");

            if (igniteSlot != SpellSlot.Unknown && spellSlot == igniteSlot)
                incDmg = attackerHero.GetSummonerSpellDamage(ObjectManager.Player, Damage.SummonerSpell.Ignite);
            else if (spellSlot == SpellSlot.Item1 || spellSlot == SpellSlot.Item2 ||
                     spellSlot == SpellSlot.Item3 || spellSlot == SpellSlot.Item4 ||
                     spellSlot == SpellSlot.Item5 || spellSlot == SpellSlot.Item6)
            {
                incDmg = 200;
                if (args.SData.Name.Contains("Bilgewater"))
                    incDmg = attackerHero.GetItemDamage(ObjectManager.Player, Damage.DamageItems.Bilgewater);
                if (args.SData.Name.Contains("Ruined"))
                    incDmg = attackerHero.GetItemDamage(ObjectManager.Player, Damage.DamageItems.Botrk);
                if (args.SData.Name.Contains("Deathfire"))
                    incDmg = attackerHero.GetItemDamage(ObjectManager.Player, Damage.DamageItems.Dfg);
                if (args.SData.Name.Contains("Hextech"))
                    incDmg = attackerHero.GetItemDamage(ObjectManager.Player, Damage.DamageItems.Hexgun);
                if (args.SData.Name.Contains("Hydra"))
                    incDmg = attackerHero.GetItemDamage(ObjectManager.Player, Damage.DamageItems.Hydra);
                if (args.SData.Name.Contains("Tiamat"))
                    incDmg = attackerHero.GetItemDamage(ObjectManager.Player, Damage.DamageItems.Tiamat);
            }
            else if (spellSlot == SpellSlot.Unknown)
                incDmg = attackerHero.GetAutoAttackDamage(ObjectManager.Player);
            else
                incDmg = attackerHero.GetSpellDamage(ObjectManager.Player, spellSlot);
            Console.WriteLine("Damage from : " + args.SData.Name + " amount: " + incDmg);
            return ObjectManager.Player.Health <= incDmg;
        }

        public static bool TowerIsOuter(Obj_AI_Base sender)
        {
            return sender.InventoryItems.Any(t => t.DisplayName == "Penetrating Bullets");
        }

        public static bool TowerIsInhib(Obj_AI_Base sender)
        {
            return sender.InventoryItems.Any(t => t.DisplayName == "Lightning Rod");
        }

        public static double GetTowerDamage(Obj_AI_Base sender)
        {
            var towerDamage = sender.CalcDamage(ObjectManager.Player, Damage.DamageType.Physical, sender.BaseAttackDamage);
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

        public static void ResetTowerStacks()
        {
            HeatStacks = 0;
            HeatedUpStacks = 0;
            WarmingUpStacks = 0;
        }

        public static void ResetTowerWarming()
        {
            WarmingUpStacks = 0;
            HeatStacks = 0;
        }

    }


}