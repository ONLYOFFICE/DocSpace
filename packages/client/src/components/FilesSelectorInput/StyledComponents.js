import styled, { css } from "styled-components";

const StyledBodyWrapper = styled.div`
  margin: 16px 0;
  max-width: ${(props) => (props.maxWidth ? props.maxWidth : "350px")};
`;

export { StyledBodyWrapper };
