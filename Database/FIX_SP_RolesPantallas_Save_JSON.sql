-- ============================================================
-- FIX: PR_Seguridad_RolesPantallas_Save
-- Migra el SP a la firma JSON con permisos CRUD (consultar/insertar/editar/eliminar)
-- ============================================================
USE PETSHOMEDB
GO

CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RolesPantallas_Save]
    @rol_Id INT,
    @permisosJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION

        -- Desactivar todas las pantallas del rol
        UPDATE [Seguridad].[tbRolesPantallas]
        SET ropan_EsActivo = 0,
            ropan_Consultar = 0,
            ropan_Insertar = 0,
            ropan_Editar = 0,
            ropan_Eliminar = 0
        WHERE rol_Id = @rol_Id

        IF @permisosJson IS NOT NULL AND @permisosJson <> '' AND @permisosJson <> '[]'
        BEGIN
            DECLARE @Permisos TABLE (
                pan_Id INT,
                consultar BIT,
                insertar BIT,
                editar BIT,
                eliminar BIT
            )

            INSERT INTO @Permisos (pan_Id, consultar, insertar, editar, eliminar)
            SELECT pan_Id, ropan_Consultar, ropan_Insertar, ropan_Editar, ropan_Eliminar
            FROM OPENJSON(@permisosJson)
            WITH (
                pan_Id          INT  '$.pan_Id',
                ropan_Consultar BIT  '$.ropan_Consultar',
                ropan_Insertar  BIT  '$.ropan_Insertar',
                ropan_Editar    BIT  '$.ropan_Editar',
                ropan_Eliminar  BIT  '$.ropan_Eliminar'
            )

            MERGE [Seguridad].[tbRolesPantallas] AS target
            USING @Permisos AS source
            ON target.rol_Id = @rol_Id AND target.pan_Id = source.pan_Id
            WHEN MATCHED THEN
                UPDATE SET
                    ropan_EsActivo = 1,
                    ropan_Consultar = source.consultar,
                    ropan_Insertar = source.insertar,
                    ropan_Editar = source.editar,
                    ropan_Eliminar = source.eliminar
            WHEN NOT MATCHED THEN
                INSERT (rol_Id, pan_Id, ropan_EsActivo, ropan_Consultar, ropan_Insertar, ropan_Editar, ropan_Eliminar)
                VALUES (@rol_Id, source.pan_Id, 1, source.consultar, source.insertar, source.editar, source.eliminar);
        END

        COMMIT TRANSACTION
        SELECT 1 AS Resultado
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        DECLARE @msg NVARCHAR(2048) = ERROR_MESSAGE()
        RAISERROR(@msg, 16, 1)
    END CATCH
END
GO

PRINT 'PR_Seguridad_RolesPantallas_Save actualizado a firma JSON con CRUD.'
GO
