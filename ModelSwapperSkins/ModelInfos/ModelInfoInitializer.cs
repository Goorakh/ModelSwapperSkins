using ModelSwapperSkins.ModelParts;
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
                Log.Warning($"Unable to calculate model bounds for {modelTransform.name} ({bodyPrefab.name})");
                return;
            }

            float worldSpaceModelHeight = modelBounds.max.y - modelBounds.min.y;
            float modelHeightScale = worldSpaceModelHeight / modelTransform.localScale.y;

            ModelInfoProvider modelInfoProvider = modelTransform.gameObject.AddComponent<ModelInfoProvider>();
            modelInfoProvider.ModelInfo = new ModelInfo(modelHeightScale);

            SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab.gameObject);
            if (survivorDef && survivorDef.displayPrefab)
            {
                CharacterModel displayPrefabCharacterModel = survivorDef.displayPrefab.GetComponentInChildren<CharacterModel>();
                if (displayPrefabCharacterModel && !displayPrefabCharacterModel.GetComponent<ModelInfoProvider>())
                {
                    ModelInfoProvider displayModelInfoProvider = displayPrefabCharacterModel.gameObject.AddComponent<ModelInfoProvider>();
                    modelInfoProvider.CopyTo(displayModelInfoProvider);
                }
            }
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

        [SystemInitializer(typeof(BodyCatalog), typeof(SurvivorCatalog))]
        static void Init()
        {
            SetModelInfo("AcidLarvaBody", new ModelInfo(15f));
            SetModelInfo("AltarSkeletonBody", new ModelInfo(2f));
            SetModelInfo("BeetleQueen2Body", new ModelInfo(20f));
            SetModelInfo("ClayBruiserBody", new ModelInfo(4f));
            SetModelInfo("ClayGrenadierBody", new ModelInfo(3f));
            SetModelInfo("FlyingVerminBody", new ModelInfo(5f));
            SetModelInfo("GrandParentBody", new ModelInfo(50f));
            SetModelInfo("ImpBody", new ModelInfo(2f));
            SetModelInfo("ImpBossBody", new ModelInfo(12.5f));
            SetModelInfo("JellyfishBody", new ModelInfo(7.5f));

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                AutoCalculateModelInfo(body);
            }
        }

#if DEBUG
        [ConCommand(commandName = "set_model_height_scale")]
        static void CCSetModelHeightScale(ConCommandArgs args)
        {
            args.CheckArgumentCount(2);

            BodyIndex targetBodyIndex = args.GetArgBodyIndex(0);
            if (targetBodyIndex == BodyIndex.None)
                return;
            
            float newHeightScale = args.GetArgFloat(1);

            CharacterBody bodyPrefab = BodyCatalog.GetBodyPrefabBodyComponent(targetBodyIndex);
            if (!bodyPrefab)
                return;

            SetModelInfo(bodyPrefab, new ModelInfo(newHeightScale));
        }
#endif
    }
}
