using coffee_bar_demo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
						if (textBoxName.Text.Equals("") || textBoxDescription.Text.Equals("") || textBoxPrice.Text.Equals("") || textBoxImagePath.Text.Equals("") || comboBoxType.SelectedItem == null)
						{
							throw new Exception("All fields are required.");
						}

						// Check if the image path is valid
						if (!File.Exists(textBoxImagePath.Text))
						{
							MessageBox.Show("The specified image path is not valid. Please select a valid image file.", "Invalid Image Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}

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

						// Check for duplicate names
						if (items.Any(i => i.Name.Equals(textBoxName.Text, StringComparison.OrdinalIgnoreCase) && i != item))
						{
							MessageBox.Show("An item with this name already exists. Please choose a different name.", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}

						// Copy the image file to the destination
						File.Copy(textBoxImagePath.Text, destinationImagePath, true);

						// Add the new item to the list
						items.Add(item);

						Image image = null;
						try
						{
							using (var tempImage = Image.FromFile(destinationImagePath))
							{
								image = new Bitmap(tempImage, new Size(128, 128));
							}
							imageList.Images.Add(image);
							imagePathIndexMap[item.ImagePath] = imageList.Images.Count - 1;
						}
						catch (Exception ex)
						{
							MessageBox.Show($"Error loading image");
							return;
						}

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
						return ;
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

				addMenuItemForm.KeyPreview = true;
				addMenuItemForm.KeyDown += (s, e) =>
				{
					if (e.KeyCode == Keys.Enter)
					{
						buttonSave.PerformClick();
					}
				};

				addMenuItemForm.ShowDialog();
			}
		}

		private void EditBarMenuItem(BarMenuItem selectedItem)
		{
			using (Form editMenuItemForm = new Form { Size = new Size(600, 330), Text = "Edit Menu Item", FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
			{
				// Font and Colors
				Font labelFont = new Font("Segoe UI", 10, FontStyle.Bold);
				Font textBoxFont = new Font("Segoe UI", 10);
				Color backgroundColor = Color.FromArgb(175, 143, 111); // Light Brown
				Color buttonColor = Color.FromArgb(101, 67, 33); // Medium Brown
				Color textColor = Color.White;

				editMenuItemForm.BackColor = backgroundColor;

				// Labels and TextBoxes
				Label labelName = new Label { Text = "Name:", ForeColor = textColor, Location = new Point(10, 10), AutoSize = true, Font = labelFont };
				TextBox textBoxName = new TextBox { Text = selectedItem.Name, Location = new Point(150, 10), Width = 400, Font = textBoxFont };

				Label labelDescription = new Label { Text = "Description:", ForeColor = textColor, Location = new Point(10, 50), AutoSize = true, Font = labelFont };
				TextBox textBoxDescription = new TextBox { Text = selectedItem.Description, Location = new Point(150, 50), Width = 400, Font = textBoxFont };

				Label labelPrice = new Label { Text = "Price:", ForeColor = textColor, Location = new Point(10, 90), AutoSize = true, Font = labelFont };
				TextBox textBoxPrice = new TextBox { Text = selectedItem.Price.ToString(), Location = new Point(150, 90), Width = 400, Font = textBoxFont };

				Label labelImagePath = new Label { Text = "Image Path:", ForeColor = textColor, Location = new Point(10, 130), AutoSize = true, Font = labelFont };
				TextBox textBoxImagePath = new TextBox { Text = selectedItem.ImagePath, Location = new Point(150, 130), Width = 300, Font = textBoxFont };

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
					Items = { "Coffee", "NonCoffee", "Dessert" },
					SelectedItem = selectedItem.Category.ToString()
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
					FlatStyle = FlatStyle.Flat
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
						if (textBoxName.Text.Equals("") || textBoxDescription.Text.Equals("") || textBoxPrice.Text.Equals("") || textBoxImagePath.Text.Equals("") || comboBoxType.SelectedItem == null)
						{
							throw new Exception("All fields are required.");
						}

						// Check if the image path is valid
						if (!File.Exists(textBoxImagePath.Text))
						{
							MessageBox.Show("The specified image path is not valid. Please select a valid image file.", "Invalid Image Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}

						// Get the base directory
						string basePath = AppDomain.CurrentDomain.BaseDirectory;

						// Define the relative path for the image
						string relativeImagePath = Path.Combine("Images", Path.GetFileName(textBoxImagePath.Text));
						string destinationImagePath = Path.Combine(basePath, relativeImagePath);

						// Check for duplicate names
						if (items.Any(i => i.Name.Equals(textBoxName.Text, StringComparison.OrdinalIgnoreCase) && i != selectedItem))
						{
							MessageBox.Show("An item with this name already exists. Please choose a different name.", "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}

						// Store the old image path
						string oldImagePath = selectedItem.ImagePath;

						// Update the item properties
						selectedItem.Name = textBoxName.Text;
						selectedItem.Description = textBoxDescription.Text;
						selectedItem.Price = decimal.Parse(textBoxPrice.Text);
						selectedItem.Category = (ItemType)Enum.Parse(typeof(ItemType), comboBoxType.SelectedItem.ToString());

						// Update the image path if it has changed
						if (!selectedItem.ImagePath.Equals(relativeImagePath, StringComparison.OrdinalIgnoreCase))
						{
							// Remove old image if it's no longer used by any other items
							if (imagePathIndexMap.ContainsKey(selectedItem.ImagePath) && !items.Any(i => i.ImagePath == selectedItem.ImagePath))
							{
								imagePathIndexMap.Remove(selectedItem.ImagePath);
							}

							selectedItem.ImagePath = relativeImagePath;
							File.Copy(textBoxImagePath.Text, destinationImagePath, true);

							// Update the image list and dictionary
							using (var image = Image.FromFile(destinationImagePath))
							{
								var resizedImage = new Bitmap(image, new Size(128, 128));
								imageList.Images.Add(resizedImage);
								imagePathIndexMap[selectedItem.ImagePath] = imageList.Images.Count - 1;
							}

							// Delete the old image if it's not used by any other items
							if (!items.Any(i => i.ImagePath == oldImagePath))
							{
								string oldImageFullPath = Path.Combine(basePath, oldImagePath);
								if (File.Exists(oldImageFullPath))
								{
									File.Delete(oldImageFullPath);
								}
							}
						}

						// Save the updated list
						SaveMenuItemData();

						MessageBox.Show("Menu item updated successfully!");

						// Update the list views
						UpdateAdminDataGridView();
						UpdateMenuListView();

						// Close the edit menu item form
						editMenuItemForm.Close();
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Error updating item: {ex.Message}");
						return;
					}
				};

				// Adding controls to the form
				editMenuItemForm.Controls.Add(labelName);
				editMenuItemForm.Controls.Add(textBoxName);
				editMenuItemForm.Controls.Add(labelDescription);
				editMenuItemForm.Controls.Add(textBoxDescription);
				editMenuItemForm.Controls.Add(labelPrice);
				editMenuItemForm.Controls.Add(textBoxPrice);
				editMenuItemForm.Controls.Add(labelImagePath);
				editMenuItemForm.Controls.Add(textBoxImagePath);
				editMenuItemForm.Controls.Add(buttonSelectImage);
				editMenuItemForm.Controls.Add(labelType);
				editMenuItemForm.Controls.Add(comboBoxType);
				editMenuItemForm.Controls.Add(buttonSave);

				editMenuItemForm.KeyPreview = true;
				editMenuItemForm.KeyDown += (s, e) =>
				{
					if (e.KeyCode == Keys.Enter)
					{
						buttonSave.PerformClick();
					}
				};

				// Show the edit menu item form as a dialog
				editMenuItemForm.ShowDialog();
			}
		}

		private void DeleteSelectedBarMenuItem()
		{
			if (dataGridViewAdmin.SelectedRows.Count > 0)
			{
				// Get the selected row
				DataGridViewRow selectedRow = dataGridViewAdmin.SelectedRows[0];
				var selectedItemName = Convert.ToString(selectedRow.Cells["Name"].Value);
				var selectedItem = items.FirstOrDefault(i => i.Name == selectedItemName);

				if (selectedItem != null)
				{
					// Check if the image is used by more than one item
					bool isImageUsedByOthers = IsImageUsedByOtherItems(selectedItem.ImagePath);

					// Remove the item
					items.Remove(selectedItem);

					if (!isImageUsedByOthers)
					{
						// Delete the image file if it's not used by any other items
						string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedItem.ImagePath);
						if (File.Exists(imagePath))
						{
							File.Delete(imagePath);
						}

						// Remove the image from the image list and image path index map
						if (imagePathIndexMap.ContainsKey(selectedItem.ImagePath))
						{
							int imageIndex = imagePathIndexMap[selectedItem.ImagePath];
							imageList.Images.RemoveAt(imageIndex);
							imagePathIndexMap.Remove(selectedItem.ImagePath);

							// Update indices in the map
							foreach (var key in imagePathIndexMap.Keys.ToList())
							{
								if (imagePathIndexMap[key] > imageIndex)
								{
									imagePathIndexMap[key]--;
								}
							}
						}
					}

					// Save the updated list
					SaveMenuItemData();

					// Update the list views
					UpdateAdminDataGridView();
					UpdateMenuListView();
				}
			}
			else
			{
				MessageBox.Show("Please select an item to delete.");
			}
		}

		private bool IsImageUsedByOtherItems(string imagePath)
		{
			return items.Count(item => item.ImagePath == imagePath) > 1;
		}
	}
}