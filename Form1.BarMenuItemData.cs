using coffee_bar_demo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace coffe_bar_demo
{
    public partial class Form1
    {
        private void InitializeBarMenuData()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string imagesFolder = Path.Combine(basePath, "Images");
            string dataFilePath = Path.Combine(basePath, "menuItems.dat");

            if (File.Exists(dataFilePath))
            {
                try
                {
                    using (FileStream stream = new FileStream(dataFilePath, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        items = (List<BarMenuItem>)formatter.Deserialize(stream);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading menu item data: {ex.Message}");
                    items = new List<BarMenuItem>();
                }
            }
            else
            {
                items = new List<BarMenuItem>
                {
                    new BarMenuItem { Name = "Espresso", Description = "Strong and bold", ImagePath = Path.Combine(imagesFolder, "espresso.jpg"), Price = 2.99m, Category = ItemType.Coffee },
                    new BarMenuItem { Name = "Latte", Description = "Smooth and creamy",  ImagePath = Path.Combine(imagesFolder, "latte.jpg"), Price = 3.49m, Category = ItemType.Coffee },
                    new BarMenuItem { Name = "Cappuccino", Description = "Frothy and rich", ImagePath = Path.Combine(imagesFolder, "cappuccino.jpg"), Price = 3.99m, Category = ItemType.Coffee },
                };
            }

            imageList = new ImageList
            {
                ImageSize = new Size(128, 128), // Set the desired image size
                ColorDepth = ColorDepth.Depth32Bit
            };

            foreach (var item in items)
            {
                string fullImagePath = Path.Combine(basePath, item.ImagePath);
                if (File.Exists(fullImagePath))
                {
                    using (Image image = Image.FromFile(fullImagePath))
                    {
                        imageList.Images.Add(new Bitmap(image, new Size(128, 128))); // Ensure image is resized to fit ImageList size
                        imagePathIndexMap[item.ImagePath] = imageList.Images.Count - 1; // Store the index of the image
                    }
                }
                else
                {
                    imageList.Images.Add(new Bitmap(128, 128));
                    imagePathIndexMap[item.ImagePath] = imageList.Images.Count - 1; // Store the index of the placeholder image
                }
            }
        }

        private void SaveMenuItemData()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menuItems.dat");
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, items);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving menu item data: {ex.Message}");
            }
        }

        private void AddNewBarMenuItem()
        {
            using (Form addMenuItemForm = new Form { Size = new Size(600, 330), Text = "Add Menu Item", FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                // Font and Colors
                Font labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
                Font textBoxFont = new Font("Segoe UI", 10);
                Color backgroundColor = Color.FromArgb(175, 143, 111); // Light Brown
                Color buttonColor = Color.FromArgb(101, 67, 33); // Medium Brown
                Color textColor = Color.White;

                addMenuItemForm.BackColor = backgroundColor;

                // Labels and TextBoxes
                Label labelName = new Label { Text = "Name:", ForeColor = textColor, Location = new Point(10, 10), AutoSize = true, Font = labelFont };
                TextBox textBoxName = new TextBox { Location = new Point(150, 10), Width = 400, Font = textBoxFont };

                Label labelDescription = new Label { Text = "Description:", ForeColor = textColor, Location = new Point(10, 50), AutoSize = true, Font = labelFont };
                TextBox textBoxDescription = new TextBox { Location = new Point(150, 50), Width = 400, Font = textBoxFont };

                Label labelPrice = new Label { Text = "Price:", ForeColor = textColor, Location = new Point(10, 90), AutoSize = true, Font = labelFont };
                TextBox textBoxPrice = new TextBox { Location = new Point(150, 90), Width = 400, Font = textBoxFont };

                Label labelImagePath = new Label { Text = "Image Path:", ForeColor = textColor, Location = new Point(10, 130), AutoSize = true, Font = labelFont };
                TextBox textBoxImagePath = new TextBox { Location = new Point(150, 130), Width = 300, Font = textBoxFont };
                Button buttonSelectImage = new Button
                {
                    Text = "Select Image",
                    Location = new Point(460, 130),
                    Width = 90,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    BackColor = textColor,
                    ForeColor = buttonColor,
                    FlatStyle = FlatStyle.Flat
                };

                Label labelType = new Label { Text = "Type:", ForeColor = textColor, Location = new Point(10, 170), AutoSize = true, Font = labelFont };
                ComboBox comboBoxType = new ComboBox
                {
                    Location = new Point(150, 170),
                    Width = 400,
                    Font = textBoxFont,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Items = { "Coffee", "NonCoffee", "Dessert" }
                };

                Button buttonSave = new Button
                {
                    Text = "Save",
                    Location = new Point(250, 230),
                    Width = 100,
                    Height = 40,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    BackColor = textColor,
                    ForeColor = buttonColor,     
                    FlatStyle = FlatStyle.Flat,
                };

                // Event handlers
                buttonSelectImage.Click += (s, e) =>
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            textBoxImagePath.Text = openFileDialog.FileName;
                        }
                    }
                };

                buttonSave.Click += (s, e) =>
                {
                    try
                    {
                        // Get the base directory
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;

                        // Define the relative path for the image
                        string relativeImagePath = Path.Combine("Images", Path.GetFileName(textBoxImagePath.Text));
                        string destinationImagePath = Path.Combine(basePath, relativeImagePath);

                        BarMenuItem item = new BarMenuItem
                        {
                            Name = textBoxName.Text,
                            Description = textBoxDescription.Text,
                            ImagePath = relativeImagePath,
                            Price = decimal.Parse(textBoxPrice.Text),
                            Category = (ItemType)Enum.Parse(typeof(ItemType), comboBoxType.SelectedItem.ToString())
                        };

                        // Copy the image file to the destination
                        File.Copy(textBoxImagePath.Text, destinationImagePath, true);

                        // Add the new item to the list
                        items.Add(item);

                        // Load the image and add it to the ImageList
                        Image image = Image.FromFile(destinationImagePath);
                        imageList.Images.Add(image);

                        // Update the dictionary with the new image index
                        imagePathIndexMap[item.ImagePath] = imageList.Images.Count - 1;

                        // Save the updated list
                        SaveMenuItemData();

                        MessageBox.Show("New menu item added successfully!");

                        // Update the list views
                        UpdateAdminDataGridView();
                        UpdateMenuListView();

                        // Close the add menu item form
                        addMenuItemForm.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding new item: {ex.Message}");
                    }
                };

                // Adding controls to the form
                addMenuItemForm.Controls.Add(labelName);
                addMenuItemForm.Controls.Add(textBoxName);
                addMenuItemForm.Controls.Add(labelDescription);
                addMenuItemForm.Controls.Add(textBoxDescription);
                addMenuItemForm.Controls.Add(labelPrice);
                addMenuItemForm.Controls.Add(textBoxPrice);
                addMenuItemForm.Controls.Add(labelImagePath);
                addMenuItemForm.Controls.Add(textBoxImagePath);
                addMenuItemForm.Controls.Add(buttonSelectImage);
                addMenuItemForm.Controls.Add(labelType);
                addMenuItemForm.Controls.Add(comboBoxType);
                addMenuItemForm.Controls.Add(buttonSave);

                addMenuItemForm.ShowDialog();
            }
        }



        private void EditBarMenuItem(BarMenuItem itemToEdit = null)
        {
            using (Form editBarMenuItemForm = new Form { Size = new Size(600, 330), Text = itemToEdit == null ? "Add Menu Item" : "Edit Menu Item", FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                // Font and Colors
                Font labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
                Font textBoxFont = new Font("Segoe UI", 10);
                Color backgroundColor = Color.FromArgb(175, 143, 111); // Light Brown
                Color buttonColor = Color.FromArgb(101, 67, 33); // Medium Brown
                Color textColor = Color.White;

                editBarMenuItemForm.BackColor = backgroundColor;

                // Labels and TextBoxes
                Label labelName = new Label { Text = "Name:", ForeColor = textColor, Location = new Point(10, 10), AutoSize = true, Font = labelFont };
                TextBox textBoxName = new TextBox { Location = new Point(150, 10), Width = 400, Font = textBoxFont };

                Label labelDescription = new Label { Text = "Description:", ForeColor = textColor, Location = new Point(10, 50), AutoSize = true, Font = labelFont };
                TextBox textBoxDescription = new TextBox { Location = new Point(150, 50), Width = 400, Font = textBoxFont };

                Label labelPrice = new Label { Text = "Price:", ForeColor = textColor, Location = new Point(10, 90), AutoSize = true, Font = labelFont };
                TextBox textBoxPrice = new TextBox { Location = new Point(150, 90), Width = 400, Font = textBoxFont };

                Label labelImagePath = new Label { Text = "Image Path:", ForeColor = textColor, Location = new Point(10, 130), AutoSize = true, Font = labelFont };
                TextBox textBoxImagePath = new TextBox { Location = new Point(150, 130), Width = 300, Font = textBoxFont };
                Button buttonSelectImage = new Button
                {
                    Text = "Select Image",
                    Location = new Point(460, 130),
                    Width = 90,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    BackColor = textColor,
                    ForeColor = buttonColor,
                    FlatStyle = FlatStyle.Flat
                };

                Label labelType = new Label { Text = "Type:", ForeColor = textColor, Location = new Point(10, 170), AutoSize = true, Font = labelFont };
                ComboBox comboBoxType = new ComboBox
                {
                    Location = new Point(150, 170),
                    Width = 400,
                    Font = textBoxFont,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Items = { "Coffee", "NonCoffee", "Dessert" }
                };

                Button buttonSave = new Button
                {
                    Text = "Save",
                    Location = new Point(250, 230),
                    Width = 100,
                    Height = 40,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    BackColor = textColor,
                    ForeColor = buttonColor,
                    FlatStyle = FlatStyle.Flat,
                };

                // Initialize form controls based on whether editing an existing item
                if (itemToEdit != null)
                {
                    textBoxName.Text = itemToEdit.Name;
                    textBoxDescription.Text = itemToEdit.Description;
                    textBoxPrice.Text = itemToEdit.Price.ToString();
                    textBoxImagePath.Text = itemToEdit.ImagePath;
                    comboBoxType.SelectedItem = itemToEdit.Category.ToString();
                }

                // Event handlers
                buttonSelectImage.Click += (s, e) =>
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            textBoxImagePath.Text = openFileDialog.FileName;
                        }
                    }
                };

                buttonSave.Click += (s, e) =>
                {
                    try
                    {
                        // Get the base directory
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;

                        // Define the relative path for the image
                        string relativeImagePath = Path.Combine("Images", Path.GetFileName(textBoxImagePath.Text));
                        string destinationImagePath = Path.Combine(basePath, relativeImagePath);
                        string oldDestinationImagePath = Path.Combine(basePath, itemToEdit.ImagePath);

                        // Create new or update existing item
                        if (itemToEdit == null)
                        {
                            BarMenuItem newItem = new BarMenuItem
                            {
                                Name = textBoxName.Text,
                                Description = textBoxDescription.Text,
                                ImagePath = relativeImagePath,
                                Price = decimal.Parse(textBoxPrice.Text),
                                Category = (ItemType)Enum.Parse(typeof(ItemType), comboBoxType.SelectedItem.ToString())
                            };
                            items.Add(newItem);

                            // Copy image file to the destination path
                            File.Copy(textBoxImagePath.Text, destinationImagePath, true);

                            // Add new image to image list
                            Image image = Image.FromFile(destinationImagePath);
                            imageList.Images.Add(new Bitmap(image, new Size(128, 128)));
                            imagePathIndexMap[newItem.ImagePath] = imageList.Images.Count - 1;
                        }
                        else
                        {
                            // Update existing item
                            itemToEdit.Name = textBoxName.Text;
                            itemToEdit.Description = textBoxDescription.Text;
                            itemToEdit.Price = decimal.Parse(textBoxPrice.Text);
                            itemToEdit.Category = (ItemType)Enum.Parse(typeof(ItemType), comboBoxType.SelectedItem.ToString());

                            if (itemToEdit.ImagePath != relativeImagePath)
                            {
                                // Copy image file to the destination path
                                File.Copy(textBoxImagePath.Text, destinationImagePath, true);

                                // Replace the old image with the new one in the image list
                                if (imagePathIndexMap.TryGetValue(itemToEdit.ImagePath, out int oldImageIndex))
                                {
                                    Image image = Image.FromFile(destinationImagePath);
                                    imageList.Images[oldImageIndex] = new Bitmap(image, new Size(128, 128));
                                    imagePathIndexMap.Remove(itemToEdit.ImagePath);
                                    imagePathIndexMap[relativeImagePath] = oldImageIndex;
                                }

                                // Update the image path in the item
                                itemToEdit.ImagePath = relativeImagePath;                                

                                if (File.Exists(oldDestinationImagePath))
                                {
                                    File.Delete(oldDestinationImagePath);
                                }
                                else
                                {
                                    MessageBox.Show($"Failed to delete image file '{oldDestinationImagePath}'. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }

                        // Save and update list views
                        SaveMenuItemData();
                        UpdateAdminDataGridView();
                        UpdateMenuListView();

                        MessageBox.Show(itemToEdit == null ? "New menu item added successfully!" : "Menu item updated successfully!");

                        editBarMenuItemForm.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error {(itemToEdit == null ? "adding new item" : "updating item")}: {ex.Message}");
                    }
                };


                // Adding controls to the form
                editBarMenuItemForm.Controls.Add(labelName);
                editBarMenuItemForm.Controls.Add(textBoxName);
                editBarMenuItemForm.Controls.Add(labelDescription);
                editBarMenuItemForm.Controls.Add(textBoxDescription);
                editBarMenuItemForm.Controls.Add(labelPrice);
                editBarMenuItemForm.Controls.Add(textBoxPrice);
                editBarMenuItemForm.Controls.Add(labelImagePath);
                editBarMenuItemForm.Controls.Add(textBoxImagePath);
                editBarMenuItemForm.Controls.Add(buttonSelectImage);
                editBarMenuItemForm.Controls.Add(labelType);
                editBarMenuItemForm.Controls.Add(comboBoxType);
                editBarMenuItemForm.Controls.Add(buttonSave);

                editBarMenuItemForm.ShowDialog();
            }
        }

        void DeleteSelectedBarMenuItem()
        {
            if (dataGridViewAdmin.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow selectedRow in dataGridViewAdmin.SelectedRows)
                {
                    var itemName = Convert.ToString(selectedRow.Cells["Name"].Value);
                    var item = items.FirstOrDefault(i => i.Name == itemName);

                    if (item != null)
                    {
                        // Remove the item from the list
                        items.Remove(item);

                        // Remove the DataGridView row
                        dataGridViewAdmin.Rows.Remove(selectedRow);

                        // Remove the image from the ImageList
                        if (imagePathIndexMap.TryGetValue(item.ImagePath, out int imageIndex))
                        {
                            imageList.Images.RemoveAt(imageIndex);
                            imagePathIndexMap.Remove(item.ImagePath);

                            // Update indices in the dictionary
                            var keysToUpdate = imagePathIndexMap.Where(kv => kv.Value > imageIndex).Select(kv => kv.Key).ToList();
                            foreach (var key in keysToUpdate)
                            {
                                imagePathIndexMap[key]--;
                            }
                        }
                        else
                        {
                            throw new Exception("Image index not found in the map.");
                        }

                        string basePath = AppDomain.CurrentDomain.BaseDirectory;
                        string imagePath = Path.Combine(basePath, item.ImagePath);

                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }
                        else
                        {
                            MessageBox.Show($"Failed to delete image file '{imagePath}'. Please try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                
                SaveMenuItemData();

                
                UpdateMenuListView();
                UpdateAdminDataGridView(); 
            }
        }







    }
}