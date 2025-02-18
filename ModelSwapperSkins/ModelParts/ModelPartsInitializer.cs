﻿using ModelSwapperSkins.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

        public static void OverrideParts(string bodyPrefabPath, params ModelPartConstructor[] partOverrides)
        {
            if (partOverrides.Length == 0)
                return;

            GameObject bodyPrefab = Addressables.LoadAssetAsync<GameObject>(bodyPrefabPath).WaitForCompletion();
            if (!bodyPrefab)
            {
                Log.Warning($"{bodyPrefabPath} is not a valid GameObject asset");
                return;
            }

            OverrideParts(bodyPrefab, partOverrides);
        }

        public static void OverrideParts(GameObject bodyPrefab, params ModelPartConstructor[] partOverrides)
        {
            if (partOverrides.Length == 0)
                return;

            if (!bodyPrefab)
            {
                throw new ArgumentNullException(nameof(bodyPrefab));
            }

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

            ModelPart[] newParts = partOverrides.Select(p => p.Construct(modelTransform))
                                                .Where(p => p.Transform)
                                                .ToArray();

            if (modelTransform.TryGetComponent(out ModelPartsProvider partsProvider))
            {
                List<ModelPart> newPartsList = newParts.ToList();
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

            List<ModelPart> partsList = partsProvider.Parts.ToList();
            int numRemovedParts = partsList.RemoveAll(p => p.Flags == ModelPartFlags.None);
            if (numRemovedParts > 0)
            {
                Log.Debug($"Removed {numRemovedParts} part(s) from {modelTransform.name} ({bodyPrefab.name})");

                partsProvider.Parts = partsList.ToArray();
            }

            SurvivorDef survivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab.gameObject);
            if (survivorDef && survivorDef.displayPrefab)
            {
                CharacterModel displayPrefabCharacterModel = survivorDef.displayPrefab.GetComponentInChildren<CharacterModel>();
                if (displayPrefabCharacterModel)
                {
                    if (!displayPrefabCharacterModel.TryGetComponent(out ModelPartsProvider displayModelPartsProvider))
                        displayModelPartsProvider = displayPrefabCharacterModel.gameObject.AddComponent<ModelPartsProvider>();

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

                modelPartsProvider.Parts = parts.ToArray();

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

            OverrideParts("RoR2/Junk/AncientWisp/AncientWispBody.prefab",
                          new ModelPartConstructor("AncientWispArmature/Head/GameObject", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Junk/Assassin/AssassinBody.prefab",
                          new ModelPartConstructor("AssassinArmature/ROOT,CENTER/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/lower_arm.l/hand.l/Dagger.l/AssassinDaggerMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("AssassinArmature/ROOT,CENTER/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/Dagger.r/AssassinDaggerMesh.001", ModelPartFlags.Weapon));

            // TODO: This should target 2 separate objects, both with the same parent and the same name. Figure out a way to do this.
            OverrideParts("RoR2/Base/Drones/BackupDroneBody.prefab",
                          new ModelPartConstructor("BackupDroneArmature/ROOT/Body/MissileDroneBladeActive/Blades", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BackupDroneArmature/ROOT/Body/MissileDroneBladeActive/Blades", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/Junk/BackupDroneOld/BackupDroneOldBody.prefab",
                          new ModelPartConstructor("DroneBackupArmature/BladeRoot/BladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("DroneBackupArmature/BladeRoot (1)/BladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("DroneBackupArmature/BladeRoot (2)/BladeActive", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/Base/Bandit2/Bandit2Body.prefab",
                          new ModelPartConstructor("Bandit2AccessoriesMesh", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BanditArmature/ROOT/base/MainWeapon/BanditShotgunMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BanditArmature/SideWeapon/SideWeaponSpinner/BanditPistolMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BanditArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/lower_arm.l/hand.l/BladeMesh", ModelPartFlags.None));

            OverrideParts("RoR2/Junk/Bandit/BanditBody.prefab",
                          new ModelPartConstructor("BanditArmature/ROOT/base/stomach/BanditPistolMeshHip", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BanditArmature/ROOT/base/stomach/chest/upper_arm.l/lower_arm.l/hand.l/ShotgunBone/BanditShotgunMesh", ModelPartFlags.Weapon));

            OverrideParts("RoR2/Base/Beetle/BeetleWard.prefab",
                          new ModelPartConstructor("Indicator", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Base/Brother/BrotherBody.prefab",
                          new ModelPartConstructor("BrotherHammerConcrete", ModelPartFlags.Weapon),
                          new ModelPartConstructor("BrotherArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/BrotherShoulderArmor", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/Junk/BrotherGlass/BrotherGlassBody.prefab",
                          new ModelPartConstructor("BrotherBodyMesh", ModelPartFlags.Body, new ModelPartRendererInfo(false, false)),
                          new ModelPartConstructor("BrotherHammerConcrete", ModelPartFlags.Weapon, new ModelPartRendererInfo(true, false)),
                          new ModelPartConstructor("BrotherHammerConcrete/BrotherHammerStib", ModelPartFlags.Weapon, new ModelPartRendererInfo(true, false)));

            OverrideParts("RoR2/Base/Brother/BrotherHurtBody.prefab",
                          new ModelPartConstructor("BrotherClothPieces", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BrotherArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/BrotherShoulderArmor", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/Base/Captain/CaptainBody.prefab",
                          new ModelPartConstructor("CaptainGunArm", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/Junk/ClayMan/ClayBody.prefab",
                          new ModelPartConstructor("ClaymanArmature/ROOT/base/stomach/chest/clavicle.l/upper_arm.l/lower_arm.l/hand.l/shield/ClaymanShieldMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("ClaymanArmature/ROOT/base/stomach/chest/clavicle.r/upper_arm.r/lower_arm.r/hand.r/sword/ClaymanSwordMesh", ModelPartFlags.Weapon));

            OverrideParts("RoR2/Base/ClayBruiser/ClayBruiserBody.prefab",
                          new ModelPartConstructor("ClayBruiserCannonMesh", ModelPartFlags.Weapon));

            OverrideParts("RoR2/Base/Commando/CommandoBody.prefab",
                          new ModelPartConstructor("CommandoArmature/ROOT/base/stomach/chest/upper_arm.l/lower_arm.l/hand.l/gun.l/GunMesh.001", ModelPartFlags.Weapon),
                          new ModelPartConstructor("CommandoArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/gun.r/GunMesh", ModelPartFlags.Weapon));

            OverrideParts("RoR2/Base/Croco/CrocoBody.prefab",
                          new ModelPartConstructor("CrocoSpineMesh", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/Base/Drones/Drone1Body.prefab",
                          new ModelPartConstructor("Drone1BladeActive", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/DLC1/DroneCommander/DroneCommanderBody.prefab",
                          new ModelPartConstructor("mdlDroneHat", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/Base/ElectricWorm/ElectricWormBody.prefab",
                          new ModelPartConstructor("WormArmature/Head/PPVolume", ModelPartFlags.Decoration));

            // TODO: This should target 3 separate objects, all with the same parent and the same name. Figure out a way to do this.
            OverrideParts("RoR2/Base/Drones/EquipmentDroneBody.prefab",
                          new ModelPartConstructor("EquipmentDroneArmature/BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("EquipmentDroneArmature/BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("EquipmentDroneArmature/BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature));

            // TODO: This should target 2 separate objects, all with the same parent and the same name. Figure out a way to do this.
            OverrideParts("RoR2/Base/Drones/FlameDroneBody.prefab",
                          new ModelPartConstructor("BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("BladeOn/DroneBladeActive", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/DLC2/FalseSon/FalseSonBody.prefab",
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon),
                          new ModelPartConstructor("FSArmature/Root/Hips/Spine1/Spine2/Spine3/SpineEnd/L_Clav/L_Upperarm/L_Forearm/L_Hand/L_Hand_Object/L_Weapon/", ModelPartFlags.Weapon));

            OverrideParts("RoR2/DLC2/FalseSonBoss/FalseSonBossBody.prefab",
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon),
                          new ModelPartConstructor("FSArmature/Root/Hips/Spine1/Spine2/Spine3/SpineEnd/L_Clav/L_Upperarm/L_Forearm/L_Hand/L_Hand_Object/L_Weapon/", ModelPartFlags.Weapon));

            OverrideParts("RoR2/DLC2/FalseSonBoss/FalseSonBossBodyLunarShard.prefab",
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon),
                          new ModelPartConstructor("FSArmature/Root/Hips/Spine1/Spine2/Spine3/SpineEnd/L_Clav/L_Upperarm/L_Forearm/L_Hand/L_Hand_Object/L_Weapon/", ModelPartFlags.Weapon));

            OverrideParts("RoR2/DLC2/FalseSonBoss/FalseSonBossBodyBrokenLunarShard.prefab",
                          new ModelPartConstructor("L_FalseSon_Weapon", ModelPartFlags.Weapon),
                          new ModelPartConstructor("FSArmature/Root/Hips/Spine1/Spine2/Spine3/SpineEnd/L_Clav/L_Upperarm/L_Forearm/L_Hand/L_Hand_Object/L_Weapon/", ModelPartFlags.Weapon));

            OverrideParts("RoR2/Base/Golem/GolemBody.prefab",
                          new ModelPartConstructor("GolemArmature/ROOT/base/pelvis/thigh.l/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/pelvis/thigh.r/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.l/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.l/upper_arm.l.001/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.r/Debris", ModelPartFlags.Decoration),
                          new ModelPartConstructor("GolemArmature/ROOT/base/stomach/chest/upper_arm.r/upper_arm.r.001/upper_arm.r.002/Debris", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Base/Grandparent/GrandParentBody.prefab",
                          new ModelPartConstructor("GrandparentLowMesh", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Base/Gravekeeper/GravekeeperBody.prefab",
                          new ModelPartConstructor("GravekeeperMaskMesh", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("Sphere", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Junk/HAND/HANDBody.prefab",
                          new ModelPartConstructor("HANDArmature/hammerBone/HANDHammerMesh", ModelPartFlags.Weapon));

            OverrideParts("RoR2/Base/Huntress/HuntressBody.prefab",
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

            OverrideParts("RoR2/Base/Loader/LoaderBody.prefab",
                          new ModelPartConstructor("LoaderMechMesh", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Base/Mage/MageBody.prefab",
                          new ModelPartConstructor("MageCapeMesh", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/Base/Merc/MercBody.prefab",
                          new ModelPartConstructor("MercSwordMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("MercArmature/ROOT/base/pelvis/mdlMercAltPrisonerFrontCloth", ModelPartFlags.Body),
                          new ModelPartConstructor("MercArmature/ROOT/base/stomach/chest/neck/mdlMercAltPrisonerBackCloth", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("MercArmature/ROOT/base/stomach/chest/neck/head/MercAltPrisonerHead", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/DLC1/Railgunner/RailgunnerBody.prefab",
                          new ModelPartConstructor("mdlRailgunBackpackMesh", ModelPartFlags.Decoration),
                          new ModelPartConstructor("mdlRailGunBackpackScreen", ModelPartFlags.Decoration),
                          new ModelPartConstructor("RailGunnerArmature/ROOT/base/stomach/backpack/AllBackpackVFX Root/Idle/Monitor, Charging", ModelPartFlags.None),
                          new ModelPartConstructor("RailGunnerArmature/ROOT/base/stomach/backpack/AllBackpackVFX Root", ModelPartFlags.Decoration),
                          new ModelPartConstructor("mdlRailgunProto", ModelPartFlags.Weapon),
                          new ModelPartConstructor("RailGunnerArmature/ROOT/base/GunRoot/GunBarrel/SMG/SMGBarrel/SMGLaser", ModelPartFlags.Weapon));

            OverrideParts("RoR2/Base/Scav/ScavBody.prefab",
                          new ModelPartConstructor("ScavArmature/ROOT/base/WeaponParent/ScavWeaponMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("ScavBackpackMesh", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Base/ScavLunar/ScavLunar1Body.prefab",
                          new ModelPartConstructor("ScavArmature/ROOT/base/WeaponParent/ScavWeaponMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("ScavBackpackMesh", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Base/ScavLunar/ScavLunar2Body.prefab",
                          new ModelPartConstructor("ScavArmature/ROOT/base/WeaponParent/ScavWeaponMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("ScavBackpackMesh", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Base/ScavLunar/ScavLunar3Body.prefab",
                          new ModelPartConstructor("ScavArmature/ROOT/base/WeaponParent/ScavWeaponMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("ScavBackpackMesh", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Base/ScavLunar/ScavLunar4Body.prefab",
                          new ModelPartConstructor("ScavArmature/ROOT/base/WeaponParent/ScavWeaponMesh", ModelPartFlags.Weapon),
                          new ModelPartConstructor("ScavBackpackMesh", ModelPartFlags.Decoration));

            OverrideParts("RoR2/Base/Titan/TitanGoldBody.prefab",
                          new ModelPartConstructor("TitanArmature/ROOT/base/stomach/chest/upper_arm.r/lower_arm.r/hand.r/RightFist/Sword", ModelPartFlags.Weapon));

            OverrideParts("RoR2/Base/Treebot/TreebotBody.prefab",
                          new ModelPartConstructor("TreebotFlowerMesh", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("TreebotRootMesh", ModelPartFlags.BodyFeature));

            OverrideParts("RoR2/DLC1/VoidSurvivor/VoidSurvivorBody.prefab",
                          new ModelPartConstructor("mdlVoidSurvivorMetal", ModelPartFlags.BodyFeature),
                          new ModelPartConstructor("metalcollar.001", ModelPartFlags.Body));

            OverrideParts("RoR2/Base/Vulture/VultureBody.prefab",
                          new ModelPartConstructor("VultureWingFeatherMesh", ModelPartFlags.Decoration),
                          new ModelPartConstructor("VultureWingFeatherMeshSet2", ModelPartFlags.Decoration));
        }
    }
}
