// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Represents the view-model for the main window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfApp1
{
    using Newtonsoft.Json;
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System;
    using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
    using System.Data.SQLite;
    using System.Text.Json;
    using System.Xml.Linq;

    /// <summary>
    /// Represents the view-model for the main window.
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>

        public static DateTime UnixTimestampToDateTime(double unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }
        public MainViewModel()
        {
            // Create the plot model
            var tmp = new PlotModel();

            DateTimeAxis xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
//                StringFormat = "dd/MM/yyyy HH:mm",
                StringFormat = "dd/MM HH:mm",

                MinorIntervalType = DateTimeIntervalType.Days,
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
            };
            tmp.Axes.Add(xAxis);

            var series1 = new LineSeries { Title = "Min temperature", MarkerType = MarkerType.Circle };
            var series2 = new LineSeries { Title = "Max temperature", MarkerType = MarkerType.Square };
            var series3 = new LineSeries { Title = "Avg temperature", MarkerType = MarkerType.Triangle };

            string K2_lat = "35.88";
            string K2_lon = "76.51";
            string units = "metric";

            int positionId = 0;
            int unitsId = 0;

            SQLiteDB db = new SQLiteDB();
            using(var connection = db.createDbConnection())
            {
                db.command.Parameters.Clear();
                db.command.CommandText = "select id from LatLonPositions where lat = @lat and lon = @lon";
                db.command.Parameters.AddWithValue("@lat", K2_lat);
                db.command.Parameters.AddWithValue("@lon", K2_lon);


                SQLiteDataReader reader = db.command.ExecuteReader();
                if (reader.Read())
                    positionId = reader.GetInt32(0);
                reader.Close();

                db.command.Parameters.Clear();
                db.command.CommandText = "select id from Units where key = @key";
                db.command.Parameters.AddWithValue("@key", units);
                reader = db.command.ExecuteReader();
                if (reader.Read())
                    unitsId = reader.GetInt32(0);
                reader.Close();

                string json = "";

                db.command.Parameters.Clear();
                db.command.CommandText = "select json from ForecastJsons where unitId = @unitId and positionId = @positionId";
                db.command.Parameters.AddWithValue("@unitId", unitsId);
                db.command.Parameters.AddWithValue("@positionId", positionId);
                reader = db.command.ExecuteReader();
                if (reader.Read())
                    json = reader.GetString(0);
                reader.Close();

                dynamic stuff = JsonConvert.DeserializeObject(json);

                for(int i = 0; i < stuff.list.Count; i++)
                {
                    long dt = stuff.list[i].dt;
                    double temp = stuff.list[i].main.temp;
                    double temp_min = stuff.list[i].main.temp_min;
                    double temp_max = stuff.list[i].main.temp_max;
                    string dt_txt = stuff.list[i].dt_txt;
                    DateTime dt2 = UnixTimestampToDateTime(dt);

                    series1.Points.Add(new DataPoint(dt2.ToOADate(), temp_min));
                    series2.Points.Add(new DataPoint(dt2.ToOADate(), temp_max));
                    series3.Points.Add(new DataPoint(dt2.ToOADate(), temp));
                }


            }

            // Add the series to the plot model
            tmp.Series.Add(series1);
            tmp.Series.Add(series2);
            tmp.Series.Add(series3);

            // Axes are created automatically if they are not defined

            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            this.Model = tmp;
        }

        /// <summary>
        /// Gets the plot model.
        /// </summary>
        public PlotModel Model { get; private set; }
    }
}
