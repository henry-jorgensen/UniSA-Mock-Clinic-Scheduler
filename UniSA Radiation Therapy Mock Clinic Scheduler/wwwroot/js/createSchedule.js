//TODO SEPARATE THESE OUT TO AN ACTUAL CLASS FILE AND IMPORT THEM
class TableManager {
    constructor(list = null, firstNameInput = null, lastNameInput = null, studentIdInput = null, studentUserNameInput = null) {
        this.listNum = 0;
        this.list = list;
        this.fNInput = firstNameInput;
        this.lNInput = lastNameInput;
        this.sIInput = studentIdInput;
        this.sUInput = studentUserNameInput;
    }

    /**
     * Remove any entries that are currently within the table.
     */
    clearTable = (table) => {
        table.empty();
    }

    /**
     * Collect the inputs from the class list form, using the data create a new table
     * row that reflects the data and append it to the main table.
     */
    addList = () => {
        let row = this.createRow(
            this.fNInput.val(),
            this.lNInput.val(),
            this.sIInput.val(),
            this.sUInput.val(),
            true
        );

        this.list.append(row);

        this.resetInputs();
        this.listNum++;
    }

    /**
     * Using the data read from an uploaded excel spreadsheet create a new table
     * row that reflects the data and append it to the main table.
     */
    autoAdd = (object) => {
        object.forEach(row => {
            let nameSplit = row["Student Name"].split(",");
            let newRow = this.createRow(
                nameSplit[0],
                nameSplit[1].trim(),
                row["Student ID"],
                row["Student Username"],
                true
            );

            this.list.append(newRow);
        });
    }

    /**
     * Using the data retrieved from firebase add the entire classes student list to the
     * table.
     */
    classAdd = (classObject, isPreview) => {
        let split = classObject.students.split("|");

        if (split[0] == "") {
            return;
        }

        split.forEach(student => {
            let jStudent = JSON.parse(student);
            let row = this.createRow(
                jStudent.FirstName,
                jStudent.LastName,
                jStudent.StudentId,
                jStudent.Username,
                isPreview
            );

            this.list.append(row);
        });
    }

    /**
     * Using string literal and html create and return a new table row element with the supplied
     * information as the filler details.
     */
    createRow = (fname, lname, id, uname, canDelete) => {
        let tr = $('<tr>', {
            class: "studentEntry"
        })

        let tdFName = $('<td>', { text: fname });
        let tdLName = $('<td>', { text: lname });
        let tdId = $('<td>', { text: id });
        let tdUName = $('<td>', { text: uname });

        //Create delete button
        if (canDelete) {
            let tdDelete = $('<td>');
            let button = $('<button>', { id: `text${this.listNum}`, text: "X" })
            button.click((id) => {
                id.currentTarget.parentNode.parentNode.remove();
            });

            tdDelete.append(button);


            tr.append(tdFName, tdLName, tdId, tdUName, tdDelete);
        } else {
            tr.append(tdFName, tdLName, tdId, tdUName);
        }

        return tr;
    }

    /**
     * Add a newly generated schedule to the supplied table.
     */
    scheduleAdd = (scheduleObject, table) => {
        if (scheduleObject == "") {
            return;
        }

        scheduleObject.forEach(student => {
            let row = this.createScheduleRow(
                student.Time,
                student.Patient,
                student.Site,
                student.RT1,
                student.RT2
            );

            table.append(row);
        });
    }

    /**
     * Used to create a new row in the scheduling table. Using string literal and html create and 
     * return a new table row element with the supplied information as the filler details.
     */
    createScheduleRow = (time, patient, site, RT1, RT2) => {
        let tr = $('<tr>', {
            class: "scheduleEntry"
        })

        let tdTime = $('<td>', { text: time });
        let tdPatient = $('<td>', { text: patient });
        let tdSite = $('<td>', { text: site });
        let tdRT1 = $('<td>', { text: RT1 });
        let tdRT2 = $('<td>', { text: RT2 });

        //Create delete button
        tr.append(tdTime, tdPatient, tdSite, tdRT1, tdRT2);

        return tr;
    }

    /**
     * After a new entry has been appended clear the inputs for the next entry.
     */
    resetInputs = () => {
        this.fNInput.val("");
        this.lNInput.val("");
        this.sIInput.val("");
        this.sUInput.val("");
    }

    /**
     * Populate the preview table with the values that were in the main table when saved.
     */
    populatePreview = (entries, previewList) => {
        for (let x = 0; x < entries.length; x++) {
            let cells = entries[x].cells;
            let row = this.createRow(cells[0].textContent, cells[1].textContent, cells[2].textContent, cells[3].textContent, false);
            previewList.append(row)
        };
    }
}

class AJAXManager {
    constructor(controller) {
        this.controller = controller;
        this.selectedClass = null;
        this.selectedClassCode = null;
        this.classes = []; // hold each class that is grabbed from firebase
    }

    /**
     * Collected the form inputs for the new class and then POST them to the parent controller
     * using the AJAX post method. A successful call will increment the form wizard to the
     * appropriate form.
     */
    createAClass = async (classNameValue, studyPeriodValue, semesterValue, yearValue) => {
        this.selectedClass = classNameValue;

        let response = await $.ajax({
            type: 'POST',
            url: `/${this.controller}/CreateAClass`,
            data: {
                name: classNameValue,
                studyPeriod: studyPeriodValue,
                semester: semesterValue,
                year: yearValue
            },
            success: function (result) {
                console.log(result); //Keep as log for now for testing
                return result;
            },
            failure: function (result) {
                console.log(result);
                return 0;
            }
        });

        if (response == 0) {
            return 0;
        } else {
            let classObject = JSON.parse(response);
            this.selectedClassCode = classObject.ClassCode;
            return 2; //2 to skip over the load class form
        }
    };

    /**
     * Load all the classes associated with the current user.
     */
    loadAllClasses = async () => {
        return await $.ajax({
            type: 'GET',
            url: `/${this.controller}/LoadAllClasses`,
            data: {},
            success: function (result) {
                console.log(result);
                return result;
            },
            failure: function (error) {
                console.log(error);
                return -1;
            }
        });
    }

    /**
     * Perform a GET call to the parent controller, the objective is to retrieve details about
     * a previously saved class. A successful call will increment the form wizard to the
     * appropriate form.
     */
    loadAClass = async (classNameValue) => {
        this.selectedClass = classNameValue;

        return $.ajax({
            type: 'GET',
            url: `/${this.controller}/LoadAClass`,
            data: {
                className: classNameValue
            },
            success: function (result) {
                //Populate the list with the results
                return result
            },
            failure: function (error) {
                console.log(error);
                return 0;
            }
        });
    };

    /**
     * After a class list has been either manually entered or automatically loaded perform a
     * POST call to the parent controller. The information passed is an array of student objects
     * that reflects what was in the table.
     */
    saveAClassList = async (tableClass) => {
        let entries = tableClass;
        let studentArray = [];

        console.log(entries);
        for (let x = 0; x < entries.length; x++) {
            let cells = entries[x].cells;

            let studentObject = {
                FirstName: cells[0].textContent,
                LastName: cells[1].textContent,
                StudentId: cells[2].textContent,
                Username: cells[3].textContent
            }

            studentArray.push(JSON.stringify(studentObject));
        };

        console.log(studentArray.toString());

        console.log(this.selectedClassCode);

        return await $.ajax({
            type: 'POST',
            url: `/${this.controller}/SaveAClassList`,
            data: {
                classCode: this.selectedClassCode,
                className: this.selectedClass,
                studentList: studentArray
            },
            success: function (result) {
                console.log(result);
                return 1;
            },
            failure: function (result) {
                console.log(result);
                return 0;
            }
        });
    };

    /**
     * Collected the form inputs for the new class and then POST them to the parent controller
     * using the AJAX post method. A successful call will increment the form wizard to the
     * appropriate form.
     */
    createASchedule = async (classNameValue, scheduleNameValue, dateValue, startTimeValue, appointmentDurationValue, locationsValue, scheduleValue) => {

        let response = await $.ajax({
            type: 'POST',
            url: `/${this.controller}/CreateASchedule`,
            data: {
                className: classNameValue,
                name: scheduleNameValue,
                date: dateValue,
                startTime: startTimeValue,
                appointmentDuration: appointmentDurationValue,
                locations: locationsValue,
                schedule: scheduleValue
            },
            success: function (result) {
                console.log(result); //Keep as log for now for testing
                return result;
            },
            failure: function (result) {
                console.log(result);
                return 0;
            }
        });

        if (response == 0) {
            return 0;
        } else {
            let scheduleObject = JSON.parse(response);
            console.log(scheduleObject);
            return 1; //2 to skip over the load class form
        }
    };
}

class FormWizard {
    constructor(startingTab) {
        this.currentTab = startingTab;
    }

    /**
     * Display the initial starting form.
     */
    showInitialTab = () => {
        this.showTab(this.currentTab)
    }

    /**
     * Change the display property of the appropriate tab in regards to the current
     * tab number.
     */
    showTab = (n) => {
        // This function will display the specified tab of the form ...
        var x = $(".tab");
        x[n].style.display = "block";

        $(".step")[this.currentTab].classList += " finish";

        //Sort out the next/previous buttons
        n == 0 ? $("#prevBtn").css("display", "none") : $("#prevBtn").css("display", "inline");

        this.fixStepIndicator(n)
    }

    /**
     * Change the visible form tab by incrementing or decrementing the current tab
     * value. If moving backwards, undo the 'finished' effect placed on the form's
     * step.
     */
    nextPrev = (n) => {
        var x = $(".tab");
        x[this.currentTab].style.display = "none"; // Hide the current tab

        if (n < 0) {
            $(".step")[this.currentTab].classList.remove("finish");
        }

        this.currentTab = this.currentTab + n;

        if (this.currentTab >= x.length) { // if you have reached the end of the form
            //...the form gets submitted:
            // document.getElementById("regForm").submit();
            return false;
        }

        this.showTab(this.currentTab); // Otherwise, display the correct tab
    }

    /**
     * Change a form's step display to reflect that the form has been completed. A
     * step is the small circlur object displayed at the bottom of the form.
     */
    fixStepIndicator = (n) => {
        // This function removes the "active" class of all steps...
        var i, x = $(".step");
        for (i = 0; i < x.length; i++) {
            x[i].className = x[i].className.replace(" active", "");
        }
        //... and adds the "active" class to the current step:
        x[n].className += " active";
    }
}

//Create necessary classes
const ajaxManager = new AJAXManager("Coordinator");
const tableManager = new TableManager();

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
                case "createAScheduleForm":
                    //Optimise the schedule -- WHEN COMPLETE ADD TO THE AJAX BELOW
                    let schedule = optimiseSchedule(
                        $("#scheduleStartTimeInput").val(),
                        $("#scheduleDurationInput").val(),
                        $("#locationInput").val(),
                        $(".studentEntry")
                    );

                    generateSchedulePreview(
                        $("#scheduleDateInput").val(),
                        schedule
                    );

                    //response = await ajaxManager.createASchedule(
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

                    //Populate the tables with the information
                    performStep(1);

                    break;

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

//TODO finish this in the next sprint
function optimiseSchedule(startTime, duration, location, entries) {
    let arrayScheduleData = [];

    // Can split the list at the end into however many sections need based on the number of locations
    let numOfLocations = location.split(",");

    let headers = ["Time", "Patient", "Site", "RT1", "RT2"];

    //TODO probably better to get these from the tableManage instead?
    for (let x = 0; x < entries.length; x++) {
        let cells = entries[x].cells;
        let jsonData = {}

        jsonData[headers[0]] = startTime;
        jsonData[headers[1]] = cells[3].textContent;
        jsonData[headers[2]] = "Brain";
        jsonData[headers[3]] = cells[3].textContent;
        jsonData[headers[4]] = cells[3].textContent;

        arrayScheduleData.push(jsonData);
    };

    return arrayScheduleData;
}

function generateSchedulePreview(date, schedule) {
    console.log(schedule);
    tableManager.clearTable($("#scheduleList0"));
    tableManager.scheduleAdd(schedule, $("#scheduleList0"));
}

//Generate a table to hold a schedule in, there may be one or many depending on the number
//of locations
function generateTable() {
    //Main table object
    var table = $("<table>");
    table.addClass("table table-sm")

    var thead = $("<thead>");

    //Location header area
    var trLoc = $("<tr>");
    trLoc.addClass("text-center");

    var thLoc = $("<th>").attr({
        colspan: "5",
    });
    thLoc.addClass("text-center");
    thLoc.text("Location 2");

    trLoc.append(thLoc);

    //Main column area
    var trMain = $("<tr>");

    var thTime = $("<th>").attr({
        scope: "col"
    });
    thTime.text("Time");

    var thPatient = $("<th>").attr({
        scope: "col"
    });
    thPatient.text("Patient");

    var thSite = $("<th>").attr({
        scope: "col"
    });
    thSite.text("Site");

    var thRT1 = $("<th>").attr({
        scope: "col"
    });
    thRT1.text("RT1");

    var thRT2 = $("<th>").attr({
        scope: "col"
    });
    thRT2.text("RT2");

    //Main body area
    var body = $("<tbody>").attr({
        id: "list2"
    });

    trMain.append(thTime, thPatient, thSite, thRT1, thRT2);
    thead.append(trLoc, trMain);
    table.append(thead, body);

    return table;
}
