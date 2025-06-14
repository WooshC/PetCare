﻿using System;

namespace PetCare.Models
{
    public class UsuarioRol
    {
        public int UsuarioRolID { get; set; }
        public int UsuarioID { get; set; }
        public int RolID { get; set; }
        public DateTime FechaAsignacion { get; set; }

        // Propiedades de navegación
        public Usuario Usuario { get; set; }
        public Rol Rol { get; set; }
    }
}