using UnityEngine;

namespace ModelSwapperSkins.BoneMapping
{
    public enum DebugMatchMode : byte
    {
        None,
        MatchBoneToTransform,
        MatchTargetBoneToTransform
    }

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

        DebugMatchMode _debugMatchMode;

        float calculateTargetScaleMultiplier()
        {
            float targetBoneScale = _targetBone != null ? _targetBone.Info.Scale : 1f;
            float boneScale = _bone != null ? _bone.Info.Scale : 1f;

            return targetBoneScale / boneScale;
        }

#if DEBUG
        Vector3 _currentLocalPositionOffset;
#endif

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
                _localTargetRotationOffset = _targetBone.Info.RotationOffset * Quaternion.Inverse(_bone.Info.RotationOffset);

                _localTargetScaleMultiplier = calculateTargetScaleMultiplier();

                if (ParentBone)
                {
                    _localTargetScaleMultiplier /= ParentBone.calculateTargetScaleMultiplier();
                }
            }
            else
            {
                _localTargetRotationOffset = Quaternion.identity;
                _localTargetScaleMultiplier = 1f;
            }
        }

        void LateUpdate()
        {
            if (!_boneTransform || !_targetTransform)
                return;

#if DEBUG
            switch (_debugMatchMode)
            {
                case DebugMatchMode.None:
                    break;
                case DebugMatchMode.MatchBoneToTransform:
                {
                    _bone.Info.PositionOffset = -_boneTransform.InverseTransformVector(_boneTransform.parent.TransformVector(_boneTransform.localPosition - _boneTransform.parent.InverseTransformPoint(_targetTransform.TransformPoint(_targetBone.Info.PositionOffset))));

                    _boneTransform.localScale = _boneTransform.localScale.x * Vector3.one;

                    Vector3 scaleVector = Utils.VectorUtils.Divide(_boneTransform.localScale, _baseLocalScale);
                    float scaleFactor = scaleVector.x;

                    if (ParentBone)
                    {
                        scaleFactor *= ParentBone.calculateTargetScaleMultiplier();
                    }

                    _bone.Info.Scale = _targetBone.Info.Scale / scaleFactor;

                    Vector3 forward = _targetTransform.InverseTransformDirection(_boneTransform.rotation * Vector3.forward);
                    Vector3 up = _targetTransform.InverseTransformDirection(_boneTransform.rotation * Vector3.up);

                    Quaternion targetLocalRotationOffset = Quaternion.LookRotation(forward, up);

                    _bone.Info.RotationOffset = Quaternion.Inverse(targetLocalRotationOffset) * _targetBone.Info.RotationOffset;

                    return;
                }
                case DebugMatchMode.MatchTargetBoneToTransform:
                {
                    _targetBone.Info.PositionOffset = _targetTransform.InverseTransformPoint(_boneTransform.parent.TransformPoint(_boneTransform.localPosition - _boneTransform.parent.InverseTransformVector(_boneTransform.TransformVector(-_bone.Info.PositionOffset))));

                    _boneTransform.localScale = _boneTransform.localScale.x * Vector3.one;

                    Vector3 scaleVector = Utils.VectorUtils.Divide(_boneTransform.localScale, _baseLocalScale);
                    float scaleFactor = scaleVector.x;

                    if (ParentBone)
                    {
                        scaleFactor *= ParentBone.calculateTargetScaleMultiplier();
                    }

                    _targetBone.Info.Scale = scaleFactor * _bone.Info.Scale;

                    Vector3 forward = _targetTransform.InverseTransformDirection(_boneTransform.rotation * Vector3.forward);
                    Vector3 up = _targetTransform.InverseTransformDirection(_boneTransform.rotation * Vector3.up);

                    Quaternion targetLocalRotationOffset = Quaternion.LookRotation(forward, up);

                    _targetBone.Info.RotationOffset = targetLocalRotationOffset * _bone.Info.RotationOffset;

                    return;
                }
                default:
                    throw new System.NotImplementedException($"Match mode {_debugMatchMode} is not implemented");
            }

            recalculateOffsets();
#endif

            Vector3 boneTargetForward = _targetTransform.TransformDirection(_localTargetRotationOffset * Vector3.forward);
            Vector3 boneTargetUp = _targetTransform.TransformDirection(_localTargetRotationOffset * Vector3.up);

            _boneTransform.rotation = Quaternion.LookRotation(boneTargetForward, boneTargetUp);

            _boneTransform.localScale = _baseLocalScale * _localTargetScaleMultiplier;

            _boneTransform.localPosition = _boneTransform.parent.InverseTransformPoint(_targetTransform.TransformPoint(_targetBone.Info.PositionOffset))
                                           + _boneTransform.parent.InverseTransformVector(_boneTransform.TransformVector(-_bone.Info.PositionOffset));

#if DEBUG
            _currentLocalPositionOffset = _boneTransform.localPosition - _boneTransform.parent.InverseTransformPoint(_targetTransform.position);
#endif
        }
    }
}
