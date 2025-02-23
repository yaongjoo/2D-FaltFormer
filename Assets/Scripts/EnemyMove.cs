using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    public int nextMove; //1: 오른쪽, -1: 왼쪽 // 행동지표를 결정할 변수 생성


    void Awake()
    {

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();

        Invoke("Think", 0.5f); //5초 뒤에 Think 함수 실행

    }


    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y/*현재 가지고 있는 벡터 값*/);

        //Platform Check    
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y); //앞의 지형물을 예측해야 함, 앞에 있는 벡터 값
        Debug.DrawRay(frontVec, Vector3.down/*쏘는 방향*/, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1/*거리*/, LayerMask.GetMask("Platform")/*플랫폼 레이어에만 닿겠습니다*/);
        if (rayHit.collider == null) //
            Turn();

    }

    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2); //(최소값(최소는 랜덤 값에 포함이 됨), 최대값(최대는 랜덤 값에 포함이 되지 않음)) -1, 0, 1 중에 랜덤으로 나옴

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0) //멈추는 경우가 아니면
            spriteRenderer.flipX = nextMove == 1; //1이면 true

        //Think(); 재귀함수(에러 발생 가능성 있음)
        float nextThinkTime = Random.Range(2f, 4f); //2초에서 4초 사이에 랜덤으로 나옴
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1; //방향 바꾸기
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke("Think"); //Think 함수 취소
        Invoke("Think", 1f); //2초 뒤에 Think 함수 실행
        Debug.Log(nextMove);

    }

    public void OnDamaged()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //투명도 조절
        //Sprote Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        GetComponent<Collider2D>().enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Destroy
        Invoke("DeActive", 3);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}

