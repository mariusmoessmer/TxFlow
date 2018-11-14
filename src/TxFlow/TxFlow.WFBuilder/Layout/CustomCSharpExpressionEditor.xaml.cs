using System;
using System.Activities.Presentation.Expressions;
using System.Windows;

namespace TxFlow.WFBuilder.Layout
{
    /// <summary>
    /// Interaction logic for CustomCSharpExpressionEditor.xaml
    /// </summary>
    public partial class CustomCSharpExpressionEditor : System.Activities.Presentation.Expressions.TextualExpressionEditor
    {
        private static readonly DependencyProperty textProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomCSharpExpressionEditor));

        public CustomCSharpExpressionEditor()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            switch (e.Property.Name)
            {
                case "Expression":
                    {
                        var ex = this.Expression;
                        if(ex != null)
                        {
                            this.Text = ex.Content != null ? ex.Content.ComputedValue.ToString() : ex.ToString();
                        }

                        
                        break;
                    }

                    
            }

            base.OnPropertyChanged(e);
        }

        internal static DependencyProperty TextProperty
        {
            get
            {
                return textProperty;
            }
        }

        public override bool Commit(bool isExplicitCommit)
        {
            return true;
        }
    }
}
