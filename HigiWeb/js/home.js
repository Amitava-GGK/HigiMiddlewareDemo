(function () {
    
    jQuery(function ($) {

        $("#btn-clear-messages").click(function () {
            $("#message-list").empty();
        });

        var processHub = $.connection.processHub;

        processHub.client.messageAddedToQueue = function (messageID, region, isJobQueued) {
            console.log("Message added to queue: ", messageID, "Job Queued: ", isJobQueued);
            addMessageInList(messageID, region, isJobQueued);
        };

        processHub.client.jobCompleted = function (jobID, jobStatus, user) {
            console.log("Job Completed.", jobID, jobStatus, user);
            updateJobStatus(jobID, jobStatus, user);
        };

        $.connection.hub.start().done(function () {

                $("#fetch-user-form").submit(function (event) {

                    var region = $("#region-field").val() || null;

                    processHub.server.send(region);

                    event.preventDefault();
                    return false;
                });

        });

    });

}());

function addMessageInList(messageID, region, isJobQueued) {

    var messageCard = `
        <div style="margin-top: 12px;">
            <div class ="panel panel-default" data-job-id="${ messageID }">
                <div class ="panel-body">
                    MessageId: ${ messageID } <br>
                    Status: <span class="job-status">${ isJobQueued ? "Queued" : "Failed to queue" }</span> <br>
                    Query: <code>{ "region": ${ region} }</code> <br>
                    Result: <code class="job-result"></code>
                </div>
            </div>
        </div>
   `;

    $("#message-list").append(messageCard);

}

function updateJobStatus(jobId, status, user) {

    $(`[data-job-id="${jobId}"]`).find(".job-status").text(status);
    $(`[data-job-id="${jobId}"]`).find(".job-result").text(user);

}