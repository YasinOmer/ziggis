using System.Runtime.InteropServices;

namespace ZigGis.PostGis.Catalog
{
    //[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    //Guid("2E9B4CA5-4F53-4068-AEB0-72D04EE8F742"), ComVisible(true)]
    [Guid("2E9B4CA5-4F53-4068-AEB0-72D04EE8F742")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IGxPostGisDatabase
    {
        string Host { get;set;}
        int Port { get;set;}
        string Database { get;set;}
        string UserName { get;set;}
        string Password { get;set;}
        void setZigFile(string zigFilePath);
    }
}