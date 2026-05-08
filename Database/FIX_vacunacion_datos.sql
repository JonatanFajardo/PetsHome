USE PETSHOMEDB
GO

INSERT INTO [Refugio].[tbCitaMedica]
    (masc_Id, com_Id, medic_FechaConsulta, medic_TipoConsulta, medic_MotivoConsulta, vac_Id,
     medic_EsEliminado, medic_UsuarioCrea, medic_FechaCrea)
VALUES
-- Thor (1) Perro: ok Rabia, ok Parvovirus, warn Moquillo
(1,1,DATEADD(MONTH,-6,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(1,1,DATEADD(MONTH,-6,GETDATE()),'Vacunacion','Control rutinario',1,0,1,GETDATE()),
(1,1,DATEADD(DAY,-345,GETDATE()),'Vacunacion','Control rutinario',2,0,1,GETDATE()),
-- Luna (2) Perro: ok Rabia, ok Polivalente, warn Bordetella
(2,1,DATEADD(MONTH,-4,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(2,1,DATEADD(MONTH,-4,GETDATE()),'Vacunacion','Control rutinario',4,0,1,GETDATE()),
(2,1,DATEADD(DAY,-340,GETDATE()),'Vacunacion','Control rutinario',7,0,1,GETDATE()),
-- Max (3) Perro: todo ok
(3,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',1,0,1,GETDATE()),
(3,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',2,0,1,GETDATE()),
(3,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(3,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',4,0,1,GETDATE()),
(3,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',7,0,1,GETDATE()),
(3,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',8,0,1,GETDATE()),
-- Mia (4) Gato: ok Rabia, warn Triple Felina
(4,1,DATEADD(MONTH,-5,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(4,1,DATEADD(DAY,-348,GETDATE()),'Vacunacion','Control rutinario',5,0,1,GETDATE()),
-- Rocky (5) Perro: todo ok
(5,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',1,0,1,GETDATE()),
(5,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',2,0,1,GETDATE()),
(5,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(5,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',4,0,1,GETDATE()),
(5,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',8,0,1,GETDATE()),
-- Coco (6) Perro: ok Parvovirus, warn Rabia
(6,1,DATEADD(MONTH,-6,GETDATE()),'Vacunacion','Control rutinario',1,0,1,GETDATE()),
(6,1,DATEADD(DAY,-342,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
-- Nala (7) Gato: ok Rabia, ok Triple Felina, warn Leucemia
(7,1,DATEADD(MONTH,-4,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(7,1,DATEADD(MONTH,-4,GETDATE()),'Vacunacion','Control rutinario',5,0,1,GETDATE()),
(7,1,DATEADD(DAY,-352,GETDATE()),'Vacunacion','Control rutinario',6,0,1,GETDATE()),
-- Bruno (8) Perro: ok Rabia, warn Moquillo
(8,1,DATEADD(MONTH,-5,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(8,1,DATEADD(DAY,-347,GETDATE()),'Vacunacion','Control rutinario',2,0,1,GETDATE()),
-- Kira (9) Perro: ok Rabia, ok Parvovirus, ok Leptospirosis
(9,1,DATEADD(MONTH,-1,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(9,1,DATEADD(MONTH,-1,GETDATE()),'Vacunacion','Control rutinario',1,0,1,GETDATE()),
(9,1,DATEADD(MONTH,-1,GETDATE()),'Vacunacion','Control rutinario',8,0,1,GETDATE()),
-- Simba (10) Gato: ok Triple Felina, ok Rabia
(10,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',5,0,1,GETDATE()),
(10,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
-- Bella (11) Perro: warn Polivalente
(11,1,DATEADD(DAY,-338,GETDATE()),'Vacunacion','Control rutinario',4,0,1,GETDATE()),
-- Oliver (12) Gato: todo ok
(12,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(12,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',5,0,1,GETDATE()),
(12,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',6,0,1,GETDATE()),
-- Duke (13) Perro: ok Rabia, warn Bordetella
(13,1,DATEADD(MONTH,-5,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(13,1,DATEADD(DAY,-355,GETDATE()),'Vacunacion','Control rutinario',7,0,1,GETDATE()),
-- Lily (14) Gato: warn Rabia
(14,1,DATEADD(DAY,-344,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
-- Zeus (15) Perro: todo ok
(15,1,DATEADD(MONTH,-4,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(15,1,DATEADD(MONTH,-4,GETDATE()),'Vacunacion','Control rutinario',2,0,1,GETDATE()),
(15,1,DATEADD(MONTH,-4,GETDATE()),'Vacunacion','Control rutinario',1,0,1,GETDATE()),
-- Canela (16) Perro: warn Leptospirosis
(16,1,DATEADD(DAY,-350,GETDATE()),'Vacunacion','Control rutinario',8,0,1,GETDATE()),
-- Toby (17) Perro: ok Rabia, ok Polivalente
(17,1,DATEADD(MONTH,-6,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(17,1,DATEADD(MONTH,-6,GETDATE()),'Vacunacion','Control rutinario',4,0,1,GETDATE()),
-- Michi (18) Gato: ok Triple Felina, warn Leucemia
(18,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',5,0,1,GETDATE()),
(18,1,DATEADD(DAY,-354,GETDATE()),'Vacunacion','Control rutinario',6,0,1,GETDATE()),
-- Jack (19) Perro: ok Rabia
(19,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
-- Lola (20) Perro: ok Parvovirus, warn Rabia
(20,1,DATEADD(MONTH,-3,GETDATE()),'Vacunacion','Control rutinario',1,0,1,GETDATE()),
(20,1,DATEADD(DAY,-341,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
-- Cleo (21) Gato: ok Rabia, ok Triple Felina
(21,1,DATEADD(MONTH,-5,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(21,1,DATEADD(MONTH,-5,GETDATE()),'Vacunacion','Control rutinario',5,0,1,GETDATE()),
-- Rex (22) Perro: ok Rabia, warn Parvovirus
(22,1,DATEADD(MONTH,-6,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(22,1,DATEADD(DAY,-338,GETDATE()),'Vacunacion','Control rutinario',1,0,1,GETDATE()),
-- Pelusa (23) Gato: todo ok
(23,1,DATEADD(MONTH,-1,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(23,1,DATEADD(MONTH,-1,GETDATE()),'Vacunacion','Control rutinario',5,0,1,GETDATE()),
(23,1,DATEADD(MONTH,-1,GETDATE()),'Vacunacion','Control rutinario',6,0,1,GETDATE()),
-- Firulais (24) Perro: ok Rabia, warn Moquillo, ok Leptospirosis
(24,1,DATEADD(MONTH,-4,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(24,1,DATEADD(DAY,-350,GETDATE()),'Vacunacion','Control rutinario',2,0,1,GETDATE()),
(24,1,DATEADD(MONTH,-4,GETDATE()),'Vacunacion','Control rutinario',8,0,1,GETDATE()),
-- Daisy (25) Perro: todo ok
(25,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',1,0,1,GETDATE()),
(25,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',2,0,1,GETDATE()),
(25,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',3,0,1,GETDATE()),
(25,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',4,0,1,GETDATE()),
(25,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',7,0,1,GETDATE()),
(25,1,DATEADD(MONTH,-2,GETDATE()),'Vacunacion','Control rutinario',8,0,1,GETDATE())

PRINT 'Datos de vacunacion insertados OK'
GO
