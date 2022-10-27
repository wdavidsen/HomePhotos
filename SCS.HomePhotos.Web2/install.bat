sc create HomePhotosService binPath="%~dp0\SCS.HomePhotos.Web.exe" displayname="HomePhotos Service" 
sc failure HomePhotosService actions= restart/60000/restart/60000/""/60000 reset= 86400
sc start HomePhotosService 
sc config HomePhotosService start=auto