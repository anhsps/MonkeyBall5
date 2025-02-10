using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private bool moveRight = true;
    private bool isTurning, state, isGrounded;
    private float rotationX = 0f;

    [SerializeField] private float speed = 2.5f;
    [SerializeField] private Sprite sr1_1, sr1_2, sr2_1, sr2_2, sr3_1, sr3_2;

    private readonly string[] layerNames = { "Finish", "Switch1", "Switch2", "Switch3", "Switch4" };

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();

        if (!isTurning)
        {
            Move();
            if (isGrounded) CheckWall();
        }

        UpdateAnimation();
        HandleInput();
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveRight ? speed : -speed, rb.velocity.y);
        transform.rotation = Quaternion.Euler(rotationX, moveRight ? 0 : 180, 0);
    }

    private void CheckWall()
    {
        Vector2 dir = moveRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.5f, LayerMask.GetMask("Wall"));

        if (hit.collider != null)
            StartCoroutine(TurnDelay());
    }

    private void CheckGround()
    {
        Vector2 groundDir = rotationX % 360 == 0 ? Vector2.down : Vector2.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, groundDir, 1f, LayerMask.GetMask("Wall", "Bridge"));
        isGrounded = hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 dir = moveRight ? Vector2.right : Vector2.left;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)dir * 0.5f);

        Gizmos.color = Color.blue;
        Vector3 groundDir = rotationX % 360 == 0 ? Vector3.down : Vector3.up;
        Gizmos.DrawLine(transform.position, transform.position + groundDir * 1f);
    }

    private IEnumerator TurnDelay()
    {
        isTurning = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        moveRight = !moveRight;
        isTurning = false;
    }

    private void UpdateAnimation()
    {
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;
        animator.SetBool("Run", isMoving);
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && state)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            foreach (var layerName in layerNames)
            {
                Collider2D hit = Physics2D.OverlapPoint(mousePos, LayerMask.GetMask(layerName));
                if (hit != null)
                {
                    animator.SetTrigger("On");
                    SoundManager5.instance.PlaySound(5);

                    if (layerName == "Finish")
                        GameManager5.instance.GameWin();
                    else if (layerName == "Switch4")
                    {
                        rb.gravityScale *= -1;
                        rotationX += 180f;
                    }
                    else SwitchSprite(layerName);
                }
            }
        }
    }

    private void SwitchSprite(string layerName)
    {
        string tag = layerName.Replace("Switch", "SR");
        GameObject[] parentObjects = GameObject.FindGameObjectsWithTag(tag);

        foreach (var parent in parentObjects)
        {
            SpriteRenderer[] childSRs = parent.GetComponentsInChildren<SpriteRenderer>();

            foreach (var sr in childSRs)
            {
                sr.sprite = ChangeSprite(sr.sprite);
            }

            Collider2D parentCollider = parent.GetComponent<Collider2D>();
            parentCollider.enabled = !parentCollider.enabled;
        }
    }

    private Sprite ChangeSprite(Sprite sprite)
    {
        if (sprite == sr1_1) return sr1_2;
        if (sprite == sr1_2) return sr1_1;
        if (sprite == sr2_1) return sr2_2;
        if (sprite == sr2_2) return sr2_1;
        if (sprite == sr3_1) return sr3_2;
        if (sprite == sr3_2) return sr3_1;
        return sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap"))
        {
            SoundManager5.instance.PlaySound(4);
            GameManager5.instance.GameLose();
        }

        HandleTrigger(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        HandleTrigger(collision, false);
    }

    private void HandleTrigger(Collider2D collision, bool state)
    {
        foreach (var layerName in layerNames)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                this.state = state;
                collision.gameObject.GetComponentInChildren<Animator>().SetBool("Run", state);// arrow
                break;
            }
        }
    }
}
