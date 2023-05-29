using HarmonyLib;
using SongSelect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SongSelectCrownIcons.Patches
{
    internal class SongSelectCrownIconsPatch
    {
        private static readonly Dictionary<string, Animator> CrownIcons = new();
        private static readonly Dictionary<string, Animator> UraCrownIcons = new();

        private static readonly Dictionary<EnsoData.EnsoLevelType, Sprite> LevelIcons = new();
        private static readonly Dictionary<EnsoData.EnsoLevelType, Sprite> UraLevelIcons = new();

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch("Start")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void Start_Postfix(SongSelectManager __instance)
        {
            CrownIcons.Clear();
            LevelIcons.Clear();

            UraCrownIcons.Clear();
            UraLevelIcons.Clear();

            for (var i = 0; i < 4; i++)
            {
                LevelIcons.Add((EnsoData.EnsoLevelType)i, __instance.songFilterSetting.difficultyIconSprites[i]);
            }
            UraLevelIcons.Add(EnsoData.EnsoLevelType.Ura, __instance.songFilterSetting.difficultyIconSprites[(int)EnsoData.EnsoLevelType.Ura]);
        }

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch("OnDestroy")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void Destroy_Postfix(SongSelectManager __instance)
        {
            CrownIcons.Clear();
            LevelIcons.Clear();

            UraCrownIcons.Clear();
            UraLevelIcons.Clear();
        }


        private static Transform RecursiveFindChild(Transform parent, string childName)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.name.ToLower().Contains(childName.ToLower()))
                {
                    return child;
                }
                else
                {
                    var found = RecursiveFindChild(child, childName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }

        static float initCrownScale = 0.75f;
        static float selectedCrownScale = 1.20f;

        static bool selected = false;

        [HarmonyPatch(typeof(SongSelectKanban))]
        [HarmonyPatch("UpdateDisplay")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void UpdateDisplay_Postfix(SongSelectKanban __instance, in SongSelectManager.Song song)
        {
            if (!CrownIcons.TryGetValue(__instance.name, out var animator))
            {
                var crownImageObj = __instance.transform.GetChild(0).GetChild(16).GetChild(0).GetChild(2);
                var clonedCrownObj = UnityEngine.Object.Instantiate(crownImageObj.gameObject, __instance.iconFavorite1P.transform, true);

                clonedCrownObj.name = $"CrownIcon for {__instance.name}";

                var diffIconObj = new GameObject($"DiffIcon for {__instance.name}");
                diffIconObj.transform.parent = clonedCrownObj.transform;
                diffIconObj.transform.localPosition = new Vector3(13.3f, -16f, 0);
                diffIconObj.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);

                var diffIconImage = diffIconObj.AddComponent<Image>();
                diffIconImage.sprite = LevelIcons[EnsoData.EnsoLevelType.Mania];
                diffIconImage.gameObject.SetActive(false);

                clonedCrownObj.transform.localPosition = new Vector3(1.48f, -31, 0);
                clonedCrownObj.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);

                var rankImage = clonedCrownObj.GetComponent<Image>();
                animator = clonedCrownObj.GetComponent<Animator>();
                animator.Play("None");

                rankImage.color = Color.white;

                CrownIcons.Add(__instance.name, animator);
            }

            if (!UraCrownIcons.TryGetValue(__instance.name, out var uraAnimator))
            {
                var crownImageObj = __instance.transform.GetChild(0).GetChild(16).GetChild(0).GetChild(2);
                var clonedCrownObj = UnityEngine.Object.Instantiate(crownImageObj.gameObject, __instance.iconFavorite1P.transform, true);

                clonedCrownObj.name = $"CrownIcon for {__instance.name}";

                var diffIconObj = new GameObject($"DiffIcon for {__instance.name}");
                diffIconObj.transform.parent = clonedCrownObj.transform;
                diffIconObj.transform.localPosition = new Vector3(13.3f, -16f, 0);
                diffIconObj.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);

                var diffIconImage = diffIconObj.AddComponent<Image>();
                diffIconImage.sprite = UraLevelIcons[EnsoData.EnsoLevelType.Ura];
                diffIconImage.gameObject.SetActive(false);

                clonedCrownObj.transform.localPosition = new Vector3(1.48f, -61f, 0);
                clonedCrownObj.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);

                var rankImage = clonedCrownObj.GetComponent<Image>();
                uraAnimator = clonedCrownObj.GetComponent<Animator>();
                uraAnimator.Play("None");

                rankImage.color = Color.white;

                UraCrownIcons.Add(__instance.name, uraAnimator);
            }

            var diffIcon = animator.gameObject.transform.GetChild(0).GetComponent<Image>();
            var uraDiffIcon = uraAnimator.gameObject.transform.GetChild(0).GetComponent<Image>();

            if (song.HighScores != null)
            {
                var maxCrown = DataConst.CrownType.None;
                var maxEnsoLevel = EnsoData.EnsoLevelType.Easy;
                for (var i = 0; i < 4; i++)
                {
                    if (song.HighScores[i].crown is DataConst.CrownType.None or DataConst.CrownType.Off)
                    {
                        continue;
                    }

                    maxCrown = song.HighScores[i].crown;
                    maxEnsoLevel = (EnsoData.EnsoLevelType)i;
                }

                if (maxCrown == DataConst.CrownType.None)
                {
                    animator.gameObject.SetActive(true);
                    animator.Play("None");
                    animator.gameObject.SetActive(false);
                    diffIcon.gameObject.SetActive(false);
                }
                else
                {
                    switch (maxCrown)
                    {
                        case DataConst.CrownType.Silver:
                            animator.gameObject.SetActive(true);
                            animator.Play("Silver");
                            break;
                        case DataConst.CrownType.Gold:
                            animator.gameObject.SetActive(true);
                            animator.Play("Gold");
                            break;
                        case DataConst.CrownType.Rainbow:
                            animator.gameObject.SetActive(true);
                            animator.Play("Rainbow");
                            break;
                        default:
                            animator.gameObject.SetActive(true);
                            animator.Play("None");
                            animator.gameObject.SetActive(false);
                            break;
                    }

                    diffIcon.sprite = LevelIcons[maxEnsoLevel];
                    diffIcon.gameObject.SetActive(true);
                }

                if (song.Stars[(int)EnsoData.EnsoLevelType.Ura] != 0)
                {
                    uraDiffIcon.sprite = UraLevelIcons[EnsoData.EnsoLevelType.Ura];
                    uraDiffIcon.gameObject.SetActive(true);

                    switch (song.HighScores[(int)EnsoData.EnsoLevelType.Ura].crown)
                    {
                        case DataConst.CrownType.Silver:
                            uraAnimator.gameObject.SetActive(true);
                            uraAnimator.Play("Silver");
                            break;
                        case DataConst.CrownType.Gold:
                            uraAnimator.gameObject.SetActive(true);
                            uraAnimator.Play("Gold");
                            break;
                        case DataConst.CrownType.Rainbow:
                            uraAnimator.gameObject.SetActive(true);
                            uraAnimator.Play("Rainbow");
                            break;
                        default:
                            uraAnimator.gameObject.SetActive(true);
                            uraAnimator.Play("None");
                            uraAnimator.gameObject.SetActive(false);
                            uraDiffIcon.gameObject.SetActive(false);
                            break;
                    }
                }
                else
                {
                    uraAnimator.gameObject.SetActive(true);
                    uraAnimator.Play("None");
                    uraAnimator.gameObject.SetActive(false);
                    uraDiffIcon.gameObject.SetActive(false);
                }
            }
            else
            {
                animator.Play("None");
                diffIcon.gameObject.SetActive(false);
                uraAnimator.Play("None");
                uraDiffIcon.gameObject.SetActive(false);
            }

            if (__instance.name != "Kanban1")
            {
                if (uraAnimator.gameObject.activeSelf)
                {
                    animator.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!diffIcon.gameObject.activeSelf)
                {
                    animator.gameObject.SetActive(true);
                    animator.Play("None");
                }
                if (song.Stars[(int)EnsoData.EnsoLevelType.Ura] != 0)
                {
                    uraAnimator.gameObject.SetActive(uraDiffIcon.gameObject.activeSelf);
                }
            }

        }

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch("UpdateSongSelect")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void UpdateSongSelect_Postfix(SongSelectManager __instance)
        {
            if (!__instance.isKanbanMoving)
            {
                if (UraCrownIcons.TryGetValue(__instance.kanbans[0].name, out var uraAnimator))
                {
                    uraAnimator.transform.localPosition = new Vector3(1.48f, -176, 0);
                    uraAnimator.transform.localScale = new Vector3(selectedCrownScale, selectedCrownScale, selectedCrownScale);
                }
                if (CrownIcons.TryGetValue(__instance.kanbans[0].name, out var animator))
                {
                    animator.transform.localPosition = new Vector3(1.48f, -106, 0);
                    animator.transform.localScale = new Vector3(selectedCrownScale, selectedCrownScale, selectedCrownScale);
                }

                for (int i = 1; i < __instance.kanbans.Length; i++)
                {
                    if (UraCrownIcons.TryGetValue(__instance.kanbans[i].name, out var otherUraAnimator))
                    {
                        otherUraAnimator.transform.localPosition = new Vector3(1.48f, -35, 0);
                        otherUraAnimator.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);
                    }
                    if (CrownIcons.TryGetValue(__instance.kanbans[i].name, out var otherAnimator))
                    {
                        otherAnimator.transform.localPosition = new Vector3(1.48f, -35, 0);
                        otherAnimator.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);
                    }
                }
            }
            else
            {
                if (__instance.currentKanbanMoveType == SongSelectManager.KanbanMoveType.MoveUp)
                {
                    for (int i = 0; i < __instance.kanbans.Length - 1; i++)
                    {
                        if (UraCrownIcons.TryGetValue(__instance.kanbans[i].name, out var uraAnimator))
                        {
                            uraAnimator.transform.localPosition = new Vector3(1.48f, -35, 0);
                            uraAnimator.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);
                        }
                        if (CrownIcons.TryGetValue(__instance.kanbans[i].name, out var animator))
                        {
                            animator.transform.localPosition = new Vector3(1.48f, -35, 0);
                            animator.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);
                        }
                    }
                }
                else if (__instance.currentKanbanMoveType == SongSelectManager.KanbanMoveType.MoveDown)
                {
                    for (int i = 0; i < __instance.kanbans.Length - 1; i++)
                    {
                        if (UraCrownIcons.TryGetValue(__instance.kanbans[i].name, out var uraAnimator))
                        {
                            uraAnimator.transform.localPosition = new Vector3(1.48f, -35, 0);
                            uraAnimator.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);
                        }
                        if (CrownIcons.TryGetValue(__instance.kanbans[i].name, out var animator))
                        {
                            animator.transform.localPosition = new Vector3(1.48f, -35, 0);
                            animator.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);
                        }
                    }
                }
                else if (__instance.currentKanbanMoveType == SongSelectManager.KanbanMoveType.MoveEnded)
                {
                    for (int i = 0; i < __instance.kanbans.Length; i++)
                    {
                        if (UraCrownIcons.TryGetValue(__instance.kanbans[i].name, out var uraAnimator))
                        {
                            uraAnimator.transform.localPosition = new Vector3(1.48f, -35, 0);
                            uraAnimator.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);
                        }
                        if (CrownIcons.TryGetValue(__instance.kanbans[i].name, out var animator))
                        {
                            animator.transform.localPosition = new Vector3(1.48f, -35, 0);
                            animator.transform.localScale = new Vector3(initCrownScale, initCrownScale, initCrownScale);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(SongSelectManager))]
        [HarmonyPatch(nameof(SongSelectManager.UpdateRandomSelect))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static void UpdateRandomSelect_Prefix(SongSelectManager __instance)
        {
            if (__instance.currentRandomSelectState == SongSelectManager.RandomSelectState.DecideSong)
            {
                UpdateDisplay_Postfix(__instance.kanbans[0], __instance.SongList[__instance.SelectedSongIndex]);
            }
        }
    }
}
