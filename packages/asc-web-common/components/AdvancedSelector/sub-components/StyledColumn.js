import React from "react";
import styled, { css } from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({ ...props }) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledColumn = styled(Container)`
  width: 320px;
  height: 100%;
`;

export default StyledColumn;
