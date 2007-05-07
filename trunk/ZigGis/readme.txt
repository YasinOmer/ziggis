developers: Abe Gillspie, Paolo Corti, Bill Dollins

[http://code.google.com/p/ziggis http://portfolio.fringeblog.com/ill/zg-logo.jpg]

PostGIS connector for ArcGIS
zigGIS is an ArcGis plug-in that allows loading and viewing of PostGIS layers. The hope is to eventually get zigGIS so that it will also edit PostGIS layers.

The official zigGIS web site is http://code.google.com/p/ziggis/. The old ZigGis web site is no longer supported.

We have imported the svn project in google code, source code can be checked out at
this address:

http://ziggis.googlecode.com/svn/

The ZigGIS logo - designed by Jeremiah Lewis, a very good friend of Abe - represents the aim of zigGIS, an Open Source GIS package that span Open Source and commercial GIS database packages. Launch zig for great justice!

==If you want to install and try zigGIS in a while==

People just interested in downloading and installing zigGIS can just download the installer, that can be found here:

http://ziggis.googlecode.com/svn/trunk/download/ZigGis11.zip

Quick installation steps:

zigGIS should correctly work with ArcGIS Desktop 9.0 (you must have esri service pach 3!), 9.1 and 9.2.
zigGIS is developed with .NET 2.0 framework and ArcObject .NET assemblies, so you have to make sure you have both in the computer where you want to install zigGIS.
After having checked and maybe installed both, you can finally install zigGIS.

After intalling zigGIS open ArcMap and go to menu Tools>Customize. In the Customize dialog box browse to Commands tab, and from there select the zigGIS category. Select the 'Add PostGIS layer' and drag & drop it on any ArcMap's toolbar.
Close the dialog and click on the new botton you have in the toolbar.
You will need to provide a zig file, where there are the settings for your PostGis connections.

A sample zigGIS file, example.zig, is provided with the installation, under the zigGIS installation path (default is: C:\Program Files\zigGIS).
You can use this example.zig file or copy it and rename it as you think.

Now open and edit the zig file with your PostGis settings:

{{{
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
}}}

Finally from ArcMap, from the PostGis dialog box, browse to the zig file you just created/modified.
In the layer list check the PostGis layer(s) you want to add in ArcMap.
Press ok, and if everything is fine the PostGis layer should be added in ArcMap.

Please consider that this is a very early first releaze of zigGIS, so there maybe be several issues.
Some issues (symbology not working, selection not working, editing not let,...) are already being tracked, and we are working in the next weeks to solve them.

Prerequisite (at 1.2.0.0) for viewing PostGIS layers in ArcMap with zigGIS:
 * your PostGIS table MUST have a field named gid (integer data type) that will uniquely identify each feature (ref: http://code.google.com/p/ziggis/issues/detail?id=13&can=2&q=);
 * your srid should be a valid one in the Esri factorycode (ref: http://code.google.com/p/ziggis/issues/detail?id=12&can=2&q=);

You can check the Issues here: http://code.google.com/p/ziggis/issues/list
Feel free to add any issue you get in if it is not in this list, or any request you wish to be implemented in future releases.

For support and help please don't email us or post the help request in the PostGis or Esri forums, but use the zigGIS group: http://groups.google.com/group/ziggis

==RELEASE HISTORY==

===xx/yy/2007, zigGIS 1.2.1.0===
 
 * Added support for PostGIS layer with a srid not supported from Esri (factory code not exists for such a srid, in that case the PostGIS layer is still added to ArcMap but it won't be able to project it in a different projection system)
 * Added a warning if a gid field is not provided in the PostGIS layer (in this case layer is not added)

===21/03/2007, zigGIS 1.2.0.0===
 
 * Added support for selection
 * All of the symbology renderers are now working
 * Document's persistence was repaired (in 1.1.1.0 and 1.1.0.20768 was broken)
 * Work for Postgis ArcCatalog object has started (thanks Bill!). You will now find in ArcCatalog a pretty "OGC Database Connection" node. From there it will be possible to create the OGC connection, for now only PostGIS ("Add new PostGis connection"), but maybe in the future also for other OGC db (Oracle Spatial, MySQL Spatial...). The tool is still not working, at this release, but gives an idea of what zigGis will become in the next releases.

===21/02/2007, zigGIS 1.1.1.0===
 
 * Added support for using zigGis for ArcObjects customisation (ArcMap VBA, Visual Basic 6...). Previous versions of PostGisWorkspaceFactory was not correctly exposed to COM.

===14/02/2007, zigGIS 1.1.0.20768===

 * Added support for reprojecting zigGIS data
 * Implemented PostGisEnumDatasetName for iterating PostGis layer tables
 * Implemented PostGisWorkspaceFactory.Open from opening a PostGIS workspace factory not necessarily from a zig File but from a PropertySet (in the SDEWorkspaceFactory fashion)
 * The Add PostGIS data form has been made it better: there is not need now to type the PostGIS table name, but after selecting a zig file it will be possible to add on or more layers, shown in a checkboxlist

===10/01/2007, zigGIS 1.1.0===
 
 * Added support for PostGIS layer with a srid different than the undefined one (srid=-1). 

Previous versions were not drawing these layer.

===02/01/2007, zigGIS 1.1.0===
 
 * Added support for int8, numeric and date Postgre datatypes. At this time Postgre tested datatypes for ZigGis are: serial, int8, int4, numeric, geometry, date

 

