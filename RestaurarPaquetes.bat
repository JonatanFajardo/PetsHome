@echo off
echo Restaurando paquetes NuGet para PetsHome...

echo.
echo Restaurando paquetes en PetsHome.DataAccess...
cd PetsHome.DataAccess
dotnet restore
cd ..

echo.
echo Restaurando paquetes en PetsHome.Business...
cd PetsHome.Business
dotnet restore
cd ..

echo.
echo Restaurando paquetes en PetsHome.UI...
cd PetsHome.UI
dotnet restore
cd ..

echo.
echo Restaurando paquetes en la solución completa...
dotnet restore

echo.
echo Compilando la solución...
dotnet build

echo.
echo ¡Proceso completado!
pause