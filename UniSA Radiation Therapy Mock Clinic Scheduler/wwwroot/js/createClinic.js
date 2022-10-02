//Create necessary classes - provided through loading the class scripts on the CreateClinic.html
const ajaxManager = new AJAXManager("Coordinator");
const tableManager = new TableManager();
const scheduleManager = new ScheduleManager();

$(document).ready(function () {
    const formWizard = new FormWizard(0);
    formWizard.showInitialTab(); // Display the first tab

    tableManager.list = $("#previewList");

    preloadClasses();

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

        try {
            switch (e.currentTarget.id) {
                case "createAClinicForm":
                    let data = scheduleManager.generateSchedule();

                    //Schedules
                    console.log(data[0]);
                    //Locations
                    console.log(data[1]);
                    

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
