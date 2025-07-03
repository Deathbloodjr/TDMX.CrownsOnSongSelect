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
    internal class ButtonCrownObject
    {
        public Dictionary<CrownId, GameObject> CrownGameObjects = new Dictionary<CrownId, GameObject>();
        bool isSelected;

        SongSelectKanban Parent;
        SongSelectManager.Song Song => Parent.displayingSong;

        public ButtonCrownObject(SongSelectKanban parent)
        {
            Parent = parent;
            isSelected = parent.name == "Kanban1";
            InitializeCrownGameObjects(parent.gameObject);
        }


        public bool IsInitialized()
        {
            foreach (var gameObjects in CrownGameObjects)
            {
                if (gameObjects.Value == null)
                {
                    return false;
                }
            }

            return true;
        }

        void InitializeCrownGameObjects(GameObject parent)
        {
            GameObject crownParent = new GameObject("Crown");
            crownParent.transform.SetParent(parent.transform);
            crownParent.transform.localPosition = Vector2.zero;
            InitializeCrownGameObject(CrownId.P1Oni, crownParent);
            InitializeCrownGameObject(CrownId.P1Ura, crownParent);
            InitializeCrownGameObject(CrownId.P2Oni, crownParent);
            InitializeCrownGameObject(CrownId.P2Ura, crownParent);
        }

        void InitializeCrownGameObject(CrownId crownId, GameObject parent)
        {
            GameObject crownObj = new GameObject(crownId.ToString());
            crownObj.transform.SetParent(parent.transform);
            CrownPosition pos = PlayerCrownPositions.GetCrownPosition(crownId, isSelected);
            crownObj.transform.localPosition = pos.Position;
            crownObj.transform.localScale = pos.Scale;

            GameObject crown = new GameObject("Crown");
            crown.transform.SetParent(crownObj.transform);
            var crownImage = crown.AddComponent<Image>();
            crownImage.raycastTarget = false;
            var crownRect = crown.GetComponent<RectTransform>();
            crownRect.localPosition = new Vector2(0, 0);
            crownRect.localScale = new Vector2(1f, 1f);

            GameObject diffObj = new GameObject("Difficulty");
            diffObj.transform.SetParent(crownObj.transform);
            var diffImage = diffObj.AddComponent<Image>();
            diffImage.raycastTarget = false;
            var diffRect = diffObj.GetComponent<RectTransform>();
            diffRect.localPosition = new Vector2(10, -10);
            diffRect.localScale = new Vector2(0.7f, 0.7f);

            crownObj.SetActive(false);

            CrownGameObjects.Add(crownId, crownObj);
        }

        internal IEnumerator ChangeCrowns()
        {
            //Logger.Log("musicInfo.Id:" + musicInfo.Id);
            while (!SpriteInitialization.IsInitialized())
            {
                yield return new WaitForEndOfFrame();
            }

            // I need to test this when I have a controller available
            var numPlayers = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.playerNum;
            //Logger.Log(numPlayers.ToString(), LogType.Debug);

            for (int i = 0; i < 1; i++)
            {
                var scores = (i == 0 ? Song.HighScores : Song.HighScores2P);
                if (Song.Stars.Length >= (int)EnsoData.EnsoLevelType.Ura &&
                    Song.Stars[(int)EnsoData.EnsoLevelType.Ura] == 0)
                {
                    ChangeCrown(new CrownData(i, DataConst.CrownType.Off, EnsoData.EnsoLevelType.Ura));
                }
                else
                {
                    ChangeCrown(new CrownData(i, scores[(int)EnsoData.EnsoLevelType.Ura].crown, EnsoData.EnsoLevelType.Ura));
                }

                bool crownFound = false;
                for (EnsoData.EnsoLevelType j = EnsoData.EnsoLevelType.Mania; j >= EnsoData.EnsoLevelType.Easy; j--)
                {
                    if (scores[(int)j].crown != DataConst.CrownType.None)
                    {
                        crownFound = true;
                        ChangeCrown(new CrownData(i, scores[(int)j].crown, j));
                        break;
                    }
                }

                if (!crownFound)
                {
                    ChangeCrown(new CrownData(i, DataConst.CrownType.None, EnsoData.EnsoLevelType.Num));
                }
            }
        }

        void ChangeCrown(CrownData data)
        {
            var crownId = data.CrownId;
            if (data.Crown == DataConst.CrownType.Off ||
                data.Crown == DataConst.CrownType.None)
            {
                //if (CrownGameObjects.ContainsKey(crownId))
                {
                    CrownGameObjects[crownId].SetActive(false);
                }
                return;
            }

            CrownGameObjects[crownId].SetActive(true);

            var crownObj = CrownGameObjects[crownId].transform.FindChild("Crown");
            if (SpriteInitialization.CrownSprites.ContainsKey(data.Crown) &&
                SpriteInitialization.CrownSprites[data.Crown] != null)
            {
                var crownImage = crownObj.GetComponent<Image>();
                crownImage.sprite = SpriteInitialization.CrownSprites[data.Crown];
                // I'm not sure if we need to set the sizeDelta each time
                var crownRect = crownObj.GetComponent<RectTransform>();
                crownRect.sizeDelta = new Vector2(crownImage.sprite.rect.width, crownImage.sprite.rect.height);
            }

            var diffObj = CrownGameObjects[crownId].transform.FindChild("Difficulty");
            if (data.Level == EnsoData.EnsoLevelType.Num || data.Crown == DataConst.CrownType.None)
            {
                diffObj.gameObject.SetActive(false);
            }
            else if (SpriteInitialization.DifficultySprites.ContainsKey(data.Level) &&
                SpriteInitialization.DifficultySprites[data.Level] != null)
            {
                diffObj.gameObject.SetActive(true);
                var crownImage = diffObj.GetComponent<Image>();
                crownImage.sprite = SpriteInitialization.DifficultySprites[data.Level];
                // I'm not sure if we need to set the sizeDelta each time
                var crownRect = diffObj.GetComponent<RectTransform>();
                crownRect.sizeDelta = new Vector2(crownImage.sprite.rect.width, crownImage.sprite.rect.height);
            }
        }


        public void ExpandCrowns()
        {
            Plugin.Instance.StartCoroutine(MoveCrown(CrownId.P1Oni, CrownIdPosition.P1OniUnselected, CrownIdPosition.P1OniSelected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveCrown(CrownId.P1Ura, CrownIdPosition.P1UraUnselected, CrownIdPosition.P1UraSelected, 0.125f));

            Plugin.Instance.StartCoroutine(MoveCrown(CrownId.P2Oni, CrownIdPosition.P2OniUnselected, CrownIdPosition.P2OniSelected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveCrown(CrownId.P2Ura, CrownIdPosition.P2UraUnselected, CrownIdPosition.P2UraSelected, 0.125f));
        }

        public void ExpandCrownsImmediate()
        {
            MoveCrownImmediate(CrownId.P1Oni, CrownIdPosition.P1OniSelected, CrownIdPosition.P1OniSelected);
            MoveCrownImmediate(CrownId.P1Ura, CrownIdPosition.P1UraSelected, CrownIdPosition.P1UraSelected);
                                                 
            MoveCrownImmediate(CrownId.P2Oni, CrownIdPosition.P2OniSelected, CrownIdPosition.P2OniSelected);
            MoveCrownImmediate(CrownId.P2Ura, CrownIdPosition.P2UraSelected, CrownIdPosition.P2UraSelected);
        }

        public void ShrinkCrowns()
        {
            Plugin.Instance.StartCoroutine(MoveCrown(CrownId.P1Oni, CrownIdPosition.P1OniSelected, CrownIdPosition.P1OniUnselected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveCrown(CrownId.P1Ura, CrownIdPosition.P1UraSelected, CrownIdPosition.P1UraUnselected, 0.125f));

            Plugin.Instance.StartCoroutine(MoveCrown(CrownId.P2Oni, CrownIdPosition.P2OniSelected, CrownIdPosition.P2OniUnselected, 0.125f));
            Plugin.Instance.StartCoroutine(MoveCrown(CrownId.P2Ura, CrownIdPosition.P2UraSelected, CrownIdPosition.P2UraUnselected, 0.125f));
        }

        public void ShrinkCrownsImmediate()
        {
            MoveCrownImmediate(CrownId.P1Oni, CrownIdPosition.P1OniUnselected, CrownIdPosition.P1OniUnselected);
            MoveCrownImmediate(CrownId.P1Ura, CrownIdPosition.P1UraUnselected, CrownIdPosition.P1UraUnselected);
                                                 
            MoveCrownImmediate(CrownId.P2Oni, CrownIdPosition.P2OniUnselected, CrownIdPosition.P2OniUnselected);
            MoveCrownImmediate(CrownId.P2Ura, CrownIdPosition.P2UraUnselected, CrownIdPosition.P2UraUnselected);
        }

        IEnumerator MoveCrown(CrownId crown, CrownIdPosition start, CrownIdPosition end, float duration)
        {
            if (CrownGameObjects.ContainsKey(crown))
            {
                GameObject crownObj = CrownGameObjects[crown];

                float elapsedTime = 0f;
                CrownPosition startPos = PlayerCrownPositions.GetCrownPosition(start);
                CrownPosition endPos = PlayerCrownPositions.GetCrownPosition(end);

                while (elapsedTime < duration)
                {
                    crownObj.transform.localPosition = Vector2.Lerp(startPos.Position, endPos.Position, elapsedTime / duration);
                    crownObj.transform.localScale = Vector2.Lerp(startPos.Scale, endPos.Scale, elapsedTime / duration);

                    elapsedTime += Time.deltaTime;

                    yield return new WaitForEndOfFrame();
                }

                crownObj.transform.localPosition = endPos.Position;
                crownObj.transform.localScale = endPos.Scale;
            }
        }

        void MoveCrownImmediate(CrownId crown, CrownIdPosition start, CrownIdPosition end)
        {
            if (CrownGameObjects.ContainsKey(crown))
            {
                GameObject crownObj = CrownGameObjects[crown];

                CrownPosition startPos = PlayerCrownPositions.GetCrownPosition(start);
                CrownPosition endPos = PlayerCrownPositions.GetCrownPosition(end);

                crownObj.transform.localPosition = endPos.Position;
                crownObj.transform.localScale = endPos.Scale;
            }
        }
    }
}
