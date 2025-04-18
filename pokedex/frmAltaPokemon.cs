using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;
using System.Configuration;


namespace pokedex
{
    public partial class frmAltaPokemon : Form
    {
        private Pokemon pokemon = null;

        private OpenFileDialog archivo = null;
        public frmAltaPokemon()
        {
            InitializeComponent();
        }

        public frmAltaPokemon(Pokemon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;
            Text = "Modificar Pokemon";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                if (pokemon == null)
                    pokemon = new Pokemon();
                pokemon.Numero = int.Parse(txtNumero.Text);
                pokemon.Nombre = txtNombre.Text;
                pokemon.Descripcion = txtDescripcion.Text;
                pokemon.urlImagen = txtImagen.Text;
                pokemon.Tipo = (Elemento)cboTipo.SelectedItem;
                pokemon.Debilidad = (Elemento)cboDebilidad.SelectedItem;

                if(pokemon.Id != 0)
                {
                    negocio.modificar(pokemon);
                    MessageBox.Show("Modificado exitosamente");
                }
                else
                {
                    negocio.agregar(pokemon);
                    MessageBox.Show("Agregado exitosamente");
                }

                //Guardo imagen si la levantó localmente
                if(archivo != null && (txtImagen.Text.ToUpper().Contains("HTTP")))
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
                }
                   
                Close();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void frmAltaPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();
            try
            {
                cboTipo.DataSource = elementoNegocio.listar();
                //Preecargo los desplegables
                cboTipo.ValueMember = "Id";
                cboTipo.DisplayMember = "Descripcion";
                cboDebilidad.DataSource = elementoNegocio.listar();
                //Preecargo los desplegables
                cboDebilidad.ValueMember = "Id";
                cboDebilidad.DisplayMember = "Descripcion";

                //Preecargo el pokemon en el modificar
                if(pokemon != null)
                {
                    //preeselecciono valores
                    txtNumero.Text = pokemon.Numero.ToString();
                    txtNombre.Text = pokemon.Nombre;
                    txtDescripcion.Text = pokemon.Descripcion;
                    txtImagen.Text = pokemon.urlImagen;
                    cargarImagen(pokemon.urlImagen);
                    cboTipo.SelectedValue = pokemon.Tipo.Id;
                    cboDebilidad.SelectedValue = pokemon.Debilidad.Id;

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtImagen.Text);
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxPokemon.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxPokemon.Load("https://static.vecteezy.com/system/resources/thumbnails/008/695/917/small_2x/no-image-available-icon-simple-two-colors-template-for-no-image-or-picture-coming-soon-and-placeholder-illustration-isolated-on-white-background-vector.jpg");
            }
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg|png|*.png";//filtrar que tipo de archivo
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                txtImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);

                //guardo la imagen
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
                
            }
        }
    }
}
