import React from "react";
import styled from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({
    displayType,
    ...props
}) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledBody = styled(Container)`
    width: 100%;
    height: 100%;
    `;

export default StyledBody;