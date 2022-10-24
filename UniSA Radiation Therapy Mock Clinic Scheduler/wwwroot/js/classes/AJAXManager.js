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
     * Edit the values of an existing class in firebase.
     */
    editAClass = async (oldNameValue, classNameValue, studyPeriodValue, semesterValue, yearValue, classCodeValue) => {
        this.selectedClass = classNameValue;

        return await $.ajax({
            type: 'POST',
            url: `/${this.controller}/EditAClass`,
            data: {
                oldName: oldNameValue,
                name: classNameValue,
                studyPeriod: studyPeriodValue,
                semester: semesterValue,
                year: yearValue,
                code: classCodeValue
            },
            success: function (result) {
                console.log(result); //Keep as log for now for testing
                return result;
            },
            failure: function (result) {
                console.log(result);
                return null;
            }
        });
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
     * Load all the clinics associated with a course coordinator
     */
    loadAllClinics = async (classNameValue) => {
        return await $.ajax({
            type: 'POST',
            url: `/${this.controller}/LoadAllClinics`,
            data: {
                className: classNameValue
            },
            success: function (result) {
                console.log(result);
                return result;
            },
            failure: function (error) {
                console.log(error);
                return error;
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
     * 
     */
    deleteAClass = async (classNameValue) => {
        return await $.ajax({
            type: 'POST',
            url: `/${this.controller}/DeleteAClass`,
            data: {
                className: classNameValue
            },
            success: function (result) {
                return result
            },
            failure: function (error) {
                console.log(error);
                return false;
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

        return await $.ajax({
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
                console.log(result);
                return true;
            },
            failure: function (result) {
                console.log(result);
                return null;
            }
        });
    };

    /**
     * Update a saved clinic with new inputs.
     * @param {any} scheduleCode
     * @param {any} scheduleNameValue
     * @param {any} dateValue
     * @param {any} startTimeValue
     * @param {any} appointmentDurationValue
     * @param {any} locationsValue
     * @param {any} scheduleValue
    */
    editAClinic = async (scheduleCode, scheduleNameValue, dateValue, startTimeValue, appointmentDurationValue, locationsValue, scheduleValue) => {
        return await $.ajax({
            type: 'POST',
            url: `/${this.controller}/EditAClinic`,
            data: {
                code: scheduleCode,
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
    }

    /**
     * Update the details of appointments that have been used during a Clinic Day.
     * This may update the time or status of an appointment.
     * @param {any} details
     */
    updateAppointments = async (details) => {
        return $.ajax({
            type: 'POST',
            url: `/${this.controller}/UpdateAppointments`,
            data: {
                appointmentDetails: details
            },
            success: function (result) {
                //Populate the list with the results
                return result
            },
            failure: function (error) {
                console.log(error);
                return error;
            }
        });
    }

    /**
     * Load all the appointments associated with a particular schedule
     * @param {any} scheduleCode
     */
    loadScheduleAppointments = async (scheduleCode) => {
        return $.ajax({
            type: 'GET',
            url: `/${this.controller}/LoadScheduleAppointments`,
            data: {
                scheduleId: scheduleCode
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
    }

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
     * Delete a selected schedule, this removes any associated appointments and student 
     * links within their save models.
     */
    deleteASchedule = async (scheduleCodeValue, classNameValue) => {
        return $.ajax({
            type: 'POST',
            url: `/${this.controller}/DeleteASchedule`,
            data: {
                scheduleCode: scheduleCodeValue,
                className: classNameValue
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
     * Send an email to a single patient detailing what complication to act out.
     * @param {any} scheduleCode
     * @param {any} patientName
     */
    emailPatient = async (dateValue, usernameValue, nameValue, complicationValue, appointmentRefValue) => {
        return $.ajax({
            type: 'POST',
            url: `/${this.controller}/EmailAPatient`,
            data: {
                date: dateValue,
                username: usernameValue,
                name: nameValue,
                complication: complicationValue,
                appointmentRef: appointmentRefValue
            },
            success: function (result) {
                return result
            },
            failure: function (error) {
                console.log(error);
                return error;
            }
        });
    }

    /**
     * Send an email to all patients within a schedule detailing what complication to act out.
     * @param {any} patientName
     * @param {any} scheduleCode
     */
    emailAllPatients = async (patientListValue, scheduleCodeValue) => {
        return $.ajax({
            type: 'POST',
            url: `/${this.controller}/EmailAllPatients`,
            data: {
                patientList: patientListValue,
                scheduleCode: scheduleCodeValue
            },
            success: function (result) {
                return result
            },
            failure: function (error) {
                console.log(error);
                return error;
            }
        });
    }

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
                return result;
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


    /**
     * Collect the saved list of sites available for the clinic 
     */
    collectSites = async () => {
        return await $.ajax({
            type: 'GET',
            url: `/${this.controller}/CollectSites`,
            data: {},
            success: function (result) {
               return result;
            },
            failure: function (error) {
                console.log(error);
            }
        });
    }

    /**
     * Update the list of sites saved within the database.
     * @param {any} newlist A list of strings which represent the sites.
     */
    updateSites = async (newlist) => {
        return await $.ajax({
            type: 'POST',
            url: `/${this.controller}/UpdateSites`,
            data: {
                sites: newlist
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
