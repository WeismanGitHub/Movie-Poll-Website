public static class ErrorHandlerClass {
	public static void ErrorHandler(this IApplicationBuilder app) {
		app.Use((HttpContext httpContext, Func<Task> next) => {
			return Task.CompletedTask;
		});
	}
}
