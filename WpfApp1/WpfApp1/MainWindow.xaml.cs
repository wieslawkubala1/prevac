using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void bGetDataFromWeatherAPI_Click(object sender, RoutedEventArgs e)
        {
            string APPID = "7ac5de381df48cb1d4a61c8f066498c4";
            string K2_lat = "35.88";
            string K2_lon = "76.51";
            string units = "metric";

            HttpClient http = new HttpClient();

            string request1 = "http://api.openweathermap.org/data/2.5/weather?lat=" + K2_lat + "&lon=" + K2_lon + "&APPID=" + APPID + "&units=" + units;
            string result1 = await http.GetStringAsync(request1);

            string request2 = "http://api.openweathermap.org/data/2.5/forecast?lat=" + K2_lat + "&lon=" + K2_lon + "&APPID=" + APPID + "&units=" + units;
            string result2 = await http.GetStringAsync(request2);

            int positionId = 0;
            int unitsId = 0;

            SQLiteDB db = new SQLiteDB();
            using (var connection = db.createDbConnection())
            {
                db.command.Parameters.Clear();
                db.command.CommandText = "select id from LatLonPositions where lat = @lat and lon = @lon";
                db.command.Parameters.AddWithValue("@lat", K2_lat);
                db.command.Parameters.AddWithValue("@lon", K2_lon);


                SQLiteDataReader reader = db.command.ExecuteReader();
                if (reader.Read())
                    positionId = reader.GetInt32(0);
                reader.Close();

                if(positionId == 0)
                {
                    db.command.Parameters.Clear();
                    db.command.CommandText = @"insert into LatLonPositions (lat, lon)
                                                    (@lat, @lon)";
                    db.command.Parameters.AddWithValue("@lat", K2_lat);
                    db.command.Parameters.AddWithValue("@lon", K2_lon);
                    db.command.ExecuteNonQuery();

                    db.command.Parameters.Clear();
                    db.command.CommandText = "select id from LatLonPositions where lat = @lat and lon = @lon";
                    db.command.Parameters.AddWithValue("@lat", K2_lat);
                    db.command.Parameters.AddWithValue("@lon", K2_lon);


                    reader = db.command.ExecuteReader();
                    if (reader.Read())
                        positionId = reader.GetInt32(0);
                    reader.Close();

                }

                db.command.Parameters.Clear();
                db.command.CommandText = "select id from Units where key = @key";
                db.command.Parameters.AddWithValue("@key", units);
                reader = db.command.ExecuteReader();
                if(reader.Read())
                    unitsId = reader.GetInt32(0);
                reader.Close();

                db.command.Parameters.Clear();
                db.command.CommandText = "delete from WeatherJsons";
                db.command.ExecuteNonQuery();

                db.command.Parameters.Clear();
                db.command.CommandText = @"insert into WeatherJsons (generatedAt, unitId, positionId, json)
                                                values(@generatedAt, @unitId, @positionId, @json)
                                            ";
                db.command.Parameters.AddWithValue("@generatedAt", DateTime.Now);
                db.command.Parameters.AddWithValue("@unitId", unitsId);
                db.command.Parameters.AddWithValue("@positionId", positionId);
                db.command.Parameters.AddWithValue("@json", result1);
                db.command.ExecuteNonQuery();

                db.command.Parameters.Clear();
                db.command.CommandText = "delete from ForecastJsons";
                db.command.ExecuteNonQuery();

                db.command.Parameters.Clear();
                db.command.CommandText = @"insert into ForecastJsons (generatedAt, unitId, positionId, json, rangeDays, periodHours)
                                                values(@generatedAt, @unitId, @positionId, @json, 5, 3)
                                            ";
                /*
                 * In free version of Open Weather API forecasts have range equal 5 days and periods: 3 hours
                 */
                db.command.Parameters.AddWithValue("@generatedAt", DateTime.Now);
                db.command.Parameters.AddWithValue("@unitId", unitsId);
                db.command.Parameters.AddWithValue("@positionId", positionId);
                db.command.Parameters.AddWithValue("@json", result2);
                db.command.ExecuteNonQuery();



            }

            MessageBox.Show("The data was downloaded and saved to the database.");
        }
    }
}