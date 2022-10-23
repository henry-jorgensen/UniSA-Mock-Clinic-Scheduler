class TableManager {
    constructor(list = null, firstNameInput = null, lastNameInput = null, studentIdInput = null, studentUserNameInput = null) {
        this.listNum = 0;
        this.list = list;
        this.fNInput = firstNameInput;
        this.lNInput = lastNameInput;
        this.sIInput = studentIdInput;
        this.sUInput = studentUserNameInput;
        this.sites = null;
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
                nameSplit[1].trim(),
                nameSplit[0],
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
        let studentArray = classObject.students;

        if (studentArray[0] == "") {
            return;
        }

        studentArray.forEach(student => {
            //let jStudent = JSON.parse(student);
            let row = this.createRow(
                student.firstName,
                student.lastName,
                student.studentId,
                student.username,
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
        });

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
     * Create a row for a new site on the settings page
     */
    createSiteRow = (name) => {
        let tr = $('<tr>', {
            class: "siteEntry"
        });

        let tdName = $('<td>', { text: name });

        let tdDelete = $('<td>');
        let button = $('<button>', { text: "X" })
        button.click((e) => {
            e.currentTarget.parentNode.parentNode.remove();
        });

        tdDelete.append(button);
        tr.append(tdName, tdDelete);

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
            if (student.infectious == null) {
                student.infectious = "False:False";
            }

            let row = this.createScheduleRow(
                student.Time,
                student.Patient,
                student.Infectious,
                student.Site,
                student.RT1,
                student.RT2,
                student.Complication
            );

            table.append(row);
        });
    }

    /**
     * Used to create a new row in the scheduling table. Using string literal and html create and 
     * return a new table row element with the supplied information as the filler details.
     */
    createScheduleRow = (time, patient, infectious, site, RT1, RT2, complication) => {
        let tr = $('<tr>', {
            class: "scheduleEntry"
        })

        let tdTime = $('<td>', { text: time });
        let tdPatient = $('<td>', { text: patient, contenteditable: "true" });

        let tdSite = $('<td>');
        let selectSite = $('<select>');

        this.sites.forEach(entry => {
            let option;

            if (entry === site) {
                option = $('<option>', { text: entry, selected: true });
            }
            else {
                option = $('<option>', { text: entry });
            }
            
            selectSite.append(option);
        });
        
        tdSite.append(selectSite)


        let tdRT1 = $('<td>', { text: RT1, contenteditable: "true" });
        let tdRT2 = $('<td>', { text: RT2, contenteditable: "true" });

        console.log(complication);

        if (complication == null) {
            complication = "No:False";
        } else {
            complication = `Yes:${complication}`;
        }

        let splitC = complication.split(":");
        let tdComplication = $('<td>', { text: splitC[0] });
        tdComplication.attr('data-value', splitC[1])
        tdComplication.on('click', function (e) {
            //Load the current values
            $("#modalComplicationDetails").val(e.target.getAttribute("data-value"));

            let modal = $("#complicationModal");
            let close = $("#closeComplicationButton");
            close.on('click', function () {
                modal.css("display", "none");
            });

            modal.css("display", "block");

            $("#saveComplicationDetails").on('click', function () {
                e.target.innerHTML = 'Yes';
                e.target.setAttribute('data-value', $("#modalComplicationDetails").val());
                modal.css("display", "none");

                //Reset the modal
                $("#modalComplicationDetails").val("");
            });
        });

        if (infectious == null) {
            infectious = "False:False";
        }
        let split = infectious.split(":");
        let tdInfectious = $('<td>', { text: split[0], class: "text-center" });
        tdInfectious.attr('data-value', split[1])
        tdInfectious.on('click', function (e) {
            //Load the current values
            $("#modalInfectionTitle").val(e.target.innerHTML)
            $("#modalinfectionDetails").val(e.target.getAttribute("data-value"));

            let modal = $("#infectionModal");
            let close = $("#closeButton");
            close.on('click', function () {
                modal.css("display", "none");
            });

            modal.css("display", "block");

            $("#saveInfectionDetails").on('click', function () {
                e.target.innerHTML = $("#modalInfectionTitle").val();
                e.target.setAttribute('data-value', $("#modalinfectionDetails").val());
                modal.css("display", "none");

                //Reset the modal
                $("#modalInfectionTitle").val("")
                $("#modalinfectionDetails").val("");
            });
        });

        tr.append(tdTime, tdPatient, tdInfectious, tdSite, tdRT1, tdRT2, tdComplication);

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

    /**
     * Generate a preview table of the schedules that have been created
     */
    generateSchedulePreview = (holder, date, locations, schedules) => {
        //Set the date
        $("#clinicDate").text(date);

        //Remove any old tables, generate new tables and append to the html page
        for (let i = 0; i < locations.length; i++) {
            let location = locations[i].trim();

            if ($(`#${location}`)) {
                $(`#table${location}`).remove()
            }

            let table = this.generateTable(location);
            holder.append(table);
            this.scheduleAdd(schedules[i], $(`#${location}`));
        }
    }

    generateScheduleEditor = (holder, date, data) => {
        //Set the date
        $("#clinicDate").text(date);

        //Remove any old tables, generate new tables and append to the html page
        for (let child in data) {
            console.log(data[child]);

            let location = child.trim();

            if ($(`#${location}`)) {
                $(`#table${location}`).remove()
            }

            let table = this.generateTable(location);
            holder.append(table);
            this.scheduleAdd(data[child], $(`#${location}`));
        }
    }

    /**
     * Generate a table to hold a schedule in, there may be one or many depending on the number 
     * of locations. Use the location as the id of where to add students
    */
    generateTable = (location) => {
        //Main table object
        var table = $("<table>").attr({
            id: `table${location}`
        });
        table.addClass("table table-sm table-hover text-center")

        var thead = $("<thead>");

        //Location header area
        var trLoc = $("<tr>");
        trLoc.addClass("text-center");

        var thLoc = $("<th>").attr({
            colspan: "12"
        });
        thLoc.addClass("text-center");
        thLoc.text(location);

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

        var thInfectious = $("<th>").attr({
            scope: "col"
        });
        thInfectious.text("Infectious");

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

        //What the patient has to act out
        var thComplication = $("<th>").attr({
            scope: "col"
        });
        thComplication.text("Complication");

        //Main body area
        var body = $("<tbody>").attr({
            id: location
        });
        body.addClass("text-center");

        trMain.append(thTime, thPatient, thInfectious, thSite, thRT1, thRT2, thComplication);
        thead.append(trLoc, trMain);
        table.append(thead, body);

        return table;
    }

}
