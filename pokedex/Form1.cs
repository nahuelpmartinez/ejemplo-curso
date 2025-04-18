using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace pokedex
{
    public partial class Pokemons : Form
    {
        private List<Pokemon> listaPokemon;
        public Pokemons()
        {
            InitializeComponent();
        }

        private void Pokemons_Load(object sender, EventArgs e)
        {

            cargar();
            cboCampo.Items.Add("Número");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripción");

            //ejemplo con la utilización de la clase AccesoDatos y ElementoNegocio
            ElementoNegocio ejemplo = new ElementoNegocio();
            //dgvElementoNegocio.DataSource = ejemplo.listar();

        }

        private void cargar()
        {
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                listaPokemon = negocio.listar();
                dgvPokemons.DataSource = listaPokemon;
                ocultarColumnas();       
                cargarImagen(listaPokemon[0].urlImagen);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            //ocultar una columna 
            dgvPokemons.Columns["urlImagen"].Visible = false;
            dgvPokemons.Columns["Id"].Visible = false;
        }

        private void dgvPokemons_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            if(dgvPokemons.CurrentRow != null)
            {
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.urlImagen);
            }
            
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbPokemon.Load(imagen);
            }
            catch (Exception ex)
            {
                pbPokemon.Load("https://static.vecteezy.com/system/resources/thumbnails/008/695/917/small_2x/no-image-available-icon-simple-two-colors-template-for-no-image-or-picture-coming-soon-and-placeholder-illustration-isolated-on-white-background-vector.jpg");
            }
        }



        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();

            elementoNegocio.listar();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaPokemon altaPokemon = new frmAltaPokemon();
            altaPokemon.ShowDialog();//no te permite salir de la ventana sin antes terminarla de usar o cerrarla
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Pokemon seleccionado;
            seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

            frmAltaPokemon modificarPokemon = new frmAltaPokemon(seleccionado);
            modificarPokemon.ShowDialog();//no te permite salir de la ventana sin antes terminarla de usar o cerrarla
            cargar();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }

        private void eliminar(bool logico = false)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            Pokemon seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("¿De verdad querés eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

                    if(logico)
                        negocio.eliminarLogico(seleccionado.Id);
                    else
                        negocio.eliminar(seleccionado.Id);
                    cargar();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if(cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar");
                return true;
            }
            if(cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar");
                return true;
            }
            if(cboCampo.SelectedItem.ToString() == "Número")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar el filtro numérico");
                    return true;
                }
                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Solo nros para filtrar por un campo numérico");
                    return true;
                }
                
            }

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach(char caracter in cadena)
            {
                if(!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
            
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                if(validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvPokemons.DataSource = negocio.filtrar(campo, criterio, filtro);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtFiltro_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Pokemon> listaFiltrada;
            string filtro = txtFiltro.Text;

            if (filtro.Length >= 3)
            {
                //FindAll realiza una especie de for each
                listaFiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()));//devuelve una lista
                //El .ToUpper hace que se convierta todo en mayus y así poder buscar tanto por
                //minus como por mayus            
                //.Contains busca por coincidencia
            }
            else
            {
                listaFiltrada = listaPokemon;
            }

            dgvPokemons.DataSource = null; //hay que hacer una "limpieza" primero al datasource
            dgvPokemons.DataSource = listaFiltrada;
            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();//guarda el elemento seleccionado
            if (opcion == "Número")
            {
                cboCriterio.Items.Clear();

                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();

                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }

        private void txtFiltroAvanzado_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
