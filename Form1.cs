using iText.IO.Image;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using iText.Layout;
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
        }

       
     }
}