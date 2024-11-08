using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WEBAPI_Rick_Morty.Controllers;
using WEBAPI_Rick_Morty.Models;

namespace WEBAPI_Rick_Morty
{
    public partial class Form1 : Form
    {
        private PersonajesController PersonajesController;
        private ListPersonajes ListPersonajes;
        public Form1()
        {
            InitializeComponent();
            PersonajesController = new PersonajesController();
            ListPersonajes = new ListPersonajes();
        }


        private async void GetListPersonajes()
        {
            ListPersonajes = await PersonajesController.GetAllPersonajes();

            if (ListPersonajes != null)
            {
                foreach (var Personajes in ListPersonajes?.results!)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvPersonajes);

                    row.Cells[0].Value = Personajes.id;


                    // Descargar la imagen desde la URL
                    try
                    {
                        if (!string.IsNullOrEmpty(Personajes.image))
                        {
                            using (var httpClient = new HttpClient())
                            {
                                // Descargar la imagen como flujo de bytes
                                var imageBytes = await httpClient.GetByteArrayAsync(Personajes.image);
                                using (var ms = new MemoryStream(imageBytes))
                                {
                                    // Convertir el flujo de bytes en un objeto Image
                                    Image img = Image.FromStream(ms);


                                    // Ajustar el tamaño de la imagen al tamaño deseado
                                    const int desiredHeight = 90;
                                    const int desiredWidth = 90;

                                    // Crear una nueva imagen con el tamaño deseado
                                    Image resizedImg = new Bitmap(img, new Size(desiredWidth, desiredHeight));

                                    row.Cells[1].Value = resizedImg;
                                }
                            }
                        }
                        else
                        {
                            row.Cells[1].Value = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                        row.Cells[1].Value = null; // Asignar un valor nulo en caso de error
                    }


                    row.Cells[2].Value = Personajes.name;
                    row.Cells[3].Value = Personajes.status;
                    row.Cells[4].Value = Personajes.location.name;
                    row.Cells[5].Value = Personajes.gender;
                    row.Cells[6].Value = Personajes.species;
                    row.Cells[7].Value = Personajes.origin.name;

                    dgvPersonajes.Rows.Add(row);

                    // Ajustar el alto de la fila para que se ajuste a la imagen
                    dgvPersonajes.Rows[dgvPersonajes.Rows.Count - 1].Height = 100;

                }

            }
            else
            {
                MessageBox.Show("No se pudo consultar los personajes", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            GetListPersonajes();
        }

    

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            // Verificar si se han cargado los personajes
            if (ListPersonajes == null || ListPersonajes.results == null || !ListPersonajes.results.Any())
            {
                MessageBox.Show("Debe consultar primero todos los personajes antes de buscar.", "Advertencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }




            string searchTerm = txtSearch.Text.ToLower();

            // Limpiar el DataGridView antes de mostrar los resultados filtrados
            dgvPersonajes.Rows.Clear();

            // Filtrar personajes que coincidan con el término de búsqueda
            var filteredPersonajes = ListPersonajes.results
                .Where(p => p.name.ToLower().Contains(searchTerm))
                .ToList();

            // Mostrar los personajes filtrados en el DataGridView
            foreach (var personajes in filteredPersonajes)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvPersonajes);

                row.Cells[0].Value = personajes.id;

                try
                {
                    if (!string.IsNullOrEmpty(personajes.image))
                    {
                        using (var httpClient = new HttpClient())
                        {
                            var imageBytes = httpClient.GetByteArrayAsync(personajes.image).Result;
                            using (var ms = new MemoryStream(imageBytes))
                            {
                                Image img = Image.FromStream(ms);

                                const int desiredHeight = 90;
                                const int desiredWidth = 90;
                                Image resizedImg = new Bitmap(img, new Size(desiredWidth, desiredHeight));

                                row.Cells[1].Value = resizedImg;
                            }
                        }
                    }
                    else
                    {
                        row.Cells[1].Value = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                    row.Cells[1].Value = null;
                }

                row.Cells[2].Value = personajes.name;
                row.Cells[3].Value = personajes.status;
                row.Cells[4].Value = personajes.location.name;
                row.Cells[5].Value = personajes.gender;
                row.Cells[6].Value = personajes.species;
                row.Cells[7].Value = personajes.origin.name;

                dgvPersonajes.Rows.Add(row);
                dgvPersonajes.Rows[dgvPersonajes.Rows.Count - 1].Height = 100;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dgvPersonajes.Rows.Clear();
        }
    }
}
