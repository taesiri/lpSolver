using System;
using System.Collections.Generic;

namespace LinearProgramming
{
    /// <summary>
    /// Interaction logic for DocumentOutline.xaml
    /// </summary>
    public partial class DocumentOutline
    {
        public DocumentOutline()
        {
            InitializeComponent();
        }


        public void DoUpdate(IEnumerable<string> variables)
        {
            lboxVariables.Items.Clear();

            try
            {
                foreach (string variable in variables)
                {
                    lboxVariables.Items.Add(variable);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}