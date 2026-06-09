# Instalación con SQL Server Management Studio

1. Abra SSMS y anote el valor exacto de **Server name**.
2. Modifique `CineStreamCR/appsettings.json`.
3. Abra `CineStreamCR.sln` en Visual Studio.
4. Establezca `CineStreamCR` como proyecto de inicio.
5. Ejecute con `Ctrl + F5`.
6. En SSMS, clic derecho en Databases y seleccione Refresh.
7. Ejecute `database/Verify_CineStreamCR.sql`.

Para crear todo manualmente, ejecute `database/CineStreamCR_SQLServer_FullSetup.sql` en una consulta nueva.
