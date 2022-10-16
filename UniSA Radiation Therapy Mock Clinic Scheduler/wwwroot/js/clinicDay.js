const ajaxManager = new AJAXManager("Coordinator");
let currentClinic = null;

$(document).ready(function () {
    //Check the local storage to see if there is a clinic in progress
    currentClinic = JSON.parse(localStorage.getItem("ClinicDay"));

    if (currentClinic != null) {
        setupSavedClinic(currentClinic);
    } else {
        setupNewClinic();
    }
});

/**
 * Setup the required elements for a previous saved clinic
 * */
function setupSavedClinic(clinic) {
    //Hide the selection area and show the schedule
    $("#savedClinicOptions").removeClass("hidden");
    $("#selectAClinic").addClass("hidden");
    $("#clinicArea").removeClass("hidden");

    $("#addBreak").on('click', function () {
        //Search for the current place to put a new table row for the break
        findListEntryPoint();
    });

    $("#newClinic").on('click', function () {
        if (confirm("WARNING: This will remove the current Clinic for memory, no progress will be saved!") == true) {
            localStorage.removeItem("ClinicDay");
            $("#mockClinic").empty();
            setupNewClinic();
        }
    });

    $("#updateClinic").on('click', function () {
        if (confirm("WARNING: This will update the clinic to reflect the changes in made.") == true) {
            let details = []
            let entries = $(".clinicAppointment");

            for (let i = 0; i < entries.length; i++) {
                let newData = {
                    "id": entries[i].id,
                    "time": entries[i].children[0].children[0].innerHTML,
                    "status": entries[i].children[6].children[0].innerHTML
                }

                details.push(JSON.stringify(newData));
            }
            
            ajaxManager.updateAppointments(details);
        }
    });

    //Load the schedule into the table
    clinic.forEach(entry => {
        console.log(entry)
        if (entry.type != null) {
            $("#mockClinic").append(createBreakRow(entry.start, entry.end));
        } else {
            $("#mockClinic").append(createClinicDayRow(entry));
        }
    });
}

/**
 * Setup the required elements to begin a new clinic
 * */
function setupNewClinic() {
    //If not clinic is in progress load the selection area
    loadClasses();

    //Hide the selection area and show the schedule
    $("#savedClinicOptions").addClass("hidden");
    $("#selectAClinic").removeClass("hidden");
    $("#clinicArea").addClass("hidden");

    $("#classSelection").on("change", (e) => {
        loadClinics($("#classSelection").val());
        $("#clinicSelection").prop("disabled", false);
    });

    $("#clinicSelection").on("change", (e) => {
        loadSelectedClinic($("#clinicSelection").val());
    });
}

/**
 * Load in the classes that are assoicated with this clinic
 */
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

/**
 * Load the clinics from the firebase database based on the class name selected from
 * the dropdown.
 */
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

/**
 * Load the selected clinic
 */
async function loadSelectedClinic(scheduleCode) {
    let result = await ajaxManager.loadScheduleAppointments(scheduleCode); //reference to the dropdown menu

    if (result == -1) {
        //Handle error message to user
        return;
    }

    //Organise the schedule by time
    result.sort((a, b) => a.time.localeCompare(b.time));

    //Save to local storage
    localStorage.setItem("ClinicDay", JSON.stringify(result));

    //Load the schedule into the table
    result.forEach(entry => {
        console.log(entry)
        $("#mockClinic").append(createClinicDayRow(entry));
    });

    //Hide the selection area and show the schedule
    $("#selectAClinic").addClass("hidden");
    $("#clinicArea").removeClass("hidden");
    $("#savedClinicOptions").removeClass("hidden");
}

/**
 * Add each appiontment to the mock clinic table.
 * @param {any} appointment
 */
function createClinicDayRow(appointment) {
    let tr = $('<tr>', { id: appointment.appointmentID, class: "clinicAppointment" });

    //set up the date area
    let tdDate = $('<td>');
    let h6Time = $('<h6>', { text: appointment.time });
    let date = $('<small>', { text: appointment.date });
    tdDate.append(h6Time, date);

    //setup the location
    let tdLocation = $('<td>');
    let h6Loc = $('<h6>', { text: appointment.room });
    tdLocation.append(h6Loc);

    //setup the patient
    let tdPatient = $('<td>');
    let h6Patient = $('<h6>', { text: appointment.patient });
    tdPatient.append(h6Patient);

    //setup infectious
    let tdInfectious = $('<td>');
    let h6Infectious = $('<h6>', { text: appointment.infectious });
    tdInfectious.append(h6Infectious);

    //setup radiation therapists
    let tdRads = $('<td>');
    let pRad1 = $('<p>', { text: appointment.radiationTherapist1 });
    let pRad2 = $('<p>', { text: appointment.radiationTherapist2 });
    tdRads.append(pRad1, pRad2);

    //setup the location
    let tdSite = $('<td>');
    let h6Site = $('<h6>', { text: appointment.site });
    tdSite.append(h6Site);

    //setup the status
    let tdStatus = $('<td>');
    let h6Status = $('<h6>', { text: appointment.status, id: appointment.appointmentID + "_status" });
    tdStatus.append(h6Status);

    //setup the treatment button
    let tdTreatment = $('<td>');
    let btnTreatment = $('<button>', { text: "Treatment", class: "btn button" });

    //Needs to update the Clinic in the local storage
    btnTreatment.on('click', function () {
        //Update the status
        currentClinic.forEach(entry => {
            if (entry.appointmentID == appointment.appointmentID) {
                entry.status = "In progress";

                console.log(currentClinic)

                $(`#${appointment.appointmentID}_status`).text("In Progress");

                localStorage.setItem("ClinicDay", JSON.stringify(currentClinic));
                return
            }
        });

        //Move to treatment
        let url = '/Home/Treatment/';
        url += appointment.appointmentID;
        url += `?date=${appointment.date}`;
        url += `&time=${appointment.time}`;
        url += `&room=${appointment.room}`;
        url += `&patient=${appointment.patient}`;
        url += `&infectious=${appointment.infectious}`;
        url += `&rt1=${appointment.radiationTherapist1}`;
        url += `&rt2=${appointment.radiationTherapist2}`;
        url += `&site=${appointment.site}`;
        url += `&complication=${appointment.complication}`;
        url += `&appointmentID=${appointment.appointmentID}`;
        url += `&clinicDay=true`;

        location.href = url;
    });

    tdTreatment.append(btnTreatment)

    tr.append(tdDate, tdLocation, tdPatient, tdInfectious, tdRads, tdSite, tdStatus, tdTreatment);

    return tr;
}

/**
 * Search for an entry point to place the break, this will search via an appointments
 * start time, inserting itself at the correct place and adjusting all times following.
 * */
function findListEntryPoint() {
    let start = $("#startTimeInput").val();
    let end = $("#endTimeInput").val();
    let newDuration = $("#durationInput").val();

    let children = $("#mockClinic").children();

    //determine the current appointment duration incase a new one is not supplied
    if (newDuration == "") {
        let moment1 = moment(currentClinic[1].time, 'h:mm');
        let moment2 = moment(currentClinic[0].time, 'h:mm');
        newDuration = moment.duration(moment1.diff(moment2)).asMinutes();
    }

    let startIndex = 0;

    //Find where to place the break on the clinic table
    for (let i = 0; i < children.length; i++) {
        if (start.localeCompare(children[i].children[0].children[0].innerHTML) <= 0) {
            console.log(i);

            //Insert here
            $('#mockClinic > tr').eq(i - 1).after(createBreakRow(start, end));
            startIndex = i;
            break;
        }
    }

    //Update the text and local storage with the new times
    let step = 0;
    for (let i = startIndex; i < children.length; i++) {
        var time = moment(end, 'HH:mm');
        time.add(step * newDuration, 'm');
        children[i].children[0].children[0].innerHTML = time.format("HH:mm");
        currentClinic[i].time = time.format("HH:mm"); //update the current clinic
        step++;
    }

    //Update the current clinic with the break item
    let breakItem = {
        "type": "break",
        "start": start,
        "end": end
    }
    currentClinic.splice(startIndex, 0, breakItem);

    //Update the local storage entry
    localStorage.setItem("ClinicDay", JSON.stringify(currentClinic));
}

/**
 * Create a basic row describing a break during the schedule with a delete button to
 * revert the schedule back to usual.
 * @param {any} start
 * @param {any} end
 */
function createBreakRow(start, end) {
    let tr = $('<tr>');

    let tdStart = $('<td>');
    let h6Start = $('<small>', { text: `Start: ${start}` });
    let br = $('<br>');
    let h6End = $('<small>', { text: `End: ${end}` });
    tdStart.append(h6Start, br, h6End);

    let tdBreak = $('<td>');
    let h6break = $('<h6>', { text: "BREAK" });
    tdBreak.append(h6break);

    let tdEmpty1 = $('<td>');
    let tdEmpty2 = $('<td>');
    let tdEmpty3 = $('<td>');
    let tdEmpty4 = $('<td>');
    let tdEmpty5 = $('<td>');

    //setup the delete button
    let tdDelete = $('<td>');
    let btnDelete = $('<button>', { text: "Delete", class: "btn button" });
    btnDelete.on('click', function (e) {
        console.log(e.target);
        if (confirm("Are you sure you want to delete this break?") == true) {
            e.target.parentNode.parentNode.remove();
            let startTime = e.target.parentNode.parentNode.children[0].children[0].innerHTML.replace("Start: ", "");

            let children = $("#mockClinic").children();

            //determine the current appointment duration pre-break
            let moment1 = moment(currentClinic[1].time, 'h:mm');
            let moment2 = moment(currentClinic[0].time, 'h:mm');
            let newDuration = moment.duration(moment1.diff(moment2)).asMinutes();

            //Update the text and local storage with the new times
            for (let i = 0; i < children.length; i++) {
                var time = moment(currentClinic[0].time, 'HH:mm');
                time.add(i * newDuration, 'm');
                children[i].children[0].children[0].innerHTML = time.format("HH:mm");
                currentClinic[i].time = time.format("HH:mm"); //update the current clinic
            }

            for (let x = 0; x < currentClinic.length; x++) {
                if (currentClinic[x].type != null) {
                    if (currentClinic[x].start == startTime) {
                        currentClinic.splice(x, 1);
                    }
                }
            }

            localStorage.setItem("ClinicDay", JSON.stringify(currentClinic));
        }
    });

    tdDelete.append(btnDelete);

    tr.append(tdStart, tdEmpty1, tdBreak, tdEmpty2, tdEmpty3, tdEmpty4, tdEmpty5, tdDelete);

    return tr;
}