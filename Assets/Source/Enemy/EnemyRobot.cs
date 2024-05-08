using NetDive.Attackable;
using NetDive.Player;
using UnityEngine;
using UnityEngine.AI;

namespace NetDive.Enemy
{
    public class EnemyRobot : MonoBehaviour, IAttackable
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        private Transform _attackTarget = null;
        public float AttackThreshold = 1f;
        public float AttackCoolTime = 2f;
        private float _currentCoolTime = 0;
        private float _maxHp = 50f;
        private bool _isAlive = true;
        private float _currentHp;

        private static readonly int IsRunning = Animator.StringToHash("IsRunning");
        private static readonly int Punch = Animator.StringToHash("Punch");

        private void Start()
        {
            _currentHp = _maxHp;
        }

        private void Update()
        {
            if (_currentHp <= 0 && _isAlive)
            {
                _agent.enabled = false;
                _isAlive = false;
                _animator.SetTrigger("Die");

            }
            
            if (!_isAlive) return;
            
            if (_currentCoolTime > 0)
            {
                _currentCoolTime -= Time.deltaTime;
                return;
            }
            
            if (_attackTarget == null) return;

            if (Vector3.Distance(transform.position, _attackTarget.position) > AttackThreshold)
            {
                _animator.SetBool(IsRunning, true);
                _agent.destination = _attackTarget.position;
            }
            else
            {
                _animator.SetBool(IsRunning, false);
                _animator.SetTrigger(Punch);
                _currentCoolTime = AttackCoolTime;
                _agent.destination = transform.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerCharacterController>();

            if (player == null) return;

            _attackTarget = player.transform;
        }
        
        public void OnAttack(Transform attacker, float damage)
        {
            Debug.Log("hit");
            _currentHp -= damage;
            return;
        }
    }
}