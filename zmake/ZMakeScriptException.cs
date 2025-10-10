using Jint.Native;
using Jint.Runtime;

namespace ZMake;

public class ZMakeScriptException : Exception
{
    public ZMakeScriptException(string msg) : base(msg){}
    public ZMakeScriptException(string msg,Exception inner):base(msg,inner){}

    public static void ThrowIfArgumentsCountWrong(string funcName,JsValue[] args,int expected)
    {
        var got = args.Length;
        if (expected == got)
        {
            return;
        }
        throw new ZMakeScriptException($"zmake.{funcName} expect {expected} arguments but get {got}");
    }
    
    public static void ThrowIfArgumentsTypeWrong(string funcName,JsValue[] args,int index,Types expected)
    {
        var arg = args[index];
        if (arg.Type == expected)
        {
            return;
        }
        throw new ZMakeScriptException(
            $"zmake.{funcName} expect {expected} type at arguments[{index} but get {arg.Type}");
    }

    public static void ThrowIfArgumentNotMatch(string funcName, JsValue[] args, params Types[] types)
    {
        ThrowIfArgumentsCountWrong(funcName, args, types.Length);

        int index = 0;
        foreach (var arg in args)
        {
            ThrowIfArgumentsTypeWrong(funcName,args,index, types[index]);
            index += 1;
        }
    }
}