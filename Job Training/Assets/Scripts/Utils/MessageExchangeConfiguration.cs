using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MessageExchangeConfiguration
{
    public void SendMesage() {
        if (MagicRoomManager.instance != null)
        {
            MagicRoomManager.instance.ExperienceManagerComunication.SendMessage(CreateObject());
        }
    }

    private JObject CreateObject()
    {
        JObject obj = new JObject();
        Type t = this.GetType();
        foreach (PropertyInfo p in t.GetProperties())
        {
            if (!p.GetValue(this).GetType().IsArray)
            {
                obj[p.Name] = p.GetValue(this).ToString();
            }
            else {
                Array a = (Array)p.GetValue(this);
                JArray x = new JArray();

                if (p.GetValue(this).GetType().GetElementType() == typeof(Boolean)) {
                    foreach (var el in a)
                    {
                        x.Add(bool.Parse(el.ToString()));
                    }
                }
                if (p.GetValue(this).GetType().GetElementType() == typeof(float))
                {
                    foreach (var el in a)
                    {
                        x.Add(float.Parse(el.ToString()));
                    }
                }
                if (p.GetValue(this).GetType().GetElementType() == typeof(int))
                {
                    foreach (var el in a)
                    {
                        x.Add(int.Parse(el.ToString()));
                    }
                }
                if (p.GetValue(this).GetType().GetElementType() == typeof(string))
                {
                    foreach (var el in a)
                    {
                        x.Add(el.ToString());
                    }
                }

                obj[p.Name] = x;
            }
        }
        return obj;
    }

}


public class MagicRoomInitMessage : MessageExchangeConfiguration { 
    public int var1 { get; set; }
    public bool[] var2 { get; set; }
}
