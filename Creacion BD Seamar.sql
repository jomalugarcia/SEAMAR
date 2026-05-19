USE master;
CREATE DATABASE SEAMAR;


USE SEAMAR;

    CREATE TABLE Participantes (
        IdParticipante  INT             NOT NULL IDENTITY(1,1),
        NombreCompleto  NVARCHAR(150)   NOT NULL,
        Email           NVARCHAR(200)   NOT NULL,
        Telefono        NVARCHAR(20)    NULL,
        FechaRegistro   DATE            NOT NULL DEFAULT (CAST(GETDATE() AS DATE)),

        CONSTRAINT PK_Participantes PRIMARY KEY (IdParticipante)
    );

    --EMAILS UNICOS
    CREATE UNIQUE INDEX NO_EMAILS_REPETIDOS
        ON Participantes (Email);


    CREATE TABLE Eventos (
        IdEvento        INT             NOT NULL IDENTITY(1,1),
        Nombre          NVARCHAR(100)   NOT NULL,
        Fecha           DATETIME2       NOT NULL,
        Lugar           NVARCHAR(200)   NOT NULL,
        Cupos INT             NOT NULL,
        -- Estado: 0 = Activo, 1 = Cancelado, 2 = Finalizado
        Estado          INT             NOT NULL DEFAULT 0,

        CONSTRAINT PK_Eventos PRIMARY KEY (IdEvento),
    );



    CREATE TABLE Inscripciones (
        IdInscripcion    INT      NOT NULL IDENTITY(1,1),
        EventoId         INT      NOT NULL,
        ParticipanteId   INT      NOT NULL,
        FechaInscripcion DATETIME2 NOT NULL DEFAULT GETDATE(),
        SeVino          BIT      NOT NULL DEFAULT 0,

        CONSTRAINT PK_Inscripciones PRIMARY KEY (IdInscripcion),

        -- FK  Eventos 
        CONSTRAINT FK_Inscripciones_Eventos
            FOREIGN KEY (EventoId)
            REFERENCES Eventos(IdEvento)
            ON DELETE CASCADE,

        -- FK  Participantes 
        CONSTRAINT FK_Inscripciones_Participantes
            FOREIGN KEY (ParticipanteId)
            REFERENCES Participantes(IdParticipante)
            ON DELETE CASCADE
    );

    -- Índice compuesto único: un participante no puede inscribirse
    -- dos veces al mismo evento
    CREATE UNIQUE INDEX IX_Inscripciones_EventoId_ParticipanteId
        ON Inscripciones (EventoId, ParticipanteId);




