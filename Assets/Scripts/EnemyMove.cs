using JetBrains.Annotations;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    public int nextMove; //1: ������, -1: ���� // �ൿ��ǥ�� ������ ���� ����


    void Awake()
    {

        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();

       Invoke("Think", 0.5f); //5�� �ڿ� Think �Լ� ����

    }


    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y/*���� ������ �ִ� ���� ��*/);

        //Platform Check    
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove *0.2f, rigid.position.y); //���� �������� �����ؾ� ��, �տ� �ִ� ���� ��
        Debug.DrawRay(frontVec, Vector3.down/*��� ����*/, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1/*�Ÿ�*/, LayerMask.GetMask("Platform")/*�÷��� ���̾�� ��ڽ��ϴ�*/);
        if (rayHit.collider == null) //
            Turn();
        
    }

    void Think()
    {
        //Set Next Active
        nextMove = Random.Range(-1, 2); //(�ּҰ�(�ּҴ� ���� ���� ������ ��), �ִ밪(�ִ�� ���� ���� ������ ���� ����)) -1, 0, 1 �߿� �������� ����

        //Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        //Flip Sprite
        if (nextMove != 0) //���ߴ� ��찡 �ƴϸ�
            spriteRenderer.flipX = nextMove == 1; //1�̸� true

        //Think(); ����Լ�(���� �߻� ���ɼ� ����)
        float nextThinkTime = Random.Range(2f, 4f); //2�ʿ��� 4�� ���̿� �������� ����
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1; //���� �ٲٱ�
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke("Think"); //Think �Լ� ���
        Invoke("Think", 1f); //2�� �ڿ� Think �Լ� ����
        Debug.Log(nextMove);

    }

    public void OnDamaged()
    {
        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); //���� ����
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

