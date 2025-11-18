using HG;
using HG.Coroutines;
using ModelSwapperSkins.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2BepInExPack.GameAssetPathsBetter;
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
                            Log.Warning($"Adding already non-existing part {part.Path} with flags None for {modelTransform.name} ({BodyCatalog.GetBodyName(BodyCatalog.FindBodyIndex(bodyPrefab))})");
                            return true;
                        }

                        return false;
                    }

                    ModelPart existingPart = partsProvider.Parts[existingPartIndex];
                    if (part.Flags != existingPart.Flags)
                    {
                        Log.Debug($"Override model part type {existingPart.Flags}->{part.Flags} at {part.Path} for {modelTransform.name} ({BodyCatalog.GetBodyName(BodyCatalog.FindBodyIndex(bodyPrefab))})");
                        existingPart.Flags = part.Flags;
                    }
                    else
                    {
                        Log.Info($"Unnecessary override of model part {part.Path} ({part.Flags}) for {modelTransform.name} ({BodyCatalog.GetBodyName(BodyCatalog.FindBodyIndex(bodyPrefab))})");
                    }

                    return true;
                });

                if (newPartsList.Count > 0)
                {
                    ArrayUtil.Append(ref partsProvider.Parts, newPartsList);

                    Log.Debug($"Appended {newPartsList.Count} part(s) to {modelTransform.name} ({BodyCatalog.GetBodyName(BodyCatalog.FindBodyIndex(bodyPrefab))})");
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
                Log.Debug($"Removed {numRemovedParts} part(s) from {modelTransform.name} ({BodyCatalog.GetBodyName(BodyCatalog.FindBodyIndex(bodyPrefab))})");
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
        static void Init()
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

            static void overrideParts(string bodyName, params ModelPartConstructor[] modelPartConstructors)
            {
                GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(bodyName);
                if (bodyPrefab)
                {
                    OverrideParts(bodyPrefab, modelPartConstructors);
                }
                else
                {
                    Log.Error($"Failed to find body prefab '{bodyName}'");
                }
            }

            overrideParts("AncientWispBody",
                          new ModelPartConstructor("AncientWispArmature/Head/GameObject", ModelPartFlags.Decoration));

            overrideParts("AssassinBody",
                          new ModelPartConstructor("AssassinArmature/ROOT,CENTER/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/lower_arm.l/hand.l/Dagger.l/AssassinDaggerMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("AssassinArmature/ROOT,CENTER/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/Dagger.r/AssassinDaggerMesh.001", ModelPartFlags.Weapon));

            // TODO: This should target 2 separate objects, both with the same parent and the same name. Figure out a way to do this.
            overrideParts("BackupDroneBody",
                          new ModelPartConstructor("BackupDroneArmature/ROOT/Body/MissileDroneBladeActive/Blades", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BackupDroneArmature/ROOT/Body/MissileDroneBladeActive/Blades", ModelPartFlags.BodyFeature));

            overrideParts("BackupDroneOldBody",
                          new ModelPartConstructor("DroneBackupArmature/BladeRoot/BladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("DroneBackupArmature/BladeRoot (1)/BladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("DroneBackupArmature/BladeRoot (2)/BladeActive", ModelPartFlags.BodyFeature));

            overrideParts("Bandit2Body",
                          new ModelPartConstructor("Bandit2AccessoriesMesh", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BanditArmature/ROOT/base/MainWeapon/BanditShotgunMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BanditArmature/SideWeapon/SideWeaponSpinner/BanditPistolMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BanditArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/lower_arm.l/hand.l/BladeMesh", ModelPartFlags.None));

            overrideParts("BanditBody",
                          new ModelPartConstructor("BanditArmature/ROOT/base/stomach/BanditPistolMeshHip", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BanditArmature/ROOT/base/stomach/chest/upper_arm.l/lower_arm.l/hand.l/ShotgunBone/BanditShotgunMesh", ModelPartFlags.Weapon));

            overrideParts("BeetleWard",
                          new ModelPartConstructor("Indicator", ModelPartFlags.Decoration));

            overrideParts("BrotherBody",
                          new ModelPartConstructor("BrotherHammerConcrete", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BrotherArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/BrotherShoulderArmor", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BrotherArmature/ROOT/base/stomach/chest/neck/head/eyeball/BrotherEye/EyeTrail", ModelPartFlags.Decoration),
                          new ModelPartConstructor("BrotherArmature/ROOT/2HWeaponBase", ModelPartFlags.Weapon));

            overrideParts("BrotherGlassBody",
                          new ModelPartConstructor("BrotherBodyMesh", ModelPartFlags.Body, new ModelPartRendererInfo(false, false)),
                          new ModelPartConstructor("BrotherHammerConcrete", ModelPartFlags.Weapon, new ModelPartRendererInfo(true, false)),
                          new ModelPartConstructor("BrotherHammerConcrete/BrotherHammerStib", ModelPartFlags.Weapon, new ModelPartRendererInfo(true, false)),
                          new ModelPartConstructor("BrotherArmature/ROOT/2HWeaponBase", ModelPartFlags.Weapon));

            overrideParts("BrotherHurtBody",
                          new ModelPartConstructor("BrotherClothPieces", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BrotherHammerConcrete", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BrotherArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/BrotherShoulderArmor", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BrotherArmature/ROOT/2HWeaponBase", ModelPartFlags.Weapon));

            overrideParts("CaptainBody",
                          new ModelPartConstructor("CaptainGunArm", ModelPartFlags.BodyFeature));

            overrideParts("ClayBody",
                          new ModelPartConstructor("ClaymanArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/lower_arm.l/hand.l/shield/ClaymanShieldMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("ClaymanArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/sword/ClaymanSwordMesh", ModelPartFlags.Weapon));

            overrideParts("ClayBruiserBody",
                          new ModelPartConstructor("ClayBruiserCannonMesh", ModelPartFlags.Weapon));

            overrideParts("CommandoBody",
                          new ModelPartConstructor("CommandoArmature/ROOT/base/stomach/chest/upper_arm.l/lower_arm.l/hand.l/gun.l/GunMesh.001", ModelPartFlags.Weapon),
                          new ModelPartConstructor("CommandoArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/gun.r/GunMesh", ModelPartFlags.Weapon));

            overrideParts("CrocoBody",
                          new ModelPartConstructor("CrocoSpineMesh", ModelPartFlags.BodyFeature));

            overrideParts("Drone1Body",
                          new ModelPartConstructor("Drone1BladeActive", ModelPartFlags.BodyFeature));

            overrideParts("DroneCommanderBody",
                          new ModelPartConstructor("mdlDroneHat", ModelPartFlags.BodyFeature));

            overrideParts("ElectricWormBody",
                          new ModelPartConstructor("WormArmature/Head/PPVolume", ModelPartFlags.Decoration));

            // TODO: This should target 3 separate objects, all with the same parent and the same name. Figure out a way to do this.
            overrideParts("EquipmentDroneBody",
                          new ModelPartConstructor("EquipmentDroneArmature/BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("EquipmentDroneArmature/BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("EquipmentDroneArmature/BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature));

            // TODO: This should target 2 separate objects, all with the same parent and the same name. Figure out a way to do this.
            overrideParts("FlameDroneBody",
                          new ModelPartConstructor("BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature));

            overrideParts("FalseSonBody",
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon));

            overrideParts("FalseSonBossBody",
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon));

            overrideParts("FalseSonBossBodyLunarShard",
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon));

            overrideParts("FalseSonBossBodyBrokenLunarShard",
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon),
                          new ModelPartConstructor("FalseSon_LunarSpike3rdPhase", ModelPartFlags.Weapon));

            overrideParts("GolemBody",
                          new ModelPartConstructor("GolemArmature/ROOT/base/pelvis/thigh.l/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/pelvis/thigh.r/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.l/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.l/upper_arm.l.001/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.r/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.r/upper_arm.r.001/upper_arm.r.002/Debris", ModelPartFlags.Decoration));

            overrideParts("GrandParentBody",
                          new ModelPartConstructor("GrandparentLowMesh", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GrandparentArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/ConnectorSystem", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GrandparentArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/ConnectorSystem", ModelPartFlags.Decoration));

            overrideParts("GravekeeperBody",
                          new ModelPartConstructor("GravekeeperMaskMesh", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("Sphere", ModelPartFlags.Decoration));

            overrideParts("HANDBody",
                          new ModelPartConstructor("HANDArmature/hammerBone/HANDHammerMesh", ModelPartFlags.Weapon));

            overrideParts("HuntressBody",
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

            overrideParts("LoaderBody",
                          new ModelPartConstructor("LoaderMechMesh", ModelPartFlags.Decoration));

            overrideParts("MageBody",
                          new ModelPartConstructor("MageCapeMesh", ModelPartFlags.BodyFeature));

            overrideParts("MercBody",
                          new ModelPartConstructor("MercMesh", ModelPartFlags.Body),
                          new ModelPartConstructor("MercSwordMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("MercArmature/ROOT/base/pelvis/mdlMercAltPrisonerFrontCloth", ModelPartFlags.Body),
                          new ModelPartConstructor("MercArmature/ROOT/base/stomach/chest/neck/mdlMercAltPrisonerBackCloth", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("MercArmature/ROOT/base/stomach/chest/neck/head/MercAltPrisonerHead", ModelPartFlags.BodyFeature));

            overrideParts("RailgunnerBody",
                          new ModelPartConstructor("mdlRailgunBackpackMesh", ModelPartFlags.Decoration),
                          new ModelPartConstructor("mdlRailGunBackpackScreen", ModelPartFlags.Decoration),
                          new ModelPartConstructor("RailGunnerArmature/ROOT/base/stomach/backpack/AllBackpackVFX Root/Idle/Monitor, Charging", ModelPartFlags.None),
                          new ModelPartConstructor("RailGunnerArmature/ROOT/base/stomach/backpack/AllBackpackVFX Root", ModelPartFlags.Decoration),
                          new ModelPartConstructor("mdlRailgunProto", ModelPartFlags.Weapon),
                          new ModelPartConstructor("RailGunnerArmature/ROOT/base/GunRoot/GunBarrel/SMG/SMGBarrel/SMGLaser", ModelPartFlags.Weapon));

            string[] scavBodyNames = [
                "ScavBody",
                "ScavLunar1Body",
                "ScavLunar2Body",
                "ScavLunar3Body",
                "ScavLunar4Body",
            ];

            foreach (string scavBodyName in scavBodyNames)
            {
                overrideParts(scavBodyName,
                              new ModelPartConstructor("ScavArmature/ROOT/base/WeaponParent/ScavWeaponMesh", ModelPartFlags.Weapon),
                              new ModelPartConstructor("ScavBackpackMesh", ModelPartFlags.Decoration));
            }

            string[] titanBodyNames = [
                "TitanBody",
                "TitanGoldBody",
            ];

            foreach (string titanBodyName in titanBodyNames)
            {
                overrideParts(titanBodyName,
                              new ModelPartConstructor("TitanArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/RightFist/Sword", ModelPartFlags.Weapon),
                              new ModelPartConstructor("TitanArmature/ROOT/base/stomach/chest/Head/EyeGlow", ModelPartFlags.Decoration),
                              new ModelPartConstructor("TitanArmature/ROOT/base/stomach/Particle System", ModelPartFlags.Decoration));
            }

            overrideParts("TreebotBody",
                          new ModelPartConstructor("TreebotFlowerMesh", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("TreebotRootMesh", ModelPartFlags.BodyFeature));

            overrideParts("VoidSurvivorBody",
                          new ModelPartConstructor("mdlVoidSurvivorMetal", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("metalcollar.001", ModelPartFlags.Body));

            overrideParts("VultureBody",
                          new ModelPartConstructor("VultureWingFeatherMesh", ModelPartFlags.Decoration),
                          new ModelPartConstructor("VultureWingFeatherMeshSet2", ModelPartFlags.Decoration));

            overrideParts("ImpBossBody",
                          new ModelPartConstructor("DustCenter", ModelPartFlags.Decoration));

            overrideParts("ChefBody",
                          new ModelPartConstructor("meshChef", ModelPartFlags.Body),
                          new ModelPartConstructor("meshChefIceBox", ModelPartFlags.Body),
                          new ModelPartConstructor("meshChefPizzaCutter", ModelPartFlags.Weapon),
                          new ModelPartConstructor("meshlChefCleaver", ModelPartFlags.Weapon));

            overrideParts("ChildBody",
                          new ModelPartConstructor("meshChild", ModelPartFlags.Body));

            overrideParts("VoidInfestorBody",
                          new ModelPartConstructor("mdlVoidAffixEyes", ModelPartFlags.Body),
                          new ModelPartConstructor("mdlVoidAffixFlesh", ModelPartFlags.Body),
                          new ModelPartConstructor("mdlVoidAffixMetal", ModelPartFlags.Body));

            overrideParts("HalcyoniteBody",
                          new ModelPartConstructor("Halcyonite_G/Halcyonite_Mesh_G/meshHalcyoniteBody", ModelPartFlags.Body),
                          new ModelPartConstructor("Halcyonite_G/Halcyonite_Mesh_G/meshHalcyoniteSword", ModelPartFlags.Weapon));

            overrideParts("DrifterBody",
                          new ModelPartConstructor("meshBag", ModelPartFlags.Weapon),
                          new ModelPartConstructor("meshJunk", ModelPartFlags.Decoration),
                          new ModelPartConstructor("meshDrifter", ModelPartFlags.Body));

            overrideParts("DroneTechBody",
                          new ModelPartConstructor("DroneTech_Body", ModelPartFlags.Body),
                          new ModelPartConstructor("DroneTech_Gun", ModelPartFlags.Weapon),
                          new ModelPartConstructor("Operator_Glow_01", ModelPartFlags.Body),
                          new ModelPartConstructor("Operator_Glow_02", ModelPartFlags.Body),
                          new ModelPartConstructor("Operator_Glow_03", ModelPartFlags.Body),
                          new ModelPartConstructor("Operator_Glow_04", ModelPartFlags.Body));
        }
    }
}
