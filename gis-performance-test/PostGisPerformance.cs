using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace main
{
    public class PostGisPerformance
    {
        private const string ConnString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";
        private const string Query = "SELECT ST_AsText(geom) as geom FROM public.tm_world_borders";
        private readonly Random _rnd;

        public PostGisPerformance()
        {
            _rnd = new Random();

            GetRandomValue(1);
            GetRandomValue(10);
            GetRandomValue(100);
            GetRandomValue(1000);
        }

        private void GetRandomValue(int numRuns)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Parallel.For(0, numRuns, index =>
            {
                using (var conn = new NpgsqlConnection(ConnString))
                {
                    conn.Open();

                    var gid = _rnd.Next(0, 246).ToString();

                    using (var cmd = new NpgsqlCommand(Query + " WHERE gid = " + gid, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var value = reader.GetString(0);
//                                Console.WriteLine(value);
                            }
                        }
                    }
                }
            });
            Console.WriteLine(numRuns + " took: " + watch.ElapsedMilliseconds + " ms.");
        }
    }
}