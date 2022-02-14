import React from "react";
import styled from "styled-components";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({ ...props }) => <div {...props} />;
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledHeader = styled(Container)`
  /*height: 64px;*/
`;

export default StyledHeader;
