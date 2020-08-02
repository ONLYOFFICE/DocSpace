import React from 'react';
import styled from 'styled-components';
import { Button, Text, utils } from "asc-web-components";

const { tablet } = utils.device;

const StyledSaveSettingsButtons = styled.div`
    position:absolute;
    bottom:0;
    width:100%;
    display:flex;
    left: 0;
    padding: 8px 24px 8px 16px;
    justify-content: space-between;
    box-sizing: border-box;
    align-items: center;

    .save-button{
        margin-right: 8px;
    }

    .unsaved-changes{
        color:#A3A9AE;
    }

    @media ${tablet} {
        justify-content:flex-end;

        .unsaved-changes{
            display:none;
        }
    }
`;

class SaveSettingsButtons extends React.Component{
    constructor(props) {
        super(props);

        this.state = {

        }
    }

    render(){

        const { onSaveClick, onCancelClick, showReminder } = this.props
        return(
            <StyledSaveSettingsButtons>
                <div>
                    <Button 
                        className="save-button"
                        size="big"
                        isDisabled={false}
                        primary
                        onClick={onSaveClick}
                        label="Save"
                    />
                    <Button 
                        size="big"
                        isDisabled={false}
                        onClick={onCancelClick}
                        label="Cancel"
                    />
                </div>
                {showReminder && <Text className="unsaved-changes"> You have unsaved changes </Text>}
            </StyledSaveSettingsButtons>
        )
    }
}

export default SaveSettingsButtons