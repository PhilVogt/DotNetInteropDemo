import React, { useState } from 'react';

// Needed to fix "no-undef" linter error
/* global fin */

export function SayHello() {
    const [text, setText] = useState("");

    let handleSubmit = async (evt) => {
        evt.preventDefault();

        if (fin) {
            let view = fin.View.getCurrentSync();
            await fin.InterApplicationBus.publish('messages', { name: view.identity.name, text });
            console.log("Message sent!");
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <label>
                Name:
                <input type="text" value={text} onChange={(evt) => setText(evt.currentTarget.value)} />
            </label>
            <input type="submit" value="Submit" />
        </form>
    );
}