using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, Subcriber
{
    private static SceneLoader instance;

    public static SceneLoader getInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<SceneLoader>();
        }
        return instance;
    }

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] public Vector3 playerPos;
    [SerializeField] public Player player;
    [SerializeField] public Animator ani;

    [Header("----------Time----------")]
    [SerializeField] float timeChangeScene;
    [SerializeField] float timeSetPlayerPos;
    float timeStart = 0f;
    bool isChangeScene = false;
    bool isSetplayerPos = false;

    public int sceneNumber;

    [Header("----------Hollow Shade-----------")]
    [SerializeField] HollowShade hollowShadeFrefabs;

    private void Start()
    {
        if(player == null)
        {
            player = GameObject.FindObjectOfType<Player>();
        }

        sceneNumber = SceneManager.GetActiveScene().buildIndex;
        playerPos = player.transform.position;
        checkHollowShade();
    }

    private void Update()
    {
        if(isChangeScene)
        {
            if (timeStart > timeChangeScene)
            {
                isChangeScene = false;
                isSetplayerPos = true;

                SceneManager.LoadScene(sceneNumber);

                timeStart = 0f;
            }
            else timeStart += Time.deltaTime;
        }

        if(isSetplayerPos)
        {
            if (timeStart > timeSetPlayerPos)
            {
                isSetplayerPos = false;
                setPlayerPosition();
                checkHollowShade();
                timeStart = 0f;
            }
            else timeStart += Time.deltaTime;
        }
    }

    public void loadScene(int sceneNumber, Vector3 playerPos)
    {
        ani.gameObject.SetActive(true);
        ani.Play("Sceneloader_START_CHANGE_SCENE");
        this.sceneNumber = sceneNumber;
        this.playerPos = playerPos;
        isChangeScene=true;
        timeStart = 0f;
    }

    public void loadSceneCurrent()
    {
        ani.gameObject.SetActive(true);
        ani.Play("Sceneloader_START_CHANGE_SCENE");
        isChangeScene = true;
        timeStart = 0f;
    }

    public void setPlayerPosition()
    {
        player.transform.position = playerPos;
        if(player.isDead)
        {
            player.Revial();
        }
    }

    public void checkHollowShade()
    {
        // check hollow shade
        HollowShadeData data = SaveLoadSystem.LoadHollowShadeData();
        if (data != null)
        {
            if(data.sceneNumber == this.sceneNumber)
            {
                HollowShade obj = Instantiate(hollowShadeFrefabs, new Vector3(data.position[0], data.position[1], data.position[2]), Quaternion.identity);
                obj.coin = data.coin;
                obj.soul = data.soul;
            }
        }
    }

    public void update(int state)
    {
        if (state == (int)Game_State.BacktoMenu)
        {
            Destroy(this.gameObject);
        }
    }
}
