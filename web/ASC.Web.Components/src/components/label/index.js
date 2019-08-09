import React from "react";
import PropTypes from "prop-types";
import { Text } from '../text';

const Label = (props) => {
    const { isRequired, error, title, truncate, isInline, htmlFor} = props;
    const errorProp = error ? {color: "#c30"} : {}
    const displayProp = isInline ? {} : {display : 'block'}

    console.log("Label render");
    return (
        <Text.Body as='label' htmlFor={htmlFor}  style={displayProp} {...errorProp} fontWeight={600} truncate={truncate} title={title}>
            {props.children} {isRequired && " *"}
        </Text.Body>
    );
}

Label.propTypes = {
    isRequired: PropTypes.bool,
    error: PropTypes.bool,
    isInline: PropTypes.bool,
    title: PropTypes.string,
    truncate: PropTypes.bool,
    htmlFor: PropTypes.string
};

Label.defaultProps = {
    isRequired: false,
    error: false,
    isInline: true,
    truncate: false
};

export default Label;