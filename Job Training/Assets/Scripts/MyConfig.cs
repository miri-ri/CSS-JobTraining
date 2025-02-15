using System;

[Serializable]
public class MyConfig : GameConfiguration
{


    public override string ToString()
    {
        return "my configuration";
    }
}

public enum myenum
{
    val1, val2, val3
}