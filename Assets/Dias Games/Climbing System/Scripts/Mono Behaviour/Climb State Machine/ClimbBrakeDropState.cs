using System.Collections;
using System.Collections.Generic;
using DiasGames.Abilities;
using UnityEngine;

namespace DiasGames.Climbing
{
    [System.Serializable]
    public class ClimbBrakeDropState : ClimbStateBase
    {
        [SerializeField] private string brakeDrop = "Climb.Hop Drop"; // 使用与普通下降相同的动画，或创建专用动画
        [SerializeField] private float dropDuration = 0.3f;
        [SerializeField] private float particleLifetime = 1.0f;
        [SerializeField] private GameObject brakeParticlePrefab; // 刹车粉尘特效

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private float _startTime;
        private Transform _targetTransform; // 攀附目标transform
        private GameObject _currentParticle; // 当前粉尘特效实例

        public override void EnterState(ClimbStateContext context)
        {
            context.climb.StopAllCoroutines(); // 停止所有协程，避免干扰
            context.climb.BlockCurrentLedge(); // 阻止当前攀附点的使用
            if (_targetTransform == null)
            {
                Debug.LogWarning("刹车状态需要一个有效的目标Transform！");
                context.SetState(context.Idle);
                return;
            }

            // 播放刹车动画
            context.animator.CrossFadeInFixedTime(brakeDrop, 0.05f); // 更快的过渡时间，显得更紧急

            // 计算目标位置和旋转
            _targetPosition = GetCharacterPositionOnLedge(context, _targetTransform);
            _targetRotation = GetCharacterRotationOnLedge(_targetTransform);

            // 执行Tween
            context.climb.DoTween(_targetPosition, _targetRotation, dropDuration, _targetTransform.GetComponent<Collider>());

            // 设置手部IK
            SetHandIK(context);
            context.climb.StartCoroutine(ResetIK(context));

            // 生成刹车粉尘特效
            if (brakeParticlePrefab != null)
            {
                Vector3 particlePosition = _targetTransform.position;
                Quaternion particleRotation = Quaternion.LookRotation(-_targetTransform.forward);
                _currentParticle = GameObject.Instantiate(brakeParticlePrefab, particlePosition, particleRotation);
                
                // 计时销毁粒子
                GameObject.Destroy(_currentParticle, particleLifetime);
            }

            // 播放刹车音效
            AudioSource audioSource = context.transform.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f); // 随机音调变化
                audioSource.Play();
            }

            // 减少体力值（如果需要）
            PlayerPhysicalStrength.Instance?.ReducePhysicalStrength(5f);

            // 记录开始时间
            _startTime = Time.time;
            Debug.Log("刹车状态开始，目标位置：" + _targetPosition + "，目标旋转：" + _targetRotation);
        }

        public override void ExitState(ClimbStateContext context)
        {
            _startTime = 0;
            _targetTransform = null;
        }

        public override void Update(ClimbStateContext context)
        {
            // 可以在这里添加额外的更新逻辑，例如随时间变化的特效强度等
            
            // 添加相机轻微抖动效果
            // float intensity = Mathf.Lerp(0.3f, 0.0f, (Time.time - _startTime) / dropDuration);
            // //CameraShaker shaker = Camera.main?.GetComponent<CameraShaker>();
            // if (shaker != null && intensity > 0.05f)
            // {
            //     shaker.ShakeCamera(intensity, 0.05f);
            // }
            
            // // 如果粉尘特效存在，确保其位置正确
            // if (_currentParticle != null)
            // {
            //     _currentParticle.transform.position = _targetTransform.position;
            // }
        }

        private void SetHandIK(ClimbStateContext context)
        {
            // 设置手部IK目标为抓钉点
            if (_targetTransform != null)
            {
                Vector3 handPosition = _targetTransform.position + _targetTransform.forward * 0.1f;
                context.ik.SetLeftHandJumpEffector(handPosition);
                context.ik.SetRightHandJumpEffector(handPosition + Vector3.right * 0.2f);
            }
        }

        private IEnumerator ResetIK(ClimbStateContext context)
        {
            yield return new WaitForSeconds(dropDuration * 0.6f);
            context.ik.SetLeftHandIKTarget(ClimbIK.TargetHandIK.OnLedge);
            context.ik.SetRightHandIKTarget(ClimbIK.TargetHandIK.OnLedge);
        }

        public override void Idle(ClimbStateContext context)
        {
            // 等待动画完成后再转换到空闲状态
            if (Time.time - _startTime < dropDuration) return;

            context.SetState(context.Idle);
        }

        // 设置攀附目标
        public void SetTarget(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        //与ClimbDropState相同的位置计算逻辑
        private Vector3 GetCharacterPositionOnLedge(ClimbStateContext context, Transform targetTransform)
        {
            // 计算角色在攀爬点上的位置
            Vector3 normal = targetTransform.forward;
            normal.y = 0;
            normal.Normalize();
            Vector2 offsetOnLedge = context.climb.GetComponent<ClimbAbility>().offsetOnLedge;
            return targetTransform.position + Vector3.up * offsetOnLedge.y + normal * offsetOnLedge.x;
        }

        private Quaternion GetCharacterRotationOnLedge(Transform targetTransform)
        {
            Vector3 normal = targetTransform.forward;
            normal.y = 0;
            normal.Normalize();

            return Quaternion.LookRotation(-normal);
        }
    }
}