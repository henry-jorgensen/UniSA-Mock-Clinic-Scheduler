﻿class TableManager {
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

        let tdTime = $('<td>', { text: time, contenteditable: "true" });
        let tdPatient = $('<td>', { text: patient, contenteditable: "true" });
        let tdSite = $('<td>', { text: site, contenteditable: "true" });
        let tdRT1 = $('<td>', { text: RT1, contenteditable: "true" });
        let tdRT2 = $('<td>', { text: RT2, contenteditable: "true" });

        let tdInfectious = $('<td>', { class: "text-center" });
        let checkbox = $('<input>', { type: "checkbox" });
        tdInfectious.append(checkbox);

        tr.append(tdTime, tdPatient, tdInfectious, tdSite, tdRT1, tdRT2);

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

        //Main body area
        var body = $("<tbody>").attr({
            id: location
        });
        body.addClass("text-center");

        trMain.append(thTime, thPatient, thInfectious, thSite, thRT1, thRT2);
        thead.append(trLoc, trMain);
        table.append(thead, body);

        return table;
    }

}