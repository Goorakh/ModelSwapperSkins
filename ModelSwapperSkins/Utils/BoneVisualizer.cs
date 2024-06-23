using ModelSwapperSkins.BoneMapping;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ModelSwapperSkins.Utils
{
    static class BoneVisualizer
    {
        static bool _drawModelBones = false;

        [ConCommand(commandName = "draw_model_bones")]
        static void CCEnableBoneVisualizer(ConCommandArgs args)
        {
            _drawModelBones = !_drawModelBones;

            if (_drawModelBones)
            {
                foreach (CharacterModel characterModel in InstanceTracker.GetInstancesList<CharacterModel>())
                {
                    characterModel.gameObject.AddComponent<ModelBonesDrawer>();
                }
            }
            else
            {
                List<ModelBonesDrawer> bonesDrawers = InstanceTracker.GetInstancesList<ModelBonesDrawer>();
                for (int i = bonesDrawers.Count - 1; i >= 0; i--)
                {
                    GameObject.Destroy(bonesDrawers[i]);
                }
            }
        }

        [SystemInitializer]
        static void Init()
        {
            On.RoR2.CharacterModel.Awake += CharacterModel_Awake;
        }

        static void CharacterModel_Awake(On.RoR2.CharacterModel.orig_Awake orig, CharacterModel self)
        {
            orig(self);

            if (_drawModelBones)
            {
                self.gameObject.AddComponent<ModelBonesDrawer>();
            }
        }

        class ModelBonesDrawer : MonoBehaviour
        {
            static readonly Mesh _lineMesh;

            static ModelBonesDrawer()
            {
                using WireMeshBuilder meshBuilder = new WireMeshBuilder();
                meshBuilder.AddLine(Vector3.zero, Color.yellow, Vector3.forward, Color.yellow);

                _lineMesh = meshBuilder.GenerateMesh();
            }

            readonly List<DebugOverlay.MeshDrawer> _activeMeshDrawers = [];

            CharacterModel _model;

            void Awake()
            {
                _model = GetComponent<CharacterModel>();

                BonesProvider bonesProvider = _model.GetComponent<BonesProvider>();
                if (bonesProvider)
                {
                    foreach (Bone bone in bonesProvider.Bones)
                    {
                        switch (bone.Info.Type)
                        {
                            case BoneType.Root:
                                continue;
                        }

                        Transform boneTransform = bone.BoneTransform;
                        if (!boneTransform)
                            continue;

                        int childCount = boneTransform.childCount;
                        for (int i = 0; i < childCount; i++)
                        {
                            Transform child = boneTransform.GetChild(i);

                            DebugOverlay.MeshDrawer meshDrawer = new DebugOverlay.MeshDrawer(boneTransform)
                            {
                                mesh = _lineMesh,
                                hasMeshOwnership = false
                            };

                            Vector3 localPosition = child.localPosition;
                            float childDistanceFromBone = localPosition.magnitude;

                            meshDrawer.transform.localPosition = Vector3.zero;
                            meshDrawer.transform.localRotation = Util.QuaternionSafeLookRotation(localPosition);
                            meshDrawer.transform.localScale = Vector3.one * childDistanceFromBone;

                            _activeMeshDrawers.Add(meshDrawer);
                        }
                    }
                }

                InstanceTracker.Add(this);
            }

            void OnDestroy()
            {
                InstanceTracker.Remove(this);

                foreach (DebugOverlay.MeshDrawer meshDrawer in _activeMeshDrawers)
                {
                    if (!meshDrawer.gameObject)
                        continue;

                    meshDrawer.Dispose();
                }

                _activeMeshDrawers.Clear();
            }
        }
    }
}
