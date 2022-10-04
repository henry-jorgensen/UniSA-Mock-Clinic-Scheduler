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
     * Perform a GET call to the parent controller, the objective is to retrieve details about
     * a previously saved class. A successful call will increment the form wizard to the
     * appropriate form.
     */
    deleteAClass = async (classNameValue, classCodeValue) => {
        return $.ajax({
            type: 'POST',
            url: `/${this.controller}/DeleteAClass`,
            data: {
                className: classNameValue,
                classCode: classCodeValue
            },
            success: function (result) {
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
                className: this.selectedClass,
                classCode: this.selectedClassCode,
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
    saveAClinic = async (classNameValue, scheduleNameValue, dateValue, startTimeValue, appointmentDurationValue, locationsValue, scheduleValue) => {

        let response = await $.ajax({
            type: 'POST',
            url: `/${this.controller}/SaveAClinic`,
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

    /**
     * Perform a GET call to the parent controller, the objective is to retrieve details about
     * a previously saved class. A successful call will increment the form wizard to the
     * appropriate form.
     */
    loadASchedule = async (scheduleValue) => {
        return $.ajax({
            type: 'GET',
            url: `/${this.controller}/LoadASchedule`,
            data: {
                scheduleId: scheduleValue
            },
            success: function (result) {
                //Populate the list with the results
                console.log(result);
                return result
            },
            failure: function (error) {
                console.log(error);
                return 0;
            }
        });
    };

    /**
     * Upload a PDF to firebase.
     */
    uploadPDF = async (formData) => {
        return await $.ajax({
            type: 'POST',
            url: `/${this.controller}/UploadPDF`,
            data: formData,
            processData: false,  // tell jQuery not to process the data
            contentType: false,  // tell jQuery not to set contentType
            success: function (result) {
                //console.log(result);
            },
            failure: function (error) {
                console.log(error);
            }
        });
    }

    /**
     * Retireve a PDF link from firebase.
     */
    retrievePDF = async (documentID) => {
        return await $.ajax({
            type: 'GET',
            url: `/${this.controller}/RetrievePDF`,
            data: {
                ID: documentID
            },
            success: function (result) {
                //console.log(result);
            },
            failure: function (error) {
                console.log(error);
            }
        });
    }

    /**
     * Delete a PDF link from firebase.
     */
    deletePDF = async (documentID) => {
        return await $.ajax({
            type: 'POST',
            url: `/${this.controller}/DeletePDF`,
            data: {
                ID: documentID
            },
            success: function (result) {
                console.log(result);
            },
            failure: function (error) {
                console.log(error);
            }
        });
    }
}
