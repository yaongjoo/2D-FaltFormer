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
        rigid = GetComponent<Rigidbody2D>(); // 초기화
        spriteRenderer = GetComponent<SpriteRenderer>(); // 초기화
        anim = GetComponent<Animator>(); // 초기화
    }

    void Start()
    {

    }

    // 단발적인 키 업데이트는 notmalized를 이용한다

    void Update() //단발적인 업데이트
    {
        //jump

        if ((Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))) //Get으로 쓰면 파라미터 값을 불러올 수 있음
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }
        if (Input.GetButtonUp("Horizontal"))
        {
            //Stop speed
            //normalized : 벡터 크기를 1로 만든 상태(단위 벡터), 단위를 구할 때 사용(방향)
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y); //소수점 곱할 때는 f붙여주기
        }

        //Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
            // -1이면 true, 1이면 false
        }

        //Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.3) //절대값을 써야됨 (0.3보다 작으면 멈춘 것으로 간주)
        {
            anim.SetBool("isWalking", false); //멈췄을 때
        }
        else
        {
            anim.SetBool("isWalking", true); //걸을 때
        }
    }
    void FixedUpdate() //1초에 50번 정도 실행
    {
        //Move speed
        //Move By Control
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * 5, ForceMode2D.Impulse); //AddForce : 힘을 가하는 것, Impulse : 순간적인 힘을 가함

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
        if (rigid.velocity.y < 0) //떨어질 때만 raycast를 쏴서 땅에 닿았는지 확인
        {
            Debug.DrawRay(rigid.position, Vector3.down/*쏘는 방향*/, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1/*거리*/, LayerMask.GetMask("Platform")/*플랫폼 레이어에만 닿겠습니다*/);
            if (rayHit.collider != null) //맞지 않으면 콜라이더도 없다
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
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //투명도 조절
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


