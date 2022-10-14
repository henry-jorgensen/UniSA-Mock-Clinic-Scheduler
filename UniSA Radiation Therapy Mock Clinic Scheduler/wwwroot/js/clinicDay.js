const ajaxManager = new AJAXManager("Coordinator");

$(document).ready(function () {
    //Check the local storage to see if there is a clinic in progress


    //If not clinic is in progress load the selection area
    loadClasses();


    $("#classSelection").on("change", (e) => {
        loadClinics($("#classSelection").val());
        $("#clinicSelection").prop("disabled", false);
    });

    $("#clinicSelection").on("change", (e) => {
        loadSelectedClinic($("#clinicSelection").val());
    });
});

//Load in the classes
async function loadClasses() {
    let result = await ajaxManager.loadAllClasses(); //reference to the dropdown menu

    if (result == -1) {
        //Handle error message to user
        return;
    }

    //Clear any previously loaded classes (except for the first disabled option)
    $("#classSelection").children().not(':first').remove();

    //Populate the list with the results
    result.forEach(entry => {
        ajaxManager.classes.push(entry);

        let element = $("<option>");
        element.text(entry.name + " : " + entry.year);
        element.val(entry.name);
        $("#classSelection").append(element);
    });
}

//Load the clinics
async function loadClinics(className) {
    let result = await ajaxManager.loadAllClinics(className); //reference to the dropdown menu

    if (result == -1) {
        //Handle error message to user
        return;
    }

    //Clear any previously loaded clinics (except for the first disabled option)
    $("#clinicSelection").children().not(':first').remove();

    //Get from dict
    //Populate the list with the results
    result.forEach(entry => {
        let element = $("<option>");
        element.text(entry[0]);
        element.val(entry[1]);
        $("#clinicSelection").append(element);
    });
}

//Load the selected clinic
async function loadSelectedClinic(scheduleCode) {
    let result = await ajaxManager.loadScheduleAppointments(scheduleCode); //reference to the dropdown menu

    if (result == -1) {
        //Handle error message to user
        return;
    }

    console.log(result);
}