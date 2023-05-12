var reports = window.reports;
var embedToken = window.embedToken;

var models = window['powerbi-client'].models;


// Embed a report
var embedReport = (report) => {
    try {
    // Get a reference to the embed container
    var embedContainer = document.getElementById('embed-container');
    // Create the report embed config object
    var config = {
        type: 'report',
        id: report.Id,
        embedUrl: report.EmbedUrl,
        accessToken: embedToken,
        tokenType: models.TokenType.Embed,
        settings: {
            localeSettings: {
                language: "et-ee"
            }
        }
       

    };
    // Embed the report
    var embeddedReport = powerbi.embed(embedContainer, config);
    
 
    
    } catch (error) {
        console.log(error)
    }
}




  // Add event listener when the DOM is loaded
  document.addEventListener('DOMContentLoaded', function() {
    // Get all the button elements by their class name
    var embedButtons = document.querySelectorAll('.embed-button');

    // Loop through the button elements and add a click event listener to each one
    embedButtons.forEach(function(embedButton) {
      embedButton.addEventListener('click', function() {
        const parent = document.getElementById("container2");
        if (parent.hasChildNodes()) {
            parent.removeChild(parent.firstChild);
        }
        document.getElementById("row").style.visibility = "visible";
        // create the child element
        const child = document.createElement("div");
        child.setAttribute("id", "embed-container");
        // append the child element to the parent
        parent.appendChild(child);
        var reports = window.reports;
        var i = 0;
        const reportName = embedButton.dataset.reportName;
        for (i = 0; i < reports.length; i++) {
            if (reports[i].Name == reportName) {
    
                break;
            }
    
        }
        report = window.reports[i];
        try {
            embedReport(report);
        } catch (error) {
            console.log(error)
        }
      });
    });
  });