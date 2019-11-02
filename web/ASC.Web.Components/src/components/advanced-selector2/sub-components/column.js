import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
    displayType,
    ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledContainer = styled(Container)`
    width: ${props => props.displayType === "dropdown" ? 345 : 325}px;
    height: ${props => props.displayType === "dropdown" ? "544px" : "100%"};
    `;

class ADSelectorColumn extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        const { children, displayType, className, style } = this.props;
        return (
            <StyledContainer displayType={displayType} className={className} style={style}>
                {children}
            </StyledContainer>
        );
    }
}

ADSelectorColumn.propTypes = {
    children: PropTypes.any,
    className: PropTypes.string,
    style: PropTypes.object,
    displayType: PropTypes.oneOf(["dropdown", "aside"]),
};

export default ADSelectorColumn;