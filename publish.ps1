# Publishes to an .NET self-contained single executable.
# Avaliable RIDs: https://docs.microsoft.com/dotnet/core/rid-catalog#using-rids

$NAME = "chiral"
$PROJECT = "Chiral.Console"
$FRAMEWORK = "net8.0"

$RID_UNX = "linux-x64"
$RID_OSX = "osx-x64"
$RID_WIN = "win-x64"

Remove-Item -Recurse -Force "./release" -ErrorAction SilentlyContinue

New-Item -ItemType Directory -Path "./release"

New-Item -ItemType Directory -Path "./release/$RID_UNX"
New-Item -ItemType Directory -Path "./release/$RID_OSX"
New-Item -ItemType Directory -Path "./release/$RID_WIN"

dotnet publish "./src/$PROJECT" -r $RID_UNX -c Release --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
dotnet publish "./src/$PROJECT" -r $RID_OSX -c Release --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
dotnet publish "./src/$PROJECT" -r $RID_WIN -c Release --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true

Copy-Item "./src/$PROJECT/bin/Release/$FRAMEWORK/$RID_UNX/publish/$PROJECT"     "./release/$RID_UNX/$NAME"
Copy-Item "./src/$PROJECT/bin/Release/$FRAMEWORK/$RID_OSX/publish/$PROJECT"     "./release/$RID_OSX/$NAME"
Copy-Item "./src/$PROJECT/bin/Release/$FRAMEWORK/$RID_WIN/publish/$PROJECT.exe" "./release/$RID_WIN/$NAME.exe"

Copy-Item "./src/$PROJECT/bin/Release/$FRAMEWORK/$RID_UNX/publish/settings.json" "./release/$RID_UNX"
Copy-Item "./src/$PROJECT/bin/Release/$FRAMEWORK/$RID_OSX/publish/settings.json" "./release/$RID_OSX"
Copy-Item "./src/$PROJECT/bin/Release/$FRAMEWORK/$RID_WIN/publish/settings.json" "./release/$RID_WIN"
