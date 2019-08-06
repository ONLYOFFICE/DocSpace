import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { Tabs, Text } from 'asc-web-components';
import Section from '../../.storybook/decorators/section';
import { BooleanValue } from 'react-values';

const something_items = [
    {
        id: "0",
        title: <Text.Body> Title1 </Text.Body>,
        content:
            <div >
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
            </div>
    },
    {
        id: "1",
        title: <Text.Body> Title2 </Text.Body>,
        content:
            <div >
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
            </div>
    },
    {
        id: "2",
        title: <Text.Body> Title3 </Text.Body>,
        content:
            <div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
            </div>
    },
    {
        id: "3",
        title: <Text.Body> Title4 </Text.Body>,
        content:
            <div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
            </div>
    },
    {
        id: "4",
        title: <Text.Body> Title5 </Text.Body>,
        content:
            <div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
            </div>
    }
];

storiesOf('Components|Tabs', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => {
        return (
            <Section>
                <BooleanValue>
                    <Tabs>
                        {something_items}
                    </Tabs>
                </BooleanValue>
            </Section>
        );
    });