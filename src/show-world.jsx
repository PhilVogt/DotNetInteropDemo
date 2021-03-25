import React, { useEffect, useState } from 'react';
import { create } from 'openfin-notifications';

// Needed to fix "no-undef" linter error
/* global fin */

export function ShowWorld() {
    const [text, setText] = useState("");
    useEffect(() => {

        if (fin) {
            fin.InterApplicationBus.subscribe({ uuid: "*" }, 'messages', async payload => {
                let identity = fin.me.identity;
                if (identity.name !== payload.name) {
                    setText(`Hello ${payload.text}!`);
                    create({
                        title: `Hello ${payload.text}!`,
                        body: 'This is your first notification',
                        category: 'hello',
                    });
                }
            });
        }
    }, [setText]);

    return (
        <p>{text}</p >
    );
}