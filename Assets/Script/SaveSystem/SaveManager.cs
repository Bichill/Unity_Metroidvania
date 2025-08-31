using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;

    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;

    // 公共屬性，用於外部訪問 gameData
    public GameData GameData => gameData;

    [ContextMenu("Delete save file")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        //根据游戏文件路径（游戏存档）和文件名创建数据处理器
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        // 获取所有实现了 ISaveManager 接口的 MonoBehaviour 对象
        saveManagers = FindAllSaveManagers();

        LoadGame();
    }



    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        //获取存档文件并且返回一个纯数据类（总表）
        gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data found,Creat a New GameData");
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        if (this.gameObject != null)
            Debug.Log("Save");

        dataHandler.Save(gameData);
    }

    //游戏退出自动调用
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public bool HasSaveData()
    {
        if (dataHandler.Load() != null)
        {
            return true;
        }
        return false;
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        // 获取所有实现了 ISaveManager 接口的 MonoBehaviour 对象
        // FindObjectsOfType<MonoBehaviour>(true)中添加true可以使其捕获到未激活的Object
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }
}
