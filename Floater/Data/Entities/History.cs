using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sazid.github.io;
using System.Data;
using System.Collections;

namespace Floater.Data.Entities
{
    public class History
    {
        // keep Id internal so that its invisible in DataGrid
        internal int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime Timestamp { get; set; }

        public static List<History> GetHistories(string filter)
        {
            List<History> histories = new List<History>();
            ConnectionManager manager = new ConnectionManager();

            string query = "SELECT * FROM history ";

            if (filter != null)
            {
                // Prevent SQL injection by removing unescaped characters
                filter = ConnectionManager.Escape(filter);
                query += $" WHERE lower(title) LIKE '%{filter}%' OR lower(url) LIKE '%{filter}%' ";
            }

            query += " ORDER BY timestamp DESC";

            DataTable dt = manager.Query(query);

            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                histories.Add(new History
                {
                    Id = int.Parse(dt.Rows[i][0].ToString()),
                    Title = dt.Rows[i][2].ToString(),
                    Url = dt.Rows[i][1].ToString(),
                    Timestamp = DateTime.Parse(dt.Rows[i][3].ToString())
                });
            }

            return histories;
        }

        public static int CreateHistory(History h)
        {
            try
            {
                ConnectionManager manager = new ConnectionManager();
                return manager.NonQuery($"INSERT INTO history (url, title, timestamp) VALUES ('{h.Url}', '{h.Title}', '{h.Timestamp}')");
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static int DeleteHistory()
        {
            try
            {
                ConnectionManager manager = new ConnectionManager();
                return manager.NonQuery(@"DELETE FROM history");
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
