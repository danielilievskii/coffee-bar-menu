using System.Collections.Generic;
using System.Windows.Forms;
using coffee_bar_demo;

namespace coffe_bar_demo
{
    public partial class Form1 : Form
    {
        private List<BarMenuItem> items;
        private ImageList imageList;
        private Dictionary<string, int> imagePathIndexMap = new Dictionary<string, int>();

        public Form1()
        {
            InitializeComponent();
			InitializeBarMenuData();
            InitializePanels();
            InitializeButtons();
            ShowPanel(panelHome);
            UpdateAdminDataGridView(); 
            UpdateMenuListView();
            
			this.KeyPreview = true;
			this.KeyDown += new KeyEventHandler(Form_KeyDown);
		}
     }
}