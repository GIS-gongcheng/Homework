using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OSGeo.OGR;
using OSGeo.OSR;
using OSGeo.GDAL;

namespace GISProject_rjy
{
    class Statistic
    {
        //矢量转栅格
        private void RasterizeLayer(Layer layer, string outRaster, string field, float resolution, int xSize, int ySize)
        {
            const double noDataValue = -9999;   // NoData值 
            string outputRasterFile = outRaster;
            Envelope envelope = new Envelope(); //原图层外接矩形
            layer.GetExtent(envelope, 0);
            //计算栅格化后行列数             
            //新建栅格图层 
            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("GTiff");
            Dataset outputDataset = outputDriver.Create(outputRasterFile, xSize, ySize, 1, DataType.GDT_Int32, null);//DataType.GDT_Float64
            //获取原矢量图层坐标系 
            string inputShapeSrs;
            OSGeo.OSR.SpatialReference spatialRefrence = layer.GetSpatialRef();
            spatialRefrence.ExportToWkt(out inputShapeSrs);
            outputDataset.SetProjection(inputShapeSrs);

            double[] argin = new double[] { envelope.MinX, resolution, 0, envelope.MaxY, 0, -resolution };
            outputDataset.SetGeoTransform(argin);

            Band band = outputDataset.GetRasterBand(1);
            band.SetNoDataValue(noDataValue);

            outputDataset.FlushCache();
            outputDataset.Dispose();
            //矢量转栅格
            int[] bandlist = new int[] { 1 };
            double[] burnValues = new double[] { 10.0 };
            Dataset myDataset = Gdal.Open(outputRasterFile, Access.GA_Update);
            string[] rasterizeOptions;
            //rasterizeOptions = new string[] { "ALL_TOUCHED=TRUE", "ATTRIBUTE=" + field };
            rasterizeOptions = new string[] { "ATTRIBUTE=" + field, "ALL_TOUCHED=TRUE" };
            //Gdal.RasterizeLayer(myDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, null, null, null);
            int tets = Gdal.RasterizeLayer(myDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, rasterizeOptions, new Gdal.GDALProgressFuncDelegate(ProgressFunc), "Raster conversion");
            myDataset.FlushCache();
            myDataset.Dispose();
        }

        private static int ProgressFunc(double complete, IntPtr message, IntPtr data)
        {
            return 1;
        }

        //统计计算
        public List<float[]> ComputeStatistic(Layer layer, Dataset ds)
        {
            //计算栅格文件分辨率
            double[] adfGeoTransform = new double[6];
            float minX, minY, maxX, maxY;
            ds.GetGeoTransform(adfGeoTransform);
            minX = (float)(adfGeoTransform[0] + adfGeoTransform[2] * ds.RasterYSize);
            minY = (float)(adfGeoTransform[3] + adfGeoTransform[5] * ds.RasterYSize);
            maxX = (float)(adfGeoTransform[0] + adfGeoTransform[1] * ds.RasterXSize);
            maxY = (float)(adfGeoTransform[3] + adfGeoTransform[4] * ds.RasterXSize);
            float resolution = (float)(adfGeoTransform[1] - adfGeoTransform[2] * ds.RasterYSize / ds.RasterXSize);
            //矢量文件转栅格
            Envelope envelope = new Envelope(); //原图层外接矩形
            layer.GetExtent(envelope, 0);
            //计算栅格化后行列数  
            int xSize = Convert.ToInt32((envelope.MaxX - envelope.MinX) / resolution);
            int ySize = Convert.ToInt32((envelope.MaxY - envelope.MinY) / resolution);
            string outRaster = @"layerRas.shp";
            RasterizeLayer(layer, outRaster, "CODE", resolution, xSize, ySize);
            Dataset dsLayer = Gdal.Open(outRaster, Access.GA_ReadOnly);
            //计算偏置
            int xOffRas = 0, xOffLayer = 0, yOffRas = 0, yOffLayer = 0;
            int offset = Convert.ToInt32((envelope.MinX - minX) / resolution);
            if (offset > 0)
                xOffRas += offset;
            else
                xOffLayer -= offset;
            offset = Convert.ToInt32((envelope.MaxY - maxY) / resolution);
            if (offset > 0)
                yOffLayer += offset;
            else
                yOffRas -= offset;
            //计算长、宽
            int width, height;
            if (maxX > envelope.MaxX)
                maxX = (float)envelope.MaxX;
            if (minX < envelope.MinX)
                minX = (float)envelope.MinX;
            width = Convert.ToInt32((maxX - minX) / resolution);

            if (maxY > envelope.MaxY)
                maxY = (float)envelope.MaxY;
            if (minY < envelope.MinY)
                minY = (float)envelope.MinY;
            height = Convert.ToInt32((maxY - minY) / resolution);
            //提取band
            float[] rRas = new float[width * height];
            int[] rLayer = new int[width * height];
            Band bRas = ds.GetRasterBand(1);
            Band bLayer = dsLayer.GetRasterBand(1);
            bRas.ReadRaster(xOffRas, yOffRas, width, height, rRas, width, height, 0, 0);
            bLayer.ReadRaster(xOffLayer, yOffLayer, width, height, rLayer, width, height, 0, 0);
            //逐个像元统计计算
            List<float[]> result = new List<float[]>();//code-sum-max-min-num
            for (int i = 0; i < rRas.Length; i++)
            {
                int code = rLayer[i];
                if (code > 0)
                {
                    float value = rRas[i];
                    if (value > -30000)
                    {
                        int j = 0;
                        for (; j < result.Count(); j++)
                        {
                            if (result[j][0] == code)
                            {
                                result[j][1] += value;
                                result[j][4]++;
                                if (result[j][2] < value)
                                    result[j][2] = value;
                                if (result[j][3] > value)
                                    result[j][3] = value;
                                break;
                            }
                        }
                        if (j == result.Count())
                        {
                            float[] newCounty = { code, value, value, value, 1 };
                            result.Add(newCounty);
                        }
                    }
                }
            }
            dsLayer.FlushCache();
            dsLayer.Dispose();
            File.Delete(outRaster);
            //计算平均值
            for (int i = 0; i < result.Count(); i++)
                result[i][1] = result[i][1] / result[i][4];
            //将CODE与County_EN对应起来
            Feature feature;
            List<string[]> county = new List<string[]>();
            while ((feature = layer.GetNextFeature()) != null)
            {
                string[] str = { feature.GetFieldAsString("CODE"), feature.GetFieldAsString("County_EN") };
                county.Add(str);
            }
            //显示结果
            StatisticResult statisticResult = new StatisticResult(result, county);
            statisticResult.Show();
            return result;
        }

    }
}
