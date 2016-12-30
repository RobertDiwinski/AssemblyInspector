using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyInspector
{
   public static class Inspector
   {
      public static string GetDefinitionString(PropertyInfo pi)
      {
         StringBuilder ret = new StringBuilder();

         string modifier = null;
         string setModifier = null;

         if (pi.CanRead) modifier = GetModifier(pi.GetMethod.Attributes);
         if (pi.CanWrite) setModifier = GetModifier(pi.SetMethod.Attributes);

         if (modifier != null && setModifier != null) setModifier = GetModifier(pi.SetMethod.Attributes & ~pi.GetMethod.Attributes);

         if (modifier == null && setModifier != null)
         {
            modifier = setModifier;
            setModifier = null;
         }

         ret.Append(modifier);
         ret.Append(GetTypeName(pi.PropertyType));
         ret.Append(" ");
         ret.Append(pi.Name);
         ret.Append(" {");

         if (pi.CanRead) ret.Append(" get;");

         if (pi.CanWrite)
         {
            if (!string.IsNullOrEmpty(setModifier))
            {
               ret.Append(" ");
               ret.Append(setModifier);
            }

            ret.Append(" set;");
         }

         ret.Append(" }");

         return ret.ToString().Trim();
      }

      public static string GetDefinitionString(FieldInfo fi)
      {
         StringBuilder ret = new StringBuilder();

         ret.Append(GetModifier(fi.Attributes));
         ret.Append(GetTypeName(fi.FieldType));
         ret.Append(" ");
         ret.Append(fi.Name);

         object val = null;

         try { val = fi.GetValue(null); }
         catch (Exception) { val = null; }

         if (val != null)
         {
            if (val is string) val = "\"" + val.ToString() + "\"";
            else if (val is char) val = "'" + val.ToString() + "'";
            else if (val is bool) val = val.ToString().ToLower();
            else if (val is int || val is uint || val is byte || val is float || val is double || val is long || val is ulong || val is sbyte) val = val.ToString();
            else val = null;
         }

         if (val != null)
         {
            ret.Append(" = ");
            ret.Append(val.ToString());
         }

         ret.Append(";");

         return ret.ToString().Trim();
      }

      public static string GetDefinitionString(ConstructorInfo ci)
      {
         StringBuilder ret = new StringBuilder();
         ret.Append(GetModifier(ci.Attributes));
         ret.Append(ci.DeclaringType.Name);

         if (ci.IsGenericMethod) ret.Append(GetGenericMethodString(ci.GetGenericArguments()));
         ret.Append(GetParametersString(ci.GetParameters(), null));

         ret.Append(";");

         return ret.ToString().Trim();
      }

      public static string GetDefinitionString(MethodInfo mi)
      {
         try
         {
            StringBuilder ret = new StringBuilder();
            ret.Append(GetModifier(mi.Attributes));
            ret.Append(GetTypeName(mi.ReturnType));
            ret.Append(" ");
            ret.Append(mi.Name);
            if (mi.IsGenericMethod) ret.Append(GetGenericMethodString(mi.GetGenericArguments()));

            ret.Append(GetParametersString(mi.GetParameters(), mi.GetGenericArguments()));
            ret.Append(";");

            return ret.ToString().Trim();
         }
         catch (Exception ex) { return ex.ToString(); }
      }

      public static string GetDefinitionString(EventInfo ei)
      {
         StringBuilder ret = new StringBuilder();

         ret.Append(GetModifier(ei.AddMethod.Attributes));
         ret.Append("event ");
         ret.Append(GetTypeName(ei.EventHandlerType));
         ret.Append(" ");
         ret.Append(ei.Name);
         ret.Append(";");

         return ret.ToString().Trim();
      }

      public static string GetDefinitionString(Type type)
      {
         StringBuilder ret = new StringBuilder();

         ret.Append("namespace ");
         ret.Append(type.Namespace);
         ret.AppendLine();
         ret.AppendLine("{");

         ret.Append("   ");
         ret.Append(GetModifier(type.Attributes, type.IsEnum, type.IsValueType));

         if (type.IsClass)
            ret.Append("class");
         else if (type.IsInterface)
            ret.Append("interface");
         else if (type.IsEnum)
            ret.Append("enum");
         else if (type.IsValueType)
            ret.Append("struct");

         ret.Append(" ");
         ret.Append(GetTypeName(type, type.GetGenericArguments()));

         if (!type.IsEnum)
         {
            var interfaces = type.GetInterfaces().ToList();

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
               ret.Append(" : ");
               ret.Append(GetTypeName(type.BaseType));

               var bti = new HashSet<Type>(type.BaseType.GetInterfaces());

               for (int i = interfaces.Count - 1; i >= 0; i--)
                  if (bti.Contains(interfaces[i]))
                     interfaces.RemoveAt(i);
            }

            if (interfaces != null && interfaces.Count != 0)
            {
               if (type.BaseType == null || type.BaseType == typeof(object))
                  ret.Append(" : ");

               for (int i = 0; i < interfaces.Count; i++)
               {
                  if ((type.BaseType != null && type.BaseType != typeof(object)) || i != 0) ret.Append(", ");
                  ret.Append(GetTypeName(interfaces[i]));
               }
            }
         }
         else
         {
            ret.Append(" : ");
            ret.Append(GetTypeName(type.GetEnumUnderlyingType()));
         }

         ret.AppendLine();
         ret.AppendLine("   {");

         var fis = type.GetFields();

         if (!type.IsEnum && fis != null && fis.Length != 0)
         {
            bool found = false;

            foreach (var fi in fis)
            {
               if (fi.DeclaringType == type && !fi.IsSpecialName)
               {
                  ret.Append("      ");
                  ret.AppendLine(GetDefinitionString(fi));
                  found = true;
               }
            }

            if (found) ret.AppendLine();
         }

         var eis = type.GetEvents();

         if (eis != null && eis.Length != 0)
         {
            bool found = false;

            foreach (var ei in eis)
            {
               if (ei.DeclaringType == type && !ei.IsSpecialName)
               {
                  ret.Append("      ");
                  ret.AppendLine(GetDefinitionString(ei));
                  found = true;
               }
            }

            if (found) ret.AppendLine();
         }

         var cis = type.GetConstructors();

         if (cis != null && cis.Length != 0)
         {
            bool found = false;

            foreach (var ci in cis)
            {
               if (ci.DeclaringType == type)
               {
                  ret.Append("      ");
                  ret.AppendLine(GetDefinitionString(ci));
                  found = true;
               }
            }

            if (found) ret.AppendLine();
         }

         var pis = type.GetProperties();

         if (pis != null && pis.Length != 0)
         {
            bool found = false;

            foreach (var pi in pis)
            {
               if (pi.DeclaringType == type && !pi.IsSpecialName)
               {
                  ret.Append("      ");
                  ret.AppendLine(GetDefinitionString(pi));
                  found = true;
               }
            }

            if (found) ret.AppendLine();
         }

         var mis = type.GetMethods();

         if (mis != null && mis.Length != 0)
         {
            bool found = false;

            foreach (var mi in mis)
            {
               if (mi.DeclaringType == type && !mi.IsSpecialName)
               {
                  ret.Append("      ");
                  ret.AppendLine(GetDefinitionString(mi));
                  found = true;
               }
            }

            if (found) ret.AppendLine();
         }

         if (type.IsEnum)
         {
            var names = type.GetEnumNames();
            var vals = type.GetEnumValues();


            for (int i = 0; i < names.Length; i++)
            {
               if (i != 0) ret.AppendLine(", ");

               ret.Append("      ");
               ret.Append(names[i]);
               ret.Append(" = ");

               object val = vals.GetValue(i);
               if (val == null) ret.Append("null");
               else ret.Append(Convert.ToInt32(val));
            }

            ret.AppendLine();
         }

         if (ret.ToString().EndsWith("\r\n\r\n")) ret = ret.Remove(ret.Length - 2, 2);

         ret.AppendLine("   }");

         ret.Append("}");

         return ret.ToString().Trim();
      }

      public static NamespaceCollection[] ReadTypes(Assembly asm)
      {
         List<NamespaceCollection> ret = new List<NamespaceCollection>();

         var types = asm.GetTypes();
         Array.Sort(types, new FunctionalComparer((x, y) => ((Type)x).Name.CompareTo(((Type)y).Name)));

         int maxLength = 0;

         foreach (var t in types)
         {
            NamespaceCollection nc = new NamespaceCollection();

            if (!string.IsNullOrEmpty(t.Namespace) && !t.IsSpecialName && t.IsVisible)
            {
               nc.Name = t.Namespace;
               maxLength = Math.Max(maxLength, nc.Name.Length - nc.Name.Replace(".", "").Length + 1);

               nc.Types.Add(t);
               ret.Add(nc);
            }
         }

         ret.Sort();

         ret = SortNamespaces(ret);
         ret = ConsolidateNamespaces(ret);

         return ret.ToArray();
      }

      private static List<NamespaceCollection> ConsolidateNamespaces(List<NamespaceCollection> namespaces)
      {
         List<NamespaceCollection> ret = new List<NamespaceCollection>();

         foreach (NamespaceCollection nc in namespaces)
         {
            int tc = nc.Types.Count;

            var ncs = ConsolidateNamespaces(nc.Namespaces);
            nc.Namespaces.Clear();
            nc.Namespaces.AddRange(ncs);

            if (nc.Namespaces.Count == 1 && tc == 0)
            {
               nc.Namespaces[0].Name = nc.Name + "." + nc.Namespaces[0].Name;
               ret.Add(nc.Namespaces[0]);
            }
            else
               ret.Add(nc);
         }

         return ret;
      }

      private static List<NamespaceCollection> SortNamespaces(List<NamespaceCollection> namespaces)
      {
         Dictionary<string, NamespaceCollection> dic = new Dictionary<string, NamespaceCollection>();

         foreach (var nc in namespaces)
         {
            string n = nc.Name;
            if (n.Contains(".")) n = n.Substring(0, n.IndexOf("."));

            if (!dic.ContainsKey(n)) dic.Add(n, new NamespaceCollection() { Name = n });

            if (n.Equals(nc.Name)) dic[n].Types.AddRange(nc.Types);
            else
            {
               nc.Name = nc.Name.Substring(nc.Name.IndexOf(".") + 1);
               dic[n].Namespaces.Add(nc);
            }
         }

         List<NamespaceCollection> ret = new List<NamespaceCollection>();

         foreach (var kvp in dic)
         {
            var ncs = SortNamespaces(kvp.Value.Namespaces);
            kvp.Value.Namespaces.Clear();
            kvp.Value.Namespaces.AddRange(ncs);

            ret.Add(kvp.Value);
         }

         return ret;
      }

      private static string GetParametersString(ParameterInfo[] parameters, Type[] genericArguments)
      {
         StringBuilder ret = new StringBuilder();

         ret.Append("(");

         if (parameters != null && parameters.Length != 0)
         {
            for (int i = 0; i < parameters.Length; i++)
            {
               if (i != 0) ret.Append(", ");

               ret.Append(GetModifier(parameters[i].Attributes, parameters[i].CustomAttributes.ToArray()));
               ret.Append(GetTypeName(parameters[i].ParameterType, genericArguments));
               ret.Append(" ");
               ret.Append(parameters[i].Name);

               if (parameters[i].IsOptional)
               {
                  ret.Append(" = ");
                  if (parameters[i].DefaultValue == null) ret.Append("null");
                  else ret.Append(parameters[i].DefaultValue);
               }
            }
         }

         ret.Append(")");

         return ret.ToString().Trim();
      }

      private static string GetGenericMethodString(Type[] genericArguments)
      {
         StringBuilder ret = new StringBuilder();

         if (genericArguments != null && genericArguments.Length != 0)
         {
            ret.Append("<");

            for (int i = 0; i < genericArguments.Length; i++)
            {
               if (i != 0) ret.Append(", ");
               ret.Append(GetTypeName(genericArguments[i]));
            }

            ret.Append(">");
         }

         return ret.ToString().Trim();
      }

      private static string GetModifier(ParameterAttributes pa, CustomAttributeData[] cad)
      {
         StringBuilder ret = new StringBuilder();

         foreach (CustomAttributeData ca in cad)
         {
            ret.Append(" ");
            ret.Append(GetTypeName(ca.AttributeType));
         }

         if (pa == (pa | ParameterAttributes.Out)) ret.Append(" out");
         if (pa == (pa | ParameterAttributes.Retval)) ret.Append(" ref");

         if (ret.Length != 0 && ret[0] == ' ') ret = ret.Remove(0, 1);
         if (ret.Length != 0) ret.Append(" ");
         return ret.ToString();
      }

      private static string GetModifier(FieldAttributes fa)
      {
         StringBuilder ret = new StringBuilder();

         if (fa == (fa | FieldAttributes.Public)) ret.Append("public");
         else if (fa == (fa | FieldAttributes.Assembly)) ret.Append("internal");
         else if (fa == (fa | FieldAttributes.Family)) ret.Append("protected");
         else if (fa == (fa | FieldAttributes.Private)) ret.Append("private");

         if (fa == (fa | FieldAttributes.Literal)) ret.Append(" const");
         else if (fa == (fa | FieldAttributes.Static)) ret.Append(" static");
         else if (fa == (fa | FieldAttributes.InitOnly)) ret.Append(" readonly");

         if (ret.Length != 0 && ret[0] == ' ') ret = ret.Remove(0, 1);
         if (ret.Length != 0) ret.Append(" ");
         return ret.ToString();
      }

      private static string GetModifier(MethodAttributes ma)
      {
         StringBuilder ret = new StringBuilder();

         if (ma == (ma | MethodAttributes.Public)) ret.Append("public");
         else if (ma == (ma | MethodAttributes.Assembly)) ret.Append("internal");
         else if (ma == (ma | MethodAttributes.Family)) ret.Append("protected");
         else if (ma == (ma | MethodAttributes.Private)) ret.Append("private");

         if (ma == (ma | MethodAttributes.Static)) ret.Append(" static");
         if (ma == (ma | MethodAttributes.Abstract)) ret.Append(" abstract");
         if (ma == (ma | MethodAttributes.Virtual)) ret.Append(" virtual");
         if (ma == (ma | MethodAttributes.Final)) ret.Append(" sealed");

         if (ret.Length != 0 && ret[0] == ' ') ret = ret.Remove(0, 1);
         if (ret.Length != 0) ret.Append(" ");
         return ret.ToString();
      }

      private static string GetModifier(TypeAttributes ta, bool isEnum, bool isValueType)
      {
         StringBuilder ret = new StringBuilder();

         if (ta == (ta | TypeAttributes.Public)) ret.Append("public");
         else if (ta == (ta | TypeAttributes.NestedAssembly)) ret.Append("internal");
         else if (ta == (ta | TypeAttributes.NestedFamily)) ret.Append("protected");
         else if (ta == (ta | TypeAttributes.NestedPrivate)) ret.Append("private");

         if (!isValueType)
         {
            if (ta == (ta | TypeAttributes.Abstract | TypeAttributes.Sealed)) ret.Append(" static");
            else if (ta == (ta | TypeAttributes.Abstract)) ret.Append(" abstract");
            else if (!isEnum && ta == (ta | TypeAttributes.Sealed)) ret.Append(" sealed");
         }

         if (ret.Length != 0 && ret[0] == ' ') ret = ret.Remove(0, 1);
         if (ret.Length != 0) ret.Append(" ");
         return ret.ToString();
      }

      public static string GetTypeName(Type type)
      {
         return GetTypeName(type, type.GetGenericArguments());
      }

      private static string GetTypeName(string name)
      {
         switch (name)
         {
            case "Void": return "void";
            case "Int32": return "int";
            case "Single": return "float";
            case "Double": return "double";
            case "Int64": return "long";
            case "Char": return "char";
            case "Boolean": return "bool";
            case "Byte": return "byte";
            case "SByte": return "sbyte";
            case "UInt32": return "uint";
            case "UInt64": return "ulong";
            case "String": return "string";
            case "Object": return "object";
            case "ParamArrayAttribute": return "params";
            default: return name;
         }
      }

      public static string GetTypeName(Type type, Type[] baseMethodGenericArguments = null)
      {
         string appendix = string.Empty;
         string name = type.Name;

         if (type.IsArray)
         {
            int idx = type.Name.IndexOf("[");

            if (idx != -1)
            {
               appendix = type.Name.Substring(idx);
               name = type.Name.Substring(0, idx);
            }
         }

         if (name.Contains("`"))
         {
            int idx = name.IndexOf("`");

            Type[] gts = type.GenericTypeArguments;
            if (gts == null || gts.Length == 0) gts = baseMethodGenericArguments;

            if (gts != null && gts.Length != 0)
            {
               appendix = ">" + appendix;

               for (int i = gts.Length - 1; i >= 0; i--)
               {
                  appendix = GetTypeName(gts[i], baseMethodGenericArguments) + appendix;
                  if (i != 0) appendix = ", " + appendix;
               }

               appendix = "<" + appendix;

               name = type.Name.Substring(0, idx);
            }
         }

         return GetTypeName(name) + appendix;
      }
   }

   public class NamespaceCollection : IComparable<NamespaceCollection>
   {
      public string Name { get; set; }
      public List<NamespaceCollection> Namespaces { get; } = new List<NamespaceCollection>();
      public List<Type> Types { get; } = new List<Type>();

      public int CompareTo(NamespaceCollection other)
      {
         return Name.CompareTo(other.Name);
      }

      public override string ToString()
      {
         return Name + " [Types: " + Types.Count.ToString() + "]";
      }
   }
}
