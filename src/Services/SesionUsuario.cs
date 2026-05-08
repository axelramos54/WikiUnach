namespace WUNACH
{
    /// <summary>
    /// Almacena los datos del usuario autenticado durante la sesión activa.
    /// Se llena en FrmAcceso al hacer login exitoso y se usa en todos los demás formularios.
    /// </summary>
    internal static class SesionUsuario
    {
        public static string ID_Usuario      { get; set; }
        public static string Nombre          { get; set; }
        public static string Correo          { get; set; }
        public static string Rol             { get; set; }
        public static string Matricula       { get; set; }
        public static string ID_Facultad     { get; set; }
        /// <summary>Solo se llena para rol ALUMNO.</summary>
        public static string ID_Licenciatura { get; set; }
        /// <summary>Solo se llena para rol ALUMNO.</summary>
        public static string Semestre        { get; set; }

        /// <summary>
        /// Solo aplica a ADMINISTRADOR: si está activo, al hacer clic en una
        /// tarea desde FrmPrincipal en lugar de abrirla, la bloquea.
        /// </summary>
        public static bool ModoBloqueoAdmin  { get; set; } = false;

        /// <summary>
        /// Limpia todos los datos al cerrar sesión.
        /// </summary>
        public static void CerrarSesion()
        {
            ID_Usuario      = null;
            Nombre          = null;
            Correo          = null;
            Rol             = null;
            Matricula       = null;
            ID_Facultad     = null;
            ID_Licenciatura = null;
            Semestre        = null;
            ModoBloqueoAdmin = false;
        }
    }
}
