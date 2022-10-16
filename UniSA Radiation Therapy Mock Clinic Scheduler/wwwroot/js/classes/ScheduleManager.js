class ScheduleManager {
    constructor() {
        this.locations = null;
        this.headers = ["Time", "Patient", "Infectious", "Site", "RT1", "RT2", "Complication"];
    }

    /**
     * Create an optimised schedule for the individuals that have been entered into a table
     */
    optimiseSchedule = (startTime, duration, entries) => {
        let arrayScheduleData = [];
        let entriesArray = [].slice.call(entries); //turn the HTMLCollection into an array

        //Assign patients
        for (let x = 0; x < entriesArray.length; x++) {
            let cells = entriesArray[x].cells;
            let jsonData = {}

            jsonData[this.headers[0]] = this.calculateTimeDifference(startTime, x, duration);;
            jsonData[this.headers[1]] = cells[3].textContent; //cells[3].textContent is a students username
            jsonData[this.headers[3]] = "TBA";

            arrayScheduleData.push(jsonData);
        };

        entriesArray = this.shiftArray(entriesArray, 2);

        //Assign rt1
        for (let x = 0; x < entriesArray.length; x++) {
            let cells = entriesArray[x].cells;

            let jsonData = arrayScheduleData[x]

            jsonData[this.headers[4]] = cells[3].textContent;
        };

        entriesArray = this.shiftArray(entriesArray, 4);

        //Assign rt2
        for (let x = 0; x < entriesArray.length; x++) {
            let cells = entriesArray[x].cells;
            let jsonData = arrayScheduleData[x]

            jsonData[this.headers[5]] = cells[3].textContent;
        };

        return arrayScheduleData;
    }

    /**
     * Split an array into equal chunks
     */
    splitToChunks = (array, parts) => {
        let result = [];
        for (let i = parts; i > 0; i--) {
            result.push(array.splice(0, Math.ceil(array.length / i)));
        }
        return result;
    }

    /**
     * Calculate the time gap between different
     */
    calculateTimeDifference = (start, step, duration) => {
        var time = moment(start, 'HH:mm');
        time.add(step * duration, 'm');
        return time.format("HH:mm");
    }

    /**
     * Move the first x (step) number of entries to the end of the supplied array
     */
    shiftArray = (arr, step) => {
        for (let x = 0; x < step; x++) {
            arr.push(arr.shift());
        }

        return arr;
    }

    /**
     * Generate the required schedules
     */
    generateSchedule = () => {
        let schedules = [];

        //Determine how many locations there are
        this.locations = $("#locationInput").val().split(",");

        //For each location split the students?
        let studentArray = [].slice.call($(".studentEntry"));
        let chunks = this.splitToChunks(studentArray, this.locations.length);

        //Create a schedule for each chunk of students
        for (let i = 0; i < chunks.length; i++) {
            //Optimise the schedule
            let schedule = this.optimiseSchedule(
                $("#scheduleStartTimeInput").val(),
                $("#scheduleDurationInput").val(),
                chunks[i]
            );

            schedules.push(schedule);
        }

        return [schedules, this.locations];
    }

    /**
     * Create a JSON data structure holding the details of the finalised schedule.
     
        SAMPLE SCHEDULE STRUCTURE
        {
            locationA: [
                {Time: xx:yy, Patient: string, Infectious: bool, RT1: string, RT2: string},
                {Time: xx:yy, Patient: string, Infectious: bool, RT1: string, RT2: string},
            ],
            locationB: [
                {Time: xx:yy, Patient: string, Infectious: bool, RT1: string, RT2: string},
                {Time: xx:yy, Patient: string, Infectious: bool, RT1: string, RT2: string},
            ],
        }
    */
    generateScheduleJSON = () => {
        //Collect the tables that have been generated
        let tables = [];
        for (let i = 0; i < this.locations.length; i++) {
            let location = this.locations[i].trim()
            tables.push($(`#${location}`));
        }

        let JSONdata = {};

        //From the collected tables each child element's rows
        for (let i = 0; i < tables.length; i++) {
            //Collect the rows within the table
            let rows = []

            for (let y = 0; y < tables[i][0].children.length; y++) {
                //Generate a JSON of each row
                let row = tables[i][0].children[y].children;
                let rowData = {};

                rowData[this.headers[0]] = row[0].innerHTML;
                rowData[this.headers[1]] = row[1].innerHTML;
                rowData[this.headers[2]] = row[2].innerHTML;
                rowData[this.headers[3]] = row[3].innerHTML;
                rowData[this.headers[4]] = row[4].innerHTML;
                rowData[this.headers[5]] = row[5].innerHTML;
                rowData[this.headers[6]] = row[6].innerHTML;

                rows.push(rowData)
            }

            //Place the array of rows under the location key
            JSONdata[this.locations[i].trim()] = rows;
        }

        return JSONdata;
    }
}
