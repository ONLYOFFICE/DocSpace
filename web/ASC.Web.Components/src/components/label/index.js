import React from "react";
import PropTypes from "prop-types";
import { Text } from '../text';
import IconButton from '../icon-button';
import styled from 'styled-components';

const IconStyle = styled.span`
    position: absolute;
    margin: 2px 0 0 5px;
`;

const Label = (props) => {
    const { isRequired, error, title, truncate, isInline, htmlFor, text, display, className, tooltipId, tooltipEvent, iconButton} = props;
    const errorProp = error ? {color: "#c30"} : {}

    
    //console.log("Label render");
    return (

            <Text.Body as='label' htmlFor={htmlFor} isInline={isInline} display={display} {...errorProp} fontWeight={600} truncate={truncate} title={title} className={className}>
                {text} {isRequired && " *"} 
                <>
                {tooltipId && 
                <IconStyle  
                    data-for={tooltipId}
                    data-tip
                    data-event={tooltipEvent}
                    data-place="top"
                >
                    <IconButton className="icon-button" isClickable={true} iconName={iconButton} size={13} />
                </IconStyle>}
                </>
            </Text.Body>
    );
}

Label.propTypes = {
    isRequired: PropTypes.bool,
    error: PropTypes.bool,
    isInline: PropTypes.bool,
    title: PropTypes.string,
    truncate: PropTypes.bool,
    htmlFor: PropTypes.string,
    text: PropTypes.string,
    display: PropTypes.string,
    className: PropTypes.string,
    tooltipId: PropTypes.string,
    iconButton: PropTypes.string,
    tooltipEvent: PropTypes.string
};

Label.defaultProps = {
    isRequired: false,
    error: false,
    isInline: false,
    truncate: false,
    iconButton: "QuestionIcon"
};

export default Label;