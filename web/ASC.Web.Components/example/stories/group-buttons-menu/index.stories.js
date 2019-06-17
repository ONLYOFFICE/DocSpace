import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import styled from '@emotion/styled';
import Readme from './README.md'
import { GroupButtonsMenu, GroupButton } from 'asc-web-components'

const GroupButtonsMenuContainer = styled.div`
  height: 2000px;
`;

storiesOf('Components|GroupButtonsMenu', module)
    .addDecorator(withReadme(Readme))
    .add('empty', () => (
        <GroupButtonsMenuContainer>
            <GroupButtonsMenu />
        </GroupButtonsMenuContainer>
    ))
    .add('documents', () => (
        <GroupButtonsMenuContainer>
            <GroupButtonsMenu needCollapse>
                <GroupButton text='' isCheckbox isDropdown>
                    <GroupButton text='All'/>
                    <GroupButton text='Folders'/>
                    <GroupButton text='Documents'/>
                    <GroupButton text='Presentations'/>
                    <GroupButton text='Spreadsheets'/>
                    <GroupButton text='Images'/>
                    <GroupButton text='Media'/>
                    <GroupButton text='Archives'/>
                    <GroupButton text='All files'/>
                </GroupButton>
                <GroupButton text='Sharing Settings'/>
                <GroupButton text='Download' />
                <GroupButton text='Download as'/>
                <GroupButton text='Move to' hovered={false} />
                <GroupButton text='Copy'/>
                <GroupButton text='Delete'/>
            </GroupButtonsMenu>
        </GroupButtonsMenuContainer>
    ))
    .add('people', () => (
        <GroupButtonsMenuContainer>
            <GroupButtonsMenu needCollapse>
                <GroupButton text='' isCheckbox />
                <GroupButton text='Change type' isDropdown>
                    <GroupButton text='User'/>
                    <GroupButton text='Guest'/>
                </GroupButton>
                <GroupButton text='Change status' isDropdown>
                    <GroupButton text='Active'/>
                    <GroupButton text='Disabled'/>
                </GroupButton>
                <GroupButton text='Send activation link once again' />
                <GroupButton text='Write Letter'/>
                <GroupButton text='Delete'/>
            </GroupButtonsMenu>
        </GroupButtonsMenuContainer>
    ));