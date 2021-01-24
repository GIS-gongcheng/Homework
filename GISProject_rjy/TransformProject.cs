using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSGeo.OGR;
using OSGeo.OSR;
using OSGeo.GDAL;

namespace GISProject_rjy
{
    class TransformProject
    {
        // 将给定shp转换为Web Mercator投影
        public void TransformShp(DataSource ds, string FilePath)
        {
            Layer layer = ds.GetLayerByIndex(0);
            SpatialReference sr = layer.GetSpatialRef();
            SpatialReference Mercator = new SpatialReference("");
            Mercator.ImportFromEPSG(3857); // Web Mercator
            Mercator.SetMercator(0d, 0d, 1d, 0d, 0d);

            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            oDriver.Register();
            //创造新矢量图层
            DataSource ds1 = oDriver.CreateDataSource(FilePath, null);
            Layer layer1 = ds1.CreateLayer(layer.GetName(), Mercator, layer.GetGeomType(), null);
            //读取属性字段名
            Feature feature = layer.GetFeature(0);
            for (int i = 0; i < feature.GetFieldCount(); i++)
            {
                FieldDefn fieldDefn = feature.GetFieldDefnRef(i);
                layer1.CreateField(fieldDefn, 1);
            }
            //遍历图层中每个要素
            feature = layer.GetNextFeature();
            while (feature != null)
            {
                Geometry geom = feature.GetGeometryRef();
                geom.TransformTo(Mercator);
                feature.SetGeometry(geom);
                layer1.CreateFeature(feature);
                feature = layer.GetNextFeature();
            }
			ds1.FlushCache();
			ds1.Dispose();
		}

		private static int ProgressFunc(double complete, IntPtr message, IntPtr data)
		{
			return 1;
		}

		// 将给定Tif转换为Web Mercator投影
		public void TransformTiff(Dataset ds, double[] VerticeX, double[] VerticeY, string FilePath, ResampleAlg eResampleMethod)
		{
			//获取Web墨卡托坐标系
			SpatialReference Mercator = new SpatialReference("");
			Mercator.ImportFromEPSG(3857); // Web Mercator
			Mercator.SetMercator(0d, 0d, 1d, 0d, 0d);
			string MercatorWkt;
			Mercator.ExportToWkt(out MercatorWkt);
			//原栅格坐标信息
			SpatialReference Raster_spf = new SpatialReference(ds.GetProjectionRef());
			//影像四个顶点投影转换
			CoordinateTransformation coordinateTrans = Osr.CreateCoordinateTransformation(Raster_spf, Mercator);
			coordinateTrans.TransformPoints(4, VerticeX, VerticeY, null);//VerticeX和VerticeY存储的是影像四个顶点坐标
			coordinateTrans.Dispose();
			//计算重投影后栅格顶点坐标
			double dbMinx = 0;
			double dbMaxx = 0;
			double dbMiny = 0;
			double dbMaxy = 0;
			dbMinx = Math.Min(Math.Min(Math.Min(VerticeX[0], VerticeX[1]), VerticeX[2]), VerticeX[3]);
			dbMaxx = Math.Max(Math.Max(Math.Max(VerticeX[0], VerticeX[1]), VerticeX[2]), VerticeX[3]);
			dbMiny = Math.Min(Math.Min(Math.Min(VerticeY[0], VerticeY[1]), VerticeY[2]), VerticeY[3]);
			dbMaxy = Math.Max(Math.Max(Math.Max(VerticeY[0], VerticeY[1]), VerticeY[2]), VerticeY[3]);
			//计算新的仿射变换参数
			double[] newTransform = new double[6];
			newTransform[0] = dbMinx;//左上角点x坐标
			newTransform[3] = dbMaxy; //左上角点y坐标
			newTransform[1] = 100;//像素宽度
			newTransform[5] = -100;//像素高度
								   //计算大小
			int width = (int)Math.Ceiling(Math.Abs(dbMaxx - dbMinx) / 100.0);
			int height = (int)Math.Ceiling(Math.Abs(dbMaxy - dbMiny) / 100.0);
			//创建新的栅格影像
			OSGeo.GDAL.Driver pGDalDriver = Gdal.GetDriverByName("GTiff");
			Dataset poDataset = pGDalDriver.Create(FilePath, width, height, 1, DataType.GDT_Float32, null);
			poDataset.SetGeoTransform(newTransform);
			poDataset.SetProjection(MercatorWkt);
			//重投影
			Gdal.ReprojectImage(ds, poDataset, ds.GetProjectionRef(), MercatorWkt, eResampleMethod, 0, 0, new Gdal.GDALProgressFuncDelegate(ProgressFunc), null, null);
			//设置NoData值
			Band band = poDataset.GetRasterBand(1);
			band.SetNoDataValue(-10000000);
			poDataset.FlushCache();
			poDataset.Dispose();
		}


	}
}
