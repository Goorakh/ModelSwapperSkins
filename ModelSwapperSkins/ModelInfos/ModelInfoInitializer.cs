using RoR2;
using System;
using UnityEngine;

namespace ModelSwapperSkins.ModelInfos
{
    public static class ModelInfoInitializer
    {
        public static void AutoCalculateModelInfo(CharacterBody bodyPrefab)
        {
            if (!bodyPrefab)
                return;

            if (!bodyPrefab.TryGetComponent(out ModelLocator modelLocator))
                return;

            Transform modelTransform = modelLocator.modelTransform;
            if (!modelTransform)
            {
                Log.Warning($"{bodyPrefab} has no model transform");
                return;
            }

            if (modelTransform.GetComponent<ModelInfoProvider>())
                return;

            if (!Util.GuessRenderBoundsMeshOnly(modelTransform.gameObject, out Bounds modelBounds))
            {
#if DEBUG
                Log.Debug($"Unable to calculate model bounds for {modelTransform.name} ({bodyPrefab.name})");
#endif
                return;
            }

            float worldSpaceModelHeight = modelBounds.max.y - modelBounds.min.y;
            float modelHeightScale = worldSpaceModelHeight / modelTransform.localScale.y;

            ModelInfo modelInfo = new ModelInfo(modelHeightScale);

            SetModelInfo(bodyPrefab, modelInfo);
        }

        public static void SetModelInfo(string bodyName, ModelInfo modelInfo)
        {
            if (string.IsNullOrWhiteSpace(bodyName))
                throw new ArgumentException($"'{nameof(bodyName)}' cannot be null or whitespace.", nameof(bodyName));

            BodyIndex bodyIndex = BodyCatalog.FindBodyIndex(bodyName);
            if (bodyIndex == BodyIndex.None)
                return;

            CharacterBody bodyPrefab = BodyCatalog.GetBodyPrefabBodyComponent(bodyIndex);
            if (!bodyPrefab)
                return;

            SetModelInfo(bodyPrefab, modelInfo);
        }

        public static void SetModelInfo(CharacterBody bodyPrefab, ModelInfo modelInfo)
        {
            if (!bodyPrefab)
                return;

            if (!bodyPrefab.TryGetComponent(out ModelLocator modelLocator))
                return;

            Transform modelTransform = modelLocator.modelTransform;
            if (!modelTransform)
            {
                Log.Warning($"{bodyPrefab} has no model transform");
                return;
            }

            if (!modelTransform.TryGetComponent(out ModelInfoProvider modelInfoProvider))
                modelInfoProvider = modelTransform.gameObject.AddComponent<ModelInfoProvider>();

            modelInfoProvider.ModelInfo = modelInfo;

            SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab.gameObject);
            if (survivorDef && survivorDef.displayPrefab)
            {
                CharacterModel displayPrefabCharacterModel = survivorDef.displayPrefab.GetComponentInChildren<CharacterModel>();
                if (displayPrefabCharacterModel)
                {
                    if (!displayPrefabCharacterModel.TryGetComponent(out ModelInfoProvider displayModelInfoProvider))
                        displayModelInfoProvider = displayPrefabCharacterModel.gameObject.AddComponent<ModelInfoProvider>();

                    modelInfoProvider.CopyTo(displayModelInfoProvider);
                }
            }
        }

        public static bool TryGetModelInfo(BodyIndex bodyIndex, out ModelInfo modelInfo)
        {
            return TryGetModelInfo(BodyCatalog.GetBodyPrefabBodyComponent(bodyIndex), out modelInfo);
        }

        public static bool TryGetModelInfo(CharacterBody bodyPrefab, out ModelInfo modelInfo)
        {
            if (bodyPrefab && bodyPrefab.TryGetComponent(out ModelLocator modelLocator))
            {
                Transform modelTransform = modelLocator.modelTransform;
                if (modelTransform && modelTransform.TryGetComponent(out ModelInfoProvider modelInfoProvider))
                {
                    modelInfo = modelInfoProvider.ModelInfo;
                    return true;
                }
            }

            modelInfo = default;
            return false;
        }

        [SystemInitializer(typeof(BodyCatalog), typeof(SurvivorCatalog))]
        static void Init()
        {
            SetModelInfo("AcidLarvaBody", new ModelInfo(15f));
            SetModelInfo("AltarSkeletonBody", new ModelInfo(2f));
            SetModelInfo("Assassin2Body", new ModelInfo(3.75f));
            SetModelInfo("BeetleQueen2Body", new ModelInfo(20f));
            SetModelInfo("BrotherBody", new ModelInfo(2f));
            SetModelInfo("BrotherGlassBody", new ModelInfo(2f));
            SetModelInfo("BrotherHurtBody", new ModelInfo(2f));
            SetModelInfo("ClayBruiserBody", new ModelInfo(3.7f));
            SetModelInfo("ClayGrenadierBody", new ModelInfo(3f));
            SetModelInfo("CrocoBody", new ModelInfo(22.5f));
            SetModelInfo("FlyingVerminBody", new ModelInfo(5f));
            SetModelInfo("GolemBody", new ModelInfo(5.5f));
            SetModelInfo("GrandParentBody", new ModelInfo(50f));
            SetModelInfo("HereticBody", new ModelInfo(4f));
            SetModelInfo("ImpBody", new ModelInfo(2f));
            SetModelInfo("ImpBossBody", new ModelInfo(12.5f));
            SetModelInfo("JellyfishBody", new ModelInfo(7.5f));
            SetModelInfo("LemurianBody", new ModelInfo(20f));
            SetModelInfo("LemurianBruiserBody", new ModelInfo(25f));
            SetModelInfo("LunarExploderBody", new ModelInfo(5f));
            SetModelInfo("MiniMushroomBody", new ModelInfo(4.5f));
            SetModelInfo("MiniVoidRaidCrabBodyBase", new ModelInfo(250f));
            SetModelInfo("NullifierBody", new ModelInfo(10f));
            SetModelInfo("NullifierAllyBody", new ModelInfo(10f));
            SetModelInfo("RailgunnerBody", new ModelInfo(2f));
            SetModelInfo("ToolbotBody", new ModelInfo(20f));
            SetModelInfo("TreebotBody", new ModelInfo(5f));
            SetModelInfo("VerminBody", new ModelInfo(5f));

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                AutoCalculateModelInfo(body);
            }
        }

        [ConCommand(commandName = "model_height_scale", helpText = "Overrides the height of a character's model. Used if the automatic scale calculation isn't correct. 1-2 arguments: [body name] (optional: [new height]). if no height is provided, the current height is printed to the console")]
        static void CCModelHeightScale(ConCommandArgs args)
        {
            args.CheckArgumentCount(1);

            BodyIndex targetBodyIndex = args.GetArgBodyIndex(0);
            if (targetBodyIndex == BodyIndex.None)
                return;

            CharacterBody bodyPrefab = BodyCatalog.GetBodyPrefabBodyComponent(targetBodyIndex);
            if (!bodyPrefab)
                return;

            if (args.Count == 1)
            {
                if (TryGetModelInfo(bodyPrefab, out ModelInfo modelInfo))
                {
                    Debug.Log($"{bodyPrefab.name} height scale: {modelInfo.HeightScale}");
                }
                else
                {
                    Debug.Log($"{bodyPrefab.name} has no model info provider");
                }
            }
            else
            {
                float newHeightScale = args.GetArgFloat(1);
                SetModelInfo(bodyPrefab, new ModelInfo(newHeightScale));
            }
        }
    }
}
