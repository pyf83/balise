# Beacon Audio Remote Control - Projet Windows 10/11

Ce projet contient l'application Windows 10/11 complète (IHM modernisée + Intégration Bluetooth C#).

## 1. Génération du fichier Exécutable (.exe)

Pour générer le fichier `BeaconAudioRC.exe` :

### Méthode A : Ligne de commande (.NET 8 SDK)
Dans un terminal à la racine du projet :
```bash
dotnet publish BeaconAudioRC/BeaconAudioRC.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```
Le fichier **`BeaconAudioRC.exe`** sera généré directement dans :
`BeaconAudioRC/bin/Release/net8.0-windows10.0.19041.0/win-x64/publish/`

### Méthode B : Visual Studio 2022
1. Ouvrez `BeaconAudioRC.sln`.
2. Sélectionnez la configuration **Release** et la plateforme **x64**.
3. Faites un clic droit sur le projet `BeaconAudioRC` -> **Publier (Publish)**.
4. Choisissez le profil **Fichier unique (.exe autonome)**.

## 2. Génération de l'installateur (Setup_Beacon_Audio_RC.exe)
1. Téléchargez et installez **Inno Setup**.
2. Ouvrez le fichier `Installer/Setup_BeaconAudioRC.iss`.
3. Cliquez sur **Compile** dans Inno Setup pour générer l'installateur.