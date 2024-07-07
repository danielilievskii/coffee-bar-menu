using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Layout.Properties;
using System;
using System.Windows.Forms;
using iText.Kernel.Pdf.Canvas.Draw;
using System.Collections.Generic;
using System.IO;
using coffee_bar_demo;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using iText.StyledXmlParser.Jsoup.Nodes;
using iText.Kernel.Font;
using System.Linq;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Geom;

namespace coffe_bar_demo
{
	public partial class Form1
	{
		private void buttonGeneratePDF_Click(object sender, EventArgs e)
		{
			GeneratePDF(items);
		}
		private void GeneratePDF(List<BarMenuItem> items)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "PDF files (*.pdf)|*.pdf",
				Title = "Save menu as PDF"
			};

			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
					string pdfPath = saveFileDialog.FileName;
					using (PdfWriter writer = new PdfWriter(pdfPath))
					{
						PdfDocument pdf = new PdfDocument(writer);
						iText.Layout.Document document = new iText.Layout.Document(pdf);

						Paragraph header = new Paragraph("Menu").SetTextAlignment(TextAlignment.CENTER).SetFontSize(24).SetBold();

						document.Add(header);
						document.Add(new Paragraph("\n"));

						List<BarMenuItem> Coffees = new List<BarMenuItem>();
						List<BarMenuItem> NonCoffees = new List<BarMenuItem>();
						List<BarMenuItem> Deserts = new List<BarMenuItem>();

						foreach (var item in items)
						{
							switch (item.Category)
							{
								case ItemType.Coffee:
									Coffees.Add(item);
									break;
								case ItemType.NonCoffee:
									NonCoffees.Add(item);
									break;
								case ItemType.Dessert:
									Deserts.Add(item);
									break;
							}
						}

						addNewTable(Coffees, document);
						addNewTable(NonCoffees, document);
						addNewTable(Deserts, document);

						document.Close();
					}

					MessageBox.Show("PDF generated successfully.");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error generating PDF: {ex.Message}");
				}
			}
		}

		public void addNewTable(List<BarMenuItem> menu, iText.Layout.Document document)
		{
			if (menu.Count != 0)
			{
				Paragraph title = new Paragraph(menu[0].Category.ToString()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(17).SetBold();

				document.Add(title);
				document.Add(new LineSeparator(new SolidLine()));

				Table table = new Table(UnitValue.CreatePercentArray(new float[] { 35, 65 }));
				foreach (var item in menu)
				{

					table.SetWidth(UnitValue.CreatePercentValue(100)); // Set the table to take the full width of the document

					if (File.Exists(item.ImagePath))
					{
						ImageData imageData = ImageDataFactory.Create(item.ImagePath);
						Image image = new Image(imageData).SetHeight(85);
						Cell imageCell = new Cell().Add(image).SetBorder(Border.NO_BORDER).SetPaddingTop(20);
						table.AddCell(imageCell);
					}
					else
					{
						// Add an empty cell if the image does not exist
						table.AddCell(new Cell().SetBorder(Border.NO_BORDER));
					}

					// Create the text content
					Paragraph textContent = new Paragraph().Add(new Text($"{item.Name} - {item.Price:C}" + "MKD.\n").SetFontSize(15)).Add(new Text(item.Description));

					Cell textCell = new Cell().Add(textContent).SetBorder(Border.NO_BORDER).SetPaddingTop(20);
					table.AddCell(textCell);
				}
				// Add the table to the document
				document.Add(table);
				document.Add(new Paragraph("\n"));
			}
		}
	}
}
