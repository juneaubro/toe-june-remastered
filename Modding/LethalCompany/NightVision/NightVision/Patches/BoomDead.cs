using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace NightVision.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class BoomDead
    {
        static PlayerControllerB plr;
        public static ModHotkey dead = new ModHotkey(MouseAndKeyboard.MouseMiddle, KillHimNow,true);
        public static bool deadPressed = false;

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void Update(PlayerControllerB __instance)
        {
            plr = __instance;
            dead.Update();
            if (deadPressed)
            {
                RaycastHit hit;
                Vector3 ori = new Vector3(plr.transform.position.x, plr.transform.position.y, plr.transform.position.z);
                Vector3 oric = new Vector3(plr.gameplayCamera.transform.position.x, plr.gameplayCamera.transform.position.y, plr.gameplayCamera.transform.position.z);
                if (Physics.Raycast(oric+__instance.transform.forward*1.1f, plr.gameplayCamera.transform.forward, out hit, float.MaxValue))
                {
                    UnityEngine.Debug.Log(hit.transform.gameObject.name);
                    if (hit.transform.gameObject.GetComponent<PlayerControllerB>() != null)
                    {
                        UnityEngine.Debug.Log("PlayerControllerB not null");
                        hit.transform.gameObject.GetComponent<PlayerControllerB>().DamagePlayer(2222);
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<SandSpiderAI>() != null)
                    {
                        UnityEngine.Debug.Log("SandSpiderAI not null");
                        hit.transform.gameObject.GetComponentInParent<SandSpiderAI>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<SandWormAI>() != null)
                    {
                        UnityEngine.Debug.Log("SandWormAI not null");
                        hit.transform.gameObject.GetComponentInParent<SandWormAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<SandWormAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<BlobAI>() != null)
                    {
                        UnityEngine.Debug.Log("BlobAI not null");
                        hit.transform.gameObject.GetComponentInParent<BlobAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<BlobAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<CentipedeAI>() != null)
                    {
                        UnityEngine.Debug.Log("CentipedeAI not null");
                        hit.transform.gameObject.GetComponentInParent<CentipedeAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<CentipedeAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<CrawlerAI>() != null)
                    {
                        UnityEngine.Debug.Log("CrawlerAI not null");
                        hit.transform.gameObject.GetComponentInParent<CrawlerAI>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<RedLocustBees>() != null)
                    {
                        UnityEngine.Debug.Log("RedLocustBees not null");
                        hit.transform.gameObject.GetComponentInParent<RedLocustBees>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<RedLocustBees>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<DressGirlAI>() != null)
                    {
                        UnityEngine.Debug.Log("DressGirlAI not null");
                        hit.transform.gameObject.GetComponentInParent<DressGirlAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<DressGirlAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<FlowermanAI>() != null)
                    {
                        UnityEngine.Debug.Log("FlowermanAI not null");
                        hit.transform.gameObject.GetComponentInParent<FlowermanAI>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<ForestGiantAI>() != null)
                    {
                        UnityEngine.Debug.Log("ForestGiantAI not null");
                        hit.transform.gameObject.GetComponentInParent<ForestGiantAI>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<HoarderBugAI>() != null)
                    {
                        UnityEngine.Debug.Log("HoarderBugAI not null");
                        hit.transform.gameObject.GetComponentInParent<HoarderBugAI>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<MouthDogAI>() != null)
                    {
                        UnityEngine.Debug.Log("MouthDogAI not null");
                        hit.transform.gameObject.GetComponentInParent<MouthDogAI>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<SpringManAI>() != null)
                    {
                        UnityEngine.Debug.Log("SpringManAI not null");
                        hit.transform.gameObject.GetComponentInParent<SpringManAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<SpringManAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<PufferAI>() != null)
                    {
                        UnityEngine.Debug.Log("PufferAI not null");
                        hit.transform.gameObject.GetComponentInParent<PufferAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<PufferAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<NutcrackerEnemyAI>() != null)
                    {
                        UnityEngine.Debug.Log("NutcrackerEnemyAI not null");
                        hit.transform.gameObject.GetComponentInParent<NutcrackerEnemyAI>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<MaskedPlayerEnemy>() != null)
                    {
                        UnityEngine.Debug.Log("EnemMaskedPlayerEnemyyAI not null");
                        hit.transform.gameObject.GetComponentInParent<MaskedPlayerEnemy>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<LassoManAI>() != null)
                    {
                        UnityEngine.Debug.Log("LassoManAI not null");
                        hit.transform.gameObject.GetComponentInParent<LassoManAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<LassoManAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<JesterAI>() != null)
                    {
                        UnityEngine.Debug.Log("JesterAI not null");
                        hit.transform.gameObject.GetComponentInParent<JesterAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<JesterAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<DoublewingAI>() != null)
                    {
                        UnityEngine.Debug.Log("DoublewingAI not null");
                        hit.transform.gameObject.GetComponentInParent<DoublewingAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<DoublewingAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<DocileLocustBeesAI>() != null)
                    {
                        UnityEngine.Debug.Log("DocileLocustBeesAI not null");
                        hit.transform.gameObject.GetComponentInParent<DocileLocustBeesAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<DocileLocustBeesAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<BaboonBirdAI>() != null)
                    {
                        UnityEngine.Debug.Log("BaboonBirdAI not null");
                        hit.transform.gameObject.GetComponentInParent<BaboonBirdAI>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<ButlerBeesEnemyAI>() != null)
                    {
                        UnityEngine.Debug.Log("ButlerBeesEnemyAI not null");
                        hit.transform.gameObject.GetComponentInParent<ButlerBeesEnemyAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<ButlerBeesEnemyAI>().isEnemyDead = true;
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<ButlerEnemyAI>() != null)
                    {
                        UnityEngine.Debug.Log("ButlerEnemyAI not null");
                        hit.transform.gameObject.GetComponentInParent<ButlerEnemyAI>().KillEnemyOnOwnerClient();
                    }
                    else if (hit.transform.gameObject.GetComponentInParent<RadMechAI>() != null)
                    {
                        UnityEngine.Debug.Log("RadMechAI not null");
                        hit.transform.gameObject.GetComponentInParent<RadMechAI>().KillEnemyOnOwnerClient();
                        hit.transform.gameObject.GetComponentInParent<RadMechAI>().isEnemyDead = true;
                    }
                }
            }
        }

        static void KillHimNow()
        {
            deadPressed = !deadPressed;
        }
    }
}
