using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;


namespace ZigGis.Tester
{

    [Guid("E913CD4A-D925-4b8d-A55B-7BD15D47B142"), ClassInterface(ClassInterfaceType.None)]
    public class VerySimpleCustomRenderer :
        IFeatureRenderer,
		ILegendInfo

    {
		ILegendGroup lg;
		ISymbol sym;

		public VerySimpleCustomRenderer()
		{
			lg = new LegendGroupClass();
			ILegendClass lc = new LegendClassClass();
			sym = new SimpleFillSymbolClass();
			lc.Label = "A very simple custom renderer";
			lc.Symbol = sym;
			lg.AddClass(lc);
		}

        #region IFeatureRenderer

        bool IFeatureRenderer.CanRender(IFeatureClass featClass, IDisplay Display)
        {
			return true;
        }

        void IFeatureRenderer.Draw(IFeatureCursor cursor, esriDrawPhase DrawPhase, IDisplay Display, ITrackCancel trackCancel)
        {
            Display.SetSymbol(sym);
            IFeature f = cursor.NextFeature();
            while (f != null)
            {
                IFeatureDraw fd = f as IFeatureDraw;
                fd.Draw(DrawPhase, Display, sym, true, f.Shape, esriDrawStyle.esriDSNormal);
                f = cursor.NextFeature();
            }
        }

        IFeatureIDSet IFeatureRenderer.ExclusionSet
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        void IFeatureRenderer.PrepareFilter(IFeatureClass fc, IQueryFilter queryFilter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IFeatureRenderer.get_RenderPhase(esriDrawPhase DrawPhase)
        {
			if (DrawPhase != esriDrawPhase.esriDPAnnotation)
			{
				return true;
			}
			else
			{
				return false;
			}
        }

        ESRI.ArcGIS.Display.ISymbol IFeatureRenderer.get_SymbolByFeature(IFeature Feature)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion


		#region ILegendInfo Members

		int ILegendInfo.LegendGroupCount
		{
			get { return 1; }
		}

		ILegendItem ILegendInfo.LegendItem
		{
			get { return null; }
		}

		bool ILegendInfo.SymbolsAreGraduated
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		ILegendGroup ILegendInfo.get_LegendGroup(int Index)
		{
			return lg;
		}

		#endregion
	}

	[Guid("ED096EE1-8448-4ac3-8FAD-B6A5D5FDA8BE"), ClassInterface(ClassInterfaceType.None)]
	public class CustomUniqueValueRenderer :
		IFeatureRenderer,
		ILegendInfo,
		ILevelRenderer,
		IPersist,
		IPersistStream,
		IRendererClasses,
		IUniqueValueRenderer
	{

		public CustomUniqueValueRenderer()
		{
		}

		private IUniqueValueRenderer m_renderer = new UniqueValueRendererClass();
		protected IUniqueValueRenderer uvr { get { return m_renderer; } }

		#region IFeatureRenderer

		bool IFeatureRenderer.CanRender(IFeatureClass featClass, IDisplay Display)
		{
			return ((IFeatureRenderer)uvr).CanRender(featClass, Display);
		}

		void IFeatureRenderer.Draw(IFeatureCursor cursor, esriDrawPhase DrawPhase, IDisplay Display, ITrackCancel trackCancel)
		{
			((IFeatureRenderer)uvr).Draw(cursor, DrawPhase, Display, trackCancel);
		}

		IFeatureIDSet IFeatureRenderer.ExclusionSet
		{
			set 
			{ 
				((IFeatureRenderer)uvr).ExclusionSet = value; 
			}
		}

		void IFeatureRenderer.PrepareFilter(IFeatureClass fc, IQueryFilter queryFilter)
		{

			((IFeatureRenderer)uvr).PrepareFilter(fc, queryFilter);
		}

		bool IFeatureRenderer.get_RenderPhase(esriDrawPhase DrawPhase)
		{
			return ((IFeatureRenderer)uvr).get_RenderPhase(DrawPhase);
		}

		ISymbol IFeatureRenderer.get_SymbolByFeature(IFeature Feature)
		{
			return ((IFeatureRenderer)uvr).get_SymbolByFeature(Feature);
		}

		#endregion


		#region ILegendInfo

		int ILegendInfo.LegendGroupCount
		{
			get { return ((ILegendInfo)uvr).LegendGroupCount; }
		}

		ILegendItem ILegendInfo.LegendItem
		{
			get { return ((ILegendInfo)uvr).LegendItem; }
		}

		bool ILegendInfo.SymbolsAreGraduated
		{
			get
			{
				return ((ILegendInfo)uvr).SymbolsAreGraduated;
			}
			set
			{
				((ILegendInfo)uvr).SymbolsAreGraduated = value;
			}
		}

		ILegendGroup ILegendInfo.get_LegendGroup(int Index)
		{
			return ((ILegendInfo)uvr).get_LegendGroup(Index);
		}

		#endregion

		#region IUniqueValueRenderer

		void IUniqueValueRenderer.AddReferenceValue(string Value, string refValue)
		{
			((IUniqueValueRenderer)uvr).AddReferenceValue(Value, refValue);
		}

		void IUniqueValueRenderer.AddValue(string Value, string Heading, ISymbol Symbol)
		{
			((IUniqueValueRenderer)uvr).AddValue(Value, Heading, Symbol);
		}

		string IUniqueValueRenderer.ColorScheme
		{
			get
			{
				return ((IUniqueValueRenderer)uvr).ColorScheme;
			}
			set
			{
				((IUniqueValueRenderer)uvr).ColorScheme = value;
			}
		}

		string IUniqueValueRenderer.DefaultLabel
		{
			get
			{
				return ((IUniqueValueRenderer)uvr).DefaultLabel;
			}
			set
			{
				((IUniqueValueRenderer)uvr).DefaultLabel = value;
			}
		}

		ISymbol IUniqueValueRenderer.DefaultSymbol
		{
			get
			{
				return ((IUniqueValueRenderer)uvr).DefaultSymbol;
			}
			set
			{
				((IUniqueValueRenderer)uvr).DefaultSymbol = value;
			}
		}

		int IUniqueValueRenderer.FieldCount
		{
			get
			{
				return ((IUniqueValueRenderer)uvr).FieldCount;
			}
			set
			{
				((IUniqueValueRenderer)uvr).FieldCount = value;
			}
		}

		string IUniqueValueRenderer.FieldDelimiter
		{
			get
			{
				return ((IUniqueValueRenderer)uvr).FieldDelimiter;
			}
			set
			{
				((IUniqueValueRenderer)uvr).FieldDelimiter = value;
			}
		}

		string IUniqueValueRenderer.LookupStyleset
		{
			get
			{
				return ((IUniqueValueRenderer)uvr).LookupStyleset;
			}
			set
			{
				((IUniqueValueRenderer)uvr).LookupStyleset = value;
			}
		}

		void IUniqueValueRenderer.RemoveAllValues()
		{
			((IUniqueValueRenderer)uvr).RemoveAllValues();
		}

		void IUniqueValueRenderer.RemoveValue(string Value)
		{
			((IUniqueValueRenderer)uvr).RemoveValue(Value);
		}

		bool IUniqueValueRenderer.UseDefaultSymbol
		{
			get
			{
				return ((IUniqueValueRenderer)uvr).UseDefaultSymbol;
			}
			set
			{
				((IUniqueValueRenderer)uvr).UseDefaultSymbol = value;
			}
		}

		int IUniqueValueRenderer.ValueCount
		{
			get { return ((IUniqueValueRenderer)uvr).ValueCount; }
		}

		string IUniqueValueRenderer.get_Description(string Value)
		{
			return ((IUniqueValueRenderer)uvr).get_Description(Value);
		}

		string IUniqueValueRenderer.get_Field(int Index)
		{
			return ((IUniqueValueRenderer)uvr).get_Field(Index);
		}

		string IUniqueValueRenderer.get_Heading(string Value)
		{
			return ((IUniqueValueRenderer)uvr).get_Heading(Value);
		}

		string IUniqueValueRenderer.get_Label(string Value)
		{
			return ((IUniqueValueRenderer)uvr).get_Label(Value);
		}

		string IUniqueValueRenderer.get_ReferenceValue(string Value)
		{
			return ((IUniqueValueRenderer)uvr).get_ReferenceValue(Value);
		}

		ISymbol IUniqueValueRenderer.get_Symbol(string Value)
		{
			return ((IUniqueValueRenderer)uvr).get_Symbol(Value);
		}

		string IUniqueValueRenderer.get_Value(int Index)
		{
			return ((IUniqueValueRenderer)uvr).get_Value(Index);
		}

		void IUniqueValueRenderer.set_Description(string Value, string Text)
		{
			((IUniqueValueRenderer)uvr).set_Description(Value, Text);
		}

		void IUniqueValueRenderer.set_Field(int Index, string Field)
		{
			((IUniqueValueRenderer)uvr).set_Field(Index, Field);
		}

		void IUniqueValueRenderer.set_FieldType(int Index, bool __p2)
		{
			((IUniqueValueRenderer)uvr).set_FieldType(Index, __p2);
		}

		void IUniqueValueRenderer.set_Heading(string Value, string Heading)
		{
			((IUniqueValueRenderer)uvr).set_Heading(Value, Heading);
		}

		void IUniqueValueRenderer.set_Label(string Value, string Label)
		{
			((IUniqueValueRenderer)uvr).set_Label(Value, Label);
		}

		void IUniqueValueRenderer.set_Symbol(string Value, ISymbol Symbol)
		{
			((IUniqueValueRenderer)uvr).set_Symbol(Value, Symbol);
		}

		void IUniqueValueRenderer.set_Value(int Index, string Value)
		{
			((IUniqueValueRenderer)uvr).set_Value(Index, Value);
		}

		#endregion

		#region ILevelRenderer

		int ILevelRenderer.CurrentDrawLevel
		{
			set { ((ILevelRenderer)uvr).CurrentDrawLevel = value; }
		}

		object ILevelRenderer.LevelArray
		{
			get { return ((ILevelRenderer)uvr).LevelArray; }
		}

		#endregion

		#region IRendererClasses

		int IRendererClasses.ClassCount
		{
			get { return ((IRendererClasses)uvr).ClassCount; }
		}

		string IRendererClasses.get_Class(int Index)
		{
			return ((IRendererClasses)uvr).get_Class(Index);
		}

		string IRendererClasses.get_WhereClause(int Index, ITable Table)
		{
			return ((IRendererClasses)uvr).get_WhereClause(Index, Table);
		}

		#endregion

		#region IPersist

		void IPersist.GetClassID(out Guid pClassID)
		{
			((IPersist)uvr).GetClassID(out pClassID);
		}

		#endregion

		#region IPersistStream

		void IPersistStream.GetClassID(out Guid pClassID)
		{
			((IPersistStream)uvr).GetClassID(out pClassID);
		}

		void IPersistStream.GetSizeMax(out _ULARGE_INTEGER pcbSize)
		{
			((IPersistStream)uvr).GetSizeMax(out pcbSize);
		}

		void IPersistStream.IsDirty()
		{
			((IPersistStream)uvr).IsDirty();
		}

		void IPersistStream.Load(IStream pstm)
		{
			((IPersistStream)uvr).Load(pstm);
		}

		void IPersistStream.Save(IStream pstm, int fClearDirty)
		{
			((IPersistStream)uvr).Save(pstm, fClearDirty);
		}

		#endregion
	}
}
