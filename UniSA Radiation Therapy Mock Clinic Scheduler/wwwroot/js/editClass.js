//Create necessary classes - provided through loading the class scripts on the CreateClinic.html
const ajaxManager = new AJAXManager("Coordinator");
let tableManager = null;

$(document).ready(function () {
    tableManager = new TableManager(
        $("#list"),
        $("#firstNameInput"),
        $("#lastNameInput"),
        $("#studentIdInput"),
        $("#usernameInput")
    );

    //Detect if there are any URL queries to load - this means the users is coming from the 
    //View Clinics page to edit a class
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
    });

    //Prefill the inputs if editing a schedule
    if (params.name != null) {
        loadClassForEdit(params);
    }

    //Handle the multiple form submissions from the same page
    $("form").on('submit', async (e) => {
        e.preventDefault();

        $("#loader").css("display", "block");

        try {
            switch (e.currentTarget.id) {
                case "editAClassForm":
                    //Create a new class with the details
                    if ($("#saveAsNewInput").is(':checked')) {
                        //Create the class
                        await ajaxManager.createAClass(
                            $("#classNameInput").val(),
                            $("#studyPeriodInput").val(),
                            $("#semesterInput").val(),
                            $("#yearInput").val()
                        );

                        //Save the student list details
                        await ajaxManager.saveAClassList(
                            $(".studentEntry")
                        );
                    } else {
                        //Edit the class details
                        await ajaxManager.editAClass(
                            params.name,
                            $("#classNameInput").val(),
                            $("#studyPeriodInput").val(),
                            $("#semesterInput").val(),
                            $("#yearInput").val(),
                            params.code
                        );

                        //Edit the student list details
                        await ajaxManager.saveAClassList(
                            $(".studentEntry")
                        );
                    }

                default:
                    break;
            }
        }
        catch (error) {
            console.log(error);
        }

        $("#loader").css("display", "none");
        window.location.replace("/Coordinator/Classes");
    });
});

async function loadClassForEdit(params) {
    $("#classNameInput").val(params.name);
    $("#studyPeriodInput").val(params.studyPeriod);
    $("#semesterInput").val(params.semester);
    $("#yearInput").val(params.year);

    ajaxManager.selectedClassCode = params.code;

    response = await ajaxManager.loadAClass(
        params.name
    );

    //Clear the table from any previous entries
    tableManager.clearTable(tableManager.list);

    console.log(response);

    //The current class
    tableManager.classAdd(response, true);
}
