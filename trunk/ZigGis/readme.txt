ZigGis 1.1
developers: Abe Gillspie, Paolo Corti, Bill Dollins

PostGIS connector for ArcGIS
ZigGis is an ArcGis plug-in that allows loading and viewing of PostGIS layers. The hope is to eventually get ZigGis so that it will also edit PostGIS layers.

The official ZigGis web site is http://code.google.com/p/ziggis/. The old ZigGis web site is no longer supported.

We have imported the svn project (ZigGis 1.1 release) in google code, source code can be checked out at
this address:

http://ziggis.googlecode.com/svn/

***** Want to install and try ZigGis in a while? *****

People just interested in downloading and installing ZigGis can just download the installer, that can be found here:

http://ziggis.googlecode.com/svn/trunk/download/ZigGis11.zip

Quick installation steps:

ZigGis is developed with .NET 2.0 framework and ArcObject .NET assemblies, so you have to make sure you have both in the computer where you want to install ZigGis.
After having checked and maybe installed both, you can finally install ZigGis.

After intalling ZigGis open ArcMap and go to menu Tools>Customize. In the Customize dialog box browse to Commands tab, and from there select the ZigGis category. Select the 'Add PostGIS layer' and drag & drop it on any ArcMap's toolbar.
Close the dialog and click on the new botton you have in the toolbar.
You will need to provide a zig file, where there are the settings for your PostGis connections.

A sample ZigGis file, example.zig, is provided with the installation, under the ZigGis installation path (default is: C:\Program Files\ZigGis).
You can use this example.zig file or copy it and rename it as you think.

Now open and edit the zig file with your PostGis settings:

; Your PostgreSQL server.
server = localhost
; Your PostgreSQL server's port.
port = 5432
; Your PostGIS database.
database = TUTORIAL
; Your PostGIS user.
user = myuser
; Your PostGIS password.
password = mypassword

Finally from ArcMap, from the PostGis dialog box, browse to the zig file you just created/modified.
In the PostGis Table textbox type the PostGis layer you want to add in ArcMap.
Press ok, and if everything is fine the PostGis layer should be added in ArcMap.

Please consider that this is a very early first releaze of ZigGis, so there maybe be several issues.
Some issues (bad field data types for ArcMap, symbology, editing not let,...) are already being tracked, and we are working in the next weeks to solve them.

You can check the Issues here: http://code.google.com/p/ziggis/issues/list
Feel free to add any issue you get in if it is not in this list, or any request you wish to be implemented in future releases.

For support and help please don't email us or post the help request in the PostGis or Esri forums, but use the ZigGis group: http://groups.google.com/group/ziggis

***** RELEASE HISTORY *****

10/01/2006, zigGIS 1.1.0.30072
Added support for PostGIS layer with a srid different than the undefined one (srid=-1). Previous versions were not drawing these layer.

02/01/2006, zigGIS 1.1.0.29230
Added support for int8, numeric and date Postgre datatypes.
At this time Postgre tested datatypes for ZigGis are: 
serial, int8, int4, numeric, geometry, date
