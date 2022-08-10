public class Events
{
	private static readonly EventId custom_ = new EventId(999, "custom-event-name");

	public static EventId Custom
		=> custom_;
}
