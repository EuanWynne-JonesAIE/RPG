using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerence = 1f;

        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.8f;

        Mover mover;
        Fighter fighter;
        GameObject player;
        Health health;
        
        
        float normalSpeed;

        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;

        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float waypointDwellTime;
        int currentWaypointIndex = 0;

        private void Start()
        {
            normalSpeed = GetComponent<NavMeshAgent>().speed;
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();

            guardPosition = transform.position;
        }
        private void Update()
        {
            if (health.IsDead()) return;
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {

                //attack the player
                AttackBehaviour();

            }
            else if (timeSinceLastSawPlayer < suspicionTime && !health.IsDead())
            {
                //suspicion state
                SuspicionBehaviour();

            }
            else
            {
                //return to guardPosition
                PatrolBehaviour();
            }
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            StopSuspicion();
            fighter.Attack(player);
        }


        private void SuspicionBehaviour()
        {
            GetComponent<ActionSchedueler>().CancelCurrentAction();
            TriggerSuspicion();
            mover.Cancel();
        }


        private void PatrolBehaviour()
        {

            StopSuspicion();
            Vector3 nextPosition = guardPosition;
            if(patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if(timeSinceArrivedAtWaypoint > GetRandomDwellTime(waypointDwellTime))
            {
            mover.StartMoveAction(nextPosition,patrolSpeedFraction);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerence;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextWaypoint(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }


        private bool InAttackRangeOfPlayer()
        {
            GameObject player = GameObject.FindWithTag("Player");
            float  distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;

        }

        //Called by Unity to Draw chase distance sphere
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
           
        }

        private void TriggerSuspicion()
        {
            GetComponent<Animator>().ResetTrigger("StopSuspicion");
            GetComponent<Animator>().SetTrigger("Suspicious");
        }
        private void StopSuspicion()
        {
            GetComponent<Animator>().SetTrigger("StopSuspicion");
            GetComponent<Animator>().ResetTrigger("Suspicious");

        }

        private void ResetSuspicion()
        {
            GetComponent<Animator>().ResetTrigger("StopSuspicion");
            GetComponent<Animator>().ResetTrigger("Suspicious");
        }

        private float GetRandomDwellTime(float waypointDwellTime)
        {
           return waypointDwellTime = Random.Range(1, 5);
           
        }
    }



}
