using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GamePlayerInfo : MonoBehaviour
{
    public static GamePlayerInfo instance
    {
        get
        {
            if (gamePlayerInfo == null)
            {
                gamePlayerInfo = FindObjectOfType<GamePlayerInfo>();
            }
            return gamePlayerInfo;
        }
    }

    private static GamePlayerInfo gamePlayerInfo;

    public int representativePlayer = -1;
    public List<Player> havePlayers = new List<Player>();
    public List<Player> usingPlayers = new List<Player>();

    public int cleardStage = 0;
    public string playername = "�̰���";

    [Space(10f)]
    [Header("Resource")]
    public int money = 10000;
    public int crystal = 10000;
    public int contractTicket = 10000;
    public int stamina = 0;
    public List<int> XpItem;

    [HideInInspector]
    public int IDcode = 0;
    [HideInInspector]
    public int PresetCode = 0;
    public List<List<float>> Presets;

    public List<int> tradeCenter = new List<int>();
    public DateTime lastRecruitTime = DateTime.MinValue;
    public bool isInit = false;

    private PlayerTable pt;
    [HideInInspector]
    public List<Sprite> itemSpriteList = new List<Sprite>();    
    private void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            usingPlayers.Add(new Player());
        }

        Presets = new List<List<float>>();
        for (int i = 0; i < 4; i++)
        {
            XpItem.Add(0);
            Presets.Add(new List<float>());
            for (int j = 0; j < 8; j++)
            {
                Presets[i].Add(-1f);
            }
        }
        for (int i = 0; i < 5; i++)
        {
            tradeCenter.Add(1);
        }
        for (int i = 0; i < 3; i++)
        {
            itemSpriteList.Add(Resources.Load<Sprite>(
              Path.Combine("Items", i.ToString())));
        }

        LoadFile();
    }

    private void Start()
    {
        pt = DataTableManager.instance.Get<PlayerTable>(DataType.Player);
    }

    public void SortPlayersWithLevel(bool Orderby)
    {
        if (!Orderby)
        {
            var sortedPeople = havePlayers.OrderBy(p => p.level).ThenBy(p => p.grade).ThenBy(p => p.name).ThenBy(p => p.ID);
            havePlayers = sortedPeople.ToList();
        }
        else
        {
            var sortedPeople = havePlayers.OrderByDescending(p => p.level).ThenByDescending(p => p.grade).ThenByDescending(p => p.name).ThenByDescending(p => p.ID);
            havePlayers = sortedPeople.ToList();
        }
    }

    public void SortPlayersWithGrade(bool Orderby)
    {
        if (!Orderby)
        {
            var sortedPeople = havePlayers.OrderBy(p => p.grade).ThenBy(p => p.name).ThenBy(p => p.level).ThenBy(p => p.ID);
            havePlayers = sortedPeople.ToList();
        }
        else
        {
            var sortedPeople = havePlayers.OrderByDescending(p => p.grade).ThenByDescending(p => p.name).ThenByDescending(p => p.level).ThenByDescending(p => p.ID);
            havePlayers = sortedPeople.ToList();
        }
    }

    public void SortPlayersWithID(bool Orderby)
    {
        if (!Orderby) 
        { 
            var sortedPeople = havePlayers.OrderBy(p => p.ID);
            havePlayers = sortedPeople.ToList();
        }
        else
        {
            var sortedPeople = havePlayers.OrderByDescending(p => p.ID);
            havePlayers = sortedPeople.ToList();
        }
        
    }

    public Player AddPlayer(int code)
    {
        Player newPlayer = new Player();
        if (pt == null)
        {
            pt = DataTableManager.instance.Get<PlayerTable>(DataType.Player);
        }
        PlayerInfo info = pt.playerDatabase[code];
        newPlayer.name = info.name;
        newPlayer.code = code;
        newPlayer.type = info.type;
        newPlayer.grade = info.grade;
        newPlayer.ID = IDPrinter();
        newPlayer.cost = info.cost;
        havePlayers.Add(newPlayer);
        return newPlayer;
    }

    public void RemoveUsePlayer(int slotIndex)
    {
        if (slotIndex > 7)
        {
            return;
        }
        Player newPlayer = new Player();
        usingPlayers[slotIndex] = newPlayer;
    }

    private float IDPrinter()
    {
        if (IDcode == int.MaxValue)
        {
            IDcode = 0;
        }
        float ID = 0.0000001f * IDcode;
        IDcode++;
        return ID;
    }

    public void LoadPreset()
    {
        LoadPreset(PresetCode);
    }
    public void LoadPreset(int index)
    {
        if (index > 4)
        {
            return;
        }

        List<float> newPreset = new List<float>();
        foreach (var item in usingPlayers)
        {
            newPreset.Add(item.ID);
        }
        Presets[PresetCode] = newPreset;

        if (PresetCode == index)
        {
            return;
        }

        PresetCode = index;
        List<float> presetCodeList = Presets[PresetCode];

        for (int i = 0; i < usingPlayers.Count; i++)
        {
            if (usingPlayers[i].code != -1)
            {
                havePlayers.Add(usingPlayers[i]);
                RemoveUsePlayer(i);
            }
        }

        int count = 0;
        List<Player> deletePlayer = new List<Player>();
        foreach (float presetCode in presetCodeList) 
        {
            if (presetCode != -1)
            {
                foreach (var havePlayer in havePlayers)
                {
                    if (havePlayer.ID == presetCode)
                    {
                        usingPlayers[count] = havePlayer;
                        deletePlayer.Add(havePlayer);
                        continue;
                    }
                }
            }
            count++;
        }

        foreach (var item in deletePlayer)
        {
            havePlayers.Remove(item);
        }
    }
    public bool AddMoney(int money, int crystal, int contractTicket)
    {
        if (this.money + money > 999999999||
            this.crystal + crystal > 999999999 ||
            this.contractTicket + contractTicket > 99999)
        {
            this.money = Mathf.Min(this.money+money, 999999999);
            this.money = Mathf.Min(this.crystal + crystal, 999999999);
            this.money = Mathf.Min(this.contractTicket + contractTicket, 99999);
            return false;
        }
        this.money += money;
        this.crystal += crystal;
        this.contractTicket += contractTicket;
        return true;
    }
    public bool UseMoney(int money, int crystal, int contractTicket)
    {
        if (this.money < money ||
            this.crystal < crystal ||
            this.contractTicket < contractTicket)
        {
            return false;
        }
        this.money -= money;
        this.crystal -= crystal;
        this.contractTicket -= contractTicket;
        return true;
    }

    public bool CheckMoney(int money, int crystal, int contractTicket)
    {
        if (this.money < money ||
            this.crystal < crystal ||
            this.contractTicket < contractTicket)
        {
            return false;
        }
        return true;
    }


    public List<Player> CopyOfSortPlayersWithLevel(bool Orderby)
    {
        if (!Orderby)
        {
            var sortedPeople = havePlayers.OrderBy(p => p.level).ThenBy(p => p.grade).ThenBy(p => p.name).ThenBy(p => p.ID);
            return sortedPeople.ToList();
        }
        else
        {
            var sortedPeople = havePlayers.OrderByDescending(p => p.level).ThenByDescending(p => p.grade).ThenByDescending(p => p.name).ThenByDescending(p => p.ID);
            return sortedPeople.ToList();
        }
    }

    public List<Player> CopyOfSortPlayersWithGrade(bool Orderby)
    {
        if (!Orderby)
        {
            var sortedPeople = havePlayers.OrderBy(p => p.grade).ThenBy(p => p.name).ThenBy(p => p.level).ThenBy(p => p.ID);
            return sortedPeople.ToList();
        }
        else
        {
            var sortedPeople = havePlayers.OrderByDescending(p => p.grade).ThenByDescending(p => p.name).ThenByDescending(p => p.level).ThenByDescending(p => p.ID);
            return sortedPeople.ToList();
        }
    }

    public List<Player> CopyOfSortPlayersWithID(bool Orderby)
    {
        if (!Orderby)
        {
            var sortedPeople = havePlayers.OrderBy(p => p.ID);
            return sortedPeople.ToList();
        }
        else
        {
            var sortedPeople = havePlayers.OrderByDescending(p => p.ID);
            return sortedPeople.ToList();
        }

    }

    public List<Player> GetUsingPlayers()
    {
        List<Player> list = new List<Player>();
        foreach (var item in usingPlayers)
        {
            if (item.code >= 0)
            {
                list.Add(item);
            }
        }
        return list;
    }

    public void AnalyzePlayer(Player player, int level, float xp, float maxXp)
    {
        foreach (var item in usingPlayers) 
        {
            if (item.code != -1 && item.ID == player.ID)
            {
                item.level = level;
                item.xp = xp;
                item.maxXp = maxXp;
                return;
            }
        }
        foreach (var item in havePlayers)
        {
            if (item.ID == player.ID)
            {
                item.level = level;
                item.xp = xp;
                item.maxXp = maxXp;
                return;
            }
        }
        Debug.Log("Can't find Char");
    }

    public bool GetXpItems(int one,int two, int three, int four)
    {

        if (XpItem[0] + one > 9999 ||
            XpItem[1] + two > 9999 ||
            XpItem[2] + three > 9999 ||
            XpItem[3] + four > 9999)
        {
            XpItem[0] = Math.Min(XpItem[0] + one , 9999);
            XpItem[1] = Math.Min(XpItem[1] + two, 9999);
            XpItem[2] = Math.Min(XpItem[2] + three, 9999);
            XpItem[3] = Math.Min(XpItem[3] + four, 9999);
            return false;
        }
        XpItem[0] += one;
        XpItem[1] += two;
        XpItem[2] += three;
        XpItem[3] += four;
        return true;
    }

    public void TrainPlayer(Player player, List<int>train,int potential)
    {
        foreach (var item in usingPlayers)
        {
            if (item.code != -1 && item.ID == player.ID)
            {
                item.training.AddRange(train);
                item.potential = potential;
                return;
            }
        }
        foreach (var item in havePlayers)
        {
            if (item.ID == player.ID)
            {
                item.training.AddRange(train);
                item.potential = potential;
                return;
            }
        }
        Debug.Log("Can't find Char");
    }

    public void BreakPlayer(Player player, List<Player> delete)
    {
        foreach (var item in delete)
        {
            if (usingPlayers.Contains(item))
            {
                RemoveUsePlayer(usingPlayers.IndexOf(item));
            }
            else if (havePlayers.Contains(item))
            {
                havePlayers.Remove(item);
            }
        }

        player.breakthrough++;
        player.maxLevel += 5;
        player.skillLevel = player.breakthrough switch
        {
            1 => 1,
            2 => 2,
            3 => 3,
            _ => 1,
        };
    }

    public void SetTradeCenter(List<int> setValue)
    {
        tradeCenter = setValue;
    }

    public void CheckRepresentPlayers()
    {
        List<Player> playerList = new List<Player>();
        playerList.AddRange(GetUsingPlayers());
        playerList.AddRange(havePlayers);
        List<int> distinctCodeList = playerList
  .Select(item => item.code)
  .Distinct()
  .ToList();

        if (!distinctCodeList.Contains(representativePlayer))
        {
            representativePlayer = -1;
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Saved!");
    }


    public void SaveFile()
    {
        var saveData = new SaveData();

        saveData.representativePlayer = representativePlayer;
        saveData.havePlayers = havePlayers;
        saveData.usingPlayers = usingPlayers;
        
        saveData.cleardStage = cleardStage;
        saveData.playername = playername;
        saveData.money  = money;
        saveData.crystal = crystal;
        saveData.contractTicket = contractTicket;
        saveData.stamina = stamina;
        saveData.XpItem = XpItem;
        saveData.IDcode = IDcode;
        saveData.PresetCode = PresetCode;
        saveData.tradeCenter = tradeCenter;
        saveData.lastRecruitTime = lastRecruitTime;
        saveData.Presets = Presets;

        var path = Path.Combine(Application.persistentDataPath, "savefile.json");
        var json = JsonConvert.SerializeObject(saveData,new PlayerConverter());
        Debug.Log(path);
        File.WriteAllText(path, json);
    }

    public void LoadFile()
    {
        var path = Path.Combine(Application.persistentDataPath, "savefile.json");
        if (!File.Exists(path))
        {
            isInit = true;
            return;
        }

        var json = File.ReadAllText(path);
        var saveData = JsonConvert.DeserializeObject<SaveData>(json, new PlayerConverter());

        representativePlayer = saveData.representativePlayer;
        havePlayers = saveData.havePlayers;

        int count = 0;
        foreach (var item in saveData.usingPlayers)
        {
            usingPlayers[count] = saveData.usingPlayers[count];
            count++;
        }

        cleardStage = saveData.cleardStage;
        playername = saveData.playername;
        money = saveData.money;
        crystal = saveData.crystal;
        contractTicket  = saveData.contractTicket;
        stamina = saveData.stamina;
        XpItem = saveData.XpItem;
        IDcode = saveData.IDcode;
        PresetCode = saveData.PresetCode;
        tradeCenter = saveData.tradeCenter;
        lastRecruitTime = saveData.lastRecruitTime;
        Presets = saveData.Presets;
    }
}
public class PlayerConverter : JsonConverter<Player>
{
    public override Player ReadJson(JsonReader reader, Type objectType, Player existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jobj = JObject.Load(reader);
        Player player = new Player();
        player.ID = (float)jobj["ID"];
        player.name = (string)jobj["name"];
        player.code = (int)jobj["code"];
        player.type = (int)jobj["type"];
        player.grade = (int)jobj["grade"];
        player.level = (int)jobj["level"];
        player.maxLevel = (int)jobj["maxLevel"];
        player.breakthrough = (int)jobj["breakthrough"];
        player.skillLevel = (int)jobj["skillLevel"];
        player.gearCode = (int)jobj["gearCode"];
        player.gearLevel = (int)jobj["gearLevel"];
        player.xp = (float)jobj["xp"];
        player.maxXp = (float)jobj["maxXp"];
        player.condition = (int)jobj["condition"];
        player.cost = (int)jobj["cost"];
        player.potential = (int)jobj["potential"];
        int trainingCount = (int)jobj["trainingCount"];
        player.training = new List<int>();
        for (int i = 0; i < trainingCount; i++)
        {
            player.training.Add((int)jobj[$"training{i}"]);
        }

        return player;
    }

    public override void WriteJson(JsonWriter writer, Player value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("ID");
        writer.WriteValue(value.ID);
        writer.WritePropertyName("name");
        writer.WriteValue(value.name);
        writer.WritePropertyName("code");
        writer.WriteValue(value.code);
        writer.WritePropertyName("type");
        writer.WriteValue(value.type);
        writer.WritePropertyName("grade");
        writer.WriteValue(value.grade);
        writer.WritePropertyName("level");
        writer.WriteValue(value.level);
        writer.WritePropertyName("maxLevel");
        writer.WriteValue(value.maxLevel);
        writer.WritePropertyName("breakthrough");
        writer.WriteValue(value.breakthrough);
        writer.WritePropertyName("skillLevel");
        writer.WriteValue(value.skillLevel);
        writer.WritePropertyName("gearCode");
        writer.WriteValue(value.gearCode);
        writer.WritePropertyName("gearLevel");
        writer.WriteValue(value.gearLevel);
        writer.WritePropertyName("xp");
        writer.WriteValue(value.xp);
        writer.WritePropertyName("maxXp");
        writer.WriteValue(value.maxXp);
        writer.WritePropertyName("condition");
        writer.WriteValue(value.condition);
        writer.WritePropertyName("cost");
        writer.WriteValue(value.cost);
        writer.WritePropertyName("potential");
        writer.WriteValue(value.potential);
        writer.WritePropertyName("trainingCount");
        writer.WriteValue(value.training.Count);
        for (int i = 0; i < value.training.Count; i++)
        {
            writer.WritePropertyName($"training{i}");
            writer.WriteValue(value.training[i]);
        }

        writer.WriteEndObject();
    }
}