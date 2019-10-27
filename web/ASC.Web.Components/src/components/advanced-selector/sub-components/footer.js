import React from "react";
import PropTypes from "prop-types";
import Button from "../../button";

const ADSelectorFooter = (props) => {
    const { buttonLabel, isDisabled, onButtonClick } = props;

    return (
        <div className="button_container">
            <Button
                className="add_members_btn"
                primary={true}
                size="big"
                label={buttonLabel}
                scale={true}
                isDisabled={isDisabled}
                onClick={onButtonClick}
            />
        </div>
    );
};

ADSelectorFooter.propTypes = {
    buttonLabel: PropTypes.string,
    isDisabled: PropTypes.bool,
    onButtonClick: PropTypes.func
}

export default ADSelectorFooter;