//#nullable enable
//using GameNetcodeStuff;
//using HarmonyLib;

//namespace NightVision.Patches
//{

//    [HarmonyPatch(typeof(PlayerControllerB))]
//    internal class Player
//    {
//        private static PlayerControllerB? _playerController;

//        [HarmonyPatch("Awake")]
//        [HarmonyPostfix]
//        public static PlayerControllerB? LocalPlayer()
//        {
//            return _playerController;
//        }

//        [HarmonyPatch("Update")]
//        [HarmonyPostfix]
//        public static void Update()
//        {
//            if (GameNetworkManager.Instance == null)
//                return;
//            _playerController = GameNetworkManager.Instance.localPlayerController;
//        }

//    }
//}
