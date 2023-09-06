using System.Runtime.CompilerServices;
using ViewModel;
namespace UserInterface
{
    public partial class Form1 : Form
    {
        ModelViewRequirementBase MV;
        public Form1()
        {
            MV = new ModelViewRequirementBase();
            InitializeComponent();
            DataBind();
        }
        private void DataBind()
        {
            MoreButton.DataBindings.Add("Visible", MV, "More");
        }
        private void CreateLinkTable(int row)
        {

        }




    }
}