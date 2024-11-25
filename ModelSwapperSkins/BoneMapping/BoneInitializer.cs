using BepInEx.Bootstrap;
using ModelSwapperSkins.BoneMapping.InitializerRules;
using ModelSwapperSkins.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    public static class BoneInitializer
    {
        public static readonly BoneInitializerRules DefaultBoneRules = BoneInitializerRules_AutoName.Instance;

        static readonly HashSet<BoneInitializerRules> _overrideInitializerRules = [];

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
            AddCustomBoneInitializerRules(BoneInitializerRules_AltarSkeleton.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Assassin.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Assassin2.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Bandit2.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_BeetleGuard.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_BeetleQueen2.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Bison.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Brother.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Captain.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_ClayBruiser.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_ClayGrenadier.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Clay.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Croco.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Engi.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_FlyingVermin.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Golem.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_GrandParent.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Gravekeeper.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_GreaterWisp.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Gup.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Heretic.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_HermitCrab.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Huntress.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Imp.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_ImpBoss.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Jellyfish.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Lemurian.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_LemurianBruiser.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_LunarExploder.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Mage.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Merc.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_MiniMushroom.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_MiniVoidRaidCrab.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Nullifier.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Parent.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Railgunner.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Scav.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Seeker.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Shopkeeper.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Titan.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Toolbot.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Treebot.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Vermin.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_VoidJailer.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_VoidSurvivor.Instance);
            AddCustomBoneInitializerRules(BoneInitializerRules_Vulture.Instance);

            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                CharacterBody boneSourceBody = body;

                if (Chainloader.PluginInfos.ContainsKey("com.CherryDye.MonsterMash"))
                {
                    if (boneSourceBody.name.StartsWith("PC"))
                    {
                        string baseBodyName = boneSourceBody.name.Substring(2);
                        BodyIndex baseBodyIndex = BodyCatalog.FindBodyIndex(baseBodyName);
                        if (baseBodyIndex != BodyIndex.None)
                        {
                            boneSourceBody = BodyCatalog.GetBodyPrefabBodyComponent(baseBodyIndex);
                        }
                    }
                }

                BoneInitializerRules initializerRules = FindInitializerRulesFor(boneSourceBody.bodyIndex);

                if (initializerRules == DefaultBoneRules)
                {
                    switch (body.name)
                    {
                        case "AncientWispBody": // Weird parenting issues, blacklist for now
                        case "BeetleBody": // Has no bones, is animated by black magic
                        case "BeetleCrystalBody": // Has no bones, is animated by black magic
                        case "BeetleGuardCrystalBody": // Bad material, logspam
                        case "BomberBody": // Just Commando
                        case "CommandoPerformanceTestBody": // Just Commando
                        case "DevotedLemurianBody":
                        case "DevotedLemurianBruiserBody":
                        case "EnforcerBody": // Literally just a cube
                        case "GolemBodyInvincible": // Just Stone Golem
                        case "SniperBody": // Broken texture commando model
                        case "VoidMegaCrabBody": // No easy mappings, blacklist for now
                        case "VoidMegaCrabAllyBody": // No easy mappings, blacklist for now
                            continue;
                    }

                    if (body.name.EndsWith("_opt"))
                    {
                        if (BodyCatalog.FindBodyIndex(body.name.Remove(body.name.Length - 4)) != BodyIndex.None)
                        {
                            continue;
                        }
                    }
                }

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
#if DEBUG
                Log.Debug($"{bodyPrefab} has no ModelLocator component");
#endif
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

            List<Bone> bones = TransformUtils.GetAllChildrenRecursive(modelTransform).Select(boneTransform =>
            {
                BoneInfo boneInfo = rules.GetBoneInfo(modelTransform, boneTransform);
                if (boneInfo.Type == BoneType.None)
                    return null;

                return new Bone(boneInfo, modelTransform, boneTransform);
            }).Where(b => b != null).ToList();

            bones.AddRange(rules.GetAdditionalBones(modelTransform, bones));

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
