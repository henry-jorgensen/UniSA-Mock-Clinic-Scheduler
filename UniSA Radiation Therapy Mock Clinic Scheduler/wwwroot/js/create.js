class TableManager {
    constructor(list, firstNameInput, lastNameInput, studentIdInput, studentUserNameInput) {
        this.listNum = 0;
        this.list = list;
        this.fNInput = firstNameInput;
        this.lNInput = lastNameInput;
        this.sIInput = studentIdInput;
        this.sUInput = studentUserNameInput;
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
            this.sUInput.val()
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
                row["Student Username"]
            );

            this.list.append(newRow);
        });
    }

    /**
     * Using string literal and html create and return a new table row element with the supplied
     * information as the filler details.
     */
    createRow = (fname, lname, id, uname) => {
        let tr = $('<tr>', {
            class: "studentEntry"
        })

        let tdFName = $('<td>', { text: fname });
        let tdLName = $('<td>', { text: lname });
        let tdId = $('<td>', { text: id });
        let tdUName = $('<td>', { text: uname });

        //Create delete button
        let tdDelete = $('<td>');
        let button = $('<button>', {id: `text${this.listNum}`, text: "X"})
        button.click((id) => {
            id.currentTarget.parentNode.parentNode.remove();
        });

        tdDelete.append(button);

        tr.append(tdFName, tdLName, tdId, tdUName, tdDelete);
        
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
}

class AJAXManager {
    constructor(controller) {
        this.controller = controller;
    }

    /**
     * Collected the form inputs for the new class and then POST them to the parent controller
     * using the AJAX post method. A successful call will increment the form wizard to the
     * appropriate form.
     */
    createAClass = async (classNameValue, studyPeriodValue, semesterValue, yearValue) => {
        return await $.ajax({
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

                return 2; //2 to skip over the load class form
            },
            failure: function (result) {
                console.log(result);
                return 0;
            }
        });
    };

    /**
     * Load all the classes associated with the current user.
     */
    loadAllClasses = () => {
        $.ajax({
            type: 'GET',
            url: `/${this.controller}/LoadAllClasses`,
            data: {},
            success: function (result) {
                console.log(result);
                //Populate the list with the results
                return 1
            },
            failure: function (result) {
                console.log(result);
                return 0;
            }
        });
    }

    /**
     * Perform a GET call to the parent controller, the objective is to retrieve details about
     * a previously saved class. A successful call will increment the form wizard to the
     * appropriate form.
     */
    loadAClass = async (classNameValue) => {
        return $.ajax({
            type: 'GET',
            url: `/${this.controller}/LoadAClass`,
            data: { 
                className: classNameValue 
            },
            success: function (result) {
                console.log(result);
                //Populate the list with the results
                return 1
            },
            failure: function (result) {
                console.log(result);
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
        let students = [];

        for(let x=0; x<entries.length; x++) {
            let cells = entries[x].cells;

            let student = {
                firstName: cells[0].value,
                lastName: cells[1].value,
                studentId: cells[2].value,
                username: cells[3].value
            }

            students.push(student);
        };

        console.log(students);

        // return $.ajax({
        //     type: 'POST',
        //     url: '/Home/SaveAClass',
        //     data: { 
        //         students: "array"
        //      },
        //     success: function (result) {
        //         return 1;
        //     },
        //     failure: function (result) {
        //         console.log(result);
        //         return 0;
        //     }
        // });
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

        if(n < 0) {
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

$( document ).ready(function() {
    //Create necessary classes
    const tableManager = new TableManager(
        $("#list"),
        $("#firstNameInput"),
        $("#lastNameInput"),
        $("#studentIdInput"),
        $("#usernameInput")    
    );

    const ajaxManager = new AJAXManager("Home");
    const formWizard = new FormWizard(0); 

    formWizard.showInitialTab(); // Display the first tab

    //Assign click listeners - Setup the steps
    $("#loadClassButton").on('click', () => {
        ajaxManager.loadAllClasses();
        formWizard.nextPrev(2)
    });

    $("#newClassButton").on('click', () => formWizard.nextPrev(1));
    $("#prevBtn").on('click', () => formWizard.nextPrev(-1));
    $("#nextBtn").on('click', () => formWizard.nextPrev(1));
    
    //==========================================================
    //AJAX QUERY SECTION
    //==========================================================
    //TODO CHANGE THIS TO SOMETHING BETTER
    //Stop all default behaviour for forms and implement custom
    $("form").on('submit', async (e) => {
        e.preventDefault();
        console.log(e.currentTarget.id);
        let step;

        switch (e.currentTarget.id) {
            case "createAClassForm":
                step = await ajaxManager.createAClass(
                    $("#classNameInput").val(),
                    $("#studyPeriodInput").val(),
                    $("#semesterInput").val(),
                    $("#yearInput").val()
                );

                console.log(step);
                performStep(step);
                break;

            case "loadAClassForm":
                step = await ajaxManager.loadAClass(
                    $("#classSelection").val()
                );
                break;

            case "addStudentToClassForm":
                step = await tableManager.addList();
                break;

            case "saveClassListForm":
                step = await ajaxManager.saveAClassList(
                    $(".studentEntry")
                );
                break;

            default:
                break;
        }
    });

    performStep = (stepNum) => {
        if(stepNum != 0 && stepNum != null) {
            formWizard.nextPrev(stepNum);
        }
    }

    /**
     * Instantiate a new reader object to read the newly uploaded excel spreadsheet.
     */
    let ExcelToJSON = function() {
        this.parseExcel = function(file) {
            let reader = new FileReader();
    
            reader.onload = function(e) {
                let data = e.target.result;
                let workbook = XLSX.read(data, {
                    type: 'binary'
                });

                workbook.SheetNames.forEach(function(sheetName) {
                    // Here is your object
                    let XL_row_object = XLSX.utils.sheet_to_row_object_array(workbook.Sheets[sheetName]);
                    let json_object = JSON.stringify(XL_row_object);
                    tableManager.autoAdd(JSON.parse(json_object));
                });
            };
    
            reader.onerror = function(ex) {
                console.log(ex);
            };
    
            reader.readAsBinaryString(file);
        };
    };

    /**
     * A listener function that is attached to the upload's input element. Responsible
     * for selecting the file from the event and parsing it to a new ExcelToJSON object.
     */
    handleFileSelect = (evt) => {
        let files = evt.target.files; // FileList object
        let xl2json = new ExcelToJSON();
        xl2json.parseExcel(files[0]);
    }

    /**
     * Add the event listener to the DOM
     */
    $("#upload").change(handleFileSelect);
});
