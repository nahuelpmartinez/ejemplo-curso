using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;

namespace negocio
{
    public class PokemonNegocio
    {
        public List<Pokemon> listar()
        {
            List<Pokemon> lista = new List<Pokemon>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=POKEDEX_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "Select P.Numero, P.Nombre, P.Descripcion, UrlImagen, E.Descripcion as Tipo, D.Descripcion as Debilidad, P.IdTipo, P.IdDebilidad, P.Id From POKEMONS P, ELEMENTOS E, ELEMENTOS D Where E.Id = P.IdTipo And D.Id = P.IdDebilidad And P.Activo = 1";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                { 
                    Pokemon aux = new Pokemon();
                    aux.Id = (int)lector["Id"];
                    aux.Numero = (int)lector["Numero"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];

                    //dos formas de validar que las columnas no sean null
                    //if (!(lector.IsDBNull(lector.GetOrdinal("urlImagen"))))
                        //aux.urlImagen = (string)lector["urlImagen"];
                    if (!(lector["urlImagen"] is DBNull))
                        aux.urlImagen = (string)lector["urlImagen"];

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)lector["Debilidad"];

                    lista.Add(aux);
                }

                conexion.Close();
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            
        }
    
        public void agregar(Pokemon nuevo) 
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Insert into POKEMONS (Numero, Nombre, Descripcion, Activo, IdTipo, IdDebilidad, UrlImagen) Values (" + nuevo.Numero + ", '" + nuevo.Nombre + "', '" + nuevo.Descripcion + "', 1, @idTipo, @IdDebilidad, @UrlImagen)");
                datos.setearParametro("@IdTipo", nuevo.Tipo.Id);
                datos.setearParametro("@IdDebilidad", nuevo.Debilidad.Id);
                datos.setearParametro("@UrlImagen", nuevo.urlImagen);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Pokemon poke)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Update POKEMONS set Numero = @Numero, Nombre = @Nombre, Descripcion = @Descripcion, UrlImagen = @Imagen, IdTipo = @IdTipo, IdDebilidad = @IdDebilidad Where Id = @Id");
                datos.setearParametro("@Numero", poke.Numero);
                datos.setearParametro("@Nombre", poke.Nombre);
                datos.setearParametro("@Descripcion", poke.Descripcion);
                datos.setearParametro("@Imagen", poke.urlImagen);
                datos.setearParametro("@IdTipo", poke.Tipo.Id);
                datos.setearParametro("@IdDebilidad", poke.Debilidad.Id);
                datos.setearParametro("@Id", poke.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from POKEMONS where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void eliminarLogico(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("Update POKEMONS set Activo = 0 Where Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Pokemon> filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "Select P.Numero, P.Nombre, P.Descripcion, UrlImagen, E.Descripcion as Tipo, D.Descripcion as Debilidad, P.IdTipo, P.IdDebilidad, P.Id From POKEMONS P, ELEMENTOS E, ELEMENTOS D Where E.Id = P.IdTipo And D.Id = P.IdDebilidad And P.Activo = 1 And ";
                if (campo == "Número")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Numero > " + filtro;
                            break;
                        case "Menor a":
                            consulta += "Numero < " + filtro;
                            break;
                        default:
                            consulta += "Numero = " + filtro;
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "Nombre like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "Nombre like '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "P.Descipcion like '" + filtro + "%'";
                            break;
                        case "Termina con":
                            consulta += "P.Descipcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "P.Descipcion like '%" + filtro + "%'";
                            break;
                    }
                }
                datos.setearConsulta(consulta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Numero = (int)datos.Lector["Numero"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    //dos formas de validar que las columnas no sean null
                    //if (!(lector.IsDBNull(lector.GetOrdinal("urlImagen"))))
                    //aux.urlImagen = (string)lector["urlImagen"];
                    if (!(datos.Lector["urlImagen"] is DBNull))
                        aux.urlImagen = (string)datos.Lector["urlImagen"];

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)datos.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)datos.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)datos.Lector["Debilidad"];

                    lista.Add(aux);
                }
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
