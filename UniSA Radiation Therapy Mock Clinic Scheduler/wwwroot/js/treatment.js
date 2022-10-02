﻿//Create the necessary classes
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
		setupPreview(success);
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
	$("#displayDocument").removeClass("hidden");
	$("#noDocument").addClass("hidden");
}

$(document).ready(async () => {
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

		$("#displayDocument").addClass("hidden");
		$("#noDocument").removeClass("hidden");
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
});