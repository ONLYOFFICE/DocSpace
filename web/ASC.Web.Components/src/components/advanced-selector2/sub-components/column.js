import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
    displayType,
    ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledContainer = styled(Container)`
    ${props => props.displayType === "dropdown" 
        ? css`
            ${props => props.size === "compact"  
                ? css`
                    width: 379px;
                    height: 267px;
                ` 
                : css`
                    width: 345px;
                    height: 544px;
                `
            }
        `
        : css`
            width: 325px;
            height: 100%;
        `
    }
`;

class ADSelectorColumn extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        const { children, displayType, className, style, size } = this.props;
        return (
            <StyledContainer displayType={displayType} className={className} style={style} size={size}>
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
    size: PropTypes.oneOf(["compact", "full"])
};

export default ADSelectorColumn;