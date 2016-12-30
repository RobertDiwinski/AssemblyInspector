using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssemblyInspector
{
   public enum NodeType { None = 0, References = 1, Namespaces = 2, Enums = 3 }

   public partial class frmMain : Form
   {
      private string baseDir = null;

      public frmMain()
      {
         InitializeComponent();
         AppDomain.CurrentDomain.AssemblyResolve += new System.ResolveEventHandler(AssemblyResolve);
      }

      private void FillTree(string file)
      {
         tree.SuspendLayout();
         tree.BeginUpdate();

         tree.Nodes.Clear();
         txt.Text = string.Empty;

         Assembly asm = Assembly.LoadFile(file);
         baseDir = Path.GetDirectoryName(file);
         lbl.Text = file;

         FillTreeChilds(AddTreeNode(asm.GetName().Name, asm, tree.Nodes));

         tree.TreeViewNodeSorter = new FunctionalComparer((x, y) =>
         {
            TreeNode tn1 = x as TreeNode;
            TreeNode tn2 = y as TreeNode;

            if (tn1.Tag is NodeType && tn2.Tag is NodeType)
            {
               NodeType nt1 = (NodeType)tn1.Tag;
               NodeType nt2 = (NodeType)tn2.Tag;

               return nt1.CompareTo(nt2);
            }
            else if (tn1.Tag is NodeType && tn2.Tag is NamespaceCollection) return -1;
            else if (tn1.Tag is NamespaceCollection && tn2.Tag is NodeType) return 1;
            else if (tn1.Tag is NamespaceCollection && !(tn2.Tag is NamespaceCollection)) return -1;
            else if (!(tn1.Tag is NamespaceCollection) && tn2.Tag is NamespaceCollection) return 1;
            else if (tn1.Tag is NamespaceCollection && tn2.Tag is NamespaceCollection)
            {
               NamespaceCollection nc1 = tn1.Tag as NamespaceCollection;
               NamespaceCollection nc2 = tn2.Tag as NamespaceCollection;

               return nc1.Name.CompareTo(nc2.Name);
            }
            else if (tn1.Tag is Type && !(tn2.Tag is Type)) return 1;
            else if (!(tn1.Tag is Type) && tn2.Tag is Type) return -1;
            else if (tn1.Tag is Type && tn2.Tag is Type)
            {
               Type t1 = tn1.Tag as Type;
               Type t2 = tn2.Tag as Type;

               int v1 = int.MaxValue;
               int v2 = int.MaxValue;

               if (t1.IsEnum) v1 = 0;
               else if (t1.IsInterface) v1 = 1;
               else if (t1.IsClass) v1 = 2;
               else v1 = 3;

               if (t2.IsEnum) v2 = 0;
               else if (t2.IsInterface) v2 = 1;
               else if (t2.IsClass) v2 = 2;
               else v2 = 3;

               if (v1 == v2) return tn1.Text.CompareTo(tn2.Text);
               return v1.CompareTo(v2);
            }
            else if (tn1.Tag is MemberInfo && !(tn2.Tag is MemberInfo)) return 1;
            else if (!(tn1.Tag is MemberInfo) && tn2.Tag is MemberInfo) return -1;
            else if (tn1.Tag is MemberInfo && tn2.Tag is MemberInfo)
            {
               MemberInfo i1 = tn1.Tag as MemberInfo;
               MemberInfo i2 = tn2.Tag as MemberInfo;

               int v1 = int.MaxValue;
               int v2 = int.MaxValue;

               if (i1 is FieldInfo) v1 = 0;
               else if (i1 is EventInfo) v1 = 1;
               else if (i1 is ConstructorInfo) v1 = 2;
               else if (i1 is PropertyInfo) v1 = 3;
               else v1 = 4;

               if (i2 is FieldInfo) v2 = 0;
               else if (i2 is EventInfo) v2 = 1;
               else if (i2 is ConstructorInfo) v2 = 2;
               else if (i2 is PropertyInfo) v2 = 3;
               else v2 = 4;

               if (v1 == v2) return tn1.Text.CompareTo(tn2.Text);
               return v1.CompareTo(v2);
            }
            else if (tn1.ImageKey == tn2.ImageKey || (tn1.ImageKey != "gac" && tn2.ImageKey != "gac")) return tn1.Text.CompareTo(tn2.Text);
            else if (tn1.ImageKey == "gac") return 1;
            else return -1;
         });

         tree.EndUpdate();
         tree.ResumeLayout();
      }

      private TreeNode AddTreeNode(string text, object tag, TreeNodeCollection collection)
      {
         TreeNode tn = new TreeNode();
         tn.Text = text;
         tn.Tag = tag;

         Assembly asm = tag as Assembly;
         string msg = tag as string;
         Type type = tag as Type;
         PropertyInfo pi = tag as PropertyInfo;
         MethodInfo mi = tag as MethodInfo;
         EventInfo ei = tag as EventInfo;
         ConstructorInfo ci = tag as ConstructorInfo;
         FieldInfo fi = tag as FieldInfo;
         NamespaceCollection nc = tag as NamespaceCollection;

         if (asm != null)
         {
            if (asm.GlobalAssemblyCache)
            {
               tn.ImageKey = "gac";
               tn.ForeColor = Color.Gray;
            }
            else if (asm.EntryPoint == null) tn.ImageKey = "dll";
            else tn.ImageKey = "exe";
         }
         else if (type != null)
         {
            if (type.IsClass) tn.ImageKey = "class";
            else if (type.IsEnum) tn.ImageKey = "enum";
            else if (type.IsInterface) tn.ImageKey = "interface";
            else if (type.IsValueType) tn.ImageKey = "struct";
         }
         else if (pi != null) tn.ImageKey = "prop";
         else if (mi != null) tn.ImageKey = "method";
         else if (ei != null) tn.ImageKey = "event";
         else if (ci != null) tn.ImageKey = "ctor";
         else if (fi != null) tn.ImageKey = "field";
         else if (nc != null) tn.ImageKey = "ns";
         else if (tag is NodeType && (NodeType)tag == NodeType.Enums) tn.ImageKey = "enum";
         else if (tag is NodeType && (NodeType)tag == NodeType.References) tn.ImageKey = "link";
         else if (tag is NodeType && (NodeType)tag == NodeType.Namespaces) tn.ImageKey = "ns";
         else tn.ImageKey = "err";

         tn.SelectedImageKey = tn.ImageKey;
         tn.StateImageKey = tn.ImageKey;

         collection.Add(tn);

         return tn;
      }

      private void FillReferences(TreeNode parent, Assembly asm)
      {
         AssemblyName[] asms = asm.GetReferencedAssemblies();

         if (asms != null && asms.Length != 0)
         {
            TreeNode tn = AddTreeNode("References", NodeType.References, parent.Nodes);
            tn.NodeFont = new Font(tree.Font, FontStyle.Italic | FontStyle.Bold);

            foreach (var n in asms)
            {
               Assembly asm2 = null;
               string msg2 = null;

               try { asm2 = Assembly.Load(n); }
               catch (Exception ex) { msg2 = ex.ToString(); }

               if (asm2 != null) AddTreeNode(n.Name, asm2, tn.Nodes);
               else AddTreeNode(n.Name, msg2, tn.Nodes);
            }
         }
      }

      private void FillTypes(TreeNode parent, Type[] types)
      {
         if (types != null && types.Length != 0)
         {
            foreach (var t in types)
            {
               if (t.IsVisible)
               {
                  if (t.IsClass) AddTreeNode(Inspector.GetTypeName(t), t, parent.Nodes);
                  else if (t.IsEnum) AddTreeNode(Inspector.GetTypeName(t), t, parent.Nodes);
                  else if (t.IsInterface) AddTreeNode(Inspector.GetTypeName(t), t, parent.Nodes);
                  else if (t.IsValueType) AddTreeNode(Inspector.GetTypeName(t), t, parent.Nodes);
               }
            }
         }
      }

      private void FillNamespaces(TreeNode parent, Assembly asm)
      {
         NamespaceCollection[] ncs = Inspector.ReadTypes(asm);
         FillNamespaces(parent, ncs);
      }

      private void FillNamespaces(TreeNode parent, NamespaceCollection[] ncs)
      {
         if (ncs != null && ncs.Length != 0)
         {
            foreach (var nc in ncs)
               AddTreeNode(nc.Name, nc, parent.Nodes);
         }
      }

      private void FillMethods(TreeNode parent, Type type)
      {
         PropertyInfo[] props = type.GetProperties();
         MethodInfo[] methods = type.GetMethods();
         EventInfo[] events = type.GetEvents();
         ConstructorInfo[] ctors = type.GetConstructors();
         FieldInfo[] fields = type.GetFields();

         if (type.IsEnum)
         {
            string[] enumNames = type.GetEnumNames();
            Array enumVals = type.GetEnumValues();

            if (enumNames != null && enumNames.Length != 0)
            {
               for (int i = 0; i < enumNames.Length; i++)
               {
                  object val = enumVals.GetValue(i);

                  if (val == null) AddTreeNode(enumNames[i] + " = null", NodeType.Enums, parent.Nodes);
                  else AddTreeNode(enumNames[i] + " = " + (Convert.ToInt32(val)).ToString(), NodeType.Enums, parent.Nodes);
               }
            }
         }

         if (props != null && props.Length != 0)
         {
            foreach (var pi in props)
               if (!pi.IsSpecialName && pi.DeclaringType == type)
                  AddTreeNode(pi.Name + ": " + Inspector.GetTypeName(pi.PropertyType, type.GetGenericArguments()), pi, parent.Nodes);
         }

         if (methods != null && methods.Length != 0)
         {
            foreach (var mi in methods)
            {
               if (!mi.IsSpecialName && mi.DeclaringType == type)
               {
                  StringBuilder sb = new StringBuilder();
                  sb.Append(mi.Name);
                  sb.Append("(");

                  try
                  {
                     ParameterInfo[] par = mi.GetParameters();

                     if (par != null && par.Length != 0)
                     {
                        for (int i = 0; i < par.Length; i++)
                        {
                           if (i != 0) sb.Append(", ");

                           if (par[i].IsOptional) sb.Append("[");
                           if (par[i].IsOut) sb.Append("out ");
                           if (par[i].IsRetval) sb.Append("ref ");
                           sb.Append(Inspector.GetTypeName(par[i].ParameterType, type.GetGenericArguments()));
                           sb.Append(" ");
                           sb.Append(par[i].Name);
                           if (par[i].IsOptional)
                           {
                              if (par[i].DefaultValue != null) sb.Append(" = " + par[i].DefaultValue.ToString());
                              else sb.Append(" = null");

                              sb.Append("]");
                           }

                        }
                     }
                  }
                  catch (Exception ex) { sb.Append(ex.Message); }

                  sb.Append(")");

                  AddTreeNode(sb.ToString(), mi, parent.Nodes);
               }
            }
         }

         if (events != null && events.Length != 0)
         {
            foreach (var ei in events)
            {
               if (!ei.IsSpecialName && ei.DeclaringType == type)
               {
                  StringBuilder sb = new StringBuilder();
                  sb.Append(ei.Name);
                  sb.Append("(");

                  try
                  {
                     ParameterInfo[] par = ei.AddMethod.GetParameters();

                     if (par != null && par.Length != 0)
                     {
                        for (int i = 0; i < par.Length; i++)
                        {
                           if (i != 0) sb.Append(", ");

                           if (par[i].IsOptional) sb.Append("[");
                           if (par[i].IsOut) sb.Append("out ");
                           if (par[i].IsRetval) sb.Append("ref ");
                           sb.Append(Inspector.GetTypeName(par[i].ParameterType, type.GetGenericArguments()));
                           sb.Append(" ");
                           sb.Append(par[i].Name);
                           if (par[i].IsOptional)
                           {
                              if (par[i].DefaultValue != null) sb.Append(" = " + par[i].DefaultValue.ToString());
                              else sb.Append(" = null");

                              sb.Append("]");
                           }

                        }
                     }
                  }
                  catch (Exception ex) { sb.Append(ex.Message); }

                  sb.Append(")");

                  AddTreeNode(sb.ToString(), ei, parent.Nodes);
               }
            }
         }

         if (ctors != null && ctors.Length != 0)
         {
            foreach (var ci in ctors)
            {
               if (ci.DeclaringType == type)
               {
                  StringBuilder sb = new StringBuilder();
                  sb.Append(type.Name);
                  sb.Append("(");

                  try
                  {
                     ParameterInfo[] par = ci.GetParameters();

                     if (par != null && par.Length != 0)
                     {
                        for (int i = 0; i < par.Length; i++)
                        {
                           if (i != 0) sb.Append(", ");

                           if (par[i].IsOptional) sb.Append("[");
                           if (par[i].IsOut) sb.Append("out ");
                           if (par[i].IsRetval) sb.Append("ref ");
                           sb.Append(Inspector.GetTypeName(par[i].ParameterType, type.GetGenericArguments()));
                           sb.Append(" ");
                           sb.Append(par[i].Name);
                           if (par[i].IsOptional)
                           {
                              if (par[i].DefaultValue != null) sb.Append(" = " + par[i].DefaultValue.ToString());
                              else sb.Append(" = null");

                              sb.Append("]");
                           }

                        }
                     }
                  }
                  catch (Exception ex) { sb.Append(ex.Message); }

                  sb.Append(")");

                  AddTreeNode(sb.ToString(), ci, parent.Nodes);
               }
            }
         }

         if (!type.IsEnum && fields != null && fields.Length != 0)
         {
            foreach (var fi in fields)
            {
               if (!fi.IsSpecialName && fi.DeclaringType == type)
               {
                  StringBuilder sb = new StringBuilder();
                  sb.Append(fi.Name);
                  sb.Append(": ");
                  sb.Append(Inspector.GetTypeName(fi.FieldType, type.GetGenericArguments()));

                  AddTreeNode(sb.ToString(), fi, parent.Nodes);
               }
            }
         }
      }

      private void FillTreeChilds(TreeNode parent)
      {
         if (parent.Nodes.Count == 0)
         {
            Assembly asm = parent.Tag as Assembly;
            Type type = parent.Tag as Type;
            NamespaceCollection nc = parent.Tag as NamespaceCollection;

            if (asm != null && !asm.GlobalAssemblyCache)
            {
               FillReferences(parent, asm);
               FillNamespaces(parent, asm);
            }
            else if (nc != null)
            {
               if (nc.Namespaces.Count != 0) FillNamespaces(parent, nc.Namespaces.ToArray());
               if (nc.Types.Count != 0) FillTypes(parent, nc.Types.ToArray());
            }
            else if (type != null)
               FillMethods(parent, type);
         }
         
      }

      private void cmdOpen_Click(object sender, EventArgs e)
      {
         OpenFileDialog dlg = new OpenFileDialog();
         dlg.Filter = ".NET-Assemblies|*.exe;*.dll";

         if (dlg.ShowDialog() == DialogResult.OK)
            FillTree(dlg.FileName);
      }

      private string GetPropertyList(object o)
      {
         Type t = o.GetType();

         PropertyInfo[] props = t.GetProperties();
         Array.Sort(props, (x, y) =>
         {
            PropertyInfo pi1 = x as PropertyInfo;
            PropertyInfo pi2 = y as PropertyInfo;

            return pi1.Name.CompareTo(pi2.Name);
         });

         StringBuilder sb = new StringBuilder();

         foreach (var pi in props)
         {
            sb.Append(pi.Name);
            sb.Append(": ");

            object val = null;
            try { val = pi.GetValue(o); }
            catch (Exception ex) { val = ex.Message; }

            if (val != null)
               if (val.GetType().IsArray)
               {
                  sb.AppendLine();

                  Array a = (Array)val;
                  
                  for (int i = 0; i < a.Length; i++)
                  {
                     object ao = a.GetValue(i);

                     sb.Append("   [");
                     sb.Append(i);
                     sb.Append("]: ");

                     if (ao != null) sb.AppendLine(ao.ToString());
                     else sb.AppendLine("null");
                  }
               }
               else
                  sb.AppendLine(val.ToString().Replace("\r", "\\r").Replace("\n", "\\n"));
            else
               sb.AppendLine("null");
         }

         return sb.ToString();
      }

      private void tree_AfterSelect(object sender, TreeViewEventArgs e)
      {
         TreeNode tn = tree.SelectedNode;

         if (tn != null)
         {
            Assembly asm = tn.Tag as Assembly;
            string msg = tn.Tag as string;
            Type type = tn.Tag as Type;
            PropertyInfo pi = tn.Tag as PropertyInfo;
            MethodInfo mi = tn.Tag as MethodInfo;
            EventInfo ei = tn.Tag as EventInfo;
            ConstructorInfo ci = tn.Tag as ConstructorInfo;
            FieldInfo fi = tn.Tag as FieldInfo;

            if (asm != null)
            {
               txt.Text = GetPropertyList(asm);
               Inspector.ReadTypes(asm);
            } 
            else if (type != null) txt.Text = Inspector.GetDefinitionString(type);
            else if (pi != null) txt.Text = Inspector.GetDefinitionString(pi);
            else if (mi != null) txt.Text = Inspector.GetDefinitionString(mi);
            else if (ei != null) txt.Text = Inspector.GetDefinitionString(ei);
            else if (ci != null) txt.Text = Inspector.GetDefinitionString(ci);
            else if (fi != null) txt.Text = Inspector.GetDefinitionString(fi);
            else if (msg != null) txt.Text = msg;
            else txt.Text = string.Empty;
         }
      }

      private Assembly AssemblyResolve(object sender, System.ResolveEventArgs args)
      {
         string file = (string)(args.Name.Substring(0, System.Convert.ToInt32(args.Name.IndexOf(" ") - 1)) + ".dll");

         string p = null;

         if (file.ToLower().EndsWith("resources.dll"))
         {
            string culture = System.Threading.Thread.CurrentThread.CurrentCulture.IetfLanguageTag;

            if (!string.IsNullOrEmpty(culture))
            {
               p = Path.GetDirectoryName(Application.ExecutablePath);
               p = System.IO.Path.Combine(p, culture);

               if (!Directory.Exists(p))
                  p = System.IO.Path.Combine(baseDir, culture.Substring(0, culture.IndexOf('-')));

               p = System.IO.Path.Combine(p, file);
            }
         }
         else
            p = System.IO.Path.Combine(baseDir, file);

         if (System.IO.File.Exists(p))
         {
            System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFile(p);
            return asm;
         }

         return null;
      }

      private void tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
      {
         foreach (TreeNode tn in e.Node.Nodes)
            FillTreeChilds(tn);
      }

      private void frmMain_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
      {
         if (sender == txt && e.Modifiers == Keys.Control && e.KeyCode == Keys.A) txt.SelectAll();
         else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            cmdOpen_Click(cmdOpen, new EventArgs());
      }
   }

   public class FunctionalComparer : IComparer
   {
      private Func<object, object, int> comparer;
      public FunctionalComparer(Func<object, object, int> comparer)
      {
         this.comparer = comparer;
      }
      public static IComparer Create(Func<object, object, int> comparer)
      {
         return new FunctionalComparer(comparer);
      }
      public int Compare(object x, object y)
      {
         return comparer(x, y);
      }
   }
}
