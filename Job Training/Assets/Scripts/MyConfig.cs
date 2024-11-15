using System;

[Serializable]
public class MyConfig : GameConfiguration
{
    [PropertyRange(5, 30)]
    [PropertyRename("Questo intero è speciale")]
    public int myint { get; set; }

    [PropertyRange(0, 20)]
    public float myfloat { get; set; }

    [PropertyLimitedSet(new string[] { "mysample1", "mysample2" })]
    [PropertyDefaultValue("mysample2")]
    public string mysecondstring { get; set; }

    public string mystring { get; set; }

    [PropertyDefaultValue("sckca")]
    public string mystring3 { get; set; }

    [PropertyDefaultValue(true)]
    public bool mybool { get; set; }

    public bool mybool2 { get; set; }
    public myenum enumvar { get; set; }

    public override string ToString()
    {
        return "my configuration";
    }
}

public enum myenum
{
    val1, val2, val3
}