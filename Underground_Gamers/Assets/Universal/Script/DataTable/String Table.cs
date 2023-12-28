using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public enum Language
{
    Kor,
    Eng,
    Count
}

public class StringTable : DataTable
{
    private List<Dictionary<string, string>> tables;

    public StringTable() : base(DataType.String) { }
    public override void DataAdder()
    {
        tables = new List<Dictionary<string, string>>
        {
            CSVReader.ReadStringTable(Path.Combine("CSV", "StringTable_kr")),
            CSVReader.ReadStringTable(Path.Combine("CSV", "StringTable_eng"))
        };
    }


    public string Get(string id)
    {
        return tables[GamePlayerInfo.instance.language][id];
    }

}
