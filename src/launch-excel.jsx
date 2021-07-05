import React from 'react';
// Needed to fix "no-undef" linter error
/* global fin */
export function LaunchExcel() {

    let handleOpenExcelAppAssetClick = (ev) => {
        fin.System.launchExternalProcess({
            alias: 'TemplateExcelExample',
            listener: (result) => {
                console.log('the exit code', result.exitCode);
            }
        }).then(processIdentity => {
            console.log(processIdentity);
        }).catch(error => {
            console.log(error);
        });
    }

    return <input type="button" onClick={handleOpenExcelAppAssetClick} value="Launch Excel Template" />;
}