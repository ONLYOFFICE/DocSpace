import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, text } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme'
import styled from '@emotion/styled';
import Readme from './README.md'
import { GroupButtonsMenu, DropDownItem } from 'asc-web-components'

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
        items.push({label:menuItemLabel});
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
        <GroupButtonsMenuContainer>
            <GroupButtonsMenu
                checked={false}                
                menuItems={menuItems}
                visible={true}
                moreLabel={text('moreLabel', 'More')}
                closeTitle={text('closeTitle', 'Close')}
            />
        </GroupButtonsMenuContainer>
        );
    });