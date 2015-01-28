using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Jarvan4
{
    class OutgoingDamage
    {
        public static bool PassiveUp;

        public static float ComboDamage(Obj_AI_Hero target)
        {
            var dmg = 0d;
            //Passive damage and at least a couple of autos
            if (PassiveUp)
            {
                dmg += Math.Min(0.1d * target.Health, 400d);
            }
            dmg += J.Player.GetAutoAttackDamage(target) * 2;
            //R means more AA!
            if (J.Spells[SpellSlot.R].IsReady())
            {
                dmg += J.Player.GetSpellDamage(target, SpellSlot.R);
                dmg += J.Player.GetAutoAttackDamage(target) * 2;
            }
            // Q and E damage
            if (J.Spells[SpellSlot.E].IsReady())
            {
                dmg += J.Player.GetSpellDamage(target, SpellSlot.E);
            }
            if (J.Spells[SpellSlot.Q].IsReady())
            {
                dmg += J.Player.GetSpellDamage(target, SpellSlot.Q);
            }
            //Smite and Ignite
            if (J.IgniteSlot != SpellSlot.Unknown && J.Player.Spellbook.CanUseSpell(J.IgniteSlot) == SpellState.Ready)
            {
                dmg += J.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
            }
            if (J.SmiteSlot != SpellSlot.Unknown && J.Player.Spellbook.CanUseSpell(J.SmiteSlot) == SpellState.Ready &&
                (J.GetSmiteType() == "s5_summonersmiteplayerganker" ||
                 J.GetSmiteType() == "s5_summonersmiteduel"))
            {
                dmg += J.Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Smite);
            }
            //Items
            if (Items.CanUseItem(3074))
            {
                dmg += J.Player.GetItemDamage(target, Damage.DamageItems.Hydra);
            }
            if (Items.CanUseItem(3077))
            {
                dmg += J.Player.GetItemDamage(target, Damage.DamageItems.Tiamat);
            }
            if (Items.CanUseItem(3153))
            {
                dmg += J.Player.GetItemDamage(target, Damage.DamageItems.Bilgewater);
            }
            if (Items.CanUseItem(3144))
            {
                dmg += J.Player.GetItemDamage(target, Damage.DamageItems.Botrk);
            }
            
            return (float) Math.Round(dmg);
        }

    }
}
