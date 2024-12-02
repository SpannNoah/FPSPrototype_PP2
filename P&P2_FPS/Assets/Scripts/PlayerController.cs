using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private CharacterController m_controller = null;
    [SerializeField]
    private LayerMask m_ignoreMask = 0;

    [Space][Header("Movement Settings")]
    [SerializeField]
    private float m_speed = 10.0f;
    [SerializeField]
    private float m_sprintModifier = 2.0f;
    [SerializeField]
    private int m_jumpMax = 2;
    [SerializeField]
    private int m_jumpSpeed = 10;
    [SerializeField]
    private int m_gravity = 5;

    [Space][Header("Shooting Settings")]
    [SerializeField]
    private int m_shootDamage = 25;
    [SerializeField]
    private int m_shootDistance = 50;
    [SerializeField]
    private int m_fireRate = 20;

    private Vector3 m_moveDir = Vector3.zero;
    private Vector3 m_playerVelocity = Vector3.zero;
    private int m_jumpCount = 0;
    private bool m_isSprinting = false;
    private bool m_isShooting = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * m_shootDistance, Color.red);
        Move();
        Sprint();
    }

    private void Move()
    {
        if(m_controller.isGrounded)
        {
            m_jumpCount = 0;
            m_playerVelocity = Vector3.zero;
        }

        m_moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        m_controller.Move(m_moveDir * m_speed * Time.deltaTime);

        Jump();

        m_controller.Move(m_playerVelocity * Time.deltaTime);
        m_playerVelocity.y -= m_gravity * Time.deltaTime;

        if((m_controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            m_playerVelocity.y -= m_jumpSpeed;
        }

        if(Input.GetButton("Fire1") && !m_isShooting)
        {
            StartCoroutine(ShootingCoroutine());
        }
    }

    private void Jump()
    {
        if(Input.GetButtonDown("Jump") && m_jumpCount < m_jumpMax)
        {
            m_jumpCount++;
            m_playerVelocity.y = m_jumpSpeed;
        }
    }

    private void Sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            m_speed *= m_sprintModifier;
            m_isSprinting = true;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            m_speed /= m_sprintModifier;
            m_isSprinting = false;
        }
    }

    private IEnumerator ShootingCoroutine()
    {
        m_isShooting = true;
        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, m_shootDistance, ~m_ignoreMask))
        {
            IDamage damage;

            if(hit.collider.TryGetComponent<IDamage>(out damage))
            {
                damage.TakeDamage(m_shootDamage);
            }
        }
        yield return new WaitForSeconds(m_fireRate);
        m_isShooting = false;
    }
}
