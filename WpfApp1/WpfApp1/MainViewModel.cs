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
    using OxyPlot;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using System;

    /// <summary>
    /// Represents the view-model for the main window.
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            // Create the plot model
            var tmp = new PlotModel();

            DateTimeAxis xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
//                StringFormat = "dd/MM/yyyy HH:mm",
                StringFormat = "dd/MM HH:mm",

                //                Title = "Year",
                MinorIntervalType = DateTimeIntervalType.Days,
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
            };
            tmp.Axes.Add(xAxis);

            DateTime now = DateTime.Now;

            // Create two line series (markers are hidden by default)
            var series1 = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle };
            series1.Points.Add(new DataPoint(now.ToOADate(), 0));
            series1.Points.Add(new DataPoint(now.AddDays(1).ToOADate(), -18));
            series1.Points.Add(new DataPoint(now.AddDays(2).ToOADate(), -12));
            series1.Points.Add(new DataPoint(now.AddDays(3).ToOADate(), -8));
            series1.Points.Add(new DataPoint(now.AddDays(4).ToOADate(), -15));

            var series2 = new LineSeries { Title = "Series 2", MarkerType = MarkerType.Square };
            series2.Points.Add(new DataPoint(now.ToOADate(), -4));
            series2.Points.Add(new DataPoint(now.AddDays(1).ToOADate(), -12));
            series2.Points.Add(new DataPoint(now.AddDays(2).ToOADate(), -16));
            series2.Points.Add(new DataPoint(now.AddDays(3).ToOADate(), -25));
            series2.Points.Add(new DataPoint(now.AddDays(4).ToOADate(), -5));


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
        public PlotModel Model { get; private set; }
    }
}