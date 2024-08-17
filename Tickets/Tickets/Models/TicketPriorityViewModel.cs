namespace Tickets.Models
{
    public class TicketPriorityViewModel
    {
        public int HighPriorityCount { get; set; }
        public int MediumPriorityCount { get; set; }
        public int LowPriorityCount { get; set; }

        public IEnumerable<Ticket> HighPriorityTickets { get; set; }
        public IEnumerable<Ticket> MediumPriorityTickets { get; set; }
        public IEnumerable<Ticket> LowPriorityTickets { get; set; }


    }
}
