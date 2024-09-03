
using System.Windows;
using System.Text.Json;
using OxyPlot;
using OxyPlot.Series;
using System.Diagnostics;
using OxyPlot.Wpf;
using Microsoft.Win32;

using Accord.Math;


/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Markup;
 
 */

namespace life_table_wpf
{
	/// <summary>
	/// Window1.xaml 的交互逻辑
	/// </summary>
	public partial class Window1 : System.Windows.Window
	{
		public PlotModel Model { get; set; }
		public string log10ListString { get; set; }
		public Window1()
		{
			InitializeComponent();
		}

		public float[] ExponentialFunctionFitting(float[] X_Floats, float[] Y_Floats, out string func , out double corr_vect)
		{
			double[] X_double = X_Floats.Select(f => (double)f).ToArray();
			double[] Y_double = Y_Floats.Select(f => (double)f).ToArray();


			corr_vect =  Regression(X_double, Y_double, out double a_est, out double b_est, out double c_est, out double d_est);

			List<float> result = new List<float>();

            foreach (var x in X_double)
            {
				var y = function(a_est, b_est, c_est, d_est, x);
				var Y_F =(float)y;
				result.Add(Y_F);
			}
			func = $"{a_est.ToString("F4")} * arctan({b_est.ToString("F4")} * x + {c_est.ToString("F4")}) + {d_est.ToString("F4")}";
			var Y_Floats2 = result.ToArray();
			return Y_Floats2;
		}
		private void PhotoSave_Click(object sender, RoutedEventArgs e)
		{
			PlotModel plotModel = MyPlotView.Model;

			var pngExporter = new PngExporter { Width = 600, Height = 400 };
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.FileName = "result.png"; // Default file name
			saveFileDialog.DefaultExt = ".png"; // Default file extension
			saveFileDialog.Filter = "图像文件|*.png"; // Filter files by extension
												  // saveFileDialog.InitialDirectory = initFolder;
			if ((bool)saveFileDialog.ShowDialog())
			{
				string fileName = saveFileDialog.FileName;
				pngExporter.ExportToFile(plotModel, fileName);
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var tmp = new PlotModel { Subtitle = "using OxyPlot" };
			MainViewModel mainViewModel = new MainViewModel();
			var plotRules = JsonSerializer.Deserialize<PlotRules>(log10ListString);
			float[] nx_floats = plotRules.nXresult;
			// 生成一个从0开始的整数序列，与原始数组同长度
			int[] sequence = Enumerable.Range(0, nx_floats.Length).ToArray();
			// 将整数序列转换为float数组
			float[] X_floatSequence = sequence.Select(x => (float)x).ToArray();
			int i = 0;
			var titleString = "";
			if (plotRules.type == "Concave_line_type_Enum")
			{
				titleString = "凹线型"; Debug.WriteLine($"{titleString}");
				var Y_fit = ExponentialFunctionFitting(X_floatSequence, nx_floats, out string func, out double corr_vect);

				var lineSeries1 = new LineSeries
				{
					Color = OxyColors.Red
				};
				lineSeries1.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
				// lineSeries1.Points.Add(new DataPoint(0, Y_fit));
				int x_fit_item_i = 0;
				foreach (var Y_fit_item in Y_fit)
                {
					lineSeries1.Points.Add(new DataPoint(x_fit_item_i, Y_fit_item));
					x_fit_item_i++;
				}

                tmp.Series.Add(lineSeries1);

				if (tmp != null)
				{
					// tmp.Title = $"{titleString} 示例绘图";
					tmp.Subtitle = "拟合的曲线 y = "+ func + $"\tR = {corr_vect.ToString("F4")}";
				}

			}
			else if (plotRules.type == "Diagonal_line_type_Enum")
			{
				titleString = "对角线型"; Debug.WriteLine($"{titleString}");
				LeastSquareMethodResult leastSquareMethodResult = new LeastSquareMethodResult(X_floatSequence, nx_floats);
				List<float> Y_MEAN_floats_L = new List<float>();
				foreach (var x in X_floatSequence)
				{
					var Y_M = leastSquareMethodResult.getLineY(x);
					Y_MEAN_floats_L.Add(Y_M);
				}
				float[] Y_fit = Y_MEAN_floats_L.ToArray();

				var lineSeries = new LineSeries
				{
					Points = { new DataPoint(X_floatSequence[0], Y_fit[0]), new DataPoint(X_floatSequence[X_floatSequence.Length - 1], Y_fit[X_floatSequence.Length - 1]) },
					Color = OxyColors.Red
				};

				tmp.Series.Add(lineSeries);

				if (tmp != null)
				{
					// tmp.Title = $"{titleString} 示例绘图";
					tmp.Subtitle = $"拟合的直线 y = {leastSquareMethodResult.m:0.00}x + {leastSquareMethodResult.b:0.00}";
				}
			}
			else if (plotRules.type == "Convex_line_type_Enum")
			{
				titleString = "凸线型"; Debug.WriteLine($"{titleString}");
				var Y_fit = ExponentialFunctionFitting(X_floatSequence, nx_floats, out string func, out double corr_vect);

				var lineSeries1 = new LineSeries
				{
					Color = OxyColors.Red
				};
				lineSeries1.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
				// lineSeries1.Points.Add(new DataPoint(0, Y_fit));
				int x_fit_item_i = 0;
				foreach (var Y_fit_item in Y_fit)
				{
					lineSeries1.Points.Add(new DataPoint(x_fit_item_i, Y_fit_item));
					x_fit_item_i++;
				}

				tmp.Series.Add(lineSeries1);

				if (tmp != null)
				{
					// tmp.Title = $"{titleString} 示例绘图";
					tmp.Subtitle = "拟合的曲线 y = " + func + $"\tR = {corr_vect.ToString("F4")}";
				}


			}
			else
			{
				throw (new NoneLineTypeException("请在主页面选择类型！"));
			}

			// Title = $"{titleString} 示例绘图",

			if (tmp != null)
			{
				tmp.Title = $"{titleString} 示例绘图";
				// tmp.Subtitle = $"拟合的直线 y = {m:0.00}x + {b:0.00}";}​
			}
			var scatterSeries = new ScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 3,
				MarkerFill = OxyColors.Blue
			};

			// Create two line series (markers are hidden by default)
			// var series3 = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle };

			foreach (var nx_float in nx_floats)
			{
				scatterSeries.Points.Add(new ScatterPoint(i, nx_float));
				i = i + 1;
			}

			tmp.Series.Add(scatterSeries);

			mainViewModel.Model = tmp;
			//MessageBox.Show(log10ListString);
			Debug.WriteLine(log10ListString);
			// MessageBox.Show(plotRules.type + plotRules.nXresult.ToString());
			MyPlotView.Model = mainViewModel.Model;
		}

		#region LevenbergMarquardtFitting

		/// <summary>
		/// LM拟合，函数形式：a * Atan(b * x + c) + d; 
		/// </summary>
		/// <param name="data">自变量x</param>
		/// <param name="obs">观测值</param>
		/// <param name="a_est">估计的a</param>
		/// <param name="b_est">估计的b</param>
		/// <param name="c_est">估计的c</param>
		/// <param name="d_est">估计的d</param>
		/// <returns>相似度</returns>
		public static double Regression(double[] data, double[] obs, out double a_est, out double b_est, out double c_est, out double d_est)
		{
			double r = -1;
			if (data.Length != obs.Length)
			{
				throw new Exception("Observation Data and Correspaned Data are not the same dimention!");
			}
			// 数据个数
			int Ndata = obs.Length;

			// 参数维数
			int Nparams = 4;
			// 迭代最大次数
			int n_iters = 1000;
			// LM算法的阻尼系数初值
			double lamda = 0.01;

			// 定义雅克比矩阵
			double[,] J = new double[Ndata, Nparams];

			// 定义海塞矩阵
			double[,] H = new double[Nparams, Nparams];
			double[,] H_lm = new double[Nparams, Nparams];
			double[,] invH_lm = new double[Nparams, Nparams];

			// 定义误差向量
			double[] ev = new double[Ndata];
			double[] ev_lm = new double[Ndata];

			// 定义中间变量
			double[,] g = new double[Nparams, 1];
			double[] dp = new double[Nparams];

			// 初始猜测s
			double a0 = 0.434, b0 = 1.24e-02, c0 = 0.942, d0 = 0.685;

			double[] y_init = new double[Ndata];
			double[] y_est = new double[Ndata];
			double y_est_lm;

			for (int i = 0; i < Ndata; i++)
				y_init[i] = function(a0, b0, c0, d0, data[i]);

			// step1: 变量赋值
			int updateJ = 1;
			a_est = a0;
			b_est = b0;
			c_est = c0;
			d_est = d0;
			double linX, e = 0;
			double a_lm, b_lm, c_lm, d_lm, e_lm;

			// step2: 迭代
			for (int it = 0; it < n_iters; it++)
			{
				if (updateJ == 1)
				{
					//根据当前估计值，计算雅克比矩阵
					for (int i = 0; i < data.Length; i++)
					{
						linX = b_est * data[i] + c_est;
						J[i, 0] = Math.Atan(linX);
						J[i, 1] = a_est * data[i] / (linX * linX + 1);
						J[i, 2] = a_est / (linX * linX + 1);
						J[i, 3] = 1;
						// 根据当前参数，得到函数值
						y_est[i] = function(a_est, b_est, c_est, d_est, data[i]);
						// 计算误差
						ev[i] = obs[i] - y_est[i];
					}
					// 计算（拟）海塞矩阵
					H = Accord.Math.Matrix.TransposeAndMultiply(J, J);

					// 若是第一次迭代，计算误差
					if (it == 0)
						e = Accord.Math.Matrix.InnerProduct(ev, ev);
				}
				// 根据阻尼系数lamda混合得到H矩阵
				System.Array.Copy(H, H_lm, H.Length);
				for (int ii = 0; ii < Nparams; ii++)
					H_lm[ii, ii] = H[ii, ii] + lamda;
				// 计算步长dp，并根据步长计算新的可能的\参数估计值
				double det = H_lm.Determinant();
				if (det < 1.0e-10)
				{
					break;
				}
				else
				{
					invH_lm = H_lm.Inverse(false);
				}

				J.TransposeAndMultiply(ev, dp);
				dp = invH_lm.Multiply(dp);

				a_lm = a_est + dp[0];
				b_lm = b_est + dp[1];
				c_lm = c_est + dp[2];
				d_lm = d_est + dp[3];

				// 计算新的可能估计值对应的y和计算残差e
				for (int i = 0; i < data.Length; i++)
				{
					y_est_lm = function(a_lm, b_lm, c_lm, d_lm, data[i]);
					ev_lm[i] = obs[i] - y_est_lm;
				}
				e_lm = ev_lm.InnerProduct(ev_lm);
				//根据误差，决定如何更新参数和阻尼系数
				if (e_lm < e)
				{
					lamda /= 10.0f;
					a_est = a_lm;
					b_est = b_lm;
					c_est = c_lm;
					d_est = d_lm;
					e = e_lm;
					updateJ = 1;
				}
				else
				{
					updateJ = 0;
					lamda = lamda * 10.0f;
				}
			}
			r = corr_vect(y_est, obs);
			return r;
		}

		//定义函数格式
		private static double function(double a, double b, double c, double d, double x)
		{
			//  a * arctan(b * x + c) + d 
			return a * Math.Atan(b * x + c) + d;
		}

		// 计算向量的相关系数
		private static double corr_vect(double[] x, double[] y)
		{
			int n = x.Length;
			double sumX = 0, sumY = 0, sumX2 = 0, sumY2 = 0, sumXY = 0;
			for (int i = 0; i < n; i++)
			{
				sumX += x[i];
				sumY += y[i];
				sumX2 += x[i] * x[i];
				sumY2 += y[i] * y[i];
				sumXY += x[i] * y[i];
			}
			double lxx = sumX2 - sumX * sumX / n;
			double lyy = sumY2 - sumY * sumY / n;
			double lxy = sumXY - sumX * sumY / n;

			return (lxy * lxy / (lxx * lyy));
		}
		#endregion
	}

	public class LeastSquareMethodResult
	{
		public float m { get; set; }
		public float b { get; set; }

		public LeastSquareMethodResult(float[] X_Floats, float[] Y_Floats)
		{
			float a = 0;
			float c = 0;

			float x_mean = X_Floats.Average();
			float y_mean = Y_Floats.Average();

			//计算a和c
			for (int i = 0; i < X_Floats.Length; i++)
			{
				a += (X_Floats[i] - x_mean) * (Y_Floats[i] - y_mean);
				c += (X_Floats[i] - x_mean) * (X_Floats[i] - x_mean);
			}
			//计算斜率和截距
			float m_ = a / c;
			float b_ = y_mean - m_ * x_mean;
			this.m = m_;
			this.b = b_;
		}

		public float getLineY(float x) { return this.m * x + this.b; }
	}

	public class MainViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MainViewModel" /> class.
		/// </summary>
		public MainViewModel()
		{
			// Create the plot model
			var tmp = new PlotModel { Title = "Simple example", Subtitle = "using OxyPlot" };

			// Create two line series (markers are hidden by default)
			var series1 = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle };
			series1.Points.Add(new DataPoint(0, 0));
			series1.Points.Add(new DataPoint(10, 18));
			series1.Points.Add(new DataPoint(20, 12));
			series1.Points.Add(new DataPoint(30, 8));
			series1.Points.Add(new DataPoint(40, 15));

			var series2 = new LineSeries { Title = "Series 2", MarkerType = MarkerType.Square };
			series2.Points.Add(new DataPoint(0, 4));
			series2.Points.Add(new DataPoint(10, 12));
			series2.Points.Add(new DataPoint(20, 16));
			series2.Points.Add(new DataPoint(30, 25));
			series2.Points.Add(new DataPoint(40, 5));


			// Add the series to the plot model
			tmp.Series.Add(series1);
			tmp.Series.Add(series2);

			// Axes are created automatically if they are not defined

			// Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
			this.Model = tmp;
		}

		/// <summary>
		/// Gets the plot model.
		/// </summary>
		public PlotModel Model { get; set; }
	}

	public class PlotRules
	{
		public float[] nXresult { get; set; }
		public string type { get; set; }
	}

	public class NoneLineTypeException : ApplicationException
	{
		public NoneLineTypeException(string message) : base(message)
		{
		}
	}

	
}
