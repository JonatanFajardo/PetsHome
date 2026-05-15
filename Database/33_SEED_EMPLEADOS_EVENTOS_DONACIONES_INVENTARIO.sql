-- =============================================================================
-- Seed Data: Empleados, Usuarios, Donaciones, Eventos, Items, Existencias
-- Ejecutar en PETSHOMEDB
-- =============================================================================
SET NOCOUNT ON;

-- ============================================================
-- 1. EMPLEADOS — un empleado por rol en cada refugio
--    Las Lomas: Admin(extra) + Supervisor + Vet + Cuidador + Basico
--    La Esperanza y Vida Animal: Supervisor + Vet + Cuidador + Basico
-- ============================================================

-- 1a. Arreglar usuario "director" (rol 2 eliminado) → Supervisor en Vida Animal
UPDATE Seguridad.tbUsuarios SET Rol_Id = 3 WHERE usu_Id = 3;

-- 1b. Nuevas personas para los empleados que faltan
INSERT INTO General.tbPersonas (per_Identidad, per_PrimerNombre, per_SegundoNombre, per_ApellidoPaterno, per_ApellidoMaterno,
    per_FechaNacimiento, per_Domicilio, per_Telefono, per_Correo, per_EsEliminado, per_UsuarioCrea, per_FechaCrea)
VALUES
-- per 23: supervisor Las Lomas
('0801199500123','Lucia','Beatriz','Ordoñez','Murillo','1995-03-12','Col. La Travesia, SPS','99112233','lordoñez@petshome.hn',0,1,GETDATE()),
-- per 24: basico La Esperanza
('0801199700456','Diego','Andres','Paredes','Velasquez','1997-07-18','Col. Kennedy Bloque F, SPS','88334455','dparedes@petshome.hn',0,1,GETDATE()),
-- per 25: basico Vida Animal
('0801199600789','Gabriela','Sofia','Reyes','Castillo','1996-11-05','Barrio La Isla, SPS','77556677','greyes@petshome.hn',0,1,GETDATE());

-- 1c. Nuevos empleados
INSERT INTO Refugio.tbEmpleados (emp_Codigo, per_Id, refg_Id, cag_Id, emp_EsActivo)
VALUES
('EMP-L-SUP', 23, 1, 1, 1),  -- emp 11: Supervisor Las Lomas
('EMP-E-REC', 24, 2, 3, 1),  -- emp 12: Basico   La Esperanza
('EMP-V-REC', 25, 3, 3, 1);  -- emp 13: Basico   Vida Animal

-- 1d. Crear / asignar usuarios (hash reutilizado del seed existente)
DECLARE @Hash NVARCHAR(500) = '7B55C0475B136C7C682FECC5DD082F6B3AD4CA65E71BF36881943DA61C2C57A3';
DECLARE @Hoy  DATETIME      = GETDATE();

INSERT INTO Seguridad.tbUsuarios
    (Emp_Id, Usu_Nombre, Rol_Id, Usu_PasswordHash, Usu_PasswordSalt,
     Usu_EsActivo, Usu_Suspendido, Usu_EsEliminado, Usu_FechaCreacion)
VALUES
-- Roberto Zuniga   (emp 6, La Esperanza) → Veterinario
(6,  'roberto.esp',      4, @Hash, NULL, 1, 0, 0, @Hoy),
-- Sandra Avila     (emp 7, La Esperanza) → Cuidador
(7,  'sandra.esp',       5, @Hash, NULL, 1, 0, 0, @Hoy),
-- Lucia Ordoñez    (emp 11, Las Lomas)  → Supervisor
(11, 'lucia.lomas',      3, @Hash, NULL, 1, 0, 0, @Hoy),
-- Diego Paredes    (emp 12, La Esperanza)→ Basico
(12, 'diego.esp',        6, @Hash, NULL, 1, 0, 0, @Hoy),
-- Carmen Padilla   (emp 9, Vida Animal) → Veterinario
(9,  'carmen.vida',      4, @Hash, NULL, 1, 0, 0, @Hoy),
-- Laura Bonilla    (emp 10, Vida Animal)→ Cuidador
(10, 'laura.vida',       5, @Hash, NULL, 1, 0, 0, @Hoy),
-- Gabriela Reyes   (emp 13, Vida Animal)→ Basico
(13, 'gabriela.vida',    6, @Hash, NULL, 1, 0, 0, @Hoy);

-- ============================================================
-- 2. DONACIONES — repartidas en los últimos 5 meses
-- ============================================================
INSERT INTO Refugio.tbDonaciones
    (dona_TipoDonacion, dona_NombreDonante, dona_TelefonoDonante, dona_EmailDonante,
     dona_MontoMonetario, dona_DescripcionArticulos, dona_ValorEstimado,
     dona_FechaDonacion, dona_Estado, dona_Observaciones, refg_Id,
     dona_EsEliminado, dona_UsuarioCrea, dona_FechaCrea)
VALUES
-- Refugio Las Lomas (refg 1)
('Monetaria','Banco Atlántida','','bancatlantida@fundacion.hn',
  5000.00, NULL, 0, '2026-01-15','Recibida','Donación corporativa anual',1, 0,1,'2026-01-15'),
('Monetaria','Rotary Club SPS','99001122','rotarysps@org.hn',
  3500.00, NULL, 0, '2026-02-20','Recibida','Colecta mensual',1, 0,1,'2026-02-20'),
('Monetaria','María Theresa López','88223344','mlopez@gmail.com',
  800.00,  NULL, 0, '2026-03-05','Recibida','Donación personal',1, 0,1,'2026-03-05'),
('Monetaria','Supermercados La Colonia','','gerencia@lacolonia.hn',
  2200.00, NULL, 0, '2026-04-10','Recibida','Campaña solidaria',1, 0,1,'2026-04-10'),
('Monetaria','Ana Gabriela Moncada','99554477','agmoncada@hotmail.com',
  600.00,  NULL, 0, '2026-05-02','Recibida',NULL,1, 0,1,'2026-05-02'),
('Articulos', 'Ferretería El Constructor','88990011','ferrcons@gmail.com',
  0, 'Jaulas metálicas x3, mangueras de lavado x2', 1800.00, '2026-02-14','Recibida','Donación de materiales',1, 0,1,'2026-02-14'),
('Articulos','Juan Carlos Espinoza','99667788','jcespinoza@live.com',
  0, 'Concentrado Adulto 20kg x5 sacos', 2500.00, '2026-03-22','Recibida','Alimentos para animales',1, 0,1,'2026-03-22'),

-- Refugio La Esperanza (refg 2)
('Monetaria','Cámara de Comercio e Industria','','info@ccisps.hn',
  4000.00, NULL, 0, '2026-01-28','Recibida','Aporte trimestral',2, 0,1,'2026-01-28'),
('Monetaria','Rosa Marina Ávila','88112233','rmavila@yahoo.com',
  500.00,  NULL, 0, '2026-02-10','Recibida',NULL,2, 0,1,'2026-02-10'),
('Monetaria','Colegio de Médicos Veterinarios','','colmedvet@colegios.hn',
  1500.00, NULL, 0, '2026-03-18','Recibida','Donación gremial',2, 0,1,'2026-03-18'),
('Monetaria','Empresa Inversiones del Norte','','edn@inversiones.hn',
  3000.00, NULL, 0, '2026-04-25','Recibida','RSE empresa privada',2, 0,1,'2026-04-25'),
('Articulos','Almacenes Siman','','rsocial@siman.com',
  0, 'Camas para mascotas x10, juguetes surtidos x20', 3200.00,'2026-03-08','Recibida','Campaña de responsabilidad social',2, 0,1,'2026-03-08'),

-- Refugio Vida Animal (refg 3)
('Monetaria','Pedro Alonso Turcios','99334456','paturcios@gmail.com',
  1200.00, NULL, 0, '2026-02-05','Recibida',NULL,3, 0,1,'2026-02-05'),
('Monetaria','Cooperativa Siguatepeque','','admin@coosigu.hn',
  2800.00, NULL, 0, '2026-03-12','Recibida','Donación cooperativa',3, 0,1,'2026-03-12'),
('Monetaria','Karla Vanessa Pineda','88776655','kvpineda@live.com',
  450.00,  NULL, 0, '2026-04-08','Recibida',NULL,3, 0,1,'2026-04-08'),
('Monetaria','Grupo Financiero Banpaís','','fundacion@banpais.hn',
  3800.00, NULL, 0, '2026-05-06','Recibida','Aporte semestral',3, 0,1,'2026-05-06'),
('Articulos','Droguería Santa Lucía','88990022','dslucia@drogueria.hn',
  0,'Medicamentos veterinarios surtidos, vitaminas x50 frascos',4500.00,'2026-04-15','Recibida','Donación farmacéutica',3, 0,1,'2026-04-15'),
('Articulos','Club de Voluntarios Universitarios','','voluntarios@unah.hn',
  0,'Comederos inox x8, bebederos x8, arneses medianos x5',2100.00,'2026-05-10','Recibida','Proyecto social universitario',3, 0,1,'2026-05-10');

-- ============================================================
-- 3. EVENTOS — 4 adicionales por refugio
-- ============================================================
INSERT INTO Refugio.tbEventos
    (eve_Descripcion, refg_Id, eve_HoraInicio, eve_HoraFinal, eve_Fecha,
     eve_EsEliminado, eve_UsuarioCrea, eve_FechaCrea)
VALUES
-- Las Lomas
('Feria de Adopción "Adopta con Amor"',         1,'08:00','13:00','2026-04-05', 0,1,GETDATE()),
('Jornada de Vacunación Gratuita',               1,'07:30','12:00','2026-04-19', 0,1,GETDATE()),
('Campaña de Esterilización Comunitaria',        1,'08:00','14:00','2026-05-03', 0,1,GETDATE()),
('Taller: Tenencia Responsable de Mascotas',     1,'09:00','11:00','2026-05-17', 0,1,GETDATE()),

-- La Esperanza
('Feria de Adopción "Nuevo Hogar, Nueva Vida"',  2,'09:00','14:00','2026-04-12', 0,1,GETDATE()),
('Jornada de Desparasitación Externa',           2,'08:00','12:00','2026-04-26', 0,1,GETDATE()),
('Día del Voluntariado Animal',                  2,'07:00','16:00','2026-05-10', 0,1,GETDATE()),
('Exposición Fotográfica "Mascotas en Espera"',  2,'10:00','18:00','2026-05-24', 0,1,GETDATE()),

-- Vida Animal
('Feria de Adopción "Segunda Oportunidad"',      3,'08:30','13:00','2026-04-06', 0,1,GETDATE()),
('Jornada de Salud Animal Integral',             3,'07:00','13:00','2026-04-20', 0,1,GETDATE()),
('Campaña "SPS Esteriliza"',                     3,'08:00','15:00','2026-05-04', 0,1,GETDATE()),
('Taller de Adiestramiento Básico',              3,'09:00','12:00','2026-05-18', 0,1,GETDATE());

-- ============================================================
-- 4. ITEMS DE INVENTARIO — artículos adicionales necesarios
-- ============================================================
INSERT INTO Inventario.tbItems
    (itm_Codigo, itm_Descripcion, cat_Id, itm_Precio, itm_StockMinimo,
     itm_EsEliminado, itm_UsuarioCrea, itm_FechaCrea)
VALUES
-- Alimento Perros (cat 1)
('ALI-P-003','Snacks de Entrenamiento x500g',   1,  85.00, 10, 0,1,GETDATE()),
('ALI-P-004','Leche en Polvo para Cachorros 400g',1,120.00, 8, 0,1,GETDATE()),
-- Alimento Gatos (cat 2)
('ALI-G-003','Snacks Liofilizados para Gato 60g',2,  75.00, 8, 0,1,GETDATE()),
('ALI-G-004','Leche Especial para Gatitos 200ml',2,  65.00, 6, 0,1,GETDATE()),
-- Medicamentos (cat 3)
('MED-003','Vitaminas B-Complex Oral 100ml',    3, 180.00, 5, 0,1,GETDATE()),
('MED-004','Suero Rehidratante Oral 1L',        3,  95.00, 8, 0,1,GETDATE()),
('MED-005','Desparasitante Externo Spray 400ml',3, 145.00, 6, 0,1,GETDATE()),
-- Insumos Médicos (cat 8)
('INS-001','Jeringas Desechables 5ml x50',      8,  55.00,10, 0,1,GETDATE()),
('INS-002','Vendas Elásticas 5cm x6 rollos',    8,  70.00, 8, 0,1,GETDATE()),
('INS-003','Termómetro Digital Veterinario',    8, 350.00, 3, 0,1,GETDATE());

-- ============================================================
-- 5. EXISTENCIAS — stock realista para todos los items
--    Items 1-20 existentes + items 21-30 nuevos, 3 refugios
-- ============================================================

-- Primero, insertar existencias faltantes para items ya existentes
-- (ya existen algunos registros, insertar los que faltan)
INSERT INTO Inventario.tbExistencias (itm_Id, refg_Id, exi_Cantidad, exi_UltimaActualizacion)
SELECT v.itm_Id, v.refg_Id, v.cant, GETDATE()
FROM (VALUES
  -- itm 1: Concentrado Premium Adulto 15kg  (ya existen refg 1,2,3)
  -- itm 2: Concentrado Cachorro 8kg
  (2,2, 3),(2,3, 4),
  -- itm 3: Alimento Humedo Latas x12
  (3,1,12),(3,2, 8),
  -- itm 4: Concentrado Gato Adulto 8kg
  (4,1, 6),(4,2, 4),(4,3, 5),
  -- itm 5: Concentrado Gato Castrado 4kg
  (5,1, 3),(5,2, 2),(5,3, 4),
  -- itm 6: Antiparasitario Interno 50ml
  (6,1, 8),(6,2, 6),(6,3, 5),
  -- itm 7: Antibiotico Amoxicilina 100ml
  (7,1, 4),(7,2, 3),(7,3, 2),
  -- itm 8: Antipulgas Topico x3 pipetas
  (8,1,10),(8,2, 8),(8,3, 6),
  -- itm 9: Pelota de Goma Mediana
  (9,1,15),(9,2,12),(9,3,10),
  -- itm 10: Hueso Masticable Grande
  (10,1,20),(10,2,15),(10,3,12),
  -- itm 11: Raton de Peluche para Gato  (ya existe stock=0, actualizar via update abajo)
  (11,1, 3),(11,2, 2),(11,3, 4),
  -- itm 12: Collar Ajustable Mediano
  (12,1, 8),(12,2, 6),(12,3, 5),
  -- itm 13: Correa Retractil 5m
  (13,1, 5),(13,2, 4),(13,3, 3),
  -- itm 14: Placa de Identificacion  (ya existe stock=0)
  (14,1, 2),(14,2, 1),(14,3, 0),
  -- itm 15: Champu Antipulgas 500ml
  (15,1, 6),(15,2, 5),(15,3, 4),
  -- itm 16: Desinfectante de Jaulas 1L
  (16,1, 4),(16,2, 3),(16,3, 3),
  -- itm 17: Bolsas Sanitarias x100  (ya existe stock=0)
  (17,1, 2),(17,2, 1),(17,3, 0),
  -- itm 18: Cama para Perro Grande  (ya existe stock=0)
  (18,1, 3),(18,2, 2),(18,3, 0),
  -- itm 19: Jaula de Transporte Mediana  (ya existe stock=0)
  (19,1, 1),(19,2, 0),(19,3, 0),
  -- itm 20: Guantes Quirurgicos Caja x100
  (20,1, 4),(20,2, 3),(20,3, 2)
) v(itm_Id, refg_Id, cant)
WHERE NOT EXISTS (
    SELECT 1 FROM Inventario.tbExistencias e
    WHERE e.itm_Id = v.itm_Id AND e.refg_Id = v.refg_Id
);

-- Existencias para los 10 items nuevos (ids 21-30)
INSERT INTO Inventario.tbExistencias (itm_Id, refg_Id, exi_Cantidad, exi_UltimaActualizacion)
VALUES
(21,1,15,GETDATE()),(21,2,12,GETDATE()),(21,3,10,GETDATE()),
(22,1, 8,GETDATE()),(22,2, 6,GETDATE()),(22,3, 5,GETDATE()),
(23,1, 4,GETDATE()),(23,2, 3,GETDATE()),(23,3, 2,GETDATE()),
(24,1, 6,GETDATE()),(24,2, 5,GETDATE()),(24,3, 4,GETDATE()),
(25,1, 3,GETDATE()),(25,2, 2,GETDATE()),(25,3, 4,GETDATE()),
(26,1,10,GETDATE()),(26,2, 8,GETDATE()),(26,3, 6,GETDATE()),
(27,1, 5,GETDATE()),(27,2, 4,GETDATE()),(27,3, 3,GETDATE()),
(28,1,20,GETDATE()),(28,2,18,GETDATE()),(28,3,15,GETDATE()),
(29,1, 6,GETDATE()),(29,2, 5,GETDATE()),(29,3, 4,GETDATE()),
(30,1, 2,GETDATE()),(30,2, 2,GETDATE()),(30,3, 1,GETDATE());

-- ============================================================
-- 6. VERIFICACION FINAL
-- ============================================================
SELECT 'Empleados/Usuarios' AS Seccion,
    r.refg_Nombre,
    COUNT(DISTINCT e.emp_Id) AS Empleados,
    COUNT(DISTINCT u.usu_Id) AS Usuarios
FROM Refugio.tbRefugios r
JOIN Refugio.tbEmpleados e ON e.refg_Id = r.refg_Id
LEFT JOIN Seguridad.tbUsuarios u ON u.Emp_Id = e.emp_Id
WHERE r.refg_EsEliminado=0
GROUP BY r.refg_Nombre;

SELECT 'Donaciones' AS Seccion, r.refg_Nombre, COUNT(*) AS Total, SUM(dona_MontoMonetario) AS MontoTotal
FROM Refugio.tbDonaciones d JOIN Refugio.tbRefugios r ON r.refg_Id=d.refg_Id
WHERE d.dona_EsEliminado=0 GROUP BY r.refg_Nombre;

SELECT 'Eventos' AS Seccion, r.refg_Nombre, COUNT(*) AS Total
FROM Refugio.tbEventos v JOIN Refugio.tbRefugios r ON r.refg_Id=v.refg_Id
WHERE v.eve_EsEliminado=0 GROUP BY r.refg_Nombre;

SELECT 'Items' AS Seccion, COUNT(*) AS TotalItems FROM Inventario.tbItems WHERE itm_EsEliminado=0;
SELECT 'Existencias' AS Seccion, COUNT(*) AS TotalRegistros, SUM(exi_Cantidad) AS StockTotal FROM Inventario.tbExistencias;
