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
        public static bool isLethal(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            double incDmg = 100; 
            if (sender.IsEnemy && args.Target.IsMe)
            {
                 {
                         // Tower Damage
                     if (sender.Type == GameObjectType.obj_AI_Turret)
                     {
                        // foreach (InventorySlot t in sender.InventoryItems)
                        //     Console.WriteLine("Item -> " + t.DisplayName);

                         
                         Console.WriteLine("Damage: " );
                         incDmg = sender.GetAutoAttackDamage(Trynda.Player);
                         // TODO: Get real Damage
                     }

                     // Minion Damage
                     if (sender.Type == GameObjectType.obj_AI_Minion)
                     {
                         incDmg = sender.GetAutoAttackDamage(Trynda.Player);
                     }

                         // Player Damage
                     if (sender.Type == GameObjectType.obj_AI_Hero)
                     {
                         Obj_AI_Hero attackerHero = ObjectManager.Get<Obj_AI_Hero>().First(hero => hero.NetworkId == sender.NetworkId);
                         SpellSlot spellSlot = attackerHero.GetSpellSlot(args.SData.Name);
                         SpellSlot igniteSlot = attackerHero.GetSpellSlot("SummonerDot");
                         if (igniteSlot != SpellSlot.Unknown && spellSlot == igniteSlot)
                             incDmg = attackerHero.GetSummonerSpellDamage(Trynda.Player, Damage.SummonerSpell.Ignite);
                         else if (spellSlot == SpellSlot.Item1 || spellSlot == SpellSlot.Item2 ||
                                  spellSlot == SpellSlot.Item3 || spellSlot == SpellSlot.Item4 ||
                                  spellSlot == SpellSlot.Item5 || spellSlot == SpellSlot.Item6)
                         {
                             incDmg = 200;
                             if (args.SData.Name.Contains("Bilgewater"))
                                 incDmg = Damage.GetItemDamage(
                                     attackerHero, Trynda.Player, Damage.DamageItems.Bilgewater);
                             if (args.SData.Name.Contains("Ruined"))
                                 incDmg = Damage.GetItemDamage(
                                     attackerHero, Trynda.Player, Damage.DamageItems.Botrk);
                             if (args.SData.Name.Contains("Deathfire"))
                                 incDmg = Damage.GetItemDamage(
                                     attackerHero, Trynda.Player, Damage.DamageItems.Dfg);
                             if (args.SData.Name.Contains("Hextech"))
                                 incDmg = Damage.GetItemDamage(
                                     attackerHero, Trynda.Player, Damage.DamageItems.Hexgun);
                             if (args.SData.Name.Contains("Hydra"))
                                 incDmg = Damage.GetItemDamage(
                                     attackerHero, Trynda.Player, Damage.DamageItems.Hydra);
                             if (args.SData.Name.Contains("Tiamat"))
                                 incDmg = Damage.GetItemDamage(attackerHero, Trynda.Player, Damage.DamageItems.Tiamat);

                         }
                         else if (spellSlot == SpellSlot.Unknown)
                             incDmg = attackerHero.GetAutoAttackDamage(Trynda.Player);
                         else
                             incDmg = attackerHero.GetSpellDamage(Trynda.Player, spellSlot);

                     }

                 }
            }
            return Trynda.Player.Health <= incDmg;
        }

        //public static float GetTowerDamage(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        //{
        //    int i = 0;
        //    bool isOuter = false, isInhib = true;
        //    foreach (InventorySlot t in sender.InventoryItems)
        //    {
                
        //        if (t.DisplayName == "Penetrating Bullets")
        //        {
        //            isOuter = true;
        //            isInhib = false;
        //            break;
        //        }
        //        if (t.DisplayName == "Lightning Rod")
        //        {
        //            isOuter = false;
        //            isInhib = true;
        //            break;
        //        }
        //    }
        //    return 0f;
        //}
    }


}