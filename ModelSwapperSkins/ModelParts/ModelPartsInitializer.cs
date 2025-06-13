using HG;
using HG.Coroutines;
using ModelSwapperSkins.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2BepInExPack.GameAssetPaths;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ModelSwapperSkins.ModelParts
{
    public static class ModelPartsInitializer
    {
        public readonly record struct ModelPartConstructor(string ModelPath, ModelPartFlags Type, ModelPartRendererInfo? RendererInfo = null)
        {
            public readonly ModelPart Construct(Transform modelTransform)
            {
                Transform transform = modelTransform.Find(ModelPath);
                if (!transform)
                {
                    Log.Warning($"Could not find object at path \"{ModelPath}\" relative to {modelTransform}");
                }

                return new ModelPart(transform, Type, ModelPath, RendererInfo);
            }
        }

        public static void OverrideParts(string bodyPrefabAssetGuid, params ModelPartConstructor[] partOverrides)
        {
            if (partOverrides.Length == 0)
                return;

            GameObject bodyPrefab = AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(bodyPrefabAssetGuid)).WaitForCompletion();
            if (!bodyPrefab)
            {
                Log.Warning($"{bodyPrefabAssetGuid} is not a valid GameObject asset");
                return;
            }

            OverrideParts(bodyPrefab, partOverrides);
        }

        public static IEnumerator OverridePartsAsync(string bodyPrefabAssetGuid, params ModelPartConstructor[] partOverrides)
        {
            if (partOverrides.Length == 0)
                yield break;

            AsyncOperationHandle<GameObject> bodyPrefabLoadHandle = AssetAsyncReferenceManager<GameObject>.LoadAsset(new AssetReferenceT<GameObject>(bodyPrefabAssetGuid));
            yield return bodyPrefabLoadHandle;

            GameObject bodyPrefab = bodyPrefabLoadHandle.Result;
            if (!bodyPrefab)
            {
                Log.Warning($"{bodyPrefabAssetGuid} is not a valid GameObject asset");
                yield break;
            }

            OverrideParts(bodyPrefab, partOverrides);
        }

        public static void OverrideParts(GameObject bodyPrefab, params ModelPartConstructor[] partOverrides)
        {
            if (partOverrides.Length == 0)
                return;

            if (!bodyPrefab)
                throw new ArgumentNullException(nameof(bodyPrefab));

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

            ModelPart[] newParts = [.. partOverrides.Select(p => p.Construct(modelTransform)).Where(p => p.Transform)];

            if (modelTransform.TryGetComponent(out ModelPartsProvider partsProvider))
            {
                List<ModelPart> newPartsList = [.. newParts];
                newPartsList.RemoveAll(part =>
                {
                    int existingPartIndex = Array.FindIndex(partsProvider.Parts, p => p.Transform == part.Transform);
                    if (existingPartIndex == -1)
                    {
                        if (part.Flags == ModelPartFlags.None)
                        {
                            Log.Warning($"Adding already non-existing part {part.Path} with flags None for {modelTransform.name} ({bodyPrefab.name})");
                            return true;
                        }

                        return false;
                    }

                    ModelPart existingPart = partsProvider.Parts[existingPartIndex];
                    if (part.Flags != existingPart.Flags)
                    {
                        Log.Debug($"Override model part type {existingPart.Flags}->{part.Flags} at {part.Path} for {modelTransform.name} ({bodyPrefab.name})");
                        existingPart.Flags = part.Flags;
                    }
                    else
                    {
                        Log.Info($"Unnecessary override of model part {part.Path} ({part.Flags}) for {modelTransform.name} ({bodyPrefab.name})");
                    }

                    return true;
                });

                if (newPartsList.Count > 0)
                {
                    ArrayUtil.Append(ref partsProvider.Parts, newPartsList);

                    Log.Debug($"Appended {newPartsList.Count} part(s) to {modelTransform.name} ({bodyPrefab.name})");
                }
            }
            else
            {
                partsProvider = modelTransform.gameObject.AddComponent<ModelPartsProvider>();
                partsProvider.Parts = newParts;
            }

            List<ModelPart> partsList = [.. partsProvider.Parts];
            int numRemovedParts = partsList.RemoveAll(p => p.Flags == ModelPartFlags.None);
            if (numRemovedParts > 0)
            {
                Log.Debug($"Removed {numRemovedParts} part(s) from {modelTransform.name} ({bodyPrefab.name})");
                partsProvider.Parts = [.. partsList];
            }

            SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab.gameObject);
            if (survivorDef && survivorDef.displayPrefab)
            {
                CharacterModel displayPrefabCharacterModel = survivorDef.displayPrefab.GetComponentInChildren<CharacterModel>();
                if (displayPrefabCharacterModel)
                {
                    ModelPartsProvider displayModelPartsProvider = displayPrefabCharacterModel.gameObject.EnsureComponent<ModelPartsProvider>();
                    partsProvider.CopyTo(displayModelPartsProvider);
                }
            }
        }

        [SystemInitializer(typeof(BodyCatalog), typeof(SurvivorCatalog))]
        static IEnumerator Init()
        {
            foreach (CharacterBody bodyPrefab in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                if (!bodyPrefab)
                    continue;

                ModelLocator modelLocator = bodyPrefab.GetComponent<ModelLocator>();
                if (!modelLocator)
                    continue;

                Transform modelTransform = modelLocator.modelTransform;
                if (!modelTransform)
                    continue;

                if (modelTransform.GetComponent<ModelPartsProvider>())
                    continue;

                ModelPartsProvider modelPartsProvider = modelTransform.gameObject.AddComponent<ModelPartsProvider>();

                List<ModelPart> parts = [];

                ModelPartFlags pickPartTypeFromComponent(Component component)
                {
                    ModelPartFlags type = component switch
                    {
                        ParticleSystemRenderer or TrailRenderer => ModelPartFlags.Decoration,
                        Renderer => ModelPartFlags.Body,
                        Light => ModelPartFlags.Decoration,
                        _ => ModelPartFlags.Body
                    };

                    return type;
                }

                ModelPart createModelPartFromComponent(Component component)
                {
                    return new ModelPart(component.transform, modelTransform, pickPartTypeFromComponent(component), null);
                }

                if (modelTransform.TryGetComponent(out CharacterModel characterModel))
                {
                    if (characterModel.baseRendererInfos != null)
                    {
                        foreach (CharacterModel.RendererInfo rendererInfo in characterModel.baseRendererInfos)
                        {
                            if (rendererInfo.renderer)
                            {
                                if (rendererInfo.renderer.transform.IsChildOf(modelTransform))
                                {
                                    parts.Add(createModelPartFromComponent(rendererInfo.renderer));
                                }
                                else
                                {
                                    Log.Debug($"Invalid renderer in {characterModel} ({bodyPrefab}): {rendererInfo.renderer} is not in the model hierarchy");
                                }
                            }
                        }
                    }

                    if (characterModel.baseLightInfos != null)
                    {
                        foreach (CharacterModel.LightInfo lightInfo in characterModel.baseLightInfos)
                        {
                            if (lightInfo.light)
                            {
                                if (lightInfo.light.transform.IsChildOf(modelTransform))
                                {
                                    parts.Add(createModelPartFromComponent(lightInfo.light));
                                }
                                else
                                {
                                    Log.Debug($"Invalid light in {characterModel} ({bodyPrefab}): {lightInfo.light} is not in the model hierarchy");
                                }
                            }
                        }
                    }
                }

                if (parts.Count == 0)
                {
                    parts.AddRange(modelTransform.GetComponentsInChildren<Renderer>().Select(createModelPartFromComponent));
                    parts.AddRange(modelTransform.GetComponentsInChildren<Light>().Select(createModelPartFromComponent));
                }

                modelPartsProvider.Parts = [.. parts];

                SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab.gameObject);
                if (survivorDef && survivorDef.displayPrefab)
                {
                    CharacterModel displayPrefabCharacterModel = survivorDef.displayPrefab.GetComponentInChildren<CharacterModel>();
                    if (displayPrefabCharacterModel && !displayPrefabCharacterModel.GetComponent<ModelPartsProvider>())
                    {
                        ModelPartsProvider displayModelPartsProvider = displayPrefabCharacterModel.gameObject.AddComponent<ModelPartsProvider>();
                        modelPartsProvider.CopyTo(displayModelPartsProvider);
                    }
                }
            }

            ParallelCoroutine parallelOverridePartsCoroutine = new ParallelCoroutine();

            void overrideParts(string bodyPrefabAssetGuid, params ModelPartConstructor[] modelPartConstructors)
            {
                parallelOverridePartsCoroutine.Add(OverridePartsAsync(bodyPrefabAssetGuid, modelPartConstructors));
            }

            overrideParts(RoR2_Junk_AncientWisp.AncientWispBody_prefab,
                          new ModelPartConstructor("AncientWispArmature/Head/GameObject", ModelPartFlags.Decoration));

            overrideParts(RoR2_Junk_Assassin.AssassinBody_prefab,
                          new ModelPartConstructor("AssassinArmature/ROOT,CENTER/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/lower_arm.l/hand.l/Dagger.l/AssassinDaggerMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("AssassinArmature/ROOT,CENTER/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/Dagger.r/AssassinDaggerMesh.001", ModelPartFlags.Weapon));

            // TODO: This should target 2 separate objects, both with the same parent and the same name. Figure out a way to do this.
            overrideParts(RoR2_Base_Drones.BackupDroneBody_prefab,
                          new ModelPartConstructor("BackupDroneArmature/ROOT/Body/MissileDroneBladeActive/Blades", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BackupDroneArmature/ROOT/Body/MissileDroneBladeActive/Blades", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_Junk_BackupDroneOld.BackupDroneOldBody_prefab,
                          new ModelPartConstructor("DroneBackupArmature/BladeRoot/BladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("DroneBackupArmature/BladeRoot (1)/BladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("DroneBackupArmature/BladeRoot (2)/BladeActive", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_Base_Bandit2.Bandit2Body_prefab,
                          new ModelPartConstructor("Bandit2AccessoriesMesh", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BanditArmature/ROOT/base/MainWeapon/BanditShotgunMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BanditArmature/SideWeapon/SideWeaponSpinner/BanditPistolMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BanditArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/lower_arm.l/hand.l/BladeMesh", ModelPartFlags.None));

            overrideParts(RoR2_Junk_Bandit.BanditBody_prefab,
                          new ModelPartConstructor("BanditArmature/ROOT/base/stomach/BanditPistolMeshHip", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BanditArmature/ROOT/base/stomach/chest/upper_arm.l/lower_arm.l/hand.l/ShotgunBone/BanditShotgunMesh", ModelPartFlags.Weapon));

            overrideParts(RoR2_Base_BeetleGroup_BeetleWard.BeetleWard_prefab,
                          new ModelPartConstructor("Indicator", ModelPartFlags.Decoration));

            overrideParts(RoR2_Base_Brother.BrotherBody_prefab,
                          new ModelPartConstructor("BrotherHammerConcrete", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BrotherArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/BrotherShoulderArmor", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BrotherArmature/ROOT/base/stomach/chest/neck/head/eyeball/BrotherEye/EyeTrail", ModelPartFlags.Decoration));

            overrideParts(RoR2_Junk_BrotherGlass.BrotherGlassBody_prefab,
                          new ModelPartConstructor("BrotherBodyMesh", ModelPartFlags.Body, new ModelPartRendererInfo(false, false)),
                          new ModelPartConstructor("BrotherHammerConcrete", ModelPartFlags.Weapon, new ModelPartRendererInfo(true, false)),
                          new ModelPartConstructor("BrotherHammerConcrete/BrotherHammerStib", ModelPartFlags.Weapon, new ModelPartRendererInfo(true, false)));

            overrideParts(RoR2_Base_Brother.BrotherHurtBody_prefab,
                          new ModelPartConstructor("BrotherClothPieces", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BrotherHammerConcrete", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BrotherArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/BrotherShoulderArmor", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_Base_Captain.CaptainBody_prefab,
                          new ModelPartConstructor("CaptainGunArm", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_Junk_ClayMan.ClayBody_prefab,
                          new ModelPartConstructor("ClaymanArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/lower_arm.l/hand.l/shield/ClaymanShieldMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("ClaymanArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/sword/ClaymanSwordMesh", ModelPartFlags.Weapon));

            overrideParts(RoR2_Base_ClayBruiser.ClayBruiserBody_prefab,
                          new ModelPartConstructor("ClayBruiserCannonMesh", ModelPartFlags.Weapon));

            overrideParts(RoR2_Base_Commando.CommandoBody_prefab,
                          new ModelPartConstructor("CommandoArmature/ROOT/base/stomach/chest/upper_arm.l/lower_arm.l/hand.l/gun.l/GunMesh.001", ModelPartFlags.Weapon),
                          new ModelPartConstructor("CommandoArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/gun.r/GunMesh", ModelPartFlags.Weapon));

            overrideParts(RoR2_Base_Croco.CrocoBody_prefab,
                          new ModelPartConstructor("CrocoSpineMesh", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_Base_Drones.Drone1Body_prefab,
                          new ModelPartConstructor("Drone1BladeActive", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_DLC1_DroneCommander.DroneCommanderBody_prefab,
                          new ModelPartConstructor("mdlDroneHat", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_Base_ElectricWorm.ElectricWormBody_prefab,
                          new ModelPartConstructor("WormArmature/Head/PPVolume", ModelPartFlags.Decoration));

            // TODO: This should target 3 separate objects, all with the same parent and the same name. Figure out a way to do this.
            overrideParts(RoR2_Base_Drones.EquipmentDroneBody_prefab,
                          new ModelPartConstructor("EquipmentDroneArmature/BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("EquipmentDroneArmature/BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("EquipmentDroneArmature/BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature));

            // TODO: This should target 2 separate objects, all with the same parent and the same name. Figure out a way to do this.
            overrideParts(RoR2_Base_Drones.FlameDroneBody_prefab,
                          new ModelPartConstructor("BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_DLC2_FalseSon.FalseSonBody_prefab,
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon));

            overrideParts(RoR2_DLC2_FalseSonBoss.FalseSonBossBody_prefab,
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon));

            overrideParts(RoR2_DLC2_FalseSonBoss.FalseSonBossBodyLunarShard_prefab,
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon));

            overrideParts(RoR2_DLC2_FalseSonBoss.FalseSonBossBodyBrokenLunarShard_prefab,
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon),
                          new ModelPartConstructor("FalseSon_LunarSpike3rdPhase", ModelPartFlags.Weapon));

            overrideParts(RoR2_Base_Golem.GolemBody_prefab,
                          new ModelPartConstructor("GolemArmature/ROOT/base/pelvis/thigh.l/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/pelvis/thigh.r/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.l/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.l/upper_arm.l.001/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.r/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.r/upper_arm.r.001/upper_arm.r.002/Debris", ModelPartFlags.Decoration));

            overrideParts(RoR2_Base_Grandparent.GrandParentBody_prefab,
                          new ModelPartConstructor("GrandparentLowMesh", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GrandparentArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/ConnectorSystem", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GrandparentArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/ConnectorSystem", ModelPartFlags.Decoration));

            overrideParts(RoR2_Base_Gravekeeper.GravekeeperBody_prefab,
                          new ModelPartConstructor("GravekeeperMaskMesh", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("Sphere", ModelPartFlags.Decoration));

            overrideParts(RoR2_Junk_HAND.HANDBody_prefab,
                          new ModelPartConstructor("HANDArmature/hammerBone/HANDHammerMesh", ModelPartFlags.Weapon));

            overrideParts(RoR2_Base_Huntress.HuntressBody_prefab,
                          new ModelPartConstructor("BowString", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BowMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ArrowDisplay/Quad 1", ModelPartFlags.None),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ArrowDisplay/Quad 2", ModelPartFlags.None),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ArrowDisplay/Flash", ModelPartFlags.None),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ArrowDisplay", ModelPartFlags.Weapon),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ClusterArrowDisplay/Quad Cluster 1", ModelPartFlags.None),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ClusterArrowDisplay/Quad Cluster 2", ModelPartFlags.None),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ClusterArrowDisplay/Quad Cluster 3", ModelPartFlags.None),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ClusterArrowDisplay/Quad Cluster 4", ModelPartFlags.None),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ClusterArrowDisplay/Quad Cluster 5", ModelPartFlags.None),
                          new ModelPartConstructor("HuntressArmature/ROOT/base/BowRoot/BowStringIKTarget/ClusterArrowDisplay", ModelPartFlags.None));

            overrideParts(RoR2_Base_Loader.LoaderBody_prefab,
                          new ModelPartConstructor("LoaderMechMesh", ModelPartFlags.Decoration));

            overrideParts(RoR2_Base_Mage.MageBody_prefab,
                          new ModelPartConstructor("MageCapeMesh", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_Base_Merc.MercBody_prefab,
                          new ModelPartConstructor("MercMesh", ModelPartFlags.Body),
                          new ModelPartConstructor("MercSwordMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("MercArmature/ROOT/base/pelvis/mdlMercAltPrisonerFrontCloth", ModelPartFlags.Body),
                          new ModelPartConstructor("MercArmature/ROOT/base/stomach/chest/neck/mdlMercAltPrisonerBackCloth", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("MercArmature/ROOT/base/stomach/chest/neck/head/MercAltPrisonerHead", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_DLC1_Railgunner.RailgunnerBody_prefab,
                          new ModelPartConstructor("mdlRailgunBackpackMesh", ModelPartFlags.Decoration),
                          new ModelPartConstructor("mdlRailGunBackpackScreen", ModelPartFlags.Decoration),
                          new ModelPartConstructor("RailGunnerArmature/ROOT/base/stomach/backpack/AllBackpackVFX Root/Idle/Monitor, Charging", ModelPartFlags.None),
                          new ModelPartConstructor("RailGunnerArmature/ROOT/base/stomach/backpack/AllBackpackVFX Root", ModelPartFlags.Decoration),
                          new ModelPartConstructor("mdlRailgunProto", ModelPartFlags.Weapon),
                          new ModelPartConstructor("RailGunnerArmature/ROOT/base/GunRoot/GunBarrel/SMG/SMGBarrel/SMGLaser", ModelPartFlags.Weapon));

            string[] scavBodyPaths = [
                RoR2_Base_Scav.ScavBody_prefab,
                RoR2_Base_ScavLunar.ScavLunar1Body_prefab,
                RoR2_Base_ScavLunar.ScavLunar2Body_prefab,
                RoR2_Base_ScavLunar.ScavLunar3Body_prefab,
                RoR2_Base_ScavLunar.ScavLunar4Body_prefab
            ];

            foreach (string scavBodyPath in scavBodyPaths)
            {
                overrideParts(scavBodyPath,
                              new ModelPartConstructor("ScavArmature/ROOT/base/WeaponParent/ScavWeaponMesh", ModelPartFlags.Weapon),
                              new ModelPartConstructor("ScavBackpackMesh", ModelPartFlags.Decoration));
            }

            string[] titanBodyPaths = [
                RoR2_Base_Titan.TitanBody_prefab,
                RoR2_Base_Titan.TitanGoldBody_prefab
            ];

            foreach (string titanBodyPath in titanBodyPaths)
            {
                overrideParts(titanBodyPath,
                              new ModelPartConstructor("TitanArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/RightFist/Sword", ModelPartFlags.Weapon),
                              new ModelPartConstructor("TitanArmature/ROOT/base/stomach/chest/Head/EyeGlow", ModelPartFlags.Decoration),
                              new ModelPartConstructor("TitanArmature/ROOT/base/stomach/Particle System", ModelPartFlags.Decoration));
            }

            overrideParts(RoR2_Base_Treebot.TreebotBody_prefab,
                          new ModelPartConstructor("TreebotFlowerMesh", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("TreebotRootMesh", ModelPartFlags.BodyFeature));

            overrideParts(RoR2_DLC1_VoidSurvivor.VoidSurvivorBody_prefab,
                          new ModelPartConstructor("mdlVoidSurvivorMetal", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("metalcollar.001", ModelPartFlags.Body));

            overrideParts(RoR2_Base_Vulture.VultureBody_prefab,
                          new ModelPartConstructor("VultureWingFeatherMesh", ModelPartFlags.Decoration),
                          new ModelPartConstructor("VultureWingFeatherMeshSet2", ModelPartFlags.Decoration));

            overrideParts(RoR2_Base_ImpBoss.ImpBossBody_prefab,
                          new ModelPartConstructor("DustCenter", ModelPartFlags.Decoration));

            yield return parallelOverridePartsCoroutine;
        }
    }
}
