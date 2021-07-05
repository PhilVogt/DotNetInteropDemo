import React, { useEffect, useState } from 'react';
// Needed to fix "no-undef" linter error
/* global fin */
export function ExcelWorkbookManager() {
    var [excelConnected, setExcelConnected] = useState(false);
    var [workbooks, setworkbooks] = useState(null);

    useEffect(() => {
        console.log("Initialising excel connected/disconnected events");
        async function connectExcelEvents() {
            await fin.desktop.ExcelService.init();
            fin.desktop.ExcelService.addEventListener("excelConnected", onExcelConnected);
            fin.desktop.ExcelService.addEventListener("excelDisconnected", onExcelDisconnected);
        };

        connectExcelEvents();
    }, []);

    async function onExcelConnected(data) {
        console.log("Excel Connected: " + data.connectionUuid);
        let connected = await window.fin.desktop.Excel.getConnectionStatus();
        console.log("Connected: " + connected);
        setExcelConnected(true);
    };

    async function onExcelDisconnected(data) {
        console.log("Excel Disconnected: " + data.connectionUuid);
        setExcelConnected(false);
    };

    async function handleGetAllWorkbooksClick(ev) {
        var workbooks = await fin.desktop.Excel.getWorkbooks();
        setworkbooks(workbooks);
    };

    async function workbookSelected(ev) {

    }

    return (
        <>
            <input type="button" style={excelConnected ? { display: 'unset' } : { display: 'none' }} onClick={handleGetAllWorkbooksClick} value="Get Workbooks" />
            {workbooks && <select onChange={ev => workbookSelected(ev)}>{workbooks.map((workbook) => { return <option key={workbook.name}>{workbook.name}</option> })}</select>}
        </>);
}