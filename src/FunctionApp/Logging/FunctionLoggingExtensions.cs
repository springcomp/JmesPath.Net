public static class FunctionLoggingExtensions
{
	public static void LogCustom(this ILogger logger, string trackingId, string message)
	{
		logger.LogDebug(
			Events.Custom,
			GlobalConstants.LogMessageTemplate,
			new[] {
				trackingId,
				message,
			}
		);
	}
}
