$(document).ready(async function () {
    //Detect if there are any URL queries to load - this means the users is coming from the 
    //View Clinics page to edit a class
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    });

    //Create necessary classes
    const tableManager = new TableManager(
        $("#list"),
        $("#firstNameInput"),
        $("#lastNameInput"),
        $("#studentIdInput"),
        $("#usernameInput")    
    );

    const ajaxManager = new AJAXManager("Coordinator");
    const formWizard = params.name == null ? new FormWizard(0) : new FormWizard(3);

    if (params.name != null) {
        ajaxManager.selectedClassCode = params.code;

        response = await ajaxManager.loadAClass(
            params.name
        );

        //Clear the table from any previous entries
        tableManager.clearTable(tableManager.list);

        //The current class
        tableManager.classAdd(response, true);
    }

    formWizard.showInitialTab(); // Display the first tab

    //Assign click listeners - Setup the steps
    $("#loadClassButton").on('click', async () => {
        let result = await ajaxManager.loadAllClasses(); //reference to the dropdown menu

        if(result == -1) {
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
        let response;

        $("#loader").css("display", "block");

        try {
            switch (e.currentTarget.id) {
                case "createAClassForm":
                    response = await ajaxManager.createAClass(
                        $("#classNameInput").val(),
                        $("#studyPeriodInput").val(),
                        $("#semesterInput").val(),
                        $("#yearInput").val()
                    );

                    if(response == null) {
                        return;
                    }

                    performStep(2);
                    break;

                case "loadAClassForm":
                    response = await ajaxManager.loadAClass(
                        $("#classSelection").val()
                    );
                    
                    //Clear the table from any previous entries
                    tableManager.clearTable(tableManager.list);
                    
                    //The current class
                    tableManager.classAdd(response, true);

                    if(response == null) {
                        return;
                    }

                    performStep(1);
                    break;

                case "addStudentToClassForm":
                    response = await tableManager.addList();
                    break;

                case "saveClassListForm":
                    response = await ajaxManager.saveAClassList(
                        $(".studentEntry")
                    );

                    if(response == null) {
                        return;
                    }

                    tableManager.clearTable($("#previewList"));
                    tableManager.populatePreview($(".studentEntry"), $("#previewList"));
                    performStep(1);
                    break;

                default:
                    break;
            }
        }
        catch (error) {
            console.log(error);
        }

        $("#loader").css("display", "none");
    });

    performStep = (stepNum) => {
        if(stepNum != 0 && stepNum != null) {
            formWizard.nextPrev(stepNum);
        }
    }

    //==========================================================
    //EXCEL UPLOAD SECTION
    //==========================================================
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
