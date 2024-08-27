using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using On.RoR2;
using RoR2;
using System;
using UnityEngine;

namespace Jimmy.AcridDamageMod
{
    [HarmonyPatch(typeof(HealthComponent))]
    public class AcridPoisonOnHitEffects : BaseUnityPlugin
    {
        private void TriggerOnHitEffectsForAcridPoisons(HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.dotIndex != 4)
                return;
            damageInfo.procCoefficient = (__Null)(double)AcridPoisonOnHitEffects.ProcCoefficient.get_Value();
            ((GlobalEventManager)GlobalEventManager.instance).OnHitEnemy(damageInfo, ((Component)self).gameObject);
        }

        private void CalculateCritForAcridsPoisons(HealthComponent self, DamageInfo damageInfo)
        {
            CharacterBody component = ((GameObject)damageInfo.attacker).GetComponent<CharacterBody>();
            damageInfo.crit = (__Null)(Util.CheckRoll(component.get_crit(), component.get_master()) ? 1 : 0);
        }

        private void HealthComponent_TakeDamage(
          HealthComponent.orig_TakeDamage orig,
          HealthComponent self,
          DamageInfo damageInfo)
        {
            if ((Object)damageInfo.attacker != (Object)null && ((Object)damageInfo.attacker).name == "CrocoBody(Clone)")
            {
                if (AcridPoisonOnHitEffects.EnablePoisonCriticalStrikes.get_Value())
                    this.CalculateCritForAcridsPoisons(self, damageInfo);
                if (AcridPoisonOnHitEffects.EnableOnHitEffectsForPoisons.get_Value())
                    this.TriggerOnHitEffectsForAcridPoisons(self, damageInfo);
            }
            orig.Invoke(self, damageInfo);
        }

        public AcridPoisonOnHitEffects() => base.\u002Ector();
    }
}