using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DIMS_Backend.Models;

public partial class UcbPortalContext : DbContext
{
    public UcbPortalContext()
    {
    }

    public UcbPortalContext(DbContextOptions<UcbPortalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CampoLaboral> CampoLaborals { get; set; }

    public virtual DbSet<Carrera> Carreras { get; set; }

    public virtual DbSet<Evento> Eventos { get; set; }

    public virtual DbSet<EventoSuscripcione> EventoSuscripciones { get; set; }

    public virtual DbSet<Materia> Materias { get; set; }

    public virtual DbSet<Noticia> Noticias { get; set; }

    public virtual DbSet<Paralelo> Paralelos { get; set; }

    public virtual DbSet<PerfilEgresado> PerfilEgresados { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Publicacione> Publicaciones { get; set; }

    public virtual DbSet<User> Users { get; set; }

    // Connection string is provided via DI in Program.cs (from environment variables).

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<CampoLaboral>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("campo_laboral_pkey");

            entity.ToTable("campo_laboral");

            entity.HasIndex(e => e.CarreraId, "idx_campo_carrera");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarreraId).HasColumnName("carrera_id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Orden)
                .HasDefaultValue(0)
                .HasColumnName("orden");

            entity.HasOne(d => d.Carrera).WithMany(p => p.CampoLaborals)
                .HasForeignKey(d => d.CarreraId)
                .HasConstraintName("campo_laboral_carrera_id_fkey");
        });

        modelBuilder.Entity<Carrera>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carreras_pkey");

            entity.ToTable("carreras", tb => tb.HasComment("Carreras universitarias ofrecidas"));

            entity.HasIndex(e => e.Slug, "carreras_slug_key").IsUnique();

            entity.HasIndex(e => e.Activa, "idx_carreras_activa");

            entity.HasIndex(e => e.Slug, "idx_carreras_slug");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activa)
                .HasDefaultValue(true)
                .HasColumnName("activa");
            entity.Property(e => e.Color)
                .HasMaxLength(7)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Duracion)
                .HasMaxLength(50)
                .HasColumnName("duracion");
            entity.Property(e => e.Icono)
                .HasMaxLength(10)
                .HasColumnName("icono");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Modalidad)
                .HasMaxLength(50)
                .HasColumnName("modalidad");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Slug)
                .HasMaxLength(100)
                .HasColumnName("slug");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Evento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("eventos_pkey");

            entity.ToTable("eventos", tb => tb.HasComment("Eventos académicos y extracurriculares"));

            entity.HasIndex(e => e.CarreraId, "idx_eventos_carrera");

            entity.HasIndex(e => e.FechaEvento, "idx_eventos_fecha");

            entity.HasIndex(e => e.Publicado, "idx_eventos_publicado");

            entity.HasIndex(e => e.Tipo, "idx_eventos_tipo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Capacidad).HasColumnName("capacidad");
            entity.Property(e => e.CarreraId).HasColumnName("carrera_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.FechaEvento)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_evento");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Inscritos)
                .HasDefaultValue(0)
                .HasColumnName("inscritos");
            entity.Property(e => e.Lugar)
                .HasMaxLength(255)
                .HasColumnName("lugar");
            entity.Property(e => e.Publicado)
                .HasDefaultValue(false)
                .HasColumnName("publicado");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(500)
                .HasColumnName("titulo");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Carrera).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.CarreraId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("eventos_carrera_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Eventos)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("eventos_created_by_fkey");
        });

        modelBuilder.Entity<EventoSuscripcione>(entity =>
        {
            entity.HasKey(e => new { e.EventoId, e.UserId }).HasName("evento_suscripciones_pkey");

            entity.ToTable("evento_suscripciones");

            entity.Property(e => e.EventoId).HasColumnName("evento_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FechaSuscripcion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_suscripcion");

            entity.HasOne(d => d.Evento).WithMany(p => p.EventoSuscripciones)
                .HasForeignKey(d => d.EventoId)
                .HasConstraintName("evento_suscripciones_evento_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.EventoSuscripciones)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("evento_suscripciones_user_id_fkey");
        });

        modelBuilder.Entity<Materia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("materias_pkey");

            entity.ToTable("materias", tb => tb.HasComment("Materias del plan de estudios"));

            entity.HasIndex(e => e.CarreraId, "idx_materias_carrera");

            entity.HasIndex(e => e.Semestre, "idx_materias_semestre");

            entity.HasIndex(e => e.Sigla, "idx_materias_sigla");

            entity.HasIndex(e => e.Sigla, "materias_sigla_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activa)
                .HasDefaultValue(true)
                .HasColumnName("activa");
            entity.Property(e => e.Area)
                .HasMaxLength(100)
                .HasColumnName("area");
            entity.Property(e => e.CarreraId).HasColumnName("carrera_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Creditos).HasColumnName("creditos");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.Semestre).HasColumnName("semestre");
            entity.Property(e => e.Sigla)
                .HasMaxLength(20)
                .HasColumnName("sigla");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Carrera).WithMany(p => p.Materia)
                .HasForeignKey(d => d.CarreraId)
                .HasConstraintName("materias_carrera_id_fkey");

            entity.HasMany(d => d.Docentes).WithMany(p => p.Materia)
                .UsingEntity<Dictionary<string, object>>(
                    "MateriaDocente",
                    r => r.HasOne<Persona>().WithMany()
                        .HasForeignKey("DocenteId")
                        .HasConstraintName("materia_docentes_docente_id_fkey"),
                    l => l.HasOne<Materia>().WithMany()
                        .HasForeignKey("MateriaId")
                        .HasConstraintName("materia_docentes_materia_id_fkey"),
                    j =>
                    {
                        j.HasKey("MateriaId", "DocenteId").HasName("materia_docentes_pkey");
                        j.ToTable("materia_docentes");
                        j.IndexerProperty<int>("MateriaId").HasColumnName("materia_id");
                        j.IndexerProperty<int>("DocenteId").HasColumnName("docente_id");
                    });

            entity.HasMany(d => d.MateriaNavigation).WithMany(p => p.Requisitos)
                .UsingEntity<Dictionary<string, object>>(
                    "MateriaRequisito",
                    r => r.HasOne<Materia>().WithMany()
                        .HasForeignKey("MateriaId")
                        .HasConstraintName("materia_requisitos_materia_id_fkey"),
                    l => l.HasOne<Materia>().WithMany()
                        .HasForeignKey("RequisitoId")
                        .HasConstraintName("materia_requisitos_requisito_id_fkey"),
                    j =>
                    {
                        j.HasKey("MateriaId", "RequisitoId").HasName("materia_requisitos_pkey");
                        j.ToTable("materia_requisitos");
                        j.IndexerProperty<int>("MateriaId").HasColumnName("materia_id");
                        j.IndexerProperty<int>("RequisitoId").HasColumnName("requisito_id");
                    });

            entity.HasMany(d => d.Requisitos).WithMany(p => p.MateriaNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "MateriaRequisito",
                    r => r.HasOne<Materia>().WithMany()
                        .HasForeignKey("RequisitoId")
                        .HasConstraintName("materia_requisitos_requisito_id_fkey"),
                    l => l.HasOne<Materia>().WithMany()
                        .HasForeignKey("MateriaId")
                        .HasConstraintName("materia_requisitos_materia_id_fkey"),
                    j =>
                    {
                        j.HasKey("MateriaId", "RequisitoId").HasName("materia_requisitos_pkey");
                        j.ToTable("materia_requisitos");
                        j.IndexerProperty<int>("MateriaId").HasColumnName("materia_id");
                        j.IndexerProperty<int>("RequisitoId").HasColumnName("requisito_id");
                    });
        });

        modelBuilder.Entity<Noticia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("noticias_pkey");

            entity.ToTable("noticias", tb => tb.HasComment("Noticias y artículos del portal"));

            entity.HasIndex(e => e.CarreraId, "idx_noticias_carrera");

            entity.HasIndex(e => e.Destacada, "idx_noticias_destacada");

            entity.HasIndex(e => e.Fecha, "idx_noticias_fecha").IsDescending();

            entity.HasIndex(e => e.Publicada, "idx_noticias_publicada");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarreraId).HasColumnName("carrera_id");
            entity.Property(e => e.Contenido).HasColumnName("contenido");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Destacada)
                .HasDefaultValue(false)
                .HasColumnName("destacada");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Publicada)
                .HasDefaultValue(false)
                .HasColumnName("publicada");
            entity.Property(e => e.Titulo)
                .HasMaxLength(500)
                .HasColumnName("titulo");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Carrera).WithMany(p => p.Noticia)
                .HasForeignKey(d => d.CarreraId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("noticias_carrera_id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Noticia)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("noticias_created_by_fkey");
        });

        modelBuilder.Entity<Paralelo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("paralelos_pkey");

            entity.ToTable("paralelos", tb => tb.HasComment("Paralelos (secciones) de cada materia"));

            entity.HasIndex(e => e.DocenteId, "idx_paralelos_docente");

            entity.HasIndex(e => e.MateriaId, "idx_paralelos_materia");

            entity.HasIndex(e => new { e.MateriaId, e.Codigo }, "paralelos_materia_id_codigo_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Aula)
                .HasMaxLength(100)
                .HasColumnName("aula");
            entity.Property(e => e.Codigo)
                .HasMaxLength(5)
                .HasColumnName("codigo");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Cupo).HasColumnName("cupo");
            entity.Property(e => e.Disponible)
                .HasComputedColumnSql("(inscritos < cupo)", true)
                .HasColumnName("disponible");
            entity.Property(e => e.DocenteId).HasColumnName("docente_id");
            entity.Property(e => e.Horario)
                .HasMaxLength(255)
                .HasColumnName("horario");
            entity.Property(e => e.Inscritos)
                .HasDefaultValue(0)
                .HasColumnName("inscritos");
            entity.Property(e => e.MateriaId).HasColumnName("materia_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Docente).WithMany(p => p.Paralelos)
                .HasForeignKey(d => d.DocenteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("paralelos_docente_id_fkey");

            entity.HasOne(d => d.Materia).WithMany(p => p.Paralelos)
                .HasForeignKey(d => d.MateriaId)
                .HasConstraintName("paralelos_materia_id_fkey");
        });

        modelBuilder.Entity<PerfilEgresado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("perfil_egresado_pkey");

            entity.ToTable("perfil_egresado");

            entity.HasIndex(e => e.CarreraId, "idx_perfil_carrera");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarreraId).HasColumnName("carrera_id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Orden)
                .HasDefaultValue(0)
                .HasColumnName("orden");

            entity.HasOne(d => d.Carrera).WithMany(p => p.PerfilEgresados)
                .HasForeignKey(d => d.CarreraId)
                .HasConstraintName("perfil_egresado_carrera_id_fkey");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("personas_pkey");

            entity.ToTable("personas", tb => tb.HasComment("Docentes, directores, coordinadores e investigadores"));

            entity.HasIndex(e => e.Activo, "idx_personas_activo");

            entity.HasIndex(e => e.CarreraId, "idx_personas_carrera");

            entity.HasIndex(e => e.Email, "idx_personas_email");

            entity.HasIndex(e => e.Rol, "idx_personas_rol");

            entity.HasIndex(e => e.Email, "personas_email_key").IsUnique();

            entity.HasIndex(e => e.UserId, "personas_user_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CarreraId).HasColumnName("carrera_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Especialidad).HasColumnName("especialidad");
            entity.Property(e => e.FotoUrl)
                .HasMaxLength(500)
                .HasColumnName("foto_url");
            entity.Property(e => e.GoogleScholarUrl)
                .HasMaxLength(500)
                .HasColumnName("google_scholar_url");
            entity.Property(e => e.GradoAcademico)
                .HasMaxLength(255)
                .HasColumnName("grado_academico");
            entity.Property(e => e.LinkedinUrl)
                .HasMaxLength(500)
                .HasColumnName("linkedin_url");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.PublicacionesCount)
                .HasDefaultValue(0)
                .HasColumnName("publicaciones_count");
            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .HasColumnName("rol");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Carrera).WithMany(p => p.Personas)
                .HasForeignKey(d => d.CarreraId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("personas_carrera_id_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.Persona)
                .HasForeignKey<Persona>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("personas_user_id_fkey");
        });

        modelBuilder.Entity<Publicacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("publicaciones_pkey");

            entity.ToTable("publicaciones", tb => tb.HasComment("Publicaciones académicas (tesis, proyectos, artículos)"));

            entity.HasIndex(e => e.CarreraId, "idx_publicaciones_carrera");

            entity.HasIndex(e => e.FechaPublicacion, "idx_publicaciones_fecha").IsDescending();

            entity.HasIndex(e => e.PersonaId, "idx_publicaciones_persona");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArchivoUrl)
                .HasMaxLength(500)
                .HasColumnName("archivo_url");
            entity.Property(e => e.Autor)
                .HasMaxLength(255)
                .HasColumnName("autor");
            entity.Property(e => e.CarreraId).HasColumnName("carrera_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FechaPublicacion).HasColumnName("fecha_publicacion");
            entity.Property(e => e.PersonaId).HasColumnName("persona_id");
            entity.Property(e => e.Resumen).HasColumnName("resumen");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .HasColumnName("tipo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(500)
                .HasColumnName("titulo");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Carrera).WithMany(p => p.Publicaciones)
                .HasForeignKey(d => d.CarreraId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("publicaciones_carrera_id_fkey");

            entity.HasOne(d => d.Persona).WithMany(p => p.Publicaciones)
                .HasForeignKey(d => d.PersonaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("publicaciones_persona_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users", tb => tb.HasComment("Usuarios del sistema con autenticación básica"));

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.Rol, "idx_users_rol");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .HasColumnName("nombre");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Rol)
                .HasMaxLength(50)
                .HasColumnName("rol");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
