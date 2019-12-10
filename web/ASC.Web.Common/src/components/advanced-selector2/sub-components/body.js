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
    width: 100%;
    height: 100%;
    `;

class ADSelectorBody extends React.Component {
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

ADSelectorBody.propTypes = {
    children: PropTypes.any,
    className: PropTypes.string,
    style: PropTypes.object,
    displayType: PropTypes.oneOf(["dropdown", "aside"]),
};

export default ADSelectorBody;