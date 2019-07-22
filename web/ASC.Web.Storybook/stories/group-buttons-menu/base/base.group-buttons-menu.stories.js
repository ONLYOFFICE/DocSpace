import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, text } from '@storybook/addon-knobs/react';
import { BooleanValue } from 'react-values'
import withReadme from 'storybook-readme/with-readme'
import styled from '@emotion/styled';
import Readme from './README.md'
import { GroupButtonsMenu, DropDownItem, Button } from 'asc-web-components'

const GroupButtonsMenuContainer = styled.div`
  height: 2000px;
`;

const createItems = (label, dropDownLabel, menuItemLabel, count) => {
    var items =[
        {
            label: label,
            isDropdown: true,
            isSeparator: true,
            fontWeight: 'bold',
            children: [
                <DropDownItem key='1' label={dropDownLabel}/>,
                <DropDownItem key='2' label={dropDownLabel}/>,
                <DropDownItem key='3' label={dropDownLabel}/>
            ]
        }
    ];

    for (var i=0; i<count; i++){
        items.push({label:menuItemLabel, onClick: () => console.log('Click action')});
    }

    return items;
}

storiesOf('Components|GroupButtonsMenu', module)
    .addDecorator(withReadme(Readme))
    .addDecorator(withKnobs)
    .add('base', () => {

        const elements = 10;
        const selectLabel = 'Select';
        const dropLabel = 'Dropdown item';
        const menuItemLabel = 'Menu item';

        const menuItems = createItems(selectLabel, dropLabel, menuItemLabel, elements);

        return (
            <BooleanValue>
                {({ value: visible, toggle }) => (
                    <>
                        <Button
                            label="Show menu"
                            onClick={(e) => {
                                toggle(visible);
                            }}
                        />
                        <GroupButtonsMenuContainer>
                            <BooleanValue>
                                {({ value: checked, toggle }) => (

                                    <GroupButtonsMenu
                                        checked={checked}
                                        menuItems={menuItems}
                                        visible={visible}
                                        moreLabel={text('moreLabel', 'More')}
                                        closeTitle={text('closeTitle', 'Close')}
                                        onClose={() => console.log('Close action')}
                                        onChange={(e) => toggle(checked)}
                                    />
                                )}
                            </BooleanValue>

                        </GroupButtonsMenuContainer>
                    </>
                )}
            </BooleanValue>
        );
    });