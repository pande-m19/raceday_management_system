using raceday_management_system.Models;

namespace raceday_management_system.Database.Interfaces
{//start of namespace
    public interface Ievents_connection
    {//start of interface

        List<Events> get_all_events();
        List<Events> get_upcoming_events();
        List<Events> get_organiser_events(int organiser_id);
        Events? get_event_by_id(int event_id);
        List<Categories> get_event_categories(int event_id);
        List<Enrolments> get_event_enrolments(int event_id);
        List<Models.Results> get_event_results(int event_id);

        //eeturns int (the new event ID)
        int create_event(int organiser_id, string event_name, string event_description, DateTime event_date, string location, decimal distance, string event_type, int? max_participants, decimal? entry_fee);

        string update_event(int event_id, string event_name, string event_description, DateTime event_date, string location, decimal distance, string event_type, string event_status, int? max_participants, decimal? entry_fee);
        string delete_event(int event_id);
        string set_event_banner(int event_id, string banner_url);

    }//end of interface

}//end of namespace