using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PetCare.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Vacío: no hacemos ningún cambio porque la BD ya tiene las tablas
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Vacío: reversa vacía
        }
    }
}
