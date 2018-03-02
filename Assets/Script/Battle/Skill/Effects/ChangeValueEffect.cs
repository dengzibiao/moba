using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

public class ChangeValueEffect : BattleEffect
{
    public object property;
    public string mProperName;
    public object newValue;
    public SkillEffectNode EffectNode;
    object oldValue;
    int last;
    int repeat;
    string mProperExpression;
    BattleAgent Caster;
    BattleAgent Target;

    public ChangeValueEffect ( SkillEffectNode effectNode)
    {
        EffectNode = effectNode;
    }

    public void Cast ( BattleAgent target, BattleAgent caster )
    {
        if(!CheckBACondition(target))
            Reverse();

        Target = target;
        Caster = caster;
        ParseEffectExpression(EffectNode.config);

        if(repeat > 0)
        {
            if(last > 0)
            {
                CDTimer.GetInstance().AddCD(last, InvokeRep, Mathf.FloorToInt(1f * last/repeat));
            }
            else
            {
                Debug.LogError("CONFIG ERROR: last time should be more than 0");
            }
        }
        else
        {
            if(last > 0)
                CDTimer.GetInstance().AddCD(last, ReverseOldValue); ;
        }
        ChangeProperValue(Target, mProperName, newValue);
    }

    void ParseEffectExpression ( string effectExpression )
    {
        string[] effectParams = effectExpression.Split('|');
        if(effectParams.Length < 2)
        {
            Debug.LogError("CONFIG ERROR: effect param count less than 2");
        }
        if(effectParams.Length > 2)
        {
            if(effectParams[2].Contains("*"))
            {
                string[] lastParams = effectParams[2].Split('*');
                last = int.Parse(lastParams[0]);
                repeat = int.Parse(lastParams[1]);
            }
            else {
                last = int.Parse(effectParams[2]);
            }
        }
        mProperName = effectParams[0];
        mProperExpression = effectParams[1];
        if(mProperExpression.StartsWith("#"))
        {
            newValue = mProperExpression.Substring(1);
        } else
        {
            newValue = CalculateExpression(mProperExpression);
        }
    }

    Stack<char> Operators = new Stack<char>();
    Stack<float> Operands = new Stack<float>();

    public float CalculateExpression (string inputString)
    {
        //Debug.Log("CalculateExpression： " + inputString);
        Operators.Clear();
        Operands.Clear();
        string numString = "";
        inputString = Regex.Replace(inputString, @"\s", "");
        inputString += "#";
        int i = 0;
        while(inputString[i] != '#')
        {
            if(!isOperatorChar(inputString[i]))
            {
                numString += inputString[i];
            }
            else if(inputString[i] == '(')
            {
                Operators.Push(inputString[i]);
                //Debug.Log("Opt.Push " + inputString[i]);
            }
            else if(inputString[i] == ')')
            {
                if(numString.Length > 0)
                {
                    Operands.Push((float)GetPropertyNumOrNum(numString));
                    //Debug.Log("Operands.Push " + numString);
                    numString = "";
                }
                while(Operators.Peek() != '(')
                {
                    Operands.Push(Calculate(Operators.Pop()));
                }
                Operators.Pop();
            }
            else
            {
                if(numString.Length>0)
                {
                    Operands.Push((float)GetPropertyNumOrNum(numString));
                    //Debug.Log("Operands.Push " + numString);
                    numString = "";
                }
                if(Operators.Count == 0)
                {
                    Operators.Push(inputString[i]);
                    //Debug.Log("Count is 0 and Opt.Push " + inputString[i]);
                }
                else
                {
                    if(Level(inputString[i]) <= Level(Operators.Peek()))
                        Operands.Push(Calculate(Operators.Pop()));
                    Operators.Push(inputString[i]);
                    //Debug.Log("Opt.Push " + inputString[i]);
                }
            }
            i++;
        }
        if(numString.Length > 0)
        {
            Operands.Push((float)GetPropertyNumOrNum(numString));
            //Debug.Log("Operands.Push " + numString);
            numString = "";
        }

        while(Operators.Count>0)
        {
            Operands.Push(Calculate(Operators.Pop()));
        }

        return Operands.Peek();
    }

    int Level ( char m )
    {
        switch(m)
        {
            case '+':
            case '-':
                return 1;
            case '*':
            case '/':
                return 2;
            case '(':
            case ')':
                return 0;
            case '#':
                return -1;
            default:
                return 0;
        }
    }

    float Calculate (char opt)
    {
        float a = Operands.Pop();
        float b = Operands.Pop();
        //Debug.Log("Calculate " + a + opt + b);
        switch(opt)
        {
            case '+':
                return a + b;
            case '-':
                return b - a;
            case '*':
                return a * b;
            case '/':
                return b / a;
            default:
                return 0f;
        }
    }

    bool isOperatorChar ( char c )
    {
        return c == '+' || c == '-' || c == '*' || c == '/' || c == '(' || c == ')';
    }

    bool isNumChar ( char c )
    {
        return c > 45 && c < 58 && c != 47;
    }

    object GetPropertyNumOrNum ( string s )
    {
        if(isNumChar(s[0]))
            return Convert.ChangeType(s, typeof(float));
        if(s.StartsWith("$"))
        {
            return Convert.ChangeType(GetProperValue(Caster, s.Substring(1)), typeof(float));
        }
        else
        {
            return Convert.ChangeType(GetProperValue(Target, s), typeof(float));
        }
    }

    void InvokeRep (int count, long id)
    {
        newValue = CalculateExpression(mProperExpression);
        ChangeProperValue(Target, mProperName, newValue);
    }

    void ReverseOldValue ( int count, long id )
    {
        ChangeProperValue(Target, mProperName, oldValue);
    }

    public void Reverse ()
    {
        property = oldValue;
    }

    bool CheckBACondition ( BattleAgent target )
    {
        return true;
    }

    void ChangeProperValue ( BattleAgent target, string properName, object newValue)
    {
        Type t = target.GetType();
        PropertyInfo pi = t.GetProperty(properName);
        if(pi != null)
        {
            oldValue = pi.GetValue(target, null);
            pi.SetValue(target, Convert.ChangeType(newValue, pi.PropertyType), null);
        } else
        {
            FieldInfo fi = t.GetField(properName);
            if(fi != null)
            {
                oldValue = fi.GetValue(target);
                fi.SetValue(target, Convert.ChangeType(newValue, fi.FieldType));
            }
            else
            {
                Debug.LogError("CONFIG ERROR: There is no property or field [ " + properName + " ] in class >" + t);
            }
        }
    }

    object GetProperValue ( BattleAgent ba, string properName)
    {
        Type t = ba.GetType();
        PropertyInfo pi = t.GetProperty(properName);
        if(pi != null)
        {
            return pi.GetValue(ba, null);
        }
        else
        {
            FieldInfo fi = t.GetField(properName);
            if(fi != null)
            {
                return fi.GetValue(ba);
            }
            else
            {
                Debug.LogError("CONFIG ERROR: There is no property or field [ " + properName + " ] in class >" + t);
                return null;
            }
        }
    }

    public int EffectTiming ()
    {
        return EffectNode.effectTiming;
    }

    public SkillEffectNode GetNode ()
    {
        return EffectNode;
    }
}