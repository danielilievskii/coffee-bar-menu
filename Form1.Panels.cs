using coffee_bar_demo;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;



namespace coffe_bar_demo
{
    public partial class Form1
    {
        private Panel panelMenu;
        private Panel panelAdmin;

        private Panel panelHome;
        private Button buttonOpenMenu;
        private Button buttonAdmin;
         
        private DataGridView dataGridViewAdmin;        

        private ListView listViewCoffee;
        private ListView listViewNonCoffee;
        private ListView listViewDessert;


        private void InitializePanels()
        {
            panelHome = new Panel { Dock = DockStyle.Fill, BackColor = Color.Beige };
            string backgroundImageFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "background2.jpg");
            if (File.Exists(backgroundImageFile))
            {
                panelHome.BackgroundImage = System.Drawing.Image.FromFile(backgroundImageFile);
                panelHome.BackgroundImageLayout = ImageLayout.Stretch;
            }            

            InitializeMenuPanel();
            InitializeAdminPanel();

            Controls.Add(panelHome);
            Controls.Add(panelMenu);
            Controls.Add(panelAdmin);
        }

        private void InitializeButtons()
        {
            buttonOpenMenu = new System.Windows.Forms.Button
            {
                Text = "Menu",
                Size = new Size(150, 50),
                Location = new Point((panelHome.Width - 150) / 2, (panelHome.Height - 50) / 2 - 40),
                BackColor = Color.FromArgb(175, 143, 111),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, BorderColor = Color.White },
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
            };

            buttonAdmin = new System.Windows.Forms.Button
            {
                Text = "Admin Panel",
                Size = new Size(150, 50),
                Location = new Point((panelHome.Width - 150) / 2, (panelHome.Height - 50) / 2 + 40),
                BackColor = Color.FromArgb(175, 143, 111),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, BorderColor = Color.White },
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
            };

            panelHome.Controls.Add(buttonOpenMenu);
            panelHome.Controls.Add(buttonAdmin);

            buttonOpenMenu.Click += (s, e) => ShowPanel(panelMenu);
            buttonAdmin.Click += (s, e) => ShowPanel(panelAdmin);
        }

        private void ShowPanel(Panel panelToShow)
        {
            panelMenu.Visible = false;
            panelAdmin.Visible = false;
            panelHome.Visible = false;
            panelToShow.Visible = true;
        }

        private void ExitMenu()
        {
            ShowPanel(panelHome);
        }

        private void InitializeMenuPanel()
        {
            panelMenu = new Panel { Dock = DockStyle.Fill };
            Controls.Add(panelMenu);

            // Create and configure TabControl
            TabControl tabControlMenu = new TabControl
            {
                Dock = DockStyle.Top,
                Height = panelMenu.Height - 70,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
            };

            TabPage tabPageCoffee = new TabPage("Coffee");
            TabPage tabPageNonCoffee = new TabPage("Non-Coffee");
            TabPage tabPageDessert = new TabPage("Dessert");

            // Create ListView for each TabPage
            listViewCoffee = new System.Windows.Forms.ListView
            {
                Dock = DockStyle.Fill,
                LargeImageList = imageList,
                View = View.Tile,
                BackColor = Color.SeaShell,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                TileSize = new Size(250, 170)
            };            
            listViewCoffee.Columns.Add("Name", 200, HorizontalAlignment.Left);
            listViewCoffee.Columns.Add("Description", 400, HorizontalAlignment.Left);
            listViewCoffee.Columns.Add("Price", 80, HorizontalAlignment.Left);

            

            listViewNonCoffee = new ListView
            {
                Dock = DockStyle.Fill,
                LargeImageList = imageList,
                View = View.Tile,
                BackColor = Color.SeaShell,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                 TileSize = new Size(250, 170)
            };
            listViewNonCoffee.Columns.Add("Name", 200, HorizontalAlignment.Left);
            listViewNonCoffee.Columns.Add("Description", 400, HorizontalAlignment.Left);
            listViewNonCoffee.Columns.Add("Price", 80, HorizontalAlignment.Left);

            listViewDessert = new ListView
            {
                Dock = DockStyle.Fill,
                LargeImageList = imageList,
                View = View.Tile,
                BackColor = Color.SeaShell,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                 TileSize = new Size(250, 170)
            };
            listViewDessert.Columns.Add("Name", 200, HorizontalAlignment.Left);
            listViewDessert.Columns.Add("Description", 400, HorizontalAlignment.Left);
            listViewDessert.Columns.Add("Price", 80, HorizontalAlignment.Left);

            // Add ListView to respective TabPage
            tabPageCoffee.Controls.Add(listViewCoffee);
            tabPageNonCoffee.Controls.Add(listViewNonCoffee);
            tabPageDessert.Controls.Add(listViewDessert);

            // Add TabPages to TabControl
            tabControlMenu.TabPages.Add(tabPageCoffee);
            tabControlMenu.TabPages.Add(tabPageNonCoffee);
            tabControlMenu.TabPages.Add(tabPageDessert);

            // Add TabControl to panelMenu
            panelMenu.Controls.Add(tabControlMenu);

            // Create and configure ComboBox for sorting
            ComboBox comboBoxSort = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Items = { "Price: Low to High", "Price: High to Low", "Name: A-Z", "Name: Z-A" },
                SelectedIndex = 0,
                Location = new Point(10, panelMenu.Height - 60),
                Width = 150,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(255, 235, 205), // Light color
                ForeColor = Color.FromArgb(80, 60, 40), // Darker color for text
                FlatStyle = FlatStyle.Standard, // Make it flat styled
            };
            comboBoxSort.SelectedIndexChanged += (s, e) => ApplySort(comboBoxSort.SelectedIndex);
            panelMenu.Controls.Add(comboBoxSort);

            // TUKA KOPCE

            // Create and configure "Exit" button
            Button buttonExitMenu = new Button
            {
                Text = "Exit",
                Size = new Size(150, 50),
                Location = new Point(panelMenu.Width - 160, panelMenu.Height - 60), // Adjusted to the right end
                BackColor = Color.FromArgb(175, 143, 111),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 3, BorderColor = Color.White },
            };
            buttonExitMenu.Click += (s, e) => ExitMenu();
            panelMenu.Controls.Add(buttonExitMenu);
        }


        private void ApplySort(int sortIndex)
        {
            // Sort items based on sortIndex
            switch (sortIndex)
            {
                case 0: // Price: Low to High
                    items.Sort((x, y) => x.Price.CompareTo(y.Price));
                    break;
                case 1: // Price: High to Low
                    items.Sort((x, y) => y.Price.CompareTo(x.Price));
                    break;
                case 2: // Name: A-Z
                    items.Sort((x, y) => string.Compare(x.Name, y.Name));
                    break;
                case 3: // Name: Z-A
                    items.Sort((x, y) => string.Compare(y.Name, x.Name));
                    break;
            }

            // Update imagePathIndexMap and imageList according to the sorted items
            imagePathIndexMap.Clear();
            imageList.Images.Clear();

            foreach (var item in items)
            {
                string fullImagePath = item.ImagePath;
                if (File.Exists(fullImagePath))
                {
                    using (Image image = Image.FromFile(fullImagePath))
                    {
                        imageList.Images.Add(new Bitmap(image, new Size(128, 128)));
                        imagePathIndexMap[item.ImagePath] = imageList.Images.Count - 1;
                    }
                }
                else
                {
                    imageList.Images.Add(new Bitmap(128, 128));
                    imagePathIndexMap[item.ImagePath] = imageList.Images.Count - 1;
                }
            }

            // Call UpdateMenuListView to refresh the ListView controls
            UpdateMenuListView();
            UpdateAdminDataGridView();
        }

        private void UpdateMenuListView()
        {
            // Clear all items from each ListView
            listViewCoffee.Items.Clear();
            listViewNonCoffee.Items.Clear();
            listViewDessert.Items.Clear();

            // Re-add items to the appropriate ListView based on their category
            foreach (var item in items)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Text = item.Name; // Set the main text (first column in Tile view)
                listViewItem.ImageIndex = imagePathIndexMap[item.ImagePath]; // Use the stored image index
                listViewItem.SubItems.Add(item.Price.ToString("C"));
                listViewItem.SubItems.Add(item.Description);
                switch (item.Category)
                {
                    case ItemType.Coffee:
                        listViewCoffee.Items.Add(listViewItem);
                        break;
                    case ItemType.NonCoffee:
                        listViewNonCoffee.Items.Add(listViewItem);
                        break;
                    case ItemType.Dessert:
                        listViewDessert.Items.Add(listViewItem);
                        break;
                }
            }

            listViewCoffee.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }



        private void InitializeAdminPanel()
        {
            panelAdmin = new Panel
            {
                Dock = DockStyle.Fill,
                Height = panelMenu.Height - 70, // Adjust height to fit buttons below
                Font = new Font("Segoe UI", 10),
            };
            Controls.Add(panelAdmin);

            dataGridViewAdmin = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = panelAdmin.Height - 70,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoGenerateColumns = false,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackgroundColor = Color.SeaShell,
            };
            dataGridViewAdmin.Columns.Add("Name", "Name");
            dataGridViewAdmin.Columns.Add("Description", "Description");
            dataGridViewAdmin.Columns.Add("Image Path", "Image Path");
            dataGridViewAdmin.Columns.Add("Price", "Price");
            dataGridViewAdmin.Columns.Add("Category", "Category");
            dataGridViewAdmin.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewAdmin.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            foreach (var barMenuItem in items)
            {
                dataGridViewAdmin.Rows.Add(barMenuItem.Name, barMenuItem.Description, barMenuItem.ImagePath,
                                            barMenuItem.Price.ToString("C"), barMenuItem.Category.ToString());
            }
            panelAdmin.Controls.Add(dataGridViewAdmin);

            Button buttonAddCoffee = new Button
            {
                Text = "Add Item",
                Size = new Size(120, 40),
                Location = new Point(10, dataGridViewAdmin.Bottom + 10),
                BackColor = Color.FromArgb(175, 143, 111),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 3, BorderColor = Color.White },
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
            };
            buttonAddCoffee.Click += (s, e) => AddNewBarMenuItem();
            panelAdmin.Controls.Add(buttonAddCoffee);

            Button buttonEditItem = new Button
            {
                Text = "Edit Item",
                Size = new Size(120, 40),
                Location = new Point(buttonAddCoffee.Right + 10, dataGridViewAdmin.Bottom + 10),
                BackColor = Color.FromArgb(175, 143, 111),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 3, BorderColor = Color.White },
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
            };
            buttonEditItem.Click += (s, e) =>
            {
                if (dataGridViewAdmin.SelectedRows.Count > 0)
                {
                    // Get the selected row
                    DataGridViewRow selectedRow = dataGridViewAdmin.SelectedRows[0];
                    var selectedItemName = Convert.ToString(selectedRow.Cells["Name"].Value);
                    var selectedItemObject = items.FirstOrDefault(i => i.Name == selectedItemName);

                    if (selectedItemObject != null)
                    {
                        EditBarMenuItem(selectedItemObject);
                    }
                    else
                    {
                        MessageBox.Show("Please select an item to edit.");
                    }
                }
            };
            panelAdmin.Controls.Add(buttonEditItem);

            Button buttonDeleteItem = new Button
            {
                Text = "Delete Item",
                Size = new Size(120, 40),
                Location = new Point(buttonEditItem.Right + 10, dataGridViewAdmin.Bottom + 10),
                BackColor = Color.FromArgb(175, 143, 111),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 3, BorderColor = Color.White },
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
            };
            buttonDeleteItem.Click += (s, e) => DeleteSelectedBarMenuItem();
            panelAdmin.Controls.Add(buttonDeleteItem);

            Button buttonExitMenu = new Button
            {
                Text = "Exit",
                Size = new Size(150, 50),
                Location = new Point(panelAdmin.Width - 160, dataGridViewAdmin.Bottom + 10),
                BackColor = Color.FromArgb(175, 143, 111),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 3, BorderColor = Color.White },
                UseVisualStyleBackColor = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
            };
            buttonExitMenu.Click += (s, e) => ExitMenu();
            panelAdmin.Controls.Add(buttonExitMenu);
        }

        private void UpdateAdminDataGridView()
        {
            dataGridViewAdmin.Rows.Clear();

            foreach (var barMenuItem in items)
            {
                dataGridViewAdmin.Rows.Add(barMenuItem.Name, barMenuItem.Description, barMenuItem.ImagePath,
                                            barMenuItem.Price.ToString("C"), barMenuItem.Category.ToString());
            }
        }
    }
}