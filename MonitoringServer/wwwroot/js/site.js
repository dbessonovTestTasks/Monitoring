$(function() {
    var connection = new signalR.HubConnectionBuilder().withUrl("/monitoringHub").build();
    var signalRStore = new DevExpress.data.ArrayStore({
        key: "recordId",
    });

    connection.on("ReceiveInfo",
        function (data) {
            $.each(data.hddInfo,
                function () {
                    var formatedData = {
                        recordId: data.clientIp + this.hddName,
                        clientIp: data.clientIp,
                        totalRAM: data.totalRAM,
                        freeRAM: data.freeRAM,
                        cpuLoad: data.cpuLoad,
                        hddName: this.hddName,
                        hddTotalSpace: this.hddTotalSpace,
                        hddFreeSpace: this.hddFreeSpace
                    };

                    signalRStore.byKey(formatedData.recordId).done(function (dataItem) {
                            signalRStore.push([{ type: "update", key: formatedData.recordId, data: formatedData }]);
                        })
                        .fail(function (error) {
                            signalRStore.push([{ type: "insert", data: formatedData }]);
                        });
                });
        });

    connection.start({ waitForPageLoad: false }).then(function() {
        connection.invoke("AddToGroup", "webClients");

        $("#gridContainer").dxDataGrid({
            dataSource: {
                store: signalRStore,
                reshapeOnPush: true
            },
            repaintChangesOnly: true,
            columnAutoWidth: true,
            showBorders: true,
            highlightChanges: true,
            columns: [
                { dataField: "clientIp" },
                { dataField: "totalRAM" },
                { dataField: "freeRAM" },
                { dataField: "cpuLoad" },
                { dataField: "hddName" },
                { dataField: "hddTotalSpace" },
                { dataField: "hddFreeSpace" }
            ]
        });
    });

    var ignoreChange = false;
    var handlerSlider = $("#frequency-slider").dxSlider({
        min: 10,
        max: 100,
        step: 10,
        value: 10,
        onValueChanged: function (e) {
            if (!ignoreChange)
                connection.invoke("SendInterval", e.value).catch(function(err) {
                    return console.error(err.toString());
                });
        },
        tooltip: {
            enabled: true,
            showMode: "always",
            format: "Обновление каждые #0 секунд",
            position: "top"
        }
    }).dxSlider("instance");

    connection.on("RefreshTimeChanged",
        function (data) {
            ignoreChange = true;
            handlerSlider.option("value", data);
            ignoreChange = false;
        });
});
