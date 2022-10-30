//Create the necessary classes
const ajaxManager = new AJAXManager("Home");

//Reference to the online document
let treatmentPDF = null;

/**
 * Load or attempt to load an available PDF from Firebase that corresponds with
 * the ID of the appointment that is being viewed.
 */
async function loadPDF(ID) {
	try {
		let success = await ajaxManager.retrievePDF(ID);

		if (success != null && success != "") {
			setupPreview(success);
        }
	}
	catch (err) {
		console.log(err);
	}
}

/**
 * Setup the preview section of the treatment page.
 * @param {any} url
 */
function setupPreview(url) {
	console.log(url);

	treatmentPDF = url;

	//Set the link as the source for the iframe?
	$("#treatmentPDF").attr("src", url + "#toolbar=0");

	//Set the anchor link with a reference to the url for downloading
	$("#downloadLink").attr("href", url);

	//Show the display area and hide the upload section
	$("#documentButtons").removeClass("hidden");
	$("#treatmentDocument").removeClass("hidden");
	$("#noDocument").addClass("hidden");

	$("#hide-pdf").removeClass("hidden");
}

/**
 * Return the user to the clinic day page when a treatment is completed.
 * Update the local storage to reflect the change.
 * */
function returnToClinicDay() {
	const queryString = window.location.search;
	const urlParams = new URLSearchParams(queryString);
	const appointmentID = urlParams.get('appointmentID');

	let currentClinic = JSON.parse(localStorage.getItem("ClinicDay"));
	//Update the local storage entry
	currentClinic.forEach(entry => {
		if (entry.appointmentID == appointmentID) {
			entry.status = "Complete";
			localStorage.setItem("ClinicDay", JSON.stringify(currentClinic));
		}
	});

	location.href = "/Home/ClinicDay";
}

$(document).ready(async () => {
	//Detect if the treatment is part of the clinic day
	const queryString = window.location.search;
	const urlParams = new URLSearchParams(queryString);
	const clinicDay = urlParams.get('clinicDay');

	if (clinicDay) {
		$("#completeTreatment").removeClass("hidden");

		$("#completeTreatment").on('click', function () {
			returnToClinicDay();
		});
    }

	//collect the ID that will be used to reference any documents
	const appointmentID = $("#appointmentID").text();

	/**
	 * On page load check if there is a document stored for this appointment
	 */
	await loadPDF(appointmentID);

	/**
	 * Allow a user to delete a document, freeing up space to upload another
	 */
	$("#deleteDocument").on('click', async () => {
		await ajaxManager.deletePDF(appointmentID);
		console.log("deleted");

		$("#treatmentDocument").addClass("hidden");
		$("#documentButtons").addClass("hidden");

		$("#noDocument").removeClass("hidden");

		hidePdfButton.classList.add("hidden");
	});

	//TODO ADD LOADING SYMBOL WHILE WAITING
	/**
	 * A listener function that is attached to the upload's input element. Responsible
	 * for selecting the file from the event and parsing it to a new ExcelToJSON object.
	 */
	handleFileSelect = async () => {
		let formData = new FormData();
		formData.append('Document', $('#fileUpload')[0].files[0]);
		formData.append('ID', appointmentID);

		let success = await ajaxManager.uploadPDF(formData);
		setupPreview(success);
	}

	$("#fileUpload").change(handleFileSelect);

	//Buttons for hiding the pdf viewer and the image matcher
	const pdfViewer = document.getElementById("treatmentDocument")
	const hidePdfButton = document.getElementById("hide-pdf")
	hidePdfButton.addEventListener('click', () => {
		pdfViewer.classList.toggle("hidden");

		if (pdfViewer.classList.contains("hidden")) {
			hidePdfButton.innerHTML = "Show PDF"
		} else {
			hidePdfButton.innerHTML = "Hide PDF"
		}
	})

	const imageMatcher = document.getElementById("matching-container")
	const hideMatcherButton = document.getElementById("hide-matching")
	hideMatcherButton.addEventListener('click', () => {
		imageMatcher.classList.toggle("hidden");

		if (imageMatcher.classList.contains("hidden")) {
			hideMatcherButton.innerHTML = "Show Image Matching"
		} else {
			hideMatcherButton.innerHTML = "Hide Image Matching"
		}
	})

	const machineControls = document.querySelector(".machine-controls-container")
	const hideMachineControlsButton = document.getElementById("hide-machine")
	hideMachineControlsButton.addEventListener('click', () => {
		machineControls.classList.toggle("hidden");

		if (machineControls.classList.contains("hidden")) {
			hideMachineControlsButton.innerHTML = "Show Machine Controls"
		} else {
			hideMachineControlsButton.innerHTML = "Hide Machine Controls"
		}
	})

	//Inputs and their handling to set local images in the image matcher
	//const bg_image_input = document.querySelector("#background-input");
	//bg_image_input.addEventListener("change", function () {
	//	const reader = new FileReader();
	//	reader.addEventListener("load", () => {
	//		const uploaded_image = reader.result;
	//		document.querySelector("#image-matcher").style.backgroundImage = `url(${uploaded_image})`;
	//	});
	//	reader.readAsDataURL(this.files[0]);
	//});

	//const draggable_image_input = document.querySelector("#draggable-input");
	//draggable_image_input.addEventListener("change", function () {
	//	const reader = new FileReader();
	//	reader.addEventListener("load", () => {
	//		const uploaded_image = reader.result;
	//		document.querySelector("#draggable-image").src = uploaded_image;
	//	});
	//	reader.readAsDataURL(this.files[0]);
	//});

	const machineStartButton = document.getElementById('start-button-area')
	machineStartButton.addEventListener('click', function () {
		alert("Machine Started")
	});

	const machineEmergButton = document.getElementById('stop-button-area')
	machineEmergButton.addEventListener('click', function () {
		alert("Machine Stopped")
	});
});