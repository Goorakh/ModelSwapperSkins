using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
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

        Vector3 _localTargetPositionOffset;
        Quaternion _localTargetRotationOffset;
        Vector3 _localTargetScaleMultiplier;

        void Awake()
        {
            recalculateOffsets();
        }

        void recalculateOffsets()
        {
            if (_targetBone != null && _bone != null)
            {
                _localTargetPositionOffset = _targetBone.Info.PositionOffset - _bone.Info.PositionOffset;
                _localTargetRotationOffset = _targetBone.Info.RotationOffset * Quaternion.Inverse(_bone.Info.RotationOffset);
            }
            else
            {
                _localTargetPositionOffset = Vector3.zero;
                _localTargetRotationOffset = Quaternion.identity;
                _localTargetScaleMultiplier = Vector3.one;
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

            Vector3 boneForward = _boneTransform.TransformDirection(_bone.Info.RotationOffset * Vector3.forward);
            Vector3 boneUp = _boneTransform.TransformDirection(_bone.Info.RotationOffset * Vector3.up);

            Vector3 boneTargetForward = _targetTransform.TransformDirection(_localTargetRotationOffset * Vector3.forward);
            Vector3 boneTargetUp = _targetTransform.TransformDirection(_localTargetRotationOffset * Vector3.up);

            _boneTransform.rotation = Quaternion.LookRotation(boneTargetForward, boneTargetUp);

            // _boneTransform.localScale = (Matrix4x4.Scale(_targetTransform.localScale) * (_targetTransform.localToWorldMatrix * Matrix4x4.Scale(_localTargetScaleMultiplier) * _targetTransform.worldToLocalMatrix)).lossyScale;
        }
    }
}
