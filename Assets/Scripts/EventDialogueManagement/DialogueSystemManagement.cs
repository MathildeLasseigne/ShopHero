using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
/// <summary>
/// Hold all the values for all the System Variables. Should be saved from one game to another
/// </summary>
public class DialogueSystemVariables
{
    public Dictionary<string, bool> boolVariables = new Dictionary<string, bool>();
    public Dictionary<string, int> intVariables = new Dictionary<string, int>();
    public Dictionary<string, string> stringVariables = new Dictionary<string, string>();

    public DialogueSystemVariables(DialogueSystemVariablesData data) {
        LoadNewVars(data);
    }

    public void LoadNewVars(DialogueSystemVariablesData data)
    {
        foreach (string var in data.boolNameVariables)
        {
            if (!boolVariables.ContainsKey(var))
            {
                boolVariables.Add(var, false);
            }
        }
        foreach (string var in data.stringNameVariables)
        {
            if (!stringVariables.ContainsKey(var))
            {
                stringVariables.Add(var, "");
            }
        }
        foreach (string var in data.intNameVariables)
        {
            if (!intVariables.ContainsKey(var))
            {
                intVariables.Add(var, 0);
            }
        }
    }
}

[Serializable]
/// <summary>
/// Contain the list of all the NAMES of the dialogue system variables. Should be a separate JSON.
/// Is used to setup DialogueSystemVariables above
/// @see DialogueSystemVariables
/// </summary>
public class DialogueSystemVariablesData
{
    public List<string> boolNameVariables = new List<string>();
    public List<string> intNameVariables = new List<string>();
    public List<string> stringNameVariables = new List<string>();
}

#region DialogueSystemVariable lists

[Serializable]
/// <summary>
/// Should be used at the end of an event to change the dialogue options.
/// Should be integrated in event json, in case of event fail or win
/// </summary>
public class DialogueSystemVariableModificationList
{
    public List<DialogueSystemVariableModificationBool> modificationsBool;
    public List<DialogueSystemVariableModificationString> modificationsString;
    public List<DialogueSystemVariableModificationInt> modificationsInt;

    public void ApplyModifications(DialogueSystemVariables gameVariables)
    {
        foreach (DialogueSystemVariableModificationBool mod in modificationsBool)
            mod.ApplyChangesToDic(gameVariables.boolVariables);

        foreach (DialogueSystemVariableModificationString mod in modificationsString)
            mod.ApplyChangesToDic(gameVariables.stringVariables);

        foreach (DialogueSystemVariableModificationInt mod in modificationsInt)
            mod.ApplyChangesToDic(gameVariables.intVariables);
    }
}

[Serializable]
/// <summary>
/// Should be used at the beginning of a dialogue json to check if the current dialogues options can start the following dialogue.
/// </summary>
public class DialogueSystemVariableConditionList
{
    public List<DialogueSystemVariableConditionBool> conditionsBool;
    public List<DialogueSystemVariableConditionString> conditionsString;
    public List<DialogueSystemVariableConditionInt> conditionsInt;

    public bool VerifyCondition(DialogueSystemVariables gameVariables)
    {
        foreach (DialogueSystemVariableConditionBool mod in conditionsBool)
        {
            if (!mod.ConditionVerify(gameVariables.boolVariables))
                return false;
        }
        foreach (DialogueSystemVariableConditionString mod in conditionsString)
        {
            if (!mod.ConditionVerify(gameVariables.stringVariables))
                return false;
        }
        foreach (DialogueSystemVariableConditionInt mod in conditionsInt)
        {
            if (!mod.ConditionVerify(gameVariables.intVariables))
                return false;
        }

        return true;
    }
}

#endregion


//Classes used in the above code. Will be JSONIfied automatically and should not be used anywhere except above

#region DialogueSystemVariableModification

[Serializable]
public  class DialogueSystemVariableModificationBool
{
    public string name;
    public bool newValue;

    public void ApplyChangesToDic(Dictionary<string, bool> dic)
    {
        dic[name] = newValue;
    }
}

[Serializable]
public class DialogueSystemVariableModificationString
{
    public string name;
    public string newValue;

    public void ApplyChangesToDic(Dictionary<string, string> dic)
    {
        dic[name] = newValue;
    }
}

[Serializable]
public class DialogueSystemVariableModificationInt
{
    public string name;
    public int addValue;

    public void ApplyChangesToDic(Dictionary<string, int> dic)
    {
        dic[name] += addValue;
    }
}

#endregion


#region DialogueSystemVariableCondition

[Serializable]
public class DialogueSystemVariableConditionBool
{
    public string name;
    public bool wantedValue;

    public bool ConditionVerify(Dictionary<string, bool> dic)
    {
        return dic[name] == wantedValue;
    }
}

[Serializable]
public class DialogueSystemVariableConditionString
{
    public string name;
    public string wantedValue;

    public bool ConditionVerify(Dictionary<string, string> dic)
    {
        return dic[name] == wantedValue;
    }
}

[Serializable]
public class DialogueSystemVariableConditionInt
{
    public string name;
    public int supOrEqual;
    public int strictInf;

    public bool ConditionVerify(Dictionary<string, int> dic)
    {
        return dic[name] >= supOrEqual && dic[name] < strictInf;
    }
}

#endregion
