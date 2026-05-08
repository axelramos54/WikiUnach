using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WUNACH
{
    /// <summary>
    /// Helpers compartidos para títulos de ventana y layout responsivo.
    /// </summary>
    public static class LayoutHelper
    {
        // ── ÍCONO DE LA APLICACIÓN ────────────────────────────────────────────────
        // Se busca en estas rutas EN ORDEN; usa la primera que exista.
        private static readonly string[] RUTAS_ICONO = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WUNACH.ico"),
            @"C:\Users\luisp\OneDrive\Pictures\WUNACH\WUNACH.ico"
        };

        private static Icon _iconoCacheado;

        /// <summary>Carga el ícono de la app (cacheado). Devuelve null si no existe.</summary>
        public static Icon ObtenerIcono()
        {
            if (_iconoCacheado != null) return _iconoCacheado;
            foreach (string ruta in RUTAS_ICONO)
            {
                try
                {
                    if (File.Exists(ruta))
                    {
                        _iconoCacheado = new Icon(ruta);
                        return _iconoCacheado;
                    }
                }
                catch { /* probar la siguiente ruta */ }
            }
            return null;
        }

        /// <summary>Asigna el ícono de la aplicación al formulario indicado.</summary>
        public static void AplicarIcono(Form f)
        {
            if (f == null) return;
            Icon ico = ObtenerIcono();
            if (ico != null) f.Icon = ico;
        }

        /// <summary>
        /// Aplica un título consistente a la ventana, incluyendo el rol del usuario.
        /// Ejemplo: "WUNACH — Administrador  •  Detalles de Tarea"
        /// </summary>
        public static void AplicarTitulo(Form f, string nombrePantalla)
        {
            if (f == null) return;

            string rol = SesionUsuario.Rol;
            string rolLegible;

            if (string.IsNullOrEmpty(rol)) rolLegible = "Sin sesión";
            else if (rol == "ADMINISTRADOR") rolLegible = "Administrador";
            else if (rol == "MAESTRO") rolLegible = "Maestro";
            else if (rol == "ALUMNO") rolLegible = "Alumno";
            else if (rol == "VISITANTE") rolLegible = "Visitante";
            else rolLegible = char.ToUpper(rol[0]) + rol.Substring(1).ToLower();

            f.Text = string.IsNullOrEmpty(nombrePantalla)
                     ? $"WUNACH  •  {rolLegible}"
                     : $"WUNACH  •  {rolLegible}  •  {nombrePantalla}";

            AplicarIcono(f);
        }

        /// <summary>
        /// Hace que al presionar Enter en cada TextBox, el foco salte al siguiente
        /// control en orden de TabIndex. Útil para formularios de captura.
        /// </summary>
        public static void EnterPasaAlSiguiente(params TextBox[] textboxes)
        {
            foreach (var tb in textboxes)
            {
                if (tb == null || tb.Multiline) continue;   // los multiline mantienen Enter normal
                var actual = tb;
                actual.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;          // evita el "ding"
                        Form parent = actual.FindForm();
                        parent?.SelectNextControl(actual, true, true, true, true);
                    }
                };
            }
        }
    }
}
