import React from 'react'
import { storiesOf } from '@storybook/react'
import { withKnobs, text} from '@storybook/addon-knobs/react';
import { BooleanValue } from 'react-values'
import styled from '@emotion/styled';
import { GroupButtonsMenu, Checkbox, DropDownItem } from 'asc-web-components'

const GroupButtonsMenuContainer = styled.div`
  height: 2000px;
`;

const peopleItems = [
    {
        label: 'Select',
        isDropdown: true,
        isSeparator: true,
        fontWeight: 'bold',
        children: [
            <DropDownItem key='active' label='Active'/>,
            <DropDownItem key='disabled' label='Disabled'/>,
            <DropDownItem key='invited' label='Invited'/>
        ]
    },
    {
        label: 'Make employee',
        onClick: () => console.log('Make employee action')
    },
    {
        label: 'Make guest',
        onClick: () => console.log('Make guest action')
    },
    {
        label: 'Set active',
        onClick: () => console.log('Set active action')
    },
    {
        label: 'Set disabled',
        onClick: () => console.log('Set disabled action')
    },
    {
        label: 'Invite again',
        onClick: () => console.log('Invite again action')
    },
    {
        label: 'Send e-mail',
        onClick: () => console.log('Send e-mail action')
    },
    {
        label: 'Delete',
        onClick: () => console.log('Delete action')
    }
];

storiesOf('EXAMPLES|GroupButtonsMenu', module)
    .addDecorator(withKnobs)
    .add('people', () => (
        <GroupButtonsMenuContainer>
            <GroupButtonsMenu checkBox={
                <BooleanValue>
                    {({ value, toggle }) => (
                        <Checkbox isChecked={value} 
                                onChange={e => {
                                    console.log(e.target.value);
                                    toggle(e.target.checked);
                                }}
                                isDisabled={false}
                                value='Checkbox'
                                id='check1' />)}
                </BooleanValue>}
                menuItems={peopleItems}
                visible={true}
                moreLabel={text('moreLabel', 'More')}
                closeTitle={text('closeTitle', 'Close')}
            />
        </GroupButtonsMenuContainer>
    ));