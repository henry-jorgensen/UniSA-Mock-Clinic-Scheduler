$( document ).ready(function() {
    //==========================================================
    //AJAX QUERY SECTION
    //==========================================================
    //Stop all default behaviour for forms and implement custom
    $("form").on('submit', function (e) {
        e.preventDefault();
        console.log(e.currentTarget.id);

        switch (e.currentTarget.id) {
            case "createAClassForm":
                createAClass();
                break;

            case "loadAClassForm":
                loadAClass();
                break;

            case "addStudentToClassForm":
                addList();
                break;

            case "saveClassListForm":
                saveAClassList();
                break;

            default:
                break;
        }
    });

    /**
     * Collected the form inputs for the new class and then POST them to the parent controller
     * using the AJAX post method. A successful call will increment the form wizard to the
     * appropriate form.
     */
    createAClass = () => {
        $.ajax({
            type: 'POST',
            url: '/Home/CreateAClass',
            data: {
                name: $("#classNameInput").val(),
                studyPeriod: $("#studyPeriodInput").val(),
                semester: $("#semesterInput").val(),
                year: $("#yearInput").val()
            },
            success: function (result) {
                console.log(result); //Keep as log for now for testing
                nextPrev(2); //2 to skip over the load class form
            },
            failure: function (result) {
                console.log(result);
            }
        });
    };

    /**
     * Perform a GET call to the parent controller, the objective is to retrieve details about
     * a previously saved class. A successful call will increment the form wizard to the
     * appropriate form.
     */
    loadAClass = () => {
        $.ajax({
            type: 'GET',
            url: '/Home/LoadAClass',
            data: { 
                className: $("#classSelection").val() 
            },
            success: function (result) {
                console.log(result);
                //Populate the list with the results
                nextPrev(1);
            },
            failure: function (result) {
                console.log(result);
            }
        });
    };

    /**
     * After a class list has been either manually entered or automatically loaded perform a
     * POST call to the parent controller. The information passed is an array of student objects
     * that reflects what was in the table.
     */
    saveAClassList = () => {
        let entries = $(".studentEntry");
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

        // $.ajax({
        //     type: 'POST',
        //     url: '/Home/SaveAClass',
        //     data: { 
        //         students: "array"
        //      },
        //     success: function (result) {
        //         alert(result);
        //     },
        //     failure: function (result) {
        //         console.log(result);
        //     }
        // });
    };
    //==========================================================
    //END AJAX QUERY SECTION
    //==========================================================

    //==========================================================
    //FORM WIZARD SECTION
    //==========================================================
    var currentTab = 3; // Current tab is set to be the first tab (0)
    
    /**
     * Change the display property of the appropriate tab in regards to the current
     * tab number.
     */
    showTab = (n) => {
        // This function will display the specified tab of the form ...
        var x = $(".tab");
        x[n].style.display = "block";

        $(".step")[currentTab].classList += " finish";

        //Sort out the next/previous buttons
        n == 0 ? $("#prevBtn").css("display", "none") : $("#prevBtn").css("display", "inline");

        fixStepIndicator(n)
    }

    /**
     * Change the visible form tab by incrementing or decrementing the current tab
     * value. If moving backwards, undo the 'finished' effect placed on the form's
     * step.
     */
    nextPrev = (n) => {
        var x = $(".tab");
        x[currentTab].style.display = "none"; // Hide the current tab

        if(n < 0) {
            $(".step")[currentTab].classList.remove("finish");
        }

        currentTab = currentTab + n;
        
        if (currentTab >= x.length) { // if you have reached the end of the form
            //...the form gets submitted:
            // document.getElementById("regForm").submit();
            return false;
        }
        
        showTab(currentTab); // Otherwise, display the correct tab
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

    showTab(currentTab); // Display the current tab
    //==========================================================
    //END FORM WIZARD SECTION
    //==========================================================

    //==========================================================
    //CLASS LIST SECTION
    //==========================================================
    /**
     * Keep track of the current list row being created.
     */
    let listNum = 0;

    /**
     * Collect the inputs from the class list form, using the data create a new table
     * row that reflects the data and append it to the main table.
     */
    addList = () => {
        let row = createRow(
            $("#firstNameInput").val(),
            $("#lastNameInput").val(),
            $("#studentIdInput").val(),
            $("#usernameInput").val()
        );
   
        $("#list").append(row);
        
        resetInputs();
        listNum++;
    }

    /**
     * Using string literal and html create and return a new table row element with the supplied
     * information as the filler details.
     */
    createRow = (fname, lname, id, uname) => {
        return (
            `<tr class="studentEntry" id="text${listNum}">
                <td>${fname}</td>
                <td>${lname}</td>
                <td>${id}</td>
                <td>${uname}</td>
                <td><button onclick="removeListItem(${listNum})">X</button></td>
            </tr>`
        );
    }

    /**
     * After a new entry has been appended clear the inputs for the next entry.
     */
    resetInputs = () => {
        $("#firstNameInput").val("");
        $("#lastNameInput").val("");
        $("#studentIdInput").val("");
        $("#usernameInput").val("");
    }

    /**
     * Remove a previously appended table row.
     */
    removeListItem = (listId) => {
        $(`#text${listId}`).remove();
    };

    //==========================================================
    //END CLASS LIST SECTION
    //==========================================================

    //==========================================================
    //EXCEL READER SECTION
    //==========================================================
    /**
     * Using the data read from an uploaded excel spreadsheet create a new table
     * row that reflects the data and append it to the main table.
     */
    autoAdd = (object) => {
        object.forEach(row => {
            let nameSplit = row["Student Name"].split(",");
            let newRow = createRow(
                nameSplit[0],
                nameSplit[1].trim(),
                row["Student ID"],
                row["Student Username"]
            );

            $("#list").append(newRow);
        });
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
                    autoAdd(JSON.parse(json_object));
                    //jQuery('#xlx_json').val(json_object);
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
    //==========================================================
    //END EXCEL READER SECTION
    //==========================================================
});
