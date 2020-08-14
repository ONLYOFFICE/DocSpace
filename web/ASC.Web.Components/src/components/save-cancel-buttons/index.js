import React from 'react';
import styled from 'styled-components';
import PropTypes from "prop-types";
import Button from "../button";
import Text from "../text";
import { tablet } from '../../utils/device'

const StyledSaveCancelButtons = styled.div`
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

class SaveCancelButtons extends React.Component {

    render() {
        const { onSaveClick, onCancelClick, showReminder, reminderTest, saveButtonLabel, cancelButtonLabel, className, id } = this.props
        return (
            <StyledSaveCancelButtons className={className} id={id}>
                <div>
                    <Button
                        className="save-button"
                        size="big"
                        isDisabled={false}
                        primary
                        onClick={onSaveClick}
                        label={saveButtonLabel}
                    />
                    <Button
                        size="big"
                        isDisabled={false}
                        onClick={onCancelClick}
                        label={cancelButtonLabel}
                    />
                </div>
                {showReminder && <Text className="unsaved-changes"> {reminderTest} </Text>}
            </StyledSaveCancelButtons>
        )
    }
}

SaveCancelButtons.propTypes = {
    id: PropTypes.string,
    className: PropTypes.string,
    reminderTest: PropTypes.string,
    saveButtonLabel: PropTypes.string,
    cancelButtonLabel: PropTypes.string,
    onSaveClick: PropTypes.func,
    onCancelClick: PropTypes.func,
    showReminder: PropTypes.bool
};


SaveCancelButtons.defaultProps = {
    saveButtonLabel: "Save",
    cancelButtonLabel: "Cancel"
}

export default SaveCancelButtons