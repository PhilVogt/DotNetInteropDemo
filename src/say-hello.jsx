import React, { useState } from 'react';
import { LaunchExcel } from './launch-excel'
import { ExcelWorkbookManager } from './excel-workbook-manager'

// Needed to fix "no-undef" linter error
/* global fin */

export function SayHello() {
    const [text, setText] = useState("");

    let handleSubmit = async (evt) => {
        evt.preventDefault();

        if (fin) {
            await fin.InterApplicationBus.publish('messages', { name: fin.me.identity.name, text });
            console.log("Message sent!");


        }
    };

    let handleOpenAppAssetClick = (evt) => {
        fin.System.launchExternalProcess({
            alias: 'ChannelApiDemo',
            listener: (result) => {
                console.log('the exit code', result.exitCode);
            }
        }).then(processIdentity => {
            console.log(processIdentity);
        }).catch(error => {
            console.log(error);
        });
    }

    return (
        <>
            <form onSubmit={handleSubmit}>
                <label>
                    Name:
                <input type="text" value={text} onChange={(evt) => setText(evt.currentTarget.value)} />
                </label>
                <input type="submit" value="Send Message" />
            </form>
            <input type="button" onClick={handleOpenAppAssetClick} value="Open App Asset" />
            <LaunchExcel />
            <ExcelWorkbookManager />
        </>
    );
}