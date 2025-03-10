﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationManager : TabletHandlerManager
{
    private GameConfiguration g;

    protected override void HandlerConfiguration(JObject configuration)
    {
        Debug.Log("received config");
        try
        {
            g = GameSetting.instance.configuration;
            GameConfiguration.SetConfigFromObject(configuration.Value<JObject>("gameConfiguration"), g);
            List<Player> players = configuration.Value<JArray>("players").ToObject<List<Player>>();
            int sessionId = configuration.Value<int>("sessionId");
            MagicRoomManager.instance.Logger.SessionID = sessionId; 
            MagicRoomManager.instance.Logger.AddToLogNewLine("SessionId " + sessionId);
            MagicRoomManager.instance.Logger.AddToLogNewLine("Received configuration " + g.ToString());
            GameSetting.instance.SetConfiguration(g, players);
            Debug.Log(GameSetting.instance.configuration);
        }
        catch (Exception e)
        {
            Debug.Log("Parsing error in the configuration " + e.Message);
            MagicRoomManager.instance.ExperienceManagerComunication.sendConfigurationCheckMessage(false);
        }
    }
}