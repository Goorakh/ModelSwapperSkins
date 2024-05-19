using ModelSwapperSkins.Utils;
using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    [DisallowMultipleComponent]
    public class MatchBoneTransform : MonoBehaviour
    {
        Transform _boneTransform;
        Bone _bone;
        public Bone Bone
        {
            get
            {
                return _bone;
            }
            set
            {
                if (_bone == value)
                    return;

                _bone = value;
                _boneTransform = _bone?.BoneTransform;
                _baseLocalScale = _boneTransform.localScale;

                recalculateOffsets();
            }
        }

        Transform _targetTransform;
        Bone _targetBone;
        public Bone TargetBone
        {
            get
            {
                return _targetBone;
            }
            set
            {
                if (_targetBone == value)
                    return;

                _targetBone = value;
                _targetTransform = _targetBone?.BoneTransform;

                recalculateOffsets();
            }
        }

        MatchBoneTransform _parentBone;
        public MatchBoneTransform ParentBone
        {
            get
            {
                return _parentBone;
            }
            set
            {
                if (_parentBone == value)
                    return;

                _parentBone = value;

                recalculateOffsets();
            }
        }

        float calculateTargetScaleMultiplier()
        {
            float targetBoneScale = _targetBone != null ? _targetBone.Info.Scale : 1f;
            float boneScale = _bone != null ? _bone.Info.Scale : 1f;

            return targetBoneScale / boneScale;
        }

        Vector3 _localTargetPositionOffset;
        Quaternion _localTargetRotationOffset;
        float _localTargetScaleMultiplier;

        Vector3 _baseLocalScale;

        void OnEnable()
        {
            recalculateOffsets();
        }

        void recalculateOffsets()
        {
            if (_targetBone != null && _bone != null)
            {
                _localTargetPositionOffset = _targetBone.Info.PositionOffset - _bone.Info.PositionOffset;
                _localTargetRotationOffset = _targetBone.Info.RotationOffset * Quaternion.Inverse(_bone.Info.RotationOffset);

                _localTargetScaleMultiplier = calculateTargetScaleMultiplier();

                if (ParentBone)
                {
                    _localTargetScaleMultiplier /= ParentBone.calculateTargetScaleMultiplier();
                }
            }
            else
            {
                _localTargetPositionOffset = Vector3.zero;
                _localTargetRotationOffset = Quaternion.identity;
                _localTargetScaleMultiplier = 1f;
            }
        }

        void LateUpdate()
        {
            if (!_boneTransform || !_targetTransform)
                return;

#if DEBUG
            recalculateOffsets();
#endif

            _boneTransform.position = (Matrix4x4.Translate(_targetTransform.position) * (_targetTransform.localToWorldMatrix * Matrix4x4.Translate(_localTargetPositionOffset) * _targetTransform.worldToLocalMatrix)).GetColumn(3);

            Vector3 boneTargetForward = _targetTransform.TransformDirection(_localTargetRotationOffset * Vector3.forward);
            Vector3 boneTargetUp = _targetTransform.TransformDirection(_localTargetRotationOffset * Vector3.up);

            _boneTransform.rotation = Quaternion.LookRotation(boneTargetForward, boneTargetUp);

            _boneTransform.localScale = _baseLocalScale * _localTargetScaleMultiplier;
        }
    }
}
