using System.Windows.Forms;

namespace WindowsForms
{
    using ReactiveUI;
    using ReactiveUI.Validation.Extensions;
    using ViewModels;

    public partial class Form1 : Form, IViewFor<MainViewModel>
    {
        public Form1()
        {
            InitializeComponent();
            this.ViewModel = new MainViewModel();
            this.WhenActivated(d =>
            {
                this.Bind(this.ViewModel, vm => vm.Cif, v => v.textBox1.Text);
                this.BindValidation(this.ViewModel, vm => vm.Cif, v => v.label1.Text);
            });
        }

        object IViewFor.ViewModel
        {
            get => this.ViewModel;
            set => this.ViewModel = (MainViewModel)value;
        }

        public MainViewModel ViewModel { get; set; }
    }
}
