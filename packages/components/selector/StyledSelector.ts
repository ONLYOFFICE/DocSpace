import styled from "styled-components";

import Base from "../themes/base";

const StyledSelector = styled.div`
  width: 100%;
  height: 100%;

  display: flex;
  flex-direction: column;

  overflow: hidden;
`;

StyledSelector.defaultProps = { theme: Base };

export { StyledSelector };
