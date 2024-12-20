using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Main Enemy behavior script.
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// Current health of the Enemy.
    /// </summary>
    [ Header("Gameplay") ]
    public float health = 10.0f;

    /// <summary>
    /// Current speed of the Enemy.
    /// </summary>
    public float speed = 1.0f;
    
    /// <summary>
    /// Rigid body of the Enemy.
    /// </summary>
    private Rigidbody mRigidBody;
    
    /// <summary>
    /// Trigger of the Enemy.
    /// </summary>
    private BoxCollider mBoxTrigger;
    
    /// <summary>
    /// Collider of the Enemy.
    /// </summary>
    private BoxCollider mBoxCollider;
    
    private GameObject nearestTarget;

    /// <summary>
    /// Called when the script instance is first loaded.
    /// </summary>
    private void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
        
        // Get the collider and the trigger, making sure we got the correct one.
        var colliders = GetComponents<BoxCollider>();
        mBoxTrigger = colliders[0]; Assert.IsTrue(mBoxTrigger.isTrigger);
        mBoxCollider = colliders[1];
    }

    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    { SetupCollider(); }

    /// <summary>
    /// Update called at fixed time delta.
    /// </summary>
    void FixedUpdate()
    {
        /*
         * Task #1B: Implement the enemy functionality
         * Useful functions and variables:
         *  - Get nearest player to given position: GameManager.Instance.NearestPlayer(position)
         *    Can return null, which should be checked for!
         *  - Transform of the currently controlled enemy: transform
         *  - Physical body of the enemy: mRigidBody
         * Implement a simple AI, which will head towards the closest player and follow them.
         */
        // Получаем текущую позицию врага
        Vector3 currentPosition = mRigidBody.position;

        // Находим ближайшего игрока
        GameObject nearestTarget = GameManager.Instance.NearestPlayer(currentPosition);
        if (nearestTarget == null)
            return;

        // Получаем позицию игрока
        Vector3 playerPosition = nearestTarget.transform.position;

        // Вычисляем направление к игроку
        Vector3 direction = (playerPosition - currentPosition).normalized;

        // Вычисляем новую позицию для движения к игроку
        Vector3 newPosition = currentPosition + direction * speed * Time.fixedDeltaTime;

        // Перемещаем врага к новой позиции
        // mRigidBody.transform.rotation = Quaternion.LookRotation(direction);
        mRigidBody.transform.position = newPosition;
    }

    /// <summary>
    /// Triggered when a collision is detected.
    /// </summary>
    /// <param name="other">The other collidable.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        { DestroyEnemy(); }
    }

    /// <summary>
    /// Destroy this Enemy.
    /// </summary>
    public void DestroyEnemy()
    { Destroy(gameObject); }

    /// <summary>
    /// Setup collider for better collision detection.
    /// </summary>
    private void SetupCollider()
    {
        // Modify the rigid body to allow collisions on the y axis.
        var colliderCenter = mBoxCollider.center;
        mBoxCollider.center = new Vector3 {
            x = colliderCenter.x,
            y = colliderCenter.y, 
            z = transform.localScale.y, 
        };
        var colliderSize = mBoxCollider.size;
        mBoxCollider.size = new Vector3 {
            x = colliderSize.x,
            y = colliderSize.y, 
            //z = 1.0f / transform.localScale.y, 
            z = colliderSize.z, 
        };
    }
}
