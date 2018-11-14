using MetroFramework.Forms;
using Microsoft.Build.Construction;
using System;
using System.Activities;
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
using System.Xml.Linq;
using TxFlow.WFActivities;

namespace TxFlow.CSharpDSL.Generator
{  
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void generateCSProj(string destinationDirectory, string nameSpace, string[] itemsToCompile, IEnumerable<Assembly> additionalAssemblies, string[] itemsToAdd)
        {
            string csProjFilePath = Path.Combine(destinationDirectory, nameSpace + ".csproj");

            ProjectRootElement root;
            if (File.Exists(csProjFilePath))
            {
                root = ProjectRootElement.Open(csProjFilePath);
            }
            else
            {
                root = ProjectRootElement.Create();
                var group = root.AddPropertyGroup();
                group.AddProperty("Configuration", "Debug");
                group.AddProperty("Platform", "AnyCPU");
                group.AddProperty("RootNamespace", nameSpace);
                group.AddProperty("AssemblyName", nameSpace);
                group.AddProperty("TargetFrameworkVersion", "v4.6.2");
                group.AddProperty("OutputType", "Library");
                group.AddProperty("OutputPath", "bin\\");

                var referenceGroup = root.AddItemGroup();
                referenceGroup.AddItem("Reference", "System");
                referenceGroup.AddItem("Reference", "System.Core");
                referenceGroup.AddItem("Reference", "System.Activities");
                referenceGroup.AddItem("Reference", "System.Xaml");
                referenceGroup.AddItem("Reference", "System.Xml");
                referenceGroup.AddItem("Reference", "System.Xml.Linq");
                referenceGroup.AddItem("Reference", "Microsoft.CSharp");
                referenceGroup.AddItem("Reference", "Microsoft.ExtendedReflection, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL");
                referenceGroup.AddItem("Reference", "Microsoft.Pex.Framework, Version=15.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL");

                foreach (var additionalAssembly in additionalAssemblies)
                {
                    referenceGroup.AddItem("Reference", additionalAssembly.FullName, new Dictionary<string, string>()
                    {
                        { "SpecificVersion", "false" },
                        { "HintPath", additionalAssembly.Location}
                    });
                }

                var import = root.AddImport(@"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props");
                import.Condition = @"Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')";
                root.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");

                string cSharpTranspilerLocation = this.GetType().Assembly.Location.Replace("TxFlow.CSharpDSL.Generator", "TxFlow.CSharpDSL.Transpiler");
                root.AddPropertyGroup()
                    .AddProperty("PostBuildEvent",  cSharpTranspilerLocation + " \"$(ProjectPath)\"");
            }

            // items to compile
            //var compileItemDefinition = root.ItemDefinitions.FirstOrDefault(x => x.ElementName == "Compile");
            //if(compileItemDefinition == null)
            //{
            //    compileItemDefinition = root.AddItemDefinition("Compile");
            //}
            foreach (var item in itemsToCompile)
            {
                // TODO
                if (!root.Items.Any(x => x.ItemType == "Compile" && x.Include == item))
                {
                    root.AddItem("Compile", item);
                }
            }

            foreach (var item in itemsToAdd)
            {
                // TODO
                if (!root.Items.Any(x => x.ItemType == "None" && x.Include == item))
                {
                    root.AddItem("None", item);
                }
            }

            root.Save(csProjFilePath);

            // bugfix: move PostBuildEvent to end of csproj-file, otherwise visual studio does not recognize it
            var doc = XDocument.Load(csProjFilePath);
            var projectElement = doc.Elements().First();
            var tmp4 = projectElement.Elements().OrderBy(x =>
            {
                if(x.Elements().Any(y => y.Name.LocalName == "PostBuildEvent"))
                {
                    return true;
                }
                return false;
            }).ToArray();

            projectElement.ReplaceNodes(tmp4);

            doc.Save(csProjFilePath);
        }

        private void generateButton_Click(object sender, EventArgs e)
        {


            string folderPath = this.resultingProjectDirTextBox.Text;
            Directory.CreateDirectory(folderPath);

            var activityDescriptors = this.checkedListBox1.CheckedItems.Cast<Type>().Select(x => new ActivityDescriptor(x)).ToArray();


            string nameSpace = Path.GetFileName(folderPath);

            string activityToolboxName = this.activityToolboxNameTextBox.Text;

            string toolboxCode =
                $"namespace {nameSpace}" + Environment.NewLine +
                "{" + Environment.NewLine +
                $"    public abstract class {activityToolboxName}" + Environment.NewLine +
                "    {" + Environment.NewLine +
                string.Join(Environment.NewLine + Environment.NewLine, activityDescriptors.Select(
                    x => "       public virtual " + x.ToCSharpMethodSignature() + Environment.NewLine
                        + "       { " + (x.HasOutArguments() ? "throw new System.NotImplementedException(\"Activity - method has a return value but no mock - implemenation\");" : "") + " }" + Environment.NewLine
                        //+   x.ToInArgumentClass("       ")
                        )) + Environment.NewLine +
                "    }" + Environment.NewLine +
                "}";

            writeTexTFile(folderPath + "\\" + activityToolboxName + ".generated.cs", toolboxCode);
            string activityToolboxLoggerName = activityToolboxName + "Logger";

            string toolboxLoggerCode =
                "using TxFlow.CSharpDSL;" + Environment.NewLine + Environment.NewLine +
                $"namespace {nameSpace}" + Environment.NewLine +
                "{" + Environment.NewLine +
                $"    public class {activityToolboxLoggerName} : {activityToolboxName}" + Environment.NewLine +
                 "    {" + Environment.NewLine +
                $"       private readonly {activityToolboxName} _realObject;" + Environment.NewLine +
                $"       private readonly ActivityInvocationLog _log = new ActivityInvocationLog();" + Environment.NewLine +
                $"       public {activityToolboxLoggerName}({activityToolboxName} realObject)" + Environment.NewLine +
                "       {" + Environment.NewLine +
                "           _realObject = realObject;" + Environment.NewLine +
                "       }" + Environment.NewLine + Environment.NewLine +
                "       public ActivityInvocationLog ActivityInvocationLog => _log;" + Environment.NewLine +
                string.Join(Environment.NewLine + Environment.NewLine, activityDescriptors.Select(
                    x => "       public override " + x.ToCSharpMethodSignature() + Environment.NewLine
                        + "       {" + Environment.NewLine
                        + $"           " + (x.HasReturnValue() ? x.ReturnTypeText + " returnValue = " : "") + $"_realObject.{x.ActivityName}({x.GetArgumentsAsString(false)});" + Environment.NewLine
                        + $"           _log.Log(new ActivityInvocation(nameof({x.ActivityNameWithoutGenerics})," + Environment.NewLine
                        + "                     new System.Collections.Generic.Dictionary<string, object>() { " + x.GetArgumentsAsDictionaryKeyValuePair() + " }," + Environment.NewLine
                        + $"                     " + (x.HasReturnValue() ? "returnValue" : "typeof(void)") + "));" + Environment.NewLine
                        + (x.HasReturnValue() ?
                           "           return returnValue;" + Environment.NewLine : "")
                        + "       }" + Environment.NewLine
                        //+   x.ToInArgumentClass("       ")
                        )) + Environment.NewLine +
                "    }" + Environment.NewLine +
                "}";

            writeTexTFile(folderPath + "\\" + activityToolboxLoggerName + ".generated.cs", toolboxLoggerCode);

            string pexActivityToolboxName = "Pex" + activityToolboxName;

            string pexToolboxCode =
                $"namespace {nameSpace}" + Environment.NewLine +
                "{" + Environment.NewLine +
                $"    public class {pexActivityToolboxName} : {activityToolboxName}" + Environment.NewLine +
                 "    {" + Environment.NewLine +
                string.Join(Environment.NewLine + Environment.NewLine, activityDescriptors.Select(
                    x => !x.HasOutArguments() ? string.Empty :
                    "       public override " + x.ToCSharpMethodSignature() + Environment.NewLine
                        + "       {" + Environment.NewLine
                        + string.Join(Environment.NewLine, x.GetOutArgumentNames().Skip(x.HasReturnValue() ? 1 : 0).Select(o =>
                        $"           {o.Name} = Microsoft.Pex.Framework.PexChoose.Value<{o.CSharpTypeString}>(nameof({x.ActivityNameWithoutGenerics}));")) + Environment.NewLine
                        + (x.HasReturnValue() ?
                           $"           return Microsoft.Pex.Framework.PexChoose.Value<{x.ReturnTypeText}>(nameof({x.ActivityNameWithoutGenerics}));" + Environment.NewLine : "")
                        + "       }" + Environment.NewLine
                        //+   x.ToInArgumentClass("       ")
                        ).Where(x => !string.IsNullOrWhiteSpace(x))) + Environment.NewLine +
                "    }" + Environment.NewLine +
                "}";

            writeTexTFile(folderPath + "\\" + pexActivityToolboxName + ".generated.cs", pexToolboxCode);



            string debugAdapterLocation = this.GetType().Assembly.Location.Replace("TxFlow.CSharpDSL.Generator", "TxFlow.Debug.Adapter");

            string launchDebugCode =
            "{" + Environment.NewLine
            + "    \"$adapter\": \""+ debugAdapterLocation.Replace("\\", "\\\\") + "\"," + Environment.NewLine
            + "	\"name\": \"Attach TxFlow\"," + Environment.NewLine
            + "	\"type\": \"TxFlow\"," + Environment.NewLine
            + "	\"request\": \"attach\"," + Environment.NewLine
            + "	\"address\": \"http://localhost:8071\"," + Environment.NewLine
            + "}";


            writeTexTFile(folderPath + "\\launchDebug.json", launchDebugCode);


            var additionalRequiredAssemblies = activityDescriptors.SelectMany(x => x.ArgumentTypes).Select(x => x.Assembly)
                .Union(new Assembly[]
                {
                    typeof(TxFlow.CSharpDSL.AbstractWorkflow<>).Assembly
                })
                .Union(activityDescriptors.Select(x=>x.ActivityAssembly))
                .Distinct().ToList();

            generateCSProj(folderPath, nameSpace, new string[] {
                activityToolboxName + ".generated.cs",
                activityToolboxLoggerName + ".generated.cs",
                pexActivityToolboxName + ".generated.cs" }, additionalRequiredAssemblies, new string[] { folderPath + "\\launchDebug.json" });

            MessageBox.Show("Generation successfully completed!");
        }

        private void writeTexTFile(string filePath, string content)
        {
            if(!File.Exists(filePath) || File.ReadAllText(filePath) != content)
            {
                File.WriteAllText(filePath, content);
            }
        }

        //public class PexHolidayApprovalToolbox : HolidayApprovalActivityToolbox
        //{
        //    public override bool CreateHolidayApprovalTaskActivity(
        //        HolidayRequestEntity HolidayRequest, string ResponsibleUser)
        //    {
        //        bool chosen = Microsoft.Pex.Framework.PexChoose.Value<bool>("CreateHolidayApprovalTaskActivityReturnValue");
        //        Microsoft.Pex.Framework.PexObserve.ValueForViewing("CreateHolidayApprovalTaskActivityReturnValue", chosen);
        //        return chosen;
        //    }
        //}

        private void activityFilePathTextBox_TextChanged(object sender, EventArgs e)
        {
            string activityFilePath = this.activityFilePathTextBox.Text;
            bool enabled = !string.IsNullOrWhiteSpace(activityFilePath) && File.Exists(activityFilePath) && Path.GetExtension(activityFilePath).ToLower() == ".dll";
            this.generateButton.Enabled = enabled;
            this.checkedListBox1.Items.Clear();
            if (enabled)
            {
                var allTypes = Assembly.LoadFrom(activityFilePath).GetTypes().Where(x => typeof(Activity).IsAssignableFrom(x)).ToArray();
                this.checkedListBox1.Items.AddRange(allTypes);
                for (int i = 0; i < allTypes.Length; i++)
                {
                    this.checkedListBox1.SetItemChecked(i, true);
                }

                string fileNameWithoutActivities = Path.GetFileNameWithoutExtension(activityFilePath);
                int idx = fileNameWithoutActivities.IndexOf("Activit");
                if (idx > 0)
                {
                    fileNameWithoutActivities = fileNameWithoutActivities.Substring(0, idx);
                }
                fileNameWithoutActivities = fileNameWithoutActivities.Replace(".", "");

                if (string.IsNullOrWhiteSpace(this.activityToolboxNameTextBox.Text))
                {
                    this.activityToolboxNameTextBox.Text = fileNameWithoutActivities + "ActivityToolbox";
                }

                if (string.IsNullOrWhiteSpace(this.resultingProjectDirTextBox.Text))
                {
                    this.resultingProjectDirTextBox.Text = Path.Combine(Path.GetDirectoryName(activityFilePath), fileNameWithoutActivities + ".TxFlow");
                }
            }
        }

        private void chooseActivityDLLButton_Click(object sender, EventArgs e)
        {
            if(this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.activityFilePathTextBox.Text = this.openFileDialog1.FileName;
            }
        }

        private void chooseResultingProjectDirButton_Click(object sender, EventArgs e)
        {
            if(this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.resultingProjectDirTextBox.Text = this.folderBrowserDialog1.SelectedPath;
            }
        }



    }
}
