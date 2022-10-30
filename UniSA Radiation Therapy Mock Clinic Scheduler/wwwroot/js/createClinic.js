//Create necessary classes - provided through loading the class scripts on the CreateClinic.html
const ajaxManager = new AJAXManager("Coordinator");
const tableManager = new TableManager();
const scheduleManager = new ScheduleManager();

$(document).ready(function () {
    const formWizard = new FormWizard(0);

    tableManager.list = $("#previewList");

    //Prefill the inputs if editing a schedule
    preloadClasses();

    formWizard.showInitialTab(); // Display the first tab

    //When a class is selected load the class value into the table
    $("#classSelection").on('change', function (e) {
        //Clear the table from any previous entries
        tableManager.clearTable(tableManager.list);

        //Get the selected class value
        let className = $(this).val()
        tableManager.selectedClass = className;

        populateTable(className, false);
    });

    //Handle the multiple form submissions from the same page
    $("form").on('submit', async (e) => {
        e.preventDefault();
        let response;

        $("#loader").css("display", "block");

        try {
            switch (e.currentTarget.id) {
                case "createAClinicForm":
                    let data = scheduleManager.generateSchedule();

                    //Assign site values to the table manager
                    let sites = await ajaxManager.collectSites();
                    tableManager.sites = sites;
                    

                    tableManager.generateSchedulePreview(
                        $("#scheduleHolder"),
                        $("#scheduleDateInput").val(),
                        data[1],
                        data[0]
                    );

                    //Populate the tables with the information
                    performStep(1);

                    break;

                case "saveAClinicForm":
                    //Collect the tables that have been generated
                    let schedule = scheduleManager.generateScheduleJSON();

                    response = await ajaxManager.saveAClinic(
                        tableManager.selectedClass,
                        $("#scheduleNameInput").val(),
                        $("#scheduleDateInput").val(),
                        $("#scheduleStartTimeInput").val(),
                        $("#scheduleDurationInput").val(),
                        $("#locationInput").val(),
                        JSON.stringify(schedule)
                    );

                    console.log(response);
                    console.log("Success");
                    $("#loader").css("display", "none");
                    window.location.replace("/Coordinator/Clinics");

                default:
                    break;
            }
        }
        catch (error) {
            console.log(error);
        }

        $("#loader").css("display", "none");
    });

    //Handling form movement buttons
    $("#prevBtn").on('click', () => formWizard.nextPrev(-1));
    $("#moveToGenerateBtn").on("click", function () {
        //Detect if a class has been chosen
        if ($("#classSelection").val() != null) {
            performStep(1);
        }
        else {
            console.log("No value chosen");
        }
    });

    //Perform a forward or backward step in the form wizard
    performStep = (stepNum) => {
        if (stepNum != 0 && stepNum != null) {
            formWizard.nextPrev(stepNum);
        }
    }

    //Handle adding a break to the schedule
    $("#addBreak").on('click', function () {
        //Search for the current place to put a new table row for the break
        findListEntryPoint();
    });

    //setup modal listeners
    let compModal = $("#complicationModal");
    let compClose = $("#closeComplicationButton");

    compClose.on('click', function () {
        compModal.css("display", "none");
    });

    $("#saveComplicationDetails").on('click', function () {
        tableManager.saveComplicationData();
        compModal.css("display", "none");
        //Reset the modal
        $("#modalComplicationDetails").val("");
    });


    let infecModal = $("#infectionModal");
    let infecClose = $("#closeButton");
    infecClose.on('click', function () {
        infecModal.css("display", "none");
    });

    $("#saveInfectionDetails").on('click', function () {
        tableManager.saveInfectionData();
        infecModal.css("display", "none");

        //Reset the modal
        $("#modalInfectionTitle").val("")
        $("#modalinfectionDetails").val("");
    });
});

//Preload classes as the drop down options when the page loads
async function preloadClasses() {
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

//Populate the preview table with the selected classes students
async function populateTable(className) {
    response = await ajaxManager.loadAClass(
        className
    );

    //The current class
    tableManager.classAdd(response);
}

/**
 * Search for an entry point to place the break, this will search via an appointments
 * start time, inserting itself at the correct place and adjusting all times following.
 * */
function findListEntryPoint() {
    let start = $("#startTimeInput").val();
    let end = $("#endTimeInput").val();
    let newDuration = $("#durationInput").val();

    //TODO add for each location in here
    scheduleManager.locations.forEach(location => {
        let table = location.trim();
        let children = $(`#${table}`).children();

        //determine the current appointment duration incase a new one is not supplied
        if (newDuration == "") {
            let moment1 = moment(children[1].children[0].innerHTML, 'h:mm');
            let moment2 = moment(children[0].children[0].innerHTML, 'h:mm');
            newDuration = moment.duration(moment1.diff(moment2)).asMinutes();
        }

        let startIndex = 0;

        //Find where to place the break on the clinic table
        for (let i = 0; i < children.length; i++) {
            if (start.localeCompare(children[i].children[0].innerHTML) <= 0) {
                //Insert here
                $(`#${table} > tr`).eq(i - 1).after(createBreakRow(start, end));
                startIndex = i;
                break;
            }
        }

        //Update the text and local storage with the new times
        let step = 0;
        for (let i = startIndex; i < children.length; i++) {
            var time = moment(end, 'HH:mm');
            time.add(step * newDuration, 'm');
            children[i].children[0].innerHTML = time.format("HH:mm");
            step++;
        }
    });
}

/**
 * Create a basic row describing a break during the schedule with a delete button to
 * revert the schedule back to usual.
 * @param { any } start
 * @param { any } end
 */
function createBreakRow(start, end) {
    let tr = $('<tr>');

    let tdStart = $('<td>', { id: `${start}` });
    let h6Start = $('<small>', { text: `${start} - ${end}` });
    tdStart.append(h6Start);

    let tdBreak = $('<td>', { text: "BREAK" });

    let tdEmpty1 = $('<td>');
    let tdEmpty2 = $('<td>');
    let tdEmpty3 = $('<td>');
    let tdEmpty4 = $('<td>');

    //setup the delete button
    let tdDelete = $('<td>');
    let btnDelete = $('<button>', { text: "Remove", class: "btn button" });
    btnDelete.on('click', function (e) {
        if (confirm("Are you sure you want to delete this break?") == true) {
            let table = e.target.parentNode.parentNode.parentElement.id;

            e.target.parentNode.parentNode.remove();
            let children = $(`#${table}`).children();;

            //determine the current appointment duration pre-break
            let moment1 = moment(children[1].children[0].innerHTML, 'h:mm');
            let moment2 = moment(children[0].children[0].innerHTML, 'h:mm');
            let newDuration = moment.duration(moment1.diff(moment2)).asMinutes();

            //Update the text and local storage with the new times
            for (let i = 0; i < children.length; i++) {
                var time = moment(children[0].children[0].innerHTML, 'HH:mm');
                time.add(i * newDuration, 'm');
                children[i].children[0].innerHTML = time.format("HH:mm");
            }
        }
    });

    tdDelete.append(btnDelete);

    tr.append(tdStart, tdBreak, tdEmpty1, tdEmpty2, tdEmpty3, tdEmpty4, tdDelete);

    return tr;
}
