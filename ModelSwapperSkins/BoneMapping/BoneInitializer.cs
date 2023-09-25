using ModelSwapperSkins.BoneMapping.InitializerRules;
using ModelSwapperSkins.Utils;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ModelSwapperSkins.BoneMapping
{
    public static class BoneInitializer
    {
        public static readonly BoneInitializerRules DefaultBoneRules = BoneInitializerRules_AutoName.Instance;

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
                return;

            BonesProvider bonesProvider = modelTransform.gameObject.AddComponent<BonesProvider>();

            IEnumerable<Bone> bones = from bone in TransformUtils.GetAllChildrenRecursive(modelTransform)
                                      let info = rules.GetBoneInfo(modelTransform, bone)
                                      where info.Type != BoneType.None
                                      select new Bone { BoneTransform = bone, Info = info, ModelPath = TransformUtils.GetObjectPath(bone, modelTransform) };

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
            AddBonesProvider("RoR2/DLC1/AcidLarva/AcidLarvaBody.prefab", BoneInitializerRules_Larva.Instance);

            AddBonesProvider("RoR2/Base/Croco/CrocoBody.prefab", BoneInitializerRules_Acrid.Instance);

            AddBonesProvider("RoR2/DLC1/VoidSurvivor/VoidSurvivorBody.prefab", BoneInitializerRules_VoidFiend.Instance);

            AddBonesProvider("RoR2/Base/Brother/BrotherBody.prefab", BoneInitializerRules_Mithrix.Instance);
            AddBonesProvider("RoR2/Junk/BrotherGlass/BrotherGlassBody.prefab", BoneInitializerRules_Mithrix.Instance);
            AddBonesProvider("RoR2/Base/Brother/BrotherHurtBody.prefab", BoneInitializerRules_Mithrix.Instance);

            AddBonesProvider("RoR2/DLC1/Assassin2/Assassin2Body.prefab", BoneInitializerRules_Assassin.Instance);

            AddBonesProvider("RoR2/Base/Beetle/BeetleQueen2Body.prefab", BoneInitializerRules_BeetleQueen.Instance);

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                AddBonesProvider(body, DefaultBoneRules);
            }
        }
    }
}
