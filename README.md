# SEAMAR
Desarrollo de proyecto escolar para la gestion de eventos, con el que podras crear, organizar, editar y manejar los eventos y sus participantes, asi como sus cupos y fechas.

## Tecnologías utilizadas 

| Tecnología | Versión |
|---|---|
| .NET SDK | 9.0 |
| ASP.NET Core MVC | 9.0 |
| Entity Framework Core | 9.0 |
| Bootstrap (Corona Admin template descargado) | 4.0 |
| HTML |  |
| C# |  |
| CSS |  |
---

## Como poder ejecutarlo

### 1. Tecnologias utilizadas
- Visual Studio 2022
- .NET 9 SDK
- SQL Server Express 

### 2. Crea la base de datos

Ejecutar el script `Creacion BD Seamar.sql` que estara aqui en github o drive segun donde lo estes viendo:

1. Abrir el sql Express
2. Abre el archivo o copea el contenido en un nuevo scrip
3. Ejecutalo

Con esto tienes la BD de SEAMAR con las tablas `Eventos`, `Participantes` e `Inscripciones`, si no es asi algo esta mal.

### 3. Restaurar paquetes NuGet

Desde la Consola del Administrador de paquetes ejecuta lo siguiente:

powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 9.0.0
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 9.0.0

### 4. Ahora revisar la conexion de tu servidor

En el `appsettings.json` debes cambiar mi servidor al servidor de tu dispositivo osea:

json
"ConnectionStrings": {
  "cadenaSQL": "<Servidor>\\SQLEXPRESS;Database=SEAMAR;Integrated Security=True;TrustServerCertificate=True;"
}

Finaliza la parte de conexion

### 5. Ejecutalo el proyecto
Se solicita conexion al LocalHost

---

## Estructura del proyecto 

```
GestionarEventasos/
├── Controllers/
│   ├── HomeController.cs           # Pantalla principal + reporte 
│   ├── EventosController.cs        # CRUD completo de Eventos
│   ├── ParticipantesController.cs  # CRUD completo de Participantes
│   └── InscripcionesController.cs  # CRUD
├── Models/
│   ├── Evento.cs                   # Generado por Scaffold
│   ├── Participante.cs             # Generado por Scaffold
│   ├── Inscripcione.cs             # Generado por Scaffold
│   ├── SeamarContext.cs            # DbContext generado por Scaffold
│   └── ReporteEventoViewModel.cs   # ViewModel para vista de reporte
├── Views/
│   ├── Home/         Index.cshtml, Reporte.cshtml
│   ├── Eventos/      Index, Create, Edit, Details, Delete
│   ├── Participantes/ Index, Create, Edit, Details, Delete
│   ├── Inscripciones/ Index, Create, Edit, Delete
│   └── Shared/       Layout.cshtml (Corona Admin Template)
├── wwwroot/
│   ├── css/          style.css (Corona Admin)
│   ├── js/           Scripts de Corona Admin
│   └── vendors/      Bootstrap, MDI Icons
├── appsettings.json
└── Program.cs
```

---

## Se implemento las siguientes caracteristicas como funcionalidades principal de nuestra pagina:

* Creacion eventos

* Registro de participantes

* Inscripcion de participantes hasta llenar eventos

* Validación de evento lleno

* Ver reportes de evento/eventos en curso, finalizados o futuros

* Filtrar eventos por fechas


---
Desarrollado por:
Joel Jomalu García Trujillo
Nestor Ramiro Grijalva Valencia
