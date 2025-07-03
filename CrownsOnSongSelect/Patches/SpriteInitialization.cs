using SongSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CrownsOnSongSelect.Patches
{
    internal class SpriteInitialization
    {
        public static Dictionary<DataConst.CrownType, Sprite> CrownSprites = new Dictionary<DataConst.CrownType, Sprite>();
        public static Dictionary<EnsoData.EnsoLevelType, Sprite> DifficultySprites = new Dictionary<EnsoData.EnsoLevelType, Sprite>();

        public static bool IsInitialized()
        {
            ClearNullsFromCrownDictionary();
            ClearNullsFromDiffDictionary();

            // This is a pretty bad check I think
            if (CrownSprites.Count > 0 &&
                DifficultySprites.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static void InitializeCrownSprites(SongSelectScoreDisplay scoreDisplay)
        {
            ModLogger.Log("InitializeCrownSprites");
            ClearNullsFromCrownDictionary();

            if (scoreDisplay == null || scoreDisplay.playerIndex != SongSelectScoreDisplay.PlayerIndex.Player1)
            {
                return;
            }

            if (CrownSprites.ContainsKey(DataConst.CrownType.Silver) &&
                CrownSprites.ContainsKey(DataConst.CrownType.Gold) &&
                CrownSprites.ContainsKey(DataConst.CrownType.Rainbow))
            {
                return;
            }

            var images = scoreDisplay.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                if (images[i] is null)
                {
                    continue;
                }
                var name = images[i].gameObject.name;
                if (name.StartsWith("IconCrown"))
                {
                    if (name == "IconCrownSilver")
                    {
                        if (!CrownSprites.ContainsKey(DataConst.CrownType.Silver))
                        {
                            CrownSprites.Add(DataConst.CrownType.Silver, images[i].sprite);
                        }
                    }
                    else if (name == "IconCrownGold")
                    {
                        if (!CrownSprites.ContainsKey(DataConst.CrownType.Gold))
                        {
                            CrownSprites.Add(DataConst.CrownType.Gold, images[i].sprite);
                        }
                    }
                    else if (name == "IconCrownRainbow")
                    {
                        if (!CrownSprites.ContainsKey(DataConst.CrownType.Rainbow))
                        {
                            CrownSprites.Add(DataConst.CrownType.Rainbow, images[i].sprite);
                        }
                    }
                }
            }
        }

        public static void InitializeDifficultySprites(SongSelectManager songSelectManager)
        {
            ModLogger.Log("InitializeDifficultySprites");
            InitializeDifficultySprites(songSelectManager.songFilterSetting);
        }

        public static void InitializeDifficultySprites(SongFilterSetting songFilterSetting)
        {
            ClearNullsFromDiffDictionary();

            if (songFilterSetting is null)
            {
                return;
            }

            // Check if everything's already initialized
            if (DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Easy) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Normal) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Hard) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Mania) &&
                DifficultySprites.ContainsKey(EnsoData.EnsoLevelType.Ura))
            {
                return;
            }

            for (EnsoData.EnsoLevelType i = 0; i < EnsoData.EnsoLevelType.Num; i++)
            {
                DifficultySprites.Add(i, songFilterSetting.difficultyIconSprites[(int)i]);
            }
        }

        // I don't know if this could be bad for performance
        // This is mainly used for storing sprites between scenes, and this mod only cares about the one scene
        //static List<GameObject> SaveObjectHolder = new List<GameObject>();
        //static void DontDestroySprite<T>(Dictionary<T, Sprite> dictionary, T key, Sprite sprite)
        //{
        //    if (!dictionary.ContainsKey(key))
        //    {
        //        dictionary.Add(key, sprite);
        //        GameObject obj = new GameObject("SaveSprite");
        //        var image = obj.AddComponent<Image>();
        //        image.sprite = sprite;
        //        UnityEngine.Object.DontDestroyOnLoad(obj);
        //    }


        //    if (!CrownSprites.ContainsKey(crown))
        //    {
        //        CrownSprites.Add(crown, sprite);
        //        GameObject obj = new GameObject("SaveSprite");
        //        var image = obj.AddComponent<Image>();
        //        image.sprite = sprite;
        //        UnityEngine.Object.DontDestroyOnLoad(obj);
        //        CrownSpriteGameObjects.Add(crown, obj);
        //        Plugin.Log.LogInfo("Sprite added for crown: " + crown.ToString());
        //    }
        //}

        private static void ClearNullsFromCrownDictionary()
        {
            foreach (var item in CrownSprites)
            {
                if (item.Value == null)
                {
                    CrownSprites.Remove(item.Key);
                }
            }
        }

        private static void ClearNullsFromDiffDictionary()
        {
            foreach (var item in DifficultySprites)
            {
                if (item.Value == null)
                {
                    DifficultySprites.Remove(item.Key);
                }
            }
        }
    }
}
