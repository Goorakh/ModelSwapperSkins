using ModelSwapperSkins.ModelInfo;
using ModelSwapperSkins.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ModelSwapperSkins.BoneMapping
{
    public static class BoneInitializer
    {
        public static readonly BoneInitializerRules DefaultBoneRules = new BoneInitializerRules_AutoName();

        public static void AddBonesProvider(string bodyPrefabPath, BoneInitializerRules rules)
        {
            GameObject bodyPrefab = Addressables.LoadAssetAsync<GameObject>(bodyPrefabPath).WaitForCompletion();
            if (!bodyPrefab)
            {
                Log.Warning($"{bodyPrefabPath} is not a valid GameObject asset");
                return;
            }

            if (bodyPrefab.TryGetComponent(out CharacterBody body))
            {
                AddBonesProvider(body, rules);
            }
            else
            {
                Log.Warning($"{bodyPrefabPath} has no CharacterBody component");
            }
        }

        public static void AddBonesProvider(CharacterBody bodyPrefab, BoneInitializerRules rules)
        {
            if (!bodyPrefab.TryGetComponent(out ModelLocator modelLocator))
            {
                Log.Warning($"{bodyPrefab} has no ModelLocator component");
                return;
            }

            Transform modelTransform = modelLocator.modelTransform;
            if (!modelTransform)
            {
                Log.Warning($"{bodyPrefab} has no model transform");
                return;
            }

            if (modelTransform.GetComponent<BonesProvider>())
            {
                Log.Warning($"Attempted to add duplicate model parts provider to {modelTransform.name}");
                return;
            }

            BonesProvider bonesProvider = modelTransform.gameObject.AddComponent<BonesProvider>();

            IEnumerable<Bone> bones = from bone in TransformUtils.GetAllChildrenRecursive(modelTransform)
                                      let type = rules.TryResolveBoneType(modelTransform, bone)
                                      where type.HasValue
                                      select new Bone { BoneTransform = bone, Type = type.Value, ModelPath = TransformUtils.GetObjectPath(bone, modelTransform) };

            bonesProvider.Bones = bones.ToArray();

            SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab.gameObject);
            if (survivorDef && survivorDef.displayPrefab)
            {
                CharacterModel displayPrefabCharacterModel = survivorDef.displayPrefab.GetComponentInChildren<CharacterModel>();
                if (displayPrefabCharacterModel && !displayPrefabCharacterModel.GetComponent<BonesProvider>())
                {
                    BonesProvider displayModelBonesProvider = displayPrefabCharacterModel.gameObject.AddComponent<BonesProvider>();
                    bonesProvider.CopyTo(displayModelBonesProvider);
                }
            }

#if DEBUG
            Log.Debug($"Added BonesProvider component ({bonesProvider.Bones.Length} bone(s)) to {modelTransform.name} ({bodyPrefab.name})");
#endif
        }

        [SystemInitializer(typeof(BodyCatalog))]
        static void Init()
        {
            // addBonesProvider("RoR2/DLC1/AcidLarva/AcidLarvaBody.prefab", DefaultBoneRules);

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                AddBonesProvider(body, DefaultBoneRules);
            }
        }
    }
}
