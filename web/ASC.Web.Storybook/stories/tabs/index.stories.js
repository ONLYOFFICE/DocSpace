import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, boolean, text, select } from '@storybook/addon-knobs/react';
import { action } from '@storybook/addon-actions';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Tabs, Text } from 'asc-web-components';
import Section from '../../.storybook/decorators/section';
import { BooleanValue } from 'react-values';

//<Text.Body tag="span">{text("Body text", "Try again later")}</Text.Body>


const something_items = [
    {
        id: "0",
        something_title: <Text.Body>{text("Title text", "Title1")} </Text.Body>,
        something_body:
            <div >
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
            </div>
    },
    {
        id: "1",
        something_title: <Text.Body>{text("Title text2", "Title2")} </Text.Body>,
        something_body:
            <div >
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
            </div>
    },
    {
        id: "2",
        something_title: <Text.Body>{text("Title text3", "Title3")} </Text.Body>,
        something_body:
            <div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
            </div>
    },
    {
        id: "3",
        something_title: <Text.Body>{text("Title text3", "Title3")} </Text.Body>,
        something_body:
            <div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
            </div>
    },
    {
        id: "4",
        something_title: <Text.Body>{text("Title text3", "Title3")} </Text.Body>,
        something_body:
            <div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
            </div>
    }
];
/*
const item = [{
    id: "0",
    something_title: <Text.Body>{text("Title text", "Title1")} </Text.Body>,
    something_body:
        <div >
            <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
            <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
            <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
        </div>
}];
*/

storiesOf('Components|Tabs', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => {
        return (
            <BooleanValue>
                    <Tabs>
                        {something_items}
                    </Tabs>
            </BooleanValue>
        );
    });