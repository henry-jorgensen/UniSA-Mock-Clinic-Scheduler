//Create necessary classes - provided through loading the class scripts on the CreateClinic.html
const ajaxManager = new AJAXManager("Coordinator");
const tableManager = new TableManager();
const scheduleManager = new ScheduleManager();

$(document).ready(function () {
    //Detect if there are any URL queries to load - this means the users is coming from the 
    //View Clinics page to edit a class
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    });

    tableManager.list = $("#previewList");

    //Prefill the inputs if editing a schedule
    if (params.name != null) {
        loadScheduleForEdit(params);
    }

    formWizard.showInitialTab(); // Display the first tab

    //On click listeners
    $("#generateNew").click(() => {
        console.log("Reshuffle list");
    });

    //Handle the multiple form submissions from the same page
    $("form").on('submit', async (e) => {
        e.preventDefault();
        let response;

        try {
            switch (e.currentTarget.id) {
                case "editAClinicForm":
                    //let data = scheduleManager.generateSchedule();

                    ////Schedules
                    //console.log(data[0]);
                    ////Locations
                    //console.log(data[1]);


                    //tableManager.generateSchedulePreview(
                    //    $("#scheduleHolder"),
                    //    $("#scheduleDateInput").val(),
                    //    data[1],
                    //    data[0]
                    //);

                    ////Populate the tables with the information
                    //performStep(1);

                    //break;

                case "saveAClinicForm":
                    ////Collect the tables that have been generated
                    //let schedule = scheduleManager.generateScheduleJSON();

                    //response = await ajaxManager.saveAClinic(
                    //    tableManager.selectedClass,
                    //    $("#scheduleNameInput").val(),
                    //    $("#scheduleDateInput").val(),
                    //    $("#scheduleStartTimeInput").val(),
                    //    $("#scheduleDurationInput").val(),
                    //    $("#locationInput").val(),
                    //    JSON.stringify(schedule)
                    //);

                    //if (response == null) {
                    //    return;
                    //}

                default:
                    break;
            }
        }
        catch (error) {
            console.log(error);
        }
    });
});

async function loadScheduleForEdit(params) {
    $("#scheduleNameInput").val(params.name);
    $("#scheduleDateInput").val(params.date);
    $("#scheduleStartTimeInput").val(params.startTime);
    $("#scheduleDurationInput").val(params.duration);
    $("#locationInput").val(params.locations);

    //TODO LOAD THE EXISTING SCHEDULE IN HERE
    let clinicJSON = await ajaxManager.loadASchedule(params.code);

    console.log(clinicJSON);

    let data = JSON.parse(clinicJSON.schedule);
    console.log(data);

    //Generate a preview
    tableManager.generateScheduleEditor(
        $("#scheduleHolder"),
        params.date,
        data
    );
}
