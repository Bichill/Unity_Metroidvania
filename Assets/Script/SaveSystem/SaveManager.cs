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

    // �������ԣ�����ⲿ�L�� gameData
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
        //������Ϸ�ļ�·������Ϸ�浵�����ļ����������ݴ�����
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        // ��ȡ����ʵ���� ISaveManager �ӿڵ� MonoBehaviour ����
        saveManagers = FindAllSaveManagers();

        LoadGame();
    }



    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        //��ȡ�浵�ļ����ҷ���һ���������ࣨ�ܱ�
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

    //��Ϸ�˳��Զ�����
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
        // ��ȡ����ʵ���� ISaveManager �ӿڵ� MonoBehaviour ����
        // FindObjectsOfType<MonoBehaviour>(true)�����true����ʹ�䲶��δ�����Object
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }
}
