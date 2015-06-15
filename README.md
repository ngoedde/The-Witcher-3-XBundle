# The-Witcher-3-XBundle
A library written in c# that allows developer to open/modify .bundle archives which are being used by the game "The Witcher 3 - Wild Hunt".


##Example
BundleArchive archive = new BundleArchive("mypath");

#You can access the files/directories via
archive.Files()
archive.Directories()
