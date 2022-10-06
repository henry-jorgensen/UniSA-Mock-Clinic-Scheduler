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

    //On click listeners
    $("#generateNew").click(() => {
        console.log("Reshuffle list");
    });

    //Handle the multiple form submissions from the same page
    $("form").on('submit', async (e) => {
        e.preventDefault();
        let response;

        $("#loader").css("display", "block");

        try {
            switch (e.currentTarget.id) {
                case "editAClinicForm":
                    //Update the schedule with the new details
                    //Assign the locations
                    scheduleManager.locations = $("#locationInput").val().split(",");

                    //TODO CHANGE THE TIMES IF NECCESSARY SOMEWHERE HERE!

                    //Collect the tables that have been generated
                    let schedule = scheduleManager.generateScheduleJSON();

                    response = await ajaxManager.editAClinic(
                        params.code,
                        $("#scheduleNameInput").val(),
                        $("#scheduleDateInput").val(),
                        $("#scheduleStartTimeInput").val(),
                        $("#scheduleDurationInput").val(),
                        $("#locationInput").val(),
                        JSON.stringify(schedule)
                    );

                    if (response == null) {
                        return;
                    }

                default:
                    break;
            }
        }
        catch (error) {
            console.log(error);
        }

        $("#loader").css("display", "none");
        window.location.replace("/Coordinator/Classes");
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
