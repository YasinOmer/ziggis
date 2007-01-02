[connection]
; Your PostgreSQL server.
server = localhost

; Your PostgreSQL server's port.
port = 5432

; Your PostGIS database.
database = TUTORIAL

; Your PostGIS user.
user = psqluser

; Your PostGIS password.
password = psqluser

; Other stuff not used yet.
; ssl = true | false
; pooling = true | false
; minpoolsize = 
; maxpoolsize =
; timeout = <wait time for connection in seconds>

; The logging section is optional but recommended.
[logging]
; This is the complete path to the log4net config file.
; An example log4net config file is included.  See
; the log4net documentation for more help.
configfile = C:\ziggis\ZigGis\logging.config