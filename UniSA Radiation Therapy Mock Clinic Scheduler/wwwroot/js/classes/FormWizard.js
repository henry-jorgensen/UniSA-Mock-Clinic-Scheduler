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

        if (n < 0) {
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
