//Create necessary classes - provided through loading the class scripts on the CreateClinic.html
const ajaxManager = new AJAXManager("Coordinator");
const tableManager = new TableManager();

$(document).ready(async function () {
    //Append the saved values
    let sites = await ajaxManager.collectSites();
    console.log(sites);
    sites.forEach(entry => {
        $("#siteList").append(tableManager.createSiteRow(entry));
    });

    //Save the new site lists to the database
    let saveButton = $("#saveButton");
    saveButton.on('click', async () => {
        let updatedSites = [];
        let entries = [].slice.call($(".siteEntry"));

        entries.forEach(entry => {
            updatedSites.push(entry.children[0].innerHTML);
        });

        await ajaxManager.updateSites(updatedSites.toString());
    });


    //Handle the multiple form submissions from the same page
    $("form").on('submit', async (e) => {
        e.preventDefault();

        try {
            switch (e.currentTarget.id) {
                case "addSiteForm":
                    $("#siteList").append(tableManager.createSiteRow($("#siteInput").val()));
                    $("#siteInput").val("");
                    break;

                default:
                    break;
            }
        }
        catch (error) {
            console.log(error);
        }
    });
});