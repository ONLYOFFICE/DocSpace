import React from "react";
import styled, { css } from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
    displayType,
    ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledColumn = styled(Container)`
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

export default StyledColumn;