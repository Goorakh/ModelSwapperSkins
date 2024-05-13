using BepInEx.Bootstrap;
using HarmonyLib;
using HG;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace ModelSwapperSkins.Patches
{
    [SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public", Justification = "Patch")]
    static class LoadoutPanelModifications
    {
        static Sprite _fallbackSkinIcon;

        [SystemInitializer]
        static void Init()
        {
            LanguageAPI.Add(new Dictionary<string, string>()
            {
                {
                    "LOADOUT_MODEL",
                    "Model"
                },
                {
                    "LOADOUT_MODEL_DEFAULT",
                    "Default"
                },
                {
                    "LOADOUT_MODEL_SKIN",
                    "Model Skin"
                },
                {
                    "LOADOUT_MODEL_SKIN_FALLBACK",
                    "Default"
                }
            });

            Texture2D skinSwatches = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texSkinSwatches.png").WaitForCompletion();
            if (skinSwatches)
            {
                _fallbackSkinIcon = Sprite.Create(skinSwatches, new Rect(128f * 6f, 128f * 1f, 128f, 128f), Vector2.zero);
                _fallbackSkinIcon.name = "fallbackSkinIcon";
            }

            On.RoR2.UI.LoadoutPanelController.SetDisplayData += LoadoutPanelController_SetDisplayData;
            IL.RoR2.UI.LoadoutPanelController.Row.FromSkin += Row_FromSkin;

            if (!ModCompatibility.SkinTuner.IsActive)
            {
                On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;
            }
            else
            {
                patchSkinTuner();
            }
        }

        static void patchSkinTuner()
        {
            Assembly skinTunerAssembly = ModCompatibility.SkinTuner.Assembly;
            if (skinTunerAssembly == null)
                throw new DllNotFoundException("Failed to load SkinTuner assembly");

            const string PATCH_TARGET_CLASS_NAME = "SkinTuning.UI";
            Type uiClass = skinTunerAssembly.GetType(PATCH_TARGET_CLASS_NAME, false);
            if (uiClass == null)
                throw new TypeLoadException($"Failed to find class '{PATCH_TARGET_CLASS_NAME}'");

            const string PATCH_TARGET_METHOD_NAME = "RebuildLoadoutPanels";
            MethodInfo rebuildLoadoutPanelsMethod = uiClass.GetMethod(PATCH_TARGET_METHOD_NAME, BindingFlags.NonPublic | BindingFlags.Instance);
            if (rebuildLoadoutPanelsMethod == null)
                throw new MissingMethodException(uiClass.FullName, PATCH_TARGET_METHOD_NAME);

            new Hook(rebuildLoadoutPanelsMethod, (Action<MonoBehaviour, On.RoR2.UI.LoadoutPanelController.orig_Rebuild, LoadoutPanelController> patchOrig, MonoBehaviour patchSelf, On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self) =>
            {
                patchOrig(patchSelf, orig, self);
                tryRebuildLoadoutPanel(self);
            });
        }

        static bool shouldModifyLoadoutPanel(LoadoutPanelController loadoutPanel)
        {
            return !ModCompatibility.SkinTuner.IsActive || loadoutPanel.name == ModCompatibility.SkinTuner.SKIN_PANEL_NAME;
        }

        static void LoadoutPanelController_SetDisplayData(On.RoR2.UI.LoadoutPanelController.orig_SetDisplayData orig, LoadoutPanelController self, LoadoutPanelController.DisplayData displayData)
        {
            if (shouldModifyLoadoutPanel(self))
            {
                if (!self.TryGetComponent(out LoadoutPanelSkinResolver skinResolver))
                {
                    skinResolver = self.gameObject.AddComponent<LoadoutPanelSkinResolver>();
                }

                skinResolver.DisplayData = displayData;
            }

            orig(self, displayData);
        }

        static void Row_FromSkin(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            VariableDefinition rowInstance = new VariableDefinition(il.Import(typeof(LoadoutPanelController.Row)));
            il.Body.Variables.Add(rowInstance);

            VariableDefinition skinToAssign = new VariableDefinition(il.Import(typeof(uint)));
            il.Body.Variables.Add(skinToAssign);

            if (c.TryGotoNext(MoveType.After, x => x.MatchNewobj<LoadoutPanelController.Row>()))
            {
                c.Emit(OpCodes.Dup);
                c.Emit(OpCodes.Stloc, rowInstance);
            }
            else
            {
                Log.Error("Failed to find row initialization");
                return;
            }

            c.Index = 0;
            if (c.TryGotoNext(MoveType.Before, x => x.MatchStfld(out FieldReference field) && field.Name == "skinToAssign"))
            {
                c.Emit(OpCodes.Dup);
                c.Emit(OpCodes.Stloc, skinToAssign);
            }
            else
            {
                Log.Error("Failed to find skin index");
                return;
            }

            c.Index = 0;
            if (c.TryGotoNext(MoveType.After,
                              x => x.MatchCallOrCallvirt(SymbolExtensions.GetMethodInfo(() => BodyCatalog.GetBodySkins(default)))))
            {
                c.EmitDelegate((SkinDef[] skins) =>
                {
                    List<SkinDef> normalSkins = new List<SkinDef>(skins.Length);
                    bool discardedAny = false;

                    foreach (SkinDef skin in skins)
                    {
                        if (skin is not ModelSwappedSkinDef)
                        {
                            normalSkins.Add(skin);
                        }
                        else
                        {
                            discardedAny = true;
                        }
                    }

                    return discardedAny ? normalSkins.ToArray() : skins;
                });
            }
            else
            {
                Log.Error("Failed to find skin discard hook location");
                return;
            }

            c.Index = 0;
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt<LoadoutPanelController.Row>(nameof(LoadoutPanelController.Row.AddButton))))
            {
                if (c.TryFindPrev(out ILCursor[] found, x => x.MatchLdftn(out _), x => x.MatchNewobj<UnityAction>()))
                {
                    ILCursor cursor = found[1];

                    cursor.Index++;
                    cursor.Emit(OpCodes.Ldarg_0);
                    cursor.Emit(OpCodes.Ldarg_1);
                    cursor.Emit(OpCodes.Ldloc, rowInstance);
                    cursor.Emit(OpCodes.Ldloc, skinToAssign);
                    cursor.EmitDelegate((UnityAction buttonCallback, LoadoutPanelController loadoutPanelController, BodyIndex bodyIndex, LoadoutPanelController.Row row, uint skinToAssign) =>
                    {
                        if (!shouldModifyLoadoutPanel(loadoutPanelController))
                            return buttonCallback;

                        return new UnityAction(() =>
                        {
                            if (loadoutPanelController.TryGetComponent(out LoadoutPanelSkinResolver skinResolver))
                            {
                                Loadout loadout = new Loadout();

                                SkinDef[] bodySkins = BodyCatalog.GetBodySkins(bodyIndex);

                                skinResolver.BaseSkin = bodySkins != null && skinToAssign < bodySkins.Length ? bodySkins[skinToAssign] : null;

                                SkinDef resolvedSkin = skinResolver.ResolvedSkin;

                                uint resolvedSkinIndex;
                                if (resolvedSkin)
                                {
                                    int localSkinIndex = SkinCatalog.FindLocalSkinIndexForBody(bodyIndex, resolvedSkin);
                                    if (localSkinIndex >= 0)
                                    {
                                        resolvedSkinIndex = (uint)localSkinIndex;
                                    }
                                    else
                                    {
                                        Log.Error("Failed to find local skin index");
                                        resolvedSkinIndex = 0;
                                    }
                                }
                                else
                                {
                                    Log.Error("Failed to resolve skin");
                                    resolvedSkinIndex = 0;
                                }

                                row.userProfile.CopyLoadout(loadout);
                                loadout.bodyLoadoutManager.SetSkinIndex(bodyIndex, resolvedSkinIndex);
                                row.userProfile.SetLoadout(loadout);
                            }
                            else
                            {
                                buttonCallback();
                            }
                        });
                    });
                }
                else
                {
                    Log.Error("Failed to find button callback patch location");
                }
            }
            else
            {
                Log.Error("Failed to find create button patch location");
            }

            c.Index = 0;
            if (c.TryGotoNext(MoveType.Before, x => x.MatchStfld<LoadoutPanelController.Row>(nameof(LoadoutPanelController.Row.findCurrentChoice))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate((Func<Loadout, int> origFindCurrentIndex, LoadoutPanelController loadoutPanelController, BodyIndex bodyIndex) =>
                {
                    if (!shouldModifyLoadoutPanel(loadoutPanelController))
                        return origFindCurrentIndex;

                    return (Loadout loadout) =>
                    {
                        SkinDef skin = SkinCatalog.GetBodySkinDef(bodyIndex, (int)loadout.bodyLoadoutManager.GetSkinIndex(bodyIndex));
                        if (skin is ModelSwappedSkinDef modelSwappedSkin)
                        {
                            SkinDef baseSkin = ArrayUtils.GetSafe(modelSwappedSkin.baseSkins, 0);
                            if (baseSkin)
                            {
                                int baseSkinIndex = SkinCatalog.FindLocalSkinIndexForBody(bodyIndex, baseSkin);
                                if (baseSkinIndex >= 0)
                                {
                                    return baseSkinIndex;
                                }
                            }
                        }

                        return origFindCurrentIndex(loadout);
                    };
                });
            }
            else
            {
                Log.Error("Failed to find current skin index patch location");
            }
        }

        static void tryRebuildLoadoutPanel(LoadoutPanelController loadoutPanel)
        {
            if (!shouldModifyLoadoutPanel(loadoutPanel))
                return;

            if (!loadoutPanel.TryGetComponent(out LoadoutPanelModelSkinsHandler modelSkinsHandler))
            {
                modelSkinsHandler = loadoutPanel.gameObject.AddComponent<LoadoutPanelModelSkinsHandler>();
                modelSkinsHandler.PanelController = loadoutPanel;
            }

            modelSkinsHandler.Rebuild();
        }

        static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);
            tryRebuildLoadoutPanel(self);
        }

        class LoadoutPanelModelSkinsHandler : MonoBehaviour
        {
            public LoadoutPanelController PanelController { get; set; }

            LoadoutPanelController.Row _modelSkinRow;

            void OnEnable()
            {
                UserProfile.onLoadoutChangedGlobal += UserProfile_onLoadoutChangedGlobal;
            }

            void OnDisable()
            {
                UserProfile.onLoadoutChangedGlobal -= UserProfile_onLoadoutChangedGlobal;
            }

            void UserProfile_onLoadoutChangedGlobal(UserProfile userProfile)
            {
                if (PanelController && PanelController.currentDisplayData.userProfile == userProfile)
                {
                    rebuildModelSkinRow();
                }
            }

            public void Rebuild()
            {
                rebuildModelRow();
                rebuildModelSkinRow();
            }

            void rebuildModelRow()
            {
                BodyIndex bodyIndex = PanelController.currentDisplayData.bodyIndex;
                if (bodyIndex == BodyIndex.None)
                    return;

                CharacterBody bodyPrefab = BodyCatalog.GetBodyPrefabBodyComponent(bodyIndex);
                if (!bodyPrefab)
                    return;

                SkinDef[] skins = BodyCatalog.GetBodySkins(bodyIndex);

                LoadoutPanelController.Row modelRow = new LoadoutPanelController.Row(PanelController, bodyIndex, "LOADOUT_MODEL");

                void setSkinModel(BodyIndex modelBodyIndex)
                {
                    if (PanelController.TryGetComponent(out LoadoutPanelSkinResolver skinResolver))
                    {
                        skinResolver.ModelBodyIndex = modelBodyIndex;
                        skinResolver.ModelSkin = BodyCatalog.GetBodySkins(modelBodyIndex).FirstOrDefault(s => s is not ModelSwappedSkinDef);

                        SkinDef resolvedSkin = skinResolver.ResolvedSkin;

                        uint resolvedSkinIndex;
                        if (resolvedSkin)
                        {
                            resolvedSkinIndex = (uint)Mathf.Max(0, Array.IndexOf(skins, resolvedSkin));
                        }
                        else
                        {
                            Log.Error("Failed to resolve skin");
                            resolvedSkinIndex = 0;
                        }

                        Loadout loadout = new Loadout();
                        modelRow.userProfile.CopyLoadout(loadout);
                        loadout.bodyLoadoutManager.SetSkinIndex(bodyIndex, resolvedSkinIndex);
                        modelRow.userProfile.SetLoadout(loadout);
                    }
                    else
                    {
                        Log.Error("Missing skin resolver");
                    }
                }

                Sprite bodyIcon = BodyIconCache.GetOrCreatePortraitIcon(bodyPrefab.portraitIcon as Texture2D);

                modelRow.AddButton(PanelController, bodyIcon, "LOADOUT_MODEL_DEFAULT", string.Empty, modelRow.primaryColor, () =>
                {
                    setSkinModel(BodyIndex.None);
                }, string.Empty, null);

                int[] buttonIndexLookup = new int[BodyCatalog.bodyCount + 1];
                int modelButtonCount = 1;

                foreach (SkinDef skin in skins)
                {
                    if (skin is ModelSwappedSkinDef modelSwappedSkin)
                    {
                        BodyIndex modelBodyIndex = modelSwappedSkin.NewModelBodyPrefab.bodyIndex;

                        ref int buttonIndex = ref buttonIndexLookup[(int)modelBodyIndex + 1];

                        // Button for this model already added
                        if (buttonIndex > 0)
                            continue;

                        modelRow.AddButton(PanelController, skin.icon, modelSwappedSkin.NewModelBodyPrefab.baseNameToken, string.Empty, modelRow.primaryColor, () =>
                        {
                            setSkinModel(modelBodyIndex);
                        }, string.Empty, null);

                        buttonIndex = modelButtonCount++;
                    }
                }

                modelRow.findCurrentChoice = loadout =>
                {
                    uint skinIndex = loadout.bodyLoadoutManager.GetSkinIndex(bodyIndex);

                    if (skins != null && skinIndex < skins.Length && skins[skinIndex] is ModelSwappedSkinDef modelSwappedSkin)
                    {
                        BodyIndex skinModel = modelSwappedSkin.NewModelBodyPrefab.bodyIndex;
                        return ArrayUtils.GetSafe(buttonIndexLookup, (int)skinModel + 1);
                    }
                    else
                    {
                        return 0;
                    }
                };

                modelRow.FinishSetup();

                PanelController.rows.Add(modelRow);

                if (ModCompatibility.SkinTuner.IsActive)
                {
                    IEnumerator waitThenSetSiblingIndex()
                    {
                        yield return new WaitForEndOfFrame();

                        if (modelRow != null && modelRow.rowPanelTransform)
                        {
                            modelRow.rowPanelTransform.SetSiblingIndex(2);
                        }
                    }

                    StartCoroutine(waitThenSetSiblingIndex());
                }
            }

            void rebuildModelSkinRow()
            {
                BodyIndex bodyIndex = PanelController.currentDisplayData.bodyIndex;
                if (bodyIndex == BodyIndex.None)
                    return;

                SkinDef[] skins = BodyCatalog.GetBodySkins(bodyIndex);

                uint selectedSkinIndex = PanelController.currentDisplayData.userProfile.loadout.bodyLoadoutManager.GetSkinIndex(bodyIndex);
                if (selectedSkinIndex >= skins.Length)
                    return;

                int rowIndex;
                if (_modelSkinRow != null)
                {
                    rowIndex = PanelController.rows.IndexOf(_modelSkinRow);
                    _modelSkinRow.Dispose();
                }
                else
                {
                    rowIndex = -1;
                }

                _modelSkinRow = new LoadoutPanelController.Row(PanelController, bodyIndex, "LOADOUT_MODEL_SKIN");

                int skinButtonCount = 0;

                BodyIndex modelBodyIndex;
                if (skins[selectedSkinIndex] is ModelSwappedSkinDef selectedModelSwapSkin)
                {
                    modelBodyIndex = selectedModelSwapSkin.NewModelBodyPrefab.bodyIndex;

                    SkinDef[] modelBodySkins = BodyCatalog.GetBodySkins(modelBodyIndex);
                    int[] buttonIndexLookup = new int[modelBodySkins.Length];

                    for (int i = 0; i < modelBodySkins.Length; i++)
                    {
                        SkinDef skin = modelBodySkins[i];
                        if (skin is ModelSwappedSkinDef)
                            continue;

                        SkinIndex skinIndex = skin.skinIndex;

                        ref int buttonIndex = ref buttonIndexLookup[i];
                        if (buttonIndex > 0)
                            continue;

                        Sprite icon = skin.icon;
                        if (!icon)
                            icon = _fallbackSkinIcon;

                        string nameToken = skin.nameToken;
                        if (string.IsNullOrEmpty(nameToken) || Language.IsTokenInvalid(nameToken))
                        {
                            nameToken = $"Variant {skinButtonCount + 1}";
                        }

                        _modelSkinRow.AddButton(PanelController, icon, nameToken, string.Empty, _modelSkinRow.primaryColor, () =>
                        {
                            if (PanelController.TryGetComponent(out LoadoutPanelSkinResolver skinResolver))
                            {
                                skinResolver.ModelSkin = skin;

                                SkinDef resolvedSkin = skinResolver.ResolvedSkin;

                                uint resolvedSkinIndex;
                                if (resolvedSkin)
                                {
                                    resolvedSkinIndex = (uint)Mathf.Max(0, SkinCatalog.FindLocalSkinIndexForBody(bodyIndex, resolvedSkin));
                                }
                                else
                                {
                                    Log.Error("Failed to resolve skin");
                                    resolvedSkinIndex = 0;
                                }

                                Loadout loadout = new Loadout();
                                _modelSkinRow.userProfile.CopyLoadout(loadout);
                                loadout.bodyLoadoutManager.SetSkinIndex(bodyIndex, resolvedSkinIndex);
                                _modelSkinRow.userProfile.SetLoadout(loadout);
                            }
                            else
                            {
                                Log.Error("Missing skin resolver");
                            }
                        }, string.Empty, null);

                        buttonIndex = skinButtonCount++;
                    }

                    _modelSkinRow.findCurrentChoice = loadout =>
                    {
                        int skinIndex = (int)loadout.bodyLoadoutManager.GetSkinIndex(bodyIndex);
                        if (skinIndex >= 0 && skinIndex < skins.Length && skins[skinIndex] is ModelSwappedSkinDef modelSwappedSkin)
                        {
                            return ArrayUtils.GetSafe(buttonIndexLookup, SkinCatalog.FindLocalSkinIndexForBody(modelBodyIndex, modelSwappedSkin.ModelSkin));
                        }

                        return 0;
                    };
                }
                else
                {
                    bodyIndex = BodyIndex.None;
                }

                if (skinButtonCount == 0)
                {
                    _modelSkinRow.AddButton(PanelController, _fallbackSkinIcon, "LOADOUT_MODEL_SKIN_FALLBACK", string.Empty, _modelSkinRow.primaryColor, () => { }, string.Empty, null);

                    _modelSkinRow.findCurrentChoice = loadout => 0;
                }

                _modelSkinRow.FinishSetup();

                if (rowIndex >= 0)
                {
                    PanelController.rows[rowIndex] = _modelSkinRow;
                }
                else
                {
                    PanelController.rows.Add(_modelSkinRow);
                }

                if (ModCompatibility.SkinTuner.IsActive)
                {
                    IEnumerator waitThenSetSiblingIndex()
                    {
                        yield return new WaitForEndOfFrame();

                        if (_modelSkinRow != null && _modelSkinRow.rowPanelTransform)
                        {
                            _modelSkinRow.rowPanelTransform.SetSiblingIndex(3);
                        }
                    }

                    StartCoroutine(waitThenSetSiblingIndex());
                }
            }
        }

        class LoadoutPanelSkinResolver : MonoBehaviour
        {
            LoadoutPanelController.DisplayData _displayData = new LoadoutPanelController.DisplayData
            {
                bodyIndex = BodyIndex.None,
                userProfile = null
            };

            public LoadoutPanelController.DisplayData DisplayData
            {
                get
                {
                    return _displayData;
                }
                set
                {
                    if (_displayData.Equals(value))
                        return;

                    _displayData = value;

                    SkinDef baseSkin = null;
                    BodyIndex modelBodyIndex = BodyIndex.None;
                    SkinDef modelSkin = null;

                    if (_displayData.userProfile != null && _displayData.bodyIndex != BodyIndex.None)
                    {
                        SkinDef[] bodySkins = BodyCatalog.GetBodySkins(_displayData.bodyIndex);
                        if (bodySkins != null)
                        {
                            uint skinIndex = _displayData.userProfile.loadout.bodyLoadoutManager.GetSkinIndex(_displayData.bodyIndex);
                            if (skinIndex < bodySkins.Length)
                            {
                                SkinDef currentSkin = bodySkins[skinIndex];
                                if (currentSkin is ModelSwappedSkinDef modelSwappedSkin)
                                {
                                    baseSkin = ArrayUtils.GetSafe(modelSwappedSkin.baseSkins, 0);
                                    modelBodyIndex = modelSwappedSkin.NewModelBodyPrefab.bodyIndex;
                                    modelSkin = modelSwappedSkin.ModelSkin;
                                }
                                else
                                {
                                    baseSkin = currentSkin;
                                    modelBodyIndex = BodyIndex.None;
                                    modelSkin = null;
                                }
                            }
                        }
                    }

                    BaseSkin = baseSkin;
                    ModelBodyIndex = modelBodyIndex;
                    ModelSkin = modelSkin;

                    markResolvedSkinDirty();
                }
            }

            SkinDef _baseSkin;
            public SkinDef BaseSkin
            {
                get
                {
                    return _baseSkin;
                }
                set
                {
                    if (_baseSkin == value)
                        return;

                    _baseSkin = value;
                    markResolvedSkinDirty();
                }
            }

            BodyIndex _modelBodyIndex;
            public BodyIndex ModelBodyIndex
            {
                get
                {
                    return _modelBodyIndex;
                }
                set
                {
                    if (_modelBodyIndex == value)
                        return;

                    _modelBodyIndex = value;
                    markResolvedSkinDirty();
                }
            }

            SkinDef _modelSkin;
            public SkinDef ModelSkin
            {
                get
                {
                    return _modelSkin;
                }
                set
                {
                    if (_modelSkin == value)
                        return;

                    _modelSkin = value;
                    markResolvedSkinDirty();
                }
            }

            SkinDef _resolvedSkin;
            public SkinDef ResolvedSkin
            {
                get
                {
                    if (!_resolvedSkin || _resolvedSkinDirty)
                    {
                        _resolvedSkin = null;

                        if (_displayData.userProfile != null && _displayData.bodyIndex != BodyIndex.None)
                        {
                            if (ModelBodyIndex != BodyIndex.None)
                            {
                                SkinDef[] bodySkins = BodyCatalog.GetBodySkins(_displayData.bodyIndex);
                                if (bodySkins != null)
                                {
                                    foreach (SkinDef skin in bodySkins)
                                    {
                                        if (skin is ModelSwappedSkinDef modelSwappedSkin)
                                        {
                                            if (BaseSkin == ArrayUtils.GetSafe(modelSwappedSkin.baseSkins, 0) &&
                                                ModelBodyIndex == modelSwappedSkin.NewModelBodyPrefab.bodyIndex &&
                                                ModelSkin == modelSwappedSkin.ModelSkin)
                                            {
                                                _resolvedSkin = modelSwappedSkin;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _resolvedSkin = BaseSkin;
                            }
                        }

                        _resolvedSkinDirty = false;
                    }

                    return _resolvedSkin;
                }
                set
                {
                    if (value is ModelSwappedSkinDef modelSwappedSkin)
                    {
                        _baseSkin = ArrayUtils.GetSafe(modelSwappedSkin.baseSkins, 0);
                        _modelBodyIndex = modelSwappedSkin.NewModelBodyPrefab.bodyIndex;
                        _modelSkin = modelSwappedSkin.ModelSkin;
                    }
                    else
                    {
                        _baseSkin = value;
                        _modelBodyIndex = BodyIndex.None;
                        _modelSkin = null;
                    }

                    _resolvedSkin = value;
                    _resolvedSkinDirty = false;
                }
            }

            bool _resolvedSkinDirty;
            void markResolvedSkinDirty()
            {
                _resolvedSkinDirty = true;
            }

            void OnEnable()
            {
                UserProfile.onLoadoutChangedGlobal += UserProfile_onLoadoutChangedGlobal;
            }

            void OnDisable()
            {
                UserProfile.onLoadoutChangedGlobal -= UserProfile_onLoadoutChangedGlobal;
            }

            void UserProfile_onLoadoutChangedGlobal(UserProfile userProfile)
            {
                if (_displayData.userProfile == userProfile)
                {
                    uint skinIndex = userProfile.loadout.bodyLoadoutManager.GetSkinIndex(_displayData.bodyIndex);
                    SkinDef skinDef = SkinCatalog.GetBodySkinDef(_displayData.bodyIndex, (int)skinIndex);

                    if (_resolvedSkin != skinDef)
                    {
                        ResolvedSkin = skinDef;
                    }
                }
            }
        }
    }
}
