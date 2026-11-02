using System.Data.SqlClient;
using raceday_management_system.Models;
using raceday_management_system.Database.Interfaces;

namespace raceday_management_system.Database
{//start of namespace
    public class events_connection : Ievents_connection
    {//start of class

        //connection string to connect to the raceday_db
        private readonly string connection_string = @"Data source=(localdb)\api_raceday;Database=raceday_db;Trusted_Connection=True;TrustServerCertificate=True;";

        // helper method to map a SqlDataReader row to an Events model
        private Events map_event(SqlDataReader reader)
        {//start of map_event()

            return new Events
            {
                Event_id = Convert.ToInt32(reader["event_id"]),
                Organiser_id = Convert.ToInt32(reader["organiser_id"]),
                Event_name = reader["event_name"].ToString() ?? "",
                Event_description = reader["event_description"].ToString() ?? "",
                Event_date = Convert.ToDateTime(reader["event_date"]),
                Location = reader["location"].ToString() ?? "",
                Distance = Convert.ToDecimal(reader["distance"]),
                Event_type = reader["event_type"].ToString() ?? "",
                Banner_image_url = reader["banner_image_url"] == DBNull.Value ? null : reader["banner_image_url"].ToString(),
                Max_participants = reader["max_participants"] == DBNull.Value ? null : Convert.ToInt32(reader["max_participants"]),
                Entry_fee = reader["entry_fee"] == DBNull.Value ? null : Convert.ToDecimal(reader["entry_fee"]),
                Event_status = reader["event_status"].ToString() ?? "",
                Created_at = Convert.ToDateTime(reader["created_at"]),
                Updated_at = reader["updated_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["updated_at"])
            };

        }//end of map_event()

        //method to get all events
        public List<Events> get_all_events()
        {//start of get_all_events()

            List<Events> events = new List<Events>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from events order by event_date";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                events.Add(map_event(reader));

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return events;

        }//end of get_all_events()

        //method to get upcoming events
        public List<Events> get_upcoming_events()
        {//start of get_upcoming_events()

            List<Events> events = new List<Events>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"select * from events 
                                     where event_date > GETDATE() and event_status = 'Upcoming' 
                                     order by event_date";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                events.Add(map_event(reader));

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return events;

        }//end of get_upcoming_events()

        //method to get an organiser's events
        public List<Events> get_organiser_events(int organiser_id)
        {//start of get_organiser_events()

            List<Events> events = new List<Events>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from events where organiser_id = @OrganiserId order by event_date";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@OrganiserId", organiser_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                events.Add(map_event(reader));

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return events;

        }//end of get_organiser_events()

        //method to get a single event by id
        public Events? get_event_by_id(int event_id)
        {//start of get_event_by_id()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from events where event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            if (reader.Read())
                            {//start of if

                                var ev = map_event(reader);
                                connect.Close();
                                return ev;

                            }//end of if

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { return null; }//end of catch

            return null;

        }//end of get_event_by_id()

        //method to get categories for an event (returns Categories models)
        public List<Categories> get_event_categories(int event_id)
        {//start of get_event_categories()

            List<Categories> cats = new List<Categories>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from categories where event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                cats.Add(new Categories
                                {
                                    Category_id = Convert.ToInt32(reader["category_id"]),
                                    Event_id = Convert.ToInt32(reader["event_id"]),
                                    Category_name = reader["category_name"].ToString() ?? "",
                                    Cat_description = reader["cat_description"] == DBNull.Value ? null : reader["cat_description"].ToString(),
                                    Min_age = reader["min_age"] == DBNull.Value ? null : Convert.ToInt32(reader["min_age"]),
                                    Max_age = reader["max_age"] == DBNull.Value ? null : Convert.ToInt32(reader["max_age"]),
                                    Min_distance = reader["min_distance"] == DBNull.Value ? null : Convert.ToDecimal(reader["min_distance"]),
                                    Max_distance = reader["max_distance"] == DBNull.Value ? null : Convert.ToDecimal(reader["max_distance"]),
                                    Created_at = Convert.ToDateTime(reader["created_at"])
                                });

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return cats;

        }//end of get_event_categories()

        // method to get enrolments for an event (returns Enrolments models)
        public List<Enrolments> get_event_enrolments(int event_id)
        {//start of get_event_enrolments()

            List<Enrolments> enrols = new List<Enrolments>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from enrolments where event_id = @EventId order by enrolment_date desc";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                enrols.Add(new Enrolments
                                {
                                    Enrolment_id = Convert.ToInt32(reader["enrolment_id"]),
                                    Participant_id = Convert.ToInt32(reader["participant_id"]),
                                    Event_id = Convert.ToInt32(reader["event_id"]),
                                    Category_id = Convert.ToInt32(reader["category_id"]),
                                    Enrolment_date = Convert.ToDateTime(reader["enrolment_date"]),
                                    Enr_status = reader["enr_status"].ToString() ?? "",
                                    Payment_status = reader["payment_status"] == DBNull.Value ? null : reader["payment_status"].ToString(),
                                    Payment_amount = reader["payment_amount"] == DBNull.Value ? null : Convert.ToDecimal(reader["payment_amount"]),
                                    Comments = reader["comments"] == DBNull.Value ? null : reader["comments"].ToString(),
                                    Updated_at = reader["updated_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["updated_at"])
                                });

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return enrols;

        }//end of get_event_enrolments()

        //method to get results for an event (returns Results models)
        public List<Models.Results> get_event_results(int event_id)
        {//start of get_event_results()

            List<Models.Results> results = new List<Models.Results>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from results where event_id = @EventId order by overall_position";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                results.Add(new Models.Results
                                {
                                    Result_id = Convert.ToInt32(reader["result_id"]),
                                    Enrolment_id = Convert.ToInt32(reader["enrolment_id"]),
                                    Event_id = Convert.ToInt32(reader["event_id"]),
                                    Participant_id = Convert.ToInt32(reader["participant_id"]),
                                    Finish_time = (TimeSpan)reader["finish_time"],
                                    Overall_position = Convert.ToInt32(reader["overall_position"]),
                                    Category_position = reader["category_position"] == DBNull.Value ? null : Convert.ToInt32(reader["category_position"]),
                                    Total_finishers = Convert.ToInt32(reader["total_finishers"]),
                                    Category_total = reader["category_total"] == DBNull.Value ? null : Convert.ToInt32(reader["category_total"]),
                                    Is_disqualified = Convert.ToBoolean(reader["is_disqualified"]),
                                    Disqualification_reason = reader["disqualification_reason"] == DBNull.Value ? null : reader["disqualification_reason"].ToString(),
                                    Recorded_by = Convert.ToInt32(reader["recorded_by"]),
                                    Recorded_at = Convert.ToDateTime(reader["recorded_at"]),
                                    Updated_at = reader["updated_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["updated_at"])
                                });

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return results;

        }//end of get_event_results()

        //method to create an event (returns success or error message)
        //method to create an event (returns new event ID)
        public int create_event(int organiser_id, string event_name, string event_description, DateTime event_date, string location, decimal distance, string event_type, int? max_participants, decimal? entry_fee)
        {//start of create_event()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    // CHANGED: Added SCOPE_IDENTITY() to get the new ID
                    string query = @"insert into events 
                                     (organiser_id, event_name, event_description, event_date, location, 
                                      distance, event_type, max_participants, entry_fee, event_status, created_at) 
                                     values 
                                     (@OrganiserId, @EventName, @EventDescription, @EventDate, @Location, 
                                      @Distance, @EventType, @MaxParticipants, @EntryFee, 'Upcoming', GETDATE());
                                     SELECT CAST(SCOPE_IDENTITY() as int);";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@OrganiserId", organiser_id);
                        cmd.Parameters.AddWithValue("@EventName", event_name);
                        cmd.Parameters.AddWithValue("@EventDescription", event_description);
                        cmd.Parameters.AddWithValue("@EventDate", event_date);
                        cmd.Parameters.AddWithValue("@Location", location);
                        cmd.Parameters.AddWithValue("@Distance", distance);
                        cmd.Parameters.AddWithValue("@EventType", event_type);
                        cmd.Parameters.AddWithValue("@MaxParticipants", max_participants.HasValue ? (object)max_participants.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@EntryFee", entry_fee.HasValue ? (object)entry_fee.Value : DBNull.Value);

                        // CHANGED: ExecuteScalar returns the new ID
                        int new_id = Convert.ToInt32(cmd.ExecuteScalar());
                        return new_id;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception)
            {//start of catch

                return 0; //return 0 to indicate failure

            }//end of catch

        }//end of create_event()

        //method to update an event
        public string update_event(int event_id, string event_name, string event_description, DateTime event_date, string location, decimal distance, string event_type, string event_status, int? max_participants, decimal? entry_fee)
        {//start of update_event()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"update events 
                                     set event_name = @EventName, 
                                         event_description = @EventDescription, 
                                         event_date = @EventDate, 
                                         location = @Location, 
                                         distance = @Distance, 
                                         event_type = @EventType, 
                                         event_status = @EventStatus, 
                                         max_participants = @MaxParticipants, 
                                         entry_fee = @EntryFee, 
                                         updated_at = GETDATE() 
                                     where event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventName", event_name);
                        cmd.Parameters.AddWithValue("@EventDescription", event_description);
                        cmd.Parameters.AddWithValue("@EventDate", event_date);
                        cmd.Parameters.AddWithValue("@Location", location);
                        cmd.Parameters.AddWithValue("@Distance", distance);
                        cmd.Parameters.AddWithValue("@EventType", event_type);
                        cmd.Parameters.AddWithValue("@EventStatus", event_status);
                        cmd.Parameters.AddWithValue("@MaxParticipants", max_participants.HasValue ? (object)max_participants.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@EntryFee", entry_fee.HasValue ? (object)entry_fee.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Event updated successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of update_event()

        //method to soft-delete an event
        public string delete_event(int event_id)
        {//start of delete_event()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "update events set event_status = 'Cancelled', updated_at = GETDATE() where event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);
                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Event deleted successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of delete_event()

        //method to set an event banner URL
        public string set_event_banner(int event_id, string banner_url)
        {//start of set_event_banner()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "update events set banner_image_url = @BannerUrl, updated_at = GETDATE() where event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@BannerUrl", banner_url);
                        cmd.Parameters.AddWithValue("@EventId", event_id);
                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Banner updated successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of set_event_banner()

    }//end of class

}//end of namespace