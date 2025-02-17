using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{ //������ ���������� ����

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public TextMeshProUGUI UIPoint;
    public TextMeshProUGUI UIStage;

    public GameObject UIRestartBtn;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        //Change Stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else // gameclear
        {

            // Result UI
            Debug.Log("Game Clear!");
            // Restart Button UI
            //��ư �ؽ�Ʈ�� �ڽĿ�����Ʈ�̹Ƿ� InChildren�� ���
            TextMeshProUGUI btnText = UIRestartBtn.GetComponentInChildren<TextMeshProUGUI>(); // TextMeshPro ���
            btnText.text = "Clear!";
            UIRestartBtn.SetActive(true);
            // Player Control Lock
            Time.timeScale = 0;
        }

        //Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f); //UI Health�� �÷��� ��Ӱ� ����
        }
        else
        {
            //All Health UI Off 
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            //Player Die Effect
            player.OnDie();

            //Result UI
            Debug.Log("Player Die");

            //Retry Button UI
            UIRestartBtn.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Player Reposition
            if (health > 1)
                PlayerReposition();
            //Health Down
            HealthDown();

        }
    }

    public void PlayerReposition()
    {
        player.transform.position = new Vector3(-8, 3, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
