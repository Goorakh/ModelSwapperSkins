using ModelSwapperSkins.BoneMapping.InitializerRules;
using ModelSwapperSkins.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ModelSwapperSkins.BoneMapping
{
    public static class BoneInitializer
    {
        public static readonly BoneInitializerRules DefaultBoneRules = BoneInitializerRules_AutoName.Instance;

        static readonly HashSet<BoneInitializerRules> _overrideInitializerRules = new HashSet<BoneInitializerRules>();

        public static void AddCustomBoneInitializerRules(BoneInitializerRules initializerRules)
        {
            if (!_overrideInitializerRules.Add(initializerRules))
            {
                Log.Warning($"Attempting to add duplicate initializer rules: {initializerRules}");
            }
        }

        public static BoneInitializerRules FindInitializerRulesFor(BodyIndex bodyIndex)
        {
            return _overrideInitializerRules.FirstOrDefault(r => r.AppliesTo(bodyIndex)) ?? DefaultBoneRules;
        }

        public static bool HasCustomIntializerRules(BodyIndex bodyIndex)
        {
            return FindInitializerRulesFor(bodyIndex) != DefaultBoneRules;
        }

        [SystemInitializer(typeof(BodyCatalog))]
        static void Init()
        {
            AddCustomBoneInitializerRules(BoneInitializerRules_AcidLarva.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Croco.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_VoidSurvivor.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Brother.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Assassin2.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_BeetleQueen2.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Bison.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Captain.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_ClayBruiser.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_ClayGrenadier.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_FlyingVermin.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_GrandParent.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Gravekeeper.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_GreaterWisp.Instance);
            // AddCustomBoneInitializerRules(BoneInitializerRules_Heretic.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_HermitCrab.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Jellyfish.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Lemurian.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_LemurianBruiser.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_LunarExploder.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Mage.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Parent.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Railgunner.Instance);

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                BoneInitializerRules initializerRules = FindInitializerRulesFor(body.bodyIndex);

#if DEBUG
                Log.Debug($"Using bone intializer rules {initializerRules} for {body.name}");
#endif

                initializeBones(body, initializerRules);
            }
        }

        static void initializeBones(CharacterBody bodyPrefab, BoneInitializerRules rules)
        {
            if (!bodyPrefab)
                throw new ArgumentNullException(nameof(bodyPrefab));

            if (rules is null)
                throw new ArgumentNullException(nameof(rules));

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
    }
}
