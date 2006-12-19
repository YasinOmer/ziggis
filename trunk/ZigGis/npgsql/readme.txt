This is a patched version of Npgsql.dll and enables
it to work with PostGIS data.  See the main readme.txt
for installation instructions.

Namely this fixes handling of the Geometry data type.
For reference, the following changes were made:

o NpgsqlDbType.cs, Ln 58 added:
, Geometry

o NpgsqlTypesHelper, Ln 393 added:
new NpgsqlBackendTypeInfo(0,
	"geometry",
	NpgsqlDbType.Bytea,
	DbType.Binary,
	typeof(Byte[]),
	new ConvertBackendToNativeHandler(
	BasicBackendToNativeTypeConverter.ToBinary))
