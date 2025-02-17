using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>(); // �ʱ�ȭ
        spriteRenderer = GetComponent<SpriteRenderer>(); // �ʱ�ȭ
        anim = GetComponent<Animator>(); // �ʱ�ȭ
    }

    void Start()
    {

    }

    // �ܹ����� Ű ������Ʈ�� notmalized�� �̿��Ѵ�

    void Update() //�ܹ����� ������Ʈ
    {
        //jump

        if ((Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))) //Get���� ���� �Ķ���� ���� �ҷ��� �� ����
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }
        if (Input.GetButtonUp("Horizontal"))
        {
            //Stop speed
            //normalized : ���� ũ�⸦ 1�� ���� ����(���� ����), ������ ���� �� ���(����)
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y); //�Ҽ��� ���� ���� f�ٿ��ֱ�
        }

        //Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            // -1�̸� true, 1�̸� false
        }

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3) //���밪�� ��ߵ� (0.3���� ������ ���� ������ ����)
        {
            anim.SetBool("isWalking", false); //������ ��
        }
        else
        {
            anim.SetBool("isWalking", true); //���� ��
        }
    }
    void FixedUpdate() //1�ʿ� 50�� ���� ����
    {
        //Move speed
        //Move By Control
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * 5, ForceMode2D.Impulse); //AddForce : ���� ���ϴ� ��, Impulse : �������� ���� ����

        //Max Speed 
        if (rigid.velocity.x > maxSpeed) //right
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1)) //left    
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        //Landing Platform
        if (rigid.velocity.y < 0) //������ ���� raycast�� ���� ���� ��Ҵ��� Ȯ��
        {
            Debug.DrawRay(rigid.position, Vector3.down/*��� ����*/, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1/*�Ÿ�*/, LayerMask.GetMask("Platform")/*�÷��� ���̾�� ��ڽ��ϴ�*/);
            if (rayHit.collider != null) //���� ������ �ݶ��̴��� ����
            {

                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("isJumping", false);
                }

            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
            {
                OnDamaged(collision.transform.position);
            }
        }


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            if (isBronze)
            {
                gameManager.stagePoint += 50;
            }
            else if (isSilver)
            {
                gameManager.stagePoint += 100;
            }
            else if (isGold)
            {
                gameManager.stagePoint += 300;
            }


            //Deactive Item
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Finish")
        {
            //Next Stage
            gameManager.NextStage();
        }

    }
    void OnAttack(Transform Enemy)
    {
        //Point
        gameManager.stagePoint += 100;

        //Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = Enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        //Health Down
        gameManager.HealthDown();

        //Change Layer (Immortal Active)    
        gameObject.layer = 11; //PlayerDamaged

        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("doDamaged");
        Invoke("offDamaged", 3);
    }

    void offDamaged()
    {
        gameObject.layer = 10; //Player 
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //���� ����
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        GetComponent<CapsuleCollider2D>().enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}


