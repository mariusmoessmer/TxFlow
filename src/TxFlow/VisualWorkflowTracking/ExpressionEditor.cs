using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Expressions;
using System.Activities.Presentation.Hosting;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TxFlow.WFBuilder.Layout
{

    public class MyExpressionActivityEditor : TextualExpressionEditor
    {

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyExpressionActivityEditor),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTextChanged), new CoerceValueCallback(OnTextCoerceValue)));


        public static readonly DependencyProperty ExpressionTextProperty = DependencyProperty.Register("ExpressionText", typeof(string), typeof(MyExpressionActivityEditor),
        new PropertyMetadata(null));

        public MyExpressionActivityEditor()
        {

        }

        void OnEditorLoaded(object sender, RoutedEventArgs e)
        {

        }


        public override IExpressionEditorService ExpressionEditorService
        {
            get
            {
                return null;
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string ExpressionText
        {
            get { return (string)GetValue(ExpressionTextProperty); }
            set { SetValue(ExpressionTextProperty, value); }
        }

        static void OnTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {

        }
        static object OnTextCoerceValue(DependencyObject dp, object value)
        {
            string tempText = value as string;
            return tempText;
        }

        public override bool Commit(bool isExplicitCommit)
        {
            return true;
            //throw new NotImplementedException();
        }
    }

    public class MyEditorService : IExpressionEditorService
    {
        public void CloseExpressionEditors()
        {

        }
        public IExpressionEditorInstance CreateExpressionEditor(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces, List<ModelItem> variables, string text)
        {
            MyExpressionEditorInstance instance = new MyExpressionEditorInstance();
            return instance;
        }
        public IExpressionEditorInstance CreateExpressionEditor(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces, List<ModelItem> variables, string text, System.Windows.Size initialSize)
        {
            MyExpressionEditorInstance instance = new MyExpressionEditorInstance();
            return instance;
        }
        public IExpressionEditorInstance CreateExpressionEditor(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces, List<ModelItem> variables, string text, Type expressionType)
        {
            MyExpressionEditorInstance instance = new MyExpressionEditorInstance();
            return instance;
        }
        public IExpressionEditorInstance CreateExpressionEditor(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces, List<ModelItem> variables, string text, Type expressionType, System.Windows.Size initialSize)
        {
            MyExpressionEditorInstance instance = new MyExpressionEditorInstance();
            return instance;
        }
        public void UpdateContext(AssemblyContextControlItem assemblies, ImportedNamespaceContextItem importedNamespaces)
        {

        }

    }

    public class MyExpressionEditorInstance : IExpressionEditorInstance
    {
        private TextBox textBox = new TextBox();

        public bool AcceptsReturn { get; set; }
        public bool AcceptsTab { get; set; }
        public bool HasAggregateFocus
        {
            get
            {
                return true;
            }
        }

        public System.Windows.Controls.ScrollBarVisibility HorizontalScrollBarVisibility { get; set; }
        public System.Windows.Controls.Control HostControl
        {
            get
            {
                return textBox;
            }
        }
        public int MaxLines { get; set; }
        public int MinLines { get; set; }
        public string Text { get; set; }
        public System.Windows.Controls.ScrollBarVisibility VerticalScrollBarVisibility { get; set; }

        public event EventHandler Closing;
        public event EventHandler GotAggregateFocus;
        public event EventHandler LostAggregateFocus;
        public event EventHandler TextChanged;

        public bool CanCompleteWord()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanCopy()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanCut()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanDecreaseFilterLevel()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanGlobalIntellisense()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanIncreaseFilterLevel()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanParameterInfo()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanPaste()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanQuickInfo()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanRedo()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool CanUndo()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }

        public void ClearSelection()
        {
            MessageBox.Show(MethodBase.GetCurrentMethod().Name);
        }
        public void Close()
        {
            MessageBox.Show(MethodBase.GetCurrentMethod().Name);
        }
        public bool CompleteWord()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool Copy()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool Cut()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool DecreaseFilterLevel()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public void Focus()
        {
            MessageBox.Show(MethodBase.GetCurrentMethod().Name);
        }
        public string GetCommittedText()
        {
            return "CommittedText";
        }
        public bool GlobalIntellisense()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool IncreaseFilterLevel()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool ParameterInfo()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool Paste()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool QuickInfo()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool Redo()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
        public bool Undo()
        {
            return (MessageBox.Show(MethodBase.GetCurrentMethod().Name, "TestEditorInstance", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }
    }
}
