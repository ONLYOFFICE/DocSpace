import React from 'react';
import { storiesOf } from '@storybook/react';
import { withKnobs, boolean, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import { TabContainer } from 'asc-web-components';
import Section from '../../.storybook/decorators/section';
import { action } from '@storybook/addon-actions';

const array_items = [
    {
        key: "tab0",
        title: "Title1",
        content:
            <div >
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
            </div>
    },
    {
        key: "tab1",
        title: "Title2",
        content:
            <div >
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
            </div>
    },
    {
        key: "tab2",
        title: "Title3",
        content:
            <div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
                <div> <input></input> <input></input> <input></input> </div>
            </div>
    },
    {
        key: "tab3",
        title: "Title4",
        content:
            <div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> <button>BUTTON</button> <button>BUTTON</button> </div>
            </div>
    },
    {
        key: "tab4",
        title: "Title5",
        content:
            <div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
                <div> <label>LABEL</label> <label>LABEL</label> <label>LABEL</label> </div>
            </div>
    }
];

storiesOf('Components|TabContainer', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => {
        return (
            <Section>
                <TabContainer
                    onSelect={index => action("Selected item")(index)}
                    isDisabled={boolean('isDisabled', false)}
                    selectedItem={array_items.indexOf(array_items[0])}
                >
                    {array_items}
                </TabContainer>
            </Section>
        );
    });