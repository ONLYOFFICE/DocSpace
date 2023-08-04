import styled from "styled-components";

import Base from "../../../themes/base";

const StyledHeader = styled.div`
  width: calc(100% - 32px);
  min-height: 53px;
  height: 53px;
  max-height: 53px;

  padding: 0 16px;

  border-bottom: ${(props) => props.theme.selector.border};

  display: flex;
  align-items: center;

  .arrow-button {
    cursor: pointer;
    margin-right: 12px;
  }

  .heading-text {
    font-weight: 700;
    font-size: 21px;
    line-height: 28px;
  }
`;

StyledHeader.defaultProps = { theme: Base };

export default StyledHeader;
