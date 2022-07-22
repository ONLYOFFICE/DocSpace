import styled, { css } from "styled-components";
import Base from "../../../../components/themes/base";

const StyledFooter = styled.div`
  box-sizing: border-box;
  border-top: ${(props) => props.theme.advancedSelector.footerBorder};
  padding: 16px;
  height: 69px;

  display: flex;
  align-items: center;

  ${(props) =>
    props.withEmbeddedComponent &&
    css`
      display: flex;
    `}

  ${(props) =>
    !props.isVisible &&
    css`
      display: none;
    `}

  button {
    min-height: 40px;
  }

  .embedded_combo-box {
    .combo-button {
      min-height: 42px;
    }
  }
`;

StyledFooter.defaultProps = { theme: Base };

export default StyledFooter;
