// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function CreateErrorToast(message) {
	const errorToast = () => `
	<div id="liveToast" class="toast text-bg-danger" role="alert" aria-live="assertive" aria-atomic="true">
		<div class="toast-header">
			<strong class="me-auto">System Notification</strong>
			<button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
		</div>
		<div class="toast-body">
		${message}
		</div>
	</div>
`.trim();
	$(".toast-container").append(errorToast);
	$(".toast-container .toast:last").toast('show');
}

