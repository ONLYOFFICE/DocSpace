import React from 'react'
import { storiesOf } from '@storybook/react'
import withReadme from 'storybook-readme/with-readme'
import styled, { css } from '@emotion/styled';
import Readme from './README.md'
import { GroupButtonsMenu, GroupButton, DropDownItem } from 'asc-web-components'

const GroupButtonsMenuContainer = styled.div`
  height: 2000px;
`;

const Checkbox = styled.input`
    vertical-align: middle;
    margin-left: 24px;
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
            <Checkbox name="checkbox" type="checkbox" />
                <GroupButton label='Select' isDropdown isSeparator fontWeight='bold'>
                    <DropDownItem label='All'/>
                    <DropDownItem label='Files'/>
                    <DropDownItem label='Folders'/>
                    <DropDownItem label='Documents'/>
                    <DropDownItem label='Presentations'/>
                    <DropDownItem label='Images'/>
                    <DropDownItem label='Archives'/>
                </GroupButton>
                <GroupButton label='Share'/>
                <GroupButton label='Download' />
                <GroupButton label='Download as'/>
                <GroupButton label='Move'/>
                <GroupButton label='Copy'/>
                <GroupButton label='Delete'/>
            </GroupButtonsMenu>
        </GroupButtonsMenuContainer>
    ))
    .add('people', () => (
        <GroupButtonsMenuContainer>
            <GroupButtonsMenu needCollapse>
                <Checkbox name="checkbox" type="checkbox" />
                <GroupButton label='Select' isDropdown isSeparator fontWeight='bold'>
                    <DropDownItem label='Active'/>
                    <DropDownItem label='Disabled'/>
                    <DropDownItem label='Invited'/>
                </GroupButton>
                <GroupButton label='Make employee' />
                <GroupButton label='Make guest' />
                <GroupButton label='Set active' />
                <GroupButton label='Set disabled' />
                <GroupButton label='Invite again' />
                <GroupButton label='Send e-mail' />
                <GroupButton label='Delete' />
            </GroupButtonsMenu>
        </GroupButtonsMenuContainer>
    ));