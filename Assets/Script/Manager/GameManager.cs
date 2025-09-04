using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    public Transform player;

    [SerializeField] private CheckPoint[] checkpoits;
    private string closestCheckpointId;

    [Header("Lost Currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;
    public bool isPickedUpLostCurrency = true;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        player = PlayerManager.instance.player.transform;
    }

    private void Start()
    {
        // 根据CheckPoint类型，获取场景中所有的CheckPoint对象
        checkpoits = FindObjectsOfType<CheckPoint>();
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));

    public void SaveData(ref GameData _data)
    {
        SaveLostCurrency(_data);

        if (FindClosestCheckpoint() != null)
            _data.closestCheckpointId = FindClosestCheckpoint().checkPointId;

        _data.checkpoints.Clear();

        foreach (var checkpoint in checkpoits)
        {
            _data.checkpoints.Add(checkpoint.checkPointId, checkpoint.activationStatus);
        }

        _data.isPickedUpLostCurrency = isPickedUpLostCurrency;
    }


    // 获取最近保存节点
    private CheckPoint FindClosestCheckpoint()
    {
        float cloestDistance = Mathf.Infinity;
        CheckPoint cloestCheckpoint = null;

        foreach (var checkpoint in checkpoits)
        {
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);

            if (distanceToCheckpoint < cloestDistance && checkpoint.activationStatus == true)
            {
                cloestDistance = distanceToCheckpoint;
                cloestCheckpoint = checkpoint;
            }
        }
        return cloestCheckpoint;
    }
    
    private void SaveLostCurrency(GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;

        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;
    }

    private void LoadLostCurrrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount > 0 || !isPickedUpLostCurrency)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;//此时已经将丢失货币具现化，因此需要将损失货币归零
    }

    // 加载节点信息
    private void LoadCheckpoint(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (CheckPoint checkpoint in checkpoits)
            {
                if (pair.Key == checkpoint.checkPointId && pair.Value == true)
                {
                    checkpoint.ActiveCheckPoint();
                }
            }
        }
    }

    //初始化主角位置
    private void LoadPlayerToClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointId == null)
            return;

        closestCheckpointId = _data.closestCheckpointId;

        foreach (var checkpoint in checkpoits)
        {
            if (closestCheckpointId == checkpoint.checkPointId)
            {
                PlayerManager.instance.player.transform.position = checkpoint.transform.position;
            }
        }
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoint(_data);
        LoadPlayerToClosestCheckpoint(_data);
        LoadLostCurrrency(_data);

        isPickedUpLostCurrency = _data.isPickedUpLostCurrency;
    }
}
