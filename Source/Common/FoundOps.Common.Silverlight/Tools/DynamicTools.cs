using System;
using System.Dynamic;
using System.Reflection;

namespace FoundOps.Common.Silverlight.Tools
{
//    Following reason and example for use:
//<-----------------Example--------------------------->
//public class Foo1 {
//    public static string TransformString(string s) { return s.ToLower(); }
//    public static string MyConstant { get { return "Constant from Foo1"; } }
//}
//public class Foo2 {
//    public static string TransformString(string s) { return s.ToUpper(); }
//    public static string MyConstant { get { return "Constant from Foo2"; } }
//}

//    dynamic typeDynamic = new StaticMembersDynamicWrapper(type);
 
//    // Call TransformString("Hello World") on this type
//    Console.WriteLine(typeDynamic.TransformString("Hello World"));
 
//    // Get the MyConstant property on this type
//    Console.WriteLine(typeDynamic.MyConstant);
//<-----------------Example--------------------------->

public class StaticMembersDynamicWrapper : DynamicObject {
    private Type _type;
    public StaticMembersDynamicWrapper(Type type) { _type = type; }
 
    // Handle static properties
    public override bool TryGetMember(GetMemberBinder binder, out object result) {
        PropertyInfo prop = _type.GetProperty(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
        if (prop == null) {
            result = null;
            return false;
        }
 
        result = prop.GetValue(null, null);
        return true;
    }
 
    // Handle static methods
    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
        MethodInfo method = _type.GetMethod(binder.Name, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
        if (method == null) {
            result = null;
            return false;
        }
        result = method.Invoke(null, args);
        return true;
    }

}

}
