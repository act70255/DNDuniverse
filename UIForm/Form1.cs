namespace UIForm
{
    public partial class Form1 : Form
    {
        DND.Domain.Service.Interface.ITerrariaService service;
        public Form1(DND.Domain.Service.Interface.ITerrariaService _service)
        {
            InitializeComponent();
            service = _service;

            var ddd = service.GetCreatures();
            var ddd2 = service.GetSkills();
            var ddc = ddd.Count();
        }
    }
}
